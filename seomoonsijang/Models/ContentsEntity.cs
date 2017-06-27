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
            this.PartitionKey = patKey;
            this.RowKey = rKey;
        }

        public ContentsEntity() { }

        public string text { get; set; }
        public string filename { get; set; }
    }
}