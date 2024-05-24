using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;

namespace GymChanger.ViewModels
{
    public class RegisterViewModel
    {
        [Display(Name = "Email Address")]
        [Required(ErrorMessage = "Эл. почта обязательна!")]
        public string EmailAddress { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Подтверждение пароля обязательно!")]
        [Compare("Password", ErrorMessage = "Пароли не совпадают!")]
        public string ConfirmPassword { get; set; }
        [Required(ErrorMessage = "Введите имя!")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Введите фамилию!")]
        public string Surname { get; set; }
        [Required(ErrorMessage = "Введите отчество!")]
        public string MiddleName { get; set; }
        [Required(ErrorMessage = "Введите дату рождения!")]
        public string BirthDay { get; set; }
    }
}
