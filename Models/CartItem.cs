using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StoreProject.Models
{
    public class CartItem
    {
        [Key]
        public string CartItemID { get; set; }
        [Required]
        public string CartID { get; set; }
        public virtual Cart Cart { get; set; }
        public string? ProductID { get; set; }
        public virtual Product Product { get; set; }
        [Required]
        public int Quantity { get; set; }
    }
}
