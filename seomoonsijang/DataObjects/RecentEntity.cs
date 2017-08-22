using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace seomoonsijang.DataObjects
{
    public class RecentEntity : Microsoft.WindowsAzure.Storage.Table.TableEntity
    {
        public RecentEntity(string id, string blobName, string shopName, string text, string shopLocation)
        {
            PartitionKey = blobName.Split('.')[0];
            RowKey = blobName;
            ShopName = shopName;
            Context = text;
            ID = id;
            ShopLocation = shopLocation;
        }

        public RecentEntity() { }

        public string ShopName { get; set; }
        public string Context { get; set; }
        public string ID { get; set; }
        public string ShopLocation { get; set; }
    }
}