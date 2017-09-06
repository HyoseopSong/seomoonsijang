using System.Web.Mvc;

namespace seomoonsijang.Areas.복덩이
{
    public class 복덩이AreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "복덩이";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "복덩이_default",
                "복덩이/{controller}/{action}/{id}",
                new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}