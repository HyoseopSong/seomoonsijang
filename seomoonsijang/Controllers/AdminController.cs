using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
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
    [Authorize]
    public class AdminController : Controller
    {
        // GET: Admin
        [HttpGet]
        public ActionResult Index(List<string> register)
        {
            //ViewBag.Message = User.Identity.Name;
            if (User.Identity.Name != "lsaforever0217@gmail.com")
            {
                return RedirectToAction("Index", "Home");
            }
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("MS_AzureStorageAccountConnectionString"));

            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();
            CloudTable table = tableClient.GetTableReference("UserInformation");
                        
            if (register != null)
            {
                foreach (string val in register)
                {

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
                        updateEntity.Period = DateTime.Now.AddMonths(1);
                        updateEntity.InitialRegister = false;
                        TableOperation updateOperation = TableOperation.Replace(updateEntity);
                        table.Execute(updateOperation);
                    }

                    var tempBuilding = ShopLocation.Split(':');
                    var building = tempBuilding[0];
                    var floor = tempBuilding[1];
                    var location = tempBuilding[2];

                    CloudTable BuildingTable = tableClient.GetTableReference(building);


                    TableOperation retrieveBuildingOperation = TableOperation.Retrieve<BuildingEntity>(floor, location);
                    TableResult retrievedBuildingResult = BuildingTable.Execute(retrieveBuildingOperation);
                    BuildingEntity shopInfo = (BuildingEntity)retrievedBuildingResult.Result;

                    if (shopInfo != null)
                    {
                        shopInfo.OnService = true;
                        TableOperation updateOperation = TableOperation.Replace(shopInfo);
                        BuildingTable.Execute(updateOperation);
                    }

                }

            }

            List<UserInfoEntity> result = new List<UserInfoEntity>();
            List<UserInfoEntity> resultFalse = new List<UserInfoEntity>();
            TableQuery<UserInfoEntity> queryFalse = new TableQuery<UserInfoEntity>().Where(
                        TableQuery.GenerateFilterConditionForBool("InitialRegister", QueryComparisons.Equal, true));
            foreach (UserInfoEntity entity in table.ExecuteQuery(queryFalse))
            {
                resultFalse.Add(entity);
            }
            result = resultFalse;

            return View(result);
           

            


            //List<UserInfoEntity> resultOutPeriod = new List<UserInfoEntity>();
            //TableQuery<UserInfoEntity> queryOutPeriod = new TableQuery<UserInfoEntity>().Where(
            //    TableQuery.CombineFilters(
            //    TableQuery.GenerateFilterConditionForDate("Period", QueryComparisons.LessThan, DateTime.Now),
            //    TableOperators.And,
            //    TableQuery.GenerateFilterConditionForBool("Paid", QueryComparisons.Equal, true)));
            //foreach (UserInfoEntity entity in table.ExecuteQuery(queryOutPeriod))
            //{
            //    entity.Period = entity.Period.AddHours(9);
            //    resultOutPeriod.Add(entity);
            //}
            //result[2] = resultOutPeriod;

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
        [HttpGet]
        public ActionResult Homepage(List<string> homepage)
        {
            if (User.Identity.Name != "lsaforever0217@gmail.com")
            {
                return RedirectToAction("Index", "Home");
            }
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("MS_AzureStorageAccountConnectionString"));

            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();
            CloudTable table = tableClient.GetTableReference("UserInformation");

            if (homepage != null)
            {
                foreach (string val in homepage)
                {

                    var value = val.Split('^');

                    var OwnerID = value[0];
                    var ShopLocation = value[1];

                    TableOperation retrieveOperation = TableOperation.Retrieve<UserInfoEntity>(OwnerID, ShopLocation);
                    TableResult retrievedResult = table.Execute(retrieveOperation);

                    UserInfoEntity updateEntity = (UserInfoEntity)retrievedResult.Result;

                    // Print the phone number of the result.
                    if (updateEntity != null)
                    {
                        updateEntity.Homepage = "제작 완료";
                        TableOperation updateOperation = TableOperation.Replace(updateEntity);
                        table.Execute(updateOperation);
                    }                  

                }

            }

            List<UserInfoEntity> result = new List<UserInfoEntity>();
            List<UserInfoEntity> resultHomepage = new List<UserInfoEntity>();
            TableQuery<UserInfoEntity> queryHomepage = new TableQuery<UserInfoEntity>().Where(
                        TableQuery.GenerateFilterCondition("Homepage", QueryComparisons.Equal, "신청 함"));
            foreach (UserInfoEntity entity in table.ExecuteQuery(queryHomepage))
            {
                resultHomepage.Add(entity);
            }
            result = resultHomepage;

            return View(result);
        }

        [HttpGet]
        public ActionResult OnService(int? months, string userInfo)
        {
            if (User.Identity.Name != "lsaforever0217@gmail.com")
            {
                return RedirectToAction("Index", "Home");
            }
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("MS_AzureStorageAccountConnectionString"));

            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();
            CloudTable table = tableClient.GetTableReference("UserInformation");

            if (months.HasValue && userInfo != null)
            {

                var value = userInfo.Split('^');

                var OwnerID = value[0];
                var ShopLocation = value[1];

                TableOperation retrieveOperation = TableOperation.Retrieve<UserInfoEntity>(OwnerID, ShopLocation);
                TableResult retrievedResult = table.Execute(retrieveOperation);

                UserInfoEntity updateEntity = (UserInfoEntity)retrievedResult.Result;

                // Print the phone number of the result.
                if (updateEntity != null)
                {
                    updateEntity.Period = updateEntity.Period.AddMonths(months.Value);
                    TableOperation updateOperation = TableOperation.Replace(updateEntity);
                    table.Execute(updateOperation);
                }
            }

            List<UserInfoEntity> result = new List<UserInfoEntity>();
            List<UserInfoEntity> resultInPeriod = new List<UserInfoEntity>();
            TableQuery<UserInfoEntity> queryInPeriod = new TableQuery<UserInfoEntity>().Where(
                TableQuery.GenerateFilterConditionForDate("Period", QueryComparisons.GreaterThanOrEqual, DateTime.Now));
            foreach (UserInfoEntity entity in table.ExecuteQuery(queryInPeriod))
            {
                if (entity.Paid)
                {
                    entity.Period = entity.Period.AddHours(9);
                    resultInPeriod.Add(entity);
                }
            }
            result = resultInPeriod;

            return View(result);
        }

        [HttpGet]
        public ActionResult TimeOver(List<string> timeover)
        {
            if (User.Identity.Name != "lsaforever0217@gmail.com")
            {
                return RedirectToAction("Index", "Home");
            }
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("MS_AzureStorageAccountConnectionString"));

            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();
            CloudTable table = tableClient.GetTableReference("UserInformation");
            if (timeover != null)
            {
                foreach (string val in timeover)
                {

                    var value = val.Split('^');

                    var OwnerID = value[0];
                    var ShopLocation = value[1];
                    var ShopName = value[2];

                    TableOperation retrieveOperation = TableOperation.Retrieve<UserInfoEntity>(OwnerID, ShopLocation);
                    TableResult retrievedResult = table.Execute(retrieveOperation);

                    UserInfoEntity timeoverEntity = (UserInfoEntity)retrievedResult.Result;

                    // Print the phone number of the result.
                    if (timeoverEntity != null)
                    {
                        timeoverEntity.Paid = false;
                        TableOperation updateOperation = TableOperation.Replace(timeoverEntity);
                        table.Execute(updateOperation);
                    }

                    var tempBuilding = ShopLocation.Split(':');
                    var building = tempBuilding[0];
                    var floor = tempBuilding[1];
                    var location = tempBuilding[2];

                    CloudTable BuildingTable = tableClient.GetTableReference(building);


                    TableOperation retrieveShopInfoOperation = TableOperation.Retrieve<BuildingEntity>(floor, location);
                    TableResult retrievedShopInfoResult = BuildingTable.Execute(retrieveShopInfoOperation);

                    BuildingEntity pauseShopInfoEntity = (BuildingEntity)retrievedShopInfoResult.Result;

                    if (pauseShopInfoEntity != null)
                    {
                        pauseShopInfoEntity.OnService = false;
                        TableOperation updateOperation = TableOperation.Replace(pauseShopInfoEntity);
                        BuildingTable.Execute(updateOperation);
                    }

                    CloudTable tableOfRecent = tableClient.GetTableReference("Recent");

                    TableQuery<RecentEntity> queryRecent = new TableQuery<RecentEntity>().Where(
                                TableQuery.GenerateFilterCondition("ShopName", QueryComparisons.Equal, ShopName));

                    foreach (RecentEntity entity in tableOfRecent.ExecuteQuery(queryRecent))
                    {
                        TableOperation deleteOperation = TableOperation.Delete(entity);
                        tableOfRecent.Execute(deleteOperation);
                    }
                }
            }

            List<UserInfoEntity> result = new List<UserInfoEntity>();
            List<UserInfoEntity> resultKeep = new List<UserInfoEntity>();
            TableQuery<UserInfoEntity> queryKeep = new TableQuery<UserInfoEntity>().Where(
                TableQuery.CombineFilters(
                TableQuery.GenerateFilterConditionForDate("Period", QueryComparisons.LessThan, DateTime.Now),
                TableOperators.And,
                TableQuery.GenerateFilterConditionForDate("Period", QueryComparisons.GreaterThan, DateTime.Now.AddDays(-7))));
            foreach (UserInfoEntity entity in table.ExecuteQuery(queryKeep))
            {
                if (entity.Paid)
                {
                    entity.Period = entity.Period.AddHours(9);
                    resultKeep.Add(entity);
                }
            }
            result = resultKeep;

            return View(result);
        }

        [HttpGet]
        public ActionResult Keep(int? months, string keepInfo)
        {
            if (User.Identity.Name != "lsaforever0217@gmail.com")
            {
                return RedirectToAction("Index", "Home");
            }
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("MS_AzureStorageAccountConnectionString"));

            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();
            CloudTable table = tableClient.GetTableReference("UserInformation");

            if (months.HasValue && keepInfo != null)
            {
                var value = keepInfo.Split('^');

                var OwnerID = value[0];
                var ShopLocation = value[1];

                TableOperation retrieveOperation = TableOperation.Retrieve<UserInfoEntity>(OwnerID, ShopLocation);
                TableResult retrievedResult = table.Execute(retrieveOperation);

                UserInfoEntity updateEntity = (UserInfoEntity)retrievedResult.Result;

                // Print the phone number of the result.
                if (updateEntity != null)
                {
                    updateEntity.Period = DateTime.Now.AddMonths(months.Value);
                    updateEntity.Paid = true;
                    TableOperation updateOperation = TableOperation.Replace(updateEntity);
                    table.Execute(updateOperation);
                }

                //건물 테이블에서 온서비스 트루로 바꿔주기
                var tempBuilding = ShopLocation.Split(':');
                var building = tempBuilding[0];
                var floor = tempBuilding[1];
                var location = tempBuilding[2];

                CloudTable BuildingTable = tableClient.GetTableReference(building);
                BuildingTable.CreateIfNotExists();
                TableOperation retrieveBuildingInfoOperation = TableOperation.Retrieve<BuildingEntity>(floor, location);
                TableResult retrievedBuildingInfoResult = BuildingTable.Execute(retrieveBuildingInfoOperation);

                BuildingEntity buildingInfoEntity = (BuildingEntity)retrievedBuildingInfoResult.Result;

                // Print the phone number of the result.
                if (buildingInfoEntity != null)
                {
                    buildingInfoEntity.OnService = true;
                    TableOperation updateOperation = TableOperation.Replace(buildingInfoEntity);
                    BuildingTable.Execute(updateOperation);
                }
            }

            List<UserInfoEntity> result = new List<UserInfoEntity>();
            List<UserInfoEntity> resultKeepFalse = new List<UserInfoEntity>();
            TableQuery<UserInfoEntity> queryKeep = new TableQuery<UserInfoEntity>().Where(
                TableQuery.CombineFilters(
                TableQuery.GenerateFilterConditionForDate("Period", QueryComparisons.LessThan, DateTime.Now),
                TableOperators.And,
                TableQuery.GenerateFilterConditionForDate("Period", QueryComparisons.GreaterThan, DateTime.Now.AddDays(-7))));
            foreach (UserInfoEntity entity in table.ExecuteQuery(queryKeep))
            {
                if (!entity.Paid)
                {
                    entity.Period = entity.Period.AddHours(9);
                    resultKeepFalse.Add(entity);
                }
            }
            result = resultKeepFalse;

            return View(result);
        }

        [HttpGet]
        public ActionResult Delete(List<string> delete)
        {
            if (User.Identity.Name != "lsaforever0217@gmail.com")
            {
                return RedirectToAction("Index", "Home");
            }
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("MS_AzureStorageAccountConnectionString"));

            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();
            CloudTable table = tableClient.GetTableReference("UserInformation");

            if (delete != null)
            {
                foreach (var i in delete)
                {
                    //유저인포에서 해당 매장 삭제, 계정 테이블에서 해당 매장으로 등록된 블롭 삭제
                    var value = i.Split('^');

                    var OwnerID = value[0];
                    var OwnerIDforTable = OwnerID.Split('@')[0];
                    var ShopLocation = value[1];
                    var ShopName = value[2];

                    TableOperation retrieveOperation = TableOperation.Retrieve<UserInfoEntity>(OwnerID, ShopLocation);
                    TableResult retrievedResult = table.Execute(retrieveOperation);

                    UserInfoEntity deleteEntity = (UserInfoEntity)retrievedResult.Result;

                    // Print the phone number of the result.
                    if (deleteEntity != null)
                    {
                        TableOperation updateOperation = TableOperation.Delete(deleteEntity);
                        table.Execute(updateOperation);
                    }


                    CloudTable tableOfOwner = tableClient.GetTableReference(OwnerIDforTable);

                    TableQuery<ContentsEntity> queryShopName = new TableQuery<ContentsEntity>().Where(
                                TableQuery.GenerateFilterCondition("ShopName", QueryComparisons.Equal, ShopName));

                    foreach (ContentsEntity entity in tableOfOwner.ExecuteQuery(queryShopName))
                    {
                        TableOperation deleteOperation = TableOperation.Delete(entity);
                        tableOfOwner.Execute(deleteOperation);
                    }

                    var tempBuilding = ShopLocation.Split(':');
                    var building = tempBuilding[0];
                    var floor = tempBuilding[1];
                    var location = tempBuilding[2];

                    CloudTable BuildingTable = tableClient.GetTableReference(building);
                    TableOperation retrieveBuildingInfoOperation = TableOperation.Retrieve<BuildingEntity>(floor, location);
                    TableResult retrievedBuildingInfoResult = BuildingTable.Execute(retrieveBuildingInfoOperation);

                    BuildingEntity buildingInfoEntity = (BuildingEntity)retrievedBuildingInfoResult.Result;

                    // Print the phone number of the result.
                    if (buildingInfoEntity != null)
                    {
                        TableOperation deleteOperation = TableOperation.Delete(buildingInfoEntity);
                        BuildingTable.Execute(deleteOperation);
                    }

                }
            }

            
            List<UserInfoEntity> result = new List<UserInfoEntity>();
            List<UserInfoEntity> resultExpired = new List<UserInfoEntity>();
            TableQuery<UserInfoEntity> queryExpired = new TableQuery<UserInfoEntity>().Where(
                TableQuery.GenerateFilterConditionForDate("Period", QueryComparisons.LessThan, DateTime.Now.AddDays(-7)));
            foreach (UserInfoEntity entity in table.ExecuteQuery(queryExpired))
            {
                entity.Period = entity.Period.AddHours(9);
                resultExpired.Add(entity);
            }
            result = resultExpired;

            return View(result);
        }        
    }
}