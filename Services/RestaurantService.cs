using System.Linq.Expressions;
using System.Security.Claims;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using RestaurantAPI.Authorization;
using RestaurantAPI.Entities;
using RestaurantAPI.Exceptions;
using RestaurantAPI.Models;

namespace RestaurantAPI.Services;

public class RestaurantService : IRestaurantService
{
    private readonly RestaurantDBContext _dbContext;
    private readonly IMapper _mapper;
    private readonly ILogger _logger;
    private readonly IAuthorizationService _authorizationService;
    private readonly IUserContextService _userContextService;

    public RestaurantService(RestaurantDBContext dbContext, IMapper mapper, ILogger<RestaurantService> logger,
        IAuthorizationService authorizationService, IUserContextService userContextService)// Wstrzykiwanie zaleznosci poprzez konstruktor
    {
        _logger = logger;
        _mapper = mapper;
        _dbContext = dbContext;
        _authorizationService = authorizationService;
        _userContextService = userContextService;
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

    public PageResult<RestaurantDto> GetAllRestaurants(GetAllRestaruantQuery? query)
    {
        var baseQuery = _dbContext //szukanie restauracji po id
            .Restaurants
            .Include(r => r.Address)
            .Include(r => r.Dishes)
            .Where(r => query.SearchPhrase == null ||
                        (r.Name.ToLower().Contains(query.SearchPhrase.ToLower())
                         || r.Description.ToLower().Contains(query.SearchPhrase.ToLower())));

        if (!string.IsNullOrEmpty(query.SortBy))
        {
            var columnsSelectors = new Dictionary<string, Expression<Func<Restaurant, object>>>
            {
                {nameof(Restaurant.Name), r => r.Name},
                {nameof(Restaurant.Description), r => r.Description},
                {nameof(Restaurant.Category), r => r.Category},
            };

            var selectedColumn = columnsSelectors[query.SortBy];

            baseQuery = query.SortDirection == SortDirection.ASC
                ? baseQuery.OrderBy(selectedColumn)
                : baseQuery.OrderByDescending(selectedColumn);
        }

        var restaurants = baseQuery
            .Skip(query.PageSize * (query.PageNumber - 1)) //Paginacja
            .Take(query.PageSize)
            .ToList();

        var restaurantsDtos = _mapper.Map<List<RestaurantDto>>(restaurants);

        var totalItemsCount = baseQuery.Count();

        var result = new PageResult<RestaurantDto>(restaurantsDtos, totalItemsCount, query.PageSize, query.PageNumber);

        return result;
    }

    public int CreateNewRestaurant(CreateRestuarantDto restaurantDto)
    {
        var restaurant = _mapper.Map<Restaurant>(restaurantDto);
        restaurant.CreatedById = _userContextService.GetUserId;
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

        if (restaurant == null) throw new NotFoundException("Restaurant not found");

        var authorizationResult = _authorizationService.AuthorizeAsync(_userContextService.User, restaurant, new ResourceOperationRequirement(ResourceOperation.Delete)).Result;

        if (!authorizationResult.Succeeded)
        {
            throw new ForbidException();
        }

        _dbContext.Restaurants.Remove(restaurant);
        _dbContext.SaveChanges();
    }

    public RestaurantDto EditRestaurant(int id, EditRestaurant newRestaurantDto)
    {
        var restaurant = _dbContext //szukanie restauracji po id
            .Restaurants
            .FirstOrDefault(r => r.Id == id);

        if (restaurant is null) throw new NotFoundException("Restaurant not found");

        var authorizationResult = _authorizationService.AuthorizeAsync(_userContextService.User, restaurant, new ResourceOperationRequirement(ResourceOperation.Update)).Result;

        if (!authorizationResult.Succeeded)
        {
            throw new ForbidException();
        }

        restaurant.Name = newRestaurantDto.Name;
        restaurant.Description = newRestaurantDto.Description;
        restaurant.HasDelivery = newRestaurantDto.HasDelivery;

        _dbContext.SaveChanges();

        return GetRestaurantById(id);
    }
}