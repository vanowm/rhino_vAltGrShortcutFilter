# vAltGr Shortcut Filter  ·  v26.7.30.1131

A minimal Windows plug-in for Rhino 8 and Rhino 9 that prevents the extra keyboard-layout character produced after Rhino executes a configured `Ctrl+Alt` or `Ctrl+Shift+Alt` shortcut.

The filter reads Rhino's current shortcut assignments, refreshes automatically when application settings change, and suppresses only the translated character associated with an exact configured shortcut. Ordinary AltGr input remains unchanged when the key combination is not a Rhino shortcut.

## Installation

1. Choose the DLL matching your Rhino version from `bin/Release`:
   - Rhino 8: `net7.0-windows/vAltGrShortcutFilter.dll`
   - Rhino 9: `net10.0-windows/vAltGrShortcutFilter.dll`
2. Open Rhino's plug-in manager and install the DLL.
3. Restart Rhino.

The plug-in loads at startup and has no commands or configuration UI. Disable it through Rhino's plug-in manager to turn the filter off.

Runtime diagnostics are written to `vAltGrShortcutFilter.log` beside the loaded DLL.

## Requirements

- Windows
- Rhino 8 for `net7.0-windows`
- Rhino 9 for `net10.0-windows`

## License

MIT
