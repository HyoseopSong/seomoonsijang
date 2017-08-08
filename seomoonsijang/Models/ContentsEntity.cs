using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.WindowsAzure.Storage.Table;
namespace seomoonsijang.Models
{
    public class ContentsEntity : TableEntity
    {
        public ContentsEntity(string patKey, string rKey)
        {
            PartitionKey = patKey;
            RowKey = rKey;
        }

        public ContentsEntity() { }

        public string Text { get; set; }
    }
}