# StoreAndRestoreLayout

In this blog, we will explore how to store and restore the layout of an Essential JS 2 (EJ2) Grid on the server-side using ASP.NET Core. Storing the layout will help users preserve their customized column order, width, visibility, and sorting settings when they revisit the application. This will provide a better user experience and save users from having to reconfigure the Grid every time they access the application.

## Step-by-Step Implementation  

### Create the EJ2 Grid in the View

In the **Index.cshtml** file, create the EJ2 Grid with some sample data to store and restore the following features.
- Filtering.
- Sorting.
- Grouping.
- Paging.
- ColumnChooser (Show/Hide Columns).


```
<ejs-button id="storestate" cssClass="e-flat" isPrimary="true" content="Store State"></ejs-button>
<ejs-button id="reset" cssClass="e-flat" content="Reset"></ejs-button>
<ejs-button id="restorestate" cssClass="e-flat" content="Restore State"></ejs-button>

<div>
    <ejs-grid id="Grid" dataSource="@ViewBag.DataSource" height="500" created="created" allowFiltering="true" allowGrouping="true" allowSorting="true" allowPaging="true" showColumnChooser="true" toolbar="@(new List<string>() { "ColumnChooser"})">
        <e-grid-columns>
            <e-grid-column field="OrderID" headerText="Order ID" isPrimaryKey="true" textAlign="Right" width="120"></e-grid-column>
            <e-grid-column field="CustomerID" headerText="Customer Name" width="150"></e-grid-column>
            <e-grid-column field="EmployeeID" headerText="Employee ID" width="170"></e-grid-column>
            <e-grid-column field="Freight" headerText="Freight" format="c2" width="170"></e-grid-column>
            <e-grid-column field="ShipCity" headerText="Ship City" width="170"></e-grid-column>
            <e-grid-column field="ShipCountry" headerText="Ship Country" width="170"></e-grid-column>
        </e-grid-columns>
    </ejs-grid>
</div>

```

### Handle the Store, Reset and Restore Actions on the Client-side

   In the **script** section of the **Index.cshtml** file, addEventListener to handle the store and restore actions. When the **storestate** button is clicked, it retrieves the grid's persisted data using the **getPersistData()** method and sends it to the server via an EJ2 AJAX POST request. When the **reset** button is clicked, it get the initial grid state from localStorage and reset the grid to initial state. When the **restorestate** button is clicked, it retrieves the persisted data from the server and applies it to the grid using the **setProperties()** method.

```

   <script>
    var grid;

    function created() {
        grid = this;
        window.localStorage.setItem('initialGrid', grid.getPersistData()); // store the initial grid state to local storage on initial rendering.
    }

    document.getElementById('reset').onclick = function () {

        var savedProperties = JSON.parse(window.localStorage.getItem('initialGrid')); // get the initial grid state from local storage
        grid.setProperties(savedProperties)// reset the grid to initial state. 
    }

    document.getElementById("storestate").addEventListener('click', function () {

        // get the persisted data from the grid by using getPersistData method.
        var persistData = JSON.stringify({ persistData: grid.getPersistData() })

        var ajax = new ej.base.Ajax({ // used EJ2 ajax to send the stored persistData to server

            url: "/Home/StorePersistData",

            type: "POST",

            contentType: "application/json; charset=utf-8",

            datatype: "json",

            data: persistData // send the data to server

        });

        ajax.send();
    });
    document.getElementById("restorestate").addEventListener('click', function () {
        var ajax = new ej.base.Ajax({ // used EJ2 ajax to  retrieve the persistData from server

            url: "/Home/Restore",

            type: "POST",

            contentType: "application/json; charset=utf-8"

        });

        ajax.send();

        ajax.onSuccess = function (result) {

            var state = JSON.parse(result); // get the data

            grid.setProperties(state); // restore the Grid state

        }
    });
     

</script>

```

### Define the Data Model and DataSource of Grid

In the **HomeController.cs** file, define the data model (OrdersDetails) and the data source (GetAllRecords) for the EJ2 Grid and stored in ViewBag.DataSource, which can be used as a local data in the client side.

```
 public IActionResult Index()
{
    var Order = OrdersDetails.GetAllRecords();
    ViewBag.DataSource = Order;
    return View();
}

```

```

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

```

### Implement the Server-side Actions to store and restore in the Controller

  In the **HomeController.cs** file, implement the server-side actions to handle storing and restoring the grid's persisted data. The **StorePersistData** action receives the data from the client and stores it in a static variable. The **Restore** action simply returns the stored data.

  ```

    public static string? persistedData;

    public IActionResult StorePersistData([FromBody] StoreData persistData)
    {
        persistedData = persistData.persistData; // you can store this to the database. 
        return new JsonResult(persistedData);
    }

    public IActionResult Restore()
    {
        return Content(persistedData); // retrieve the stored layout from the database. 
    }

  ```

## Conclusion

In this blog post, we learned how to store and restore the layout of an EJ2 Grid on the server-side using ASP.NET Core and Syncfusion EJ2 Grid. The ability to persist the Grid layout enhances the user experience and allows users to maintain their preferences across sessions. You can extend this concept to handle more complex scenarios or even save the layout in a database for individual user accounts.

By following the steps and code provided in this blog, you should now have a functional implementation of storing and restoring the EJ2 Grid layout in an ASP.NET Core application. 

