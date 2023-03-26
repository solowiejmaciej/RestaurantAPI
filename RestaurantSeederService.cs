using RestaurantAPI.Entities;
using System.Data;

namespace RestaurantAPI;

public class RestaurantSeederService
{
    private readonly RestaurantDBContext _dbContext;

    public RestaurantSeederService(RestaurantDBContext dbContext)
    {
        _dbContext = dbContext;
    }

    public void Seed()
    {
        if (_dbContext.Database.CanConnect())
        {
            if (!_dbContext.Roles.Any())
            {
                var roles = GetRoles();
                _dbContext.Roles.AddRange(roles);
                _dbContext.SaveChanges();
            }

            if (!_dbContext.Restaurants.Any())
            {
                var restaurants = GetRestaurants();
                _dbContext.Restaurants.AddRange(restaurants);
                _dbContext.SaveChanges();
            }
        }
    }

    private IEnumerable<Restaurant> GetRestaurants()
    {
        var restaurants = new List<Restaurant>()
            {
                new Restaurant()
                {
                    Name = "KFC",
                    Category = "Fast Food",
                    Description =
                        "KFC (short for Kentucky Fried Chicken) is an American fast food restaurant chain headquartered in Louisville, Kentucky, that specializes in fried chicken.",
                    ContactEmail = "contact@kfc.com",
                    ContactNumber = "997",
                    HasDelivery = true,
                    Dishes = new List<Dish>()
                    {
                        new Dish()
                        {
                            Name = "Nashville Hot Chicken",
                            Description = "Very spicy!",
                            Price = 10.30M,
                        },

                        new Dish()
                        {
                            Name = "Chicken Nuggets",
                            Description = "Has a lot of meet",
                            Price = 5.30M,
                        },
                    },
                    Address = new Address()
                    {
                        City = "Kraków",
                        Street = "Długa 5",
                        PostalCode = "30-001"
                    }
                },
                new Restaurant()
                {
                    Name = "McDonalds",
                    Category = "Fast Food",
                    Description =
                        "Every Donald is a Mac here!",
                    ContactEmail = "contact@mcd.com",
                    ContactNumber = "112",
                    HasDelivery = true,
                    Dishes = new List<Dish>()
                    {
                        new Dish()
                        {
                            Name = "McBurger",
                            Description = "Mmm tasty cow!",
                            Price = 10.30M,
                        },

                        new Dish()
                        {
                            Name = "McWrap",
                            Description = "Very wrapyy!",
                            Price = 5.30M,
                        },
                    },
                    Address = new Address()
                    {
                        City = "Poznań",
                        Street = "Półwiejska 42",
                        PostalCode = "61-245"
                    }
                }
            };

        return restaurants;
    }

    private IEnumerable<Role> GetRoles()
    {
        var roles = new List<Role>()
        {
            new ()
            {
                Name = "User"
            },
            new ()
            {
                Name = "Manager"
            },
            new ()
            {
                Name = "Admin"
            }
        };

        return roles;
    }
}