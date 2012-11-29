'
'   Copyright 2011, 2012 Alec Skelly
'
'   This file is part of SANEWinDS.
'
'   SANEWinDS is free software: you can redistribute it and/or modify
'   it under the terms of the GNU General Public License as published by
'   the Free Software Foundation, either version 3 of the License, or
'   (at your option) any later version.
'
'   SANEWinDS is distributed in the hope that it will be useful,
'   but WITHOUT ANY WARRANTY; without even the implied warranty of
'   MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
'   GNU General Public License for more details.
'
'   You should have received a copy of the GNU General Public License
'   along with SANEWinDS.  If not, see <http://www.gnu.org/licenses/>.
'
Imports System.Windows.Forms
Imports System.Drawing

Public Class FormMain
    Public Event BatchStarted()
    Public Event ImageAcquired(ByVal PageNumber As Integer, ByVal Bmp As Bitmap)
    Public Event ImageError(ByVal PageNumber As Integer, ByVal Message As String)
    Public Event BatchCompleted(ByVal Pages As Integer)

    Dim Host As SharedSettings.HostInfo

    Dim OptionValueControls(-1) As Control
    Public Got_MSG_CLOSEDS As Boolean = False
    Public Enum UIMode As Integer
        Scan = 0
        Configure = 1
    End Enum
    Public TWAIN_Is_Active As Boolean = False
    Public TWAINInstance As TWAIN_VB.DS_Entry_Pump
    Public Mode As UIMode = UIMode.Scan
    Private PanelOptIsDirty As Boolean

    Private Sub CloseCurrentHost()
        Logger.Write(DebugLogger.Level.Debug, False, "")
        Try
            Me.CloseCurrentDevice()
            If net IsNot Nothing Then
                If net.Connected Then SANE.Net_Exit(net)
                Dim stream As System.Net.Sockets.NetworkStream = net.GetStream
                stream.Close()
                stream = Nothing
                If net.Connected Then net.Close()
                net = Nothing
            End If
        Catch ex As Exception
            MsgBox(ex.Message)
        End Try
    End Sub

    Private Function BytesToString(ByVal buffer As Byte(), ByVal offset As Integer, ByVal length As Integer) As String
        Dim s As String = Nothing
        For i = offset To offset + length - 1
            s += Chr(buffer(i))
        Next
        Return s
    End Function

    Private Sub CloseCurrentDevice()
        Logger.Write(DebugLogger.Level.Debug, False, "")
        Try
            If SANE IsNot Nothing Then
                If SANE.CurrentDevice.Name IsNot Nothing Then
                    If SANE.CurrentDevice.Open Then
                        SANE.Net_Close(net, SANE.CurrentDevice.Handle)
                        SANE.CurrentDevice.Open = False
                    End If
                End If
            End If
        Catch ex As Exception
            Logger.Write(DebugLogger.Level.Error_, True, ex.Message)
        End Try
    End Sub

    Private Sub ButtonOK_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ButtonOK.Click
        Logger.Write(DebugLogger.Level.Debug, False, "")
        If Not TWAIN_Is_Active Then 'TWAIN_VB registers its own event handler
            Dim PageNo As Integer = 0
            If SANE.CurrentDevice.Name IsNot Nothing Then
                If SANE.CurrentDevice.Open Then
                    Me.ComboBoxDestination.Enabled = False
                    RaiseEvent BatchStarted()
                    Dim Status As SANE_API.SANE_Status = 0
                    Do
                        PageNo += 1
                        Dim bmp As Bitmap = Nothing
                        Try
                            Status = AcquireImage(bmp)
                            If Status = SANE_API.SANE_Status.SANE_STATUS_GOOD Then
                                If bmp IsNot Nothing Then
                                    RaiseEvent ImageAcquired(PageNo, bmp)
                                    bmp.Dispose()
                                    bmp = Nothing
                                    If Not CurrentSettings.ScanContinuously Then
                                        RaiseEvent BatchCompleted(PageNo)
                                        Exit Do
                                    End If
                                End If
                            ElseIf Status = SANE_API.SANE_Status.SANE_STATUS_NO_DOCS Then
                                RaiseEvent BatchCompleted(PageNo - 1)
                            Else
                                RaiseEvent ImageError(PageNo, Status.ToString)
                            End If
                        Catch ex As Exception
                            RaiseEvent ImageError(PageNo, ex.Message)
                            Exit Do
                        End Try
                    Loop While Status = SANE_API.SANE_Status.SANE_STATUS_GOOD And CurrentSettings.ScanContinuously = True
                    Me.ComboBoxDestination.Enabled = True
                Else
                    RaiseEvent ImageError(PageNo, "The SANE device '" & SANE.CurrentDevice.Name & "' has not been opened")
                End If
            Else
                RaiseEvent ImageError(PageNo, "The SANE device name is not defined")
            End If
            WinAPI.SetProcessWorkingSetSize(Process.GetCurrentProcess().Handle, -1, -1)
        End If
    End Sub

    Friend Sub GetOpts(ByVal Recreate As Boolean)
        Try
            If Recreate Then
                Me.TreeViewOptions.Nodes.Clear()
            End If

            Dim Descriptors() As SANE_API.SANE_Option_Descriptor
            Descriptors = SANE.Net_Get_Option_Descriptors(net, SANE.CurrentDevice.Handle)
            SANE.CurrentDevice.OptionDescriptors = Descriptors
            ReDim SANE.CurrentDevice.OptionValues(Descriptors.Length - 1)

            Dim BackEndConfigFile As String = CurrentSettings.GetDeviceConfigFileName() 'this will create the config file if it doesn't already exist

            Dim GroupNode As TreeNode = Nothing
            Dim AdvancedNode As TreeNode = Nothing
            For i As Integer = 1 To Descriptors.Length - 1 'skip the first element, which contains the array length
                ReDim SANE.CurrentDevice.OptionValues(i)(SANE.OptionValueArrayLength(Descriptors(i).size, Descriptors(i).type) - 1) 'make sure it's not Nothing, even for unreadable options
                Select Case Descriptors(i).type
                    Case SANE_API.SANE_Value_Type.SANE_TYPE_GROUP
                        If Recreate Then
                            GroupNode = New TreeNode
                            GroupNode.Tag = i
                            GroupNode.Name = Descriptors(i).name
                            GroupNode.Text = Descriptors(i).title
                            Me.TreeViewOptions.Nodes.Add(GroupNode)

                            AdvancedNode = New TreeNode
                            AdvancedNode.Tag = i * -1
                            AdvancedNode.Text = "Advanced"

                        End If
                    Case Else
                        If Recreate Then
                            Dim tn As New TreeNode
                            tn.Tag = i
                            tn.Name = Descriptors(i).name
                            tn.Text = Descriptors(i).title

                            If Descriptors(i).cap And SANE_API.SANE_CAP_ADVANCED Then
                                If AdvancedNode.Parent Is Nothing Then
                                    If GroupNode IsNot Nothing Then
                                        GroupNode.Nodes.Add(AdvancedNode)
                                    Else
                                        Me.TreeViewOptions.Nodes.Add(AdvancedNode)
                                    End If
                                End If
                                AdvancedNode.Nodes.Add(tn)
                            Else
                                If GroupNode IsNot Nothing Then
                                    GroupNode.Nodes.Add(tn)
                                Else
                                    Me.TreeViewOptions.Nodes.Add(tn)
                                End If
                            End If

                        End If
                        If Descriptors(i).type <> SANE_API.SANE_Value_Type.SANE_TYPE_BUTTON Then
                            If SANE.SANE_OPTION_IS_READABLE(Descriptors(i).cap) Then
                                Dim OptReq As New SANE_API.SANENetControlOption_Request
                                Dim OptReply As New SANE_API.SANENetControlOption_Reply

                                OptReq.handle = SANE.CurrentDevice.Handle
                                OptReq.option_ = i
                                OptReq.action = SANE_API.SANE_Action.SANE_ACTION_GET_VALUE
                                OptReq.value_type = Descriptors(i).type
                                OptReq.value_size = Descriptors(i).size
                                OptReq.values = Nothing

                                Dim Status As SANE_API.SANE_Status = SANE.Net_Control_Option(net, OptReq, OptReply)
                                If Status = SANE_API.SANE_Status.SANE_STATUS_GOOD Then
                                    ReDim SANE.CurrentDevice.OptionValues(i)(OptReply.values.Length - 1)
                                    Array.Copy(OptReply.values, SANE.CurrentDevice.OptionValues(i), OptReply.values.Length)
                                Else
                                    MsgBox("Error reading '" & Descriptors(i).type.ToString & "' option '" & Descriptors(i).title & "': " & Status.ToString)
                                End If
                            End If
                        End If
                        'MsgBox("Name: " & Devices(i).name & vbCr & "Vendor: " & Devices(i).vendor & vbCr & "Model: " & Devices(i).model & vbCr & "Type: " & Devices(i).type)
                End Select
            Next
            If Not Me.TWAIN_Is_Active Then
                If Not CurrentSettings.ScanContinuouslyUserConfigured Then
                    CurrentSettings.ScanContinuously = Device_Appears_To_Have_ADF() AndAlso Device_Appears_To_Have_ADF_Enabled()
                    Me.CheckBoxBatchMode.Checked = CurrentSettings.ScanContinuously
                End If
            End If
        Catch ex As Exception
            Logger.Write(DebugLogger.Level.Error_, True, ex.Message)
        End Try

    End Sub

    Private Sub Form1_FormClosing(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing
        Me.CloseCurrentHost()
        If net IsNot Nothing Then
            If net.Connected Then net.Close()
            net = Nothing
        End If
    End Sub

    Private Sub Form1_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        Me.Text = My.Application.Info.ProductName & " " & My.Application.Info.Version.ToString

        Dim UseRoamingAppData As Boolean = False
        Try
            UseRoamingAppData = My.Settings.UseRoamingAppData
        Catch ex As Exception
        End Try

        If Logger Is Nothing Then Logger = New DebugLogger(UseRoamingAppData)

        Logger.Write(DebugLogger.Level.Debug, False, "")

        Me.ButtonOK.Enabled = False
        Me.ButtonHost.Enabled = Not TWAIN_Is_Active 'XXX is it ok to reconfigure with TWAIN active?

        If SANE Is Nothing Then SANE = New SANE_API

        If Not TWAIN_Is_Active Then
            'If SANE Is Nothing Then SANE = New SANE_API
            If Not (CurrentSettings.HostIsValid(CurrentSettings.SANE.CurrentHost) AndAlso (CurrentSettings.SANE.CurrentDevice IsNot Nothing) AndAlso CurrentSettings.SANE.CurrentDevice.Length) Then
                Dim f As New FormSANEHostWizard
                If f.ShowDialog <> Windows.Forms.DialogResult.OK Then
                    Logger.Write(DebugLogger.Level.Debug, False, "User cancelled SANE host wizard")
                    'XXX
                End If
            End If
            Try_Init_SANE()
        End If

        If CurrentSettings.SANE.CurrentHost.NameOrAddress IsNot Nothing Then
            Host = CurrentSettings.SANE.CurrentHost
        Else

        End If

        If Me.Mode = UIMode.Scan Then Me.ButtonOK.Text = "Scan" Else Me.ButtonOK.Text = "OK"
        If TWAIN_Is_Active Then
            With Me.ComboBoxDestination
                .Items.Add("TWAIN")
                .SelectedIndex = 0
                .Enabled = False
            End With
        Else
            'Let the caller decide what output formats to enable.
        End If
    End Sub

    Public Sub AddOutputDestinationString(Destination As String)
        If Destination IsNot Nothing Then
            If Not Me.ComboBoxDestination.Items.Contains(Destination) Then
                Me.ComboBoxDestination.Items.Add(Destination)
            End If
        End If
    End Sub
    Public Sub SetOutputDestinationString(Destination As String)
        If Destination IsNot Nothing Then
            If Me.ComboBoxDestination.Items.Contains(Destination) Then
                Me.ComboBoxDestination.Text = Destination
            End If
        End If
    End Sub
    Public Function GetOutputDestinationString() As String
        Return Me.ComboBoxDestination.Text
    End Function

    Private Sub Try_Init_SANE()
        Try
            If net Is Nothing Then net = New System.Net.Sockets.TcpClient
            If net IsNot Nothing Then
                If CurrentSettings.SANE.CurrentHost.NameOrAddress IsNot Nothing Then
                    net.ReceiveTimeout = CurrentSettings.SANE.CurrentHost.TCP_Timeout_ms
                    net.SendTimeout = CurrentSettings.SANE.CurrentHost.TCP_Timeout_ms
                    Logger.Write(DebugLogger.Level.Debug, False, "TCPClient Send buffer length is " & net.SendBufferSize)
                    Logger.Write(DebugLogger.Level.Debug, False, "TCPClient Receive buffer length is " & net.ReceiveBufferSize)
                    net.Connect(CurrentSettings.SANE.CurrentHost.NameOrAddress, CurrentSettings.SANE.CurrentHost.Port)
                    Dim Status As SANE_API.SANE_Status = SANE.Net_Init(net, CurrentSettings.SANE.CurrentHost.Username)
                    Logger.Write(DebugLogger.Level.Debug, False, "Net_Init returned status '" & Status.ToString & "'")
                    If Status = SANE_API.SANE_Status.SANE_STATUS_GOOD Then CurrentSettings.SANE.CurrentHost.Open = True
                    If CurrentSettings.SANE.CurrentHost.Open Then
                        SANE.CurrentDevice = New SANE_API.CurrentDeviceInfo

                        Dim DeviceHandle As Integer
                        Status = SANE.Net_Open(net, CurrentSettings.SANE.CurrentDevice, DeviceHandle)
                        Logger.Write(DebugLogger.Level.Debug, False, "Net_Open returned status '" & Status.ToString & "'")


                        If Status = SANE_API.SANE_Status.SANE_STATUS_INVAL Then  'Auto-Locate
                            If CurrentSettings.SANE.AutoLocateDevice IsNot Nothing AndAlso CurrentSettings.SANE.AutoLocateDevice.Length > 0 Then
                                Logger.Write(DebugLogger.Level.Debug, False, "Attempting to auto-locate devices matching '" & CurrentSettings.SANE.AutoLocateDevice & "'")
                                Dim Devices(-1) As SANE_API.SANE_Device
                                Status = SANE.Net_Get_Devices(net, Devices)
                                If Status = SANE_API.SANE_Status.SANE_STATUS_GOOD Then
                                    For i As Integer = 0 To Devices.Length - 1
                                        Status = SANE_API.SANE_Status.SANE_STATUS_INVAL
                                        If Devices(i).name.Trim.Length >= CurrentSettings.SANE.AutoLocateDevice.Length Then
                                            If Devices(i).name.Trim.Substring(0, CurrentSettings.SANE.AutoLocateDevice.Length) = CurrentSettings.SANE.AutoLocateDevice Then
                                                Logger.Write(DebugLogger.Level.Debug, False, "Auto-located device '" & Devices(i).name & "'; attempting to open...")
                                                Status = SANE.Net_Open(net, Devices(i).name, DeviceHandle)
                                                Logger.Write(DebugLogger.Level.Debug, False, "Net_Open returned status '" & Status.ToString & "'")
                                                If Status = SANE_API.SANE_Status.SANE_STATUS_GOOD Then CurrentSettings.SANE.CurrentDevice = Devices(i).name
                                                Exit For
                                            End If
                                        End If
                                    Next
                                End If
                            End If
                        End If

                        If Status = SANE_API.SANE_Status.SANE_STATUS_GOOD Then
                            SANE.CurrentDevice.Name = CurrentSettings.SANE.CurrentDevice
                            SANE.CurrentDevice.Handle = DeviceHandle
                            SANE.CurrentDevice.Open = True
                        Else
                            'XXX
                        End If
                    End If
                Else
                    Logger.Write(DebugLogger.Level.Warn, True, "No host is configured")
                End If

                If SANE.CurrentDevice.Open Then

                    Me.GetOpts(True)  'must occur prior to reading GetDeviceConfigFileName()!

                    CurrentSettings.SANE.CurrentDeviceINI = New IniFile
                    Dim s As String = CurrentSettings.GetDeviceConfigFileName()
                    If s IsNot Nothing AndAlso s.Length > 0 Then CurrentSettings.SANE.CurrentDeviceINI.Load(s)

                    Me.SetUserDefaults()

                    Me.ButtonOK.Enabled = True
                Else
                    'XXX
                End If

            End If
        Catch ex As Exception
            Logger.Write(DebugLogger.Level.Error_, True, ex.Message)
            Close_SANE()
        Finally
            Close_Net()
        End Try

    End Sub

    Private Sub OptionButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)
        SetOption()
    End Sub

    Private Sub DisplayOption(ByRef Panel As Panel, ByVal OptionIndex As Int32)

        'XXX Need to implement SANE_CAP_AUTOMATIC

        Dim od As SANE_API.SANE_Option_Descriptor = SANE.CurrentDevice.OptionDescriptors(OptionIndex)
        'Dim tt As ToolTip = Me.ToolTip1

        Dim BorderHeight As Integer = 0
        Dim BorderWidth As Integer = 0
        Dim _tbox As New TextBox
        If _tbox.BorderStyle = BorderStyle.Fixed3D Then
            BorderHeight = System.Windows.Forms.SystemInformation.Border3DSize.Height
            BorderWidth = System.Windows.Forms.SystemInformation.Border3DSize.Width
        Else
            BorderHeight = System.Windows.Forms.SystemInformation.BorderSize.Height
            BorderWidth = System.Windows.Forms.SystemInformation.BorderSize.Width
        End If

        PanelOpt.Controls.Clear()
        ReDim OptionValueControls(SANE.CurrentDevice.OptionValues(OptionIndex).Length - 1)

        Select Case od.type
            Case SANE_API.SANE_Value_Type.SANE_TYPE_BOOL
                Dim vOffs As Integer = 0
                For j = 0 To SANE.CurrentDevice.OptionValues(OptionIndex).Length - 1
                    Dim chk As New CheckBox

                    chk.Top = vOffs
                    vOffs += chk.Height + BorderHeight
                    chk.Text = SANE.UnitString(od.unit)
                    chk.Name = "ctl_" & od.name
                    chk.AutoSize = True

                    chk.Enabled = SANE.SANE_OPTION_IS_ACTIVE(od.cap) And SANE.SANE_OPTION_IS_SETTABLE(od.cap)

                    If SANE.CurrentDevice.OptionValues IsNot Nothing Then
                        If SANE.CurrentDevice.OptionValues(OptionIndex) IsNot Nothing Then
                            If SANE.CurrentDevice.OptionValues(OptionIndex).length - 1 >= j Then
                                If SANE.CurrentDevice.OptionValues(OptionIndex)(j) IsNot Nothing Then
                                    chk.Checked = SANE.CurrentDevice.OptionValues(OptionIndex)(j)
                                End If
                            End If
                        End If
                    End If

                    Me.OptionValueControls(j) = chk
                    AddHandler chk.CheckedChanged, AddressOf OptionControl_TextChanged
                    AddHandler chk.Leave, AddressOf OptionControl_Leave
                    PanelOpt.Controls.Add(chk)
                Next

                Dim lbl As New Label
                If Me.OptionValueControls.Length > 0 Then
                    lbl.Top = Me.OptionValueControls(Me.OptionValueControls.Length - 1).Bottom + 10
                    lbl.Height = PanelOpt.Bottom - Me.OptionValueControls(Me.OptionValueControls.Length - 1).Top - 10
                    lbl.Left = Me.OptionValueControls(Me.OptionValueControls.Length - 1).Left
                    lbl.Enabled = Me.OptionValueControls(Me.OptionValueControls.Length - 1).Enabled
                Else
                    lbl.Top = 0
                    lbl.Height = PanelOpt.Height
                    lbl.Left = 0
                    lbl.Enabled = False
                End If
                lbl.Width = PanelOpt.Width
                lbl.TextAlign = ContentAlignment.TopLeft
                lbl.Text = od.desc
                lbl.AutoSize = False

                PanelOpt.Controls.Add(lbl)

            Case SANE_API.SANE_Value_Type.SANE_TYPE_BUTTON
                Dim btn As New Button
                'btn.Tag = od.title
                btn.Text = od.title
                btn.Name = "ctl_" & od.name
                btn.Enabled = SANE.SANE_OPTION_IS_ACTIVE(od.cap) And SANE.SANE_OPTION_IS_SETTABLE(od.cap)
                AddHandler btn.Click, AddressOf Me.SetOption
                PanelOpt.Controls.Add(btn)

                Dim lbl As New Label
                lbl.Top = btn.Bottom + 10
                lbl.Height = PanelOpt.Bottom - lbl.Top - 10
                lbl.Left = btn.Left
                lbl.Width = PanelOpt.Width
                lbl.TextAlign = ContentAlignment.TopLeft
                lbl.Enabled = btn.Enabled
                lbl.Text = od.desc
                lbl.AutoSize = False
                PanelOpt.Controls.Add(lbl)

            Case SANE_API.SANE_Value_Type.SANE_TYPE_FIXED
                Dim vOffs As Integer = 0
                For j = 0 To SANE.CurrentDevice.OptionValues(OptionIndex).Length - 1
                    Dim ctl As Control = Nothing
                    Select Case od.constraint_type
                        Case SANE_API.SANE_Constraint_Type.SANE_CONSTRAINT_NONE
                            Dim tb As New TextBox
                            tb.Top = vOffs
                            vOffs += tb.Height + BorderHeight
                            ctl = tb
                        Case SANE_API.SANE_Constraint_Type.SANE_CONSTRAINT_RANGE
                            Dim ud As New NumericUpDown
                            ud.Top = vOffs
                            vOffs += ud.Height + BorderHeight
                            ud.Minimum = SANE.SANE_UNFIX(od.constraint.range.min)
                            ud.Maximum = SANE.SANE_UNFIX(od.constraint.range.max)
                            ud.Increment = SANE.SANE_UNFIX(od.constraint.range.quant)
                            ud.DecimalPlaces = 4
                            ctl = ud
                        Case SANE_API.SANE_Constraint_Type.SANE_CONSTRAINT_WORD_LIST
                            Dim cb As New ComboBox
                            For k As Integer = 0 To od.constraint.word_list.Length - 1
                                cb.Items.Add(SANE.SANE_UNFIX(od.constraint.word_list(k)))
                            Next
                            ctl = cb
                        Case Else
                            MsgBox("Unexpected constraint type '" & od.constraint_type.ToString & "' for value type '" & od.type.ToString & "'")
                    End Select
                    ctl.Name = "ctl_" & od.name
                    ctl.Enabled = SANE.SANE_OPTION_IS_ACTIVE(od.cap) And SANE.SANE_OPTION_IS_SETTABLE(od.cap)
                    Me.OptionValueControls(j) = ctl

                    Dim ulbl As New Label
                    ulbl.Top = ctl.Top
                    ulbl.Left = ctl.Right + BorderWidth
                    ulbl.Width = PanelOpt.Right - ctl.Right - (BorderWidth * 2)
                    ulbl.TextAlign = ContentAlignment.MiddleLeft
                    ulbl.Text = SANE.UnitString(od.unit)
                    ulbl.AutoSize = False
                    ulbl.Anchor = AnchorStyles.Left
                    ulbl.Enabled = ctl.Enabled

                    If SANE.CurrentDevice.OptionValues IsNot Nothing Then
                        If SANE.CurrentDevice.OptionValues(OptionIndex) IsNot Nothing Then
                            If SANE.CurrentDevice.OptionValues(OptionIndex).length - 1 >= j Then
                                If SANE.CurrentDevice.OptionValues(OptionIndex)(j) IsNot Nothing Then
                                    If ctl.GetType = GetType(NumericUpDown) Then
                                        Dim ud As NumericUpDown = DirectCast(ctl, NumericUpDown)
                                        ud.Value = SANE.CurrentDevice.OptionValues(OptionIndex)(j)

                                        ulbl.Text += " (" & ud.Minimum.ToString("0.####") & " to " & ud.Maximum.ToString("0.####")
                                        If od.constraint.range.quant > 0 Then
                                            ulbl.Text += " in steps of " & ud.Increment.ToString("0.####")
                                        End If
                                        ulbl.Text += ")"
                                    Else
                                        Dim d As Double = SANE.CurrentDevice.OptionValues(OptionIndex)(j)
                                        ctl.Text = d.ToString("0.####")
                                    End If
                                End If
                            End If
                        End If
                    End If

                    AddHandler ctl.TextChanged, AddressOf OptionControl_TextChanged
                    AddHandler ctl.Leave, AddressOf OptionControl_Leave
                    PanelOpt.Controls.Add(ctl)
                    PanelOpt.Controls.Add(ulbl)

                Next

                Dim lbl As New Label

                If Me.OptionValueControls.Length > 0 Then
                    lbl.Top = Me.OptionValueControls(Me.OptionValueControls.Length - 1).Bottom + 10
                    lbl.Height = PanelOpt.Bottom - Me.OptionValueControls(Me.OptionValueControls.Length - 1).Top - 10
                    lbl.Left = Me.OptionValueControls(Me.OptionValueControls.Length - 1).Left
                    lbl.Enabled = Me.OptionValueControls(Me.OptionValueControls.Length - 1).Enabled
                Else
                    lbl.Top = 0
                    lbl.Height = PanelOpt.Height
                    lbl.Left = 0
                    lbl.Enabled = False
                End If
                lbl.Width = PanelOpt.Width
                lbl.TextAlign = ContentAlignment.TopLeft
                lbl.Text = od.desc
                lbl.AutoSize = False
                lbl.Anchor = AnchorStyles.Left + AnchorStyles.Right + AnchorStyles.Bottom

                PanelOpt.Controls.Add(lbl)

            Case SANE_API.SANE_Value_Type.SANE_TYPE_GROUP

            Case SANE_API.SANE_Value_Type.SANE_TYPE_INT
                Dim vOffs As Integer = 0
                For j = 0 To SANE.CurrentDevice.OptionValues(OptionIndex).Length - 1

                    Dim ctl As Control = Nothing
                    Select Case od.constraint_type
                        Case SANE_API.SANE_Constraint_Type.SANE_CONSTRAINT_NONE
                            Dim tb As New TextBox
                            tb.Top = vOffs
                            vOffs += tb.Height + BorderHeight
                            ctl = tb
                        Case SANE_API.SANE_Constraint_Type.SANE_CONSTRAINT_RANGE
                            Dim ud As New NumericUpDown
                            ud.Top = vOffs
                            vOffs += ud.Height + BorderHeight
                            ud.Minimum = od.constraint.range.min
                            ud.Maximum = od.constraint.range.max
                            ud.Increment = od.constraint.range.quant
                            ud.DecimalPlaces = 0
                            ctl = ud
                        Case SANE_API.SANE_Constraint_Type.SANE_CONSTRAINT_WORD_LIST
                            Dim cb As New ComboBox
                            For k As Integer = 0 To od.constraint.word_list.Length - 1
                                cb.Items.Add(od.constraint.word_list(k))
                            Next
                            ctl = cb
                        Case Else
                            MsgBox("Unexpected constraint type '" & od.constraint_type.ToString & "' for value type '" & od.type.ToString & "'")
                    End Select
                    ctl.Name = "ctl_" & od.name
                    ctl.Enabled = SANE.SANE_OPTION_IS_ACTIVE(od.cap) And SANE.SANE_OPTION_IS_SETTABLE(od.cap)
                    Me.OptionValueControls(j) = ctl

                    If SANE.CurrentDevice.OptionValues IsNot Nothing Then
                        If SANE.CurrentDevice.OptionValues(OptionIndex) IsNot Nothing Then
                            If SANE.CurrentDevice.OptionValues(OptionIndex).length - 1 >= j Then
                                If SANE.CurrentDevice.OptionValues(OptionIndex)(j) IsNot Nothing Then
                                    If ctl.GetType = GetType(NumericUpDown) Then
                                        Dim ud As NumericUpDown = DirectCast(ctl, NumericUpDown)
                                        ud.Value = SANE.CurrentDevice.OptionValues(OptionIndex)(j)
                                    Else
                                        ctl.Text = SANE.CurrentDevice.OptionValues(OptionIndex)(j).ToString
                                    End If
                                End If
                            End If
                        End If
                    End If

                    Dim ulbl As New Label
                    ulbl.Top = ctl.Top
                    ulbl.Left = ctl.Right + BorderWidth
                    ulbl.Width = PanelOpt.Right - ctl.Right - (BorderWidth * 2)
                    ulbl.TextAlign = ContentAlignment.MiddleLeft
                    ulbl.Text = SANE.UnitString(od.unit)
                    If od.constraint_type = SANE_API.SANE_Constraint_Type.SANE_CONSTRAINT_RANGE Then
                        ulbl.Text += " (" & od.constraint.range.min.ToString & " to " & od.constraint.range.max.ToString
                        If od.constraint.range.quant > 0 Then
                            ulbl.Text += " in steps of " & od.constraint.range.quant.ToString
                        End If
                        ulbl.Text += ")"
                    End If
                    ulbl.AutoSize = False
                    ulbl.Anchor = AnchorStyles.Left
                    ulbl.Enabled = ctl.Enabled

                    AddHandler ctl.TextChanged, AddressOf OptionControl_TextChanged
                    AddHandler ctl.Leave, AddressOf OptionControl_Leave
                    PanelOpt.Controls.Add(ctl)
                    PanelOpt.Controls.Add(ulbl)

                Next

                Dim lbl As New Label

                If Me.OptionValueControls.Length > 0 Then
                    lbl.Top = Me.OptionValueControls(Me.OptionValueControls.Length - 1).Bottom + 10
                    lbl.Height = PanelOpt.Bottom - Me.OptionValueControls(Me.OptionValueControls.Length - 1).Top - 10
                    lbl.Left = Me.OptionValueControls(Me.OptionValueControls.Length - 1).Left
                    lbl.Enabled = Me.OptionValueControls(Me.OptionValueControls.Length - 1).Enabled
                Else
                    lbl.Top = 0
                    lbl.Height = PanelOpt.Height
                    lbl.Left = 0
                    lbl.Enabled = False
                End If
                lbl.Width = PanelOpt.Width
                lbl.TextAlign = ContentAlignment.TopLeft
                lbl.Text = od.desc
                lbl.AutoSize = False

                PanelOpt.Controls.Add(lbl)

            Case SANE_API.SANE_Value_Type.SANE_TYPE_STRING
                'XXX could there be an array of strings?
                Dim ctl As Control = Nothing
                Select Case od.constraint_type
                    Case SANE_API.SANE_Constraint_Type.SANE_CONSTRAINT_NONE
                        Dim tb As New TextBox
                        tb.MaxLength = od.size
                        tb.Width = GetTextWidth(StrDup(tb.MaxLength, "A"), tb)
                        ctl = tb
                    Case SANE_API.SANE_Constraint_Type.SANE_CONSTRAINT_STRING_LIST
                        Dim cb As New ComboBox
                        Dim MaxWidth As Integer = 0
                        For j As Integer = 0 To od.constraint.string_list.Length - 1
                            cb.Items.Add(od.constraint.string_list(j))
                            If od.constraint.string_list(j) IsNot Nothing Then
                                Dim Width As Integer = GetTextWidth(od.constraint.string_list(j) + "AA", cb) '+AA to compensate for the dropdown control
                                If Width > MaxWidth Then MaxWidth = Width
                            End If
                        Next
                        cb.MaxLength = od.size
                        If MaxWidth Then
                            cb.Width = MaxWidth + (2 * BorderWidth)
                        Else
                            cb.Width = GetTextWidth(StrDup(cb.MaxLength + 2, "A"), cb) '+2 to compensate for the dropdown control
                        End If
                        ctl = cb
                    Case Else
                        MsgBox("Unexpected constraint type '" & od.constraint_type.ToString & "' for value type '" & od.type.ToString & "'")
                End Select
                ctl.Name = "ctl_" & od.name
                ctl.Enabled = SANE.SANE_OPTION_IS_ACTIVE(od.cap) And SANE.SANE_OPTION_IS_SETTABLE(od.cap)

                If SANE.CurrentDevice.OptionValues IsNot Nothing Then
                    If SANE.CurrentDevice.OptionValues(OptionIndex) IsNot Nothing Then
                        If SANE.CurrentDevice.OptionValues(OptionIndex).length - 1 >= 0 Then
                            If SANE.CurrentDevice.OptionValues(OptionIndex)(0) IsNot Nothing Then
                                ctl.Text = SANE.CurrentDevice.OptionValues(OptionIndex)(0).ToString
                            End If
                        End If
                    End If
                End If

                Me.OptionValueControls(0) = ctl
                AddHandler ctl.TextChanged, AddressOf OptionControl_TextChanged
                AddHandler ctl.Leave, AddressOf OptionControl_Leave
                PanelOpt.Controls.Add(ctl)

                Dim lbl As New Label
                lbl.Top = ctl.Top
                'lbl.Height = PanelOpt.Bottom - lbl.Top - 10
                lbl.Left = ctl.Right + 2
                lbl.Width = PanelOpt.Right - ctl.Right - 4
                lbl.TextAlign = ContentAlignment.TopLeft
                lbl.Text = SANE.UnitString(od.unit)
                lbl.AutoSize = False
                lbl.Enabled = ctl.Enabled
                PanelOpt.Controls.Add(lbl)

                lbl = New Label
                lbl.Top = ctl.Bottom + 10
                lbl.Height = PanelOpt.Bottom - lbl.Top - 10
                lbl.Left = ctl.Left
                lbl.Width = PanelOpt.Width
                lbl.TextAlign = ContentAlignment.TopLeft
                lbl.Text = od.desc
                lbl.AutoSize = False
                lbl.Enabled = ctl.Enabled
                PanelOpt.Controls.Add(lbl)

        End Select

    End Sub

    Private Function GetTextWidth(ByVal Text As String, ByVal DestinationControl As Control) As Integer
        Dim g As Graphics = Graphics.FromHwnd(DestinationControl.Handle)
        Dim f As SizeF = g.MeasureString(Text, DestinationControl.Font)
        Return f.Width
    End Function

    Private Sub OptionControl_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs)
        Me.PanelOptIsDirty = True
    End Sub

    Private Sub OptionControl_Leave(ByVal sender As System.Object, ByVal e As System.EventArgs)
        If Me.PanelOptIsDirty Then
            SetOption()
        Else
            'MsgBox("Not dirty!")
        End If
    End Sub

    Public Sub SetUserDefaults()
        'read all DefaultValue= settings from the backend.ini and set live values
        Logger.Write(DebugLogger.Level.Debug, False, "begin")
        If CurrentSettings.SANE.CurrentDeviceINI IsNot Nothing Then
            Try
                Dim opt As String = CurrentSettings.SANE.CurrentDeviceINI.GetKeyValue("General", "ScanContinuously")
                If opt IsNot Nothing Then
                    If opt.Length Then
                        CurrentSettings.ScanContinuously = Convert.ToBoolean(opt)
                        CurrentSettings.ScanContinuouslyUserConfigured = True
                    End If
                End If
            Catch ex As Exception
                Logger.Write(DebugLogger.Level.Error_, False, "Exception reading ScanContinuously setting from backend.ini: " & ex.Message)
            End Try
            Logger.Write(DebugLogger.Level.Info, False, "ScanContinuously = '" & CurrentSettings.ScanContinuously.ToString & "'")
            Me.CheckBoxBatchMode.Checked = CurrentSettings.ScanContinuously
            'XXX need to set CAP_FEEDERENABLED here or does an event fire?

            If TWAIN_Is_Active Then
                Try
                    Dim opt As String = CurrentSettings.SANE.CurrentDeviceINI.GetKeyValue("General", "MaxPaperWidth")
                    If opt IsNot Nothing Then
                        If opt.Length Then
                            Dim d As Double
                            If Double.TryParse(opt, d) Then
                                Me.TWAINInstance.SetCap(TWAIN_VB.CAP.ICAP_PHYSICALWIDTH, d, TWAIN_VB.DS_Entry_Pump.RequestSource.SANE)
                            Else
                                Throw New Exception("Unable to interpret '" & opt & "' as a decimal number")
                            End If
                        End If
                    End If
                Catch ex As Exception
                    Logger.Write(DebugLogger.Level.Error_, False, "Exception reading MaxPaperWidth setting from backend.ini: " & ex.Message)
                End Try
                Try
                    Dim opt As String = CurrentSettings.SANE.CurrentDeviceINI.GetKeyValue("General", "MaxPaperHeight")
                    If opt IsNot Nothing Then
                        If opt.Length Then
                            Dim d As Double
                            If Double.TryParse(opt, d) Then
                                Me.TWAINInstance.SetCap(TWAIN_VB.CAP.ICAP_PHYSICALHEIGHT, d, TWAIN_VB.DS_Entry_Pump.RequestSource.SANE)
                            Else
                                Throw New Exception("Unable to interpret '" & opt & "' as a decimal number")
                            End If
                        End If
                    End If
                Catch ex As Exception
                    Logger.Write(DebugLogger.Level.Error_, False, "Exception reading MaxPaperHeight setting from backend.ini: " & ex.Message)
                End Try
            End If

            For i As Integer = 1 To SANE.CurrentDevice.OptionDescriptors.Count - 1 'skip the first option, which is just the option count
                Select Case SANE.CurrentDevice.OptionDescriptors(i).type
                    Case SANE_API.SANE_Value_Type.SANE_TYPE_GROUP, SANE_API.SANE_Value_Type.SANE_TYPE_BUTTON
                        'no need to map these options
                    Case Else
                        Dim optval As String = CurrentSettings.SANE.CurrentDeviceINI.GetKeyValue("Option." & SANE.CurrentDevice.OptionDescriptors(i).name, "DefaultValue")
                        If optval IsNot Nothing Then
                            If optval.Length Then
                                If SANE.SANE_OPTION_IS_ACTIVE(SANE.CurrentDevice.OptionDescriptors(i).cap) And SANE.SANE_OPTION_IS_SETTABLE(SANE.CurrentDevice.OptionDescriptors(i).cap) Then
                                    Logger.Write(DebugLogger.Level.Debug, False, "importing setting '" & SANE.CurrentDevice.OptionDescriptors(i).name & "' = '" & optval & "'")
                                    Dim Values() As Object = optval.Split(",")
                                    SetOpt(i, Values)
                                Else
                                    Logger.Write(DebugLogger.Level.Warn, False, "Option '" & SANE.CurrentDevice.OptionDescriptors(i).title & "' is not currently settable")
                                End If
                            End If
                        End If
                End Select
            Next
            Me.PanelOpt.Controls.Clear()
        Else
            Logger.Write(DebugLogger.Level.Warn, False, "Backend configuration file uninitialized; backend-specific default values were not configured")
        End If
        Logger.Write(DebugLogger.Level.Debug, False, "end")
    End Sub

    Private Sub SetTWAINCaps(ByVal OptionDescriptor As SANE_API.SANE_Option_Descriptor, ByVal OptionValues() As Object)
        If OptionValues.Length > 1 Then Logger.Write(DebugLogger.Level.Warn, False, "Only the first value in the array will be evaluated for option '" & OptionDescriptor.title & "'")
        If CurrentSettings.SANE.CurrentDeviceINI IsNot Nothing Then
            Dim s As String = CurrentSettings.SANE.CurrentDeviceINI.GetKeyValue("Option." & OptionDescriptor.name, "TWAIN." & OptionValues(0).ToString.Replace(" ", ""))
            'If there wasn't a TWAIN mapping for the specific value that was set, look for a general mapping.
            If (s Is Nothing) OrElse (s.Length = 0) Then s = CurrentSettings.SANE.CurrentDeviceINI.GetKeyValue("Option." & OptionDescriptor.name, "TWAIN")
            If s IsNot Nothing AndAlso s.Length Then
                Dim caps() As String = s.Split(";")
                Dim capName(caps.Length - 1) As String
                Dim capVal(caps.Length - 1) As String
                For i = 0 To caps.Length - 1
                    Dim ss() As String = caps(i).Split(",")
                    If ss.Length = 2 Then
                        capName(i) = ss(0).Trim.ToUpper
                        capVal(i) = ss(1).Trim.ToUpper
                        Logger.Write(DebugLogger.Level.Debug, False, "TWAIN Capability = '" & capName(i) & "', Value = '" & capVal(i) & "'")
                        If Me.TWAINInstance IsNot Nothing Then
                            Dim n As TWAIN_VB.CAP
                            If [Enum].TryParse(capName(i), True, n) Then
                                If capVal(i) = "#" Then capVal(i) = OptionValues(0)
                                Me.TWAINInstance.SetCap(n, capVal(i), TWAIN_VB.DS_Entry_Pump.RequestSource.SANE)
                            Else
                                Logger.Write(DebugLogger.Level.Warn, False, "Unknown TWAIN capability '" & capName(i) & "'")
                            End If
                        End If
                    Else
                        Logger.Write(DebugLogger.Level.Warn, False, "Malformed TWAIN capability mapping: '" & caps(i) & "'")
                    End If
                Next
            End If
        Else
            Logger.Write(DebugLogger.Level.Warn, False, "Backend configuration file uninitialized; SANE to TWAIN capability mappings were not configured")
        End If
    End Sub

    Public Function GetSANEOption(ByVal OptionName As String) As Object()
        For Index As Integer = 0 To SANE.CurrentDevice.OptionDescriptors.Length - 1
            Dim od As SANE_API.SANE_Option_Descriptor = SANE.CurrentDevice.OptionDescriptors(Index)
            If od.name.ToUpper.Trim = OptionName.ToUpper.Trim Then
                Return SANE.CurrentDevice.OptionValues(Index)
            End If
        Next
        Return Nothing
    End Function

    Public Function SetSANEOption(ByVal OptionName As String, ByVal Values() As Object) As Boolean
        Return (SetOpt(OptionName, Values) = SANE_API.SANE_Status.SANE_STATUS_GOOD)
    End Function

    Private Function SetOpt(ByVal OptionName As String, ByVal Values() As Object) As SANE_API.SANE_Status
        For Index As Integer = 0 To SANE.CurrentDevice.OptionDescriptors.Length - 1
            Dim od As SANE_API.SANE_Option_Descriptor = SANE.CurrentDevice.OptionDescriptors(Index)
            If od.name.ToUpper.Trim = OptionName.ToUpper.Trim Then
                Return SetOpt(Index, Values)
            End If
        Next
        Return SANE_API.SANE_Status.SANE_STATUS_INVAL
    End Function
    Private Function SetOpt(ByVal OptionIndex As Integer, ByVal Values() As Object) As SANE_API.SANE_Status
        Try
            Dim od As SANE_API.SANE_Option_Descriptor = SANE.CurrentDevice.OptionDescriptors(OptionIndex)
            Dim OptReq As New SANE_API.SANENetControlOption_Request
            Dim OptReply As New SANE_API.SANENetControlOption_Reply

            OptReq.handle = SANE.CurrentDevice.Handle
            OptReq.option_ = OptionIndex
            OptReq.action = SANE_API.SANE_Action.SANE_ACTION_SET_VALUE
            OptReq.value_type = od.type
            OptReq.value_size = od.size

            ReDim OptReq.values(Values.Length - 1)
            For i As Integer = 0 To Values.Length - 1

                Logger.Write(DebugLogger.Level.Debug, False, "Value Type = '" & Values(i).GetType.ToString & "', Value = '" & Values(i).ToString & "'")
                If Values(i).GetType Is GetType(TWAIN_VB.TW_FIX32) Then Values(i) = Me.TWAINInstance.FIX32ToFloat(Values(i))

                Select Case od.type
                    Case SANE_API.SANE_Value_Type.SANE_TYPE_STRING
                        OptReq.values(i) = Values(i).ToString
                    Case SANE_API.SANE_Value_Type.SANE_TYPE_BOOL
                        Dim bool As Boolean
                        If Boolean.TryParse(Values(i), bool) Then
                            OptReq.values(i) = bool
                        Else
                            Try
                                bool = Convert.ToBoolean(Convert.ToInt32(Values(i).ToString))
                            Catch ex As Exception
                                Throw New Exception("Unable to interpret '" & Values(i).ToString & "' as a boolean")
                            End Try
                            OptReq.values(i) = bool
                        End If
                    Case SANE_API.SANE_Value_Type.SANE_TYPE_INT
                        Dim int As Int32
                        If Int32.TryParse(Values(i).ToString, int) Then
                            OptReq.values(i) = int
                        Else
                            Dim d As Double
                            If Single.TryParse(Values(i).ToString, d) Then
                                OptReq.values(i) = CInt(d)
                            Else
                                Throw New Exception("Unable to interpret '" & Values(i).ToString & "' as an integer")
                            End If
                        End If
                    Case SANE_API.SANE_Value_Type.SANE_TYPE_FIXED
                        If Values(i).GetType Is GetType(TWAIN_VB.TW_FIX32) Then Values(i) = Me.TWAINInstance.FIX32ToFloat(Values(i))

                        Dim dbl As Double
                        If Double.TryParse(Values(i).ToString, dbl) Then
                            OptReq.values(i) = dbl
                        Else
                            Throw New Exception("Unable to interpret '" & Values(i).ToString & "' as a single precision number")
                        End If
                End Select
            Next
            Dim Status As SANE_API.SANE_Status = SANE.Net_Control_Option(net, OptReq, OptReply)
            If Status = SANE_API.SANE_Status.SANE_STATUS_GOOD Then

                Array.Copy(OptReply.values, SANE.CurrentDevice.OptionValues(OptionIndex), OptReply.values.Length)

                If OptReply.info And SANE_API.SANE_INFO_RELOAD_OPTIONS Then
                    GetOpts(False)
                Else
                    'Me.DisplayOption(Me.PanelOpt, OptionIndex)
                    If Me.TreeViewOptions.SelectedNode IsNot Nothing Then
                        If Me.TreeViewOptions.SelectedNode.Tag IsNot Nothing Then
                            Me.DisplayOption(Me.PanelOpt, Me.TreeViewOptions.SelectedNode.Tag)
                        End If
                    Else
                        Me.PanelOpt.Controls.Clear()
                    End If
                End If

                'If CurrentSettings.TWAIN.Enabled Then
                If Me.TWAIN_Is_Active Then
                    'XXX
                    'translate well-Known options to TWAIN capabilities
                    'translate custom-mapped options to TWAIN capabilities
                    SetTWAINCaps(od, Values)
                End If

            Else
                MsgBox("Error setting '" & od.type.ToString & "' option '" & od.title & "': " & Status.ToString)
            End If

            Me.PanelOptIsDirty = False

            Return Status
        Catch ex As ApplicationException
            Logger.Write(DebugLogger.Level.Warn, False, ex.Message)
            Throw
        Catch ex As Exception
            Logger.Write(DebugLogger.Level.Error_, False, ex.Message)
            Throw
        End Try

    End Function
    Private Sub SetOption()
        Dim Values(Me.OptionValueControls.Length - 1) As Object
        For ctl_i As Integer = 0 To Me.OptionValueControls.Length - 1
            Dim ctl As Control = Me.OptionValueControls(ctl_i)
            Select Case ctl.GetType
                Case GetType(TextBox), GetType(ComboBox), GetType(NumericUpDown)
                    Values(ctl_i) = ctl.Text
                Case GetType(CheckBox)
                    Dim chkbox As CheckBox
                    chkbox = DirectCast(ctl, CheckBox)
                    Values(ctl_i) = chkbox.Checked
            End Select
        Next

        Try
            Dim Status As SANE_API.SANE_Status = SetOpt(Me.TreeViewOptions.SelectedNode.Name, Values)
            If Status = SANE_API.SANE_Status.SANE_STATUS_GOOD Then
                If Not Me.TWAIN_Is_Active Then
                    If Not CurrentSettings.ScanContinuouslyUserConfigured Then
                        CurrentSettings.ScanContinuously = Device_Appears_To_Have_ADF() AndAlso Device_Appears_To_Have_ADF_Enabled()
                        Me.CheckBoxBatchMode.Checked = CurrentSettings.ScanContinuously
                    End If
                End If
            End If
        Catch ex As Exception
            Logger.Write(DebugLogger.Level.Error_, True, "Unable to set option value: " & ex.Message)
        End Try
    End Sub

    Private Sub FormMain_Resize(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Resize
        'Me.PanelOpt.Width = Me.ClientRectangle.Width - PanelOpt.Left
        If Me.TreeViewOptions.SelectedNode IsNot Nothing Then
            If Me.TreeViewOptions.SelectedNode.Tag IsNot Nothing Then
                Me.DisplayOption(Me.PanelOpt, Me.TreeViewOptions.SelectedNode.Tag)
            End If
        Else
            Me.PanelOpt.Controls.Clear()
        End If

    End Sub

    Private Sub TreeViewOptions_AfterSelect(ByVal sender As Object, ByVal e As System.Windows.Forms.TreeViewEventArgs) Handles TreeViewOptions.AfterSelect
        PanelOpt.Controls.Clear()
        For Index As Integer = 0 To SANE.CurrentDevice.OptionDescriptors.Length - 1
            If Index = e.Node.Tag Then
                Me.DisplayOption(Me.PanelOpt, Index)
                Exit For
            End If
        Next
    End Sub

    Private Sub FormMain_Shown(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Shown
        Update_Host_GUI()
    End Sub

    Private Sub Update_Host_GUI()
        Me.TextBoxHost.Text = CurrentSettings.SANE.CurrentHost.NameOrAddress
        Me.TextBoxPort.Text = CurrentSettings.SANE.CurrentHost.Port
        Me.TextBoxDevice.Text = CurrentSettings.SANE.CurrentDevice

    End Sub

    Private Sub ButtonCancel_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ButtonCancel.Click
        If Not TWAIN_Is_Active Then 'TWAIN_VB registers its own event handler
            Me.Close()
        End If
    End Sub

    Private Sub CheckBoxBatchMode_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles CheckBoxBatchMode.CheckedChanged
        CurrentSettings.ScanContinuously = CheckBoxBatchMode.Checked
    End Sub

    Private Sub ButtonHost_Click(sender As Object, e As EventArgs) Handles ButtonHost.Click
        Close_SANE()
        Close_Net()
        If SANE Is Nothing Then SANE = New SANE_API
        Dim f As New FormSANEHostWizard
        If f.ShowDialog <> Windows.Forms.DialogResult.OK Then
            Logger.Write(DebugLogger.Level.Debug, False, "User cancelled SANE host wizard")
            'XXX
        End If
        Try_Init_SANE()
        Update_Host_GUI()
    End Sub

    Private Sub CheckBoxBatchMode_Click(sender As Object, e As EventArgs) Handles CheckBoxBatchMode.Click
        CurrentSettings.ScanContinuouslyUserConfigured = True

    End Sub

End Class

