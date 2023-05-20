using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Complex;

[assembly: AssemblyTitle("Top-Wallets")]
[assembly: AssemblyDescription("")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("Alex Gor")]
[assembly: AssemblyProduct("Top-Wallets")]
[assembly: AssemblyCopyright("Copyright © Alex Gor 2023")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]
[assembly: ComVisible(false)]
[assembly: AssemblyVersion("1.2")]
[assembly: AssemblyFileVersion("1.2")]
[assembly: Image("top_wallets_96.png")]
[assembly: MainAssembly]

[Obfuscation(Exclude = true, ApplyToMembers = true)]
internal static class ModuleInitializer
{
    public static void Init()
    {
        Complex.Resources.AddAssembly(typeof(ModuleInitializer));
    }
}

