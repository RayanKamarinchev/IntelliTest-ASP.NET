using System.ComponentModel.DataAnnotations;

namespace IntelliTest.Core.Models.Users
{
    public class RegisterViewModel
    {
        [Required]
        [EmailAddress]
        [StringLength(60, MinimumLength = 10)]
        [Display(Name = "Имейл")]
        public string Email { get; set; }
        [Required]
        [StringLength(20, MinimumLength = 5)]
        [DataType(DataType.Password)]
        [Display(Name = "Парола")]
        public string Password { get; set; }
        [Compare(nameof(Password))]
        [DataType(DataType.Password)]
        [Display(Name = "Повтори паролата")]
        public string ConfirmPassword { get; set; }
        [Required]
        [StringLength(20, MinimumLength = 3)]
        [Display(Name = "Име")]
        public string FirstName { get; set; }
        [Required]
        [StringLength(20, MinimumLength = 3)]
        [Display(Name = "Фамилия")]
        public string LastName { get; set; }
    }
}
