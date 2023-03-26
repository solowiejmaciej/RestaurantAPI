﻿using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RestaurantAPI.Entities;
using RestaurantAPI.Models;
using RestaurantAPI.Services;

namespace RestaurantAPI.Controllers;

[ApiController]
[Route("api/restaurant")]
[Authorize]
public class RestaurantController : Controller
{
    private readonly IRestaurantService _restaurantService;

    public RestaurantController(IRestaurantService restaurantService)
    {
        _restaurantService = restaurantService;
    }

    [HttpPost]
    public ActionResult CreateNewRestaurant([FromBody] CreateRestuarantDto newRestaurantDto)
    {
        var newRestaurantId = _restaurantService.CreateNewRestaurant(newRestaurantDto);

        return Created($"/api/restaurant/{newRestaurantId}", null);
    }

    [HttpDelete("{id}")]
    public ActionResult DeleteRestaurant([FromRoute] int id)
    {
        _restaurantService.DeleteRestaurant(id);
        return NoContent();
    }

    [HttpPut("{id}")]
    public ActionResult UpdateRestaurant([FromRoute] int id, [FromBody] EditRestaurant editedRestaurantDtoBody)
    {
        var editedRestaurant = _restaurantService.EditRestaurant(id, editedRestaurantDtoBody);

        return Ok(editedRestaurant);
    }

    [HttpGet]
    public ActionResult<IEnumerable<RestaurantDto>> GetAllRestaurants()
    {
        var restaurantsDtos = _restaurantService.GetAllRestaurants();

        return Ok(restaurantsDtos);
    }

    [HttpGet("{id}")]
    public ActionResult<RestaurantDto> GetOneRestaurantById([FromRoute] int id) // parametr ze ścieżki
    {
        var restaurantDto = _restaurantService.GetRestaurantById(id);

        if (restaurantDto is null)
        {
            return NotFound();
        }
        return Ok(restaurantDto);
    }
}