using System.Reflection;
using System.Text;

namespace vAltGrShortcutFilter;

internal static class Log
{
  private static readonly object Sync = new();
  private static readonly Encoding Utf8 = new UTF8Encoding(false);
  private static string? _path;

  internal static void Initialize()
  {
    try
    {
      var dll = Assembly.GetExecutingAssembly().Location;
      _path = Path.Combine(Path.GetDirectoryName(dll) ?? AppContext.BaseDirectory,
        "vAltGrShortcutFilter.log");
      File.WriteAllText(_path,
        $"[{DateTime.Now:HH:mm:ss.fff}] log initialized{Environment.NewLine}", Utf8);
    }
    catch { }
  }

  internal static void Write(string message)
  {
    try
    {
      lock (Sync)
      {
        if (!string.IsNullOrEmpty(_path))
          File.AppendAllText(_path,
            $"[{DateTime.Now:HH:mm:ss.fff}] {message}{Environment.NewLine}", Utf8);
      }
    }
    catch { }
  }
}
