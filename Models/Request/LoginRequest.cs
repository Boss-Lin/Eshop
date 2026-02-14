using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace EShop.Models.Request;

public class LoginRequest
{
    [Required]
    [DataType(DataType.EmailAddress)]
    [DefaultValue("customer1@example.com")]
    public string Email { get; set; } = string.Empty;[Required]
    [DataType(DataType.Password)]
    [DefaultValue("Customer123!")]
    public string Password { get; set; } = string.Empty;
}