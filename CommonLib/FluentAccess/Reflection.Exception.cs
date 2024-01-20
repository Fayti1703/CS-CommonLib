using System;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Fayti1703.CommonLib.FluentAccess;

abstract public class MemberSpecException : Exception {
	public string TypeILName { get; }
	public MemberTypes MemberType { get; }
	public string MemberName { get; }

	protected MemberSpecException(string typeILName, MemberTypes memberType, string memberName, string formattedMessage) : base(formattedMessage) {
		this.TypeILName = typeILName;
		this.MemberType = memberType;
		this.MemberName = memberName;
	}

	internal static string FormatMemberTypeName(MemberTypes memberType) {
		return memberType switch {
			MemberTypes.Constructor => "method",
			MemberTypes.Method => "method",
			MemberTypes.Field => "field",
			MemberTypes.Property => "property",
			MemberTypes.NestedType => "type",
			MemberTypes.Event => "event",
			MemberTypes.TypeInfo => "type info",
			MemberTypes.Custom => "custom member",
			MemberTypes.All => "member",
			_ => throw new ArgumentOutOfRangeException(nameof(memberType))
		};
	}
}

public class AmbiguousMemberSpecException : MemberSpecException {
	public AmbiguousMemberSpecException(string typeILName, MemberTypes memberType, string memberName) :
		base(typeILName, memberType, memberName, FormatMessage(typeILName, memberType, memberName)) { }

	private static string FormatMessage(string typeILName, MemberTypes memberType, string memberName) {
		return $"Ambiguous match: There is more than one {FormatMemberTypeName(memberType)} {typeILName}::{memberName}";
	}
}

public class MissingMemberException : MemberSpecException {
	public string[]? OverloadILNames { get; }

	public MissingMemberException(string typeILName, MemberTypes memberType, string memberName, string[]? overloadILNames = null)
		: base(typeILName, memberType, memberName, FormatMessage(typeILName, memberType, memberName, overloadILNames)) {
		this.OverloadILNames = overloadILNames;
	}


	private static string FormatMessage(string typeILName, MemberTypes memberType, string memberName, string[]? overloadILNames = null) {
		string memberTypeName = FormatMemberTypeName(memberType);
		string formattedName;
		switch(memberType) {
			case MemberTypes.Constructor:
			case MemberTypes.Method:
				if(overloadILNames != null)
					formattedName = $"{memberName}(${string.Join(", ", overloadILNames)})";
				else
					goto case MemberTypes.Field;
				break;
			case MemberTypes.Field:
			case MemberTypes.Property:
			case MemberTypes.NestedType:
				formattedName = memberName;
				break;
			default:
				throw new SwitchExpressionException();
		}

		return $"Could not find {memberTypeName} {typeILName}::{formattedName}";
	}
}
