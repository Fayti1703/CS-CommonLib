namespace Fayti1703.CommonLib.Tests;

[TestClass]
public class MiscTest {
	[TestMethod]
	public void ExchangeTest() {
		int x = 20;
		Assert.AreEqual(20, Misc.Exchange(ref x, 50));
		Assert.AreEqual(50, x);
	}
}
