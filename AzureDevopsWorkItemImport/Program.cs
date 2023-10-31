namespace AzureDevopsWorkItemImport
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string filepath = "C:/Users/s.bienhuels/Desktop/Allgemein/Book1.csv";
            ReadCsv readCsv = new ReadCsv();
            CreateTicket createTicket = new CreateTicket();
            List<CsvEntry> csvEntries = new List<CsvEntry>();
            if (File.Exists(filepath))
            {
                csvEntries = readCsv.Read(filepath);
                createTicket.Create();
            }

            else
                Console.WriteLine($"File not found: {filepath}");


        }
    }
}