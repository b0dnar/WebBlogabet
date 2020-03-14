using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using HtmlAgilityPack;
using Serilog;

namespace WebMvcBlogabet.Services
{
    public class BlogabetWeb
    {
        private HttpClient _client;
        private readonly string _urlSite = "https://blogabet.com";

        public BlogabetWeb()
        {
            InitialClient();
        }

        private void InitialClient()
        {
            HttpClientHandler handler = new HttpClientHandler { AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate };
            _client = new HttpClient(handler);
            _client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/80.0.3987.122 Safari/537.36");
            _client.DefaultRequestHeaders.Add("Accept", "*/*");
            _client.DefaultRequestHeaders.Add("Accept-Language", "en-US;q=0.7,en;q=0.6,de;q=0.5");
            _client.DefaultRequestHeaders.Add("Connection", "keep-alive");
            _client.DefaultRequestHeaders.Add("Sec-Fetch-Dest", "empty");
            _client.DefaultRequestHeaders.Add("Sec-Fetch-Mode", "cors");
            _client.DefaultRequestHeaders.Add("Sec-Fetch-Site", "same-origin");
            _client.DefaultRequestHeaders.Add("X-Requested-With", "XMLHttpRequest");
            _client.BaseAddress = new Uri(_urlSite);
        }

        public async Task<bool> Login()
        {
            var content = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("email", "goldenevermouse@gmail.com"),
                new KeyValuePair<string, string>("password", "hg76GY!!dh89JJ"),
                new KeyValuePair<string, string>("remember-me", "1")
            });

            try
            {
                var result = await _client.PostAsync("/cp/processLogin", content);
                string resultContent = await result.Content.ReadAsStringAsync();
                Boolean.TryParse(JToken.Parse(resultContent)["result"].ToString(), out var stateLogin);
                if (stateLogin)
                {
                    var cookie = String.Join(";", result.Headers.GetValues("Set-Cookie").ToList()).Split(';').FirstOrDefault(x => x.Contains("login_string"));
                    _client.DefaultRequestHeaders.Add("Cookie", cookie);
                }

                return stateLogin;
            }
            catch (Exception e)
            {
                Log.Error(e, nameof(Login));
                return false;
            }
        }

        public async Task Logout()
        {
            await _client.GetAsync("/logout");
            _client.DefaultRequestHeaders.Remove("Cookie");
        }

        public async Task<HtmlDocument> UpdatePage()
        {
            var content = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("activetab", "alltipsters"),
                new KeyValuePair<string, string>("time_posted", "0"),
                new KeyValuePair<string, string>("firstLoad", "1")
            });
            HtmlDocument html = new HtmlDocument();

            try
            {
                var result = await _client.PostAsync("/feed/reload_feed", content);
                string resultContent = await result.Content.ReadAsStringAsync();
                html.LoadHtml(resultContent);

                return html;
            }
            catch (Exception e)
            {
                Log.Error(e, nameof(UpdatePage));
                throw;
            }
        }

        public async Task<HtmlDocument> SeeOld(string point)
        {
            var content = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("activetab", "alltipsters"),
                new KeyValuePair<string, string>("feedFrom", "0"),
                new KeyValuePair<string, string>("firstLoad", "1"),
                new KeyValuePair<string, string>("ltOrGt", point),
                new KeyValuePair<string, string>("startPoint", point)
            });
            HtmlDocument html = new HtmlDocument();

            try
            {
                var result = await _client.PostAsync("/feed/reload_feed", content);
                string resultContent = await result.Content.ReadAsStringAsync();
                html.LoadHtml(resultContent);

                return html;
            }
            catch (Exception e)
            {
                Log.Error(e, nameof(SeeOld));
                throw;
            }
        }
    }
}
