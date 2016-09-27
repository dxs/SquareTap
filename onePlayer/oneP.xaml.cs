using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
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
		private Score myScore;

		private Point ThrowPosition;

		public oneP()
		{
			this.InitializeComponent();
			myScore = new Score("Sven");
			myScore.score = 0;
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
			int iteration = 0;
			while (true)
			{
				item = new Random().Next(nbCase);
				if (list[item].Fill == new SolidColorBrush(Colors.AliceBlue))
					break;
				if (iteration++ > 10)
					break;
			}

			list[item].Fill = new SolidColorBrush(Colors.Bisque);
			
			party.Interval = new TimeSpan(0, 0, 0, 0, new Random().Next(100,1000));
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
						RenderTransform = new TranslateTransform()
					});
					list.Last().Tapped += OneP_Tapped;
					list.Last().ManipulationMode = ManipulationModes.All;
					Canvas.SetLeft(list.Last(), j * (width + space) + startWidth);
					Canvas.SetTop(list.Last(), i * (height + space) + startHeight);
					Board.Children.Add(list.Last());
				}
			}
		}

		private void OneP_Tapped(object sender, TappedRoutedEventArgs e)
		{
			if ((sender as Rectangle).Fill != trans)
			{
				foreach (Rectangle item in list)
				{
					if (item == (sender as Rectangle))
					{
						item.Fill = trans;
						MyScore = (--myScore.score).ToString();
						break;
					}
				}
			}
			else
			{
				myScore.score += 10;
				MyScore = (myScore).ToString();
			}
			this.Bindings.Update();
			if(myScore.score <= 0)
			{
				foreach (Rectangle item in list)
					item.Fill = trans;
				StartMoving();
			}
		}

		private void StartMoving()
		{
			scorePanel.Visibility = Visibility.Collapsed;
			ThrowPosition = new Point();
			ThrowPosition.X = (Board.ActualWidth / 2) - width / 2;
			ThrowPosition.Y = (Board.ActualHeight) - 3 / 2 * height;
			bool done = false;
			while (!done)
			{
				foreach (Rectangle item in list)
				{
					done = false;
					Point actual = item.TransformToVisual(Window.Current.Content).TransformPoint(new Point(0, 0));
					if (Math.Round(actual.X) != Math.Round(ThrowPosition.X) || Math.Round(actual.Y) != Math.Round(ThrowPosition.Y))
					{
						(item.RenderTransform as TranslateTransform).X = ThrowPosition.X - actual.X;
						(item.RenderTransform as TranslateTransform).Y = ThrowPosition.Y - actual.Y;
						done = false;
						break;
					}
					else
						done = true;
				}
			}
			DebugSettings.writ
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
