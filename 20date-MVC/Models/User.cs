using System.ComponentModel.DataAnnotations;

namespace _20date_MVC.Models
{
    public class User
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Name is required")]
        [StringLength(100, ErrorMessage = "Name can't be longer than 100 characters")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Address is required")]
        [StringLength(200, ErrorMessage = "Address can't be longer than 200 characters")]
        public string Address { get; set; } = string.Empty;

        [Required(ErrorMessage = "Gender is required")]
        [StringLength(10, ErrorMessage = "Gender can't be longer than 10 characters")]
        public string Gender { get; set; } = string.Empty;

        public string? ImagePath { get; set; }
    }
}
