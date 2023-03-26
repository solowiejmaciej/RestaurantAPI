﻿using FluentValidation;
using RestaurantAPI.Entities;

namespace RestaurantAPI.Models.Validators;

public class RegisterUserDtoValidator : AbstractValidator<RegisterUserDto>
{
    public RegisterUserDtoValidator(RestaurantDBContext dbContext)
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress();
        RuleFor(x => x.Password)
            .NotEmpty()
            .MinimumLength(6);
        RuleFor(x => x.ConfirmPassword).Equal(e => e.Password);

        RuleFor(x => x.Email).Custom(
            (value, context) =>
            {
                var emailInUse = dbContext.Users.Any(u => u.Email == value);

                if (emailInUse)
                {
                    context.AddFailure("Email", "Already in use");
                }
            });
    }
}