using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StoreProject.Models
{
    public class Cart
    {
        [Key]
        public string CartID { get; set; }
        [Required]
        public string UserID { get; set; }
        public virtual ApplicationUser User { get; set; }
        public ICollection<CartItem>? CartItems { get; set; }

        [NotMapped]
        public decimal TotalPrice
        {
            get
            {
                if (CartItems != null)
                {
                    return CartItems.Sum(ci => ci.Product.Price * ci.Quantity);
                }
                else
                {
                    return 0;
                }
            }
        }
    }
}
