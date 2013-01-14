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
Public Class FormSANEAuth
    Private Sub OK_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles OK.Click
        Me.DialogResult = Windows.Forms.DialogResult.OK
        Me.Close()
    End Sub

    Private Sub Cancel_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Cancel.Click
        Me.DialogResult = Windows.Forms.DialogResult.Cancel
        Me.Close()
    End Sub

    Private Sub FormSANEAuth_Load(sender As Object, e As EventArgs) Handles Me.Load

    End Sub

    Private Sub FormSANEAuth_Shown(sender As Object, e As EventArgs) Handles Me.Shown
        Try
            If String.IsNullOrEmpty(Me.UsernameTextBox.Text) Then Me.UsernameTextBox.Text = Environment.UserName
            If Not String.IsNullOrEmpty(Me.UsernameTextBox.Text) Then Me.PasswordTextBox.Focus()
        Catch ex As Exception

        End Try
    End Sub
End Class
