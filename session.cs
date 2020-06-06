using System;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Globalization;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using HtmlAgilityPack;

namespace meditool
{
    class MySession
    {
        private static HttpClientHandler handler;
        private static HttpClient h;
        private static string LoginUrl;


        private  HttpRequestMessage AddHeadersLogin(HttpRequestMessage request)
        {
            request.Headers.Add("User-Agent", "Mozilla/5.0 (X11; Linux x86_64; rv:52.0) Gecko/20100101 Firefox/52.0");
            request.Headers.Add("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8");
            request.Headers.Add("Accept-Language", "pl,en-US,en;q=0.5");
            request.Headers.Add("Accept-Encoding", "gzip, deflate, br");
            request.Headers.Add("Connection", "keep-alive");
            request.Headers.Add("Upgrade-Insecure-Request", "1");
            return request;
        }
        private static HttpRequestMessage AddHeadersJsonRequest(HttpRequestMessage request)
        {
            request.Headers.Add("Host", "mol.medicover.pl");
            request.Headers.Add("Connection", "keep-alive");
            request.Headers.Add("Accept", "*/*");
            request.Headers.Add("Origin", "https://mol.medicover.pl");
            request.Headers.Add("X-Requested-With", "XMLHttpRequest");
            request.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/72.0.3626.109 Safari/537.36");
            request.Headers.Add("Accept-Encoding", "deflate, br");
            request.Headers.Add("Accept-Language", "en-US,en;q=0.9,af;q=0.8,pl;q=0.7");
            return request;
        }
        
        public string SendRequest (string url, string referer, HttpMethod method) {
            HttpRequestMessage request = new HttpRequestMessage( method, url);
            request = AddHeadersLogin(request);
            request.Headers.Add("Referer",referer);
            var response = h.SendAsync(request).Result; 
            string result = response.Content.ReadAsStringAsync().Result;
            return result;
        }
        public string SendRequestJson(string RequestBody, string url, string referer)
        {
            HttpRequestMessage request = new HttpRequestMessage( HttpMethod.Post, url);
            request = AddHeadersJsonRequest(request);
            request.Headers.Add("Referer",referer);
            request.Content = new StringContent(RequestBody);
            var MediaType = new MediaTypeHeaderValue("application/json");
            //MediaType.CharSet = "utf-8";
            request.Content.Headers.ContentType = MediaType;
            var response = h.SendAsync(request).Result; 
            string result = response.Content.ReadAsStringAsync().Result;
            return result;
        }
        
