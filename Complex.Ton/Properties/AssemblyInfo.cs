using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

[assembly: AssemblyTitle("Complex.Ton")]
[assembly: AssemblyDescription("")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("Alex Gor")]
[assembly: AssemblyProduct("Complex.Ton")]
[assembly: AssemblyCopyright("Copyright © Alex Gor 2023")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]
[assembly: AssemblyVersion("1.0.0.0")]
[assembly: AssemblyFileVersion("1.0.0.0")]

[Obfuscation(Exclude = true, ApplyToMembers = true)]
internal static class ModuleInitializer
{
    public static void Init()
    {
        Complex.Resources.AddAssembly(typeof(ModuleInitializer));
    }
}

