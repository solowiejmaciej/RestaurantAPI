using Microsoft.AspNetCore.Mvc;
using RestaurantAPI.Models;
using RestaurantAPI.Services;

namespace RestaurantAPI.Controllers;

[ApiController]
[Route("api/account")]
public class AccountController : Controller
{
    private readonly IAccountService _accountService;

    public AccountController(IAccountService accountService)
    {
        _accountService = accountService;
    }

    [HttpPost("register")]
    public ActionResult RegisterUser([FromBody] RegisterUserDto dto)
    {
        _accountService.RegisterUser(dto);
        return Ok();
    }

    [HttpPost("login")]
    public ActionResult Login([FromBody] LoginUserDto dto)
    {
        string token = _accountService.GenerateJWT(dto);
        return Ok(token);
    }
}