using System.ComponentModel.DataAnnotations;

namespace ReadOtter.Shared.Src.Data.Models;

public class AppSetting
{
    [Key]
    public Guid Id { get; set; }

    public required string SettingName { get; set; }

    public required string SettingValue { get; set; }
}
