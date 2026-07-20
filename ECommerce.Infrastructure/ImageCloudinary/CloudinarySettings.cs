using System.ComponentModel.DataAnnotations;

namespace ECommerce.Infrastructure.ImageCloudinary;

public class CloudinarySettings
{
    [Required]
    public string CloudName { get; set; } = default!;
    [Required]
    public string ApiKey { get; set; } = default!;
    [Required]
    public string ApiSecret { get; set; } = default!;
}
