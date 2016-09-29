using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace SquareTap
{
	static class SvenColors
	{
		public static SolidColorBrush trans = new SolidColorBrush(Colors.Transparent);
		public static SolidColorBrush gray = new SolidColorBrush(Colors.Gray);
		public static SolidColorBrush Red = new SolidColorBrush(Colors.Red);
		public static SolidColorBrush Orange = new SolidColorBrush(Colors.OrangeRed);
		public static SolidColorBrush Blue = new SolidColorBrush(Colors.MediumSlateBlue);
		public static SolidColorBrush Violet = new SolidColorBrush(Colors.BlueViolet);
		public static SolidColorBrush Green = new SolidColorBrush(Colors.LightGreen);
		public static SolidColorBrush Yellow = new SolidColorBrush(Colors.LightGoldenrodYellow);

		public static List<SolidColorBrush> ListColors = new List<SolidColorBrush>()
		{
			trans,
			gray,
			Red,
			Orange,
			Blue,
			Violet,
			Green,
			Yellow
		};

		public static ImageBrush redImage = new ImageBrush()
		{
			ImageSource = new BitmapImage()
			{ UriSource = new Uri("ms-appx:///Images/RedWhite.jpg") }
		};
		public static ImageBrush orangeImage = new ImageBrush()
		{
			ImageSource = new BitmapImage()
			{ UriSource = new Uri("ms-appx:///Images/OrangeWhite.jpg") }
		};


		public static SolidColorBrush GetRandomColor()
		{
			return ListColors[Rand.GetInteger(ListColors.Count)];
		}
	}
}
