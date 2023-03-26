using RestaurantAPI.Models;

namespace RestaurantAPI.Services;

public interface IDishService
{
    int CreateDishInRestaurant(int restaurantId, DishBodyDto dto);

    DishDto GetDishById(int restaurantId, int dishId);

    List<DishDto> GetDishes(int restaurantId);

    void DeleteAllDishes(int restaurantId);
}