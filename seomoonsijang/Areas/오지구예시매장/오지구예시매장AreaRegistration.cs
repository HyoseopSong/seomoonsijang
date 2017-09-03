using System.Web.Mvc;

namespace seomoonsijang.Areas.오지구예시매장
{
    public class 오지구예시매장AreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "오지구예시매장";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "오지구예시매장_default",
                "오지구예시매장/{controller}/{action}/{id}",
                new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}