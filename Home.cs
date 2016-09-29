using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Shapes;

namespace SquareTap
{
	class Home
	{
		public DispatcherTimer LockTimer { get; set; }
		public DispatcherTimer RunTimer { get; set; }
		private double caseWidth = 0;
		public double CaseWidth { get { return caseWidth; } }
		private double caseHeight = 0;
		public double CaseHeight { get { return caseHeight; } }
		public Rectangle Rect { get; set; } 
		public bool Locked { get; set; }
		public Home(double _w, double _h)
		{
			Locked = false;

			caseWidth = _w;
			caseHeight = _h;

			LockTimer = new DispatcherTimer()
				{ Interval = new TimeSpan(0, 0, 1) };
			RunTimer = new DispatcherTimer()
				{ Interval = new TimeSpan(0, 0, 1) };

			Rect = new Rectangle()
			{
				Width = caseWidth,
				Height = caseHeight,
				Fill = SvenColors.trans,
				Stroke = SvenColors.gray,
				RenderTransform = new TranslateTransform()
			};

			LockTimer.Tick += LockTimer_Tick;
			RunTimer.Tick += RunTimer_Tick;
		}

		public void SetIntervalLock(TimeSpan t)
		{
			LockTimer.Interval = t;
		}

		public void SetIntervalRun(TimeSpan t)
		{
			RunTimer.Interval = t;
		}

		private void RunTimer_Tick(object sender, object e)
		{
			(sender as DispatcherTimer).Stop();
			this.Rect.Fill = SvenColors.trans;
		}

		public void Lock()
		{
			Locked = true;
			LockTimer.Start();
		}

		private void LockTimer_Tick(object sender, object e)
		{
			Locked = false;
			(sender as DispatcherTimer).Stop();
		}
	}
}
