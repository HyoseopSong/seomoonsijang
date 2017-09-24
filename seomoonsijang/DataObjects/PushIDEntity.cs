using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace seomoonsijang.DataObjects
{
    public class PushIDEntity : TableEntity
    {
        public PushIDEntity() { }

        public PushIDEntity(string userID, string shopName, string pushID)
        {
            PartitionKey = userID;
            RowKey = shopName;
            PushID = pushID;
        }

        public string PushID { get; set; }
    }
}