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
        public static List<Resolution> InitializePoints()
        {
            if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "data.json"))
            {
               return JsonConvert.DeserializeObject<Resolution[]>(File.ReadAllText(AppDomain.CurrentDomain.BaseDirectory + "data.json")).ToList();
            }

            return new List<Resolution>();
        }
    }
}
