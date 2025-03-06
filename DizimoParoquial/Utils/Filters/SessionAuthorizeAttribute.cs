using DizimoParoquial.DTOs;
using DizimoParoquial.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace DizimoParoquial.Utils.Filters
{
    public class SessionAuthorizeAttribute : ActionFilterAttribute
    {

        private readonly string _interfaceName;

        public SessionAuthorizeAttribute(string interfaceName)
        {
            _interfaceName = interfaceName;
        }

        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            PermissionService? permissionService = context.HttpContext.RequestServices.GetService<PermissionService>();

            if(permissionService != null)
            {
                var sessionUser = context.HttpContext.Session.GetInt32("User");

                if(sessionUser != null)
                {
                    int idUser = sessionUser.Value;

                    List<UserPermissionDTO> userPermissions = await permissionService.GetUserPermissions(idUser);

                    if (!userPermissions.Exists(p => p.Name.Equals(_interfaceName)))
                    {
                        context.Result = new RedirectToActionResult("Error401", "Error", null);
                        return;
                    }
                }
            }

            await next();

        }

    }
}
