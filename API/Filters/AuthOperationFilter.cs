using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace API.Filters;

public class AuthOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        //***********************************************************************
        //Uncomment this if you want to used authentication local
        //***********************************************************************
        // var isAuthorized = context.MethodInfo.DeclaringType.GetCustomAttributes(true).OfType<AuthorizeAttribute>().Any() ||
        //                  context.MethodInfo.GetCustomAttributes(true).OfType<AuthorizeAttribute>().Any();

        //NOTE:
        // se pueden omitir algunos controladores para que no aparescan por seguridad si es necesario
        //   context.MethodInfo.Name;
            
        // if (!isAuthorized) return;

        //***********************************************************************

        operation.Responses.TryAdd("401", new OpenApiResponse { Description = "Unauthorized" });
        operation.Responses.TryAdd("403", new OpenApiResponse { Description = "Forbidden" });

        //***********************************************************************
        //Uncomment this if you want to used authentication local
        //***********************************************************************
        var jwtbearerScheme = new OpenApiSecurityScheme
        {
            Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
        };
            
        operation.Security = new List<OpenApiSecurityRequirement>
            {
                new OpenApiSecurityRequirement
                {
                    [ jwtbearerScheme ] = new string [] { }
                }
            };

        if (operation.Parameters == null)
            operation.Parameters = new List<OpenApiParameter>();

        //*********************************************************************** 
    }
}