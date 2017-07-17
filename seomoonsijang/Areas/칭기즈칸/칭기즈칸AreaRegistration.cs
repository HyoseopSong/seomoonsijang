using System.Web.Mvc;

namespace seomoonsijang.Areas.칭기즈칸
{
    public class 칭기즈칸AreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "칭기즈칸";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "칭기즈칸_default",
                "칭기즈칸/{controller}/{action}/{id}",
                new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}