using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace StoreProject.Models
{
    public class Product
    {
        [Key]
        public string ProductID { get; set; }

        [Required(ErrorMessage = "Produsul trebuie sa aiba un nume")]
        [MinLength(1)]
        public string Name { get; set; }

        [Required(ErrorMessage = "Produsul trebuie sa aiba o descriere")]
        [MinLength(1)]
        public string Description { get; set; }

        [Required(ErrorMessage = "Produsul trebuie sa aiba un pret")]
        [Range(1, 1000000)]
        [NotNull]
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
        public byte[]? Image { get; set; }

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

        public bool IsAvailable { get; set; }
        [Required(ErrorMessage = "Stoc invalid")] 
        [Range(0,1000000)]
        public int Stock { get; set; }
    }
}
