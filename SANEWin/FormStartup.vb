Imports SANEWinDS
Public Class FormStartup
    Dim GUIForm As New SANEWinDS.FormMain

    Private Sub FormStartup_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Me.Visible = False
        Try
            Dim DialogResult As DialogResult = GUIForm.ShowDialog
        Catch ex As Exception
            MsgBox(ex.Message)
        End Try
        Me.Close()
    End Sub

End Class