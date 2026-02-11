using System.ComponentModel.DataAnnotations;

namespace EShop.Models.Request;

public class RegisterRequest
{
    [Required]
    [DataType(DataType.EmailAddress)]
    public string Email { get; set; } = string.Empty;
    [Required]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
}