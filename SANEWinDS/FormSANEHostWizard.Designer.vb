<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class FormSANEHostWizard
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.ButtonNext = New System.Windows.Forms.Button()
        Me.PanelHost = New System.Windows.Forms.Panel()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.TextBoxUserName = New System.Windows.Forms.TextBox()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.TextBoxTimeout = New System.Windows.Forms.TextBox()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.TextBoxPort = New System.Windows.Forms.TextBox()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.ComboBoxHost = New System.Windows.Forms.ComboBox()
        Me.PanelDevice = New System.Windows.Forms.Panel()
        Me.ComboBoxDevices = New System.Windows.Forms.ComboBox()
        Me.Label5 = New System.Windows.Forms.Label()
        Me.ButtonPrevious = New System.Windows.Forms.Button()
        Me.ButtonCancel = New System.Windows.Forms.Button()
        Me.PanelHost.SuspendLayout()
        Me.PanelDevice.SuspendLayout()
        Me.SuspendLayout()
        '
        'ButtonNext
        '
        Me.ButtonNext.Location = New System.Drawing.Point(268, 125)
        Me.ButtonNext.Name = "ButtonNext"
        Me.ButtonNext.Size = New System.Drawing.Size(75, 23)
        Me.ButtonNext.TabIndex = 0
        Me.ButtonNext.Text = "Next"
        Me.ButtonNext.UseVisualStyleBackColor = True
        '
        'PanelHost
        '
        Me.PanelHost.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.PanelHost.Controls.Add(Me.Label4)
        Me.PanelHost.Controls.Add(Me.TextBoxUserName)
        Me.PanelHost.Controls.Add(Me.Label3)
        Me.PanelHost.Controls.Add(Me.TextBoxTimeout)
        Me.PanelHost.Controls.Add(Me.Label2)
        Me.PanelHost.Controls.Add(Me.TextBoxPort)
        Me.PanelHost.Controls.Add(Me.Label1)
        Me.PanelHost.Controls.Add(Me.ComboBoxHost)
        Me.PanelHost.Location = New System.Drawing.Point(1, 0)
        Me.PanelHost.Name = "PanelHost"
        Me.PanelHost.Size = New System.Drawing.Size(356, 119)
        Me.PanelHost.TabIndex = 1
        '
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.Location = New System.Drawing.Point(8, 94)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(60, 13)
        Me.Label4.TabIndex = 7
        Me.Label4.Text = "User Name"
        '
        'TextBoxUserName
        '
        Me.TextBoxUserName.Location = New System.Drawing.Point(140, 91)
        Me.TextBoxUserName.Name = "TextBoxUserName"
        Me.TextBoxUserName.Size = New System.Drawing.Size(100, 20)
        Me.TextBoxUserName.TabIndex = 6
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Location = New System.Drawing.Point(8, 68)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(115, 13)
        Me.Label3.TabIndex = 5
        Me.Label3.Text = "Timeout in milliseconds"
        '
        'TextBoxTimeout
        '
        Me.TextBoxTimeout.Location = New System.Drawing.Point(140, 65)
        Me.TextBoxTimeout.Name = "TextBoxTimeout"
        Me.TextBoxTimeout.Size = New System.Drawing.Size(100, 20)
        Me.TextBoxTimeout.TabIndex = 4
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Location = New System.Drawing.Point(8, 43)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(26, 13)
        Me.Label2.TabIndex = 3
        Me.Label2.Text = "Port"
        '
        'TextBoxPort
        '
        Me.TextBoxPort.Location = New System.Drawing.Point(140, 40)
        Me.TextBoxPort.Name = "TextBoxPort"
        Me.TextBoxPort.Size = New System.Drawing.Size(100, 20)
        Me.TextBoxPort.TabIndex = 2
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(8, 15)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(126, 13)
        Me.Label1.TabIndex = 1
        Me.Label1.Text = "Host Name or IP Address"
        '
        'ComboBoxHost
        '
        Me.ComboBoxHost.FormattingEnabled = True
        Me.ComboBoxHost.Location = New System.Drawing.Point(140, 12)
        Me.ComboBoxHost.Name = "ComboBoxHost"
        Me.ComboBoxHost.Size = New System.Drawing.Size(202, 21)
        Me.ComboBoxHost.TabIndex = 0
        '
        'PanelDevice
        '
        Me.PanelDevice.Controls.Add(Me.Label5)
        Me.PanelDevice.Controls.Add(Me.ComboBoxDevices)
        Me.PanelDevice.Location = New System.Drawing.Point(1, 41)
        Me.PanelDevice.Name = "PanelDevice"
        Me.PanelDevice.Size = New System.Drawing.Size(204, 40)
        Me.PanelDevice.TabIndex = 2
        '
        'ComboBoxDevices
        '
        Me.ComboBoxDevices.FormattingEnabled = True
        Me.ComboBoxDevices.Location = New System.Drawing.Point(55, 11)
        Me.ComboBoxDevices.Name = "ComboBoxDevices"
        Me.ComboBoxDevices.Size = New System.Drawing.Size(146, 21)
        Me.ComboBoxDevices.TabIndex = 0
        '
        'Label5
        '
        Me.Label5.AutoSize = True
        Me.Label5.Location = New System.Drawing.Point(8, 15)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(41, 13)
        Me.Label5.TabIndex = 2
        Me.Label5.Text = "Device"
        '
        'ButtonPrevious
        '
        Me.ButtonPrevious.Location = New System.Drawing.Point(187, 125)
        Me.ButtonPrevious.Name = "ButtonPrevious"
        Me.ButtonPrevious.Size = New System.Drawing.Size(75, 23)
        Me.ButtonPrevious.TabIndex = 3
        Me.ButtonPrevious.Text = "Previous"
        Me.ButtonPrevious.UseVisualStyleBackColor = True
        '
        'ButtonCancel
        '
        Me.ButtonCancel.Location = New System.Drawing.Point(12, 125)
        Me.ButtonCancel.Name = "ButtonCancel"
        Me.ButtonCancel.Size = New System.Drawing.Size(75, 23)
        Me.ButtonCancel.TabIndex = 4
        Me.ButtonCancel.Text = "Cancel"
        Me.ButtonCancel.UseVisualStyleBackColor = True
        '
        'FormSANEHostWizard
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(357, 153)
        Me.ControlBox = False
        Me.Controls.Add(Me.ButtonCancel)
        Me.Controls.Add(Me.ButtonPrevious)
        Me.Controls.Add(Me.PanelDevice)
        Me.Controls.Add(Me.PanelHost)
        Me.Controls.Add(Me.ButtonNext)
        Me.Name = "FormSANEHostWizard"
        Me.Text = "Configure SANE Host"
        Me.PanelHost.ResumeLayout(False)
        Me.PanelHost.PerformLayout()
        Me.PanelDevice.ResumeLayout(False)
        Me.PanelDevice.PerformLayout()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents ButtonNext As System.Windows.Forms.Button
    Friend WithEvents PanelHost As System.Windows.Forms.Panel
    Friend WithEvents Label4 As System.Windows.Forms.Label
    Friend WithEvents TextBoxUserName As System.Windows.Forms.TextBox
    Friend WithEvents Label3 As System.Windows.Forms.Label
    Friend WithEvents TextBoxTimeout As System.Windows.Forms.TextBox
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents TextBoxPort As System.Windows.Forms.TextBox
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents ComboBoxHost As System.Windows.Forms.ComboBox
    Friend WithEvents PanelDevice As System.Windows.Forms.Panel
    Friend WithEvents Label5 As System.Windows.Forms.Label
    Friend WithEvents ComboBoxDevices As System.Windows.Forms.ComboBox
    Friend WithEvents ButtonPrevious As System.Windows.Forms.Button
    Friend WithEvents ButtonCancel As System.Windows.Forms.Button
End Class
