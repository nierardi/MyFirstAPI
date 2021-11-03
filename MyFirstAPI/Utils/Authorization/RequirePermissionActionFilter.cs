using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using MyFirstAPI.Models;

namespace MyFirstAPI.Utils.Authorization
{
    public class RequirePermissionActionFilter : IAsyncActionFilter
    {
        private readonly TodoContext dbContext;
        public string Permission { get; set; }

        public RequirePermissionActionFilter(/* DI */ TodoContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            // Check if we're logged in

            // Token exists?
            var token = context.HttpContext.Request.Cookies["SessionToken"];
            if (token == null)
            {
                context.Result = new ForbidResult();
                return;
            }

            // Session exists?
            var session = await dbContext.Sessions.FirstOrDefaultAsync(s => s.Token == token);
            if (session == null)
            {
                context.Result = new ForbidResult();
                return;
            }

            // User exists?
            var user = await dbContext.Users.FirstOrDefaultAsync(u => u.Username == session.Username);
            if (user == null)
            {
                context.Result = new ForbidResult();
                return;
            }

            // User has permission?
            var perm = await dbContext.Permissions
                .FirstOrDefaultAsync(p => p.Username == user.Username && p.PermissionName == Permission);
            if (perm == null)
            {
                // No permission to do this
                context.Result = new ForbidResult();
                return;
            }

            // We are logged in and have permission, we're good to go
            // Call next.Invoke() to run the rest of the action, if you want to skip the action, use next.InvokeNext()
            await next.Invoke();
        }
    }
}