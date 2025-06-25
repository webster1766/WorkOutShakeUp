using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WosiDomain
{
  public static class Shared
  {
    public const string DB_NAME = "wosu";

    /// <summary>
    /// Extension Method to convert no-value strings to replacement string.
    /// </summary>
    /// <param name="str">string to evaluate.</param>
    /// <param name="replaceStr">replacement string to convert.</param>
    /// <returns>string.</returns>
    public static string ReplaceNoValue(this string str, string replaceStr)
    {
      // if the original string is empty, then return replacement or empty string
      return (string.IsNullOrWhiteSpace(str)) ? replaceStr.ReplaceNullString() : str;
    }

    /// <summary>
    /// Extension Method to convert null strings to empty strings.
    /// </summary>
    /// <param name="str">string to convert.</param>
    /// <returns>string.</returns>
    public static string ReplaceNullString(this string str)
    {
      // if the original string is null, then return empty string
      return (str == null) ? string.Empty : str.Trim();
    }

    /// <summary>
    /// joins items with specified delimiter.
    /// </summary>
    /// <typeparam name="T">IEnumerable type.</typeparam>
    /// <param name="items">items to concatenate.</param>
    /// <param name="separator">delimiter to use.</param>
    /// <returns>concatenated string.</returns>
    public static string JoinToString<T>(this IEnumerable<T> items, string separator)
    {
      return string.Join<T>(separator, items);
    }
  }
}
