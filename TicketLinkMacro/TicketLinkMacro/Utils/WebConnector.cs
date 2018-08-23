using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Linq;
using Newtonsoft.Json;
using System.Text;

namespace TicketLinkMacro.Utils
{
    public class WebConnector
    {
        private readonly BackgroundWorker _worker;
        private bool _restart = false;

        public WebConnector()
        {
            _worker = new BackgroundWorker();
            _worker.WorkerReportsProgress = true;
            _worker.WorkerSupportsCancellation = true;
        }

        public T CallAPI<T>(string baseAddress, string requestUri, string cookie = null)
        {
            Uri uri = new Uri(baseAddress);

            HttpClientHandler handler = new HttpClientHandler();
            if (cookie != null)
            {
                handler.CookieContainer = new CookieContainer();
                handler.CookieContainer.Add(uri, CookieParser.MakeCookieCollection(cookie));
            }

            HttpClient client = new HttpClient(handler);
            client.BaseAddress = uri;
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            HttpResponseMessage response = client.GetAsync(requestUri).Result; 
            if (response.IsSuccessStatusCode)
            {
                var results = response.Content.ReadAsAsync<T>().Result;
                Messenger.Instance.Send("", Context.PROGRESS_DESC);
                return results;
            }
            else
            {
                Messenger.Instance.Send($"{(int)response.StatusCode} ({response.ReasonPhrase})", Context.PROGRESS_DESC);
                return default(T);
            }
        }

        public string CallHtmlAPI(string baseAddress, string requestUri, string cookie = null)
        {
            Uri uri = new Uri(baseAddress);

            HttpClientHandler handler = new HttpClientHandler();
            if (cookie != null)
            {
                handler.CookieContainer = new CookieContainer();
                handler.CookieContainer.Add(uri, CookieParser.MakeCookieCollection(cookie));
            }

            HttpClient client = new HttpClient(handler);
            client.BaseAddress = uri;
            
            HttpResponseMessage response = client.GetAsync(requestUri).Result;
            if (response.IsSuccessStatusCode)
            {
                var results = response.Content.ReadAsStringAsync().Result;
                Messenger.Instance.Send("", Context.PROGRESS_DESC);
                return results;
            }
            else
            {
                Messenger.Instance.Send($"{(int)response.StatusCode} ({response.ReasonPhrase})", Context.PROGRESS_DESC);
                return null;
            }
        }

        public ReturnType CallPostAPI<PostDataType, ReturnType>(string baseAddress, string requestUri, PostDataType postData, string cookie = null)
        {
            Uri uri = new Uri(baseAddress);

            HttpClientHandler handler = new HttpClientHandler();
            if (cookie != null)
            {
                handler.CookieContainer = new CookieContainer();
                handler.CookieContainer.Add(uri, CookieParser.MakeCookieCollection(cookie));
            }

            HttpClient client = new HttpClient(handler);
            client.BaseAddress = uri;
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            
            var content = new StringContent(typeof(PostDataType) != typeof(string) ? JsonConvert.SerializeObject(postData) : postData.ToString(), Encoding.UTF8, "application/json");
            HttpResponseMessage response = client.PostAsync(requestUri, content).Result;  // 호출 블록킹!
            if (response.IsSuccessStatusCode)
            {
                // 응답 본문 파싱. 블록킹!
                var results = response.Content.ReadAsAsync<ReturnType>().Result;
                return results;
            }
            else
            {
                return default(ReturnType);
            }
        }

        public void CallAPIAsync<T>(string baseAddress, string requestUri, int time, Action<T> progressChanged, string cookie = null)
        {
            Uri uri = new Uri(baseAddress);

            HttpClientHandler handler = new HttpClientHandler();
            if(cookie != null)
            {
                handler.CookieContainer = new CookieContainer();
                handler.CookieContainer.Add(uri, CookieParser.MakeCookieCollection(cookie));
            }

            HttpClient client = new HttpClient(handler);
            client.BaseAddress = uri;
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            _worker.DoWork +=
                    (sender, e) => 
                    {
                        int count = 0;
                        while(true)
                        {
                            if(_worker.CancellationPending)
                            {
                                e.Cancel = true;
                                return;
                            }
                            else
                            {
                                Thread.Sleep(time);
                                Messenger.Instance.Send("Processing...", Context.PROGRESS_DESC);

                                HttpResponseMessage response = client.GetAsync(requestUri).Result;

                                try
                                {
                                    T result = response.Content.ReadAsAsync<T>().Result;
                                    _worker.ReportProgress(++count, result);
                                }
                                catch(Exception ex)
                                {
                                    Messenger.Instance.Send(ex.Message, Context.PROGRESS_DESC);
                                }
                            }
                        }
                    };

            _worker.ProgressChanged += (sender, e) => { progressChanged((T)e.UserState); };

            _worker.RunWorkerCompleted += 
                    (sender, e) => 
                    {
                        if (e.Cancelled)
                        {
                            Messenger.Instance.Send("Call API Async Cancelled.", Context.PROGRESS_DESC);
                        }
                        else if (e.Error != null)
                        {
                            Messenger.Instance.Send("Call API Async Exception Thrown.", Context.PROGRESS_DESC);
                            _restart = true;
                        }
                        else
                        {
                            Messenger.Instance.Send("Call API Async Finished.", Context.PROGRESS_DESC);
                        }

                        if (_restart)
                        {
                            _worker.RunWorkerAsync();
                            _restart = false;
                        }
                        else
                        {
                            _worker.Dispose();
                        }
                    };

             _worker.RunWorkerAsync();
        }

        public void CancelAsyncCall()
        {
            if (_worker.IsBusy)
            {
                _worker.ReportProgress(100, null);
                _worker.CancelAsync();
            }
        }
    }
}
