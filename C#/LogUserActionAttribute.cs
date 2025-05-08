using Microsoft.AspNetCore.Mvc.Filters;
using ConstructionCompany.Models;
using System.Security.Claims;

namespace ConstructionCompany
{
    public class LogUserActionAttribute : ActionFilterAttribute
    {
        public string ActionDescription { get; set; }

        public override void OnActionExecuted(ActionExecutedContext context)
        {
            var dbContext = context.HttpContext.RequestServices.GetService(typeof(ConstructionCompanyDbContext)) as ConstructionCompanyDbContext;

            var username = context.HttpContext.User.Identity?.Name;

            if (dbContext != null && username != null)
            {
                var user = dbContext.Users.FirstOrDefault(u => u.Username == username);
                if (user != null)
                {
                    var log = new UserActionLog
                    {
                        UserId = user.UserId,
                        ActionDescription = ActionDescription ?? context.ActionDescriptor.DisplayName
                    };

                    dbContext.UserActionLogs.Add(log);
                    dbContext.SaveChanges();
                }
            }

            base.OnActionExecuted(context);
        }
    }
}