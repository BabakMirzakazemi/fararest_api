namespace Common.Configurations;

public class BankInfo
{
    public string Name { get; set; } = null!;

    public string EnglishName { get; set; } = null!;

    public string Code { get; set; } = null!;

    public string CardPrefix { get; set; } = null!;

    public string? LogoPath { get; set; }
}