using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquareTap
{
	static class Rand
	{
		static Random R = new Random();

		public static int GetInteger()
		{
			return R.Next();
		}

		public static int GetInteger(int max)
		{
			return R.Next(max);
		}

		/// <summary>
		/// Determine un nombre aléatoire entre deux nombres
		/// </summary>
		/// <param name="min">Minimum (inclusif)</param>
		/// <param name="max">Maximum (exclusif)</param>
		/// <returns></returns>
		public static int GetInteger(int min, int max)
		{
			return R.Next(min, max);
		}

		public static double GetDouble()
		{
			return R.NextDouble();
		}
	}
}
