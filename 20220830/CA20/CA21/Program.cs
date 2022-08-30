using CA21.Data;

using (var db = new CA21.Data.NorthwindContext())
{
    foreach (var p in db.Products)
    {
        Console.WriteLine(p.ProductName);
    }
}

Console.WriteLine("READY!");