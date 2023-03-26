using AutoMapper;
using Microsoft.EntityFrameworkCore;
using RestaurantAPI.Entities;
using RestaurantAPI.Exceptions;
using RestaurantAPI.Models;

namespace RestaurantAPI.Services;

public class DishService : IDishService
{
    private readonly RestaurantDBContext _dbContext;
    private readonly IMapper _mapper;

    private Restaurant GetRestaurantById(int restaurantId)
    {
        var restaurant = _dbContext
            .Restaurants
            .Include(r => r.Dishes)
            .FirstOrDefault(r => r.Id == restaurantId);
        if (restaurant == null) throw new NotFoundException("Restaurant not found");
        return restaurant;
    }

    public DishService(RestaurantDBContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }

    public int CreateDishInRestaurant(int restaurantId, DishBodyDto dto)
    {
        var restaurant = GetRestaurantById(restaurantId);

        var dishEntity = _mapper.Map<Dish>(dto);

        dishEntity.RestaurantId = restaurant.Id;

        _dbContext.Dishes.Add(dishEntity);
        _dbContext.SaveChanges();

        return dishEntity.Id;
    }

    public DishDto GetDishById(int restaurantId, int dishId)
    {
        var restaurant = GetRestaurantById(restaurantId);

        var dish = _dbContext.Dishes.FirstOrDefault(d => d.Id == dishId);
        if (dish is null || dish.RestaurantId != restaurant.Id)
        {
            throw new NotFoundException("Dish not found");
        }

        var dishDto = _mapper.Map<DishDto>(dish);
        return dishDto;
    }

    public List<DishDto> GetDishes(int restaurantId)
    {
        var restaurant = GetRestaurantById(restaurantId);

        var dish = _dbContext.Dishes.Where(d => d.RestaurantId == restaurant.Id);

        var dishDtos = _mapper.Map<List<DishDto>>(dish);

        return dishDtos;
    }

    public void DeleteAllDishes(int restaurantId)
    {
        var restaurant = GetRestaurantById(restaurantId);
        _dbContext.RemoveRange(restaurant.Dishes);
        _dbContext.SaveChanges();
    }
}