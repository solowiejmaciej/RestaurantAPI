using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RestaurantAPI.Models;
using RestaurantAPI.Services;

namespace RestaurantAPI.Controllers;

[Authorize]
[ApiController]
[Route("api/restaurant/{restaurantId}/dish")]
public class DishController : Controller
{
    private readonly IDishService _dishService;

    public DishController(IDishService dishService)
    {
        _dishService = dishService;
    }

    [HttpPost]
    public ActionResult PostDish([FromRoute] int restaurantId, [FromBody] DishBodyDto dto)
    {
        var newDishId = _dishService.CreateDishInRestaurant(restaurantId, dto);

        return Created($"api/restaurant/{restaurantId}/dish/{newDishId}", null);
    }

    [HttpGet("{dishId}")]
    public ActionResult<DishDto> GetDish([FromRoute] int restaurantId, [FromRoute] int dishId)
    {
        var dish = _dishService.GetDishById(restaurantId, dishId);

        return Ok(dish);
    }

    [HttpGet]
    public ActionResult<List<DishDto>> GetDish([FromRoute] int restaurantId)
    {
        var dishes = _dishService.GetDishes(restaurantId);
        return Ok(dishes);
    }

    [HttpDelete]
    public ActionResult DeleteAllDishes([FromRoute] int restaurantId)
    {
        _dishService.DeleteAllDishes(restaurantId);
        return NoContent();
    }
}