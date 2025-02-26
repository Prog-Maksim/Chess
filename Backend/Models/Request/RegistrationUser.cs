using System.ComponentModel.DataAnnotations;

namespace Backend.Models.Request;

public class RegistrationUser
{
    /// <summary>
    /// Никнейм пользователя
    /// </summary>
    [Required(AllowEmptyStrings = false, ErrorMessage = "Поле Nickname обязательно к заполнению")]
    [StringLength(70, MinimumLength = 15, ErrorMessage = "Nickname должен быть от 15 до 70 символов")]
    public required string Nickname { get; set; }
    
    /// <summary>
    /// Почта пользователя
    /// </summary>
    [EmailAddress(ErrorMessage = "Некорректный адрес электронной почты")]
    [Required(AllowEmptyStrings = false, ErrorMessage = "Поле Email обязательно к заполнению")]
    [StringLength(100, MinimumLength = 5, ErrorMessage = "Email должен быть от 5 до 100 символов")]
    public required string Email { get; set; }
    
    /// <summary>
    /// Пароль пользователя
    /// </summary>
    [Required(AllowEmptyStrings = false, ErrorMessage = "Поле Password обязательно к заполнению")]
    [StringLength(100, MinimumLength = 10, ErrorMessage = "Пароль должен быть не менее 10 символов.")]
    public required string Password { get; set; }
}