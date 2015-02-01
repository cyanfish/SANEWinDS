Imports System.Windows.Forms
Imports System.Drawing

Public Class FormScanProgress
    Public Event ScanCancelled()

    Public Sub New()

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        Try
            Me.Height = 180
            Me.Width = 500

            Dim lbl As New Label
            lbl.Name = "Status"
            'lbl.Text = Text
            lbl.Top = 10
            lbl.Width = Me.ClientSize.Width
            lbl.Height = 40
            lbl.Anchor = AnchorStyles.Top + AnchorStyles.Left + AnchorStyles.Right
            lbl.Font = New Font("Arial", 14)
            lbl.TextAlign = ContentAlignment.MiddleCenter
            lbl.Parent = Me

            Dim pbar As New ProgressBar
            pbar.Name = "Progress"
            pbar.Top = lbl.Bottom + 10
            pbar.Width = Me.ClientSize.Width - 40
            pbar.Left = (Me.ClientSize.Width \ 2) - (pbar.Width \ 2)
            pbar.Parent = Me

            Dim btn As New Button
            btn.Name = "Cancel"
            btn.Text = "Cancel"
            btn.Parent = Me
            btn.Top = pbar.Bottom + 20
            btn.Width = 60
            btn.Left = (Me.ClientSize.Width \ 2) - (btn.Width \ 2)
            btn.Anchor = AnchorStyles.Top
            AddHandler btn.Click, AddressOf Me.BtnCancel_Click

            Me.FormBorderStyle = Windows.Forms.FormBorderStyle.FixedSingle
            Me.StartPosition = FormStartPosition.CenterScreen
        Catch ex As Exception
            Debug.Print(ex.Message)
        End Try

    End Sub

    Private Sub BtnCancel_Click(sender As Object, e As EventArgs)
        RaiseEvent ScanCancelled()
    End Sub

    Public Sub FormScanProgress_FormClosing(sender As Object, e As FormClosingEventArgs) Handles Me.FormClosing
        If e.CloseReason = CloseReason.UserClosing Then
            Me.Hide()
            e.Cancel = True
        End If
    End Sub

    Private Sub FormScanProgress_Load(sender As Object, e As EventArgs) Handles MyBase.Load

    End Sub

    Public Sub Reset()
        Me.Controls("Status").Text = Nothing
        Dim pbar As ProgressBar = Me.Controls("Progress")
        pbar.Value = 0
    End Sub

    Public Sub ShowProgress(ByVal Text As String)
        Try
            Me.Controls("Status").Text = Text
            Me.Show()
            Me.BringToFront()
        Catch ex As Exception
            Debug.Print(ex.Message)
        End Try
    End Sub

    Public Sub ShowFrameProgress(PercentComplete As Integer)
        If Me.InvokeRequired Then
            Dim d As New dSetImageProgress(AddressOf SetImageProgress)
            Me.Invoke(d, New Object() {PercentComplete})
        Else
            Me.SetImageProgress(PercentComplete)
        End If
    End Sub

    Private Delegate Sub dSetImageProgress(ByVal Progress As Integer)
    Private Sub SetImageProgress(ByVal Progress As Integer)
        Try
            Dim pbar As ProgressBar = Me.Controls("Progress")
            If Progress < 0 Then
                pbar.Style = ProgressBarStyle.Marquee
                pbar.MarqueeAnimationSpeed = 30
            Else
                pbar.Style = ProgressBarStyle.Continuous
                pbar.Value = Progress
            End If
        Catch ex As Exception
            Debug.Print(ex.Message)
        End Try
    End Sub

End Class