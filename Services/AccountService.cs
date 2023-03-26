using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using RestaurantAPI.Entities;
using RestaurantAPI.Exceptions;
using RestaurantAPI.Models;

namespace RestaurantAPI.Services;

public class AccountService : IAccountService
{
    private readonly RestaurantDBContext _dbContext;
    private readonly IPasswordHasher<User> _passwordHasher;
    private readonly AuthSettings _authSettings;

    public AccountService(RestaurantDBContext dbContext, IPasswordHasher<User> passwordHasher, AuthSettings authSettings)
    {
        _dbContext = dbContext;
        _passwordHasher = passwordHasher;
        _authSettings = authSettings;
    }

    public void RegisterUser(RegisterUserDto dto)
    {
        var newUser = new User()
        {
            Email = dto.Email,
            Birthday = dto.Birthday.Date,
            Nationality = dto.Nationality,
            RoleId = dto.RoleId,
        };

        var hashedPassword = _passwordHasher.HashPassword(newUser, dto.Password);
        newUser.PasswordHash = hashedPassword;

        _dbContext.Users.Add(newUser);
        _dbContext.SaveChanges();
    }

    public string GenerateJWT(LoginUserDto dto)
    {
        //Sprawdza czy user o podanym mailu jest w db
        var user = _dbContext.Users
            .Include(u => u.Role)
            .FirstOrDefault(x => x.Email == dto.Email);
        if (user is null)
        {
            throw new BadRequestException("Invalid username or password");
        }
        //Na obiekcie user pobranym z db sprawdza czy haslo z bazy danych jest zgodne z haslem z LoginUserDto
        var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, dto.Password);

        if (result == PasswordVerificationResult.Failed)
        {
            throw new BadRequestException("Invalid username or password");
        }

        var claims = new List<Claim>() // Tworzenie calimow do wygenerowania JWT
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, $"{user.FirstName} {user.LastName}"),
            new Claim(ClaimTypes.Role, $"{user.Role.Name}"),
            new Claim("DateOfBirth", $"{user.Birthday.Value.ToString("yyyy-MM-dd")}"),
            new Claim("Nationality", user.Nationality)
        };
        // Klucz na podstawie jakiego ma byc wygenerowane jwt
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_authSettings.JwtKey));
        // Algorytm szyfrowania
        var cred = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        // Ważność tokenu
        var expires = DateTime.Now.AddDays(_authSettings.JwtExpireDays);
        //Tworzenie tokenu
        var token = new JwtSecurityToken(_authSettings.JwtIssuer,
            _authSettings.JwtIssuer,
            claims,
            expires: expires,
            signingCredentials: cred);
        //Zwracanie tokena przez tokenHandler
        var tokenHandler = new JwtSecurityTokenHandler();
        return tokenHandler.WriteToken(token);
    }
}