using System;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Threading.Tasks;
using System.Globalization;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using HtmlAgilityPack;
using Diacritics.Extensions;
namespace meditool
{
    class Program
    {
        static void Main(string[] args)
        {
            var config = new Config();
            config = JsonConvert.DeserializeObject<Config>(File.ReadAllText(Directory.GetCurrentDirectory() + Path.DirectorySeparatorChar.ToString() + args[0]));
            do
            {
                Worker.Execute(args[0]);
                System.Threading.Thread.Sleep(config.CheckIntervalMinutes * 60 * 1000);
            }
            while (1 == 1);
        }
    }
}