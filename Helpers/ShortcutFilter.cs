using System.Runtime.InteropServices;
using Rhino;
using Rhino.ApplicationSettings;
using Rhino.UI;

namespace vAltGrShortcutFilter;

internal static class ShortcutFilter
{
  private const int WhGetMessage = 3;
  private const int PmRemove = 1;
  private const int WmKeyDown = 0x0100;
  private const int WmKeyUp = 0x0101;
  private const int WmChar = 0x0102;
  private const int WmDeadChar = 0x0103;
  private const int WmSysKeyDown = 0x0104;
  private const int WmSysKeyUp = 0x0105;
  private const int WmSysChar = 0x0106;
  private const int WmSysDeadChar = 0x0107;
  private const int WmUniChar = 0x0109;
  private const int VkShift = 0x10;
  private const int VkControl = 0x11;
  private const int VkAlt = 0x12;

  private static readonly HookProc Callback = OnGetMessage;
  private static HashSet<int> _shortcuts = new();
  private static IntPtr _hook;
  private static bool _started;
  private static bool _waitingForIdle;
  private static int _armedKey;
  private static long _armedAt;

  internal static void Start()
  {
    if (_started || !OperatingSystem.IsWindows())
      return;

    _started = true;
    RefreshShortcuts();
    RhinoApp.AppSettingsChanged += OnSettingsChanged;
    InstallHook();
  }

  internal static void Stop()
  {
    if (!_started)
      return;

    _started = false;
    RhinoApp.AppSettingsChanged -= OnSettingsChanged;
    if (_waitingForIdle)
    {
      RhinoApp.Idle -= OnIdle;
      _waitingForIdle = false;
    }

    ClearArmed();
    if (_hook != IntPtr.Zero)
    {
      UnhookWindowsHookEx(_hook);
      _hook = IntPtr.Zero;
    }
  }

  private static void InstallHook()
  {
    var window = RhinoApp.MainWindowHandle();
    var thread = window == IntPtr.Zero
      ? 0u
      : GetWindowThreadProcessId(window, out _);

    if (thread != 0)
      _hook = SetWindowsHookEx(WhGetMessage, Callback, IntPtr.Zero, thread);

    if (_hook != IntPtr.Zero)
    {
      Log.Write($"hook installed  shortcuts={_shortcuts.Count}");
      return;
    }

    if (!_waitingForIdle)
    {
      _waitingForIdle = true;
      RhinoApp.Idle += OnIdle;
    }
  }

  private static void OnIdle(object? sender, EventArgs e)
  {
    RhinoApp.Idle -= OnIdle;
    _waitingForIdle = false;
    if (_started)
      InstallHook();
  }

  private static void OnSettingsChanged(object? sender, EventArgs e) =>
    RefreshShortcuts();

  private static void RefreshShortcuts()
  {
    var ctrlAlt = ModifierKey.Control | ModifierKey.Alt;
    _shortcuts = ShortcutKeySettings.GetShortcuts()
      .Where(shortcut =>
        shortcut.Key != KeyboardKey.None &&
        !string.IsNullOrWhiteSpace(shortcut.Macro) &&
        (shortcut.Modifier & ctrlAlt) == ctrlAlt)
      .Select(shortcut => Encode(shortcut.Key, shortcut.Modifier))
      .ToHashSet();
    Log.Write($"shortcuts refreshed  count={_shortcuts.Count}");
  }

  private static void ArmIfShortcut(int key)
  {
    if (key <= 0)
      return;

    var modifiers = ModifierKey.None;
    if (IsDown(VkControl)) modifiers |= ModifierKey.Control;
    if (IsDown(VkShift)) modifiers |= ModifierKey.Shift;
    if (IsDown(VkAlt)) modifiers |= ModifierKey.Alt;

    if (!_shortcuts.Contains(Encode((KeyboardKey)key, modifiers)))
      return;

    _armedKey = key;
    _armedAt = Environment.TickCount64;
    Log.Write($"armed  key={(KeyboardKey)key}  modifiers={modifiers}");
  }

  private static IntPtr OnGetMessage(int code, IntPtr removeFlag, IntPtr messagePointer)
  {
    if (code >= 0 && removeFlag.ToInt64() == PmRemove &&
        messagePointer != IntPtr.Zero)
    {
      var message = Marshal.PtrToStructure<NativeMessage>(messagePointer);
      if (message.Message == WmKeyDown || message.Message == WmSysKeyDown)
      {
        ArmIfShortcut(unchecked((int)message.WParam.ToUInt64()));
      }
      else if (_armedKey != 0)
      {
        if (Environment.TickCount64 - _armedAt > 5000)
        {
          ClearArmed();
        }
        else if (IsCharacterMessage(message.Message))
        {
          Marshal.WriteInt32(messagePointer, IntPtr.Size, 0);
          Log.Write($"suppressed  key={(KeyboardKey)_armedKey}  char=U+{message.WParam.ToUInt64():X4}");
          ClearArmed();
        }
        else if ((message.Message == WmKeyUp || message.Message == WmSysKeyUp) &&
                 unchecked((int)message.WParam.ToUInt64()) == _armedKey)
        {
          ClearArmed();
        }
      }
    }

    return CallNextHookEx(_hook, code, removeFlag, messagePointer);
  }

  private static bool IsCharacterMessage(uint message) =>
    message == WmChar || message == WmDeadChar ||
    message == WmSysChar || message == WmSysDeadChar ||
    message == WmUniChar;

  private static bool IsDown(int key) => (GetKeyState(key) & 0x8000) != 0;

  private static int Encode(KeyboardKey key, ModifierKey modifiers) =>
    ((int)modifiers << 16) | ((int)key & 0xffff);

  private static void ClearArmed()
  {
    _armedKey = 0;
    _armedAt = 0;
  }

  [StructLayout(LayoutKind.Sequential)]
  private readonly struct NativePoint
  {
    public readonly int X;
    public readonly int Y;
  }

  [StructLayout(LayoutKind.Sequential)]
  private readonly struct NativeMessage
  {
    public readonly IntPtr Window;
    public readonly uint Message;
    public readonly UIntPtr WParam;
    public readonly IntPtr LParam;
    public readonly uint Time;
    public readonly NativePoint Point;
    public readonly uint Private;
  }

  private delegate IntPtr HookProc(int code, IntPtr removeFlag, IntPtr messagePointer);

  [DllImport("user32.dll", SetLastError = true)]
  private static extern IntPtr SetWindowsHookEx(
    int hookType, HookProc callback, IntPtr module, uint threadId);

  [DllImport("user32.dll")]
  [return: MarshalAs(UnmanagedType.Bool)]
  private static extern bool UnhookWindowsHookEx(IntPtr hook);

  [DllImport("user32.dll")]
  private static extern IntPtr CallNextHookEx(
    IntPtr hook, int code, IntPtr removeFlag, IntPtr messagePointer);

  [DllImport("user32.dll")]
  private static extern uint GetWindowThreadProcessId(IntPtr window, out uint processId);

  [DllImport("user32.dll")]
  private static extern short GetKeyState(int key);
}
