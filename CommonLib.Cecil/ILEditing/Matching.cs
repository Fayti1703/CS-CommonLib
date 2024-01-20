using System;
using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace Fayti1703.CommonLib.Cecil.ILEditing;

public static class Matching {
	public static bool MatchLdfld(this Instruction inst, string typeName, string fieldName) {
		if(inst.OpCode != OpCodes.Ldfld) return false;
		FieldReference fref = (FieldReference) inst.Operand;
		if(fref.DeclaringType.FullName != typeName) return false;
		return fref.Name == fieldName;
	}

	public static bool MatchCall(this Instruction inst, string typeName, string methodName) {
		if(inst.OpCode != OpCodes.Call) return false;
		MethodReference mref = (MethodReference) inst.Operand;
		if(mref.DeclaringType.FullName != typeName) return false;
		return mref.Name == methodName;
	}

	public static int FindMatch(this IList<Instruction> instructions, params Predicate<Instruction>[] matchers) {
		if(matchers.Length == 0) return 0;
		int stopAt = instructions.Count - matchers.Length;
		for(int i = 0; i < stopAt; i++) {
			/* matchers.All((t,j) => t(instructions[i + j])) */
			if(matchers.Where((t, j) => !t(instructions[i + j])).Any()) continue;
			return i;
		}

		return -1;
	}

	public static int FindMatchEnd(this IList<Instruction> instructions, params Predicate<Instruction>[] matchers) {
		int match = FindMatch(instructions, matchers);
		if(match == -1) return -1;
		return match + matchers.Length;
	}
}
