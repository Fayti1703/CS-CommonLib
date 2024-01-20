using System.Collections.Generic;
using JetBrains.Annotations;

namespace Fayti1703.CommonLib.Comparer;

[PublicAPI]
public readonly struct DelegateComparer<T> : IComparer<T> {

	public delegate int Comparer(T? x, T? y);

	public readonly Comparer comparer;

	public DelegateComparer(Comparer comparer) {
		this.comparer = comparer;
	}

	public int Compare(T? x, T? y) => this.comparer(x, y);
}
