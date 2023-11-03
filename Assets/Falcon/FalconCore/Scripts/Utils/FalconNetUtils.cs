using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;

namespace Falcon.FalconCore.Scripts.Utils
{
    /// <summary>
    /// A class which is used for accessing the network, all the functions here are synchronous and may block calling thread
    /// </summary>
    public static class FalconNetUtils
    {
        private static readonly HttpClient HttpClient = new HttpClient();

        public static string DoRequest(HttpMethod requestType, string url, Dictionary<string, string> headers, string jsonBody, TimeSpan timeout = default(TimeSpan))
        {
            using (var request = new HttpRequestMessage
                   {
                       Method = requestType,
                       RequestUri = new Uri(url)
                   })
            {
                if (jsonBody != null)
                {
                    if (!jsonBody.Trim().Equals(""))
                    {
                        request.Content = new StringContent(jsonBody, Encoding.UTF8,"application/json");
                        // request.Content = new StringContent(jsonBody);
                    }
                }
                if (headers != null)
                {
                    using (var enumerator = headers.GetEnumerator())
                    {
                        while (enumerator.MoveNext())
                            request.Headers.Add(enumerator.Current.Key, enumerator.Current.Value);
                    }
                }

                CancellationTokenSource cancelToken = new CancellationTokenSource();
                if (timeout == default(TimeSpan)) timeout = TimeSpan.FromSeconds(100);
                cancelToken.CancelAfter(timeout);

                using (HttpResponseMessage response = HttpClient.SendAsync(request, cancelToken.Token).Result)
                {
                    if (response.IsSuccessStatusCode)
                    {
                        return response.Content.ReadAsStringAsync().Result;
                    }
                    else
                    {
                        throw new System.Exception(response.StatusCode + response.Content.ReadAsStringAsync().Result);
                    }
                }
            }
        }

        public static string DoRequest(HttpMethod requestType, string url, Dictionary<string, string> headers, TimeSpan timeout = default(TimeSpan))
        {
            return DoRequest(requestType, url, headers, "", timeout);
        }

        public static string DoRequest(HttpMethod requestType, string url, string jsonBody, TimeSpan timeout=  default(TimeSpan))
        {
            return DoRequest(requestType, url, new Dictionary<string, string>(), jsonBody, timeout);
        }

        public static string DoRequest(HttpMethod requestType, string url, TimeSpan timeout = default(TimeSpan))
        {
            return DoRequest(requestType, url, new Dictionary<string, string>(), null, timeout);
        }
        
        public static void GetFile(string url, string destination)
        {
            var client = new WebClient();
            client.DownloadFile(url, destination);
        }
        
        public static bool HasInternet(int timeoutMs = 10000, string url = null)
        {
            try
            {
                if (url == null) return false;
                if (CultureInfo.InstalledUICulture.Name.StartsWith("fa"))
                {
                    url = "https://www.aparat.com";
                } else if (CultureInfo.InstalledUICulture.Name.StartsWith("zh"))
                {
                    url = "https://www.baidu.com";
                }
                else
                {
                    url = "https://www.google.com";
                }

                var request = (HttpWebRequest)WebRequest.Create(url);
                request.KeepAlive = false;
                request.Timeout = timeoutMs;
                using ((HttpWebResponse)request.GetResponse()) return true;
            }
            catch
            {
                return false;
            }
        }
    }
}