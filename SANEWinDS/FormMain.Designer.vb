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
        Me.ButtonSaveOptionValueSet = New System.Windows.Forms.Button()
        Me.ImageList1 = New System.Windows.Forms.ImageList(Me.components)
        Me.ButtonDeleteOptionValueSet = New System.Windows.Forms.Button()
        Me.ComboBoxOptionValueSet = New System.Windows.Forms.ComboBox()
        Me.ButtonHost = New System.Windows.Forms.Button()
        Me.ComboBoxPageSize = New System.Windows.Forms.ComboBox()
        Me.CheckBoxSaveOnExit = New System.Windows.Forms.CheckBox()
        Me.ButtonOK = New System.Windows.Forms.Button()
        Me.ButtonCancel = New System.Windows.Forms.Button()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.TextBoxHost = New System.Windows.Forms.TextBox()
        Me.TextBoxPort = New System.Windows.Forms.TextBox()
        Me.TextBoxDevice = New System.Windows.Forms.TextBox()
        Me.SplitContainerOptions = New System.Windows.Forms.SplitContainer()
        Me.PanelOpt = New SANEWinDS.DoubleBufferedPanel()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.LabelOptionSet = New System.Windows.Forms.Label()
        Me.GroupBoxOptionSets = New System.Windows.Forms.GroupBox()
        Me.GroupBoxDevice = New System.Windows.Forms.GroupBox()
        Me.Label1 = New System.Windows.Forms.Label()
        CType(Me.SplitContainerOptions, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SplitContainerOptions.Panel1.SuspendLayout()
        Me.SplitContainerOptions.Panel2.SuspendLayout()
        Me.SplitContainerOptions.SuspendLayout()
        Me.GroupBoxOptionSets.SuspendLayout()
        Me.GroupBoxDevice.SuspendLayout()
        Me.SuspendLayout()
        '
        'TreeViewOptions
        '
        Me.TreeViewOptions.Dock = System.Windows.Forms.DockStyle.Fill
        Me.TreeViewOptions.Location = New System.Drawing.Point(0, 0)
        Me.TreeViewOptions.Name = "TreeViewOptions"
        Me.TreeViewOptions.Size = New System.Drawing.Size(199, 299)
        Me.TreeViewOptions.TabIndex = 30
        '
        'CheckBoxBatchMode
        '
        Me.CheckBoxBatchMode.AutoSize = True
        Me.CheckBoxBatchMode.Location = New System.Drawing.Point(386, 34)
        Me.CheckBoxBatchMode.Name = "CheckBoxBatchMode"
        Me.CheckBoxBatchMode.Size = New System.Drawing.Size(200, 17)
        Me.CheckBoxBatchMode.TabIndex = 70
        Me.CheckBoxBatchMode.Text = "Scan continuously (for use with ADF)"
        Me.ToolTip1.SetToolTip(Me.CheckBoxBatchMode, resources.GetString("CheckBoxBatchMode.ToolTip"))
        Me.CheckBoxBatchMode.UseVisualStyleBackColor = True
        '
        'ButtonSaveOptionValueSet
        '
        Me.ButtonSaveOptionValueSet.ImageKey = "Save"
        Me.ButtonSaveOptionValueSet.ImageList = Me.ImageList1
        Me.ButtonSaveOptionValueSet.Location = New System.Drawing.Point(320, 14)
        Me.ButtonSaveOptionValueSet.Name = "ButtonSaveOptionValueSet"
        Me.ButtonSaveOptionValueSet.Size = New System.Drawing.Size(24, 23)
        Me.ButtonSaveOptionValueSet.TabIndex = 50
        Me.ToolTip1.SetToolTip(Me.ButtonSaveOptionValueSet, "Save the selected option set")
        Me.ButtonSaveOptionValueSet.UseVisualStyleBackColor = True
        '
        'ImageList1
        '
        Me.ImageList1.ImageStream = CType(resources.GetObject("ImageList1.ImageStream"), System.Windows.Forms.ImageListStreamer)
        Me.ImageList1.TransparentColor = System.Drawing.Color.Transparent
        Me.ImageList1.Images.SetKeyName(0, "Delete")
        Me.ImageList1.Images.SetKeyName(1, "Save")
        Me.ImageList1.Images.SetKeyName(2, "SaveAll")
        Me.ImageList1.Images.SetKeyName(3, "Undo")
        Me.ImageList1.Images.SetKeyName(4, "Reset4")
        Me.ImageList1.Images.SetKeyName(5, "Reset2")
        Me.ImageList1.Images.SetKeyName(6, "Reset")
        Me.ImageList1.Images.SetKeyName(7, "ResetGlobe")
        Me.ImageList1.Images.SetKeyName(8, "ConnectPlug")
        Me.ImageList1.Images.SetKeyName(9, "ConnectLightning")
        Me.ImageList1.Images.SetKeyName(10, "DisconnectPlug")
        Me.ImageList1.Images.SetKeyName(11, "Help")
        Me.ImageList1.Images.SetKeyName(12, "HelpBlue")
        Me.ImageList1.Images.SetKeyName(13, "Information")
        Me.ImageList1.Images.SetKeyName(14, "Settings")
        Me.ImageList1.Images.SetKeyName(15, "SettingsGlobe")
        Me.ImageList1.Images.SetKeyName(16, "NetworkServer")
        Me.ImageList1.Images.SetKeyName(17, "WebServer")
        Me.ImageList1.Images.SetKeyName(18, "NetworkFax")
        '
        'ButtonDeleteOptionValueSet
        '
        Me.ButtonDeleteOptionValueSet.ImageKey = "Delete"
        Me.ButtonDeleteOptionValueSet.ImageList = Me.ImageList1
        Me.ButtonDeleteOptionValueSet.Location = New System.Drawing.Point(345, 14)
        Me.ButtonDeleteOptionValueSet.Name = "ButtonDeleteOptionValueSet"
        Me.ButtonDeleteOptionValueSet.Size = New System.Drawing.Size(24, 23)
        Me.ButtonDeleteOptionValueSet.TabIndex = 55
        Me.ToolTip1.SetToolTip(Me.ButtonDeleteOptionValueSet, "Remove or reset the selected option set")
        Me.ButtonDeleteOptionValueSet.UseVisualStyleBackColor = True
        '
        'ComboBoxOptionValueSet
        '
        Me.ComboBoxOptionValueSet.FormattingEnabled = True
        Me.ComboBoxOptionValueSet.Location = New System.Drawing.Point(67, 16)
        Me.ComboBoxOptionValueSet.Name = "ComboBoxOptionValueSet"
        Me.ComboBoxOptionValueSet.Size = New System.Drawing.Size(247, 21)
        Me.ComboBoxOptionValueSet.TabIndex = 45
        Me.ToolTip1.SetToolTip(Me.ComboBoxOptionValueSet, "Select an existing option set for the current SANE backend, or type a name to cre" &
        "ate a new one")
        '
        'ButtonHost
        '
        Me.ButtonHost.ImageKey = "ConnectLightning"
        Me.ButtonHost.ImageList = Me.ImageList1
        Me.ButtonHost.Location = New System.Drawing.Point(9, 12)
        Me.ButtonHost.Name = "ButtonHost"
        Me.ButtonHost.Size = New System.Drawing.Size(24, 23)
        Me.ButtonHost.TabIndex = 5
        Me.ToolTip1.SetToolTip(Me.ButtonHost, "Connect to a different host or device")
        Me.ButtonHost.UseVisualStyleBackColor = True
        '
        'ComboBoxPageSize
        '
        Me.ComboBoxPageSize.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.ComboBoxPageSize.FormattingEnabled = True
        Me.ComboBoxPageSize.Location = New System.Drawing.Point(67, 46)
        Me.ComboBoxPageSize.Name = "ComboBoxPageSize"
        Me.ComboBoxPageSize.Size = New System.Drawing.Size(133, 21)
        Me.ComboBoxPageSize.TabIndex = 60
        Me.ToolTip1.SetToolTip(Me.ComboBoxPageSize, "Set the scan area to match a predefined page size")
        '
        'CheckBoxSaveOnExit
        '
        Me.CheckBoxSaveOnExit.AutoSize = True
        Me.CheckBoxSaveOnExit.Location = New System.Drawing.Point(386, 16)
        Me.CheckBoxSaveOnExit.Name = "CheckBoxSaveOnExit"
        Me.CheckBoxSaveOnExit.Size = New System.Drawing.Size(170, 17)
        Me.CheckBoxSaveOnExit.TabIndex = 65
        Me.CheckBoxSaveOnExit.Text = "Save current option set on exit"
        Me.ToolTip1.SetToolTip(Me.CheckBoxSaveOnExit, "Save the currently active option set on exit")
        Me.CheckBoxSaveOnExit.UseVisualStyleBackColor = True
        '
        'ButtonOK
        '
        Me.ButtonOK.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.ButtonOK.Location = New System.Drawing.Point(534, 441)
        Me.ButtonOK.Name = "ButtonOK"
        Me.ButtonOK.Size = New System.Drawing.Size(75, 23)
        Me.ButtonOK.TabIndex = 80
        Me.ButtonOK.Text = "OK"
        Me.ButtonOK.UseVisualStyleBackColor = True
        '
        'ButtonCancel
        '
        Me.ButtonCancel.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.ButtonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.ButtonCancel.Location = New System.Drawing.Point(453, 441)
        Me.ButtonCancel.Name = "ButtonCancel"
        Me.ButtonCancel.Size = New System.Drawing.Size(75, 23)
        Me.ButtonCancel.TabIndex = 75
        Me.ButtonCancel.Text = "Close"
        Me.ButtonCancel.UseVisualStyleBackColor = True
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Location = New System.Drawing.Point(262, 18)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(26, 13)
        Me.Label2.TabIndex = 22
        Me.Label2.Text = "Port"
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Location = New System.Drawing.Point(350, 18)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(41, 13)
        Me.Label3.TabIndex = 23
        Me.Label3.Text = "Device"
        '
        'TextBoxHost
        '
        Me.TextBoxHost.Enabled = False
        Me.TextBoxHost.Location = New System.Drawing.Point(77, 15)
        Me.TextBoxHost.Name = "TextBoxHost"
        Me.TextBoxHost.Size = New System.Drawing.Size(179, 20)
        Me.TextBoxHost.TabIndex = 10
        '
        'TextBoxPort
        '
        Me.TextBoxPort.Enabled = False
        Me.TextBoxPort.Location = New System.Drawing.Point(294, 15)
        Me.TextBoxPort.Name = "TextBoxPort"
        Me.TextBoxPort.Size = New System.Drawing.Size(50, 20)
        Me.TextBoxPort.TabIndex = 15
        '
        'TextBoxDevice
        '
        Me.TextBoxDevice.Enabled = False
        Me.TextBoxDevice.Location = New System.Drawing.Point(397, 15)
        Me.TextBoxDevice.Name = "TextBoxDevice"
        Me.TextBoxDevice.Size = New System.Drawing.Size(185, 20)
        Me.TextBoxDevice.TabIndex = 20
        '
        'SplitContainerOptions
        '
        Me.SplitContainerOptions.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.SplitContainerOptions.Location = New System.Drawing.Point(10, 55)
        Me.SplitContainerOptions.Name = "SplitContainerOptions"
        '
        'SplitContainerOptions.Panel1
        '
        Me.SplitContainerOptions.Panel1.Controls.Add(Me.TreeViewOptions)
        '
        'SplitContainerOptions.Panel2
        '
        Me.SplitContainerOptions.Panel2.Controls.Add(Me.PanelOpt)
        Me.SplitContainerOptions.Size = New System.Drawing.Size(600, 299)
        Me.SplitContainerOptions.SplitterDistance = 199
        Me.SplitContainerOptions.TabIndex = 25
        '
        'PanelOpt
        '
        Me.PanelOpt.Dock = System.Windows.Forms.DockStyle.Fill
        Me.PanelOpt.Location = New System.Drawing.Point(0, 0)
        Me.PanelOpt.Name = "PanelOpt"
        Me.PanelOpt.Size = New System.Drawing.Size(397, 299)
        Me.PanelOpt.TabIndex = 35
        '
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.Location = New System.Drawing.Point(6, 49)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(55, 13)
        Me.Label4.TabIndex = 53
        Me.Label4.Text = "Page Size"
        '
        'LabelOptionSet
        '
        Me.LabelOptionSet.AutoSize = True
        Me.LabelOptionSet.Location = New System.Drawing.Point(6, 21)
        Me.LabelOptionSet.Name = "LabelOptionSet"
        Me.LabelOptionSet.Size = New System.Drawing.Size(55, 13)
        Me.LabelOptionSet.TabIndex = 57
        Me.LabelOptionSet.Text = "Option set"
        '
        'GroupBoxOptionSets
        '
        Me.GroupBoxOptionSets.Anchor = CType(((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.GroupBoxOptionSets.Controls.Add(Me.ButtonDeleteOptionValueSet)
        Me.GroupBoxOptionSets.Controls.Add(Me.CheckBoxSaveOnExit)
        Me.GroupBoxOptionSets.Controls.Add(Me.LabelOptionSet)
        Me.GroupBoxOptionSets.Controls.Add(Me.CheckBoxBatchMode)
        Me.GroupBoxOptionSets.Controls.Add(Me.ComboBoxPageSize)
        Me.GroupBoxOptionSets.Controls.Add(Me.ButtonSaveOptionValueSet)
        Me.GroupBoxOptionSets.Controls.Add(Me.Label4)
        Me.GroupBoxOptionSets.Controls.Add(Me.ComboBoxOptionValueSet)
        Me.GroupBoxOptionSets.Location = New System.Drawing.Point(10, 358)
        Me.GroupBoxOptionSets.Name = "GroupBoxOptionSets"
        Me.GroupBoxOptionSets.Size = New System.Drawing.Size(600, 77)
        Me.GroupBoxOptionSets.TabIndex = 40
        Me.GroupBoxOptionSets.TabStop = False
        '
        'GroupBoxDevice
        '
        Me.GroupBoxDevice.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.GroupBoxDevice.Controls.Add(Me.Label1)
        Me.GroupBoxDevice.Controls.Add(Me.TextBoxHost)
        Me.GroupBoxDevice.Controls.Add(Me.Label2)
        Me.GroupBoxDevice.Controls.Add(Me.ButtonHost)
        Me.GroupBoxDevice.Controls.Add(Me.Label3)
        Me.GroupBoxDevice.Controls.Add(Me.TextBoxDevice)
        Me.GroupBoxDevice.Controls.Add(Me.TextBoxPort)
        Me.GroupBoxDevice.Location = New System.Drawing.Point(10, 5)
        Me.GroupBoxDevice.Name = "GroupBoxDevice"
        Me.GroupBoxDevice.Size = New System.Drawing.Size(600, 44)
        Me.GroupBoxDevice.TabIndex = 3
        Me.GroupBoxDevice.TabStop = False
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(42, 18)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(29, 13)
        Me.Label1.TabIndex = 25
        Me.Label1.Text = "Host"
        '
        'FormMain
        '
        Me.AcceptButton = Me.ButtonOK
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.CancelButton = Me.ButtonCancel
        Me.ClientSize = New System.Drawing.Size(621, 476)
        Me.Controls.Add(Me.GroupBoxDevice)
        Me.Controls.Add(Me.GroupBoxOptionSets)
        Me.Controls.Add(Me.ButtonCancel)
        Me.Controls.Add(Me.ButtonOK)
        Me.Controls.Add(Me.SplitContainerOptions)
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.Name = "FormMain"
        Me.SplitContainerOptions.Panel1.ResumeLayout(False)
        Me.SplitContainerOptions.Panel2.ResumeLayout(False)
        CType(Me.SplitContainerOptions, System.ComponentModel.ISupportInitialize).EndInit()
        Me.SplitContainerOptions.ResumeLayout(False)
        Me.GroupBoxOptionSets.ResumeLayout(False)
        Me.GroupBoxOptionSets.PerformLayout()
        Me.GroupBoxDevice.ResumeLayout(False)
        Me.GroupBoxDevice.PerformLayout()
        Me.ResumeLayout(False)

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
    Friend WithEvents ComboBoxOptionValueSet As Windows.Forms.ComboBox
    Friend WithEvents ButtonSaveOptionValueSet As Windows.Forms.Button
    Friend WithEvents CheckBoxSaveOnExit As Windows.Forms.CheckBox
    Friend WithEvents LabelOptionSet As Windows.Forms.Label
    Friend WithEvents GroupBoxOptionSets As Windows.Forms.GroupBox
    Friend WithEvents GroupBoxDevice As Windows.Forms.GroupBox
    Friend WithEvents ButtonDeleteOptionValueSet As Windows.Forms.Button
    Friend WithEvents ImageList1 As Windows.Forms.ImageList
    Friend WithEvents Label1 As Windows.Forms.Label
End Class
