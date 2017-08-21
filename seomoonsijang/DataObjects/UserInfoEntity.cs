using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace seomoonsijang.DataObjects
{
    public class UserInfoEntity : TableEntity
    {
        public UserInfoEntity(string id, string shopLocation, string shopName, string payment, string addInfo, string phoneNumber, bool paid, bool initialRegister)
        {
            PartitionKey = id;
            RowKey = shopLocation;
            ShopName = shopName;
            Payment = payment;
            AddInfo = addInfo;
            PhoneNumber = phoneNumber;
            Paid = paid;
            InitialRegister = initialRegister;
        }

        public UserInfoEntity() { }

        public string ShopName { get; set; }
        public string Payment { get; set; }
        public string AddInfo { get; set; }
        public string PhoneNumber { get; set; }
        public bool Paid { get; set; }
        public DateTime Period { get; set; }
        public bool InitialRegister { get; set; }
    }
}