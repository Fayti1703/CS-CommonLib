using System.Diagnostics.CodeAnalysis;
using Fayti1703.CommonLib.Cecil.FluentAccess;
using Mono.Cecil;
using static CommonLib.Cecil.Tests.TestModule;

namespace CommonLib.Cecil.Tests.FluentAccess;

[TestClass]
[SuppressMessage("ReSharper", "RedundantNameQualifier")]
public class TypeDefILNameTest {
	[TestMethod]
	public void BuiltinsTest() {
		Assert.AreEqual("bool", ImportType(typeof(bool)).Rd().ILName());
		Assert.AreEqual("unsigned int8", ImportType(typeof(byte)).Rd().ILName());
		Assert.AreEqual("int16", ImportType(typeof(short)).Rd().ILName());
		Assert.AreEqual("unsigned int16", ImportType(typeof(ushort)).Rd().ILName());
		Assert.AreEqual("int32", ImportType(typeof(int)).Rd().ILName());
		Assert.AreEqual("unsigned int32", ImportType(typeof(uint)).Rd().ILName());
		Assert.AreEqual("int64", ImportType(typeof(long)).Rd().ILName());
		Assert.AreEqual("unsigned int64", ImportType(typeof(ulong)).Rd().ILName());
		Assert.AreEqual("object", ImportType(typeof(object)).Rd().ILName());
	}

	[TestMethod]
	public void RegularTypeTest() {
		Assert.AreEqual(
			"[System.Private.CoreLib]System.IO.File",
			ImportType(typeof(System.IO.File)).Rd().ILName()
		);
	}

	[TestMethod]
	public void GenericTypeTest() {
		Assert.AreEqual(
			"[System.Private.CoreLib]System.Collections.Generic.List`1",
			ImportType(typeof(System.Collections.Generic.List<>)).Rd().ILName()
		);
		Assert.AreEqual(
			"[System.Private.CoreLib]System.Collections.Generic.Dictionary`2",
			ImportType(typeof(System.Collections.Generic.Dictionary<,>)).Rd().ILName()
		);
	}
}
