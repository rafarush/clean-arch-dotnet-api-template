using System.Security.Claims; 
using Microsoft.AspNetCore.Authorization;

namespace CleanArchTemplate.Infrastructure.Services.Auth.PermissionService;

public sealed class PermissionAuthorizationHandler(IPermissionService permissionService)
    : AuthorizationHandler<PermissionRequirement>
{
    protected override async Task HandleRequirementAsync(       
        AuthorizationHandlerContext context, PermissionRequirement requirement)
    {        
        var userIdClaim = context.User.FindFirst(ClaimTypes.NameIdentifier);
        
        if (!Guid.TryParse(userIdClaim?.Value, out var userId)) 
            context.Fail();
			              
        var hasPermission = await permissionService.HasPermissionAsync(userId,
            requirement.PolicyName);
        
        if (hasPermission)
            context.Succeed(requirement);
    }
}