namespace AzureDevopsWorkItemImport
{
    internal class Program
    {
        static void Main(string[] args)
        {
            ReadCsv readCsv;
            CreateTicket createTicket;
            List<CsvEntry> csvEntryList;

            Console.WriteLine("CSV file path:");
            string filepath = Console.ReadLine() ?? "";


            if (!File.Exists(filepath))
            {
                Console.WriteLine($"File not found: {filepath}");
                return;
            }


            readCsv = new();
            csvEntryList = readCsv.Read(filepath);

            if (csvEntryList == null || csvEntryList == default(List<CsvEntry>))
                Console.WriteLine($"No CSV entries where readed");

            Console.WriteLine("Enter Azure Devops Access Token:");
            string token = Console.ReadLine() ?? "";


            createTicket = new CreateTicket(token);
            createTicket.Create();
        }
    }
}