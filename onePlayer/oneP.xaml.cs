using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage.Streams;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Shapes;

// Pour plus d'informations sur le modèle d'élément Page vierge, voir la page http://go.microsoft.com/fwlink/?LinkId=234238

namespace SquareTap.onePlayer
{
	/// <summary>
	/// Une page vide peut être utilisée seule ou constituer une page de destination au sein d'un frame.
	/// </summary>
	public sealed partial class oneP : Page
	{
		List<Rectangle> list = null;
		public string MyScore { get; set; }
		private SolidColorBrush trans = new SolidColorBrush(Colors.Transparent);
		private double width = 80;
		private double height = 80;
		private double nCaseW = 0;
		private double nCaseH = 0;
		private double space = 5;
		private double startWidth = 0;
		private double startHeight = 0;
		private int nbCase = 0;
		DispatcherTimer starter;
		DispatcherTimer party;
		private const double coefAugmentation = 1.1;
		private ImageBrush redImage, orangeImage;
		private Score myScore;

		private Point ThrowPosition;

		public oneP()
		{
			this.InitializeComponent();
			myScore = new Score("Sven");
			myScore.score = 20;
			redImage = new ImageBrush() { ImageSource = new BitmapImage()
			{ UriSource = new Uri("ms-appx:///Images/RedWhite.jpg") } };
			orangeImage = new ImageBrush() { ImageSource = new BitmapImage()
			{ UriSource = new Uri("ms-appx:///Images/OrangeWhite.jpg") } };
			this.Loaded += OneP_Loaded;
		}

		private void OneP_Loaded(object sender, RoutedEventArgs e)
		{
			GetCanvasSize();
			InitStarters();
			starter.Start();
		}

		private void InitStarters()
		{
			party = new DispatcherTimer()
			{
				Interval = new TimeSpan(0, 0, 0,0, 1000)
			};
			party.Tick += Party_Tick;

			starter = new DispatcherTimer()
			{
				Interval = new TimeSpan(0, 0, 1)
			};
			starter.Tick += (t, args) =>
			{
				centerText.Visibility = Visibility.Visible;
				party.Stop();

				if (centerText.Text == "1")
				{
					centerText.Text = "Ready?";
					centerText.Visibility = Visibility.Collapsed;
					(t as DispatcherTimer).Stop();
					InitPolygon();
					party.Start();
				}
				if (centerText.Text == "2")
					centerText.Text = "1";
				if (centerText.Text == "3")
					centerText.Text = "2";
				if (centerText.Text.Contains("R"))
					centerText.Text = "3";
			};
		}

		private void Party_Tick(object sender, object e)
		{
			nbCase = list.Count;
			if (nbCase == 0)
				return;
			int item = 0;

			for (int i = 0; i < new Random().Next(1, 6); i++)
			{
				while (true)
				{
					item = new Random().Next(nbCase);
					if (list[item].Fill == redImage)
						break;
				}
				switch(new Random().Next(3))
				{
					case 0:
					case 1:
						list[item].Fill = redImage;
						break;
					case 2:
						list[item].Fill = orangeImage;
						break;
				}

				DispatcherTimer shutDownTimer = new DispatcherTimer() { Interval = new TimeSpan(0, 0, 0, 0, new Random().Next(100, 2500))}; //use a tag or smth to detect
				shutDownTimer.Tick += (t, args) =>
				{
					foreach (Rectangle k in list)
					{; }
				};
			}

			
			party.Interval = new TimeSpan(0, 0, 0, 0, new Random().Next(100,700));
		}

		private void InitPolygon()
		{
			if (list == null)
				list = new List<Rectangle>();
			else
				list.Clear();
			for (int i = 0; i < nCaseH; i++)
			{
				for (int j = 0; j < nCaseW; j++)
				{
					list.Add(new Rectangle()
					{
						Width = width,
						Height = height,
						Fill = trans,
						Stroke = new SolidColorBrush(Colors.Gray),
						RenderTransform = new TranslateTransform(),
					});
					list.Last().Tapped += OneP_Tapped;
					list.Last().ManipulationMode = ManipulationModes.All;
					list.Last().ManipulationDelta -= Item_ManipulationDelta;
					list.Last().ManipulationCompleted -= Item_ManipulationCompleted;
					Canvas.SetLeft(list.Last(), j * (width + space) + startWidth);
					Canvas.SetTop(list.Last(), i * (height + space) + startHeight);
					Canvas.SetZIndex(list.Last(), i*j + j);
					Board.Children.Add(list.Last());
				}
			}
		}

		private void Item_ManipulationCompleted(object sender, ManipulationCompletedRoutedEventArgs e)
		{
			throw new NotImplementedException();
		}

		private void Item_ManipulationDelta(object sender, ManipulationDeltaRoutedEventArgs e)
		{
			throw new NotImplementedException();
		}

		private void OneP_Tapped(object sender, TappedRoutedEventArgs e)
		{
			if ((sender as Rectangle).Fill != trans)
			{
				foreach (Rectangle item in list)
				{
					if (item == (sender as Rectangle))
					{
						if(item.Fill == redImage)
							MyScore = (--myScore.score).ToString();
						else
						{
							myScore.score += 5;
							MyScore = (myScore).score.ToString();
						}
						item.Fill = trans;

						break;
					}
				}
			}
			else
			{
				myScore.score += 10;
				MyScore = (myScore).score.ToString();
			}
			this.Bindings.Update();
			if(myScore.score <= 0)
			{
				party.Stop();
				foreach (Rectangle item in list)
				{
					item.Fill = trans;
					item.ManipulationCompleted += Item_ManipulationCompleted;
					item.ManipulationDelta += Item_ManipulationDelta;
				}
				StartMoving();
			}
		}

		private async void StartMoving()
		{
			scorePanel.Visibility = Visibility.Collapsed;
			ThrowPosition = new Point();
			ThrowPosition.X = (Board.ActualWidth / 2) - width / 2;
			ThrowPosition.Y = (Board.ActualHeight) - 3 / 2 * height;
			int iteration = 0;
			foreach (Rectangle item in list)
			{
				iteration = 0;
				Point actual = item.TransformToVisual(Window.Current.Content).TransformPoint(new Point(0, 0));
				while (Math.Round(actual.X) != Math.Round(ThrowPosition.X) || Math.Round(actual.Y) != Math.Round(ThrowPosition.Y))
				{
					(item.RenderTransform as TranslateTransform).X = (ThrowPosition.X - actual.X);
					(item.RenderTransform as TranslateTransform).Y = (ThrowPosition.Y - actual.Y);
					actual = item.TransformToVisual(Window.Current.Content).TransformPoint(new Point(0, 0));
					await Task.Delay(5);
					if(iteration++ > 50)
						break;
				}
				Debug.WriteLine("X : {0}", actual.X);
			}
		}

		private void GetCanvasSize()
		{
			double _w = Board.ActualWidth;
			double _h = Board.ActualHeight;
			nCaseW = (int)(_w / (width + space));
			nCaseH = (int)(_h / (height + space));
			startWidth = (_w - nCaseW * (width+space)) / 2;
			startHeight = (_h - nCaseH * height) / 2;
			if (startHeight < 0)
				startHeight = 0;
			if (startWidth < 0)
				startWidth = 0;
		}
	}
}
