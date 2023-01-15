using Microsoft.AspNetCore.Authorization;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Threading.Tasks;

namespace EF_DotNetCore.Security
{
    public class ManageSuperAdmin : AuthorizationHandler<ManageAdminRolesAndClaimRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, ManageAdminRolesAndClaimRequirement requirement)
        {
            if(context.User.IsInRole("SuperAdmin"))
            {
                context.Succeed(requirement);
            }
            return Task.CompletedTask;  
        }
        
        
    }
}
