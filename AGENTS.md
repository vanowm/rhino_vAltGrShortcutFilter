# Repository Instructions

- Derive every plug-in build version from the latest modified source `*.cs` file, using `yy.M.d.Hmm` with no seconds and non-padded month, day, and hour.
- Keep tracked text files on CRLF line endings through `.gitattributes`.
- Route runtime diagnostics through the shared `Log` helper; keep the log beside the loaded DLL, clear it on startup, and record both plug-in and Rhino versions in the startup entry.
- Build both `net7.0-windows` against Rhino 8 and `net10.0-windows` against Rhino 9.
- Immediately after every agent-made project change, refresh the pending summary with `build.ps1 -ComposeOnly -Message '<specific behavioral summary of all uncommitted changes>'`.
- Never accept generic commit summaries; describe the actual behavior changed.
- `build.ps1` without options performs a standalone no-commit Release build.
- After agent-made changes, run the standalone Release build whenever Rhino is not running. Leave staging, commits, and pushes to the user unless the user explicitly requests publication.
- Build versions shown in README are updated automatically by successful Release builds.
- Keep paths relocatable and do not embed machine-specific absolute paths.
