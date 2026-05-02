

namespace Services.DTOs.Mails;


public class PutObjectRequest
{
    public string Bucket { get; set; } = string.Empty;
    public byte[] Data { get; set; } = [];
}

public class UploadRequest
{
    public UploadTypeList Type { get; set; }
    public byte[] Data { get; set; } = [];
}

public enum UploadTypeList
{
    Avatar = 0,
    Logo = 1,
    Payment = 2,
    Workflow = 3,
}

