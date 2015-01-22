Changelog:

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