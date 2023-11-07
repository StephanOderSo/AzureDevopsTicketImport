//using Microsoft.AspNetCore.JsonPatch;
using Microsoft.TeamFoundation.Build.WebApi;
using Microsoft.TeamFoundation.Test.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using Microsoft.VisualStudio.Services.WebApi.Patch;
using Microsoft.VisualStudio.Services.WebApi.Patch.Json;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace AzureDevopsWorkItemImport
{
    internal class CreateTicket
    {
        private HttpClient client;
        private List<ADIssue> adIssueList = new List<ADIssue>();

        private string token = "";
        private const string ORG = "ISEFPWManager";
        private const string PROJECT = "Sandbox";
        private const string WIT_TYPE_TASK = "Task";
        private const string WIT_TYPE_ISSUE = "Issue";

        #region Constructor and co.
        public CreateTicket(string token, List<CsvEntry> csvEntryList)
        {
            this.token = token;
            client = new HttpClient();

            client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
            InitializeADIssueList(csvEntryList);
        }

        /// <summary>
        /// Converts CSV Elements to Work Items
        /// </summary>
        /// <param name="csvEntryList"></param>
        private void InitializeADIssueList(List<CsvEntry> csvEntryList)
        {
            string br = "<br>";
            // for every dialogmase => one ADIssue
            List<string> dialogmasken = csvEntryList.Select(x => x.Dialogmaske).Distinct().ToList();

            foreach (string dialogmaskeName in dialogmasken)
            {
                ADIssue adIssue = new ADIssue() { Title = dialogmaskeName };

                List<CsvEntry> childList = csvEntryList.Where(x => x.Dialogmaske == dialogmaskeName).ToList();

                //Add Task Children
                foreach (CsvEntry childEntry in childList)
                {
                    ADTask task = new ADTask();
                    task.Title = childEntry.Beschreibung;

                    StringBuilder sb = new StringBuilder();
                    sb.AppendLine("Beschreibung:"+br);
                    sb.AppendLine(childEntry.Beschreibung + br);

                    sb.AppendLine(br);
                    sb.AppendLine("Label:" + br);
                    sb.AppendLine(childEntry.Label + br);

                    if (!string.IsNullOrEmpty(childEntry.Constraint))
                    {
                        sb.AppendLine(br);
                        sb.AppendLine("Constraint:" + br);
                        sb.AppendLine(childEntry.Constraint + br);
                        sb.AppendLine("Constrainttype:" + br);
                        sb.AppendLine(childEntry.Constrainttype + br);
                    }

                    if (!string.IsNullOrEmpty(childEntry.Hinweis))
                    {
                        sb.AppendLine(br);
                        sb.AppendLine("Hinweis:" + br);
                        sb.AppendLine(childEntry.Hinweis + br);
                    }

                    if (!string.IsNullOrEmpty(childEntry.Eingabedatentyp))
                    {
                        sb.AppendLine(br);
                        sb.AppendLine("Eingabedatentyp:" + br);
                    }

                    if (!string.IsNullOrEmpty(childEntry.Ausgabedatentyp))
                    {
                        sb.AppendLine(childEntry.Eingabedatentyp + br);
                        sb.AppendLine("Ausgabedatentyp:" + br);
                        sb.AppendLine(childEntry.Ausgabedatentyp + br);
                    }

                    task.Description = sb.ToString();

                    adIssue.TaskList.Add(task);
                }
                
                adIssueList.Add(adIssue);
            }
        }
        #endregion

        public void SynItems()
        {
            int taskCounter = 0;
            foreach (ADIssue issue in adIssueList)
            {
                if (!CreateWorkItem(issue))
                    Console.WriteLine($"Failure while creating Issue '{issue.Title}'");
                else
                {
                    foreach (ADTask task in issue.TaskList)
                    {
                        if (!CreateWorkItem(task, issue.Id))
                            Console.WriteLine($"Failure while creating Task '{task.Title}'");
                        else
                            taskCounter++;
                    }
                    Console.WriteLine($"Issue successfully synchronised.{Environment.NewLine}IssueID: {issue.Id}.{Environment.NewLine}Tasks: {taskCounter}/{issue.TaskList.Count}");
                    taskCounter = 0;
                }
            }
        }

        private bool CreateWorkItem(WorkItem workItem, int? parentId = null)
        {
            string workitemType = string.Empty;
            if (workItem.GetType() == typeof(ADIssue)) workitemType = WIT_TYPE_ISSUE;
            if (workItem.GetType() == typeof(ADTask)) workitemType = WIT_TYPE_TASK;

            Uri uri = new Uri($"https://dev.azure.com/{ORG}");
            VssBasicCredential credentials = new VssBasicCredential("", token);
            JsonPatchDocument patchDocument = new JsonPatchDocument();

            patchDocument.Add(new JsonPatchOperation()
            {
                Operation = Operation.Add,
                Path = "/fields/System.Title",
                //max 128 chars
                Value = workItem.Title.Count()<=127? workItem.Title : workItem.Title.Remove(127)
            });

            if(workItem.GetType() == typeof(ADTask))
            {
                patchDocument.Add(new JsonPatchOperation()
                {
                    Operation = Operation.Add,
                    Path = "/fields/System.Description",
                    Value = ((ADTask)workItem).Description
                });

                patchDocument.Add(new JsonPatchOperation()
                {
                    Operation = Operation.Add,
                    Path = "/relations/-",
                    Value = new
                    {
                        rel = "System.LinkTypes.Hierarchy-Reverse",
                        url = $"https://dev.azure.com/{ORG}/_apis/wit/workItems/{parentId}"
                    }
                });
            }

            VssConnection connection = new VssConnection(uri, credentials);
            WorkItemTrackingHttpClient workItemTrackingHttpClient = connection.GetClient<WorkItemTrackingHttpClient>();

            try
            {
                Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItem result = workItemTrackingHttpClient.CreateWorkItemAsync(patchDocument, PROJECT, workitemType).Result;

                workItem.Id = result.Id ?? -1;
                return workItem.Id == -1 ? false : true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }
    }
}