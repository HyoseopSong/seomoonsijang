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
            //ViewBag.ID = User.Identity.Name;
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("MS_AzureStorageAccountConnectionString"));

            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();
            CloudTable table = tableClient.GetTableReference("Recent");
            TableQuery<RecentEntity> query = new TableQuery<RecentEntity>();

            List<IndexToView> myActivity = new List<IndexToView>();
            // Print the fields for each customer.
            foreach (RecentEntity entity in table.ExecuteQuery(query))
            {
                var imageURL = "https://westgateproject.blob.core.windows.net/" + entity.ID.Split('@')[0] + "/" + entity.RowKey;
                var imgOrientation = ImageOrientation(imageURL);
                var text = entity.Context;
                var location = entity.ShopLocation;
                if (entity.Context.Length > 20)
                {
                    text = entity.Context.Substring(0, 20) + "...";
                }
                var shopName = "/" + entity.ShopName;
                IndexToView result = new IndexToView(shopName, imageURL, text, imgOrientation, location);
                myActivity.Add(result);
            }
            myActivity.Reverse();
            return View(myActivity);
            
        }

        public ActionResult About()
        {

            return View();
        }

        public ActionResult HowToRegister()
        {

            return View();
        }

        public ActionResult Intro()
        {
            ViewBag.Message = "서문시장의 건물별 위치와 이름을 확인하실 수 있습니다.";

            return View();
        }

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
    }
}