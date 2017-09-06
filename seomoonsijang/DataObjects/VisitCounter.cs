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
            PartitionKey = KRT.Year.ToString() + "년" + KRT.Month.ToString() + "월" + KRT.Day.ToString() + "일";

            string rowKey = "";
            if (KRT.Hour > 0 && KRT.Hour < 10)
            {
                rowKey = "0" + KRT.Hour.ToString() + "시";
            }
            else
            {
                rowKey = KRT.Hour.ToString() + "시";
            }

            RowKey = rowKey;
            Number = 1;
        }

        public int Number { get; set; }
    }
}