        public void RefreshMain () {
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, "https://mol.medicover.pl");
                request = AddHeadersLogin(request);
                //request.Headers.Add("Referer", LoginUrl);
                var response = h.SendAsync(request).Result;
                string result = response.Content.ReadAsStringAsync().Result;
                string aaaa = "";

        }
        public void Login(string username, string password) {
            handler = new HttpClientHandler();
            handler.AutomaticDecompression = System.Net.DecompressionMethods.GZip;
            h = new HttpClient(handler);
            h.Timeout = TimeSpan.FromSeconds(120);
            
            //zaczynamy .
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, "https://mol.medicover.pl/Users/Account/LogOn?ReturnUrl=%2F");
            request = AddHeadersLogin(request);
            HttpResponseMessage response = h.SendAsync(request).Result;
            string result = response.Content.ReadAsStringAsync().Result;
            string[] tmp = Regex.Split(result,@"url =");
            string[] tmp2 = Regex.Split(tmp[1].Trim(),@"""");
            string Url = tmp2[0];
            request = new HttpRequestMessage(HttpMethod.Get,Url);
            
            request = AddHeadersLogin(request);
            response = h.SendAsync(request).Result;
            result = response.Content.ReadAsStringAsync().Result;
            var lines = Regex.Split(result, @"\n").ToList();
            string LineWithText = lines.Where(l => l.IndexOf(@"input name=""__RequestVerificationToken""",StringComparison.OrdinalIgnoreCase) >= 0).FirstOrDefault();
            tmp = Regex.Split(LineWithText,@"value\=");
            tmp2 = Regex.Split(tmp[1].Trim(),@"""");
            string RequestValidationToken = tmp2[1];
            string ReturnUrl = request.RequestUri.Query.Replace("?ReturnUrl=","");
            
            request = new HttpRequestMessage(HttpMethod.Post, request.RequestUri.AbsoluteUri);
            string RequestBody = String.Format(@"__RequestVerificationToken={0}&UserName={1}&Password={2}&ReturnUrl={3}", RequestValidationToken, username, password,ReturnUrl);
            request = AddHeadersLogin(request);
            request.Content = new StringContent(RequestBody);
            var MediaType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");
            MediaType.CharSet = "utf-8";
            request.Content.Headers.ContentType = MediaType;
            request.Headers.Add("Origin", "https://login.medicover.pl");
            request.Headers.Add("Referer", request.RequestUri.AbsoluteUri);
            response = h.SendAsync(request).Result;
            result = response.Content.ReadAsStringAsync().Result;
            HtmlDocument htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(result);
            var node = htmlDoc.DocumentNode.SelectSingleNode("//form");
            //var nodes = node.SelectNodes(".//input[@name]");
            
            request = new HttpRequestMessage(HttpMethod.Post, "https://oauth.medicover.pl/signin-oidc");
            RequestBody = String.Format(@"code={0}&id_token={1}&scope={2}&state={3}&session_state={4}", 
            node.SelectNodes(".//input[@name='code']").FirstOrDefault().Attributes["value"].Value,
            node.SelectNodes(".//input[@name='id_token']").FirstOrDefault().Attributes["value"].Value,
            node.SelectNodes(".//input[@name='scope']").FirstOrDefault().Attributes["value"].Value,
            node.SelectNodes(".//input[@name='state']").FirstOrDefault().Attributes["value"].Value,
            node.SelectNodes(".//input[@name='session_state']").FirstOrDefault().Attributes["value"].Value
            );
            request = AddHeadersLogin(request);
            request.Content = new StringContent(RequestBody);
            request.Content.Headers.ContentType = MediaType;
            //request.Headers.Add("Origin", "https://login.medicover.pl");
            //request.Headers.Add("Referer", request.RequestUri.AbsoluteUri);
            response = h.SendAsync(request).Result;
            result = response.Content.ReadAsStringAsync().Result;
            
            htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(result);
            node = htmlDoc.DocumentNode.SelectSingleNode("//form");
            //var nodes = node.SelectNodes(".//input[@name]");
            request = new HttpRequestMessage(HttpMethod.Post, "https://mol.medicover.pl/Medicover.OpenIdConnectAuthentication/Account/OAuthSignIn");
            RequestBody = String.Format(@"code={0}&id_token={1}&scope={2}&state={3}&session_state={4}", 
            node.SelectNodes(".//input[@name='code']").FirstOrDefault().Attributes["value"].Value,
            node.SelectNodes(".//input[@name='id_token']").FirstOrDefault().Attributes["value"].Value,
            node.SelectNodes(".//input[@name='scope']").FirstOrDefault().Attributes["value"].Value,
            node.SelectNodes(".//input[@name='state']").FirstOrDefault().Attributes["value"].Value,
            node.SelectNodes(".//input[@name='session_state']").FirstOrDefault().Attributes["value"].Value
            );
            request = AddHeadersLogin(request);
            request.Content = new StringContent(RequestBody);
            request.Content.Headers.ContentType = MediaType;
            //request.Headers.Add("Origin", "https://login.medicover.pl");
            //request.Headers.Add("Referer", request.RequestUri.AbsoluteUri);
            response = h.SendAsync(request).Result;
            result = response.Content.ReadAsStringAsync().Result;
             RefreshMain();
            string Bbb = "";


        }
        public void Login_old(string username, string password)
        {
            handler = new HttpClientHandler();
            handler.AutomaticDecompression = System.Net.DecompressionMethods.GZip;
            h = new HttpClient(handler);
            h.Timeout = TimeSpan.FromSeconds(120);
            
            //zaczynamy .
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, "https://mol.medicover.pl/Users/Account/LogOn?ReturnUrl=%2F");
            request = AddHeadersLogin(request);
            HttpResponseMessage response = h.SendAsync(request).Result;
            
            //przekierowało nas do formularza z logowaniem
            string result = response.Content.ReadAsStringAsync().Result;
            var lines = Regex.Split(result, @"\n").ToList();
            //szukanie i parsowanie formularza by wyciągnąć dane, do kolejnego etapu
            string idsrvline = lines.Where(l => l.IndexOf("idsrv.xsrf") >= 0).FirstOrDefault();
            if (idsrvline != null)
            {
                idsrvline = idsrvline.Replace("&quot;", @"""");

                string[] tmp1 = Regex.Split(idsrvline, "application/json'>");
                string[] tmp2 = Regex.Split(tmp1[1], "</script>");
                dynamic test = (JsonConvert.DeserializeObject<dynamic>(tmp2[0]));
                string xsrf = test.antiForgery.value;
                
                //mamy token/hash , dodajemy użytkownika, hasło by wysłać dalej
                string RequestBody = String.Format(@"idsrv.xsrf={0}&username={1}&password={2}", xsrf, username, password);
                request = new HttpRequestMessage(HttpMethod.Post, request.RequestUri.AbsoluteUri);
                LoginUrl = request.RequestUri.AbsoluteUri;
                request = AddHeadersLogin(request);
                request.Headers.Add("Origin", "https://oauth.medicover.pl");
                request.Headers.Add("Referer", request.RequestUri.AbsoluteUri);
                request.Content = new StringContent(RequestBody);
                var MediaType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");
                MediaType.CharSet = "utf-8";
                request.Content.Headers.ContentType = MediaType;
                response = h.SendAsync(request).Result;

                //formularz logowania wysłany , w odpowiedzi szukamy danych do kolejnego etapu
                result = response.Content.ReadAsStringAsync().Result;     
                lines = Regex.Split(result, @"\n").ToList();
                string tmpline = lines.Where(l => l.IndexOf(@"name=""code""") >= 0).FirstOrDefault();
                tmp1 = Regex.Split(tmpline, @"value=""");
                tmp2 = Regex.Split(tmp1[1], @"""");
                string code = tmp2[0];
                tmpline = lines.Where(l => l.IndexOf(@"name=""id_token""") >= 0).FirstOrDefault();
                tmp1 = Regex.Split(tmpline, @"value=""");
                tmp2 = Regex.Split(tmp1[1], @"""");
                string id_token = tmp2[0];
                tmpline = lines.Where(l => l.IndexOf(@"name=""scope""") >= 0).FirstOrDefault();
                tmp1 = Regex.Split(tmpline, @"value=""");
                tmp2 = Regex.Split(tmp1[1], @"""");
                string scope = tmp2[0];
                tmpline = lines.Where(l => l.IndexOf(@"name=""state""") >= 0).FirstOrDefault();
                tmp1 = Regex.Split(tmpline, @"value=""");
                tmp2 = Regex.Split(tmp1[1], @"""");
                string state = tmp2[0];
                tmpline = lines.Where(l => l.IndexOf(@"name=""session_state""") >= 0).FirstOrDefault();
                tmp1 = Regex.Split(tmpline, @"value=""");
                tmp2 = Regex.Split(tmp1[1], @"""");
                string session_state = tmp2[0];
                
                //budujemy treść do wysłania dalej, na podstawie danych zebranych wyżej
                RequestBody = String.Format(@"code={0}&id_token={1}&scope={2}&state={3}&session_state={4}", code, id_token, scope, state, session_state);
                request = new HttpRequestMessage(HttpMethod.Post, "https://mol.medicover.pl/Medicover.OpenIdConnectAuthentication/Account/OAuthSignIn");
                request = AddHeadersLogin(request);
                request.Headers.Add("Origin", "https://oauth.medicover.pl");
                request.Headers.Add("Referer", request.RequestUri.AbsoluteUri);
                request.Content = new StringContent(RequestBody);
                MediaType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");
                MediaType.CharSet = "utf-8";
                request.Content.Headers.ContentType = MediaType;
                response = h.SendAsync(request).Result;
                result = response.Content.ReadAsStringAsync().Result;

                //powinnismy być zalogowani, pobierzmy witrynę główną jeszcze raz. 
                RefreshMain();
            }

        }
    }
}
