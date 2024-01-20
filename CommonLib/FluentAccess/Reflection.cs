using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using Fayti1703.CommonLib.Enumeration;
using JetBrains.Annotations;

namespace Fayti1703.CommonLib.FluentAccess;

[PublicAPI]
public static partial class Reflection {
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Type Ref(this TypeInfo info) => info.AsType();
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static TypeInfo Rd(this Type @ref) => @ref.GetTypeInfo();
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Type Gmake(this Type @base, params Type[] args) => @base.MakeGenericType(args);
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static MethodInfo GMake(this MethodInfo @base, params Type[] args) => @base.MakeGenericMethod(args);

	public static Type ByRef(this Type @ref) => @ref.MakeByRefType();

	#region Optional Variants (Q)

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static ConstructorInfo? CtorQ(this TypeInfo type, params Type[] args) =>
		type.DeclaredConstructors.Where(x => !x.IsStatic).FindBestOverloadForCall(args);

	public static ConstructorInfo? CctorQ(this TypeInfo type) =>
		type.DeclaredConstructors.FirstOrDefault(x => x.IsStatic);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static FieldInfo? FldQ(this TypeInfo type, string name) =>
		type.GetDeclaredField(name);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static PropertyInfo? PropQ(this TypeInfo type, string name) =>
		type.GetDeclaredProperty(name);

	public static MethodInfo? MthQ(this TypeInfo type, string name) =>
		type.GetDeclaredMethods(name).OneIfAny(
			(_, _) => CreateAmbigiousException(type.ILName(), MemberTypes.Method, name)
		);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static MethodInfo? MthQ(this TypeInfo type, string name, params Type[] args) =>
		type.GetDeclaredMethods(name).FindBestOverloadForCall(args);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static TypeInfo? TypeQ(this TypeInfo type, string name) =>
		type.GetDeclaredNestedType(name);

	#endregion

	#region Required Variants ()

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static ConstructorInfo Ctor(this TypeInfo type, params Type[] args) =>
		type.CtorQ(args) ?? throw CreateMissingMemberException(type.ILName(), MemberTypes.Constructor, ".ctor", args.Select(x => x.ILName()));

	public static ConstructorInfo Cctor(this TypeInfo type) =>
		type.CctorQ() ?? throw CreateMissingMemberException(type.ILName(), MemberTypes.Constructor, ".cctor");

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static FieldInfo Fld(this TypeInfo type, string name) =>
		type.FldQ(name) ?? throw CreateMissingMemberException(type.ILName(), MemberTypes.Field, name);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static PropertyInfo Prop(this TypeInfo type, string name) =>
		type.PropQ(name) ?? throw CreateMissingMemberException(type.ILName(), MemberTypes.Property, name);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static MethodInfo Mth(this TypeInfo type, string name) =>
		type.GetDeclaredMethods(name).One(
			() => CreateMissingMemberException(type.ILName(), MemberTypes.Method, name),
			(_, _) => CreateAmbigiousException(type.ILName(), MemberTypes.Method, name)
		);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static MethodInfo Mth(this TypeInfo type, string name, params Type[] args) =>
		type.MthQ(name, args) ?? throw CreateMissingMemberException(type.ILName(), MemberTypes.Method, name, args.Select(x => x.ILName()));

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static TypeInfo Type(this TypeInfo type, string name) =>
		type.TypeQ(name) ?? throw CreateMissingMemberException(type.ILName(), MemberTypes.NestedType, name);

	#endregion

	internal static Exception CreateAmbigiousException(string typeILName, MemberTypes memberType, string name) {
		return new AmbiguousMemberSpecException(typeILName, memberType, name);
	}

	internal static Exception CreateMissingMemberException(string typeILName, MemberTypes what, string name, IEnumerable<string>? overloadILNames = null) {
		return new MissingMemberException(typeILName, what, name, overloadILNames?.ToArray());
	}

	public static T? FindBestOverloadForCall<T>(this IEnumerable<T> candidates, Type[] args) where T : MethodBase {
		T? bestCandidate = null;
		foreach(T candidate in candidates) {
			ParameterInfo[] @params = candidate.GetParameters();
			if(@params.Length < args.Length) continue;
			if(!@params.WithIndex().All(b => {
				   (int index, ParameterInfo param) = b;
				   return index >= args.Length ? param.IsOptional : param.ParameterType == args[index];
			})) continue;
			if(bestCandidate == null)
				bestCandidate = candidate;
			else {
				if(bestCandidate.GetParameters().Length > candidate.GetParameters().Length)
					bestCandidate = candidate;
			}
		}

		return bestCandidate;
	}
}
