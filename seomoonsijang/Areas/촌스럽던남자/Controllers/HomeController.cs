using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using seomoonsijang.DataObjects;
using seomoonsijang.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace seomoonsijang.Areas.촌스럽던남자.Controllers
{
    public class HomeController : Controller
    {
        // GET: 촌스럽던남자/Home
        public ActionResult Index()
        {
            ViewBag.Building = "2지구";
            ViewBag.Floor = "지하1층";
            ViewBag.Location = "동99";
            ViewBag.URL = "SecondBaseFirst";

            string buildingName = "";
            switch(ViewBag.Building)
            {
                case "2지구":
                    buildingName = "SecondBuilding";
                    break;
                case "5지구":
                    buildingName = "FIfthBuilding";
                    break;
                case "동산상가":
                    buildingName = "Dongsan";
                    break;
            }

           CloudStorageAccount storageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("MS_AzureStorageAccountConnectionString"));

            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();
            CloudTable table = tableClient.GetTableReference(buildingName);
            // Construct the query operation for all customer entities where PartitionKey="Smith".

            // Create a retrieve operation that takes a customer entity.
            TableOperation retrieveOperation = TableOperation.Retrieve<ShopInfoEntity>(ViewBag.Floor, ViewBag.Location);

            // Execute the retrieve operation.
            TableResult retrievedResult = table.Execute(retrieveOperation);

            ShopInfoEntity retrievedEntity = (ShopInfoEntity)retrievedResult.Result;

            if (retrievedEntity == null)
            {
                return RedirectToAction("EmptyShop", new { Building = ViewBag.Building, Floor = ViewBag.Floor, Shop = ViewBag.Location });
            }

            // Print the phone number of the result.
            CloudTable tableUserInfo = tableClient.GetTableReference("UserInformation");
            TableOperation retrieveUserInfoOperation = TableOperation.Retrieve<UserInfoEntity>(retrievedEntity.OwnerID, buildingName + ":" + ViewBag.Floor + ":" + ViewBag.Location);

            TableResult retrievedUserInfoResult = tableUserInfo.Execute(retrieveUserInfoOperation);
            UserInfoEntity retrievedUserInfoEntity = (UserInfoEntity)retrievedUserInfoResult.Result;
            //오너 값으로 등록된 사진 가져오기
            string[] tempOwnerId = retrievedEntity.OwnerID.Split('@');

            CloudTable tableOwner = tableClient.GetTableReference(tempOwnerId[0]);

            TableQuery<ContentsEntity> rangeQuery = new TableQuery<ContentsEntity>().Where(
                    TableQuery.GenerateFilterCondition("ShopName", QueryComparisons.Equal, retrievedEntity.ShopName));
            
            ViewBag.ShopName = retrievedEntity.ShopName;
            ViewBag.PhoneNumber = retrievedUserInfoEntity.PhoneNumber;
            List<IndexToView> myActivity = new List<IndexToView>();
            foreach (ContentsEntity entity in tableOwner.ExecuteQuery(rangeQuery))
            {
                var imageURL = "https://westgateproject.blob.core.windows.net/" + entity.PartitionKey.Split('@')[0] + "/" + entity.RowKey;
                var text = entity.Context;

                IndexToView temp = new IndexToView(imageURL, text);
                myActivity.Add(temp);
            }
            myActivity.Reverse();
            return View(myActivity);
            //return View();
        }
    }
}