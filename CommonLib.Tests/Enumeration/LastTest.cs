using System.Linq;
using Fayti1703.CommonLib.Enumeration;

namespace Fayti1703.CommonLib.Tests.Enumeration;

[TestClass]
public class LastTest {

	[TestMethod]
	public void EmptyCollectionTest() {
		foreach((int _, bool _) in Enumerable.Empty<int>().WithLast()) {
			Assert.Fail();
		}
	}

	[TestMethod]
	public void SingleItemTest() {
		foreach((int value, bool last) in new[] { 20 }.WithLast()) {
			Assert.AreEqual(value, 20);
			Assert.IsTrue(last);
		}
	}

	[TestMethod]
	public void FiveItemTest() {
		using var enumerator = new[] { 20, 30, 40, 50, 60 }.WithLast().GetEnumerator();
		Assert.IsTrue(enumerator.MoveNext());
		Assert.AreEqual(enumerator.Current.value, 20);
		Assert.IsFalse(enumerator.Current.last);

		Assert.IsTrue(enumerator.MoveNext());
		Assert.AreEqual(enumerator.Current.value, 30);
		Assert.IsFalse(enumerator.Current.last);

		Assert.IsTrue(enumerator.MoveNext());
		Assert.AreEqual(enumerator.Current.value, 40);
		Assert.IsFalse(enumerator.Current.last);

		Assert.IsTrue(enumerator.MoveNext());
		Assert.AreEqual(enumerator.Current.value, 50);
		Assert.IsFalse(enumerator.Current.last);

		Assert.IsTrue(enumerator.MoveNext());
		Assert.AreEqual(enumerator.Current.value, 60);
		Assert.IsTrue(enumerator.Current.last);
		Assert.IsFalse(enumerator.MoveNext());

	}

}
