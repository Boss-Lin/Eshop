using System.ComponentModel.DataAnnotations;

namespace EShop.DTO;

public class LoginRequestDto
{
    [Required]
    [DataType(DataType.EmailAddress)]
    public string Email { get; set; } = string.Empty;[Required]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;
}