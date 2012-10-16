Imports System.Runtime.InteropServices
Public Class WinAPI
    Public Enum GlobalAllocFlags As UInt32
        GMEM_FIXED = &H0
        GMEM_MOVEABLE = &H2
        GMEM_ZEROINIT = &H40
        GPTR = &H40
        GHND = GMEM_MOVEABLE And GMEM_ZEROINIT
    End Enum

    <DllImport("user32.dll", CharSet:=CharSet.Auto, ExactSpelling:=True)> _
    Public Shared Function IsDialogMessage(ByVal hWndDlg As IntPtr, ByVal lpMsg As IntPtr) As Boolean
    End Function

    <DllImport("kernel32.dll", CharSet:=CharSet.Auto, ExactSpelling:=True)> _
    Public Shared Function GlobalAlloc(ByVal flags As Int32, ByVal size As Int32) As IntPtr
    End Function

    <DllImport("kernel32.dll", CharSet:=CharSet.Auto, ExactSpelling:=True)> _
    Public Shared Function GlobalLock(ByVal handle As IntPtr) As IntPtr
    End Function

    <DllImport("kernel32.dll", CharSet:=CharSet.Auto, ExactSpelling:=True)> _
    Public Shared Function GlobalUnlock(ByVal handle As IntPtr) As Boolean
    End Function

    <DllImport("kernel32.dll", CharSet:=CharSet.Auto, ExactSpelling:=True)> _
    Public Shared Function GlobalFree(ByVal handle As IntPtr) As IntPtr
    End Function

End Class
