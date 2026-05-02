namespace Services.DTOs.Common;

public class BaseValidationResultDTO
{
    public bool IsValid { get; set; }

    public string ErrorMessage { get; set; } = string.Empty;
}
