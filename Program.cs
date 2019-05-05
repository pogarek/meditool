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
            string c = s.SendRequest("https://mol.medicover.pl/MyVisits?bookingTypeId=2&mex=True&pfm=1", "https://mol.medicover.pl", HttpMethod.Get);
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
            string c = s.SendRequest("https://mol.medicover.pl/MyVisits?bookingTypeId=1&mex=True&pfm=1", "https://mol.medicover.pl", HttpMethod.Get);
            string b = s.SendRequestJson(jsonoutput, "https://mol.medicover.pl/api/MyVisits/SearchFreeSlotsToBook?language=pl-PL", referer);
            ConsultationsFound test = (JsonConvert.DeserializeObject<ConsultationsFound>(b));
            return test;

        }

        private static ConsultationsFound PfmSearch(PfmSearch JClass)
        {
            //string MeetingString = "PREVENTION_AND_VACCINATION_OTHER_VACCINATIONS_ADULT";
            //string surveyName = "vaccinations";
            //string surveyresponse = @"[""pyt44odp1.2""]";
            //string MeetingString = "INTERNIST_COLD_WITHOUT_SURVEY";
            string c = s.SendRequest(String.Format("https://mol.medicover.pl/pfm/pfm4s2/api/case/create?s2ctx={0}", JClass.MeetingString), "https://mol.medicover.pl", HttpMethod.Post);
            PfmSession pfmSession = JsonConvert.DeserializeObject<PfmSession>(c);
            if (JClass.surveyName != null)
            {
                c = s.SendRequest(String.Format("https://mol.medicover.pl/pfm/pfm4s2/api/case/{0}/{1}/data", pfmSession.id, JClass.surveyName), "https://mol.medicover.pl", HttpMethod.Get);
                c = s.SendRequestJson(JClass.surveyresponse, String.Format("https://mol.medicover.pl/pfm/pfm4s2/api/case/{0}/{1}/complete", pfmSession.id, JClass.surveyName), "https://mol.medicover.pl");

            }
            c = s.SendRequest(String.Format("https://mol.medicover.pl/pfm/pfm4s2/api/case/{0}/appointment/data", pfmSession.id), "https://mol.medicover.pl", HttpMethod.Get);
            AppointmentData p2 = JsonConvert.DeserializeObject<AppointmentData>(c);
            PfmSearch1 sk1 = new PfmSearch1();
            sk1.caseId = pfmSession.id;
            sk1.region = JClass.regionId.ToString();
            sk1.date = JClass.date;
            sk1.clinic = JClass.clinic;
            sk1.doctor = JClass.doctor;
            string jsonoutput = JsonConvert.SerializeObject(sk1, Formatting.Indented,
                                    new JsonSerializerSettings
                                    {
                                        DateFormatHandling = DateFormatHandling.IsoDateFormat,
                                        DateFormatString = "yyyy-MM-ddTHH:mm:ss",
                                        //NullValueHandling = NullValueHandling.Ignore
                                    });

            c = s.SendRequestJson(jsonoutput, String.Format("https://mol.medicover.pl/pfm/pfm4s2/api/appointment-slots/{0}", pfmSession.id), "https://mol.medicover.pl");
            AppointmentData2 p3 = JsonConvert.DeserializeObject<AppointmentData2>(c);
            //c = s.SendRequest(String.Format("https://mol.medicover.pl/pfm/pfm4s2/api/dictionary/clinics?region={0}&specialties={1}&vendors={2}", sk1.region, String.Join(", ", p3.specialties.ToArray()), String.Join(", ", p3.vendors.ToArray())), "https://mol.medicover.pl", HttpMethod.Get);
            c = s.SendRequest(String.Format("https://mol.medicover.pl/pfm/pfm4s2/api/dictionary/clinics"), "https://mol.medicover.pl", HttpMethod.Get);
            List<PfmDictionaryItem> clinics = (JsonConvert.DeserializeObject<List<PfmDictionaryItem>>(c));

            //c = s.SendRequest(String.Format("https://mol.medicover.pl/pfm/pfm4s2/api/dictionary/doctors?region={0}&specialties={1}&vendors={2}", sk1.region,String.Join(", ", p3.specialties.ToArray()),String.Join(", ", p3.vendors.ToArray())), "https://mol.medicover.pl", HttpMethod.Get);
            c = s.SendRequest(String.Format("https://mol.medicover.pl/pfm/pfm4s2/api/dictionary/doctors"), "https://mol.medicover.pl", HttpMethod.Get);
            List<PfmDictionaryItem> doctors = (JsonConvert.DeserializeObject<List<PfmDictionaryItem>>(c));
            jsonoutput = JsonConvert.SerializeObject(doctors, Formatting.Indented,
                         new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });

            File.WriteAllText("doctors.json", jsonoutput);


            c = s.SendRequest(String.Format("https://mol.medicover.pl/pfm/pfm4s2/api/dictionary/specialty"), "https://mol.medicover.pl", HttpMethod.Get);
            List<PfmDictionaryItem> specializations = (JsonConvert.DeserializeObject<List<PfmDictionaryItem>>(c));
            PfmSearch2 sk2 = new PfmSearch2();
            sk2.caseId = sk1.caseId;
            sk2.clinic = sk1.clinic;
            sk2.date = sk1.date;
            sk2.doctor = sk1.doctor;
            sk2.region = sk1.region;
            sk2.slot = sk2.slot;
            sk2.ticket = p3.ticket;
            jsonoutput = JsonConvert.SerializeObject(sk2, Formatting.Indented,
                                    new JsonSerializerSettings
                                    {
                                        DateFormatHandling = DateFormatHandling.IsoDateFormat,
                                        DateFormatString = "yyyy-MM-ddTHH:mm:ss",
                                        //NullValueHandling = NullValueHandling.Ignore
                                    });
            c = s.SendRequestJson(jsonoutput, String.Format("https://mol.medicover.pl/pfm/pfm4s2/api/appointment-slots/{0}", pfmSession.id), "https://mol.medicover.pl");
            ConsultationFoundInternalV2 cs2 = JsonConvert.DeserializeObject<ConsultationFoundInternalV2>(c);
            ConsultationsFound csf = new ConsultationsFound();
            foreach (var entry in cs2.appointmentSlots)
            {
                ConsultationFound cf = new ConsultationFound();
                cf.appointmentDate = entry.dateTime;
                var aaa = doctors.Where(cf2 => cf2.code == entry.doctor);
                cf.doctorName = doctors.Where(cf2 => cf2.code == entry.doctor).FirstOrDefault().label;
                cf.clinicName = clinics.Where(cf1 => cf1.code == entry.clinic).FirstOrDefault().label;
                cf.id = entry.id;
                cf.specializationName = specializations.Where(cf3 => cf3.code == entry.specialty).FirstOrDefault().label;
                csf.items.Add(cf);
            }
            csf.items = csf.items.OrderBy(w => w.appointmentDate).ToList();
            return csf;
        }
        private static void Run(string[] args)
        {
            s = new meditool.MySession();
            string OutText = "";
            s.Login(config.UserName, config.Password);


            bool DataOk = false;
            ConsultationsFound searchResults = new ConsultationsFound();
            DateTime StartDate = DateTimeOffset.Now.Date;

            if (config.ConsultationSearchData != null)
            {
                SearchVisit_Konsultacja JClass = new SearchVisit_Konsultacja();
                JClass = config.ConsultationSearchData;
                JClass.searchSince = JClass.searchSince.AddHours(3);
                searchResults = SearchForConsultation(JClass);
                DataOk = true;
                StartDate = config.ConsultationSearchData.searchSince;
            }
            if (config.ExaminationSearchData != null)
            {
                DataOk = true;
                SearchVisit_Badanie JClass = new SearchVisit_Badanie();
                JClass = config.ExaminationSearchData;
                JClass.searchSince = JClass.searchSince.AddHours(3);
                searchResults = SearchForExamination(JClass);
                StartDate = config.ExaminationSearchData.searchSince;
            }
            if (config.PfmSearchData != null)
            {
                DataOk = true;
                PfmSearch JClass = new PfmSearch();
                JClass = config.PfmSearchData;
                JClass.date = JClass.date.AddHours(3);
                searchResults = PfmSearch(JClass);
                StartDate = config.PfmSearchData.date;
            }
            Console.Title = String.Format("{0}: {1}  + {2} dni", args[0], StartDate.ToString("yyyy-MM-dd"), config.DoNotSendPushForSlotsAboveDays.ToString());
            //Console.WriteLine(String.Format("StartDate: {0}",StartDate.ToString()));    
            if (DataOk)
            {

                if (searchResults.items.Count > 0)

                {
                    var searchResults2 = searchResults.items.Where(w => w.appointmentDate.Hour >= config.AfterHour).ToList();
                    if (searchResults2.Count > 0)
                    {

                        OutText = string.Format("{3}: Kiedy: {0}, {5}  Gdzie:  {1}   Kto: {2}, {4}", searchResults2[0].appointmentDate.ToString("yyyy-MM-dd HH:mm"), searchResults.items[0].clinicName, searchResults.items[0].doctorName, DateTime.Now.ToShortTimeString(), searchResults.items[0].specializationName, DateTimeFormatInfo.CurrentInfo.GetDayName(searchResults.items[0].appointmentDate.DayOfWeek));
                        Console.WriteLine(OutText);
                        if (searchResults2[0].appointmentDate != LastResult.appointmentDate)
                        {
                            LastResult = searchResults2[0];
                            var dt = LastResult.appointmentDate - StartDate;
                            if (dt.Days <= config.DoNotSendPushForSlotsAboveDays)
                            {
                                if (LastResult.appointmentDate.Hour >= config.AfterHour)
                                {
                                    if (config.UsePushOver)
                                    {
                                        PushOverSender.SendPushMessage(config.pushOverUserId, config.pushOverAppTokenId, "Medicover Hunt", OutText);
                                        Console.WriteLine("==> Push wysłany");
                                    }
                                }
                                else
                                {
                                    Console.WriteLine(String.Format("==> Wizyta jest o złej godzinie. Nie wysyłam powiadomienia", config.DoNotSendPushForSlotsAboveDays.ToString(), StartDate.ToString("yyyy-MM-dd")));
                                }
                            }
                            else
                            {
                                Console.WriteLine(String.Format("==> Wizyta jest za więcej niz {0} dni od {1}. Nie wysyłam powiadomienia", config.DoNotSendPushForSlotsAboveDays.ToString(), StartDate.ToString("yyyy-MM-dd")));
                            }
                        }
                    } else {
                         Console.WriteLine(String.Format("{0}: Brak wizyt spełniających zadane kryteria po danej godzinie", DateTime.Now.ToShortTimeString()));
                    }
                }
                else
                {
                    Console.WriteLine(String.Format("{0}: Brak wizyt spełniających zadane kryteria", DateTime.Now.ToShortTimeString()));
                }
            }

            //string sss = "";
        }

        static void Main(string[] args)
        {


            do
            {
                config = new Config();
                config = JsonConvert.DeserializeObject<Config>(File.ReadAllText(Directory.GetCurrentDirectory() + Path.DirectorySeparatorChar.ToString() + args[0]));
                try
                {
                    Run(args);
                }
                catch
                {
                    if (config.UsePushOver)
                    {
                        {
                            PushOverSender.SendPushMessage(config.pushOverUserId, config.pushOverAppTokenId, "Medicover Hunt", String.Format("{0} : Blad pobierania danych.", args[0]));
                        }
                        Console.WriteLine(String.Format("{0}: Wystapil blad pobierania danych", DateTime.Now.ToShortTimeString()));
                    }
                }
                System.Threading.Thread.Sleep(config.CheckIntervalMinutes * 60 * 1000);
            } while (1 == 1);
        }

    }
}
