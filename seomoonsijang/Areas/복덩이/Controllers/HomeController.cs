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

namespace seomoonsijang.Areas.복덩이.Controllers
{
    public class HomeController : Controller
    {
        // GET: 복덩이/Home
        public ActionResult Index()
        {
            ViewBag.Building = "2지구";
            ViewBag.Floor = "1층";
            ViewBag.Location = "서마12";
            ViewBag.URL = "SecondFirst";



            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("MS_AzureStorageAccountConnectionString"));

            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();
            CloudTable table = tableClient.GetTableReference("SecondBuilding");
            // Construct the query operation for all customer entities where PartitionKey="Smith".

            // Create a retrieve operation that takes a customer entity.
            TableOperation retrieveOperation = TableOperation.Retrieve<ShopInfoEntity>("1층", "서마12");

            // Execute the retrieve operation.
            TableResult retrievedResult = table.Execute(retrieveOperation);

            ShopInfoEntity retrievedEntity = (ShopInfoEntity)retrievedResult.Result;

            if (retrievedEntity == null)
            {
                return RedirectToAction("EmptyShop", new { Building = ViewBag.Building, Floor = "1층", Shop = "서마12" });
            }

            // Print the phone number of the result.
            CloudTable tableUserInfo = tableClient.GetTableReference("UserInformation");
            TableOperation retrieveUserInfoOperation = TableOperation.Retrieve<UserInfoEntity>(retrievedEntity.OwnerID, "SecondBuilding:1층:서마12");

            TableResult retrievedUserInfoResult = tableUserInfo.Execute(retrieveUserInfoOperation);
            UserInfoEntity retrievedUserInfoEntity = (UserInfoEntity)retrievedUserInfoResult.Result;
            //오너 값으로 등록된 사진 가져오기
            string[] tempOwnerId = retrievedEntity.OwnerID.Split('@');

            CloudTable tableOwner = tableClient.GetTableReference(tempOwnerId[0]);

            TableQuery<ContentsEntity> rangeQuery = new TableQuery<ContentsEntity>().Where(
                    TableQuery.GenerateFilterCondition("ShopName", QueryComparisons.Equal, retrievedEntity.ShopName));

            //IDictionary<string, string> myActivity = new Dictionary<string, string>
            //{
            //    { "ShopName", retrievedEntity.ShopName },
            //    { "ShopOwner", retrievedEntity.OwnerID.Split('@')[0] },
            //    { "PhoneNumber", retrievedUserInfoEntity.PhoneNumber }
            //};
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