using System.Text.RegularExpressions;

namespace TD.WebApi.Infrastructure.Common.Extensions;

public static class RegexExtensions
{
    private static readonly Regex Whitespace = new(@"\s+");
    private static readonly Regex SpecialCharacter = new(@"[^a-zA-Z0-9_.]+");


    public static string ReplaceWhitespace(this string input, string replacement)
    {
        return Whitespace.Replace(input, replacement);
    }

    public static string RemoveSpecialCharacters(this string input, string replacement)
    {
        //return Regex.Replace(str, "[^a-zA-Z0-9_.]+", string.Empty, RegexOptions.Compiled);
        return SpecialCharacter.Replace(input, replacement);
    }
}