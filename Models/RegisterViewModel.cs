using System.ComponentModel.DataAnnotations;

namespace GuestBookRepos.Models
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Логин обязателен.")]
        [StringLength(15, MinimumLength = 5, ErrorMessage = "Логин должен содержать от 5 до 15 символов.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Пароль обязателен.")]
        [StringLength(20, MinimumLength = 8, ErrorMessage = "Пароль должен содержать от 8 до 20 символов.")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).+$", ErrorMessage = "Пароль должен содержать как минимум одну цифру, одну строчную букву и одну заглавную букву.")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Подтверждение пароля обязательно.")]
        [Compare("Password", ErrorMessage = "Пароли не совпадают.")]
        public string ConfirmPassword { get; set; }
    }
}
