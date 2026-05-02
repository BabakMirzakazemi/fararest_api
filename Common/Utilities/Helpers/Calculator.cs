namespace Common.Utilities.Helpers;

public static class Calculator
{
    public static long CalculateRequiredPricePercent(long total, long required)
    {
        return total == 0 || required > total ? 0 : 100 * required / total;
    }

    public static long CalculateTotalPricePercent(long total, long required)
    {
        var requiredPercent = CalculateRequiredPricePercent(total, required);
        return requiredPercent > 100 ? 0 : 100 - requiredPercent;
    }

    public static double CalculateExtraFundPercent(long total, long required, long received)
    {
        var extraFund = received - required;
        var extraTotalPrice = total - required;

        if (extraFund <= 0 || extraTotalPrice <= 0)
            return 0;

        return 100 * extraFund / (double)extraTotalPrice;
    }

    public static int CalculateProjectRemainTime(DateTime endDate)
    {
        CalculateProjectTime(endDate, out int leftOutSecondsToEnd, out int leftOutMinutesToEnd, out int leftOutHoursToEnd, out int leftOutDaysToEnd);

        if (leftOutDaysToEnd > 0)
            return leftOutDaysToEnd;

        if (leftOutHoursToEnd > 0)
            return leftOutHoursToEnd;

        if (leftOutMinutesToEnd > 0)
            return leftOutMinutesToEnd;

        return leftOutSecondsToEnd;
    }

    public static string CalculateProjectRemainTimeLabel(DateTime endDate)
    {
        CalculateProjectTime(endDate, out int leftOutSecondsToEnd, out int leftOutMinutesToEnd, out int leftOutHoursToEnd, out int leftOutDaysToEnd);

        if (leftOutDaysToEnd > 0)
            return "روز";

        if (leftOutHoursToEnd > 0)
            return "ساعت";

        if (leftOutMinutesToEnd > 0)
            return "دقیقه";

        if (leftOutSecondsToEnd > 0)
            return "ثانیه";

        return "روز";
    }

    private static void CalculateProjectTime(DateTime endDate, out int leftOutSecondsToEnd, out int leftOutMinutesToEnd, out int leftOutHoursToEnd, out int leftOutDaysToEnd)
    {
        leftOutSecondsToEnd = (int)(endDate - DateTime.Now).TotalSeconds;
        leftOutSecondsToEnd = leftOutSecondsToEnd > 0 ? leftOutSecondsToEnd : 0;
        leftOutMinutesToEnd = leftOutSecondsToEnd > 0 ? leftOutSecondsToEnd / 60 : 0;
        leftOutHoursToEnd = leftOutSecondsToEnd > 0 ? leftOutSecondsToEnd / 60 / 60 : 0;
        leftOutDaysToEnd = leftOutSecondsToEnd > 0 ? leftOutSecondsToEnd / 60 / 60 / 24 : 0;
    }
}
