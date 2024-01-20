using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using Fayti1703.CommonLib.Enumeration;
using Mono.Cecil;
using static Fayti1703.CommonLib.Misc;

namespace Fayti1703.CommonLib.Cecil.FluentAccess;

public static partial class CecilExtensions {
	private static readonly Dictionary<string, string> specialNames = new() {
		{ typeof(bool).FullName!, "bool" },
		{ typeof(sbyte).FullName!, "int8" },
		{ typeof(short).FullName!, "int16" },
		{ typeof(int).FullName!, "int32" },
		{ typeof(long).FullName!, "int64" },
		{ typeof(byte).FullName!, "unsigned int8" },
		{ typeof(ushort).FullName!, "unsigned int16" },
		{ typeof(uint).FullName!, "unsigned int32" },
		{ typeof(ulong).FullName!, "unsigned int64" },
		{ typeof(object).FullName!, "object" },
		{ typeof(string).FullName!, "string" }
	};

	public static string ILName(this TypeReference type) => new StringBuilder().WriteILNameOf(type).ToString();
	public static string ILName(this TypeDefinition type) => new StringBuilder().WriteILNameOf(type).ToString();
	public static string ILSignature(this TypeReference type) => new StringBuilder().WriteILSignatureOf(type).ToString();
	public static string ILSignature(this TypeDefinition type) => new StringBuilder().WriteILSignatureOf(type).ToString();
	public static string ILName(this FieldDefinition field) => new StringBuilder().WriteILNameOf(field).ToString();
	public static string ILName(this FieldReference field) => new StringBuilder().WriteILNameOf(field).ToString();
	public static string ILSignature(this FieldDefinition field) => new StringBuilder().WriteILSignatureOf(field).ToString();
	public static string ILSignature(this FieldReference field) => new StringBuilder().WriteILSignatureOf(field).ToString();
	public static string ILName(this MethodReference method) => new StringBuilder().WriteILNameOf(method).ToString();
	public static string ILName(this MethodDefinition method) => new StringBuilder().WriteILNameOf(method).ToString();
	public static string ILSignature(this MethodReference method) => new StringBuilder().WriteILSignatureOf(method).ToString();
	public static string ILSignature(this MethodDefinition method) => new StringBuilder().WriteILSignatureOf(method).ToString();
	public static string ILName(this PropertyReference prop) => new StringBuilder().WriteILNameOf(prop).ToString();
	public static string ILName(this PropertyDefinition prop) => new StringBuilder().WriteILNameOf(prop).ToString();
	public static string ILSignature(this PropertyReference prop) => new StringBuilder().WriteILSignatureOf(prop).ToString();
	public static string ILSignature(this PropertyDefinition prop) => new StringBuilder().WriteILSignatureOf(prop).ToString();

	private static bool GetSpecialName(TypeReference typeRef, [NotNullWhen(true)] out string? specialName) {
		return specialNames.TryGetValue(typeRef.FullName, out specialName);
	}

	private static bool GetSpecialName(TypeDefinition typeDef, [NotNullWhen(true)] out string? specialName) {
		return specialNames.TryGetValue(typeDef.FullName, out specialName);
	}

	private static StringBuilder WriteILNameOf(this StringBuilder builder, TypeDefinition typeDef) {
		if(GetSpecialName(typeDef, out string? specialName))
			return builder.Append(specialName);

		string? moduleName = typeDef.Module.Name;
		int suffixLength = moduleName.EndsWithAnyOf(".exe", ".dll") ? 4 : 0;
		if(moduleName != null)
			builder.Append('[').Append(moduleName, TheRange[..^suffixLength]).Append(']');
		if(typeDef.Namespace != "")
			builder.Append(typeDef.Namespace).Append('.');

		builder.Append(typeDef.Name);
		return builder;
	}

	private static StringBuilder WriteNameSigSwitch<T>(
		this StringBuilder builder,
		T thing,
		Func<StringBuilder, T, StringBuilder> nameMethod,
		Func<StringBuilder, T, StringBuilder> sigMethod,
		bool writeSig
	) {
		return writeSig ? sigMethod(builder, thing) : nameMethod(builder, thing);
	}

