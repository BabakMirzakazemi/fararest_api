using System.Globalization;

namespace Common.Utilities.Extensions;

public static class DateTimeExtensions
{
    public static string GregorianToShamsiDatePickerValue(this DateTime date, bool showTime = false)
    {
        Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");

        System.Globalization.PersianCalendar pc = new System.Globalization.PersianCalendar();

        return showTime
           ? string.Format("{0}{1}{2}{1}{3}-{4}:{5}:{6}", pc.GetYear(date), "/", pc.GetMonth(date).ToString("00", CultureInfo.InvariantCulture), pc.GetDayOfMonth(date).ToString("00", CultureInfo.InvariantCulture), date.Hour.ToString("00"), date.Minute.ToString("00"), date.Second.ToString("00"))
           : string.Format("{0}{1}{2}{1}{3}", pc.GetYear(date), "/", pc.GetMonth(date).ToString("00", CultureInfo.InvariantCulture), pc.GetDayOfMonth(date).ToString("00", CultureInfo.InvariantCulture));
    }
}
