using Microsoft.WindowsAzure.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using Microsoft.WindowsAzure.Storage.Table;
using System.IO;
using System.Xml.Serialization;

namespace AzureTableStorageSample
{
    class Program
    {
        static void Main(string[] args)
        {
            //  Loads an xml file containing sample HR records.
            List<Employee> emps = TestData.GetData();

            //  Using the connection manager, load the connectionstring from the app.config in this project
            string con = ConfigurationManager.ConnectionStrings["AzureConnection"].ConnectionString;

            // Create a Storage client object from connection string
            var storeClient = CloudStorageAccount.Parse(con);

            // Create a table client to deal with Azure Table storage
            var tableClient = storeClient.CreateCloudTableClient();

            //  Create a reference to the table called "Employees"  
            //  TABLE NEED NOT EXIST
            var myTable = tableClient.GetTableReference("Employees");

            //  If the table already exists...
            if (myTable.Exists())
            {
                //  let's delete all of the records from the last run of this demo
                TableQuery qry = new TableQuery(); // A blank TableQuery = no where clause

                var allrecords = myTable.ExecuteQuery(qry);
                foreach (var rec in allrecords)
                {
                    myTable.Execute(TableOperation.Delete(rec));
                }
            }

            // New Way  - reduces transactions
            TableBatchOperation batch = null;

            //  All items added using Batch operations must contain the exact same partition key
            //  So group these by the first character of the last name (which is our partition for this demo)
            var ItemsGroupedByBatch = emps.GroupBy(e => e.PartitionKey);
            foreach (var group in ItemsGroupedByBatch)
            {
                //  for each group, start a new batch and add each one
                batch = new TableBatchOperation();
                foreach (var emp in group)
                {
                    batch.InsertOrReplace(emp);
                }
                myTable.ExecuteBatch(batch);
            }

            //  Select * from Employees
            TableQuery allrecs = new TableQuery();
            var allItems = myTable.ExecuteQuery(allrecs);

            //  Select * from Employees where LastName = 'Bartowski'
            TableQuery<Employee> query = new TableQuery<Employee>().Where(TableQuery.GenerateFilterCondition("LastName", QueryComparisons.Equal, "Bartowski"));
            var items = myTable.ExecuteQuery<Employee>(query).ToList();

            //  Count the number of records.
            query = new TableQuery<Employee>();
            var cnt = myTable.ExecuteQuery<Employee>(query).Count();

            // I want just the first and last name for folks whose position = "Green Shirt"
            var CustomQuery = new TableQuery<DynamicTableEntity>()
                .Where(TableQuery.GenerateFilterCondition("Position", QueryComparisons.Equal, "Green Shirt"))
                .Select(new string[] { "FirstName", "LastName" });
            var projection = myTable.ExecuteQuery(CustomQuery);

            //  Give me only folks who have been terminated
            //  artificial datetimeoffset...kinda yucky.
            query = new TableQuery<Employee>().Where(TableQuery.GenerateFilterConditionForDate("TerminationDate", QueryComparisons.GreaterThan, DateTimeOffset.Now.AddYears(-85)));
            var terms = myTable.ExecuteQuery<Employee>(query);
            //  END OLD WAY
            Console.ReadLine();
        }
    }
}

