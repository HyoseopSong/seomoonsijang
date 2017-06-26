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
            return View();
        }
        [HttpPost]
        public ActionResult Contact(HttpPostedFileBase file)
        {

            if (file != null)
            {
                CloudStorageAccount storageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("westgateproject_AzureStorageConnectionString"));
                CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
                CloudBlobContainer container = blobClient.GetContainerReference("blob1");
                
                CloudBlockBlob blob = container.GetBlockBlobReference(file.FileName);
                blob.UploadFromStream(file.InputStream);
                ViewBag.Message = file.FileName;
                               
            }
            else
            {
                ViewBag.Message = "파일 업로드가 실패했습니다!!";
            }
            
            return View();
        }
    }
}