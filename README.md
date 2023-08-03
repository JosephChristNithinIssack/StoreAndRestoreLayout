# StoreAndRestoreLayout

In this blog, we will explore how to store and restore the layout of an Essential JS 2 (EJ2) Grid on the server-side using ASP.NET Core. Storing the layout will help users preserve their customized column order, width, visibility, and sorting settings when they revisit the application. This will provide a better user experience and save users from having to reconfigure the Grid every time they access the application.

## Prerequisites
Before proceeding, ensure that you have the following prerequisites:

* Visual Studio or Visual Studio Code. 
* .NET Core SDK.
* Syncfusion Essential JS 2 (EJ2) Grid libraries (installed using npm or available via CDN).

## Setting up the Project

* Create a new ASP.NET Core project or use an existing one.
* Install the Syncfusion.EJ2.AspNet.Core NuGet package to work with EJ2 components.

## Step-by-Step Implementation  

### Create the EJ2 Grid in the View

In the **Index.cshtml** file, create the EJ2 Grid with some sample data. The **enablePersistence** property is set to **true**, which enables the persistence of the grid's layout and customization.

```
<ejs-button id="storestate" cssClass="e-flat" isPrimary="true" content="storestate"></ejs-button>
<ejs-button id="restorestate" cssClass="e-flat" isPrimary="true" content="restorestate"></ejs-button>
<div>
    <ejs-grid id="Grid" height="500" enablePersistence="true" showColumnChooser="true" toolbar="@(new List<string>() { "ColumnChooser"})">
        <!-- ... Grid configuration and columns ... -->
    </ejs-grid>
</div>

<script>
    // ... Client-side event listeners and script ...
</script>

```

### Handle the Store and Restore Actions on the Client-side

   In the **script** section of the **Index.cshtml** file, add event listeners to handle the store and restore actions. When the **storestate** button is clicked, it retrieves the grid's persisted data using the **getPersistData()** method and sends it to the server via an AJAX POST request. When the **restorestate** button is clicked, it retrieves the persisted data from the server and applies it to the grid using the **setProperties()** method.

```

    <script>
    document.getElementById("storestate").addEventListener('click', function () {
        var grid = document.getElementById("Grid").ej2_instances[0];
        var persistData = JSON.stringify({ persistData: grid.getPersistData() });

        var ajax = new ej.base.Ajax({
            url: "/Home/StorePersistData",
            type: "POST",
            contentType: "application/json; charset=utf-8",
            datatype: "json",
            data: persistData
        });

        ajax.send();
    });

    document.getElementById("restorestate").addEventListener('click', function () {
        var ajax = new ej.base.Ajax({
            url: "/Home/Restore",
            type: "POST",
            contentType: "application/json; charset=utf-8"
        });

        ajax.send();

        ajax.onSuccess = function (result) {
            var grid = document.getElementById("Grid").ej2_instances[0];
            var state = JSON.parse(result);
            grid.setProperties(state);
        }
    });
</script>

```

### Define the Data Model and Data Source

In the **HomeController.cs** file, define the data model (OrdersDetails) and the data source (GetAllRecords) for the EJ2 Grid.

```
public class OrdersDetails
{
    // ... Properties and constructors ...

    public static List<OrdersDetails> GetAllRecords()
    {
        // ... Sample data for the grid ...
    }
}

```

### Implement DataManagerRequest Handling for Data Source

In the **HomeController.cs** file, implement the **UrlDatasource** action to handle the DataManagerRequest from the EJ2 Grid and perform the necessary data operations (searching, sorting, filtering, paging) on the data source (OrdersDetails).

```
public IActionResult UrlDatasource([FromBody] DataManagerRequest dm)
{
    IEnumerable DataSource =  new List<OrdersDetails>();
    int count = OrdersDetails.GetAllRecords().Cast<object>().Count();
    
    // ... Perform data operations on DataSource based on dm ...

    return dm.RequiresCounts ? Json(new { result = DataSource, count = count }) : Json(DataSource);
}

```

### Implement the Server-side Actions in the Controller

  In the **HomeController.cs** file, implement the server-side actions to handle storing and restoring the grid's persisted data. The **StorePersistData** action receives the data from the client and stores it in a static variable. The **Restore** action simply returns the stored data.

  ```
  public class HomeController : Controller
{
    // ...

    public static string? persistedData;

    public IActionResult StorePersistData([FromBody] StoreData persistData)
    {
        persistedData = persistData.persistData;
        return new JsonResult(persistedData);
    }

    public IActionResult Restore()
    {
        return Content(persistedData);
    }

    // ...
}
  ```

## Conclusion

In this blog post, we learned how to store and restore the layout of an EJ2 Grid on the server-side using ASP.NET Core and Syncfusion EJ2 Grid. The ability to persist the Grid layout enhances the user experience and allows users to maintain their preferences across sessions. You can extend this concept to handle more complex scenarios or even save the layout in a database for individual user accounts.

By following the steps and code provided in this blog, you should now have a functional implementation of storing and restoring the EJ2 Grid layout in an ASP.NET Core application. 

