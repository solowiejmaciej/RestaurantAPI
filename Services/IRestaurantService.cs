using RestaurantAPI.Models;
using System.Security.Claims;

namespace RestaurantAPI.Services;

public interface IRestaurantService
{
    RestaurantDto GetRestaurantById(int id);

    PageResult<RestaurantDto> GetAllRestaurants(GetAllRestaruantQuery query);

    int CreateNewRestaurant(CreateRestuarantDto restaurantDto);

    void DeleteRestaurant(int id);

    RestaurantDto EditRestaurant(int id, EditRestaurant newRestaurantDto);
}