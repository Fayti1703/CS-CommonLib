using System;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;

namespace Fayti1703.CommonLib;

[PublicAPI]
public static class Misc {
	/**
	 * <summary>Simple, non-atomic exchange.</summary>
	 * <param name="value">A reference to the value to replace.</param>
	 * <param name="newValue">The new value to replace it with.</param>
	 * <returns>The old value.</returns>
	 */
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static T Exchange<T>(ref T value, T newValue) {
		T v = value;
		value = newValue;
		return v;
	}

	/**
	 * <summary>Hack to allow for 'range literals' outside of indexers.</summary>
	 * <example>
	 * <c>using static Fayti1703.CommonLib.Misc;</c>
	 * <code>
	 * Range x = TheRange[2..^4];
	 * </code>
	 * </example>
	 */
	public static readonly RangeAccessor TheRange = new();

	public class RangeAccessor {
		internal RangeAccessor() {}
		public Range this[Range x] => x;
	}
}
