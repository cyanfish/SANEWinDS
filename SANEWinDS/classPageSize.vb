'
'   Copyright 2011-2021 Alec Skelly
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
Public Class PageSize
    Implements IComparable(Of PageSize)

    Public Width As Single 'inches
    Public Height As Single 'inches
    Public Name As String
    Public TWAIN_TWSS As TWAIN_VB.TWSS
    Public Sub New()
    End Sub
    Public Sub New(Width As Single, Height As Single)
        Me.Width = Width
        Me.Height = Height
    End Sub
    Public Sub New(Width As Single, Height As Single, Name As String)
        Me.Width = Width
        Me.Height = Height
        Me.Name = Name
    End Sub
    Public Sub New(Width As Single, Height As Single, Name As String, TWSS As TWAIN_VB.TWSS)
        Me.Width = Width
        Me.Height = Height
        Me.Name = Name
        Me.TWAIN_TWSS = TWSS
    End Sub

    Public Function CompareTo(other As PageSize) As Integer Implements IComparable(Of PageSize).CompareTo
        Dim result As Integer = Me.Width.CompareTo(other.Width)
        If result = 0 Then result = Me.Height.CompareTo(other.Height)
        Return result
    End Function

End Class

