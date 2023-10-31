using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureDevopsWorkItemImport
{
    internal class CreateTicket
    {
        private HttpClient client;

        private const string BASE = "https://dev.azure.com";
        private const string PAT = "";
        private const string ORG = "ISEFPWManager";
        private const string API = "api-version=7.1";
        private const string PROJECT = "Sandbox";
        private const string WIT_TYPE = "$Task";

        public void Create()
        {
            client = new HttpClient();
            client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.ASCII.GetBytes(string.Format("{0}:{1}", "", PAT))));

            string uri = String.Join("?", String.Join("/", BASE, ORG, PROJECT, "_apis/wit/workitems", WIT_TYPE), API);


            string json = "[{ \"op\": \"add\", \"path\": \"/fields/System.Title\", \"from\": null, \"value\": \"REST API Demo task\"}]";
            HttpContent content = new StringContent(json, Encoding.UTF8, "application/json-patch+json");

            string result = CreateWIT(client, uri, content).Result;



            // Pretty print the JSON if result not empty or null.
            if (!String.IsNullOrEmpty(result))
            {
                dynamic wit = JsonConvert.DeserializeObject<object>(result);
                Console.WriteLine(JsonConvert.SerializeObject(wit, Formatting.Indented));
            }

            client.Dispose();
        }



        public static async Task<string> CreateWIT(HttpClient client, string uri, HttpContent content)
        {
            try
            {
                // Send asynchronous POST request.
                using (HttpResponseMessage response = await client.PostAsync(uri, content))
                {
                    response.EnsureSuccessStatusCode();
                    return (await response.Content.ReadAsStringAsync());
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return string.Empty;
            }
        } 



    }
}
