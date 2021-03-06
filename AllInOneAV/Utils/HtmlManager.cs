using HtmlAgilityPack;
using Microsoft.Win32;
using Model.JavModels;
using Serilog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Utils
{
    public class HtmlManager
    {
        private static string UserAgent = JavINIClass.IniReadValue("Html", "UserAgent");
        private static string Error = JavINIClass.IniReadValue("Jav", "error");
		private static readonly string DefaultUserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/90.0.4430.93 Safari/537.36";

		
		public static string GetChromeVersion()
        {
            object path;
            path = Registry.GetValue(@"HKEY_LOCAL_MACHINE\Software\Microsoft\Windows\CurrentVersion\App Paths\chrome.exe", "", null);
            if (path != null)
            {
                return FileVersionInfo.GetVersionInfo(path.ToString()).FileVersion;
            }
            else
            {
                return "";
            }
        }

        public static CookieCollection GetCookies(string url, string end = "utf-8", CookieContainer inCc = null)
        {
            CookieCollection cc = new CookieCollection();
            try
            {
                GC.Collect();
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Credentials = CredentialCache.DefaultCredentials;
                request.Timeout = 90000;
                request.UserAgent = string.Format(UserAgent, GetChromeVersion());
                request.Method = "GET";

                if (cc != null)
                {
                    request.CookieContainer = inCc;
                }

                request.KeepAlive = false;
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                cc = response.Cookies;
                response.Close();
            }
            catch (Exception e)
            {

            }

            return cc;
        }


		public static async Task<HtmlDocument> GetHtmlDocumentAsync(string requestUrl, CookieContainer cc = null, string userAgent = "", Dictionary<String, String> headers = null)
		{
			try
			{
				string content = string.Empty;
				var ccStr = string.Empty;

				if(cc != null && cc.Count > 0)
					ccStr = cc.GetCookieHeader(new Uri("http://www.javlibrary.com/cn/"));

				if(string.IsNullOrEmpty(userAgent))
					userAgent = DefaultUserAgent;
				
				using(HttpClient client = new HttpClient())
				{
					client.DefaultRequestHeaders.Add("Cookie", ccStr);
					client.DefaultRequestHeaders.Add("User-Agent", DefaultUserAgent);
					if(headers != null)
					{
						foreach(var item in headers)
							client.DefaultRequestHeaders.Add(item.Key, item.Value);
					}

					var html = await client.GetStringAsync(requestUrl);
					
					if(string.IsNullOrWhiteSpace(html) == false)
					{
						var doc = new HtmlDocument();
						doc.LoadHtml(html);
						return doc;
					}
					return null;
				}
			}
			catch(Exception ex)
			{
				return null;
			}
		}

		public static HtmlResponse GetHtmlContentViaUrl(string url, string end = "utf-8",  bool isJav = false, CookieContainer cc = null, string agent = "")
        {
            HtmlResponse res = new HtmlResponse
            {
                Success = false
            };

            HttpWebResponse response = null;

            try
            {
                GC.Collect();
                StringBuilder sb = new StringBuilder();
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Credentials = CredentialCache.DefaultCredentials;
                request.Timeout = 90000;
                request.UserAgent = string.IsNullOrEmpty(agent) ? string.Format(UserAgent, GetChromeVersion()) : agent;
                request.Method = "GET";

                if (isJav)
                {
                    request.CookieContainer = cc;
                }

                request.KeepAlive = true;
                response = (HttpWebResponse)request.GetResponse();
                Stream dataStream = response.GetResponseStream();
                StreamReader reader = new StreamReader(dataStream, Encoding.GetEncoding(end));
                //while (!reader.EndOfStream)
                //{
                //    sb.AppendLine(reader.ReadLine());
                //}
                res.Content = reader.ReadToEnd();
                reader.Close();
                dataStream.Close();
                response.Close();
            }
            catch (WebException ex)
            {
                response = (HttpWebResponse)ex.Response;
                res.Success = false;
                res.Content = "";

                if (isJav)
                {
                    Stream myResponseStream = response.GetResponseStream();
                    using (StreamReader myStreamReader = new StreamReader(myResponseStream, Encoding.UTF8))
                    {
                        res.Content = myStreamReader.ReadToEnd();
                    }
                    myResponseStream.Close();

                    foreach (Cookie cookie in response.Cookies)
                    {
                        cc.Add(cookie);
                    }
                }

                return res;
            }

            res.Success = true;
            return res;
        }

        

        public static string Post(string url, Dictionary<string, string> dic, CookieContainer cc = null)
        {
            string result = "";
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
            req.Method = "POST";
            req.ContentType = "application/x-www-form-urlencoded";
            if (cc != null)
            {
                req.CookieContainer = cc;
            }
            #region 添加Post 参数
            StringBuilder builder = new StringBuilder();
            int i = 0;
            foreach (var item in dic)
            {
                if (i > 0)
                    builder.Append("&");
                builder.AppendFormat("{0}={1}", item.Key, item.Value);
                i++;
            }
            byte[] data = Encoding.UTF8.GetBytes(builder.ToString());
            req.ContentLength = data.Length;
            using (Stream reqStream = req.GetRequestStream())
            {
                reqStream.Write(data, 0, data.Length);
                reqStream.Close();
            }
            #endregion
            HttpWebResponse resp = (HttpWebResponse)req.GetResponse();
            Stream stream = resp.GetResponseStream();
            //获取响应内容
            using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
            {
                result = reader.ReadToEnd();
            }
            return result;
        }

        public static HtmlResponse GetHtmlWebClient(string cookieHost, string url, CookieContainer cc = null, bool userProxy = false)
        {
            HtmlResponse res = new HtmlResponse
            {
                Success = false
            };

            List<string> my_headers = new List<string>{
                "Mozilla/5.0 (Windows NT 6.3; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/39.0.2171.95 Safari/537.36",
                "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_9_2) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/35.0.1916.153 Safari/537.36",
                "Mozilla/5.0 (Windows NT 6.1; WOW64; rv:30.0) Gecko/20100101 Firefox/30.0",
                "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_9_2) AppleWebKit/537.75.14 (KHTML, like Gecko) Version/7.0.3 Safari/537.75.14",
                "Mozilla/5.0 (compatible; MSIE 10.0; Windows NT 6.2; Win64; x64; Trident/6.0)" };

            try
            {
                var wc = new WebClient();

                if (cc != null)
                {
                    var c = cc.GetCookieHeader(new Uri(cookieHost));
                    wc.Headers.Add(HttpRequestHeader.Cookie, c);
                }

                if (userProxy)
                {
                    Random r = new Random();

                    var index = r.Next(my_headers.Count - 1);

                    wc.Headers.Add(HttpRequestHeader.UserAgent, my_headers[index]);

                    //WebProxy proxy = new WebProxy();
                    //proxy.UseDefaultCredentials = false;
                    //proxy.Address = new Uri("http://183.239.167.122:8080"); // new Uri("http://183.239.167.122:8080");
                    //wc.Proxy = proxy;
                }
                else
                {
                    wc.Headers.Add(HttpRequestHeader.UserAgent, string.Format(UserAgent, GetChromeVersion()));
                }

                var hltext = wc.DownloadData(url);
                res.Content = (Encoding.GetEncoding("UTF-8").GetString(hltext));

                res.Success = true;
            }
            catch (Exception ex)
            {
                res.Content = ex.ToString();
            }

            return res;
        }

        public static HtmlResponse GetHtmlWebClient(string url, CookieContainer cc = null, string cookieHost = "", string host = "", string refere = "")
        {
            HtmlResponse res = new HtmlResponse
            {
                Success = false
            };

            try
            {
                var wc = new WebClient();

                if (cc != null)
                {
                    wc.Headers.Add(HttpRequestHeader.Cookie, cc.GetCookieHeader(new Uri(cookieHost)));
                }

                wc.Headers.Add(HttpRequestHeader.UserAgent, string.Format(UserAgent, GetChromeVersion()));

                if (!string.IsNullOrEmpty(host))
                {
                    wc.Headers.Add(HttpRequestHeader.Host, host);
                }

                if (!string.IsNullOrEmpty(refere))
                {
                    wc.Headers.Add(HttpRequestHeader.Referer, refere);
                }

                var hltext = wc.DownloadData(url);
                res.Content = (Encoding.GetEncoding("UTF-8").GetString(hltext));

                res.Success = true;
            }
            catch (Exception ex)
            {
                res.Content = ex.ToString();
            }

            return res;
        }

        public static HtmlResponse GetHtmlWebClientWithRenewCC(string cookieHost, string url, CookieContainer cc = null, bool userProxy = false)
        {
            HtmlResponse res = new HtmlResponse
            {
                Success = false
            };

            List<string> my_headers = new List<string>{
                "Mozilla/5.0 (Windows NT 6.3; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/39.0.2171.95 Safari/537.36",
                "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_9_2) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/35.0.1916.153 Safari/537.36",
                "Mozilla/5.0 (Windows NT 6.1; WOW64; rv:30.0) Gecko/20100101 Firefox/30.0",
                "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_9_2) AppleWebKit/537.75.14 (KHTML, like Gecko) Version/7.0.3 Safari/537.75.14",
                "Mozilla/5.0 (compatible; MSIE 10.0; Windows NT 6.2; Win64; x64; Trident/6.0)" };

            try
            {
                var wc = new WebClient();

                if (cc != null)
                {
                    wc.Headers.Add(HttpRequestHeader.Cookie, cc.GetCookieHeader(new Uri(cookieHost)));
                }

                if (userProxy)
                {
                    Random r = new Random();
                    var index = r.Next(my_headers.Count - 1);
                    wc.Headers.Add(HttpRequestHeader.UserAgent, my_headers[index]);
                }
                else
                {
                    wc.Headers.Add(HttpRequestHeader.UserAgent, string.Format(UserAgent, GetChromeVersion()));
                }

                var hltext = wc.DownloadData(url);
                res.Content = (Encoding.GetEncoding("UTF-8").GetString(hltext));

                res.Success = true;
                res.IsExpire = false;
            }
            catch (Exception ex)
            {
                res.Content = ex.ToString();

                if (res.Content.Contains("503"))
                {
                    res.IsExpire = true;
                }
            }

            return res;
        }

        public static HtmlResponse GetHtmlLockedWebClient(string cookieHost, string url, CookieContainer cc = null)
        {
            HtmlResponse res = new HtmlResponse
            {
                Success = false
            };

            try
            {
                var wc = new WebClient();

                if (cc != null)
                {
                    wc.Headers.Add(HttpRequestHeader.Cookie, cc.GetCookieHeader(new Uri(cookieHost)));
                }

                wc.Headers.Add(HttpRequestHeader.UserAgent, string.Format(UserAgent, GetChromeVersion()));

                var hltext = wc.DownloadData(url);
                res.Content = (Encoding.GetEncoding("UTF-8").GetString(hltext));

                res.Success = true;
            }
            catch (Exception ex)
            {
                res.Content = ex.ToString();

                if (ex.ToString().Contains("503"))
                {
                    cc = null;
                }
            }

            return res;
        }
    }

    public class HtmlResponse
    {
        public bool Success { get; set; }
        public string Content { get; set; }
        public bool IsExpire { get; set; }
    }
}
