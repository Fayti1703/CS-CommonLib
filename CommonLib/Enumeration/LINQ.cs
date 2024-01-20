using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace Fayti1703.CommonLib.Enumeration;

[PublicAPI]
public static partial class EnumerationExtensions {
	public static bool None<T>(this IEnumerable<T> source) {
		return !source.Any();
	}

	public static bool None<T>(this IEnumerable<T> source, Func<T, bool> predicate) {
		return !source.Any(predicate);
	}

	public static T? OneIfAny<T>(this IEnumerable<T> source, Func<T, T, Exception>? lazyMultipleValuesException = null) where T : class {
		using IEnumerator<T> enumerator = source.GetEnumerator();
		if(!enumerator.MoveNext()) return null;

		T result = enumerator.Current;
		if(!enumerator.MoveNext()) return result;

		throw lazyMultipleValuesException != null ?
			lazyMultipleValuesException(result, enumerator.Current) :
			new InvalidOperationException($"Expected at most one value, found two. Offending values: ({result}, {enumerator.Current})");

	}

	public static T? OneIfAnyV<T>(this IEnumerable<T> source, Func<T, T, Exception>? lazyMultipleValuesException = null) where T : struct {
		using IEnumerator<T> enumerator = source.GetEnumerator();
		if(!enumerator.MoveNext()) return null;

		T result = enumerator.Current;
		if(!enumerator.MoveNext()) return result;

		throw lazyMultipleValuesException != null ?
			lazyMultipleValuesException(result, enumerator.Current) :
			new InvalidOperationException($"Expected at most one value, found two. Offending values: ({result}, {enumerator.Current})");

	}

	public static T One<T>(
		this IEnumerable<T> source,
		Func<Exception>? lazyNoValuesException = null,
		Func<T, T, Exception>? lazyMultipleValuesException = null
	) {
		using IEnumerator<T> enumerator = source.GetEnumerator();
		if(!enumerator.MoveNext()) {
			throw lazyNoValuesException != null ?
				lazyNoValuesException() :
				new InvalidOperationException("Expected one value, found none.");
		}

		T result = enumerator.Current;
		if(!enumerator.MoveNext()) return result;
		throw lazyMultipleValuesException != null ?
			lazyMultipleValuesException(result, enumerator.Current) :
			new InvalidOperationException($"Expected one value, found two. Offending values: ({result}, {enumerator.Current})");
	}

	[LinqTunnel]
	public static IEnumerable<T> WhereNotNull<T>(this IEnumerable<T?> enumerable) where T : class {
		return enumerable.Where(x => x != null)!;
	}

	[LinqTunnel]
	public static IEnumerable<T> WhereNotNull<T>(this IEnumerable<T?> enumerable) where T : struct {
		return enumerable.Where(x => x.HasValue).Select(x => x!.Value);
	}

	public static T? FirstOrNull<T>(this IEnumerable<T> collection) where T : struct {
		using IEnumerator<T> enumerator = collection.GetEnumerator();

		if(enumerator.MoveNext())
			return enumerator.Current;

		return null;
	}

	public static T? FirstOrNull<T>(this IEnumerable<T> collection, Predicate<T> predicate) where T : struct {
		foreach(T candidate in collection) {
			if(predicate(candidate))
				return candidate;
		}

		return null;
	}
}
