using System.ComponentModel.DataAnnotations;

namespace EShop.DTO;

public class RegisterRequestDto
{
    [Required]
    [DataType(DataType.EmailAddress)]
    public string Email { get; set; } = string.Empty;
    [Required]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
}