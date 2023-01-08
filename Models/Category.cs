using System.ComponentModel.DataAnnotations;

namespace StoreProject.Models
{
    public class Category
    {
        [Key]
        public string CategoryID { get; set; }
        [Required(ErrorMessage = "Categoria trebuie sa aiba un nume")]
        public string Name { get; set; }
        public virtual ICollection<Product> Products { get; set; }
    }
}
