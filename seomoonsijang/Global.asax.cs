using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using seomoonsijang.DataObjects;
using System;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace seomoonsijang
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }

        protected void Session_Start(object sender, EventArgs e)
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("MS_AzureStorageAccountConnectionString"));

            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();
            CloudTable table = tableClient.GetTableReference("VisitorCounter");
            DateTime KRT = DateTime.Now.AddHours(9);
            string rowKey = "";
            if (KRT.Hour > 0 && KRT.Hour < 10)
            {
                rowKey = "0" + KRT.Hour.ToString() + "시";
            }
            else
            {
                rowKey = KRT.Hour.ToString() + "시";
            }

            string partitionKey = KRT.Year.ToString() + "년";
            if (KRT.Month > 0 && KRT.Month < 10)
            {
                partitionKey += "0" + KRT.Month.ToString() + "월";
            }
            else
            {
                partitionKey += KRT.Month.ToString() + "월";
            }
            if (KRT.Day > 0 && KRT.Day < 10)
            {
                partitionKey += "0" + KRT.Day.ToString() + "일";
            }
            else
            {
                partitionKey += KRT.Day.ToString() + "일";
            }
            TableOperation retrieveOperation = TableOperation.Retrieve<VisitCounter>(partitionKey, rowKey);

            // Execute the retrieve operation.
            TableResult retrievedResult = table.Execute(retrieveOperation);
            VisitCounter retrievedEntity = (VisitCounter)retrievedResult.Result;
            if (retrievedEntity == null)
            {
                VisitCounter newVisitor = new VisitCounter(partitionKey, rowKey);
                TableOperation insertOperation = TableOperation.Insert(newVisitor);
                table.Execute(insertOperation);
            }
            else
            {
                retrievedEntity.Number += 1;

                TableOperation updateOperation = TableOperation.Replace(retrievedEntity);
                table.Execute(updateOperation);
            }
        }
    }
}
