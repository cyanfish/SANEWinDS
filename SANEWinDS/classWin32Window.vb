Imports System.Windows.Forms

Public Class classWin32Window
    Implements IWin32Window

    Public Sub New(hwnd As IntPtr)
        Handle = hwnd
    End Sub

    Public ReadOnly Property Handle As IntPtr Implements IWin32Window.Handle
End Class
