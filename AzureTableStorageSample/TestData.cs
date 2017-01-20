using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System;
using System.Xml.Serialization;

namespace AzureTableStorageSample
{
    public static class TestData
    {
        public static List<Employee> GetData()
        {
            List<Employee> returnval = null;
            var strm = new FileStream(@"Content\BuyMoria.xml", FileMode.Open);
            var xml = new XmlSerializer(typeof(List<Employee>));
            returnval = xml.Deserialize(strm) as List<Employee>;
            strm.Close();
            return returnval;
        }
    }
}
