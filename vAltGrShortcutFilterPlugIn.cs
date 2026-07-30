using System.Diagnostics;
using System.Runtime.InteropServices;
using Rhino;
using Rhino.PlugIns;

namespace vAltGrShortcutFilter;

[Guid("7ecd24c5-ca60-406e-bb08-8ac162b54de9")]
public sealed class vAltGrShortcutFilterPlugIn : PlugIn
{
#if NET7_0
  private const int TargetRhinoVersion = 8;
  private const string TargetFramework = "net7.0-windows";
#elif NET10_0
  private const int TargetRhinoVersion = 9;
  private const string TargetFramework = "net10.0-windows";
#else
#error Unsupported target framework.
#endif

  public override PlugInLoadTime LoadTime => PlugInLoadTime.AtStartup;

  protected override string LocalPlugInName => "vAltGr Shortcut Filter";

  protected override LoadReturnCode OnLoad(ref string errorMessage)
  {
    var assembly = GetType().Assembly;
    var version = FileVersionInfo.GetVersionInfo(assembly.Location).FileVersion
      ?? assembly.GetName().Version?.ToString()
      ?? "unknown";

    Log.Initialize();
    Log.Write($"startup  rhino={RhinoApp.Version}  version={version}  target=rhino{TargetRhinoVersion}/{TargetFramework}  dll={assembly.Location}");
    if (RhinoApp.ExeVersion != TargetRhinoVersion)
    {
      errorMessage = $"This vAltGr Shortcut Filter build targets Rhino {TargetRhinoVersion} " +
        $"({TargetFramework}), but Rhino {RhinoApp.ExeVersion} is running. Install the matching DLL.";
      Log.Write($"load rejected  {errorMessage}");
      return LoadReturnCode.ErrorShowDialog;
    }

    ShortcutFilter.Start();
    return LoadReturnCode.Success;
  }

  protected override void OnShutdown()
  {
    ShortcutFilter.Stop();
    base.OnShutdown();
  }
}
