using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquareTap
{
	class Score
	{
		public string Name { get; set; }
		public int score { get; set; }

		public Score(string _name)
		{
			Name = _name;
			score = 0;
		}
	}
}
