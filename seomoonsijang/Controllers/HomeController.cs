using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using seomoonsijang.Models;
using Microsoft.WindowsAzure.Storage.Table;
using System.Drawing;
using System.Net;
using seomoonsijang.DataObjects;

namespace seomoonsijang.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.ID = User.Identity.Name;
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("MS_AzureStorageAccountConnectionString"));

            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();
            CloudTable table = tableClient.GetTableReference("Recent");
            TableQuery<RecentEntity> query = new TableQuery<RecentEntity>();

            List<IndexToView> myActivity = new List<IndexToView>();
            // Print the fields for each customer.
            foreach (RecentEntity entity in table.ExecuteQuery(query))
            {
                var imageURL = "https://westgateproject.blob.core.windows.net/" + entity.PartitionKey.Split('@')[0] + "/" + entity.RowKey;
                var imgOrientation = ImageOrientation(imageURL);
                var text = entity.Text;
                if (entity.Text.Length > 20)
                {
                    text = entity.Text.Substring(0, 20) + "...";
                }
                var shopName = "/" + entity.ShopName;
                IndexToView result = new IndexToView(shopName, imageURL, text, imgOrientation);
                myActivity.Add(result);
            }
            myActivity.Reverse();
            return View(myActivity);
            
        }

        public ActionResult About()
        {
            ViewBag.Message = "서문시장의 건물별 위치와 이름을 확인하실 수 있습니다.";

            return View();
        }

        [HttpGet]
        [Authorize]
        public ActionResult Contact(List<string> homepage)
        {
            ViewBag.Message = User.Identity.Name;
            if (User.Identity.Name != "lsaforever0217@gmail.com")
            {
                return View();
            }
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("MS_AzureStorageAccountConnectionString"));

            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();
            CloudTable table = tableClient.GetTableReference("UserInformation");


            if (homepage != null)
            {
                foreach (string val in homepage)
                {
                    ViewBag.Message += " " + val;
                    var value = val.Split('^');

                    var OwnerID = value[0];
                    var ShopLocation = value[1];
                    var ShopName = value[2];
                    
                    TableOperation retrieveOperation = TableOperation.Retrieve<UserInfoEntity>(OwnerID, ShopLocation);
                    TableResult retrievedResult = table.Execute(retrieveOperation);

                    UserInfoEntity updateEntity = (UserInfoEntity)retrievedResult.Result;

                    // Print the phone number of the result.
                    if (updateEntity != null)
                    {
                        updateEntity.Paid = true;
                        TableOperation updateOperation = TableOperation.Replace(updateEntity);
                        table.Execute(updateOperation);
                    }

                    var tempBuilding = ShopLocation.Split(':');
                    var building = tempBuilding[0];
                    var floor = tempBuilding[1];
                    var location = tempBuilding[2];

                    CloudTable OwnerIDtable = tableClient.GetTableReference(building);

                    BuildingEntity shopInfo = new BuildingEntity(floor, location, OwnerID, ShopName);

                    TableOperation inserOperation = TableOperation.Insert(shopInfo);
                    OwnerIDtable.Execute(inserOperation);

                }

            }


            TableQuery<UserInfoEntity> query = new TableQuery<UserInfoEntity>();
            List<UserInfoEntity> result = new List<UserInfoEntity>();
            foreach (UserInfoEntity entity in table.ExecuteQuery(query))
            {
                result.Add(entity);
            }
            //List<string> ID = new List<string>();
            //List<string> ShopLocation = new List<string>();
            //List<string> ShopName = new List<string>();
            //List<string> Payment = new List<string>();
            //List<string> AddInfo = new List<string>();
            //List<string> PhoneNumber = new List<string>();
            //List<bool> Paid = new List<bool>();
            // Print the fields for each customer.
            //foreach (UserInfoEntity entity in table.ExecuteQuery(query))
            //{
            //    ID.Add(entity.PartitionKey);
            //    ShopLocation.Add(entity.RowKey);
            //    ShopName.Add(entity.ShopName);
            //    Payment.Add(entity.Payment);
            //    AddInfo.Add(entity.AddInfo);
            //    PhoneNumber.Add(entity.PhoneNumber);
            //    Paid.Add(entity.Paid);
            //}
            //UserInfoToView result = new UserInfoToView()
            //{
            //    ID = ID,
            //    ShopLocation = ShopLocation,
            //    ShopName = ShopName,
            //    Payment = Payment,
            //    AddInfo = AddInfo,
            //    PhoneNumber = PhoneNumber,
            //    Paid = Paid
            //};
            return View(result);
                        

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
            
        }
        

        //[HttpPost]
        //[Authorize]
        //public ActionResult Contact(HttpPostedFileBase file, ContentsEntity contents)
        //{

        //    var blobName = DateTime.Now.ToString();
        //    blobName = blobName.Replace("/", "-");
        //    CloudStorageAccount storageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("MS_AzureStorageAccountConnectionString"));

        //    if (file != null)
        //    {
        //        CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
        //        CloudBlobContainer container = blobClient.GetContainerReference("blob1");
        //        CloudBlockBlob blob = container.GetBlockBlobReference(blobName);
        //        blob.Properties.ContentType = "image/jpeg";
        //        blob.UploadFromStream(file.InputStream);
                
        //        ViewBag.Message = file.FileName;

        //    }
        //    else
        //    {
        //        blobName = "empty";
        //    }

        //    if (contents != null)
        //    {
        //        CloudTableClient tableClient = storageAccount.CreateCloudTableClient();
        //        CloudTable table = tableClient.GetTableReference("WestGateMarket");
        //        ViewBag.TableSuccess = table.CreateIfNotExists();
        //        contents.PartitionKey = User.Identity.Name;
        //        contents.RowKey = blobName;
        //        TableOperation insertOperation = TableOperation.Insert(contents);
        //        TableResult result = table.Execute(insertOperation);
        //        ViewBag.TableName = table.Name;
        //        ViewBag.Result = result.HttpStatusCode;
        //        ViewBag.Message = "PartitionKey : " + contents.PartitionKey + "RowKey : " + contents.RowKey + "Text : " + contents.Context;
        //    }
            
        //    return View();
        //}
        
        protected int ImageOrientation(string imgURL)
        {
            WebRequest req = WebRequest.Create(imgURL);
            WebResponse response = req.GetResponse();
            Image img = Image.FromStream(response.GetResponseStream());

            // Get the index of the orientation property.
            int orientation_index =
                Array.IndexOf(img.PropertyIdList, 0x0112);


            return img.GetPropertyItem(0x0112).Value[0];
        }
    }
}