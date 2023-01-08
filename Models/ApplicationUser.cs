using Microsoft.AspNetCore.Identity;

namespace StoreProject.Models
{
    public class ApplicationUser : IdentityUser
    {
        public virtual ICollection<Review>? Reviews { get; set; }
        public virtual ICollection<Product>? Products { get; set; }
        public virtual Cart? Cart { get; set; }
        public virtual ICollection<Order>? Orders { get; set; }

    }
}
