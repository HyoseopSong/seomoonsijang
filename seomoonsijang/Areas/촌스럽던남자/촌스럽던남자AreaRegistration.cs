using System.Web.Mvc;

namespace seomoonsijang.Areas.촌스럽던남자
{
    public class 촌스럽던남자AreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "촌스럽던남자";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "촌스럽던남자_default",
                "촌스럽던남자/{controller}/{action}/{id}",
                new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}