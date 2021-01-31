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
        Me.PanelOpt = New SANEWinDS.DoubleBufferedPanel()
        Me.ComboBoxPageSize = New System.Windows.Forms.ComboBox()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.ComboBoxOptionValueSet = New System.Windows.Forms.ComboBox()
        Me.ButtonSaveOptionValues = New System.Windows.Forms.Button()
        Me.CheckBoxSaveOnExit = New System.Windows.Forms.CheckBox()
        Me.LabelOptionSet = New System.Windows.Forms.Label()
        Me.GroupBoxOptionSets = New System.Windows.Forms.GroupBox()
        Me.GroupBoxDevice = New System.Windows.Forms.GroupBox()
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
        Me.TreeViewOptions.Size = New System.Drawing.Size(198, 299)
        Me.TreeViewOptions.TabIndex = 5
        '
        'CheckBoxBatchMode
        '
        Me.CheckBoxBatchMode.AutoSize = True
        Me.CheckBoxBatchMode.Location = New System.Drawing.Point(388, 48)
        Me.CheckBoxBatchMode.Name = "CheckBoxBatchMode"
        Me.CheckBoxBatchMode.Size = New System.Drawing.Size(200, 17)
        Me.CheckBoxBatchMode.TabIndex = 7
        Me.CheckBoxBatchMode.Text = "Scan continuously (for use with ADF)"
        Me.CheckBoxBatchMode.UseVisualStyleBackColor = True
        '
        'ButtonOK
        '
        Me.ButtonOK.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.ButtonOK.Location = New System.Drawing.Point(533, 442)
        Me.ButtonOK.Name = "ButtonOK"
        Me.ButtonOK.Size = New System.Drawing.Size(75, 23)
        Me.ButtonOK.TabIndex = 51
        Me.ButtonOK.Text = "OK"
        Me.ButtonOK.UseVisualStyleBackColor = True
        '
        'ButtonCancel
        '
        Me.ButtonCancel.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.ButtonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.ButtonCancel.Location = New System.Drawing.Point(452, 442)
        Me.ButtonCancel.Name = "ButtonCancel"
        Me.ButtonCancel.Size = New System.Drawing.Size(75, 23)
        Me.ButtonCancel.TabIndex = 50
        Me.ButtonCancel.Text = "Close"
        Me.ButtonCancel.UseVisualStyleBackColor = True
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Location = New System.Drawing.Point(248, 17)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(26, 13)
        Me.Label2.TabIndex = 22
        Me.Label2.Text = "Port"
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Location = New System.Drawing.Point(346, 17)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(41, 13)
        Me.Label3.TabIndex = 23
        Me.Label3.Text = "Device"
        '
        'TextBoxHost
        '
        Me.TextBoxHost.Enabled = False
        Me.TextBoxHost.Location = New System.Drawing.Point(52, 14)
        Me.TextBoxHost.Name = "TextBoxHost"
        Me.TextBoxHost.Size = New System.Drawing.Size(180, 20)
        Me.TextBoxHost.TabIndex = 2
        '
        'TextBoxPort
        '
        Me.TextBoxPort.Enabled = False
        Me.TextBoxPort.Location = New System.Drawing.Point(280, 14)
        Me.TextBoxPort.Name = "TextBoxPort"
        Me.TextBoxPort.Size = New System.Drawing.Size(50, 20)
        Me.TextBoxPort.TabIndex = 3
        '
        'TextBoxDevice
        '
        Me.TextBoxDevice.Enabled = False
        Me.TextBoxDevice.Location = New System.Drawing.Point(393, 14)
        Me.TextBoxDevice.Name = "TextBoxDevice"
        Me.TextBoxDevice.Size = New System.Drawing.Size(195, 20)
        Me.TextBoxDevice.TabIndex = 4
        '
        'ButtonHost
        '
        Me.ButtonHost.Location = New System.Drawing.Point(9, 12)
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
        Me.SplitContainerOptions.Location = New System.Drawing.Point(13, 56)
        Me.SplitContainerOptions.Name = "SplitContainerOptions"
        '
        'SplitContainerOptions.Panel1
        '
        Me.SplitContainerOptions.Panel1.Controls.Add(Me.TreeViewOptions)
        '
        'SplitContainerOptions.Panel2
        '
        Me.SplitContainerOptions.Panel2.Controls.Add(Me.PanelOpt)
        Me.SplitContainerOptions.Size = New System.Drawing.Size(596, 299)
        Me.SplitContainerOptions.SplitterDistance = 198
        Me.SplitContainerOptions.TabIndex = 28
        '
        'PanelOpt
        '
        Me.PanelOpt.Dock = System.Windows.Forms.DockStyle.Fill
        Me.PanelOpt.Location = New System.Drawing.Point(0, 0)
        Me.PanelOpt.Name = "PanelOpt"
        Me.PanelOpt.Size = New System.Drawing.Size(394, 299)
        Me.PanelOpt.TabIndex = 6
        '
        'ComboBoxPageSize
        '
        Me.ComboBoxPageSize.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.ComboBoxPageSize.FormattingEnabled = True
        Me.ComboBoxPageSize.Location = New System.Drawing.Point(67, 46)
        Me.ComboBoxPageSize.Name = "ComboBoxPageSize"
        Me.ComboBoxPageSize.Size = New System.Drawing.Size(133, 21)
        Me.ComboBoxPageSize.TabIndex = 8
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
        'ComboBoxOptionValueSet
        '
        Me.ComboBoxOptionValueSet.FormattingEnabled = True
        Me.ComboBoxOptionValueSet.Location = New System.Drawing.Point(67, 16)
        Me.ComboBoxOptionValueSet.Name = "ComboBoxOptionValueSet"
        Me.ComboBoxOptionValueSet.Size = New System.Drawing.Size(247, 21)
        Me.ComboBoxOptionValueSet.TabIndex = 54
        '
        'ButtonSaveOptionValues
        '
        Me.ButtonSaveOptionValues.Location = New System.Drawing.Point(320, 14)
        Me.ButtonSaveOptionValues.Name = "ButtonSaveOptionValues"
        Me.ButtonSaveOptionValues.Size = New System.Drawing.Size(43, 23)
        Me.ButtonSaveOptionValues.TabIndex = 55
        Me.ButtonSaveOptionValues.Text = "Save"
        Me.ButtonSaveOptionValues.UseVisualStyleBackColor = True
        '
        'CheckBoxSaveOnExit
        '
        Me.CheckBoxSaveOnExit.AutoSize = True
        Me.CheckBoxSaveOnExit.Location = New System.Drawing.Point(388, 18)
        Me.CheckBoxSaveOnExit.Name = "CheckBoxSaveOnExit"
        Me.CheckBoxSaveOnExit.Size = New System.Drawing.Size(160, 17)
        Me.CheckBoxSaveOnExit.TabIndex = 56
        Me.CheckBoxSaveOnExit.Text = "Save 'Local Defaults' on exit"
        Me.CheckBoxSaveOnExit.UseVisualStyleBackColor = True
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
        Me.GroupBoxOptionSets.Controls.Add(Me.CheckBoxSaveOnExit)
        Me.GroupBoxOptionSets.Controls.Add(Me.LabelOptionSet)
        Me.GroupBoxOptionSets.Controls.Add(Me.CheckBoxBatchMode)
        Me.GroupBoxOptionSets.Controls.Add(Me.ComboBoxPageSize)
        Me.GroupBoxOptionSets.Controls.Add(Me.ButtonSaveOptionValues)
        Me.GroupBoxOptionSets.Controls.Add(Me.Label4)
        Me.GroupBoxOptionSets.Controls.Add(Me.ComboBoxOptionValueSet)
        Me.GroupBoxOptionSets.Location = New System.Drawing.Point(13, 359)
        Me.GroupBoxOptionSets.Name = "GroupBoxOptionSets"
        Me.GroupBoxOptionSets.Size = New System.Drawing.Size(596, 77)
        Me.GroupBoxOptionSets.TabIndex = 58
        Me.GroupBoxOptionSets.TabStop = False
        '
        'GroupBoxDevice
        '
        Me.GroupBoxDevice.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.GroupBoxDevice.Controls.Add(Me.TextBoxHost)
        Me.GroupBoxDevice.Controls.Add(Me.Label2)
        Me.GroupBoxDevice.Controls.Add(Me.ButtonHost)
        Me.GroupBoxDevice.Controls.Add(Me.Label3)
        Me.GroupBoxDevice.Controls.Add(Me.TextBoxDevice)
        Me.GroupBoxDevice.Controls.Add(Me.TextBoxPort)
        Me.GroupBoxDevice.Location = New System.Drawing.Point(13, 6)
        Me.GroupBoxDevice.Name = "GroupBoxDevice"
        Me.GroupBoxDevice.Size = New System.Drawing.Size(596, 44)
        Me.GroupBoxDevice.TabIndex = 59
        Me.GroupBoxDevice.TabStop = False
        '
        'FormMain
        '
        Me.AcceptButton = Me.ButtonOK
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.CancelButton = Me.ButtonCancel
        Me.ClientSize = New System.Drawing.Size(617, 476)
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
    Friend WithEvents ButtonSaveOptionValues As Windows.Forms.Button
    Friend WithEvents CheckBoxSaveOnExit As Windows.Forms.CheckBox
    Friend WithEvents LabelOptionSet As Windows.Forms.Label
    Friend WithEvents GroupBoxOptionSets As Windows.Forms.GroupBox
    Friend WithEvents GroupBoxDevice As Windows.Forms.GroupBox
End Class
