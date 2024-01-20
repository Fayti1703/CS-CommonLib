using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace Fayti1703.CommonLib.Enumeration;

[PublicAPI]
public readonly struct Indexed<T> {
	public readonly int index;
	public readonly T value;

	public Indexed(int index, T value) {
		this.index = index;
		this.value = value;
	}

	public void Deconstruct(out int index, out T value) {
		index = this.index;
		value = this.value;
	}
}

public static partial class EnumerationExtensions {
	/** <summary>Adds index information to each element in the sequence.</summary> */
	[LinqTunnel]
	public static IEnumerable<Indexed<T>> WithIndex<T>(this IEnumerable<T> sequence) {
		return sequence.Select((value, index) => new Indexed<T>(index, value));
	}
}
