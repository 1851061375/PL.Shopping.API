using System.Globalization;
using System.Text.RegularExpressions;

namespace TD.WebApi.Shared.Utils;

public static class TDUtils
{
    public static string GetCourseIdsSearchCondition(string columnName, Guid[] ids)
    {
        var conditions = new List<string>();
        foreach (var id in ids)
        {
            conditions.Add($"CHARINDEX('{id}', {columnName}) > 0");
        }

        return string.Join(" OR ", conditions);
    }

    public static bool IsPhoneNumber(string input)
    {
        // Loại bỏ khoảng trắng và dấu "+" khỏi chuỗi
        string cleanedNumber = Regex.Replace(input, @"[\s+]", string.Empty);

        // Sử dụng biểu thức chính quy để kiểm tra
        string pattern = @"^(?:\+84|0)\d{9,10}$";

        return Regex.IsMatch(cleanedNumber, pattern);
    }

    public static bool IsEmailAddress(string input)
    {
        // Sử dụng biểu thức chính quy để kiểm tra
        string pattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";
        return Regex.IsMatch(input, pattern);
    }

    public static string GenerateUniqueCoupon(int couponLength = 10)
    {
        const string characters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        Random random = new Random();
        // Lưu mã khuyến mãi vào cơ sở dữ liệu hoặc danh sách để theo dõi
        return new string(Enumerable.Repeat(characters, couponLength).Select(s => s[random.Next(s.Length)]).ToArray());
    }
    public static string? ConvertBack(string[]? item)
    {
        return item?.Any() == true ? string.Join(",", item) : null;
    }

    public static string[] Convert(string? item)
    {
        if (!string.IsNullOrWhiteSpace(item))
        {
            return item
                .Split(',')
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Select(x => x.Trim()).ToArray();
        }

        return Array.Empty<string>();
    }

    public static TimeSpan? ConvertTimeSpan(string? time)
    {
        CultureInfo provider = CultureInfo.InvariantCulture;

        TimeSpan? result = null;
        string format = "yyyy-MM-dd HH:mm:ss";

        try
        {
            TimeSpan.ParseExact(time!, format, provider);
        }
        catch (Exception e)
        {

        }

        return result;
    }

    public static DateTime GetDateZeroTime(DateTime date)
    {
        return new DateTime(date.Year, date.Month, date.Day, 0, 0, 0);
    }

    public static string? ConvertGuidArrayToString(Guid[]? guidArray, string? text = ";")
    {
        string? joinedString = null;
        if (guidArray != null && guidArray.Length > 0)
        {
            string[] stringArray = guidArray.Select(guid => guid.ToString()).ToArray();
            joinedString = string.Join(text ?? ";", stringArray);
        }
        return joinedString;
    }

    public static string? ConvertGuidArrayToStringSQLQuery(Guid[]? guidArray, string? text = ";")
    {
        string? joinedString = null;
        if (guidArray != null && guidArray.Length > 0)
        {
            string[] stringArray = guidArray.Select(guid => $"'{guid.ToString()}'").ToArray();
            joinedString = string.Join(text ?? ";", stringArray);
        }

        return joinedString;
    }

    public static string? ConvertIntArrayToStringSQLQuery(int[]? guidArray, string? text = ";")
    {
        string? joinedString = null;
        if (guidArray != null && guidArray.Length > 0)
        {
            string[] stringArray = guidArray.Select(guid => $"{guid.ToString()}").ToArray();
            joinedString = string.Join(text ?? ";", stringArray);
        }

        return joinedString;
    }

    public static string? ConvertStringArrayToString(string[]? guidArray)
    {
        string? joinedString = null;
        if (guidArray != null && guidArray.Length > 0)
        {
            joinedString = string.Join(";", guidArray);
        }
        return joinedString;
    }

    public static Guid[]? ConvertStringToGuidArray(string? guidString, string? character = ";")
    {
        Guid[]? guidArray = null;
        if (!string.IsNullOrEmpty(guidString))
        {
            string[] stringArray = guidString.Split(character, StringSplitOptions.RemoveEmptyEntries);
            stringArray.Select(str => Guid.Parse(str)).ToArray();
        }

        return guidArray;
    }

    public static bool IsWeekday(DateTime date)
    {
        return date.DayOfWeek >= DayOfWeek.Monday && date.DayOfWeek <= DayOfWeek.Friday;
    }

    public static bool CheckDayOfWeek(DateTime date, List<int>? dayOfWeek)
    {
        if (dayOfWeek == null)
        {
            return false;
        }
        return dayOfWeek.Contains(ConvertDayOfWeekToInt(date.DayOfWeek));
    }

    public static DateTime GetLastDayOfMonth(DateTime dateTime)
    {
        return new DateTime(dateTime.Year, dateTime.Month, DateTime.DaysInMonth(dateTime.Year, dateTime.Month));
    }
    public static DateTime GetFirstDayOfMonth(DateTime dateTime)
    {
        return new DateTime(dateTime.Year, dateTime.Month, 1);
    }

    public static int ConvertDayOfWeekToInt(DayOfWeek dayOfWeek)
    {
        return (int)dayOfWeek;
    }

    public static List<string> ConvertDateTimeListToStringList(List<DateTime> dateTimes, string? format = "dd/MM/yyyy")
    {
        List<string> stringList = new List<string>();

        foreach (DateTime dateTime in dateTimes)
        {
            string dateString = dateTime.ToString(format);
            stringList.Add(dateString);
        }

        return stringList;
    }

}