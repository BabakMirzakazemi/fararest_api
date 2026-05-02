namespace Services.DTOs.Accounts.Otp;

public sealed record OtpChallengeResponse(Guid UserId, string Destination, int ExpireInSeconds);
