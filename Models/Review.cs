using System.ComponentModel.DataAnnotations;

namespace StoreProject.Models
{
    public class Review
    {
        [Key]
        public string ReviewID { get; set; }

        [Range(1,5)]
        [Required(ErrorMessage = "Recenzia trebuie sa aiba un rating")]
        public int Rating { get; set; }

        [Required(ErrorMessage = "Recenzia trebuie sa aiba un titlu")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Recenzia trebuie sa aiba un continut")]
        public string Content { get; set; }

        public string? UserID { get; set; }

        public string? ProductID { get; set; }

        public virtual ApplicationUser? User { get; set; }

        public virtual Product? Product { get; set; }
    }
}
