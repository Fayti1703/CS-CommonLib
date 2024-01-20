using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Fayti1703.CommonLib.Cecil.FluentAccess;
using JetBrains.Annotations;
using Mono.Cecil;
using Mono.Cecil.Cil;
using static CommonLib.Cecil.Tests.TestModule;

namespace CommonLib.Cecil.Tests.FluentAccess;

[TestClass]
[SuppressMessage("ReSharper", "RedundantNameQualifier")]
public class MethodILSignatureTest {
	[TestMethod]
	public void GenericMethodTest() {
		MethodReference methodRef = (MethodReference) typeof(MethodILSignatureTest)
			.GetMethod("DictILProvider", BindingFlags.NonPublic | BindingFlags.Static)!.RefIn(testModule).Rd().Body.Instructions
			.First(x => x.OpCode == OpCodes.Callvirt).Operand
		;

		Assert.AreEqual(
			"instance bool class [System.Collections]System.Collections.Generic.Dictionary`2<string, string>::TryGetValue(!0, !1&)",
			methodRef.ILSignature()
		);

	}

	[UsedImplicitly]
	private static void DictILProvider(System.Collections.Generic.Dictionary<string, string> dict) {
		dict.TryGetValue("foo", out string? bar);
	}
}
