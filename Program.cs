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

        static MySession s;
        static Config config = new Config();
        static ConsultationFound LastResult = new ConsultationFound();
        public static List<DoctorInfo> Doctors = new List<DoctorInfo>();
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
            if (config.AfterHour > 0)
            {
                c = s.SendRequest(String.Format("https://mol.medicover.pl/api/MyVisits/SearchFreeSlotsToBook/GetFiltersData?regionIds={0}&serviceTypeId={1}&serviceIds={2}&&&&searchSince={3}&startTime={4}&endTime={5}&selectedSpecialties=null", JClass.regionIds[0], JClass.serviceTypeId, JClass.serviceIds[0], JClass.searchSince, config.AfterHour.ToString() + ":00", "23:59"), "https://mol.medicover.pl", HttpMethod.Get);
            }
            string b = s.SendRequestJson(jsonoutput, "https://mol.medicover.pl/api/MyVisits/SearchFreeSlotsToBook?language=pl-PL", referer);
            //File.WriteAllText("result.json", b);
            ConsultationsFound test = (JsonConvert.DeserializeObject<ConsultationsFound>(b));
            ConsultationsFound test2 = new ConsultationsFound();
            foreach (var t in test.items) {
                if (t.serviceId == JClass.serviceIds[0]) {
                         test.items.Add(t);
                }
            }
            return test2;

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

            c = s.SendRequestJson(jsonoutput, String.Format("https://mol.medicover.pl/pfm/pfm4s2/api/appointment-slots/{0}", pfmSession.id), string.Format("https://mol.medicover.pl/pfm?pfmProcessDescriptor={0}", JClass.MeetingString));
            AppointmentData2 p3 = JsonConvert.DeserializeObject<AppointmentData2>(c);
            //c = s.SendRequest(String.Format("https://mol.medicover.pl/pfm/pfm4s2/api/dictionary/clinics?region={0}&specialties={1}&vendors={2}", sk1.region, String.Join(", ", p3.specialties.ToArray()), String.Join(", ", p3.vendors.ToArray())), "https://mol.medicover.pl", HttpMethod.Get);
            c = s.SendRequest(String.Format("https://mol.medicover.pl/pfm/pfm4s2/api/dictionary/clinics"), string.Format("https://mol.medicover.pl/pfm?pfmProcessDescriptor={0}", JClass.MeetingString), HttpMethod.Get);
            List<PfmDictionaryItem> clinics = (JsonConvert.DeserializeObject<List<PfmDictionaryItem>>(c));

            //c = s.SendRequest(String.Format("https://mol.medicover.pl/pfm/pfm4s2/api/dictionary/doctors?region={0}&specialties={1}&vendors={2}", sk1.region,String.Join(", ", p3.specialties.ToArray()),String.Join(", ", p3.vendors.ToArray())), "https://mol.medicover.pl", HttpMethod.Get);
            c = s.SendRequest(String.Format("https://mol.medicover.pl/pfm/pfm4s2/api/dictionary/doctors"), string.Format("https://mol.medicover.pl/pfm?pfmProcessDescriptor={0}", JClass.MeetingString), HttpMethod.Get);
            List<PfmDictionaryItem> doctors = (JsonConvert.DeserializeObject<List<PfmDictionaryItem>>(c));
            jsonoutput = JsonConvert.SerializeObject(doctors, Formatting.Indented,
                         new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });

            File.WriteAllText("doctors.json", jsonoutput);


            c = s.SendRequest(String.Format("https://mol.medicover.pl/pfm/pfm4s2/api/dictionary/specialty"), string.Format("https://mol.medicover.pl/pfm?pfmProcessDescriptor={0}", JClass.MeetingString), HttpMethod.Get);
            List<PfmDictionaryItem> specializations = (JsonConvert.DeserializeObject<List<PfmDictionaryItem>>(c));
            PfmSearch2 sk2 = new PfmSearch2();
            sk2.caseId = sk1.caseId;
            sk2.clinic = sk1.clinic;
            sk2.date = sk1.date;
            sk2.doctor = sk1.doctor;
            sk2.region = sk1.region;
            sk2.slot = sk2.slot;
            sk2.ticket = p3.ticket;
            jsonoutput = JsonConvert.SerializeObject(sk1, Formatting.Indented,
                                    new JsonSerializerSettings
                                    {
                                        DateFormatHandling = DateFormatHandling.IsoDateFormat,
                                        DateFormatString = "yyyy-MM-ddTHH:mm:ss",
                                        //NullValueHandling = NullValueHandling.Ignore
                                    });
            c = s.SendRequestJson(jsonoutput, String.Format("https://mol.medicover.pl/pfm/pfm4s2/api/appointment-slots/{0}", pfmSession.id), string.Format("https://mol.medicover.pl/pfm?pfmProcessDescriptor={0}", JClass.MeetingString));
            ConsultationFoundInternalV2 cs2 = JsonConvert.DeserializeObject<ConsultationFoundInternalV2>(c);
            ConsultationsFound csf = new ConsultationsFound();
            //DateTime LastVisitFound = sk2.date;
            do
            {

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
                //sk2.date = csf.items.OrderBy(w => w.appointmentDate).Last().appointmentDate.AddMinutes(1);
                sk2.ticket = cs2.ticket;
                jsonoutput = JsonConvert.SerializeObject(sk2, Formatting.Indented,
                                    new JsonSerializerSettings
                                    {
                                        DateFormatHandling = DateFormatHandling.IsoDateFormat,
                                        DateFormatString = "yyyy-MM-ddTHH:mm:ss",
                                        //NullValueHandling = NullValueHandling.Ignore
                                    });
                c = "";
                c = s.SendRequestJson(jsonoutput, String.Format("https://mol.medicover.pl/pfm/pfm4s2/api/appointment-slots/{0}", pfmSession.id), string.Format("https://mol.medicover.pl/pfm?pfmProcessDescriptor={0}", JClass.MeetingString));
                cs2 = null;
                cs2 = JsonConvert.DeserializeObject<ConsultationFoundInternalV2>(c);
            } while (cs2.appointmentSlots.Count > 0);
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

                        string Score = "";
                        Score += GetDoctorsDataMedicover(searchResults2[0].doctorName);
                        Score += GetDataFromZnanyLekarz(searchResults2[0].doctorName);

                        if (Score == "")
                        {
                            OutText = string.Format("{3}: Kiedy: {0}, {5}  Gdzie:  {1}   Kto: {2}, {4}", searchResults2[0].appointmentDate.ToString("yyyy-MM-dd HH:mm"), searchResults2[0].clinicName, searchResults2[0].doctorName, DateTime.Now.ToShortTimeString(), searchResults2[0].specializationName, DateTimeFormatInfo.CurrentInfo.GetDayName(searchResults2[0].appointmentDate.DayOfWeek));
                        }
                        else
                        {
                            OutText = string.Format("{3}: Kiedy: {0}, {5}  Gdzie:  {1}   Kto: {2}, {4}   Ocena: {6}", searchResults2[0].appointmentDate.ToString("yyyy-MM-dd HH:mm"), searchResults2[0].clinicName, searchResults2[0].doctorName, DateTime.Now.ToShortTimeString(), searchResults2[0].specializationName, DateTimeFormatInfo.CurrentInfo.GetDayName(searchResults2[0].appointmentDate.DayOfWeek), Score);
                        }
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
                    }
                    else
                    {
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
        public static string GetDataFromZnanyLekarz(string DoctorsName)
        {
            string Result = "";

            DoctorsName = DoctorsName.Replace(" - ", "_");

            List<string> tmp = Regex.Split(DoctorsName, " ").ToList();
            tmp.Reverse();
            string SearchPhrase = "";
            foreach (var a in tmp)
            {
                SearchPhrase += a + "-";
            }
            var tmp4 = Regex.Split(tmp[1],"_").ToList();
            var SearchPhraseOrig = String.Format("{0}-{1}",tmp[0],String.Join("_",tmp4)).Trim().RemoveDiacritics();
            tmp4.Reverse();
            var tmp3 = String.Join("_",tmp4);
            //SearchPhrase = SearchPhrase.Remove(SearchPhrase.Length - 1);
            SearchPhrase = String.Format("{0}-{1}",tmp[0],tmp3);
            string SearchPhrase2 = SearchPhrase.Trim().RemoveDiacritics();
            SearchPhrase2 = SearchPhrase2.Replace("_", "-");
            //string Url = String.Format("https://www.znanylekarz.pl/ranking-lekarzy/{0}", SearchPhrase2.ToLower());
            string Url = String.Format("https://www.znanylekarz.pl/szukaj?q={0}&loc=", SearchPhrase.Replace("-", "+").ToLower());

            HttpClient h = new HttpClient();
            h.DefaultRequestHeaders.TryAddWithoutValidation("Accept-Language", "pl; q=1.0");
            bool HttpOk = false;
            String tmp1 = "";
            int http_attemps = 0;
            do
            {
                try
                {
                    http_attemps++;
                    tmp1 = h.GetStringAsync(new Uri(Url)).GetAwaiter().GetResult();
                    HttpOk = true;
                }
                catch
                {

                }
            } while (!HttpOk & http_attemps < 3);
            HtmlDocument htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(tmp1);
            var node3 = htmlDoc.DocumentNode.SelectSingleNode("//a[@data-ga-action='Detail click']");
            if (node3 != null)
            {
                Url = node3.Attributes["href"].Value;   
                HttpOk = false;
                tmp1 = "";
                http_attemps = 0;
                do
                {
                    try
                    {
                        http_attemps++;
                        tmp1 = h.GetStringAsync(new Uri(Url)).GetAwaiter().GetResult();
                        HttpOk = true;
                    }
                    catch
                    {

                    }
                } while (!HttpOk & http_attemps < 3);
                htmlDoc = new HtmlDocument();
                htmlDoc.LoadHtml(tmp1);
                //File.WriteAllText("ZL.html", tmp1);

                var nodes = htmlDoc.DocumentNode.SelectNodes("//*/span[@data-test-id='doctor-header-name']");
                if (nodes != null)
                {
                    if (nodes.First().InnerText.Trim().Replace("-","_").Replace(" ", "-").Trim().RemoveDiacritics() == SearchPhraseOrig )
                    {
                        var doc2 = new HtmlDocument();
                        doc2.LoadHtml(nodes.First().InnerHtml);
                        var node = htmlDoc.DocumentNode.SelectSingleNode("//u[@class='rating rating--lg']");
                        if (node != null)
                        {
                            try
                            {
                                var node2 = node.SelectSingleNode("span");
                                var tmp2 = Regex.Split(node2.InnerText, " ");
                                Result = string.Format("ZL: {0}/5  {1} opinii", node.Attributes["data-score"].Value, tmp2[0]);
                            }
                            catch
                            {
                                Result = "Brak opinii na ZnanyLekarz";
                            }
                        }
                        else
                        {
                            Result = "Brak opinii na ZnanyLekarz";
                        }
                    }
                }
            }
            return Result;
        }
        public static string GetDoctorsDataMedicoverOnline(string DoctorsName)
        {
            string Result = "";

            DoctorsName = DoctorsName.Replace(" - ", "-");

            List<string> tmp = Regex.Split(DoctorsName, " ").ToList();
            //tmp.Reverse();
            string SearchPhrase = "";
            foreach (var a in tmp)
            {
                SearchPhrase += a + "-";
            }
            SearchPhrase = SearchPhrase.Remove(SearchPhrase.Length - 1);
            string SearchPhrase2 = SearchPhrase.Trim().RemoveDiacritics();
            SearchPhrase2 = SearchPhrase2.Replace(" ", "-");
            string Url = String.Format("https://www.medicover.pl/lekarze/{0},n,s", SearchPhrase2.ToLower());

            HttpClient h = new HttpClient();
            h.DefaultRequestHeaders.TryAddWithoutValidation("Accept-Language", "pl; q=1.0");
            bool HttpOk = false;
            String tmp1 = "";
            int http_attemps = 0;
            do
            {
                try
                {
                    http_attemps++;
                    tmp1 = h.GetStringAsync(new Uri(Url)).GetAwaiter().GetResult();
                    HttpOk = true;
                }
                catch
                {

                }
            } while (!HttpOk & http_attemps < 3);
            HtmlDocument htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(tmp1);
            File.WriteAllText("ZL.html", tmp1);

            var nodes = htmlDoc.DocumentNode.SelectNodes("//*/ul[@data-id='search-list']/li//*/div[@data-id='rank-element']");
            if (nodes != null)
            {
                HtmlNode node = null;
                foreach (var nodetmp in nodes)
                {
                    var node3 = nodetmp.SelectNodes("div//*/span[@itemprop='name']");
                    var node4 = node3.Where(n => n.InnerText.Trim().ToUpper() == SearchPhrase.Replace("-", " ").Replace("_", "-").ToUpper()).FirstOrDefault();
                    if (node4 != null)
                    {
                        node = nodetmp;
                        break;
                    }
                }


                //var node = nodes.Where(n => n.Attributes["data-eecommerce-category"].Value.ToUpper() == SearchPhrase.Replace("-", " ").Replace("_", " ").ToUpper());
                //var node = htmlDoc.DocumentNode.SelectNodes("//*/ul[@data-id='search-list']/li[1]//*/div[@data-id='rank-element']").First();

                if (node != null)
                {
                    //if (node.Attributes["data-eecommerce-category"].Value.ToUpper() == SearchPhrase.Replace("-", " ").Replace("_", "-").ToUpper())
                    if (1 == 1)
                    {

                        try
                        {
                            var node2 = node.SelectNodes("div//*/a[@class='rating rating--md text-muted']").FirstOrDefault();
                            Result = string.Format("ZL: {0}/5  {1} opinii", node2.Attributes["data-score"].Value, node2.Attributes["data-total-count"].Value);
                        }
                        catch
                        {
                            Result = "Brak opinii na ZnanyLekarz";
                        }
                    }
                }
            }
            return Result;
        }

        public static string GetDoctorsDataMedicover(string DoctorsName)
        {
            DoctorsName = DoctorsName.Replace(" - ", "_");

            List<string> tmp = Regex.Split(DoctorsName, " ").ToList();
            tmp.Reverse();
            string SearchPhrase = "";
            foreach (var a in tmp)
            {
                SearchPhrase += a + "-";
            }
            SearchPhrase = SearchPhrase.Remove(SearchPhrase.Length - 1);
            string SearchPhrase2 = SearchPhrase.Trim().RemoveDiacritics();
            SearchPhrase2 = SearchPhrase2.Replace("-", " ");
            SearchPhrase2 = SearchPhrase2.Replace("_", " - ");
            var doc = Doctors.Where(d => d.DoctorName.ToUpper().RemoveDiacritics() == SearchPhrase2.ToUpper());

            string Score = "";
            if (doc.Count() == 1)
            {
                var doc2 = doc.First();
                Score = String.Format("{0} ; {1} ocen", doc2.Rank, doc2.SurveyCount);
                if (doc2.Restrictions != "")
                {
                    Score += String.Format("  Restrykcje: {0}", doc2.Restrictions);
                }
            }
            if (Score != "")
            {
                Score += " ";
            }

            return Score;
        }
        static void Main(string[] args)
        {

            if (System.IO.File.Exists(Directory.GetCurrentDirectory() + Path.DirectorySeparatorChar.ToString() + "doctors.json.db"))
            {
                Doctors = JsonConvert.DeserializeObject<List<DoctorInfo>>(File.ReadAllText(Directory.GetCurrentDirectory() + Path.DirectorySeparatorChar.ToString() + "doctors.json.db"));
            }
            //string aa = GetDataFromZnanyLekarz("Śliwiński Marek");
            string bb = GetDoctorsDataMedicover("Bielec - Leskiewicz Anna");
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
                            //PushOverSender.SendPushMessage(config.pushOverUserId, config.pushOverAppTokenId, "Medicover Hunt", String.Format("{0} : Blad pobierania danych.", args[0]));
                        }
                        Console.WriteLine(String.Format("{0}: Wystapil blad pobierania danych", DateTime.Now.ToShortTimeString()));
                    }
                }
                System.Threading.Thread.Sleep(config.CheckIntervalMinutes * 60 * 1000);
            } while (1 == 1);
        }

    }
}
