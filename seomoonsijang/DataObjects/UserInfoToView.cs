using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace seomoonsijang.DataObjects
{
    public class UserInfoToView
    {
        public UserInfoToView() { }

        public List<string> ID { get; set; }
        public List<string> ShopLocation { get; set; }
        public List<string> ShopName { get; set; }
        public List<string> Payment { get; set; }
        public List<string> AddInfo { get; set; }
        public List<string> PhoneNumber { get; set; }
        public List<bool> Paid { get; set; }
    }


}