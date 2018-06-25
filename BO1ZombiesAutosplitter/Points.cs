using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BO1ZombiesAutosplitter
{
    public static class Points
    {
        public static List<resolution> InitializePoints()
        {
            if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "data.json"))
            {
               return JsonConvert.DeserializeObject<resolution[]>(File.ReadAllText(AppDomain.CurrentDomain.BaseDirectory + "data.json")).ToList();
            }

            //r.level_rec = new System.Drawing.Rectangle(0, 809, 250, 120);
            //r.reset_rec = new System.Drawing.Rectangle(723, 341, 160, 60);

            return new List<resolution>();

            //return new resolution[]
            //{
            //    R_1600x900(),
            //    R_1600x1024()
            //}.ToList();
        }

        private static resolution R_1600x1024()
        {
            resolution r = new resolution();
            r.window_width = 1606;
            r.window_height = 1053;

            r.levels = new List<level> {
                new level()
                {
                    lvl = 70,
                    pointsToMatch = new point[]
                    {
                        // number seven
                       new point(7,4),
                       new point(20,4),
                       new point(41,4),
                       new point(40,11),
                       new point(36,17),
                       new point(29,29),
                       new point(25,38),
                       new point(21,47),
                       new point(17,62),
                       new point(16,77),

                         // number zero
                        new point(64,42),
                        new point(100,10),
                        new point(84,2),
                        new point(68,36),
                        new point(103,31),
                        new point(104,45),
                        new point(97,56),
                        new point(101,65),
                        new point(94,74),
                        new point(70,70),
                }
            },

                new level()
                {
                    lvl = 50,
                    pointsToMatch = new point[]
                    {
                        // number five
                       new point(41,3),
                       new point(10,4),
                       new point(26,3),
                       new point(8,13),
                       new point(8,26),
                       new point(8,36),
                       new point(27,29),
                       new point(37,34),
                       new point(42,42),
                       new point(43,53),
                       new point(40,65),

                       new point(34,72),
                       new point(25,75),
                       new point(11,72),
                       new point(7,64),

                        // number zero
                        new point(64,42),
                        new point(100,10),
                        new point(84,2),
                        new point(68,36),
                        new point(103,31),
                        new point(104,45),
                        new point(97,56),
                        new point(101,65),
                        new point(94,74),
                        new point(70,70),
                }
            },

                new level()
                {
                    lvl = 30,
                    pointsToMatch = new point[]
                    {
                        // number three
                        new point(8,14),
                        new point(14,3),
                        new point(40,10),
                        new point(37,28),
                        new point(22,36),
                        new point(37,42),
                        new point(43,55),
                        new point(40,69),
                        new point(27,74),
                        new point(12,73),
                        new point(7,64),

                        // number zero
                        new point(64,42),
                        new point(100,10),
                        new point(84,2),
                        new point(68,36),
                        new point(103,31),
                        new point(104,45),
                        new point(97,56),
                        new point(101,65),
                        new point(94,74),
                        new point(70,70),
                    }
            },

                new level()
                {
                    lvl = 15,
                    pointsToMatch = new point[]
                    {
                        // number one
                        new point(30,2),
                        new point(7,14),
                        new point(28,76),

                        // number five
                        new point(100,3),
                        new point(84,76),
                        new point(103,53),
                        new point(86,28),
                        new point(68,36),
                    }
            },

                new level()
                {
                    lvl = 10,
                    pointsToMatch = new point[]
                {
                        new point(98,22),
                        new point(9,64),
                        new point(145,60),
                        new point(169,51),
                        new point(192,41),
                        new point(225,22),
                        new point(135,64),

                        new point(106,85, false),
                        new point(76,17, false),
                        new point(106,85, false),
                        new point(28,79, false),
                        new point(28,44, false),
                        new point(50,12, false),
                        new point(75,39, false),
                        new point(80,84, false),
                        new point(101,12, false),
                        new point(237,62, false),
                    }
            },

                new level()
                {
                    lvl = 5,
                    pointsToMatch = new point[]
                    {
                        new point(98,22),
                        new point(9,64),
                        new point(145,60, false),
                        new point(169,51, false),
                        new point(192,41, false),

                        new point(106,85, false),
                        new point(76,17, false),
                        new point(106,85, false),
                        new point(28,79, false),
                        new point(28,44, false),
                        new point(50,12, false),
                        new point(75,39, false),
                        new point(80,84, false),
                        new point(101,12, false),
                    }
                },

                new level()
                {
                    lvl = 4,
                    pointsToMatch = new point[]
                    {
                        new point(90,19),
                        new point(93,87),
                        new point(98,22, false),
                        new point(9,64, false),

                        new point(106,85, false),
                        new point(76,17, false),
                        new point(106,85, false),
                        new point(28,79, false),
                        new point(28,44, false),
                        new point(50,12, false),
                        new point(75,39, false),
                        new point(80,84, false),
                        new point(101,12, false),
                    }
                },

                new level()
                {
                    lvl = 3,
                    pointsToMatch = new point[]
                    {
                       new point(15,12),
                       new point(37,15),
                       new point(63,16),
                       new point(93,88, false),
                       new point(98,22, false),
                       new point(9,64, false),

                       new point(106,85, false),
                       new point(76,17, false),
                       new point(106,85, false),
                       new point(28,79, false),
                       new point(28,44, false),
                       new point(50,12, false),
                       new point(75,39, false),
                       new point(80,84, false),
                       new point(101,12, false),
                    }
            },

                new level()
                {
                    lvl = 2,
                    pointsToMatch = new point[]
                    {
                        new point(14,13),
                        new point(36,14),
                        new point(38,29),
                        new point(63,16, false),
                        new point(93,88, false),
                        new point(98,22, false),
                        new point(9,64, false),

                        new point(106,85, false),
                        new point(76,17, false),
                        new point(106,85, false),
                        new point(28,79, false),
                        new point(28,44, false),
                        new point(50,12, false),
                        new point(75,39, false),
                        new point(80,84, false),
                        new point(101,12, false),
                    }
            },

                new level()
                {
                    lvl = 1,
                    pointsToMatch = new point[]
                    {
                        new point(15,10),
                        new point(38,29, false),
                        new point(36,14, false),
                        new point(93,88, false),
                        new point(98,22, false),
                        new point(9,64, false),

                        new point(106,85, false),
                        new point(76,17, false),
                        new point(106,85, false),
                        new point(28,79, false),
                        new point(28,44, false),
                        new point(50,12, false),
                        new point(75,39, false),
                        new point(80,84, false),
                        new point(101,12, false),
                    }
                }
            };

            r.reset_points = new List<point>
            {
                new point(12,34),
                new point(150,40),

                new point(0,0, false),
                new point(0,59, false),
                new point(159,0, false),
                new point(159,59, false),
                new point(159,29, false),
                new point(0,29, false),
                new point(79,0, false),
                new point(39,0, false),
                new point(129,0, false),
            };

            r.level_rec = new System.Drawing.Rectangle(0, 809, 250, 120);

            r.reset_rec = new System.Drawing.Rectangle(723, 341, 160, 60);

            return r;
        }

        private static resolution R_1600x900()
        {
            resolution r = new resolution();
            r.window_width = 1606;
            r.window_height = 929;

            r.levels = new List<level> {
                new level()
                {
                    lvl = 70,
                    pointsToMatch = new point[]
                    {
                        // number seven
                       new point(7,4),
                       new point(20,4),
                       new point(41,4),
                       new point(40,11),
                       new point(36,17),
                       new point(29,29),
                       new point(25,38),
                       new point(21,47),
                       new point(17,62),
                       new point(16,77),

                         // number zero
                        new point(64,42),
                        new point(100,10),
                        new point(84,2),
                        new point(68,36),
                        new point(103,31),
                        new point(104,45),
                        new point(97,56),
                        new point(101,65),
                        new point(94,74),
                        new point(70,70),
                }
            },

                new level()
                {
                    lvl = 50,
                    pointsToMatch = new point[]
                    {
                        // number five
                       new point(41,3),
                       new point(10,4),
                       new point(26,3),
                       new point(8,13),
                       new point(8,26),
                       new point(8,36),
                       new point(27,29),
                       new point(37,34),
                       new point(42,42),
                       new point(43,53),
                       new point(40,65),

                       new point(34,72),
                       new point(25,75),
                       new point(11,72),
                       new point(7,64),

                        // number zero
                        new point(64,42),
                        new point(100,10),
                        new point(84,2),
                        new point(68,36),
                        new point(103,31),
                        new point(104,45),
                        new point(97,56),
                        new point(101,65),
                        new point(94,74),
                        new point(70,70),
                }
            },

                new level()
                {
                    lvl = 30,
                    pointsToMatch = new point[]
                    {
                        // number three
                        new point(8,14),
                        new point(14,3),
                        new point(40,10),
                        new point(37,28),
                        new point(22,36),
                        new point(37,42),
                        new point(43,55),
                        new point(40,69),
                        new point(27,74),
                        new point(12,73),
                        new point(7,64),

                        // number zero
                        new point(64,42),
                        new point(100,10),
                        new point(84,2),
                        new point(68,36),
                        new point(103,31),
                        new point(104,45),
                        new point(97,56),
                        new point(101,65),
                        new point(94,74),
                        new point(70,70),
                    }
            },

                new level()
                {
                    lvl = 15,
                    pointsToMatch = new point[]
                    {
                        // number one
                        new point(30,2),
                        new point(7,14),
                        new point(28,76),

                        // number five
                        new point(100,3),
                        new point(84,76),
                        new point(103,53),
                        new point(86,28),
                        new point(68,36),
                    }
            },

                new level()
                {
                    lvl = 10,
                    pointsToMatch = new point[]
                {
                        new point(98,22),
                        new point(9,64),
                        new point(145,60),
                        new point(169,51),
                        new point(192,41),
                        new point(225,22),
                        new point(135,64),

                        new point(106,85, false),
                        new point(76,17, false),
                        new point(106,85, false),
                        new point(28,79, false),
                        new point(28,44, false),
                        new point(50,12, false),
                        new point(75,39, false),
                        new point(80,84, false),
                        new point(101,12, false),
                        new point(237,62, false),
                    }
            },

                new level()
                {
                    lvl = 5,
                    pointsToMatch = new point[]
                    {
                        new point(98,22),
                        new point(9,64),
                        new point(145,60, false),
                        new point(169,51, false),
                        new point(192,41, false),

                        new point(106,85, false),
                        new point(76,17, false),
                        new point(106,85, false),
                        new point(28,79, false),
                        new point(28,44, false),
                        new point(50,12, false),
                        new point(75,39, false),
                        new point(80,84, false),
                        new point(101,12, false),
                    }
                },

                new level()
                {
                    lvl = 4,
                    pointsToMatch = new point[]
                    {
                        new point(90,19),
                        new point(93,87),
                        new point(98,22, false),
                        new point(9,64, false),

                        new point(106,85, false),
                        new point(76,17, false),
                        new point(106,85, false),
                        new point(28,79, false),
                        new point(28,44, false),
                        new point(50,12, false),
                        new point(75,39, false),
                        new point(80,84, false),
                        new point(101,12, false),
                    }
                },

                new level()
                {
                    lvl = 3,
                    pointsToMatch = new point[]
                    {
                       new point(15,12),
                       new point(37,15),
                       new point(63,16),
                       new point(93,88, false),
                       new point(98,22, false),
                       new point(9,64, false),

                       new point(106,85, false),
                       new point(76,17, false),
                       new point(106,85, false),
                       new point(28,79, false),
                       new point(28,44, false),
                       new point(50,12, false),
                       new point(75,39, false),
                       new point(80,84, false),
                       new point(101,12, false),
                    }
            },

                new level()
                {
                    lvl = 2,
                    pointsToMatch = new point[]
                    {
                        new point(14,13),
                        new point(36,14),
                        new point(38,29),
                        new point(63,16, false),
                        new point(93,88, false),
                        new point(98,22, false),
                        new point(9,64, false),

                        new point(106,85, false),
                        new point(76,17, false),
                        new point(106,85, false),
                        new point(28,79, false),
                        new point(28,44, false),
                        new point(50,12, false),
                        new point(75,39, false),
                        new point(80,84, false),
                        new point(101,12, false),
                    }
            },

                new level()
                {
                    lvl = 1,
                    pointsToMatch = new point[]
                    {
                        new point(15,10),
                        new point(38,29, false),
                        new point(36,14, false),
                        new point(93,88, false),
                        new point(98,22, false),
                        new point(9,64, false),

                        new point(106,85, false),
                        new point(76,17, false),
                        new point(106,85, false),
                        new point(28,79, false),
                        new point(28,44, false),
                        new point(50,12, false),
                        new point(75,39, false),
                        new point(80,84, false),
                        new point(101,12, false),
                    }
                }
            };

            r.reset_points = new List<point>
            {
                new point(12,34),
                new point(150,40),

                new point(0,0, false),
                new point(0,59, false),
                new point(159,0, false),
                new point(159,59, false),
                new point(159,29, false),
                new point(0,29, false),
                new point(79,0, false),
                new point(39,0, false),
                new point(129,0, false),
            };

            r.level_rec = new System.Drawing.Rectangle(0, 809, 250, 120);

            r.reset_rec = new System.Drawing.Rectangle(723, 341, 160, 60);

            return r;
        }
    }
}
