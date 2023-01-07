using System.ComponentModel.DataAnnotations;

namespace StoreProject.Models
{
    public class Address
    {
        [Key]
        public string AddressID { get; set; }
        [Required(ErrorMessage = "Adresa trebuie sa aiba o strada")]
        public string Street { get; set; }
        [Required(ErrorMessage = "Adresa trebuie sa aiba un oras")]
        public string City { get; set; }
        [Required(ErrorMessage = "Adresa trebuie sa aiba un judet")]
        public string County { get; set; }
        [Required(ErrorMessage = "Adresa trebuie sa aiba un cod postal")]
        public string ZipCode { get; set; }
        [Required(ErrorMessage = "Adresa trebuie sa aiba un nume")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Adresa trebuie sa aiba un numar de telefon")]
        public string PhoneNumber { get; set; }
    }
}
