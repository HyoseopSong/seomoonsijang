using System.Web.Mvc;

namespace seomoonsijang.Areas.쇠젓가락
{
    public class 쇠젓가락AreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "쇠젓가락";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "쇠젓가락_default",
                "쇠젓가락/{controller}/{action}/{id}"
                //new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}