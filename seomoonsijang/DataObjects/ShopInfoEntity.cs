using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace seomoonsijang.DataObjects
{
    public class ShopInfoEntity : TableEntity
    {
        public ShopInfoEntity(string floor, string name, string id, string location)
        {
            PartitionKey = floor;
            RowKey = location;
            OwnerID = id;
            ShopName = name;
        }
        public ShopInfoEntity() { }

        public string OwnerID { get; set; }
        public string ShopName { get; set; }
        public bool OnService { get; set; }


    }
}