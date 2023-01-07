using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StoreProject.Models
{
    public class Order
    {
        [Key]
        public string OrderID { get; set; }
        [Required]
        public string UserID { get; set; }
        public virtual ApplicationUser User { get; set; }
        [Required]
        public string AddressID { get; set; }
        public virtual Address Address { get; set; }
        [Required]
        public DateTime OrderDate { get; set; }
        public ICollection<OrderItem>? OrderItems { get; set; }

        [NotMapped]
        public decimal TotalPrice
        {
            get
            {
                if (OrderItems != null)
                {
                    return OrderItems.Sum(oi => oi.ProductPrice * oi.Quantity);
                }
                else
                {
                    return 0;
                }
            }
        }

    }
}
