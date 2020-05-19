using Hangfire.Dashboard;

namespace ManageSystem.Manipulate
{
    /// <summary>
    /// 
    /// </summary>
    public class MyAuthorizationFilter : IDashboardAuthorizationFilter
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public bool Authorize(DashboardContext context)
        {
            var user = context.GetHttpContext().User;
            return user.Identity.IsAuthenticated && user.IsInRole("Administrator");
        }
    }
}
