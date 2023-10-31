//using Microsoft.AspNetCore.JsonPatch;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using Microsoft.VisualStudio.Services.WebApi.Patch;
using Microsoft.VisualStudio.Services.WebApi.Patch.Json;
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
        private string token = "";
        private const string ORG = "ISEFPWManager";
        private const string API = "api-version=7.1";
        private const string PROJECT = "Sandbox";
        private const string WIT_TYPE = "Task";

        public CreateTicket(string token)
        {
            this.token = token;
            client = new HttpClient();

            client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
            //client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.ASCII.GetBytes(string.Format("{0}:{1}", "", token))));
        }

        public bool Create()
        {
            
            string _uri = String.Join("?", String.Join("/", BASE, ORG, PROJECT, "_apis/wit/workitems", WIT_TYPE), API);

            Uri uri = new Uri($"https://dev.azure.com/{ORG}");
            VssBasicCredential credentials = new VssBasicCredential("", token);
            JsonPatchDocument patchDocument = new JsonPatchDocument();

            patchDocument.Add(new JsonPatchOperation()
            {
                Operation = /*Microsoft.VisualStudio.Services.WebApi.Patch.*/Operation.Add,
                Path = "/fields/System.Title",
                Value = "Testtitle"
            });

            VssConnection connection = new VssConnection(uri, credentials);
            WorkItemTrackingHttpClient workItemTrackingHttpClient = connection.GetClient<WorkItemTrackingHttpClient>();

            try
            {
                Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItem result = workItemTrackingHttpClient.CreateWorkItemAsync(patchDocument, PROJECT, WIT_TYPE).Result;

                return true;

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }
    }
}
