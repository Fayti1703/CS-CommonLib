using Fayti1703.CommonLib.FluentAccess;

namespace Fayti1703.CommonLib.Tests.FluentAccess;

[TestClass]
public class ILNameTest {

	[TestMethod]
	public void BuiltinsTest() {
		Assert.AreEqual("unsigned int8", typeof(byte).ILName());
		Assert.AreEqual("int16", typeof(short).ILName());
		Assert.AreEqual("unsigned int16", typeof(ushort).ILName());
		Assert.AreEqual("int32", typeof(int).ILName());
		Assert.AreEqual("unsigned int32", typeof(uint).ILName());
		Assert.AreEqual("int64", typeof(long).ILName());
		Assert.AreEqual("unsigned int64", typeof(ulong).ILName());
		Assert.AreEqual("object", typeof(object).ILName());
	}

	[TestMethod]
	public void PointerTest() {
		Assert.AreEqual("unsigned int8*", typeof(byte*).ILName());
		Assert.AreEqual("int16*", typeof(short*).ILName());
		Assert.AreEqual("unsigned int16*", typeof(ushort*).ILName());
		Assert.AreEqual("int32*", typeof(int*).ILName());
		Assert.AreEqual("unsigned int32*", typeof(uint*).ILName());
		Assert.AreEqual("int64*", typeof(long*).ILName());
		Assert.AreEqual("unsigned int64*", typeof(ulong*).ILName());
		Assert.AreEqual("object*", typeof(object*).ILName());
	}

	[TestMethod]
	public void RefTest() {
		Assert.AreEqual("unsigned int8&", typeof(byte).MakeByRefType().ILName());
		Assert.AreEqual("int16&", typeof(short).MakeByRefType().ILName());
		Assert.AreEqual("unsigned int16&", typeof(ushort).MakeByRefType().ILName());
		Assert.AreEqual("int32&", typeof(int).MakeByRefType().ILName());
		Assert.AreEqual("unsigned int32&", typeof(uint).MakeByRefType().ILName());
		Assert.AreEqual("int64&", typeof(long).MakeByRefType().ILName());
		Assert.AreEqual("unsigned int64&", typeof(ulong).MakeByRefType().ILName());
		Assert.AreEqual("object&", typeof(object).MakeByRefType().ILName());
	}

	[TestMethod]
	public void GenericTypeTest() {
		Assert.AreEqual(
			"[System.Private.CoreLib]System.Collections.Generic.List`1",
			typeof(System.Collections.Generic.List<>).ILName()
		);
		Assert.AreEqual(
			"[System.Private.CoreLib]System.Collections.Generic.Dictionary`2",
			typeof(System.Collections.Generic.Dictionary<,>).ILName()
		);
	}

	[TestMethod]
	public void GenericTypeInstactiationTest() {
		Assert.AreEqual(
			"[System.Private.CoreLib]System.Collections.Generic.List`1<int32>",
			typeof(System.Collections.Generic.List<int>).ILName()
		);
		Assert.AreEqual(
			"[System.Private.CoreLib]System.Collections.Generic.Dictionary`2<string,int32>",
			typeof(System.Collections.Generic.Dictionary<string,int>).ILName()
		);
	}

	[TestMethod]
	public void GenericArgumentsTest() {
		Assert.AreEqual(
			"!0/*T*/",
			typeof(System.Collections.Generic.List<>).GetGenericArguments()[0].ILName()
		);
	}

	[TestMethod]
	public void GlobalTypeTest() {
		Assert.AreEqual("[CommonLib.Tests]GlobalType", typeof(GlobalType).ILName());
	}
}
