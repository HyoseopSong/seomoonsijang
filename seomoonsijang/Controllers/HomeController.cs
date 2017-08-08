﻿using System;
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
using System.Drawing;
using System.Net;

namespace seomoonsijang.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            var webClient = new WebClient();
            byte[] imageBytes = webClient.DownloadData(@"https://westgateproject.blob.core.windows.net/blob1/2017-08-06%20PM%206%3A39%3A15.jpg");

            WebRequest req = WebRequest.Create(@"https://westgateproject.blob.core.windows.net/blob1/2017-08-06%20PM%206%3A39%3A15.jpg");
            WebResponse response = req.GetResponse();
            Image img = Image.FromStream(response.GetResponseStream());

            // Get the index of the orientation property.
            int orientation_index =
                Array.IndexOf(img.PropertyIdList, 0x0112);
                       

            ViewBag.Orientation = img.GetPropertyItem(0x0112).Value[0];
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
        [Authorize]
        public ActionResult Contact(HttpPostedFileBase file, ContentsEntity contents)
        {

            var blobName = DateTime.Now.ToString();
            blobName = blobName.Replace("/", "-");
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("MS_AzureStorageAccountConnectionString"));

            if (file != null)
            {
                CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
                CloudBlobContainer container = blobClient.GetContainerReference("blob1");
                CloudBlockBlob blob = container.GetBlockBlobReference(blobName);
                blob.Properties.ContentType = "image/jpeg";
                blob.UploadFromStream(file.InputStream);
                
                ViewBag.Message = file.FileName;

            }
            else
            {
                blobName = "empty";
            }

            if (contents != null)
            {
                CloudTableClient tableClient = storageAccount.CreateCloudTableClient();
                CloudTable table = tableClient.GetTableReference("WestGateMarket");
                ViewBag.TableSuccess = table.CreateIfNotExists();
                contents.PartitionKey = User.Identity.Name;
                contents.RowKey = blobName;
                TableOperation insertOperation = TableOperation.Insert(contents);
                TableResult result = table.Execute(insertOperation);
                ViewBag.TableName = table.Name;
                ViewBag.Result = result.HttpStatusCode;
                ViewBag.Message = "PartitionKey : " + contents.PartitionKey + "RowKey : " + contents.RowKey + "Text : " + contents.Text;
            }
            
            return View();
        }
        
    }
}