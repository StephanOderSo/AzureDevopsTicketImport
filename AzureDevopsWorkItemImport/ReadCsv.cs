//using Microsoft.Office.Interop.Excel;
using CsvHelper;
using CsvHelper.Configuration;
using System;
using System.Collections.Generic;
using System.Formats.Asn1;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace AzureDevopsWorkItemImport
{
    internal class ReadCsv
    {
        public List<CsvEntry> Read(string csvFilepath)
        {
            Console.WriteLine("Let's read");

            CsvConfiguration config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                Delimiter = ";",
                Encoding = Encoding.UTF8
            };

            List<CsvEntry> entryList = new List<CsvEntry>();
            try
            {

                using (StreamReader reader = new StreamReader(csvFilepath))
                {
                    using (CsvReader csv = new(reader, config))
                    {
                        csv.Context.TypeConverterOptionsCache.GetOptions<DateTime?>().NullValues.Add("NULL");
                        csv.Read();
                        csv.ReadHeader();
                        entryList = csv.GetRecords<CsvEntry>().ToList();
                    }
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            Console.WriteLine("Reading completed");
            return entryList;
        }
    }
}
