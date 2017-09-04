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
            TableOperation retrieveOperation = TableOperation.Retrieve<VisitCounter>(KRT.Year.ToString() + "년" + KRT.Month.ToString() + "월" + KRT.Day.ToString() + "일", KRT.Hour.ToString() + "시");

            // Execute the retrieve operation.
            TableResult retrievedResult = table.Execute(retrieveOperation);
            VisitCounter retrievedEntity = (VisitCounter)retrievedResult.Result;
            if(retrievedEntity == null)
            {
                VisitCounter newVisitor = new VisitCounter();
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
