using System.Collections.Generic;
using JetBrains.Annotations;

namespace Fayti1703.CommonLib.Enumeration;

public readonly struct LastIndication<T> {
	public readonly T value;
	/** <summary><c>true</c> if this is the last value of the enumerable. <c>false</c> otherwise.</summary> */
	public readonly bool last;

	public LastIndication(T value, bool last) {
		this.value = value;
		this.last = last;
	}

	public void Deconstruct(out T value, out bool last) {
		value = this.value;
		last = this.last;
	}
}

public static partial class EnumerationExtensions {
	/** <summary>Adds 'is this the last element?' information to the sequence.</summary> */
	[LinqTunnel]
	public static IEnumerable<LastIndication<T>> WithLast<T>(this IEnumerable<T> sequence) {
		using IEnumerator<T> enumerator = sequence.GetEnumerator();
		if(!enumerator.MoveNext()) yield break;

		bool last;
		do {
			T current = enumerator.Current;
			last = !enumerator.MoveNext();
			yield return new LastIndication<T>(current, last);
		} while(!last);
	}
}
