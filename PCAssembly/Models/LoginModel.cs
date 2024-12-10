using System.ComponentModel.DataAnnotations;

namespace PCAssembly.Models
{
    public class LoginModel
    {
        [Required(ErrorMessage = "Введите имя пользователя")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Введите пароль")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        public bool RememberMe { get; set; } // Флаг для запоминания пользователя
    }
}
