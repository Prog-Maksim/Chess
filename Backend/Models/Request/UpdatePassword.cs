using System.ComponentModel.DataAnnotations;

namespace Backend.Models.Request;

public class UpdatePassword
{
    /// <summary>
    /// Старый Пароль пользователя
    /// </summary>
    [Required(AllowEmptyStrings = false, ErrorMessage = "Поле OldPassword обязательно к заполнению")]
    [StringLength(100, MinimumLength = 10, ErrorMessage = "Пароль должен быть не менее 10 символов.")]
    public required string OldPassword { get; set; }
    
    /// <summary>
    /// Новый Пароль пользователя
    /// </summary>
    [Required(AllowEmptyStrings = false, ErrorMessage = "Поле NewPassword обязательно к заполнению")]
    [StringLength(100, MinimumLength = 10, ErrorMessage = "Пароль должен быть не менее 10 символов.")]
    public required string NewPassword { get; set; }
}