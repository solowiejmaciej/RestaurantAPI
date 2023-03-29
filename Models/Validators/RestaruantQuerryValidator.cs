using FluentValidation;
using RestaurantAPI.Entities;

namespace RestaurantAPI.Models.Validators;

public class RestaruantQuerryValidator : AbstractValidator<GetAllRestaruantQuery>
{
    private int[] allowedPageSizes = new[] { 5, 10, 15 };

    private string[] allowedSortByColumnNames =
    {
        nameof(Restaurant.Name),
        nameof(Restaurant.Description),
        nameof(Restaurant.Category),
    };

    public RestaruantQuerryValidator()
    {
        RuleFor(r => r.PageNumber)
            .GreaterThanOrEqualTo(1);
        RuleFor(r => r.PageSize)
            .Custom((value, context) =>
                {
                    if (!allowedPageSizes.Contains(value))
                    {
                        context.AddFailure("PageSize", $"Page size must be in[{string.Join(",", allowedPageSizes)}]");
                    }
                }

            );

        RuleFor(r => r.SortBy).Must(
            value => string.IsNullOrEmpty(value) || allowedSortByColumnNames.Contains(value)
        ).WithMessage($"Sortby is optional or must be in [{string.Join(",", allowedSortByColumnNames)}]");
    }
}