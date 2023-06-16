using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.DotNet.Scaffolding.Shared.Project;
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
            if(item == null)
            {
                item = "Noodles";
            }
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







        //Edit !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!

        [HttpGet]
        public IActionResult Edit(int productid,int categoryid)
        {
            /*
             Noodles = 0;
            Rice = 1;
            Drinks =2;
            Ramen = 3;
             */

            switch (categoryid)
            {
                case 0:
                    var noodles = db.Noodles.Find(productid);
                    var Noodlemodel = new ItemModel
                    {
                        Price = noodles.Price,
                        CategoryId= noodles.CategoryId,
                        Id = noodles.Id,
                        Description = noodles.Description,
                        Data = noodles.Data,
                        Name = noodles.Name,
                    };
                    return View(Noodlemodel);
                case 1:
                    var rice = db.Rice.Find(productid);
                    var Ricemodel = new ItemModel
                    {
                        Price = rice.Price,
                        CategoryId = rice.CategoryId,
                        Id = rice.Id,
                        Description = rice.Description,
                        Data = rice.Data,
                        Name = rice.Name,
                    };
                    return View(Ricemodel);
                case 2:
                    var drink = db.Drinks.Find(productid);
                    var Drinkmodel = new ItemModel
                    {
                        Price = drink.Price,
                        CategoryId = drink.CategoryId,
                        Id = drink.Id,
                        Description = drink.Description,
                        Data = drink.Data,
                        Name = drink.Name,
                    };
                    return View(Drinkmodel);
                case 3:
                    var ramen = db.Ramen.Find(productid);
                    var Ramenmodel = new ItemModel
                    {
                        Price = ramen.Price,
                        CategoryId = ramen.CategoryId,
                        Id = ramen.Id,
                        Description = ramen.Description,
                        Data = ramen.Data,
                        Name = ramen.Name,
                    };
                    return View(Ramenmodel);
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Edit(ItemModel model)
        {
            /*
             Noodles = 0;
            Rice = 1;
            Drinks =2;
            Ramen = 3;
             */
            switch (model.CategoryId)
            {
                case 0:
                    var noodles = await db.Noodles.FindAsync(model.Id);
                    if(noodles != null)
                    {
                        noodles.Price = model.Price;
                        noodles.Description = model.Description;
                        noodles.Data = model.Data;
                        noodles.Name = model.Name;
                        noodles.Id = model.Id;
                        noodles.CategoryId = model.CategoryId;
                    }
                    else
                    {
                        throw new Exception("noodles not found");
                    }
                    await db.SaveChangesAsync();
                    return RedirectToAction("Index");
                case 1:
                    var rice = await db.Rice.FindAsync(model.Id);
                    if (rice != null)
                    {
                        rice.Price = model.Price;
                        rice.Description = model.Description;
                        rice.Data = model.Data;
                        rice.Name = model.Name;
                        rice.Id = model.Id;
                        rice.CategoryId = model.CategoryId;
                    }
                    else
                    {
                        throw new Exception("rice not found");
                    }
                    await db.SaveChangesAsync();
                    return RedirectToAction("Index");
                case 2:
                    var drink = await db.Drinks.FindAsync(model.Id);
                    if (drink != null)
                    {
                        drink.Price = model.Price;
                        drink.Description = model.Description;
                        drink.Data = model.Data;
                        drink.Name = model.Name;
                        drink.Id = model.Id;
                        drink.CategoryId = model.CategoryId;
                    }
                    else
                    {
                        throw new Exception("drink not found");
                    }
                    await db.SaveChangesAsync();
                    return RedirectToAction("Index");
                case 3:
                    var ramen = await db.Ramen.FindAsync(model.Id);
                    if (ramen != null)
                    {
                        ramen.Price = model.Price;
                        ramen.Description = model.Description;
                        ramen.Data = model.Data;
                        ramen.Name = model.Name;
                        ramen.Id = model.Id;
                        ramen.CategoryId = model.CategoryId;
                    }
                    else
                    {
                        throw new Exception("Ramen not found");
                    }
                    await db.SaveChangesAsync();
                    return RedirectToAction("Index");
            }
            return RedirectToAction("Index");

        }


        //Delete !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!


        [HttpPost]
        public async Task<IActionResult> Delete(ItemModel model)
        {
            /*
             Noodles = 0;
            Rice = 1;
            Drinks =2;
            Ramen = 3;
             */
            switch (model.CategoryId)
            {
                case 0:
                    var noodles = await db.Noodles.FindAsync(model.Id);
                    if(noodles != null)
                    {
                        db.Noodles.Remove(noodles);
                        await db.SaveChangesAsync();
                    }
                    return RedirectToAction("Index");
                case 1:
                    var rice = await db.Rice.FindAsync(model.Id);
                    if (rice != null)
                    {
                        db.Rice.Remove(rice);
                        await db.SaveChangesAsync();
                    }
                    return RedirectToAction("Index");
                case 2:
                    var drink = await db.Drinks.FindAsync(model.Id);
                    if (drink != null)
                    {
                        db.Drinks.Remove(drink);
                        await db.SaveChangesAsync();
                    }
                    return RedirectToAction("Index");
                case 3:
                    var ramen = await db.Ramen.FindAsync(model.Id);
                    if (ramen != null)
                    {
                        db.Ramen.Remove(ramen);
                        await db.SaveChangesAsync();
                    }
                    return RedirectToAction("Index");
            }
            return RedirectToAction("Index");
        }



        [HttpGet]
        public async Task<IActionResult> Delete(int productid, int categoryid)
        {
            /*
             Noodles = 0;
            Rice = 1;
            Drinks =2;
            Ramen = 3;
             */
            switch (categoryid)
            {
                case 0:
                    var noodles = await db.Noodles.FindAsync(productid);
                    if (noodles != null)
                    {
                        db.Noodles.Remove(noodles);
                        await db.SaveChangesAsync();
                    }
                    return RedirectToAction("Index");
                case 1:
                    var rice = await db.Rice.FindAsync(productid);
                    if (rice != null)
                    {
                        db.Rice.Remove(rice);
                        await db.SaveChangesAsync();
                    }
                    return RedirectToAction("Index");
                case 2:
                    var drink = await db.Drinks.FindAsync(productid);
                    if (drink != null)
                    {
                        db.Drinks.Remove(drink);
                        await db.SaveChangesAsync();
                    }
                    return RedirectToAction("Index");
                case 3:
                    var ramen = await db.Ramen.FindAsync(productid);
                    if (ramen != null)
                    {
                        db.Ramen.Remove(ramen);
                        await db.SaveChangesAsync();
                    }
                    return RedirectToAction("Index");
            }
            return RedirectToAction("Index");
        }



        //Add Items !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
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
