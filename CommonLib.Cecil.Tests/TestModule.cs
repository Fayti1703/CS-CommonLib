using Fayti1703.CommonLib.Cecil.FluentAccess;
using Mono.Cecil;

namespace CommonLib.Cecil.Tests;

public static class TestModule {
	internal static readonly ModuleDefinition testModule = AssemblyDefinition.CreateAssembly(
		new AssemblyNameDefinition("TestAssembly", new Version(1, 0, 0)),
		"TestAssembly",
		ModuleKind.Dll
	).MainModule;

	internal static TypeReference ImportType(Type type) => type.RefIn(testModule);

}
