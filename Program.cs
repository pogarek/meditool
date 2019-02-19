using System;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Threading.Tasks;

namespace meditool
{
    class Program
    {

        static MySession s;
        static Config config = new Config();
        static ConsultationFound LastResult = new ConsultationFound();
        static ConsultationsFound SearchForConsultation(SearchVisit_Konsultacja JClass)
        {
            string jsonoutput = JsonConvert.SerializeObject(JClass, Formatting.Indented,
                                    new JsonSerializerSettings
                                    {
                                        DateFormatHandling = DateFormatHandling.IsoDateFormat,
                                        DateFormatString = "yyyy-MM-ddTHH:mm:ss.000Z",
                                        //NullValueHandling = NullValueHandling.Ignore
                                    });
            string referer = "https://mol.medicover.pl/MyVisits";
            string c = s.SendRequest("https://mol.medicover.pl/MyVisits?bookingTypeId=2&mex=True&pfm=1", "https://mol.medicover.pl");
            string b = s.SendRequestJson(jsonoutput, "https://mol.medicover.pl/api/MyVisits/SearchFreeSlotsToBook?language=pl-PL", referer);
            ConsultationsFound test = (JsonConvert.DeserializeObject<ConsultationsFound>(b));
            return test;

        }
        static ConsultationsFound SearchForExamination(SearchVisit_Badanie JClass)
        {
            string jsonoutput = JsonConvert.SerializeObject(JClass, Formatting.Indented,
                                    new JsonSerializerSettings
                                    {
                                        DateFormatHandling = DateFormatHandling.IsoDateFormat,
                                        DateFormatString = "yyyy-MM-ddTHH:mm:ss.000Z",
                                        //NullValueHandling = NullValueHandling.Ignore
                                    });
            string referer = "https://mol.medicover.pl/MyVisits";
            string c = s.SendRequest("https://mol.medicover.pl/MyVisits?bookingTypeId=1&mex=True&pfm=1", "https://mol.medicover.pl");
            string b = s.SendRequestJson(jsonoutput, "https://mol.medicover.pl/api/MyVisits/SearchFreeSlotsToBook?language=pl-PL", referer);
            ConsultationsFound test = (JsonConvert.DeserializeObject<ConsultationsFound>(b));
            return test;

        }

        private static void Run(string[] args)
        {
            //Console.WriteLine(String.Format("{0}: Szukam",DateTime.Now.ToShortTimeString()));
            s = new meditool.MySession();
            string OutText = "";
            s.Login(config.UserName, config.Password);
            bool DataOk = false;
            ConsultationsFound searchResults = new ConsultationsFound();
            if (config.ConsultationSearchData != null)
            {
                var JClass = config.ConsultationSearchData;
                JClass.searchSince = JClass.searchSince.AddHours(3);
                searchResults = SearchForConsultation(JClass);
                DataOk = true;
            }
            if (config.ExamindationSearchData != null)
            {
                DataOk = true;
                var JClass = config.ExamindationSearchData;
                JClass.searchSince = JClass.searchSince.AddHours(3);
                searchResults = SearchForExamination(JClass);
            }
            if (DataOk)
            {
                if (searchResults.items.Count > 0)
                {
                    OutText = string.Format("{3}: Kiedy: {0}  Gdzie: {1} Kto: {2}, {4}", searchResults.items[0].appointmentDate.ToString("yyyy-MM-dd HH:mm"), searchResults.items[0].clinicName, searchResults.items[0].doctorName, DateTime.Now.ToShortTimeString(), searchResults.items[0].specializationName);
                    Console.WriteLine(OutText);
                    if (searchResults.items[0].appointmentDate != LastResult.appointmentDate)
                    {
                        LastResult = searchResults.items[0];
                        var dt = LastResult.appointmentDate - DateTime.Now;
                        if (dt.Days <= config.DoNotSendPushForSlotsAboveDays)
                        {
                            PushOverSender.SendPushMessage(config.pushOverUserId, config.pushOverAppTokenId, "Medicover Hunt", OutText);
                            Console.WriteLine("Push wysłany");
                        }
                        else
                        {
                            Console.WriteLine(String.Format("Wizyta jest za więcej niz {0} dni. Nie wysyłam powiadomienia", config.DoNotSendPushForSlotsAboveDays.ToString()));
                        }
                    }
                } else {
                    Console.WriteLine(String.Format("{0}: Brak wizyt spełniających zadane kryteria",DateTime.Now.ToShortTimeString()));
                }
            }
            
            //string sss = "";
        }

        static void Main(string[] args)
        {

            config = JsonConvert.DeserializeObject<Config>(File.ReadAllText(Directory.GetCurrentDirectory() + Path.DirectorySeparatorChar.ToString() + args[0]));

            do
            {
                Run(args);
                System.Threading.Thread.Sleep(config.CheckIntervalMinutes * 60 * 1000);
            } while (1 == 1);
        }

    }
}
