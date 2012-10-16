Public Class Form2
    Public ImageToOpen As String
    Private fs As System.IO.FileStream

    Private Sub Form2_FormClosed(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosedEventArgs) Handles Me.FormClosed
        'Me.PictureBox1.Image = Nothing
        'fs.Close()
    End Sub

    Private Sub Form2_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        'If Me.ImageToOpen IsNot Nothing Then
        '    fs = New System.IO.FileStream(ImageToOpen, System.IO.FileMode.Open)
        '    Me.PictureBox1.Image = New Bitmap(fs)
        'End If
    End Sub
End Class