using System.Text.RegularExpressions;

namespace Common.Utilities.Helpers;

public static class ValidationHelper
{
    public static bool IsNaturalNationalCode(this string nationalId)
    {
        if (!Regex.IsMatch(nationalId, @"^\d{10}$"))
            return false;

        var check = Convert.ToInt32(nationalId.Substring(9, 1));
        var sum = Enumerable.Range(0, 9)
            .Select(x => Convert.ToInt32(nationalId.Substring(x, 1)) * (10 - x))
            .Sum() % 11;

        return sum < 2 ? check == sum : check + sum == 11;
    }

    public static bool IsLegalNationalCode(this string nationalId)
    {
        if (!Regex.IsMatch(nationalId, @"^\d{11}$"))
            return false;

        var oneBeforeLastChar = int.Parse(nationalId.Substring(9, 1)) + 2;

        var weightDictionary = new Dictionary<int, int>()
            {
                {1,29 },
                {2,27 },
                {3,23 },
                {4,19 },
                {5,17 },
                {6,29 },
                {7,27 },
                {8,23 },
                {9,19 },
                {10,17 },
            };

        var check = Convert.ToInt32(nationalId.Substring(10, 1));
        var sum = 0;

        for (int i = 0; i < nationalId.Length - 1; i++)
        {
            var item = int.Parse(nationalId[i].ToString());
            var weight = weightDictionary[i + 1];

            var calculatedWeight = (item + oneBeforeLastChar) * weight;

            sum += calculatedWeight;
        }

        if (sum % 11 == 10)
            sum = 0;
        else sum %= 11;

        return check == sum;
    }
}
