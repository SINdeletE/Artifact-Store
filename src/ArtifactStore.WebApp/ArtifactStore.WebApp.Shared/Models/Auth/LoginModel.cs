using System.ComponentModel.DataAnnotations;

namespace ArtifactStore.WebApp.Shared.Models.Auth;

public class LoginModel
{
    [Required(ErrorMessage = "Nickname is required.")]
    [MaxLength(32, ErrorMessage = "Nickname must be less than 32 characters long.")]
    public string Nickname { get; set; } = string.Empty;

    [Required(ErrorMessage = "Password is required.")]
    public string Password { get; set; } = string.Empty;
    public bool RememberMe { get; set; }
}