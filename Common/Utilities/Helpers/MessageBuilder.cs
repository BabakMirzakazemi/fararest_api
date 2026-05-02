namespace Common.Utilities.Helpers;

public class MessageBuilder
{
    private const string NotNullMessageTemplate = "{Name} اجباری است";
    private const string NotEmptyMessageTemplate = "{Name} نمیتواند خالی باشد";
    private const string InvalidMessageTemplate = "{Name} معتبر نیست";
    private const string InvalidLengthMessageTemplate = "طول {Name} غیر مجاز است";
    private const string DuplicateErrorMessage = "{Name} تکراری است";
    private const string NotFoundErrorMessageTemplate = "{Name} یافت نشد";
    private const string ArgumentOutOfRangeMessageTemplate = "{Name} باید {LessOrGrater} از {Value} باشد";
    private const string ExpiredMessageTemplate = "{Name} منقضی شده است";
    private const string NotSameMessageTemplate = "{Name} و تکرار آن یکسان نیست";


    public static string CreateNotNullErrorMessage(string property)
        => NotNullMessageTemplate.Replace("{Name}", property);

    public static string CreateNotEmptyErrorMessage(string property)
        => NotEmptyMessageTemplate.Replace("{Name}", property);

    public static string CreateInvalidErrorMessage(string property)
        => InvalidMessageTemplate.Replace("{Name}", property);

    public static string CreateInvalidLengthErrorMessage(string property)
        => InvalidLengthMessageTemplate.Replace("{Name}", property);



    public static string CreateDuplicateErrorMessage(string property)
        => DuplicateErrorMessage.Replace("{Name}", property);

    public static string CreateNotFoundErrorMessage(string property)
        => NotFoundErrorMessageTemplate.Replace("{Name}", property);

    public static string CreateNotSameErrorMessage(string property)
        => NotSameMessageTemplate.Replace("{Name}", property);

    public static string CreateArgumentGraterThanAcceptedRangeErrorMessage(string property, int max)
        => ArgumentOutOfRangeMessageTemplate.Replace("{Name}", property).Replace("{LessOrGrater}", "کمتر").Replace("{Value}", max.ToString());

    public static string CreateArgumentLessThanAcceptedRangeErrorMessage(string property, int min)
        => ArgumentOutOfRangeMessageTemplate.Replace("{Name}", property).Replace("{LessOrGrater}", "بیشتر").Replace("{Value}", min.ToString());

    public static string CreateExpiredErrorMessage(string property)
           => ExpiredMessageTemplate.Replace("{Name}", property);
}
