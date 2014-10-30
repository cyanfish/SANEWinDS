<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class FormStartup
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(FormStartup))
        Me.ButtonAcquire = New System.Windows.Forms.Button()
        Me.ComboBoxOutputFormat = New System.Windows.Forms.ComboBox()
        Me.ComboBoxCompression = New System.Windows.Forms.ComboBox()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.ComboBoxOutputFolderName = New System.Windows.Forms.ComboBox()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.ButtonBrowseOutputFolder = New System.Windows.Forms.Button()
        Me.TextBoxOutputFileNameBase = New System.Windows.Forms.TextBox()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.CheckBoxOverwriteOutputFile = New System.Windows.Forms.CheckBox()
        Me.Label5 = New System.Windows.Forms.Label()
        Me.CheckBoxViewAfterAcquire = New System.Windows.Forms.CheckBox()
        Me.Label6 = New System.Windows.Forms.Label()
        Me.SuspendLayout()
        '
        'ButtonAcquire
        '
        Me.ButtonAcquire.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.ButtonAcquire.Location = New System.Drawing.Point(421, 141)
        Me.ButtonAcquire.Name = "ButtonAcquire"
        Me.ButtonAcquire.Size = New System.Drawing.Size(75, 23)
        Me.ButtonAcquire.TabIndex = 0
        Me.ButtonAcquire.Text = "Acquire"
        Me.ButtonAcquire.UseVisualStyleBackColor = True
        '
        'ComboBoxOutputFormat
        '
        Me.ComboBoxOutputFormat.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.ComboBoxOutputFormat.FormattingEnabled = True
        Me.ComboBoxOutputFormat.Location = New System.Drawing.Point(134, 12)
        Me.ComboBoxOutputFormat.Name = "ComboBoxOutputFormat"
        Me.ComboBoxOutputFormat.Size = New System.Drawing.Size(70, 21)
        Me.ComboBoxOutputFormat.TabIndex = 1
        '
        'ComboBoxCompression
        '
        Me.ComboBoxCompression.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.ComboBoxCompression.FormattingEnabled = True
        Me.ComboBoxCompression.Location = New System.Drawing.Point(134, 39)
        Me.ComboBoxCompression.Name = "ComboBoxCompression"
        Me.ComboBoxCompression.Size = New System.Drawing.Size(70, 21)
        Me.ComboBoxCompression.TabIndex = 2
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(12, 15)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(74, 13)
        Me.Label1.TabIndex = 3
        Me.Label1.Text = "Output Format"
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Location = New System.Drawing.Point(12, 42)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(67, 13)
        Me.Label2.TabIndex = 4
        Me.Label2.Text = "Compression"
        '
        'ComboBoxOutputFolderName
        '
        Me.ComboBoxOutputFolderName.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.ComboBoxOutputFolderName.FormattingEnabled = True
        Me.ComboBoxOutputFolderName.Location = New System.Drawing.Point(134, 66)
        Me.ComboBoxOutputFolderName.Name = "ComboBoxOutputFolderName"
        Me.ComboBoxOutputFolderName.Size = New System.Drawing.Size(328, 21)
        Me.ComboBoxOutputFolderName.TabIndex = 5
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Location = New System.Drawing.Point(12, 69)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(71, 13)
        Me.Label3.TabIndex = 6
        Me.Label3.Text = "Output Folder"
        '
        'ButtonBrowseOutputFolder
        '
        Me.ButtonBrowseOutputFolder.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.ButtonBrowseOutputFolder.Location = New System.Drawing.Point(468, 64)
        Me.ButtonBrowseOutputFolder.Name = "ButtonBrowseOutputFolder"
        Me.ButtonBrowseOutputFolder.Size = New System.Drawing.Size(28, 23)
        Me.ButtonBrowseOutputFolder.TabIndex = 7
        Me.ButtonBrowseOutputFolder.Text = "..."
        Me.ButtonBrowseOutputFolder.UseVisualStyleBackColor = True
        '
        'TextBoxOutputFileNameBase
        '
        Me.TextBoxOutputFileNameBase.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.TextBoxOutputFileNameBase.Location = New System.Drawing.Point(134, 94)
        Me.TextBoxOutputFileNameBase.Name = "TextBoxOutputFileNameBase"
        Me.TextBoxOutputFileNameBase.Size = New System.Drawing.Size(328, 20)
        Me.TextBoxOutputFileNameBase.TabIndex = 8
        '
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.Location = New System.Drawing.Point(12, 97)
        Me.Label4.Name = "Label4"
        Me.Label4.RightToLeft = System.Windows.Forms.RightToLeft.Yes
        Me.Label4.Size = New System.Drawing.Size(116, 13)
        Me.Label4.TabIndex = 9
        Me.Label4.Text = "Output File Name Base"
        '
        'CheckBoxOverwriteOutputFile
        '
        Me.CheckBoxOverwriteOutputFile.AutoSize = True
        Me.CheckBoxOverwriteOutputFile.Location = New System.Drawing.Point(134, 121)
        Me.CheckBoxOverwriteOutputFile.Name = "CheckBoxOverwriteOutputFile"
        Me.CheckBoxOverwriteOutputFile.Size = New System.Drawing.Size(15, 14)
        Me.CheckBoxOverwriteOutputFile.TabIndex = 10
        Me.CheckBoxOverwriteOutputFile.UseVisualStyleBackColor = True
        '
        'Label5
        '
        Me.Label5.AutoSize = True
        Me.Label5.Location = New System.Drawing.Point(12, 121)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(110, 13)
        Me.Label5.TabIndex = 11
        Me.Label5.Text = "Overwrite Existing File"
        '
        'CheckBoxViewAfterAcquire
        '
        Me.CheckBoxViewAfterAcquire.AutoSize = True
        Me.CheckBoxViewAfterAcquire.Location = New System.Drawing.Point(134, 142)
        Me.CheckBoxViewAfterAcquire.Name = "CheckBoxViewAfterAcquire"
        Me.CheckBoxViewAfterAcquire.Size = New System.Drawing.Size(15, 14)
        Me.CheckBoxViewAfterAcquire.TabIndex = 12
        Me.CheckBoxViewAfterAcquire.UseVisualStyleBackColor = True
        '
        'Label6
        '
        Me.Label6.AutoSize = True
        Me.Label6.Location = New System.Drawing.Point(12, 143)
        Me.Label6.Name = "Label6"
        Me.Label6.Size = New System.Drawing.Size(94, 13)
        Me.Label6.TabIndex = 13
        Me.Label6.Text = "View After Acquire"
        '
        'FormStartup
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(507, 176)
        Me.Controls.Add(Me.Label6)
        Me.Controls.Add(Me.CheckBoxViewAfterAcquire)
        Me.Controls.Add(Me.Label5)
        Me.Controls.Add(Me.CheckBoxOverwriteOutputFile)
        Me.Controls.Add(Me.Label4)
        Me.Controls.Add(Me.TextBoxOutputFileNameBase)
        Me.Controls.Add(Me.ButtonBrowseOutputFolder)
        Me.Controls.Add(Me.Label3)
        Me.Controls.Add(Me.ComboBoxOutputFolderName)
        Me.Controls.Add(Me.Label2)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.ComboBoxCompression)
        Me.Controls.Add(Me.ComboBoxOutputFormat)
        Me.Controls.Add(Me.ButtonAcquire)
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.Name = "FormStartup"
        Me.Text = "FormStartup"
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents ButtonAcquire As System.Windows.Forms.Button
    Friend WithEvents ComboBoxOutputFormat As System.Windows.Forms.ComboBox
    Friend WithEvents ComboBoxCompression As System.Windows.Forms.ComboBox
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents ComboBoxOutputFolderName As System.Windows.Forms.ComboBox
    Friend WithEvents Label3 As System.Windows.Forms.Label
    Friend WithEvents ButtonBrowseOutputFolder As System.Windows.Forms.Button
    Friend WithEvents TextBoxOutputFileNameBase As System.Windows.Forms.TextBox
    Friend WithEvents Label4 As System.Windows.Forms.Label
    Friend WithEvents CheckBoxOverwriteOutputFile As System.Windows.Forms.CheckBox
    Friend WithEvents Label5 As System.Windows.Forms.Label
    Friend WithEvents CheckBoxViewAfterAcquire As System.Windows.Forms.CheckBox
    Friend WithEvents Label6 As System.Windows.Forms.Label
End Class
