using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using Fayti1703.CommonLib.Enumeration;
using JetBrains.Annotations;
using FluentReflection = Fayti1703.CommonLib.FluentAccess.Reflection;
using Mono.Cecil;
using Mono.Collections.Generic;

namespace Fayti1703.CommonLib.Cecil.FluentAccess;

[PublicAPI]
public static partial class CecilExtensions {
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static TypeReference RefIn(this Type native, ModuleDefinition module) => module.ImportReference(native);
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static FieldReference RefIn(this FieldInfo native, ModuleDefinition module) => module.ImportReference(native);
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static MethodReference RefIn(this MethodInfo native, ModuleDefinition module) => module.ImportReference(native);


	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static TypeReference RefIn(this TypeReference type, ModuleDefinition module) => module.ImportReference(type);
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static FieldReference RefIn(this FieldReference field, ModuleDefinition module) => module.ImportReference(field);
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static MethodReference RefIn(this MethodReference field, ModuleDefinition module) => module.ImportReference(field);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static TypeDefinition Rd(this TypeReference @ref) => @ref.Resolve();
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static FieldDefinition Rd(this FieldReference @ref) => @ref.Resolve();
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static MethodDefinition Rd(this MethodReference @ref) => @ref.Resolve();
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static PropertyDefinition Rd(this PropertyReference @ref) => @ref.Resolve();

	public static GenericInstanceType Gmake(this TypeReference @base, params TypeReference[] args) {
		if(!@base.HasGenericParameters)
			throw new ArgumentException("Not a generic type.", nameof(@base));
		if(args.Length != @base.GenericParameters.Count)
			throw new ArgumentException($"Invalid number of generic arguments. Expected {@base.GenericParameters.Count} args, got {args.Length}.", nameof(args));

		GenericInstanceType instance = new(@base);
		foreach(TypeReference arg in args)
			instance.GenericArguments.Add(arg);
		return instance;
	}

	public static GenericInstanceMethod GMake(this MethodReference @base, params TypeReference[] args) {
		if(!@base.HasGenericParameters)
			throw new ArgumentException("Not a generic method.", nameof(@base));
		if(args.Length != @base.GenericParameters.Count)
			throw new ArgumentException($"Invalid number of generic arguments. Expected {@base.GenericParameters.Count} args, got {args.Length}.", nameof(args));

		GenericInstanceMethod instance = new(@base);
		foreach(TypeReference arg in args)
			instance.GenericArguments.Add(arg);
		return instance;
	}

	public static ByReferenceType ByRef(this TypeReference @ref) => new(@ref);

	#region Optional Variants (Q)

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static MethodDefinition? CtorQ(this TypeDefinition type, params TypeReference[] args) =>
		type.Methods.Where(x => x.IsStatic).FindBestOverload(args);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static MethodDefinition? CctorQ(this TypeDefinition type) =>
		type.Methods.FirstOrDefault(x => x.IsStatic && x.IsConstructor);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static FieldDefinition? FldQ(this TypeDefinition type, string name) =>
		type.Fields.FirstOrDefault(x => x.Name == name);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static PropertyDefinition? PropQ(this TypeDefinition type, string name) =>
		type.Properties.FirstOrDefault(x => x.Name == name);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static MethodDefinition? MthQ(this TypeDefinition type, string name) =>
		type.Methods.Where(x => x.Name == name).OneIfAny(
			(_, _) => FluentReflection.CreateAmbigiousException(type.ILName(), MemberTypes.Method, name)
		);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static MethodDefinition? MthQ(this TypeDefinition type, string name, params TypeReference[] args) =>
		type.Methods.Where(x => x.Name == name).FindBestOverload(args);

	public static TypeDefinition? TypeQ(this TypeDefinition type, string name)
		=> type.NestedTypes.FirstOrDefault(x => x.Name == name);

	#endregion

	#region Required Variants ()

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static MethodDefinition Ctor(this TypeDefinition type, params TypeReference[] args) =>
		type.CtorQ(args) ??
		throw FluentReflection.CreateMissingMemberException(type.ILName(), MemberTypes.Constructor, ".ctor", args.Select(x => x.ILName()));

	public static MethodDefinition Cctor(this TypeDefinition type) =>
		type.CctorQ() ?? throw FluentReflection.CreateMissingMemberException(type.ILName(), MemberTypes.Constructor, ".cctor");

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static FieldDefinition Fld(this TypeDefinition type, string name) =>
		type.FldQ(name) ?? throw FluentReflection.CreateMissingMemberException(type.ILName(), MemberTypes.Field, name);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static PropertyDefinition Prop(this TypeDefinition type, string name) =>
		type.PropQ(name) ?? throw FluentReflection.CreateMissingMemberException(type.ILName(), MemberTypes.Property, name);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static MethodDefinition Mth(this TypeDefinition type, string name) =>
		type.Methods.Where(x => x.Name == name).One(
			() => FluentReflection.CreateMissingMemberException(type.ILName(), MemberTypes.Method, name),
			(_, _) => FluentReflection.CreateAmbigiousException(type.ILName(), MemberTypes.Method, name)
		);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static MethodDefinition Mth(this TypeDefinition type, string name, params TypeReference[] args) =>
		type.MthQ(name, args) ??
		throw FluentReflection.CreateMissingMemberException(type.ILName(), MemberTypes.Method, name, args.Select(x => x.ILName()));

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static TypeDefinition Type(this TypeDefinition type, string name) =>
		type.TypeQ(name) ?? throw FluentReflection.CreateMissingMemberException(type.ILName(), MemberTypes.NestedType, name);

	#endregion

	
	public static MethodDefinition? FindBestOverload(this IEnumerable<MethodDefinition> candidates, TypeReference[] args) {
		MethodDefinition? bestCandidate = null;
		foreach(MethodDefinition candidate in candidates) {
			Collection<ParameterDefinition> @params = candidate.Parameters;
			if(@params.Count < args.Length) continue;
			if(!@params.WithIndex().All(b => {
				   (int index, ParameterDefinition param) = b;
				   return index >= args.Length ? param.IsOptional : param.ParameterType == args[index];
			   })) continue;
			if(bestCandidate == null)
				bestCandidate = candidate;
			else {
				if(bestCandidate.Parameters.Count > candidate.Parameters.Count)
					bestCandidate = candidate;
			}
		}

		return bestCandidate;
	}

	[Obsolete("Use FindBestOverload instead")]
	public static MethodDefinition? FindBestOverloadForCall(this IEnumerable<MethodDefinition> candidates, TypeReference[] args) =>
		candidates.FindBestOverload(args);
}
