using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BO1ZombiesAutosplitter
{
    public static class PointGenerator
    {
        public static List<point> GenerateResetPoints()
        {
            Color colorToFind = Color.FromArgb(214, 214, 214);

            List<point> points = new List<point>();

            if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "good_reset.png"))
            {
                Bitmap bmp = (Bitmap)Bitmap.FromFile(AppDomain.CurrentDomain.BaseDirectory + "good_reset.png");

                for (int x= 0; x < bmp.Width; x += 4)
                {
                    for (int y = 0; y < bmp.Height; y += 4)
                    {
                        var clr = bmp.GetPixel(x,y);

                        bool matches = Utils.ColorsAreClose(clr, colorToFind, 25);

                        points.Add(new point(x, y, matches));
                    }
                }

                bmp.Dispose();
            }

            return points;
        }
    }
}
