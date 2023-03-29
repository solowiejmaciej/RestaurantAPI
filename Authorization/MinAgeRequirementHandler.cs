using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace RestaurantAPI.Authorization;

public class MinAgeRequirementHandler : AuthorizationHandler<MinAgeRequirement>
{
    private readonly ILogger _logger;

    public MinAgeRequirementHandler(ILogger<MinAgeRequirementHandler> logger)
    {
        _logger = logger;
    }

    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, MinAgeRequirement requirement)
    {
        var dateOfBirth = DateTime.Parse(context.User.FindFirst(c => c.Type == "DateOfBirth").Value);

        var user = context.User.FindFirst(c => c.Type == ClaimTypes.Name).Value;

        _logger.LogInformation($"User {user} born on {dateOfBirth} is trying to auth");

        if (dateOfBirth.AddYears(requirement.MinimumAge) <= DateTime.Today)
        {
            _logger.LogInformation($"Auth succedded");
            context.Succeed(requirement);
        }
        else
        {
            _logger.LogInformation($"Auth failed");
        }

        return Task.CompletedTask;
    }
}