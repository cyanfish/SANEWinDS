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
            Me.PasswordTextBox.Focus()
        Catch ex As Exception

        End Try
    End Sub
End Class
