using System.Diagnostics.CodeAnalysis;
using Fayti1703.CommonLib.Cecil.FluentAccess;
using static CommonLib.Cecil.Tests.TestModule;

namespace CommonLib.Cecil.Tests.FluentAccess;

[TestClass]
[SuppressMessage("ReSharper", "RedundantNameQualifier")]
public class TypeRefILNameTest {

	[TestMethod]
	public void BuiltinsTest() {
		Assert.AreEqual("unsigned int8", ImportType(typeof(byte)).ILName());
		Assert.AreEqual("int16", ImportType(typeof(short)).ILName());
		Assert.AreEqual("unsigned int16", ImportType(typeof(ushort)).ILName());
		Assert.AreEqual("int32", ImportType(typeof(int)).ILName());
		Assert.AreEqual("unsigned int32", ImportType(typeof(uint)).ILName());
		Assert.AreEqual("int64", ImportType(typeof(long)).ILName());
		Assert.AreEqual("unsigned int64", ImportType(typeof(ulong)).ILName());
		Assert.AreEqual("object", ImportType(typeof(object)).ILName());
	}

	[TestMethod]
	public void PointerTest() {
		Assert.AreEqual("unsigned int8*", ImportType(typeof(byte*)).ILName());
		Assert.AreEqual("int16*", ImportType(typeof(short*)).ILName());
		Assert.AreEqual("unsigned int16*", ImportType(typeof(ushort*)).ILName());
		Assert.AreEqual("int32*", ImportType(typeof(int*)).ILName());
		Assert.AreEqual("unsigned int32*", ImportType(typeof(uint*)).ILName());
		Assert.AreEqual("int64*", ImportType(typeof(long*)).ILName());
		Assert.AreEqual("unsigned int64*", ImportType(typeof(ulong*)).ILName());
		Assert.AreEqual("object*", ImportType(typeof(object*)).ILName());
	}
	
	[TestMethod]
	public void RefTest() {
		Assert.AreEqual("unsigned int8&", ImportType(typeof(byte)).ByRef().ILName());
		Assert.AreEqual("int16&", ImportType(typeof(short)).ByRef().ILName());
		Assert.AreEqual("unsigned int16&", ImportType(typeof(ushort)).ByRef().ILName());
		Assert.AreEqual("int32&", ImportType(typeof(int)).ByRef().ILName());
		Assert.AreEqual("unsigned int32&", ImportType(typeof(uint)).ByRef().ILName());
		Assert.AreEqual("int64&", ImportType(typeof(long)).ByRef().ILName());
		Assert.AreEqual("unsigned int64&", ImportType(typeof(ulong)).ByRef().ILName());
		Assert.AreEqual("object&", ImportType(typeof(object)).ByRef().ILName());
	}

	[TestMethod]
	public void GenericTypeTest() {
		Assert.AreEqual(
			"[System.Private.CoreLib]System.Collections.Generic.List`1",
			ImportType(typeof(System.Collections.Generic.List<>)).ILName()
		);
		Assert.AreEqual(
			"[System.Private.CoreLib]System.Collections.Generic.Dictionary`2",
			ImportType(typeof(System.Collections.Generic.Dictionary<,>)).ILName()
		);
	}

	[TestMethod]
	public void GenericTypeInstactiationTest() {
		Assert.AreEqual(
			"[System.Private.CoreLib]System.Collections.Generic.List`1<int32>",
			ImportType(typeof(System.Collections.Generic.List<int>)).ILName()
		);
		Assert.AreEqual(
			"[System.Private.CoreLib]System.Collections.Generic.Dictionary`2<string, int32>",
			ImportType(typeof(System.Collections.Generic.Dictionary<string,int>)).ILName()
		);
	}

	[TestMethod]
	public void GenericArgumentsTest() {
		Assert.AreEqual(
			"!0",
			ImportType(typeof(System.Collections.Generic.List<>)).GenericParameters[0].ILName()
		);
	}

	[TestMethod]
	public void GlobalTypeTest() {
		Assert.AreEqual("[CommonLib.Tests]GlobalType", ImportType(typeof(GlobalType)).ILName());
	}
}
