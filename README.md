# vAltGr Shortcut Filter  ·  v26.10.28.2031

A minimal Windows plug-in for Rhino 8 and Rhino 9 that prevents the extra keyboard-layout character produced after Rhino executes a configured `Ctrl+Alt` or `Ctrl+Shift+Alt` shortcut.

The filter reads Rhino's current shortcut assignments, refreshes automatically when application settings change, and suppresses only the translated character associated with an exact configured shortcut. Ordinary AltGr input remains unchanged when the key combination is not a Rhino shortcut.

## Installation

1. Choose the DLL matching your Rhino version from `bin/Release`:
   - Rhino 8: `Rhino8/net7.0-windows/vAltGrShortcutFilter.Rhino8.dll`
   - Rhino 9: `Rhino9/net10.0-windows/vAltGrShortcutFilter.Rhino9.dll`
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

## Host-Isolated Installation

The installed DLLs, settings, logs and companion files live directly in bin/Release/Rhino8/net7.0-windows, bin/Release/Rhino8/net10.0-windows and bin/Release/Rhino9/net10.0-windows. Rhino 8 variants reference the Rhino 8 SDK; Rhino 9 references Rhino 9. There is no separate deployment directory or post-build copying step. Debug outputs and compiler caches remain separate by host/runtime.

For existing installations, Register.ps1 -Register validates all three DLLs, restores the per-user startup registration request, and updates plug-in/toolbar paths silently. Use -IncludeMachine as an administrator to repair existing machine-wide entries too. Previous values are backed up under backups/registration. Run registration from ordinary Windows PowerShell or from Rhino, not a Codex Windows sandbox: its registry view can differ from Rhino, so sandbox writes are refused. Confirm discovery after a normal Rhino restart; an in-process test does not verify desktop startup. Subsequent builds update the same registered files. Existing configuration values are preserved while missing defaults are added; existing toolbars are never overwritten. Close Rhino before updating a locked Release DLL, run build.ps1 once, then reopen Rhino. Use Debug for verification while Release is in use. The global material database and Rhino-native settings remain in their existing stores.

build.ps1 is a standalone Release build without Git operations; -Rebuild forces compilation when C#-derived versions are unchanged. Publishing current Release outputs runs metadata-only targets without recompiling loaded runtime variants. When compilation is required and an output is locked, the build exits successfully with a warning and preserves the pending message. Publishing remains an explicit user action. GitHub version-tag workflows validate three prebuilt DLLs and package separate Rhino 8/9 ZIPs with repository defaults, help, toolbars and notices. They never ship installed profiles or logs, and retries add only missing assets without overwriting published files.
