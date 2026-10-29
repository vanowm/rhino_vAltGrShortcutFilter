using System.Diagnostics;
using System.Runtime.InteropServices;
using Rhino;
using Rhino.PlugIns;

namespace vAltGrShortcutFilter;

[Guid(PluginBuildIdentity.Id)]
public sealed class vAltGrShortcutFilterPlugIn : PlugIn
{
  public override PlugInLoadTime LoadTime => PlugInLoadTime.AtStartup;

  protected override string LocalPlugInName => "vAltGr Shortcut Filter";

  protected override LoadReturnCode OnLoad(ref string errorMessage)
  {
    var assembly = GetType().Assembly;
    var version = FileVersionInfo.GetVersionInfo(assembly.Location).FileVersion
      ?? assembly.GetName().Version?.ToString()
      ?? "unknown";

    Log.Initialize();
    Log.Write($"startup  rhino={RhinoApp.Version}  version={version}  plugin={PluginBuildIdentity.Id}  identity={assembly.GetName().Name}  target=rhino{PluginBuildIdentity.TargetRhinoVersion}/{PluginBuildIdentity.TargetFramework}  dll={assembly.Location}");
    if (RhinoApp.ExeVersion != PluginBuildIdentity.TargetRhinoVersion)
    {
      errorMessage = $"This vAltGr Shortcut Filter build targets Rhino {PluginBuildIdentity.TargetRhinoVersion} " +
        $"({PluginBuildIdentity.TargetFramework}), but Rhino {RhinoApp.ExeVersion} is running. Install the matching DLL.";
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
