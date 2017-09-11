using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace seomoonsijang.DataObjects
{
    public class VisitCounter : TableEntity
    {
        public VisitCounter()
        {
            DateTime KRT = DateTime.Now.AddHours(9);

            string rowKey = "";
            if (KRT.Hour > 0 && KRT.Hour < 10)
            {
                rowKey = "0" + KRT.Hour.ToString() + "시";
            }
            else
            {
                rowKey = KRT.Hour.ToString() + "시";
            }

            string partitionKey = KRT.Year.ToString() + "년";
            if (KRT.Month > 0 && KRT.Month < 10)
            {
                partitionKey += "0" + KRT.Month.ToString() + "월";
            }
            else
            {
                partitionKey += KRT.Month.ToString() + "월";
            }
            if (KRT.Day > 0 && KRT.Day < 10)
            {
                partitionKey += "0" + KRT.Day.ToString() + "일";
            }
            else
            {
                partitionKey += KRT.Day.ToString() + "일";
            }

            PartitionKey = partitionKey;
            RowKey = rowKey;
            Number = 1;
        }

        public VisitCounter(string _partitionKey, string _rowKey)
        {

            PartitionKey = _partitionKey;
            RowKey = _rowKey;
            Number = 1;
        }

        public int Number { get; set; }
    }
}