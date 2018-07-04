using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZombiesAutosplitter
{
    class Point
    {
        public int X;
        public int Y;
        public bool ShouldMatch;

        public Point(int x, int y, bool shouldMatch = true)
        {
            this.X = x;
            this.Y = y;
            this.ShouldMatch = shouldMatch;
        }
    }
}
