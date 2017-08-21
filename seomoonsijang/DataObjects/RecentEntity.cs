using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace seomoonsijang.DataObjects
{
    public class RecentEntity : Microsoft.WindowsAzure.Storage.Table.TableEntity
    {
        public RecentEntity(string id, string blobName, string shopName, string text)
        {
            PartitionKey = blobName.Split('.')[0];
            RowKey = blobName;
            ShopName = shopName;
            Context = text;
            ID = id;
        }

        public RecentEntity() { }

        public string ShopName { get; set; }
        public string Context { get; set; }
        public string ID { get; set; }
    }
}