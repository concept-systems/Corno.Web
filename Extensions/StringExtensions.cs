using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Corno.Web.Extensions;

public static class StringExtensions
{
    ///// <summary>
    ///// Get string value between [first] a and [last] b.
    ///// </summary>
    //public static string Between(this string value, string a, string b)
    //{
    //    var posA = value.IndexOf(a, StringComparison.Ordinal);
    //    var posB = value.LastIndexOf(b, StringComparison.Ordinal);
    //    if (posA == -1)
    //    {
    //        return "";
    //    }
    //    if (posB == -1)
    //    {
    //        return "";
    //    }
    //    var adjustedPosA = posA + a.Length;
    //    if (adjustedPosA >= posB)
    //    {
    //        return "";
    //    }
    //    return value.Substring(adjustedPosA, posB - adjustedPosA);
    //}

    /// <summary>
    ///     Get string value between [first] a and [last] b.
    /// </summary>
    public static string Between(this string value, string a, string b)
    {
        var posA = value.IndexOf(a, StringComparison.Ordinal);
        if (posA == -1)
            return "";
        var posB = value.IndexOf(b, posA + 1, StringComparison.Ordinal);
        if (posB == -1)
            return "";
        var adjustedPosA = posA + a.Length;
        if (adjustedPosA >= posB)
            return "";
        return value.Substring(adjustedPosA, posB - adjustedPosA);
    }

    /// <summary>
    ///     Get string value after [first] a.
    /// </summary>
    public static string Before(this string value, string a)
    {
        var posA = value.IndexOf(a, StringComparison.Ordinal);
        if (posA == -1)
            return "";
        return value.Substring(0, posA);
    }

    public static string ReverseString(this string value)
    {
        var charArray = value.ToCharArray();
        Array.Reverse(charArray);
        return new string(charArray);
    }


    /// <summary>
    ///     Get string value after [last] a.
    /// </summary>
    public static string After(this string value, string a)
    {
        var posA = value.LastIndexOf(a, StringComparison.Ordinal);
        if (posA == -1)
            return "";
        var adjustedPosA = posA + a.Length;
        return adjustedPosA >= value.Length ? "" : value.Substring(adjustedPosA);
    }

    //public static string ToHtmlTable<T>(this List<T> listOfClassObjects)
    //{
    //    var ret = string.Empty;

    //    return listOfClassObjects == null || !listOfClassObjects.Any()
    //        ? ret
    //        : "<table width='100%'>" +
    //          listOfClassObjects.First().GetType().GetProperties().Select(p => p.Name).ToColumnHeaders() +
    //          listOfClassObjects.Aggregate(ret, (current, t) => current + t.ToHtmlTableRow()) +
    //          "</table>";
    //}

    public static string ToColumnHeaders<T>(this List<T> listOfProperties)
    {
        var ret = string.Empty;

        return listOfProperties == null || !listOfProperties.Any()
            ? ret
            : "<tr>" +
              listOfProperties.Aggregate(ret,
                  (current, propValue) =>
                      current + "<th style='font-size: 11pt; font-weight: bold;'>" +
                      (Convert.ToString(propValue).Length <= 100
                          ? Convert.ToString(propValue)
                          : Convert.ToString(propValue).Substring(0, 100)) + "</th>") +
              "</tr>";
    }

    public static string ToHtmlTableRow<T>(this T classObject)
    {
        var ret = string.Empty;

        return classObject == null
            ? ret
            : "<tr>" +
              classObject.GetType()
                  .GetProperties()
                  .Aggregate(ret,
                      (current, prop) =>
                          current + "<td style='font-size: 11pt; font-weight: normal;'>" +
                          (Convert.ToString(prop.GetValue(classObject, null)).Length <= 100
                              ? Convert.ToString(prop.GetValue(classObject, null))
                              : Convert.ToString(prop.GetValue(classObject, null)).Substring(0, 100) +
                                "...") + "</td>") + "</tr>";
    }

    public static string ToCamelCase(this string str)
    {
        if (string.IsNullOrEmpty(str)) 
            return string.Empty;
        var value = Regex.Replace(
            Regex.Replace(
                str,
                @"(\P{Ll})(\P{Ll}\p{Ll})",
                "$1 $2"
            ),
            @"(\p{Ll})(\P{Ll})",
            "$1 $2"
        );

        return Regex.Replace(value, @"\s+", " ");
    }

    public static string SplitCamelCaseWithNewLine(this string str)
    {
        if (string.IsNullOrEmpty(str))
            return string.Empty;
        var value = Regex.Replace(
            Regex.Replace(
                str,
                @"(\P{Ll})(\P{Ll}\p{Ll})",
                "$1 $2"
            ),
            @"(\p{Ll})(\P{Ll})",
            "$1 $2"
        );

        value = Regex.Replace(value, @"\s+", " ");
        return value.ReplaceSpaceWithNewLine();
    }

    public static string ReplaceSpaceWithNewLine(this string str)
    {
        return Regex.Replace(str, @"\s+", Environment.NewLine);
    }

    public static char GetCharFromPosition(this int position, bool isCaps)
    {
        var character = (char)((isCaps ? 65 : 97) + (position - 1));
        return character;
    }

    public static Stream ToStream(this string str, Encoding enc = null)
    {
        enc = enc ?? Encoding.UTF8;
        return new MemoryStream(enc.GetBytes(str ?? ""));
    }

    public static int Guid2Int(this Guid value)
    {
        var b = value.ToByteArray();
        var bInt = BitConverter.ToInt32(b, 0);
        return bInt;
    }

    public static Guid Int2Guid(this int value)
    {
        var bytes = new byte[16];
        BitConverter.GetBytes(value).CopyTo(bytes, 0);
        return new Guid(bytes);
    }
}