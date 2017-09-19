using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace seomoonsijang.DataObjects
{
    public class LikeEntity : TableEntity
    {
        public LikeEntity() { }

        public LikeEntity(string id, string blobName)
        {
            PartitionKey = id;
            RowKey = blobName;
        }

    }
}