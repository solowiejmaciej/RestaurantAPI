using Microsoft.AspNetCore.Authorization;

namespace RestaurantAPI.Authorization;

public class MinAgeRequirement : IAuthorizationRequirement
{
    public int MinimumAge { get; }

    public MinAgeRequirement(int minimumAge)
    {
        MinimumAge = minimumAge;
    }
}