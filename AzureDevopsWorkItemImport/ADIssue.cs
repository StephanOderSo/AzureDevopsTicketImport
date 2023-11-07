using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureDevopsWorkItemImport
{
    internal abstract class WorkItem
    {
        public abstract int Id { get; set; }
        public abstract string Title { get; set; }
    }

    internal class ADIssue : WorkItem
    {
        public override int Id { get; set; } = -1;
        public override string Title { get; set; } = string.Empty;
        public List<ADTask> TaskList { get; set; } = new List<ADTask>();
    }

    internal class ADTask : WorkItem
    {
        public override int Id { get; set; } = -1;
        public override string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }

}
