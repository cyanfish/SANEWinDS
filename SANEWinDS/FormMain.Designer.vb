<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class FormMain
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
        Me.components = New System.ComponentModel.Container()
        Me.PanelOpt = New System.Windows.Forms.Panel()
        Me.CheckBoxBatchMode = New System.Windows.Forms.CheckBox()
        Me.ToolTip1 = New System.Windows.Forms.ToolTip(Me.components)
        Me.TreeViewOptions = New System.Windows.Forms.TreeView()
        Me.ButtonOK = New System.Windows.Forms.Button()
        Me.ButtonCancel = New System.Windows.Forms.Button()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.TextBoxHost = New System.Windows.Forms.TextBox()
        Me.TextBoxPort = New System.Windows.Forms.TextBox()
        Me.TextBoxDevice = New System.Windows.Forms.TextBox()
        Me.SuspendLayout()
        '
        'PanelOpt
        '
        Me.PanelOpt.Location = New System.Drawing.Point(217, 45)
        Me.PanelOpt.Name = "PanelOpt"
        Me.PanelOpt.Size = New System.Drawing.Size(392, 250)
        Me.PanelOpt.TabIndex = 15
        '
        'CheckBoxBatchMode
        '
        Me.CheckBoxBatchMode.AutoSize = True
        Me.CheckBoxBatchMode.Location = New System.Drawing.Point(12, 311)
        Me.CheckBoxBatchMode.Name = "CheckBoxBatchMode"
        Me.CheckBoxBatchMode.Size = New System.Drawing.Size(200, 17)
        Me.CheckBoxBatchMode.TabIndex = 17
        Me.CheckBoxBatchMode.Text = "Scan continuously (for use with ADF)"
        Me.CheckBoxBatchMode.UseVisualStyleBackColor = True
        '
        'TreeViewOptions
        '
        Me.TreeViewOptions.Location = New System.Drawing.Point(12, 44)
        Me.TreeViewOptions.Name = "TreeViewOptions"
        Me.TreeViewOptions.Size = New System.Drawing.Size(199, 251)
        Me.TreeViewOptions.TabIndex = 18
        '
        'ButtonOK
        '
        Me.ButtonOK.Location = New System.Drawing.Point(534, 311)
        Me.ButtonOK.Name = "ButtonOK"
        Me.ButtonOK.Size = New System.Drawing.Size(75, 23)
        Me.ButtonOK.TabIndex = 19
        Me.ButtonOK.Text = "OK"
        Me.ButtonOK.UseVisualStyleBackColor = True
        '
        'ButtonCancel
        '
        Me.ButtonCancel.Location = New System.Drawing.Point(453, 311)
        Me.ButtonCancel.Name = "ButtonCancel"
        Me.ButtonCancel.Size = New System.Drawing.Size(75, 23)
        Me.ButtonCancel.TabIndex = 20
        Me.ButtonCancel.Text = "Cancel"
        Me.ButtonCancel.UseVisualStyleBackColor = True
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(13, 13)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(29, 13)
        Me.Label1.TabIndex = 21
        Me.Label1.Text = "Host"
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Location = New System.Drawing.Point(241, 13)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(26, 13)
        Me.Label2.TabIndex = 22
        Me.Label2.Text = "Port"
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Location = New System.Drawing.Point(350, 13)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(41, 13)
        Me.Label3.TabIndex = 23
        Me.Label3.Text = "Device"
        '
        'TextBoxHost
        '
        Me.TextBoxHost.Enabled = False
        Me.TextBoxHost.Location = New System.Drawing.Point(48, 10)
        Me.TextBoxHost.Name = "TextBoxHost"
        Me.TextBoxHost.Size = New System.Drawing.Size(179, 20)
        Me.TextBoxHost.TabIndex = 24
        '
        'TextBoxPort
        '
        Me.TextBoxPort.Enabled = False
        Me.TextBoxPort.Location = New System.Drawing.Point(273, 10)
        Me.TextBoxPort.Name = "TextBoxPort"
        Me.TextBoxPort.Size = New System.Drawing.Size(61, 20)
        Me.TextBoxPort.TabIndex = 25
        '
        'TextBoxDevice
        '
        Me.TextBoxDevice.Enabled = False
        Me.TextBoxDevice.Location = New System.Drawing.Point(397, 10)
        Me.TextBoxDevice.Name = "TextBoxDevice"
        Me.TextBoxDevice.Size = New System.Drawing.Size(212, 20)
        Me.TextBoxDevice.TabIndex = 26
        '
        'FormMain
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(617, 344)
        Me.Controls.Add(Me.TextBoxDevice)
        Me.Controls.Add(Me.TextBoxPort)
        Me.Controls.Add(Me.TextBoxHost)
        Me.Controls.Add(Me.Label3)
        Me.Controls.Add(Me.Label2)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.ButtonCancel)
        Me.Controls.Add(Me.ButtonOK)
        Me.Controls.Add(Me.TreeViewOptions)
        Me.Controls.Add(Me.CheckBoxBatchMode)
        Me.Controls.Add(Me.PanelOpt)
        Me.Name = "FormMain"
        Me.Text = "Form1"
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents PanelOpt As System.Windows.Forms.Panel
    Friend WithEvents CheckBoxBatchMode As System.Windows.Forms.CheckBox
    Friend WithEvents ToolTip1 As System.Windows.Forms.ToolTip
    Friend WithEvents TreeViewOptions As System.Windows.Forms.TreeView
    Friend WithEvents ButtonOK As System.Windows.Forms.Button
    Friend WithEvents ButtonCancel As System.Windows.Forms.Button
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents Label3 As System.Windows.Forms.Label
    Friend WithEvents TextBoxHost As System.Windows.Forms.TextBox
    Friend WithEvents TextBoxPort As System.Windows.Forms.TextBox
    Friend WithEvents TextBoxDevice As System.Windows.Forms.TextBox

End Class
