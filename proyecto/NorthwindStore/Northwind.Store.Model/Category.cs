using System.ComponentModel.DataAnnotations;

namespace Northwind.Store.Model
{
    /// <summary>
    /// [Table("Categories")]
    /// </summary>
    public partial class Category
    {
        public Category()
        {
            Products = new HashSet<Product>();
        }
        public int CategoryId { get; set; }

        public string CategoryName { get; set; } = null!;

        public string? Description { get; set; }

        public byte[]? Picture { get; set; }

        public virtual ICollection<Product> Products { get; set; }
    }
}
