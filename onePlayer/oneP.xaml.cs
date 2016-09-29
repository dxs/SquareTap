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
		List<Home> list = null;
		public string MyScore { get; set; }
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
			myScore.score = 20;
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
				Interval = new TimeSpan(0, 0, 0, 0, 1000)
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

			while (true)
			{
				item = Rand.GetInteger(nbCase);
				if (list[item].Rect.Fill == SvenColors.trans && list[item].Locked == false)
					break;
			}
			switch (Rand.GetInteger(3))
			{
				case 0:
				case 1:
					list[item].Rect.Fill = SvenColors.redImage;
					break;
				case 2:
					list[item].Rect.Fill = SvenColors.orangeImage;
					break;
			}

			list[item].SetIntervalRun(new TimeSpan(0, 0, 0, 0, Rand.GetInteger(500, 3000)));
			list[item].RunTimer.Start();


			party.Interval = new TimeSpan(0, 0, 0, 0, Rand.GetInteger(5,500));
		}

		private void InitPolygon()
		{
			if (list == null)
				list = new List<Home>();
			else
				list.Clear();
			for (int i = 0; i < nCaseH; i++)
			{
				for (int j = 0; j < nCaseW; j++)
				{
					list.Add(new Home(width, height));
					list.Last().Rect.Tapped += OneP_Tapped;
					list.Last().Rect.ManipulationMode = ManipulationModes.All;
					list.Last().Rect.ManipulationDelta -= Item_ManipulationDelta;
					list.Last().Rect.ManipulationCompleted -= Item_ManipulationCompleted;
					Canvas.SetLeft(list.Last().Rect, j * (width + space) + startWidth);
					Canvas.SetTop(list.Last().Rect, i * (height + space) + startHeight);
					Canvas.SetZIndex(list.Last().Rect, i*j + j);
					Board.Children.Add(list.Last().Rect);
				}
			}
		}

		private void Item_ManipulationCompleted(object sender, ManipulationCompletedRoutedEventArgs e)
		{
			
		}

		private void Item_ManipulationDelta(object sender, ManipulationDeltaRoutedEventArgs e)
		{
			TranslateTransform a = (sender as Rectangle).RenderTransform as TranslateTransform;
			a.X += e.Delta.Translation.X;
			a.Y += e.Delta.Translation.Y;
		}

		private void OneP_Tapped(object sender, TappedRoutedEventArgs e)
		{
			if ((sender as Rectangle).Fill != SvenColors.trans)
			{
				foreach (Home item in list)
				{
					if (item.Rect == (sender as Rectangle))
					{
						if (item.Rect.Fill == SvenColors.redImage)
						{
							MyScore = (--myScore.score).ToString();
							item.RunTimer.Stop();
							item.Lock();
						}
						else
						{
							myScore.score += 2;
							MyScore = (myScore).score.ToString();
						}
						item.Rect.Fill = SvenColors.trans;
						break;
					}
				}
			}
			else
			{
				myScore.score += 4;
				MyScore = (myScore).score.ToString();
			}
			this.Bindings.Update();
			if(myScore.score <= 0)
			{
				party.Stop();
				foreach (Home item in list)
				{
					item.Rect.Fill = SvenColors.trans;
					item.Rect.ManipulationCompleted += Item_ManipulationCompleted;
					item.Rect.ManipulationDelta += Item_ManipulationDelta;
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
			foreach (Home item in list)
			{
				iteration = 0;
				Point actual = item.Rect.TransformToVisual(Window.Current.Content).TransformPoint(new Point(0, 0));
				while (Math.Round(actual.X) != Math.Round(ThrowPosition.X) || Math.Round(actual.Y) != Math.Round(ThrowPosition.Y))
				{
					actual = item.Rect.TransformToVisual(Window.Current.Content).TransformPoint(new Point(0, 0));

					(item.Rect.RenderTransform as TranslateTransform).X = (ThrowPosition.X - actual.X);
					(item.Rect.RenderTransform as TranslateTransform).Y = (ThrowPosition.Y - actual.Y);
					await Task.Delay(5);
					if(iteration++ > 50)
						break;
					actual = item.Rect.TransformToVisual(Window.Current.Content).TransformPoint(new Point(0, 0));
				}
				GiveColorToItem();
			}
		}

		private void GiveColorToItem()
		{
			foreach (Home item in list)
				item.Rect.Fill = SvenColors.GetRandomColor();
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
