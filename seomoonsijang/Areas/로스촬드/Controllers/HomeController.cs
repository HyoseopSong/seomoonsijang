using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Table;
using seomoonsijang.DataObjects;
using seomoonsijang.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace seomoonsijang.Areas.로스촬드.Controllers
{
    public class HomeController : Controller
    {
        // GET: 로스촬드/Home
        public ActionResult Index()
        {
            ViewBag.Title = "로스촬드";

            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("MS_AzureStorageAccountConnectionString"));

            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();
            CloudTable table = tableClient.GetTableReference("lsaforever0217");

            TableQuery<ContentsEntity> query = new TableQuery<ContentsEntity>();

            List<IndexToView> myActivity = new List<IndexToView>();
            foreach (ContentsEntity entity in table.ExecuteQuery(query))
            {
                var imageURL = "https://westgateproject.blob.core.windows.net/" + entity.PartitionKey.Split('@')[0] + "/" + entity.RowKey;
                var imgOrientation = ImageOrientation(imageURL);
                var text = entity.Context;
                var shopName = "/" + entity.ShopName;
                IndexToView result = new IndexToView(shopName, imageURL, text, imgOrientation, "");
                myActivity.Add(result);
            }
            myActivity.Reverse();
            return View(myActivity);
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
    }
}