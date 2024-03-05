using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using SchneiderElectric.CBMS.GBP.DAO.Interfaces.GlobalBillPay;
using SchneiderElectric.CBMS.GBP.ViewModels.GlobalBillPay;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace SE.CBMS.InvoiceReg.Business
{
    public interface IRecalServiceCallsPost
    {
        Task<string> PostCall(string url, string data);
    }

    public interface IRecalServiceCallsGet
    {
        string GetCall(string url);
    }

    public class CommonHttpClient
    {
        string baseUrl;
        public CommonHttpClient(string _baseUrl)
        {
            this.baseUrl = _baseUrl;
        }
        protected HttpClient BuildHttpClient()
        {
            HttpClient client = new HttpClient();            
            client.BaseAddress = new Uri(baseUrl);
            client.DefaultRequestHeaders.Accept.Clear();
            client.Timeout = Timeout.InfiniteTimeSpan;
            return client;
        }
    }
    public class RecalcServicePost: CommonHttpClient, IRecalServiceCallsPost
    {
        private readonly string ErrorFilePath;
        public RecalcServicePost(string baseUrl, IConfiguration config): base(baseUrl)
        {
            ErrorFilePath = config.GetSection("AppSettings").GetSection("ErrorFilePath").Value;
        }

        public async Task<string> PostCall(string url, string data)
        {
            string responseBody = "";
            HttpResponseMessage response;
            var contentData = new StringContent(data, System.Text.Encoding.UTF8, "application/json");
            using (var client = BuildHttpClient())
            {
                try
                {
                    response = await client.PostAsync(url, contentData).ConfigureAwait(false);
                    responseBody = response.Content.ReadAsStringAsync().Result;
                    if (!response.IsSuccessStatusCode)
                    {
                        throw new Exception("Request failed.");
                    }
                }
                catch (Exception ex)
                {
                    var count = GetRetryFromErrorsList(ex.Message);
                   if (count > 0)
                   {
                      for (int i = 0; i < count; i++)
                      {
                         try
                         {
                             response = await client.PostAsync(url, contentData).ConfigureAwait(false);
                             responseBody = response.Content.ReadAsStringAsync().Result;
                                if (!response.IsSuccessStatusCode)
                                {
                                    throw new Exception("Request failed.");
                                }
                            }
                         catch (Exception)
                         {
                            continue;
                         }
                         break;
                      }
                   }               
                   
                }
                return responseBody;
            }

        }
        public int GetRetryFromErrorsList(string msg)
        {
            int retrycount = 0;
            List<BatchErrors> ErrorList = new List<BatchErrors>();
            string errorFilePath = ErrorFilePath;
            StreamReader r = new StreamReader(errorFilePath);
            var jsonString = r.ReadToEnd();
            ErrorList = JsonConvert.DeserializeObject<List<BatchErrors>>(jsonString);
            var errMsgcheck = ErrorList.Where(x => x.ErrorMessage.Contains(msg)).FirstOrDefault();
            if ((errMsgcheck != null))
                retrycount = errMsgcheck.RetryCount;
            return retrycount;

        }
    
    }

    public class RecalcServiceGet : CommonHttpClient, IRecalServiceCallsGet
    {
        public RecalcServiceGet(string baseUrl) : base(baseUrl)
        {

        }

        public string GetCall(string url)
        {
            string responseBody = "";
            using (var client = BuildHttpClient())
            {
                HttpResponseMessage response = client.GetAsync(url).Result;
                responseBody = response.Content.ReadAsStringAsync().Result;
            }
            return responseBody;
        }
    }
}