	private static StringBuilder WriteNameSigSwitch<T>(
		this StringBuilder builder,
		T thing,
		Func<StringBuilder, T, bool, StringBuilder> nameMethod,
		Func<StringBuilder, T, StringBuilder> sigMethod,
		bool writeSig
	) {
		return writeSig ? sigMethod(builder, thing) : nameMethod(builder, thing, false);
	}

	private static StringBuilder WriteGenericArguments(this StringBuilder builder, IEnumerable<TypeReference> genericArguments, bool forSignature = false) {
		builder.Append('<');
		foreach((TypeReference typeArg, bool last) in genericArguments.WithLast()) {
			builder.WriteNameSigSwitch(typeArg, WriteILNameOf, WriteILSignatureOf, forSignature);
			if(!last)
				builder.Append(", ");
		}

		builder.Append('>');
		return builder;
	}

	private static StringBuilder WriteILNameOf(this StringBuilder builder, TypeReference typeRef, bool forSignature = false) {
		if(typeRef is TypeDefinition definition) return WriteILNameOf(builder, definition);

		if(typeRef.IsGenericParameter) return WriteILNameOf(builder, (GenericParameter) typeRef);


		if(typeRef.IsPointer)
			return builder.WriteILNameOf(typeRef.GetElementType()!).Append('*');

		if(typeRef.IsByReference)
			return builder.WriteILNameOf(typeRef.GetElementType()!).Append('&');

		if(GetSpecialName(typeRef, out string? specialName))
			return builder.Append(specialName);

		builder.Append($"[{typeRef.Scope.Name}]");
		if(typeRef.Namespace != "")
			builder.Append(typeRef.Namespace).Append('.');

		builder.Append(typeRef.Name);
		if(typeRef is not { IsGenericInstance: true }) return builder;

		GenericInstanceType genericInstance = (GenericInstanceType) typeRef;
		builder.WriteGenericArguments(genericInstance.GenericArguments, forSignature);

		return builder;
	}

	private static StringBuilder WriteILNameOf(this StringBuilder builder, GenericParameter genParameter) {
		builder.Append($"!{genParameter.Position}");
		#if false
		if(genParameter.Name[0] != '!')
			builder.Append($"/*{genParameter.Name}*/");
		#endif
		return builder;
	}

	private static StringBuilder WriteILNameOf(this StringBuilder builder, MemberReference member, bool forSignature = false) {
		if(member is MethodReference method) return builder.WriteILNameOf(method, forSignature);
		return builder.WriteILNameOf(member.DeclaringType).Append("::").Append(member.Name);
	}

	private static StringBuilder WriteILNameOf(this StringBuilder builder, MethodReference method, bool forSignature = false) {
		builder
			.WriteNameSigSwitch(method.DeclaringType, WriteILNameOf, WriteILSignatureOf, forSignature)
			.Append("::")
			.Append(method.Name)
		;
		if(method is GenericInstanceMethod genericMethod)
			builder.WriteGenericArguments(genericMethod.GenericArguments,  forSignature);
		return builder;
	}

	private static StringBuilder WriteILSignatureOf(this StringBuilder builder, TypeReference type) {

		TypeReference? elementType = type.GetElementType();
		if(!(GetSpecialName(elementType, out _) || elementType.IsGenericParameter)) {
			/* builtins and generic parameters do not get `valuetype`/`class` prepended in signatures */
			builder.Append(elementType.IsValueType ? "valuetype " : "class ");
		}

		return builder.WriteILNameOf(type, forSignature: true);
	}

	private static StringBuilder WriteILSignatureOf(this StringBuilder builder, FieldReference field) =>
		builder.WriteILSignatureOf(field.FieldType).Append(' ').WriteILNameOf(field, forSignature: true);

	private static StringBuilder WriteILSignatureOf(this StringBuilder builder, MethodReference method) {
		if(method.HasThis)
			builder.Append("instance ");
		builder.WriteILSignatureOf(method.ReturnType).Append(' ').WriteILNameOf(method, true).Append('(');
		foreach((ParameterDefinition parameter, bool last) in method.Parameters.WithLast()) {
			builder.WriteILSignatureOf(parameter.ParameterType);
			if(!last)
				builder.Append(", ");
		}

		return builder.Append(')');
	}

	private static StringBuilder WriteILSignatureOf(this StringBuilder builder, PropertyReference property) =>
		builder.WriteILSignatureOf(property.PropertyType).Append(' ').WriteILNameOf(property, forSignature: true);
}
