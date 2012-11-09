
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
Imports SANEWinDS
Public Class FormStartup
    Dim WithEvents GUIForm As New SANEWinDS.FormMain

    Private Sub FormStartup_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Me.Visible = False
        Try
            Dim DialogResult As DialogResult = GUIForm.ShowDialog
        Catch ex As Exception
            MsgBox(ex.Message)
        End Try
        Me.Close()
    End Sub

    Private Sub OnBatchCompleted(ByVal Pages As Integer) Handles GUIForm.BatchCompleted
        MsgBox("Successfully acquired " & Pages.ToString & " pages")
    End Sub

    Private Sub OnImageError(ByVal PageNumber As Integer, ByVal Message As String) Handles GUIForm.ImageError
        MsgBox("Error acquiring page " & PageNumber.ToString & ": " & Message)
    End Sub

    Private Sub OnImageAcquired(ByVal PageNumber As Integer, ByVal bmp As Bitmap) Handles GUIForm.ImageAcquired
        Dim FileName As String = My.Computer.FileSystem.SpecialDirectories.Temp & "\SANEWin" 'XXX
        If bmp IsNot Nothing Then
            Dim fs As System.IO.FileStream
            Dim ms As System.IO.MemoryStream
            Try
                Dim frm As New Form
                Dim pb As New PictureBox
                pb.Name = "PictureBox1"
                pb.Parent = frm
                pb.Dock = DockStyle.Fill

                frm.Show()
                pb.BorderStyle = BorderStyle.None
                pb.SizeMode = PictureBoxSizeMode.Normal
                pb.Visible = True
                pb.Show()

                ''XXX we have to save as a PNG here or 16bit color images get jacked up.
                Dim fname As String = FileName

                fname += PageNumber.ToString
                fname += ".png"

                bmp.Save(fname, Imaging.ImageFormat.Png)

                bmp = Nothing

                fs = New System.IO.FileStream(fname, System.IO.FileMode.Open)
                ms = New System.IO.MemoryStream
                Dim b(fs.Length - 1) As Byte
                fs.Read(b, 0, fs.Length)
                ms.Write(b, 0, fs.Length)
                fs.Close()
                pb.Image = Image.FromStream(ms)
                ms.Close()
            Catch ex As Exception
                MsgBox("Error creating image: " & ex.Message)
            Finally
                fs = Nothing
                ms = Nothing
                'bmp_data = Nothing
                bmp = Nothing
            End Try
        End If
    End Sub
End Class