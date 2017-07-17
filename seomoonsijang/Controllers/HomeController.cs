using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Blob;
using seomoonsijang.Models;
using Microsoft.WindowsAzure.Storage.Table;
using SendGrid;
using SendGrid.Helpers.Mail;
using System.Threading.Tasks;

namespace seomoonsijang.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "서문시장의 건물별 위치와 이름을 확인하실 수 있습니다.";

            return View();
        }

        [HttpGet]
        public ActionResult Contact()
        {
            //var msg = new SendGridMessage();

            //msg.SetFrom(new EmailAddress("seopjjang@gmail.com", "MySelf"));

            //var recipients = new List<EmailAddress>
            //{
            //    new EmailAddress("lsaforever0217@gmail.com", "lsaforever"),
            //    new EmailAddress("csnvshs@naver.com","navercsnvshs"),
            //    new EmailAddress("novar-sonic@daum.net","novardaum")
            //};
            //msg.AddTos(recipients);

            //msg.SetSubject("Testing the SendGrid C# Library");

            //msg.AddContent(MimeType.Text, "Hello World plain text!");
            //msg.AddContent(MimeType.Html, "<p>Hello World!</p>");
            //var apikey = System.Environment.GetEnvironmentVariable("SENDGRID_APIKEY");
            //var client = new SendGridClient(apikey);
            //var response = client.SendEmailAsync(msg);


            //ViewBag.Message = System.Environment.GetEnvironmentVariable("SENDGRID_APIKEY");

            return View();
        }
        [HttpPost]
        public ActionResult Contact(HttpPostedFileBase file, ContentsEntity contents)
        {

            if (file != null & contents != null)
            {
                CloudStorageAccount storageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("MS_AzureStorageAccountConnectionString"));

                CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
                CloudBlobContainer container = blobClient.GetContainerReference("blob1");
                ViewBag.BlobSuccess = container.CreateIfNotExists();

                CloudTableClient tableClient = storageAccount.CreateCloudTableClient();
                CloudTable table = tableClient.GetTableReference("WestGateMarket");
                ViewBag.TableSuccess = table.CreateIfNotExists();
                contents.PartitionKey = "파티션키1";
                contents.RowKey = "로우키1";
                contents.filename = file.FileName;
                TableOperation insertOperation = TableOperation.Insert(contents);
                TableResult result = table.Execute(insertOperation);
                ViewBag.TableName = table.Name;
                ViewBag.Result = result.HttpStatusCode;

                CloudBlockBlob blob = container.GetBlockBlobReference(file.FileName);
                blob.UploadFromStream(file.InputStream);
                ViewBag.Message = file.FileName;
                               
            }
            else if(file == null & contents != null)
            {
                CloudStorageAccount storageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("MS_AzureStorageAccountConnectionString"));
                CloudTableClient tableClient = storageAccount.CreateCloudTableClient();
                CloudTable table = tableClient.GetTableReference("WestGateMarket");
                ViewBag.TableSuccess = table.CreateIfNotExists();
                contents.PartitionKey = "파티션키";
                contents.RowKey = "로우키";
                TableOperation insertOperation = TableOperation.Insert(contents);
                TableResult result = table.Execute(insertOperation);
                ViewBag.TableName = table.Name;
                ViewBag.Result = result.HttpStatusCode;
                ViewBag.Message = "PartitionKey : " + contents.PartitionKey + "RowKey : " + contents.RowKey + "Text : " + contents.text;
            }
            else
            {
                
            }
            
            return View();
        }
        
    }
}