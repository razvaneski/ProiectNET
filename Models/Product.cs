using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StoreProject.Models
{
    public class Product
    {
        [Key]
        public string ProductID { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public decimal Price { get; set; }

        [Required]
        public string CategoryID { get; set; }

        public virtual Category Category { get; set; }

        public string UserID { get; set; }

        public virtual ApplicationUser User { get; set; }

    }
}
