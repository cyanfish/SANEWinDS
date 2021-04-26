Changelog:
Version 1.0.7785
-Updated PDF library from iTextSharp to iText7.
-Improved detection of pages sizes supported by scanners.
-Changed PDF page size to match image size.
-Fixed exception when launching from the console of a terminal server.
-Removed obnoxious "hosted by sourceforge" messagebox during first launch.

Version 0.9.5589
-Fixed "Invalid characters in path" bug in SANEWin client when path contained a space.

Version 0.9.5511
-Dramatically increased scanning speed for most scans by reporting progress less frequently.
-Added socket shutdown prior to closing.
-Fixed prioritization of backends in AutoLocate list.
-Suppressed some error messages after failing to connect to a scanner at startup (launches wizard instead).
-Added avision.ini.
-Standardized all backend.ini defaults to 8-bit color, ~150dpi.
-Switched from Debug to Release build to improve performance.

Version 0.9.5500
-Added hpaio.ini to ProgramData\SANEWinDS.
-Automatically decrease SANE_TYPE_FIXED values if rejected by the backend.  Fixes error setting br-x and br-y values (hpaio).
-Ignore SANE option descriptors missing both name and title (hpaio).
-Changed default TCP timeout from 5s to 30s to prevent timeouts with Officejets (hpaio).
-Upgraded NLog and iTextSharp to current versions.

Version 0.9.5434
-"Suppress_Startup_Messages=True" in the [General] section of SANEWinDS.ini will disable the informational messages users get during the first startup of a new version.
-Removed all checks of the TCPClient.Connected property.  It was causing unpredictable network connection behavior on different computers.
-Fixed filename parsing in SANEWIN.exe to resolve hangs with filenames that contained no valid variables.

Version 0.9.5418

-Added fujitsu.ini to ProgramData\SANEWinDS
-Changed behavior of backend.ini processing. Only individual settings present in AppData\<backend>.ini will override settings in ProgramData\<backend>.ini. Previously the entire file was considered an override.
-AutoLocateDevice key in SANEWinDS.ini is now a CSV list of backends to autolocate and can contain an asterisk to match any backend.
-AutoLocateDevice handling improved. No longer requires a value for CurrentDevice in SANEWinDS.ini.
-Improved SANE Host Wizard form.
-Improved GUI for SANEWin.exe.
-Fixed installation to incorrect TWAIN folder in 32-bit Windows and unified 32-bit and 64-bit installers.

Version 0.9.5178

-Preemptively worked around an exception-causing .Net TCPClient class bug noticed in another project.

Version 0.9.5171

-Fixed hang when cancelling scans.
-Added progress bar and cancel button when using TWAIN.

Version 0.9.5144

-Fixed a silly display bug when scanning multiple pages from an automatic document feeder.

Version 0.9.5143

-Moved image acquisition to a new thread.
-Added cancel button during scanning (except TWAIN).
-Added progress bar during scanning (except TWAIN).
-Fixed a bug that caused incomplete image reads from plustek and hp3500 backends (at least).
-Fixed a bug that caused certain settings to get lost under unusual circumstances.
-Added epkowa.ini contributed by Thiago Augusto Almeida dos Santos.

Version 0.8.5129

-Resolved issue with genesys and gt68xx backends (at least) reporting SANE_STATUS_BUSY after scanning. They want Net_Cancel() after sending an image for some reason. Out of spec.
-Added hp3500 backend support.
