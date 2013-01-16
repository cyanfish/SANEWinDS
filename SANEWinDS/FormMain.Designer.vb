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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(FormMain))
        Me.TreeViewOptions = New System.Windows.Forms.TreeView()
        Me.CheckBoxBatchMode = New System.Windows.Forms.CheckBox()
        Me.ToolTip1 = New System.Windows.Forms.ToolTip(Me.components)
        Me.ButtonOK = New System.Windows.Forms.Button()
        Me.ButtonCancel = New System.Windows.Forms.Button()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.TextBoxHost = New System.Windows.Forms.TextBox()
        Me.TextBoxPort = New System.Windows.Forms.TextBox()
        Me.TextBoxDevice = New System.Windows.Forms.TextBox()
        Me.ButtonHost = New System.Windows.Forms.Button()
        Me.SplitContainerOptions = New System.Windows.Forms.SplitContainer()
        Me.ComboBoxPageSize = New System.Windows.Forms.ComboBox()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.PanelOpt = New SANEWinDS.DoubleBufferedPanel()
        CType(Me.SplitContainerOptions, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SplitContainerOptions.Panel1.SuspendLayout()
        Me.SplitContainerOptions.Panel2.SuspendLayout()
        Me.SplitContainerOptions.SuspendLayout()
        Me.SuspendLayout()
        '
        'TreeViewOptions
        '
        Me.TreeViewOptions.Dock = System.Windows.Forms.DockStyle.Fill
        Me.TreeViewOptions.Location = New System.Drawing.Point(0, 0)
        Me.TreeViewOptions.Name = "TreeViewOptions"
        Me.TreeViewOptions.Size = New System.Drawing.Size(198, 260)
        Me.TreeViewOptions.TabIndex = 5
        '
        'CheckBoxBatchMode
        '
        Me.CheckBoxBatchMode.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.CheckBoxBatchMode.AutoSize = True
        Me.CheckBoxBatchMode.Location = New System.Drawing.Point(13, 323)
        Me.CheckBoxBatchMode.Name = "CheckBoxBatchMode"
        Me.CheckBoxBatchMode.Size = New System.Drawing.Size(200, 17)
        Me.CheckBoxBatchMode.TabIndex = 8
        Me.CheckBoxBatchMode.Text = "Scan continuously (for use with ADF)"
        Me.CheckBoxBatchMode.UseVisualStyleBackColor = True
        '
        'ButtonOK
        '
        Me.ButtonOK.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.ButtonOK.Location = New System.Drawing.Point(534, 319)
        Me.ButtonOK.Name = "ButtonOK"
        Me.ButtonOK.Size = New System.Drawing.Size(75, 23)
        Me.ButtonOK.TabIndex = 51
        Me.ButtonOK.Text = "OK"
        Me.ButtonOK.UseVisualStyleBackColor = True
        '
        'ButtonCancel
        '
        Me.ButtonCancel.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.ButtonCancel.Location = New System.Drawing.Point(453, 319)
        Me.ButtonCancel.Name = "ButtonCancel"
        Me.ButtonCancel.Size = New System.Drawing.Size(75, 23)
        Me.ButtonCancel.TabIndex = 50
        Me.ButtonCancel.Text = "Close"
        Me.ButtonCancel.UseVisualStyleBackColor = True
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Location = New System.Drawing.Point(252, 13)
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
        Me.TextBoxHost.Location = New System.Drawing.Point(56, 10)
        Me.TextBoxHost.Name = "TextBoxHost"
        Me.TextBoxHost.Size = New System.Drawing.Size(180, 20)
        Me.TextBoxHost.TabIndex = 2
        '
        'TextBoxPort
        '
        Me.TextBoxPort.Enabled = False
        Me.TextBoxPort.Location = New System.Drawing.Point(284, 10)
        Me.TextBoxPort.Name = "TextBoxPort"
        Me.TextBoxPort.Size = New System.Drawing.Size(50, 20)
        Me.TextBoxPort.TabIndex = 3
        '
        'TextBoxDevice
        '
        Me.TextBoxDevice.Enabled = False
        Me.TextBoxDevice.Location = New System.Drawing.Point(397, 10)
        Me.TextBoxDevice.Name = "TextBoxDevice"
        Me.TextBoxDevice.Size = New System.Drawing.Size(212, 20)
        Me.TextBoxDevice.TabIndex = 4
        '
        'ButtonHost
        '
        Me.ButtonHost.Location = New System.Drawing.Point(13, 8)
        Me.ButtonHost.Name = "ButtonHost"
        Me.ButtonHost.Size = New System.Drawing.Size(37, 23)
        Me.ButtonHost.TabIndex = 1
        Me.ButtonHost.Text = "Host"
        Me.ButtonHost.UseVisualStyleBackColor = True
        '
        'SplitContainerOptions
        '
        Me.SplitContainerOptions.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.SplitContainerOptions.Location = New System.Drawing.Point(13, 45)
        Me.SplitContainerOptions.Name = "SplitContainerOptions"
        '
        'SplitContainerOptions.Panel1
        '
        Me.SplitContainerOptions.Panel1.Controls.Add(Me.TreeViewOptions)
        '
        'SplitContainerOptions.Panel2
        '
        Me.SplitContainerOptions.Panel2.Controls.Add(Me.PanelOpt)
        Me.SplitContainerOptions.Size = New System.Drawing.Size(596, 260)
        Me.SplitContainerOptions.SplitterDistance = 198
        Me.SplitContainerOptions.TabIndex = 28
        '
        'ComboBoxPageSize
        '
        Me.ComboBoxPageSize.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.ComboBoxPageSize.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.ComboBoxPageSize.FormattingEnabled = True
        Me.ComboBoxPageSize.Location = New System.Drawing.Point(293, 321)
        Me.ComboBoxPageSize.Name = "ComboBoxPageSize"
        Me.ComboBoxPageSize.Size = New System.Drawing.Size(133, 21)
        Me.ComboBoxPageSize.TabIndex = 8
        '
        'Label4
        '
        Me.Label4.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.Label4.AutoSize = True
        Me.Label4.Location = New System.Drawing.Point(232, 324)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(55, 13)
        Me.Label4.TabIndex = 53
        Me.Label4.Text = "Page Size"
        '
        'PanelOpt
        '
        Me.PanelOpt.Dock = System.Windows.Forms.DockStyle.Fill
        Me.PanelOpt.Location = New System.Drawing.Point(0, 0)
        Me.PanelOpt.Name = "PanelOpt"
        Me.PanelOpt.Size = New System.Drawing.Size(394, 260)
        Me.PanelOpt.TabIndex = 6
        '
        'FormMain
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(617, 356)
        Me.Controls.Add(Me.Label4)
        Me.Controls.Add(Me.ComboBoxPageSize)
        Me.Controls.Add(Me.ButtonHost)
        Me.Controls.Add(Me.TextBoxDevice)
        Me.Controls.Add(Me.TextBoxPort)
        Me.Controls.Add(Me.TextBoxHost)
        Me.Controls.Add(Me.Label3)
        Me.Controls.Add(Me.Label2)
        Me.Controls.Add(Me.ButtonCancel)
        Me.Controls.Add(Me.ButtonOK)
        Me.Controls.Add(Me.CheckBoxBatchMode)
        Me.Controls.Add(Me.SplitContainerOptions)
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.Name = "FormMain"
        Me.Text = "`"
        Me.SplitContainerOptions.Panel1.ResumeLayout(False)
        Me.SplitContainerOptions.Panel2.ResumeLayout(False)
        CType(Me.SplitContainerOptions, System.ComponentModel.ISupportInitialize).EndInit()
        Me.SplitContainerOptions.ResumeLayout(False)
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents PanelOpt As SANEWinDS.DoubleBufferedPanel 'System.Windows.Forms.Panel
    Friend WithEvents CheckBoxBatchMode As System.Windows.Forms.CheckBox
    Friend WithEvents ToolTip1 As System.Windows.Forms.ToolTip
    Friend WithEvents TreeViewOptions As System.Windows.Forms.TreeView
    Friend WithEvents ButtonOK As System.Windows.Forms.Button
    Friend WithEvents ButtonCancel As System.Windows.Forms.Button
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents Label3 As System.Windows.Forms.Label
    Friend WithEvents TextBoxHost As System.Windows.Forms.TextBox
    Friend WithEvents TextBoxPort As System.Windows.Forms.TextBox
    Friend WithEvents TextBoxDevice As System.Windows.Forms.TextBox
    Friend WithEvents ButtonHost As System.Windows.Forms.Button
    Friend WithEvents SplitContainerOptions As System.Windows.Forms.SplitContainer
    Friend WithEvents ComboBoxPageSize As System.Windows.Forms.ComboBox
    Friend WithEvents Label4 As System.Windows.Forms.Label

End Class
