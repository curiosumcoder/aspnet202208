// See https://aka.ms/new-console-template for more information

using CA39;

using (var db = new NWContext())
{
	foreach (var p in db.Products)
	{
		Console.WriteLine(p.ProductName);
	}    
}


Console.WriteLine("READY");
