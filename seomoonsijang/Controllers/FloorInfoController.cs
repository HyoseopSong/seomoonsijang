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

namespace seomoonsijang.Controllers
{
    public class FloorInfoController : Controller
    {
        public ActionResult SecondBaseFirst()
        {
            ViewBag.Message = "2지구 지하1층 상가 위치 정보 입니다.";
            return View();
        }
        public ActionResult SecondFirst()
        {
            ViewBag.Message = "2지구 지상1층 상가 위치 정보 입니다.";
            return View();
        }
        public ActionResult SecondSecond()
        {
            ViewBag.Message = "2지구 지상2층 상가 위치 정보 입니다.";
            return View();
        }
        public ActionResult SecondThird()
        {
            ViewBag.Message = "2지구 지상3층 상가 위치 정보 입니다.";
            return View();
        }
        public ActionResult SecondForth()
        {
            ViewBag.Message = "2지구 지상4층 상가 위치 정보 입니다.";
            return View();
        }

        public ActionResult FifthFirst()
        {
            ViewBag.Message = "5지구 지상1층 상가 위치 정보 입니다.";
            return View();
        }
        public ActionResult FifthSecond()
        {
            ViewBag.Message = "5지구 지상2층 상가 위치 정보 입니다.";
            return View();
        }

        public ActionResult DongsanFirst()
        {
            ViewBag.Message = "동산상가 지상1층 상가 위치 정보 입니다.";
            return View();
        }
        public ActionResult DongsanSecond()
        {
            ViewBag.Message = "동산상가 지상2층 상가 위치 정보 입니다.";
            return View();
        }

        public ActionResult Union()
        {
            ViewBag.Message = "상가연합회 만남의 광장 정보 입니다.";
            return View();
        }

        public ActionResult ShopInfoPage(string building, string floor, string location)
        {
            ViewBag.Message = "상가연합회 만남의 광장 정보 입니다.";
            string tempURL = "";
            switch (building)
            {
                case "Dongsan":
                    ViewBag.Building = "동산상가";
                    tempURL += building;
                    break;
                case "FifthBuilding":
                    ViewBag.Building = "5지구";
                    tempURL += "Fifth";
                    break;
                case "SecondBuilding":
                    ViewBag.Building = "2지구";
                    tempURL += "Second";
                    break;
            }
            ViewBag.Floor = floor;
            switch (floor)
            {
                case "지하1층":
                    tempURL += "BaseFirst";
                    break;
                case "1층":
                    tempURL += "First";
                    break;
                case "2층":
                    tempURL += "Second";
                    break;
                case "3층":
                    tempURL += "Third";
                    break;
                case "4층":
                    tempURL = "Forth";
                    break;
                default:
                    tempURL += "default";
                    break;
            }
            ViewBag.URL = tempURL;
            ViewBag.Location = location;
            //return View();


            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("MS_AzureStorageAccountConnectionString"));

            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();
            CloudTable table = tableClient.GetTableReference(building);
            // Construct the query operation for all customer entities where PartitionKey="Smith".

            // Create a retrieve operation that takes a customer entity.
            TableOperation retrieveOperation = TableOperation.Retrieve<ShopInfoEntity>(floor, location);

            // Execute the retrieve operation.
            TableResult retrievedResult = table.Execute(retrieveOperation);

            ShopInfoEntity retrievedEntity = (ShopInfoEntity)retrievedResult.Result;
            // Print the phone number of the result.
            CloudTable tableUserInfo = tableClient.GetTableReference("UserInformation");
            TableOperation retrieveUserInfoOperation = TableOperation.Retrieve<UserInfoEntity>(retrievedEntity.OwnerID, building + ":" + floor + ":" + location);

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