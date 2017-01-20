using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AzureTableStorageSample
{
    public class Employee : TableEntity
    {
        public int ID { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string Position { get; set; }
        public decimal Salary { get; set; }
        public DateTime? TerminationDate { get; set; }
    }
}
