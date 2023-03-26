using AutoMapper;
using Microsoft.EntityFrameworkCore;
using RestaurantAPI.Entities;
using RestaurantAPI.Exceptions;
using RestaurantAPI.Models;

namespace RestaurantAPI.Services;

public class RestaurantService : IRestaurantService
{
    private readonly RestaurantDBContext _dbContext;
    private readonly IMapper _mapper;
    private readonly ILogger _logger;

    public RestaurantService(RestaurantDBContext dbContext, IMapper mapper, ILogger<RestaurantService> logger)// Wstrzykiwanie zaleznosci poprzez konstruktor
    {
        _logger = logger;
        _mapper = mapper;
        _dbContext = dbContext;
    }

    public RestaurantDto GetRestaurantById(int id)
    {
        var restaurant = _dbContext //szukanie restauracji po id
            .Restaurants
            .Include(r => r.Address)
            .Include(r => r.Dishes)
            .FirstOrDefault(r => r.Id == id);

        if (restaurant is null) throw new NotFoundException("Restaurant not found");

        var restaurantDtos = _mapper.Map<RestaurantDto>(restaurant);

        return restaurantDtos;
    }

    public IEnumerable<RestaurantDto> GetAllRestaurants()
    {
        var restaurants = _dbContext //szukanie restauracji po id
            .Restaurants
            .Include(r => r.Address)
            .Include(r => r.Dishes)
            .ToList();

        var restaurantsDtos = _mapper.Map<List<RestaurantDto>>(restaurants);

        return restaurantsDtos;
    }

    public int CreateNewRestaurant(CreateRestuarantDto restaurantDto)
    {
        var restaurant = _mapper.Map<Restaurant>(restaurantDto);
        _dbContext.Add(restaurant);
        _dbContext.SaveChanges();
        return restaurant.Id;
    }

    public void DeleteRestaurant(int id)
    {
        _logger.LogWarning($"Restaurant with id: {id} Delete action invoked");

        var restaurant = _dbContext //szukanie restauracji po id
            .Restaurants
            .FirstOrDefault(r => r.Id == id);

        if (restaurant == null) throw new NotFoundException("Restaurant not found"); ;

        _dbContext.Restaurants.Remove(restaurant);
        _dbContext.SaveChanges();
    }

    public RestaurantDto EditRestaurant(int id, EditRestaurant newRestaurantDto)
    {
        var restaurant = _dbContext //szukanie restauracji po id
            .Restaurants
            .FirstOrDefault(r => r.Id == id);

        if (restaurant is null) throw new NotFoundException("Restaurant not found");

        restaurant.Name = newRestaurantDto.Name;
        restaurant.Description = newRestaurantDto.Description;
        restaurant.HasDelivery = newRestaurantDto.HasDelivery;

        _dbContext.SaveChanges();

        return GetRestaurantById(id);
    }
}