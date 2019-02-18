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
        
        public string SendRequest (string url, string referer) {
            HttpRequestMessage request = new HttpRequestMessage( HttpMethod.Get, url);
            request = AddHeadersLogin(request);
            request.Headers.Add("Referer",referer);
            var response = h.SendAsync(request).Result; 
            string result = response.Content.ReadAsStringAsync().Result;
            return "";
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
                request.Headers.Add("Referer", LoginUrl);
                var response = h.SendAsync(request).Result;
                //result = response.Content.ReadAsStringAsync().Result;

        }
        public void Login(string username, string password)
        {
            handler = new HttpClientHandler();
            handler.AutomaticDecompression = System.Net.DecompressionMethods.GZip;
            h = new HttpClient(handler);
            h.Timeout = TimeSpan.FromSeconds(120);
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, "https://mol.medicover.pl/Users/Account/LogOn?ReturnUrl=%2F");
            request = AddHeadersLogin(request);
            HttpResponseMessage response = h.SendAsync(request).Result;
            string result = response.Content.ReadAsStringAsync().Result;
            var lines = Regex.Split(result, @"\n").ToList();
            string idsrvline = lines.Where(l => l.IndexOf("idsrv.xsrf") >= 0).FirstOrDefault();
            if (idsrvline != null)
            {
                idsrvline = idsrvline.Replace("&quot;", @"""");

                string[] tmp1 = Regex.Split(idsrvline, "application/json'>");
                string[] tmp2 = Regex.Split(tmp1[1], "</script>");
                dynamic test = (JsonConvert.DeserializeObject<dynamic>(tmp2[0]));
                string xsrf = test.antiForgery.value;
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
                RefreshMain();
            }

        }
    }
}
