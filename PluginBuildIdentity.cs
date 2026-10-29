namespace vAltGrShortcutFilter;

internal static class PluginBuildIdentity
{
#if RHINO8
  internal const string Id = "7ecd24c5-ca60-406e-bb08-8ac162b54de9";
  internal const int TargetRhinoVersion = 8;
#else
  internal const string Id = "145cd90d-f1e7-4495-b7ca-533a782f0cf9";
  internal const int TargetRhinoVersion = 9;
#endif
#if NET7_0
  internal const string TargetFramework = "net7.0-windows";
#elif NET10_0
  internal const string TargetFramework = "net10.0-windows";
#else
#error Unsupported target framework.
#endif
}
