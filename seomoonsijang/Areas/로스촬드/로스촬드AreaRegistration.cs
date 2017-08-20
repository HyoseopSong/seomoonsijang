using System.Web.Mvc;

namespace seomoonsijang.Areas.로스촬드
{
    public class 로스촬드AreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "로스촬드";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "로스촬드_default",
                "로스촬드/{controller}/{action}/{id}",
                new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}