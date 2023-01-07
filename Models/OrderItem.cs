using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StoreProject.Models
{
    public class OrderItem
    {
        [Key]
        public string OrderItemID { get; set; }
        [Required]
        public string OrderID { get; set; }
        public virtual Order Order { get; set; }
        [Required]
        public string ProductName { get; set; }
        [Required]
        public decimal ProductPrice { get; set; }
        [Required]
        public int Quantity { get; set; }
    }
}
