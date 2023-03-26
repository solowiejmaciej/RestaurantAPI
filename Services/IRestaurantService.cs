using RestaurantAPI.Models;

namespace RestaurantAPI.Services;

public interface IRestaurantService
{
    RestaurantDto GetRestaurantById(int id);

    IEnumerable<RestaurantDto> GetAllRestaurants();

    int CreateNewRestaurant(CreateRestuarantDto restaurantDto);

    void DeleteRestaurant(int id);

    RestaurantDto EditRestaurant(int id, EditRestaurant NewData);
}