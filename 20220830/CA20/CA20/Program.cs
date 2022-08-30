// See https://aka.ms/new-console-template for more information
using CA20.Data;

using (var db = new CA20.Data.NWContext())
{
    //foreach (var p in db.Products)
    //{
    //    Console.WriteLine(p.ProductName);
    //}

    // Change Tracking
    var p1 = db.Products.Single(p => p.ProductId == 1);
    p1.ProductName += " M";
    //db.SaveChanges();

    var newProduct = new Product() { ProductName = "Demostración" };
    db.Products.Add(newProduct);
    //db.SaveChanges();

    //var deleteProduct = db.Products.Single(p => p.ProductId == 78);
    //db.Remove(deleteProduct);
    //db.Products.Remove(deleteProduct);

    var et = db.ChangeTracker.Entries<Product>();

    db.SaveChanges();
}


Console.WriteLine("READY!");