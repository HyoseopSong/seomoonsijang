using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace seomoonsijang.DataObjects
{
    public class BuildingEntity : TableEntity
    {
        public BuildingEntity(string floor, string location, string ownerID, string shopName)
        {
            PartitionKey = floor;
            RowKey = location;
            OwnerID = ownerID;
            ShopName = shopName;
            OnService = true;
        }

        public BuildingEntity() { }

        public string OwnerID { get; set; }
        public string ShopName { get; set; }
        public bool OnService { get; set; }

    }
}