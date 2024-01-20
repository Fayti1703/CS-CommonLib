using System;
using System.Collections.Generic;
using System.Text;
using Fayti1703.CommonLib.Enumeration;

namespace Fayti1703.CommonLib.FluentAccess;

public static partial class Reflection {

	private static readonly Dictionary<Type, string> specialNames = new() {
		{ typeof(sbyte), "int8" },
		{ typeof(short), "int16" },
		{ typeof(int), "int32" },
		{ typeof(long), "int64" },
		{ typeof(byte), "unsigned int8" },
		{ typeof(ushort), "unsigned int16" },
		{ typeof(uint), "unsigned int32" },
		{ typeof(ulong), "unsigned int64" },
		{ typeof(object), "object" },
		{ typeof(string), "string" }
	};

	public static string ILName(this Type typeRef) {
		return ILNameInto(new StringBuilder(), typeRef).ToString();
	}

	private static StringBuilder ILNameInto(StringBuilder builder, Type typeRef) {
		if(typeRef.IsGenericParameter)
			return builder.Append($"!{typeRef.GenericParameterPosition}/*{typeRef.Name}*/");

		if(typeRef.IsPointer)
			return ILNameInto(builder, typeRef.GetElementType()!).Append('*');

		if(typeRef.IsByRef)
			return ILNameInto(builder, typeRef.GetElementType()!).Append('&');

		if(specialNames.TryGetValue(typeRef, out string? specialName)) {
			return builder.Append(specialName);
		}

		if(typeRef.FullName == null)
			throw new NotImplementedException($"Operation not defined for type {typeRef}");
		string? assemblyName = typeRef.Assembly.GetName().Name;
		if(assemblyName != null)
			builder.Append($"[{assemblyName}]");
		if(typeRef.Namespace != null)
			builder.Append(typeRef.Namespace).Append('.');

		builder.Append(typeRef.Name);
		if(typeRef is not { IsGenericType: true, IsGenericTypeDefinition: false })
			return builder;

		builder.Append('<');
		foreach((Type typeArg, bool last) in typeRef.GenericTypeArguments.WithLast()) {
			ILNameInto(builder, typeArg);
			if(!last)
				builder.Append(',');
		}
		builder.Append('>');

		return builder;
	}
}
