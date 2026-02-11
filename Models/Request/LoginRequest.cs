using System.ComponentModel.DataAnnotations;

namespace EShop.Models.Request;

public class LoginRequest
{
    [Required]
    [DataType(DataType.EmailAddress)]
    public string Email { get; set; } = string.Empty;[Required]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;
}