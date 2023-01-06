using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StoreProject.Models
{
    public class Product
    {
        [Key]
        public string ProductID { get; set; }

        [Required(ErrorMessage = "Produsul trebuie sa aiba un nume")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Produsul trebuie sa aiba o descriere")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Produsul trebuie sa aiba un pret")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Produsul trebuie sa apartina unei categorii")]
        public string CategoryID { get; set; }

        [Required]
        public virtual Category Category { get; set; }

        [Required(ErrorMessage = "Produsul trebuie sa apartina unui utilizator")]
        public string UserID { get; set; }

        [Required]
        public virtual ApplicationUser User { get; set; }

        public virtual ICollection<Review>? Reviews { get; set; }

        [NotMapped]
        public double Rating
        {
            get
            {
                if(Reviews != null)
                {
                    if(Reviews.Count != 0)
                        return Reviews.Average(r => r.Rating);
                    else
                        return 0;
                }
                else
                {
                    return 0;
                }
            }
        }

        public string? Status { get; set; }

    }
}
