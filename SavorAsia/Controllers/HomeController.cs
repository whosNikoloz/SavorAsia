using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Newtonsoft.Json;
using SavorAsia.Data;
using SavorAsia.Models;
using SavorAsia.Models.Cart;
using SavorAsia.Models.Menu;
using SavorAsia.Models.Order;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO.Pipelines;
using System.Xml;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace SavorAsia.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly string _connectionString;
        private readonly MenuDbContext _db;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly OrderDbContext _orderdb;

        public HomeController(ILogger<HomeController> logger, IConfiguration configuration, MenuDbContext db, UserManager<IdentityUser> userManager,OrderDbContext orderContext)
        {
            _logger = logger;
            _connectionString = configuration.GetConnectionString("DefaultConnection");
            _db = db;
            _userManager = userManager; 
            _orderdb = orderContext;
        }

        public IActionResult Index()
        {
            return View();
        } 


        private readonly string queryNoodles = "SELECT id, Name,Description, Data,Price,CategoryId FROM Noodles";
        private readonly string queryRice = "SELECT id, Name,Description, Data,Price,CategoryId FROM Rice";
        private readonly string queryDrink = "SELECT id, Name,Description, Data,Price,CategoryId FROM Drinks";
        private readonly string queryRamen = "SELECT id, Name,Description, Data,Price,CategoryId FROM Ramen";


        [Authorize(Roles = "User,Admin")]
        [HttpGet]
        public async Task<IActionResult> Menu(string item)
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

                case "Drinks":
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
        private async Task<List<ItemModel>> GetItemsAsync(string querySting)
        {
            List<ItemModel> items = new List<ItemModel>();
            string query = "";

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


        [HttpPost]
        [Route("/Home/ReceiveCartData")]
        public async Task<IActionResult> ReceiveCartDataAsync([FromBody] List<Product> cartData)
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);

            decimal orderTotal = 0;
            var order = new OrderModel
            {
                UserId = user.Id,
                CustomerName = user.UserName,
                Email = user.Email,
                Phone = user.PhoneNumber
            };


            var items = new List<OrderItemModel>();


            foreach (Product product in cartData)
            {
                var item = new OrderItemModel
                {
                    ProductId = product.Id,
                    CategoryId = product.CategoryId,
                    ProductName = product.Name,
                    Quantity = product.Count,
                    Price = product.Price
                };
                orderTotal += product.Price;
                items.Add(item);
            }

            order.OrderTotal = orderTotal;

            string listString = JsonConvert.SerializeObject(items);

            order.OrderedItems = listString;

            _orderdb.Orders.Add(order);
            await _orderdb.SaveChangesAsync();

            

            return RedirectToAction("Index", "Home");
        }



        [HttpGet]
        public IActionResult Product(int id,int proudctId)
        {
            /*
             Noodles = 0;
            Rice = 1;
            Drinks =2;
            Ramen = 3;
             */
            ItemModel model = new ItemModel();
            switch (proudctId)
            {
                case 0:
                    NoodlesModel noodles = _db.Noodles.Find(id);
                    model.Id=noodles.Id;
                    model.Name = noodles.Name;
                    model.Description = noodles.Description;
                    model.Price = noodles.Price;
                    model.Data = noodles.Data;
                    break;
                case 1:
                    RiceModel rice = _db.Rice.Find(id);
                    model.Id=rice.Id;
                    model.Name = rice.Name;
                    model.Description = rice.Description;
                    model.Price = rice.Price;
                    model.Data = rice.Data;
                    break;
                case 2:
                    DrinkModel drinks = _db.Drinks.Find(id);
                    model.Id=drinks.Id;
                    model.Name = drinks.Name;
                    model.Description = drinks.Description;
                    model.Price = drinks.Price;
                    model.Data = drinks.Data;
                    break;
                case 3:
                    RamenModel ramen = _db.Ramen.Find(id);
                    model.Id=ramen.Id;
                    model.Name = ramen.Name;
                    model.Description = ramen.Description;
                    model.Price = ramen.Price;
                    model.Data = ramen.Data;
                    break;
            };

            return View(model);
        }

        public IActionResult Privacy()
        {
            return View();
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}