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
        public static List<point> GenerateResetPoints(string dir, Resolution res)
        {
            string path = Path.Combine(dir, "reset_" + res.window_width + "x" + res.window_height + ".png");

            Color colorToFind = Color.FromArgb(214, 214, 214);

            List<point> points = new List<point>();

            if (File.Exists(path))
            {
                Bitmap bmp = (Bitmap)Bitmap.FromFile(path);

                for (int x= 0; x < bmp.Width; x += 2)
                {
                    for (int y = 0; y < bmp.Height; y += 2)
                    {
                        var clr = bmp.GetPixel(x,y);

                        bool matches = Utils.ColorsAreClose(clr, colorToFind, 45);

                        points.Add(new point(x, y, matches));
                    }
                }

                bmp.Dispose();
            }

            return points;
        }

        internal static List<point> GeterateLevelPoints(string data_dir, Resolution res, int currentLevel)
        {
            Color colorToFind = Color.FromArgb(54, 0, 0);
            colorToFind = Color.FromArgb(214, 214, 214);
            List<point> points = new List<point>();

            string file_path = Path.Combine(data_dir, "level_" + res.window_width + "x" + res.window_height + ".png");

            if (File.Exists(file_path))
            {
                Bitmap bmp = (Bitmap)Bitmap.FromFile(file_path);

                for (int x = 0; x < bmp.Width; x += 2)
                {
                    for (int y = 0; y < bmp.Height; y += 2)
                    {
                        var clr = bmp.GetPixel(x, y);

                        bool matches = Utils.ColorsAreClose(clr, colorToFind, 45);

                        points.Add(new point(x, y, matches));
                    }
                }

                bmp.Dispose();

                return points;
            }

            return new List<point>();
        }
    }
}
