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
            TableQuery<ContentsEntity> query = new TableQuery<ContentsEntity>();

            List<IndexToView> myActivity = new List<IndexToView>();
            // Print the fields for each customer.
            foreach (ContentsEntity entity in table.ExecuteQuery(query))
            {
                var imageURL = "https://westgateproject.blob.core.windows.net/" + entity.PartitionKey.Split('@')[0] + "/" + entity.RowKey;
                var imgOrientation = ImageOrientation(imageURL);
                var text = entity.Context;
                if (entity.Context.Length > 20)
                {
                    text = entity.Context.Substring(0, 20) + "...";
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
        public ActionResult Contact(List<string> register, int? months, string userInfo, string keepInfo, List<string> timeover, List<string> delete)
        {
            ViewBag.Message = User.Identity.Name;
            if (User.Identity.Name != "lsaforever0217@gmail.com")
            {
                return RedirectToAction("Index", "Home");
            }
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("MS_AzureStorageAccountConnectionString"));

            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();
            CloudTable table = tableClient.GetTableReference("UserInformation");

            if(delete != null)
            {
                foreach(var i in delete)
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
            else if(months.HasValue && keepInfo != null)
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
                    BuildingTable.CreateIfNotExists();
                    BuildingEntity shopInfo = new BuildingEntity(floor, location, OwnerID, ShopName);

                    TableOperation insertOperation = TableOperation.Insert(shopInfo);
                    BuildingTable.Execute(insertOperation);

                }

            }

            if(timeover != null)
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

                    TableQuery<ContentsEntity> queryRecent = new TableQuery<ContentsEntity>().Where(
                                TableQuery.GenerateFilterCondition("ShopName", QueryComparisons.Equal, ShopName));

                    foreach (ContentsEntity entity in tableOfRecent.ExecuteQuery(queryRecent))
                    {
                        TableOperation deleteOperation = TableOperation.Delete(entity);
                        tableOfRecent.Execute(deleteOperation);
                    }
                }
            }

            List<UserInfoEntity>[] result = new List<UserInfoEntity>[5];
            List<UserInfoEntity> resultFalse = new List<UserInfoEntity>();   
            TableQuery<UserInfoEntity> queryFalse = new TableQuery<UserInfoEntity>().Where(
                        TableQuery.GenerateFilterConditionForBool("InitialRegister", QueryComparisons.Equal, true));
            foreach (UserInfoEntity entity in table.ExecuteQuery(queryFalse))
            {
                resultFalse.Add(entity);
            }
            result[0] = resultFalse;

            List<UserInfoEntity> resultInPeriod = new List<UserInfoEntity>();
            TableQuery<UserInfoEntity> queryInPeriod = new TableQuery<UserInfoEntity>().Where(
                TableQuery.GenerateFilterConditionForDate("Period", QueryComparisons.GreaterThanOrEqual, DateTime.Now));
            foreach (UserInfoEntity entity in table.ExecuteQuery(queryInPeriod))
            {
                entity.Period = entity.Period.AddHours(9);
                resultInPeriod.Add(entity);
            }
            result[1] = resultInPeriod;



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

            List<UserInfoEntity> resultKeep = new List<UserInfoEntity>();
            List<UserInfoEntity> resultKeepFalse = new List<UserInfoEntity>();
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
                else
                {
                    entity.Period = entity.Period.AddHours(9);
                    resultKeepFalse.Add(entity);
                }
            }
            result[2] = resultKeep;
            result[3] = resultKeepFalse;

            List<UserInfoEntity> resultExpired = new List<UserInfoEntity>();
            TableQuery<UserInfoEntity> queryExpired = new TableQuery<UserInfoEntity>().Where(
                TableQuery.GenerateFilterConditionForDate("Period", QueryComparisons.LessThan, DateTime.Now.AddDays(-7)));
            foreach (UserInfoEntity entity in table.ExecuteQuery(queryExpired))
            {
                entity.Period = entity.Period.AddHours(9);
                resultExpired.Add(entity);
            }
            result[4] = resultExpired;
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