using Microsoft.AspNetCore.Mvc.Filters;

namespace API.Helpers;

public class LogUserActivity : IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var resulContext = await next();

        if(!resulContext.HttpContext.User.Identity.IsAuthenticated) return;
 
        var userId= resulContext.HttpContext.User.GetUserId();
        var ouw = resulContext.HttpContext.RequestServices.GetService<IUnitOfWork>();

        var user = await ouw.UserRepository.GetUserByIdAsync(userId);
        user.LastActive = DateTime.UtcNow;
        await ouw.Complete();

    }
} 