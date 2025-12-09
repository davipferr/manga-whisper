using System.ComponentModel.DataAnnotations;

namespace MangaWhisper.Common.DTOs;

public class RoleCheckRequestDto
{
    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Invalid email format.")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Role name is required.")]
    public string RoleName { get; set; } = string.Empty;
}
