'
'   Copyright 2011, 2012 Alec Skelly
'
'   This file is part of SANEWinDS.
'
'   SANEWinDS is free software: you can redistribute it and/or modify
'   it under the terms of the GNU General Public License as published by
'   the Free Software Foundation, either version 3 of the License, or
'   (at your option) any later version.
'
'   SANEWinDS is distributed in the hope that it will be useful,
'   but WITHOUT ANY WARRANTY; without even the implied warranty of
'   MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
'   GNU General Public License for more details.
'
'   You should have received a copy of the GNU General Public License
'   along with SANEWinDS.  If not, see <http://www.gnu.org/licenses/>.
'
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

    <DllImport("kernel32.dll", CharSet:=CharSet.Auto, ExactSpelling:=True)> _
    Public Shared Function SetProcessWorkingSetSize(ByVal handle As IntPtr, ByVal minSize As Integer, ByVal maxSize As Integer) As Boolean
    End Function

End Class
