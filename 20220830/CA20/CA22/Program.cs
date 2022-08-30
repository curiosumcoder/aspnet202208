using Northwind.Store.Data;

using (var db = new NWContext())
{
    foreach (var p in db.Products)
    {
        Console.WriteLine(p.ProductName);
    }
}

Console.WriteLine("READY!");