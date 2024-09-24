

using System.Data.SqlTypes;
using System.Xml.Linq;

namespace BlobStorageSample
{
    public static class BlobStorageSample
    {
        static string StorageAccountName = "sampleaz204";
        static string StorageAccountKey = "UBygAxdWVnm6mxyWiuziiz8BTGGCf6D2UPaG2DGMWGVmxrb6LATdLiAAW9x2mzV9WhNF4tz+pqDr+AStDR++Tw==";

        public static void Main()
        {

            ///////////////////////// Get blob account properties in a storage account.
            // var xmlString = AccountOperations.GetBlobServicePropertiesAsyncREST(StorageAccountName, StorageAccountKey, CancellationToken.None).GetAwaiter().GetResult();
            // XElement x = XElement.Parse(xmlString);

            // XElement? logging = x.Element("Logging");

            // Console.WriteLine("=======> Logging properties");
            // Console.WriteLine("Read = {0}", logging?.Element("Read")?.Value ?? string.Empty);
            // Console.WriteLine("Write = {0}", logging?.Element("Write")?.Value ?? string.Empty);
            // Console.WriteLine("Delete = {0}", logging?.Element("Delete")?.Value ?? string.Empty);
            // Console.WriteLine();


            //XElement? hourMetrics = x.Element("HourMetrics");
            // Console.WriteLine("=======> HourMetrics properties");
            // Console.WriteLine("Enabled = {0}", hourMetrics?.Element("Enabled")?.Value ?? string.Empty);
            // Console.WriteLine("IncludeAPIs = {0}", hourMetrics?.Element("IncludeAPIs")?.Value ?? string.Empty);
            // Console.WriteLine();


            // XElement? minuteMetrics = x.Element("MinuteMetrics");
            // Console.WriteLine("=======> MinuteMetrics properties");
            // Console.WriteLine("Enabled = {0}", minuteMetrics?.Element("Enabled")?.Value ?? string.Empty);
            // Console.WriteLine();

            ///////////////////////// Get Account Information in a storage account.
            //var httpResponseHeaders = AccountOperations.GetAccountInformationAsyncREST(StorageAccountName, StorageAccountKey, CancellationToken.None).GetAwaiter().GetResult();
            //Console.WriteLine("=======> Account Information");
            //Console.WriteLine("x-ms-sku-name = {0}", httpResponseHeaders?.GetValues("x-ms-sku-name")?.FirstOrDefault() ?? string.Empty);
            //Console.WriteLine("x-ms-account-kind = {0}", httpResponseHeaders?.GetValues("x-ms-account-kind")?.FirstOrDefault() ?? string.Empty);
            //Console.WriteLine();

            ///////////////////////// Create a container in a storage account.
            //bool wasCreated = AccountOperations.CreateContainerAsyncREST(StorageAccountName, StorageAccountKey, CancellationToken.None, "images").GetAwaiter().GetResult();
            //Console.WriteLine(wasCreated ? "Images container created" : "error");

            //wasCreated = AccountOperations.CreateContainerAsyncREST(StorageAccountName, StorageAccountKey, CancellationToken.None, "files").GetAwaiter().GetResult();
            //Console.WriteLine(wasCreated ? "Files container created" : "error");

            //wasCreated = AccountOperations.CreateContainerAsyncREST(StorageAccountName, StorageAccountKey, CancellationToken.None, "videos").GetAwaiter().GetResult();
            //Console.WriteLine(wasCreated ? "Videos container created" : "error");


            ///////////////////////// List the containers in a storage account.
            var xmlString = AccountOperations.ListContainersAsyncREST(StorageAccountName, StorageAccountKey, CancellationToken.None).GetAwaiter().GetResult();

            XElement? containers = XElement.Parse(xmlString);
            Console.WriteLine("Containers:");
            foreach (XElement? container in containers?.Element("Containers")?.Elements("Container") ?? new List<XElement>())
            {
                Console.WriteLine(container?.Element("Name")?.Value ?? string.Empty);
            }


            Console.WriteLine("Press any key to continue.");
            Console.ReadLine();
        }

        
    }
}