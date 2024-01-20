using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using JetBrains.Annotations;

namespace Fayti1703.CommonLib;

[PublicAPI]
public static class StringExtensions {
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool StartsWithAnyOf(this string str, IEnumerable<string> suffixes) => suffixes.Any(str.StartsWith);
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool EndsWithAnyOf(this string str, IEnumerable<string> suffixes) => suffixes.Any(str.EndsWith);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool StartsWithAnyOf(this string str, params string[] suffixes) => suffixes.Any(str.StartsWith);
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool EndsWithAnyOf(this string str, params string[] suffixes) => suffixes.Any(str.EndsWith);

	/** <summary>Append a copy of a specified substring to <paramref name="builder"/>, specifying the substring with a <c>Range</c>.</summary>
	 * <remarks>Forwards to standard <see cref="StringBuilder.Append(string, int, int)"/> after resolving the range.</remarks>
	 * <param name="builder">The <see cref="StringBuilder"/> to append to.</param>
	 * <param name="str">The string that contains the substring to append.</param>
	 * <param name="subSection">The range that names the substring to append.</param>
	 * <returns>A reference to the passed string builder.</returns>
	 * <exception cref="ArgumentOutOfRangeException">
	 * <paramref name="subSection"/> does not name a valid substring of <paramref name="str"/>
	 * -or-
	 * Enlarging the value of the string builder would exceed <see cref="StringBuilder.MaxCapacity" />.
	 * </exception>
	 */
	public static StringBuilder Append(this StringBuilder builder, string str, Range subSection) {
		(int offset, int count) = subSection.GetOffsetAndLength(str.Length);
		return builder.Append(str, offset, count);
	}
}
