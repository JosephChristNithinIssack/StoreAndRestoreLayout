using Microsoft.AspNetCore.Mvc;
using netCore6x.Models;
using System.Diagnostics;
using Syncfusion.EJ2.Base;
using System.Data;
using System.Text;
using System.Collections;
using Newtonsoft.Json.Linq;
using Microsoft.VisualBasic;

namespace netCore6x.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }
        public int indexcount = 0;
        public static string? persistedData;

        public IActionResult Index()
        {
            var Order = OrdersDetails.GetAllRecords();
            ViewBag.DataSource = Order;
            return View();
        }
        public IActionResult StorePersistData([FromBody] StoreData persistData)
        {
            persistedData = persistData.persistData; // you can store this to the database. 
            return new JsonResult(persistedData);
        }

        public IActionResult Restore()
        {
            return Content(persistedData); // retrieve the stored layout from the database. 
        }

        public class StoreData
        {
            public string? persistData { get; set; }
        }

    }

    public class OrdersDetails
    {
        public static List<OrdersDetails> order = new List<OrdersDetails>();
        public OrdersDetails()
        {

        }
        public OrdersDetails(int OrderID, string CustomerId, int EmployeeId, double Freight, string ShipCity, string ShipCountry)
        {
            this.OrderID = OrderID;
            this.CustomerID = CustomerId;
            this.EmployeeID = EmployeeId;
            this.Freight = Freight;
            this.ShipCity = ShipCity;
            this.ShipCountry = ShipCountry;
        }
        public static List<OrdersDetails> GetAllRecords()
        {
            if (order.Count() == 0)
            {
                int code = 10000;
                for (int i = 1; i < 5; i++)
                {
                    order.Add(new OrdersDetails(code + 1, "ALFKI", i + 0, 0,  "Berlin", "Denmark"));
                    order.Add(new OrdersDetails(code + 2, "ANATR", i + 2, 3.3 * i,  "Madrid", "Brazil"));
                    order.Add(new OrdersDetails(code + 3, "ANTON", i + 1, 0, "Cholchester", "Germany"));
                    order.Add(new OrdersDetails(code + 4, "BLONP", i + 3, 5.3 * i,  "Marseille", "Austria"));
                    order.Add(new OrdersDetails(code + 5, "BOLID", i + 4, 6.3 * i, "Tsawassen", "Switzerland"));
                    code += 5;
                }
            }
            return order;
        }

        public int? OrderID { get; set; }
        public string CustomerID { get; set; }
        public int? EmployeeID { get; set; }
        public double? Freight { get; set; }
        public string ShipCity { get; set; }
        public string ShipCountry { get; set; }
    }

}