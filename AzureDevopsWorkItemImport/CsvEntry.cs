using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureDevopsWorkItemImport
{
    internal class CsvEntry
    {
        public CsvEntry() { }
        public string Dialogmaske { get; set; } = string.Empty;
        public string Komponente { get; set; } = string.Empty;
        public string Label { get; set; } = string.Empty;
        public string Beschreibung { get; set; } = string.Empty;
        public string Constraint { get; set; } = string.Empty;
        public string Constrainttype { get; set; } = string.Empty;
        public string Auswertungszeitpunkt { get; set; } = string.Empty;
        public string Hinweis { get; set; } = string.Empty;
        public string Eingabedatentyp { get; set; } = string.Empty;
        public string Ausgabedatentyp { get; set; } = string.Empty;
        public string Version { get; set; } = string.Empty;
    }
}
