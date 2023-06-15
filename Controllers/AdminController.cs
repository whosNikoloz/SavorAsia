using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SavorAsia.Data;
using SavorAsia.Models;
using SavorAsia.Models.Menu;
using System.Data;
using System.Data.SqlClient;

namespace SavorAsia.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly string _connectionString;
        private readonly MenuDbContext db;

        public AdminController(IConfiguration configuration, MenuDbContext db)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
            this.db = db;
        }

        private readonly string queryNoodles = "SELECT id, Name,Description, Data,Price,CategoryId FROM Noodles";
        private readonly string queryRice = "SELECT id, Name,Description, Data,Price,CategoryId FROM Rice";
        private readonly string queryDrink = "SELECT id, Name,Description, Data,Price,CategoryId FROM Drinks";
        private readonly string queryRamen = "SELECT id, Name,Description, Data,Price,CategoryId FROM Ramen";


        private async Task<List<ItemModel>> GetItemsAsync(string querySting)
        {
            List<ItemModel> items = new List<ItemModel>();
            string query = queryNoodles;

            switch (querySting)
            {
                case "Noodles":
                    query = queryNoodles;
                    break;
                case "Rice":
                    query = queryRice;
                    break;
                case "Drinks":
                    query = queryDrink;
                    break;
                case "Ramen":
                    query = queryRamen;
                    break;
            }


            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                SqlCommand command = new SqlCommand(query, connection);
                command.CommandTimeout = 120;

                await connection.OpenAsync();

                using (SqlDataReader reader = await command.ExecuteReaderAsync(CommandBehavior.CloseConnection))
                {
                    while (await reader.ReadAsync())
                    {
                        ItemModel item = new ItemModel();

                        item.Id = reader.GetInt32(reader.GetOrdinal("id"));
                        item.CategoryId = reader.GetInt32(reader.GetOrdinal("CategoryId"));
                        item.Name = reader.GetString(reader.GetOrdinal("Name"));
                        item.Description = reader.GetString(reader.GetOrdinal("Description"));
                        item.Price = reader.GetDecimal(reader.GetOrdinal("Price"));
                        item.Data = await GetBytesAsync(reader, reader.GetOrdinal("Data"));

                        items.Add(item);
                    }
                }
            }

            return items;
        }

        private async Task<byte[]> GetBytesAsync(SqlDataReader reader, int ordinal)
        {
            const int bufferSize = 4096;
            using (MemoryStream stream = new MemoryStream())
            {
                byte[] buffer = new byte[bufferSize];
                long bytesRead;
                long fieldOffset = 0;


                await Task.Run(() =>
                {
                    while ((bytesRead = reader.GetBytes(ordinal, fieldOffset, buffer, 0, bufferSize)) > 0)
                    {
                        stream.Write(buffer, 0, (int)bytesRead);
                        fieldOffset += bytesRead;
                    }
                });

                return stream.ToArray();
            }
        }


        [HttpGet]
        public async Task<IActionResult> Index(string item)
        {
            ItemViewModel viewModel = new ItemViewModel();
            var items = await GetItemsAsync(item);
            switch (item)
            {
                case "Noodles":

                    if (items != null)
                    {
                        viewModel.noodles = items.Select(i => new NoodlesModel
                        {
                            Id = i.Id,
                            CategoryId = i.CategoryId,
                            Name = i.Name,
                            Data = i.Data,
                            Description = i.Description,
                            Price = i.Price
                        }).ToList();
                    }
                    else
                    {
                        viewModel.noodles = new List<NoodlesModel>();
                    }

                    return View(viewModel);
                case "Rice":

                    if (items != null)
                    {
                        viewModel.rice = items.Select(i => new RiceModel
                        {
                            Id = i.Id,
                            CategoryId = i.CategoryId,
                            Name = i.Name,
                            Data = i.Data,
                            Description = i.Description,
                            Price = i.Price
                        }).ToList();
                    }
                    else
                    {
                        viewModel.rice = new List<RiceModel>();
                    }

                    return View(viewModel);

                case "Drink":
                    if (items != null)
                    {
                        viewModel.drink = items.Select(i => new DrinkModel
                        {
                            Id = i.Id,
                            CategoryId = i.CategoryId,
                            Name = i.Name,
                            Data = i.Data,
                            Description = i.Description,
                            Price = i.Price
                        }).ToList();
                    }
                    else
                    {
                        viewModel.drink = new List<DrinkModel>();
                    }

                    return View(viewModel);

                case "Ramen":
                    if (items != null)
                    {
                        viewModel.ramen = items.Select(i => new RamenModel
                        {
                            Id = i.Id,
                            CategoryId = i.CategoryId,
                            Name = i.Name,
                            Data = i.Data,
                            Description = i.Description,
                            Price = i.Price
                        }).ToList();
                    }
                    else
                    {
                        viewModel.ramen = new List<RamenModel>();
                    }

                    return View(viewModel);

            };

            return View();
        }



        public IActionResult AddItem()
        {
            return View();
        }



        [HttpPost]
        public async Task<IActionResult> AddItem(IFormFile image, ItemModel Model,int item)
        {
            /*
             Noodles = 0;
            Rice = 1;
            Drinks =2;
            Ramen = 3;
             */
            using var stream = new MemoryStream();
            await image.CopyToAsync(stream);
            var imageData = stream.ToArray();
            switch (item)
            {
                case 0:
                    NoodlesModel noodlesModel = new NoodlesModel
                    {
                        Name = Model.Name,
                        CategoryId = 0,
                        Price = Model.Price,
                        Description = Model.Description,
                        Data = imageData
                    };
                    db.Noodles.Add(noodlesModel);
                    await db.SaveChangesAsync();
                    return RedirectToAction("Index", "admin");

                case 1:
                    RiceModel riceModels = new RiceModel
                    {
                        Name = Model.Name,
                        CategoryId = 1,
                        Price = Model.Price,
                        Description = Model.Description,
                        Data = imageData
                    };
                    db.Rice.Add(riceModels);
                    await db.SaveChangesAsync();
                    return RedirectToAction("Index", "admin");

                case 2:

                    DrinkModel drinkModel = new DrinkModel
                    {
                        Name = Model.Name,
                        CategoryId = 2,
                        Price = Model.Price,
                        Description = Model.Description,
                        Data = imageData
                    };

                    db.Drinks.Add(drinkModel);
                    await db.SaveChangesAsync();
                    return RedirectToAction("Index", "admin");

                case 3:
                    RamenModel ramenModel = new RamenModel
                    {
                        Name = Model.Name,
                        CategoryId = 3,
                        Price = Model.Price,
                        Description = Model.Description,
                        Data = imageData
                    };
                    db.Ramen.Add(ramenModel);
                    await db.SaveChangesAsync();
                    return RedirectToAction("Index", "admin");

            };


            return RedirectToAction("Index", "admin");
        }


    }
}
