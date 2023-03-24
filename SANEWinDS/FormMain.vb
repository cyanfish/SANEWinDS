'
'   Copyright 2011-2021 Alec Skelly
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

    Private Shared Logger As NLog.Logger = NLog.LogManager.GetCurrentClassLogger()

    Public Event BatchStarted()
    Public Event ImageAcquired(ByVal PageNumber As Integer, ByVal Bmp As Bitmap)
    Public Event ImageError(ByVal PageNumber As Integer, ByVal Message As String)
    Public Event BatchCompleted(ByVal Pages As Integer)
    Public Event ImageProgress(ByVal PercentComplete As Integer) '0-100, -1 if unknown

    Friend Enum UIMode As Integer
        Scan = 0
        Configure = 1
    End Enum
    'Setting DialogResult closes the form, so we need our own result code
    Public Enum UIResult As Integer
        None = DialogResult.None
        OK = DialogResult.OK
        Cancel = DialogResult.Cancel
        CancelDuringScan = DialogResult.Abort
        ErrorDuringScan = DialogResult.Abort And &H80
    End Enum

    Public Result As UIResult = UIResult.None
    Public ShowScanProgress As Boolean = True

    Friend Got_MSG_CLOSEDS As Boolean = False
    Friend TWAIN_Is_Active As Boolean = False
    Friend TWAINInstance As TWAIN_VB.DS_Entry_Pump
    Friend Mode As UIMode = UIMode.Scan

    Private OptionValueControls(-1) As Control
    Private frmProgress As FormScanProgress
    Private PanelOptIsDirty As Boolean
    Private LastOptionIndexDisplayed As Integer = -1 'The index of the SANE option that was last displayed.
    Private LastOptionControlName As String = Nothing 'The name of the SANE option value control that last fired the Leave() event.
    Private Initialized As Boolean 'Has the Load() event already been executed? Workaround for Load() always firing when using ShowDialog().
    Private ImageCurve_KeyPoints As New System.Collections.Generic.Dictionary(Of String, System.Collections.Generic.List(Of System.Drawing.Point)) 'key is SANE option name

    Public Function GetCurrentDevicePageSizes() As List(Of PageSize)
        Dim Result As New List(Of PageSize)
        For Each ps As PageSize In CurrentSettings.PageSizes
            Result.Add(ps)
        Next
        Return Result
    End Function

    Private Sub CloseCurrentHost()
        Logger.Debug("")
        CloseCurrentDevice()
        If ControlClient IsNot Nothing Then
            Try
                SANE.Net_Exit(ControlClient)
            Catch ex As Exception
                Logger.Debug(ex, ex.Message)
            End Try
            Try
                ControlClient.Close()
            Catch ex As Exception
                Logger.Debug(ex, ex.Message)
            End Try
        End If
        ControlClient = Nothing
        'CurrentSettings.SANE.CurrentHostIndex = -1 'This jacks up TWAIN
    End Sub

    Private Function BytesToString(ByVal buffer As Byte(), ByVal offset As Integer, ByVal length As Integer) As String
        Dim s As String = Nothing
        For i = offset To offset + length - 1
            s += Chr(buffer(i))
        Next
        Return s
    End Function

    Private Sub CloseCurrentDevice()
        Logger.Debug("")
        Try
            If SANE IsNot Nothing Then
                If SANE.CurrentDevice.Name IsNot Nothing Then
                    If SANE.CurrentDevice.Open Then
                        If CheckBoxSaveOnExit.Checked Then SaveUserSettings()
                        SANE.Net_Close(ControlClient, SANE.CurrentDevice.Handle)
                    End If
                End If
            End If
        Catch ex As Exception
            Logger.Error(ex, ex.Message)
        Finally
            SANE.CurrentDevice.Open = False
        End Try
    End Sub

    Public Sub CancelScan()
        Me.Result = UIResult.CancelDuringScan
        If SANE IsNot Nothing Then
            If SANE.CurrentDevice.Open Then
                Try
                    SANE.Net_Cancel(ControlClient, SANE.CurrentDevice.Handle)
                Catch ex As Exception
                    Debug.Print(ex.Message)
                End Try
            End If
        End If
    End Sub

    Private Sub ButtonOK_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ButtonOK.Click
        Logger.Debug("")
        Me.Result = UIResult.OK
        If Not TWAIN_Is_Active Then 'TWAIN_VB registers its own event handler
            Dim PageNo As Integer = 0
            If SANE.CurrentDevice.Name IsNot Nothing Then
                If SANE.CurrentDevice.Open Then
                    If frmProgress Is Nothing OrElse frmProgress.IsDisposed Then
                        frmProgress = New FormScanProgress
                        frmProgress.Text = Me.Text
                        frmProgress.Icon = Me.Icon
                        AddHandler frmProgress.ScanCancelled, AddressOf CancelScan
                    Else
                        frmProgress.Reset()
                    End If
                    If Me.ShowScanProgress Then frmProgress.ShowProgress("Acquiring page 1...")
                    RaiseEvent BatchStarted()
                    Dim Status As SANE_API.SANE_Status = 0
                    Do
                        PageNo += 1
                        Dim bmp As Bitmap = Nothing
                        Try
                            Me.Cursor = Cursors.WaitCursor
                            Status = AcquireImage(bmp)
                            Select Case Status
                                Case SANE_API.SANE_Status.SANE_STATUS_GOOD
                                    If bmp IsNot Nothing Then
                                        If Me.ShowScanProgress Then
                                            frmProgress.ShowProgress("Acquiring page " & (PageNo + 1).ToString & "...")
                                        End If
                                        If Me.ComboBoxPageSize.SelectedItem IsNot Nothing Then
                                            For Each ps As PageSize In CurrentSettings.PageSizes
                                                If ps.Name = Me.ComboBoxPageSize.SelectedItem Then
                                                    bmp.Tag = ps 'Tag is used here to maintain the existing ImageAcquired event signature for backward compatibility.
                                                    Exit For
                                                End If
                                            Next
                                        End If
                                        RaiseEvent ImageAcquired(PageNo, bmp)
                                        'bmp.Dispose() 'let the event consumer decide whether to dispose or not.
                                        bmp = Nothing
                                        If Not CurrentSettings.ScanContinuously Then
                                            frmProgress.Visible = False
                                            RaiseEvent BatchCompleted(PageNo)
                                            Exit Do
                                        End If
                                    End If
                                Case SANE_API.SANE_Status.SANE_STATUS_NO_DOCS, SANE_API.SANE_Status.SANE_STATUS_CANCELLED
                                    frmProgress.Visible = False
                                    RaiseEvent BatchCompleted(PageNo - 1)
                                Case Else
                                    frmProgress.Visible = False
                                    Me.Result = UIResult.ErrorDuringScan
                                    RaiseEvent ImageError(PageNo, Status.ToString)
                            End Select
                        Catch ex As Exception
                            frmProgress.Visible = False
                            Me.Result = UIResult.ErrorDuringScan
                            RaiseEvent ImageError(PageNo, ex.Message)
                            Exit Do
                        Finally
                            'If frmProgress IsNot Nothing AndAlso (Not frmProgress.IsDisposed) Then frmProgress.Close()
                            Me.Cursor = Cursors.Default
                        End Try
                    Loop While Status = SANE_API.SANE_Status.SANE_STATUS_GOOD And CurrentSettings.ScanContinuously = True
                    Try
                        'Some backends expect Net_Cancel() after every batch or they stay in SANE_STATUS_BUSY.
                        'genesys and gt68xx are examples.
                        SANE.Net_Cancel(ControlClient, SANE.CurrentDevice.Handle)
                    Catch ex As Exception
                        Logger.Error(ex)
                    End Try
                    'If Net_Cancel() was called during the image transfer, the server may have disconnected the control connection (depends on backend).
                    'We'll need to reconnect and restore all settings.
                    If Not Client_Is_Connected(ControlClient) Then
                        If Not Try_Reconnect() Then
                            Logger.Warn("Try_Reconnect failed.")
                            CloseCurrentHost()
                        End If
                    End If
                Else
                    If frmProgress IsNot Nothing AndAlso (Not frmProgress.IsDisposed) Then frmProgress.Visible = False
                    Me.Result = UIResult.ErrorDuringScan
                    RaiseEvent ImageError(PageNo, "The SANE device '" & SANE.CurrentDevice.Name & "' has not been opened")
                End If
            Else
                If frmProgress IsNot Nothing AndAlso (Not frmProgress.IsDisposed) Then frmProgress.Visible = False
                Me.Result = UIResult.ErrorDuringScan
                RaiseEvent ImageError(PageNo, "The SANE device name is not defined")
            End If
            WinAPI.SetProcessWorkingSetSize(Process.GetCurrentProcess().Handle, -1, -1) 'reclaim memory
        End If
    End Sub

    Private Function Try_Reconnect() As Boolean
        Try
            ControlClient.Close()
            ControlClient = Nothing
            'store current device info
            Dim DeviceName As String = SANE.CurrentDevice.Name
            Dim OptVals() As Object = CloneOptionValueSet(SANE.CurrentDevice.OptionValueSets("Current"))
            Try_Init_SANE(False)
            Update_Host_GUI()

            If SANE.CurrentDevice.Name = DeviceName Then 'we reconnected to the same device
                If SANE.CurrentDevice.Open Then
                    For i As Integer = 1 To SANE.CurrentDevice.OptionDescriptors.Count - 1 'skip the first option, which is just the option count
                        Select Case SANE.CurrentDevice.OptionDescriptors(i).type
                            Case SANE_API.SANE_Value_Type.SANE_TYPE_GROUP, SANE_API.SANE_Value_Type.SANE_TYPE_BUTTON
                                'no need to map these options
                            Case Else
                                If OptVals(i) IsNot Nothing Then
                                    If SANE.SANE_OPTION_IS_ACTIVE(SANE.CurrentDevice.OptionDescriptors(i).cap) And SANE.SANE_OPTION_IS_SETTABLE(SANE.CurrentDevice.OptionDescriptors(i).cap) Then
                                        SetOpt(i, OptVals(i)) 'sets value for both SANE and TWAIN
                                    Else
                                        Logger.Warn("Option '{0}' is not currently settable", SANE.CurrentDevice.OptionDescriptors(i).title)
                                    End If
                                End If
                        End Select
                    Next
                    If Me.TreeViewOptions.SelectedNode IsNot Nothing Then
                        If Me.TreeViewOptions.SelectedNode.Tag IsNot Nothing Then
                            DisplayOption(PanelOpt, TreeViewOptions.SelectedNode.Tag)
                        End If
                    Else
                        ClearPanelControls()
                    End If
                End If
            End If
            Return SANE.CurrentDevice.Open
        Catch ex As Exception
            Logger.Error(ex)
            Return SANE.CurrentDevice.Open
        End Try
    End Function

    Friend Sub GetOpts(ByVal Recreate As Boolean)
        Try

            Me.ImageCurve_KeyPoints.Clear()

            If Recreate Then
                Me.TreeViewOptions.Nodes.Clear()
            End If

            Dim Descriptors() As SANE_API.SANE_Option_Descriptor
            Descriptors = SANE.Net_Get_Option_Descriptors(ControlClient, SANE.CurrentDevice.Handle)
            SANE.CurrentDevice.OptionDescriptors = Descriptors

            With SANE.CurrentDevice.OptionValueSets
                If Not .ContainsKey("Current") Then .Add("Current", New Object(Descriptors.Length - 1) {})
            End With

            Dim BackEndConfigFile As String = CurrentSettings.GetDeviceConfigFileName(SharedSettings.ConfigFileScope.User) 'this will create the config file if it doesn't already exist

            Dim GroupNode As TreeNode = Nothing
            Dim AdvancedNode As TreeNode = Nothing
            For i As Integer = 1 To Descriptors.Length - 1 'skip the first element, which contains the array length
                ReDim SANE.CurrentDevice.OptionValueSets("Current")(i)(SANE.OptionValueArrayLength(Descriptors(i).size, Descriptors(i).type) - 1) 'make sure it's not Nothing, even for unreadable options

                'Ignore the avision backend's 'nvram-values' option, which takes forever to read and isn't settable.
                If Descriptors(i).name = "nvram-values" Then Continue For

                'Ignore any option missing both name and title (hpaio backend has one).
                If Descriptors(i).name Is Nothing AndAlso Descriptors(i).title Is Nothing Then Continue For

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

                                If AdvancedNode Is Nothing Then
                                    AdvancedNode = New TreeNode
                                    AdvancedNode.Tag = -999
                                    AdvancedNode.Text = "Advanced"
                                End If

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
                                Do
                                    Dim Status As SANE_API.SANE_Status = SANE.Net_Control_Option(ControlClient, OptReq, OptReply,
                                                                         CurrentSettings.SANE.Hosts(CurrentSettings.SANE.CurrentHostIndex).Username,
                                                                         CurrentSettings.SANE.Hosts(CurrentSettings.SANE.CurrentHostIndex).Password)
                                    If Status = SANE_API.SANE_Status.SANE_STATUS_GOOD Then
                                        ReDim SANE.CurrentDevice.OptionValueSets("Current")(i)(OptReply.values.Length - 1)
                                        Array.Copy(OptReply.values, SANE.CurrentDevice.OptionValueSets("Current")(i), OptReply.values.Length)
                                        Exit Do
                                    ElseIf Status = SANE_API.SANE_Status.SANE_STATUS_ACCESS_DENIED Then
                                        Dim PwdBox As New FormSANEAuth
                                        PwdBox.UsernameTextBox.Text = CurrentSettings.SANE.Hosts(CurrentSettings.SANE.CurrentHostIndex).Username
                                        If PwdBox.ShowDialog = Windows.Forms.DialogResult.Cancel Then Exit Do
                                        CurrentSettings.SANE.Hosts(CurrentSettings.SANE.CurrentHostIndex).Username = PwdBox.UsernameTextBox.Text
                                        CurrentSettings.SANE.Hosts(CurrentSettings.SANE.CurrentHostIndex).Password = PwdBox.PasswordTextBox.Text
                                    Else
                                        Dim msg As String = "Error reading '" & Descriptors(i).type.ToString & "' option '" & Descriptors(i).title & "': " & Status.ToString
                                        Logger.Warn(msg)
                                        MsgBox(msg)
                                        Exit Do
                                    End If
                                Loop
                            End If
                        End If
                        'MsgBox("Name: " & Devices(i).name & vbCr & "Vendor: " & Devices(i).vendor & vbCr & "Model: " & Devices(i).model & vbCr & "Type: " & Devices(i).type)
                End Select
            Next

            With SANE.CurrentDevice.OptionValueSets
                If Not .ContainsKey("Backend Defaults") Then
                    .Add("Backend Defaults", CloneOptionValueSet(.Item("Current")))
                End If
            End With

            If Recreate Then
                If Me.TreeViewOptions.Nodes.Count > 0 Then Me.TreeViewOptions.SelectedNode = Me.TreeViewOptions.TopNode
            End If
            If Not Me.TWAIN_Is_Active Then
                If Not CurrentSettings.ScanContinuouslyUserConfigured Then
                    CurrentSettings.ScanContinuously = Device_Appears_To_Have_ADF() AndAlso Device_Appears_To_Have_ADF_Enabled()
                    Me.CheckBoxBatchMode.Checked = CurrentSettings.ScanContinuously
                End If
            End If
        Catch ex As Exception
            Logger.Error(ex, ex.Message)
        End Try

    End Sub

    Private Sub FormMain_FormClosed(sender As Object, e As FormClosedEventArgs) Handles Me.FormClosed
        Logger.Trace("")
        Me.CloseCurrentHost()
        If ControlClient IsNot Nothing Then
            Try
                ControlClient.Close()
            Catch ex As Exception
                Logger.Debug(ex, ex.Message)
            End Try
            ControlClient = Nothing
        End If
        If CurrentSettings IsNot Nothing Then CurrentSettings.Save()
    End Sub

    Private Sub Form1_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not Me.Initialized Then
            Me.Initialized = True
            'Me.Text = GetType(SANE_API).Assembly.GetName.Name & " " & GetType(SANE_API).Assembly.GetName.Version.ToString()
            With GetType(SANE_API).Assembly.GetName
                '"Assembly Version" from project properties
                Me.Text = .Name _
                    & " " & .Version.Major.ToString _
                    & "." & .Version.Minor.ToString _
                    & " (" & .Version.Build.ToString & ")"
            End With

            With GetType(SANE_API).Assembly.GetName
                '"Assembly Version" from project properties
                Me.Text = .Name _
                    & " " & .Version.Major.ToString _
                    & "." & .Version.Minor.ToString _
                    & " (" & .Version.Build.ToString & ")"
            End With

            With GetType(SANE_API).Assembly.GetName
                '"Assembly Version" from project properties
                Me.Text = .Name _
                    & " " & .Version.Major.ToString _
                    & "." & .Version.Minor.ToString _
                    & " (" & .Version.Build.ToString & ")"
            End With

            With GetType(SANE_API).Assembly.GetName
                '"Assembly Version" from project properties
                Me.Text = .Name _
                    & " " & .Version.Major.ToString _
                    & "." & .Version.Minor.ToString _
                    & " (" & .Version.Build.ToString & ")"
            End With

            With GetType(SANE_API).Assembly.GetName
                '"Assembly Version" from project properties
                Me.Text = .Name _
                    & " " & .Version.Major.ToString _
                    & "." & .Version.Minor.ToString _
                    & " (" & .Version.Build.ToString & ")"
            End With

            With GetType(SANE_API).Assembly.GetName
                '"Assembly Version" from project properties
                Me.Text = .Name _
                    & " " & .Version.Major.ToString _
                    & "." & .Version.Minor.ToString _
                    & " (" & .Version.Build.ToString & ")"
            End With

            With GetType(SANE_API).Assembly.GetName
                '"Assembly Version" from project properties
                Me.Text = .Name _
                    & " " & .Version.Major.ToString _
                    & "." & .Version.Minor.ToString _
                    & " (" & .Version.Build.ToString & ")"
            End With

            Me.MinimumSize = Me.Size

            Me.TreeViewOptions.HideSelection = False

            Dim UseRoamingAppData As Boolean = False
            Try
                UseRoamingAppData = My.Settings.UseRoamingAppData
            Catch ex As Exception
            End Try

            Logger.Debug("")

            Me.ButtonOK.Enabled = False
            'Me.ButtonHost.Enabled = Not TWAIN_Is_Active 'XXX is it ok to reconfigure with TWAIN active?

            If SANE Is Nothing Then SANE = New SANE_API

            AddHandler SANE.FrameProgress, AddressOf ImageFrameProgress

            If Not TWAIN_Is_Active Then
                Me.Mode = UIMode.Scan
                If Not ((CurrentSettings.SANE.CurrentHostIndex > -1) _
                        AndAlso (CurrentSettings.SANE.CurrentHostIndex < CurrentSettings.SANE.Hosts.Length) _
                        AndAlso CurrentSettings.ResolveHost(CurrentSettings.SANE.Hosts(CurrentSettings.SANE.CurrentHostIndex)) _
                        AndAlso CurrentSettings.HostIsValid(CurrentSettings.SANE.Hosts(CurrentSettings.SANE.CurrentHostIndex)) _
                        AndAlso CurrentSettings.DeviceIsValid(CurrentSettings.SANE.Hosts(CurrentSettings.SANE.CurrentHostIndex))) Then
                    Dim f As New FormSANEHostWizard
                    If f.ShowDialog <> Windows.Forms.DialogResult.OK Then
                        Logger.Debug("User cancelled SANE host wizard")
                    Else
                        Try_Init_SANE(False)
                    End If
                Else
                    Try_Init_SANE(True)
                    If Not SANE.CurrentDevice.Open Then
                        Dim f As New FormSANEHostWizard
                        If f.ShowDialog <> Windows.Forms.DialogResult.OK Then
                            Logger.Debug("User cancelled SANE host wizard")
                        Else
                            Try_Init_SANE(False)
                        End If
                    End If
                End If
            End If
            Update_Host_GUI()
            If Me.Mode = UIMode.Scan Then Me.ButtonOK.Text = "Scan" Else Me.ButtonOK.Text = "OK"
            Me.Initialized = True
        End If
    End Sub

    Private Sub Try_Init_SANE(ByVal SuppressCommonErrorMessages As Boolean)
        Try
            If ControlClient IsNot Nothing Then
                Close_SANE()
                Close_Net()
            End If
            ControlClient = New System.Net.Sockets.TcpClient
            If ControlClient IsNot Nothing Then
                If (CurrentSettings.SANE.CurrentHostIndex > -1) And (CurrentSettings.SANE.CurrentHostIndex < CurrentSettings.SANE.Hosts.Count) Then
                    CurrentSettings.SANE.Hosts(CurrentSettings.SANE.CurrentHostIndex).Open = False
                    If CurrentSettings.ResolveHost(CurrentSettings.SANE.Hosts(CurrentSettings.SANE.CurrentHostIndex)) Then
                        If CurrentSettings.HostIsValid(CurrentSettings.SANE.Hosts(CurrentSettings.SANE.CurrentHostIndex)) Then
                            ControlClient.ReceiveTimeout = CurrentSettings.SANE.Hosts(CurrentSettings.SANE.CurrentHostIndex).TCP_Timeout_ms
                            ControlClient.SendTimeout = CurrentSettings.SANE.Hosts(CurrentSettings.SANE.CurrentHostIndex).TCP_Timeout_ms
                            Logger.Debug("TCPClient Send buffer length is {0}", ControlClient.SendBufferSize)
                            Logger.Debug("TCPClient Receive buffer length is {0}", ControlClient.ReceiveBufferSize)
                            ControlClient.Connect(CurrentSettings.SANE.Hosts(CurrentSettings.SANE.CurrentHostIndex).NameOrAddress, CurrentSettings.SANE.Hosts(CurrentSettings.SANE.CurrentHostIndex).Port)
                            Dim Status As SANE_API.SANE_Status = SANE.Net_Init(ControlClient, IIf(String.IsNullOrEmpty(Environment.UserName), Me.ProductName, Environment.UserName))
                            Logger.Debug("Net_Init returned status {0}'", Status)
                            If Status = SANE_API.SANE_Status.SANE_STATUS_GOOD Then CurrentSettings.SANE.Hosts(CurrentSettings.SANE.CurrentHostIndex).Open = True
                            If CurrentSettings.SANE.Hosts(CurrentSettings.SANE.CurrentHostIndex).Open Then

                                Do
                                    SANE.CurrentDevice = New SANE_API.CurrentDeviceInfo

                                    Dim DeviceHandle As Integer
                                    Status = SANE.Net_Open(ControlClient, CurrentSettings.SANE.Hosts(CurrentSettings.SANE.CurrentHostIndex).Device, DeviceHandle,
                                                           CurrentSettings.SANE.Hosts(CurrentSettings.SANE.CurrentHostIndex).Username,
                                                           CurrentSettings.SANE.Hosts(CurrentSettings.SANE.CurrentHostIndex).Password)
                                    Logger.Debug("Net_Open returned status '{0}'", Status)
                                    If (Status <> SANE_API.SANE_Status.SANE_STATUS_GOOD) And (Status <> SANE_API.SANE_Status.SANE_STATUS_ACCESS_DENIED) Then  'Auto-Locate
                                        If Not String.IsNullOrWhiteSpace(CurrentSettings.SANE.Hosts(CurrentSettings.SANE.CurrentHostIndex).AutoLocateDevice) Then
                                            Logger.Debug("Attempting to auto-locate devices matching '{0}'", CurrentSettings.SANE.Hosts(CurrentSettings.SANE.CurrentHostIndex).AutoLocateDevice)
                                            Dim Devices(-1) As SANE_API.SANE_Device
                                            Status = SANE.Net_Get_Devices(ControlClient, Devices)
                                            If Status = SANE_API.SANE_Status.SANE_STATUS_GOOD Then
                                                Status = SANE_API.SANE_Status.SANE_STATUS_INVAL
                                                Dim AutoLocateDeviceStrings() As String = CurrentSettings.SANE.Hosts(CurrentSettings.SANE.CurrentHostIndex).AutoLocateDevice.Split(New Char() {",", ";"})
                                                Dim FoundDevice As Boolean = False
                                                Dim FoundDeviceName As String = Nothing
                                                For Each DevName As String In AutoLocateDeviceStrings
                                                    If DevName IsNot Nothing Then DevName = DevName.Trim
                                                    If Not String.IsNullOrWhiteSpace(DevName) Then
                                                        For i As Integer = 0 To Devices.Length - 1
                                                            If DevName = "*" Then
                                                                FoundDevice = True
                                                                FoundDeviceName = Devices(i).name
                                                                Exit For
                                                            Else
                                                                If Devices(i).name.Trim.Length >= DevName.Length Then
                                                                    If Devices(i).name.Trim.Substring(0, DevName.Length) = DevName Then
                                                                        FoundDevice = True
                                                                        FoundDeviceName = Devices(i).name
                                                                        Exit For
                                                                    End If
                                                                End If
                                                            End If
                                                        Next
                                                        If FoundDevice Then Exit For
                                                    End If
                                                Next
                                                If FoundDevice Then
                                                    Logger.Debug("Auto-located device '{0}'; attempting to open...", FoundDeviceName)
                                                    Status = SANE.Net_Open(ControlClient, FoundDeviceName, DeviceHandle,
                                                                          CurrentSettings.SANE.Hosts(CurrentSettings.SANE.CurrentHostIndex).Username,
                                                                          CurrentSettings.SANE.Hosts(CurrentSettings.SANE.CurrentHostIndex).Password)
                                                    Logger.Debug("Net_Open returned status '{0}'", Status)
                                                    If Status = SANE_API.SANE_Status.SANE_STATUS_GOOD Then CurrentSettings.SANE.Hosts(CurrentSettings.SANE.CurrentHostIndex).Device = FoundDeviceName
                                                Else
                                                    Dim s As String = Nothing
                                                    For Each DevName As String In AutoLocateDeviceStrings
                                                        If Not String.IsNullOrWhiteSpace(DevName) Then
                                                            s += DevName.Trim & vbCr
                                                        End If
                                                    Next
                                                    MsgBox("No device using any of the following Autolocate backends was found on host '" & CurrentSettings.SANE.Hosts(CurrentSettings.SANE.CurrentHostIndex).NameOrAddress & "': " & vbCr & s & vbCr & "Please check your connections or select a different scanner.", MsgBoxStyle.Exclamation, "Device not found")
                                                End If
                                            End If
                                        End If
                                    End If

                                    If Status = SANE_API.SANE_Status.SANE_STATUS_GOOD Then
                                        SANE.CurrentDevice.Name = CurrentSettings.SANE.Hosts(CurrentSettings.SANE.CurrentHostIndex).Device
                                        SANE.CurrentDevice.Handle = DeviceHandle
                                        SANE.CurrentDevice.Open = True
                                        Exit Do
                                    ElseIf Status = SANE_API.SANE_Status.SANE_STATUS_ACCESS_DENIED Then
                                        Dim PwdBox As New FormSANEAuth
                                        PwdBox.UsernameTextBox.Text = CurrentSettings.SANE.Hosts(CurrentSettings.SANE.CurrentHostIndex).Username
                                        If PwdBox.ShowDialog = Windows.Forms.DialogResult.Cancel Then Exit Do
                                        CurrentSettings.SANE.Hosts(CurrentSettings.SANE.CurrentHostIndex).Username = PwdBox.UsernameTextBox.Text
                                        CurrentSettings.SANE.Hosts(CurrentSettings.SANE.CurrentHostIndex).Password = PwdBox.PasswordTextBox.Text
                                    Else
                                        Dim msg As String = "The SANE backend reported " & Status.ToString & " during initialization"
                                        Logger.Warn(msg)
                                        If SuppressCommonErrorMessages Then
                                            Exit Do
                                        Else
                                            If MsgBox(msg, MsgBoxStyle.Exclamation + MsgBoxStyle.RetryCancel) = MsgBoxResult.Cancel Then Exit Do
                                        End If
                                    End If
                                Loop

                            End If
                        Else
                            Logger.Warn("The host entry is invalid")
                        End If
                    Else
                        Dim msg As String = "Unable to resolve host '" & CurrentSettings.SANE.Hosts(CurrentSettings.SANE.CurrentHostIndex).NameOrAddress & "'"
                        Logger.Warn(msg)
                        If Not SuppressCommonErrorMessages Then MsgBox(msg, MsgBoxStyle.Exclamation)
                    End If

                    If SANE.CurrentDevice.Open Then
                        SANE.CurrentDevice.OptionValueSets = New Dictionary(Of String, Object())(StringComparer.InvariantCultureIgnoreCase)

                        GetOpts(True)  'must occur prior to reading GetDeviceConfigFileName()!

                        Dim s As String = CurrentSettings.GetDeviceConfigFileName(SharedSettings.ConfigFileScope.Shared)
                        If s IsNot Nothing AndAlso s.Length > 0 Then CurrentSettings.SANE.Hosts(CurrentSettings.SANE.CurrentHostIndex).DeviceINI.Shared = s

                        s = CurrentSettings.GetDeviceConfigFileName(SharedSettings.ConfigFileScope.User)
                        If s IsNot Nothing AndAlso s.Length > 0 Then CurrentSettings.SANE.Hosts(CurrentSettings.SANE.CurrentHostIndex).DeviceINI.User = s

                        ApplyUserSettings() 'Apply merged settings from User and Shared INI files.

                        Me.ButtonOK.Enabled = True
                    Else
                        Logger.Warn("SANE device is not open")
                    End If
                Else
                    Logger.Warn("Current host index is out of range")
                End If
            End If
        Catch ex As Exception
            Logger.Error(ex, ex.Message)
            MsgBox(ex.Message, MsgBoxStyle.Critical)
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

        Debug.Print("OptionIndex=" & OptionIndex.ToString & ", Last=" & LastOptionIndexDisplayed.ToString)

        If OptionIndex > SANE.CurrentDevice.OptionDescriptors.Count - 1 Then
            Logger.Error("DisplayOption called with invalid OptionIndex:" & OptionIndex.ToString)
            Debug.Print("*** Invalid OptionIndex ***")
            Exit Sub
        End If

        Dim RecreateControls As Boolean = (LastOptionIndexDisplayed < 0) Or (LastOptionIndexDisplayed <> OptionIndex)
        LastOptionIndexDisplayed = OptionIndex

        Dim od As SANE_API.SANE_Option_Descriptor = SANE.CurrentDevice.OptionDescriptors(OptionIndex)
        'Dim tt As ToolTip = Me.ToolTip1

        Dim BorderHeight As Integer = 0
        Dim BorderWidth As Integer = 0
        Dim _tbox As New TextBox
        If RecreateControls Then
            If _tbox.BorderStyle = BorderStyle.Fixed3D Then
                BorderHeight = System.Windows.Forms.SystemInformation.Border3DSize.Height
                BorderWidth = System.Windows.Forms.SystemInformation.Border3DSize.Width
            Else
                BorderHeight = System.Windows.Forms.SystemInformation.BorderSize.Height
                BorderWidth = System.Windows.Forms.SystemInformation.BorderSize.Width
            End If

            ClearPanelControls()

            'WinAPI.SetProcessWorkingSetSize(Process.GetCurrentProcess().Handle, -1, -1) 'reclaim memory

            ReDim OptionValueControls(SANE.CurrentDevice.OptionValueSets("Current")(OptionIndex).Length - 1)
        End If

        Select Case od.type
            Case SANE_API.SANE_Value_Type.SANE_TYPE_BOOL
                Dim vOffs As Integer = 0
                For j = 0 To SANE.CurrentDevice.OptionValueSets("Current")(OptionIndex).Length - 1
                    Dim chk As New CheckBox
                    If RecreateControls Then

                        chk.Top = vOffs
                        vOffs += chk.Height + BorderHeight
                        chk.Text = SANE.UnitString(od.unit)
                        chk.Name = "ctl_" & od.name & "_" & j.ToString
                        chk.AutoSize = True

                        chk.Enabled = SANE.SANE_OPTION_IS_ACTIVE(od.cap) And SANE.SANE_OPTION_IS_SETTABLE(od.cap)
                    Else
                        chk = Me.OptionValueControls(j)
                    End If

                    If SANE.CurrentDevice.OptionValueSets("Current") IsNot Nothing Then
                        If SANE.CurrentDevice.OptionValueSets("Current")(OptionIndex) IsNot Nothing Then
                            If SANE.CurrentDevice.OptionValueSets("Current")(OptionIndex).length - 1 >= j Then
                                If SANE.CurrentDevice.OptionValueSets("Current")(OptionIndex)(j) IsNot Nothing Then
                                    chk.Checked = SANE.CurrentDevice.OptionValueSets("Current")(OptionIndex)(j)
                                End If
                            End If
                        End If
                    End If

                    If RecreateControls Then
                        Me.OptionValueControls(j) = chk
                        AddHandler chk.CheckedChanged, AddressOf OptionControl_TextChanged
                        AddHandler chk.Leave, AddressOf OptionControl_Leave
                        PanelOpt.Controls.Add(chk)
                    End If
                Next

                If RecreateControls Then
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
                End If

            Case SANE_API.SANE_Value_Type.SANE_TYPE_BUTTON
                Dim btn As New Button
                If RecreateControls Then
                    'btn.Tag = od.title
                    btn.Text = od.title
                    btn.Name = "ctl_" & od.name
                    btn.Enabled = SANE.SANE_OPTION_IS_ACTIVE(od.cap) And SANE.SANE_OPTION_IS_SETTABLE(od.cap)
                    AddHandler btn.Click, AddressOf SetOption
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
                End If

            Case SANE_API.SANE_Value_Type.SANE_TYPE_FIXED
                Dim vOffs As Integer = 0
                For j = 0 To SANE.CurrentDevice.OptionValueSets("Current")(OptionIndex).Length - 1
                    Dim ctl As Control = Nothing
                    If RecreateControls Then
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
                                ud.Increment = IIf(od.constraint.range.quant <> 0, SANE.SANE_UNFIX(od.constraint.range.quant), 1)
                                ud.DecimalPlaces = 4
                                ctl = ud
                            Case SANE_API.SANE_Constraint_Type.SANE_CONSTRAINT_WORD_LIST
                                Dim cb As New ComboBox
                                cb.Top = vOffs
                                vOffs += cb.Height + BorderHeight
                                For k As Integer = 0 To od.constraint.word_list.Length - 1
                                    cb.Items.Add(SANE.SANE_UNFIX(od.constraint.word_list(k)))
                                Next
                                cb.DropDownStyle = ComboBoxStyle.DropDownList
                                ctl = cb
                            Case Else
                                MsgBox("Unexpected constraint type '" & od.constraint_type.ToString & "' for value type '" & od.type.ToString & "'")
                        End Select
                        ctl.Name = "ctl_" & od.name & "_" & j.ToString
                        ctl.Enabled = SANE.SANE_OPTION_IS_ACTIVE(od.cap) And SANE.SANE_OPTION_IS_SETTABLE(od.cap)
                        Me.OptionValueControls(j) = ctl
                    Else
                        ctl = Me.OptionValueControls(j)
                    End If

                    Dim ulbl As New Label
                    If RecreateControls Then
                        ulbl.Top = ctl.Top
                        ulbl.Left = ctl.Right + BorderWidth
                        ulbl.Width = PanelOpt.Right - ctl.Right - (BorderWidth * 2)
                        ulbl.TextAlign = ContentAlignment.MiddleLeft
                        ulbl.Text = SANE.UnitString(od.unit)
                        ulbl.AutoSize = False
                        ulbl.Anchor = AnchorStyles.Left
                        ulbl.Enabled = ctl.Enabled
                    End If

                    If SANE.CurrentDevice.OptionValueSets("Current") IsNot Nothing Then
                        If SANE.CurrentDevice.OptionValueSets("Current")(OptionIndex) IsNot Nothing Then
                            If SANE.CurrentDevice.OptionValueSets("Current")(OptionIndex).length - 1 >= j Then
                                If SANE.CurrentDevice.OptionValueSets("Current")(OptionIndex)(j) IsNot Nothing Then
                                    If ctl.GetType = GetType(NumericUpDown) Then
                                        Dim ud As NumericUpDown = DirectCast(ctl, NumericUpDown)
                                        If (SANE.CurrentDevice.OptionValueSets("Current")(OptionIndex)(j) < ud.Minimum) Then
                                            Logger.Warn("Current option value '{0}' is below the minimum of '{1}' specified in the option constraint; changing value to constraint minimum.", SANE.CurrentDevice.OptionValueSets("Current")(OptionIndex)(j), ud.Minimum)
                                            SANE.CurrentDevice.OptionValueSets("Current")(OptionIndex)(j) = ud.Minimum
                                        End If
                                        If (SANE.CurrentDevice.OptionValueSets("Current")(OptionIndex)(j) > ud.Maximum) Then
                                            Logger.Warn("Current option value '{0}' is above the maximum of '{1}' specified in the option constraint; changing value to constraint maximum.", SANE.CurrentDevice.OptionValueSets("Current")(OptionIndex)(j), ud.Maximum)
                                            SANE.CurrentDevice.OptionValueSets("Current")(OptionIndex)(j) = ud.Maximum
                                        End If
                                        ud.Value = SANE.CurrentDevice.OptionValueSets("Current")(OptionIndex)(j)

                                        If RecreateControls Then
                                            ulbl.Text += " (" & ud.Minimum.ToString("0.####") & " to " & ud.Maximum.ToString("0.####")
                                            If od.constraint.range.quant > 0 Then
                                                ulbl.Text += " in steps of " & ud.Increment.ToString("0.####")
                                            End If
                                            ulbl.Text += ")"
                                        End If
                                    Else
                                        Dim d As Double = SANE.CurrentDevice.OptionValueSets("Current")(OptionIndex)(j)
                                        ctl.Text = d.ToString("0.####")
                                    End If
                                End If
                            End If
                        End If
                    End If

                    If RecreateControls Then
                        'MS bug: TextChanged event does not fire on a combobox when DropDownStyle is set to DropDownList.
                        If ctl.GetType = GetType(ComboBox) Then
                            Dim cb As ComboBox = DirectCast(ctl, ComboBox)
                            AddHandler cb.SelectedIndexChanged, AddressOf OptionControl_TextChanged
                        Else
                            AddHandler ctl.TextChanged, AddressOf OptionControl_TextChanged
                        End If
                        AddHandler ctl.Leave, AddressOf OptionControl_Leave
                        PanelOpt.Controls.Add(ctl)
                        PanelOpt.Controls.Add(ulbl)
                    End If

                Next

                Dim lbl As New Label

                If RecreateControls Then
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
                End If

            Case SANE_API.SANE_Value_Type.SANE_TYPE_GROUP

            Case SANE_API.SANE_Value_Type.SANE_TYPE_INT
                'look for a gamma-table option
                If od.name IsNot Nothing _
                  AndAlso od.name.ToLower.Contains("gamma-table") _
                  AndAlso od.constraint_type = SANE_API.SANE_Constraint_Type.SANE_CONSTRAINT_RANGE _
                  AndAlso SANE.CurrentDevice.OptionValueSets("Current")(OptionIndex).Length >= 255 Then

                    Dim ic As ImageCurve
                    If RecreateControls Then
                        ic = New ImageCurve(SANE.CurrentDevice.OptionValueSets("Current")(OptionIndex).Length, od.constraint.range.max)
                        ic.Top = 4
                        ic.Left = 50
                        ic.Width = 300
                        ic.Height = 200
                        PanelOpt.Controls.Add(ic)
                        ic.Name = "ctl_" & od.name
                        ic.Enabled = SANE.SANE_OPTION_IS_ACTIVE(od.cap) And SANE.SANE_OPTION_IS_SETTABLE(od.cap)
                        ReDim OptionValueControls(0)
                        Me.OptionValueControls(0) = ic
                    Else
                        ic = Me.OptionValueControls(0)
                    End If

                    'restore keypoints
                    If Me.ImageCurve_KeyPoints.ContainsKey(ic.Name) Then 'we saved the keypoints earlier.
                        ic.keyPt = Me.ImageCurve_KeyPoints(ic.Name).ToList
                    Else 'supply values for the control to calculate new keypoints.
                        Try
                            Dim values(SANE.CurrentDevice.OptionValueSets("Current")(OptionIndex).Length - 1) As Integer
                            Array.Copy(SANE.CurrentDevice.OptionValueSets("Current")(OptionIndex), values, SANE.CurrentDevice.OptionValueSets("Current")(OptionIndex).Length)
                            ic.LevelValue = values
                        Catch
                        End Try
                    End If

                    If RecreateControls Then
                        'save keypoints
                        AddHandler ic.ImageLevelChanged, AddressOf imageCurve_ImageLevelChanged
                        'save values
                        AddHandler ic.Leave, AddressOf OptionControl_Leave

                        Dim lbl As New Label
                        lbl.Top = ic.Bottom + 35
                        lbl.Height = PanelOpt.Bottom - ic.Top - 10
                        'lbl.Left = ic.Left
                        lbl.Enabled = ic.Enabled
                        lbl.Width = PanelOpt.Width
                        lbl.TextAlign = ContentAlignment.TopLeft
                        lbl.Text = od.desc
                        lbl.AutoSize = False

                        PanelOpt.Controls.Add(lbl)
                    End If

                Else
                Dim vOffs As Integer = 0
                    For j = 0 To SANE.CurrentDevice.OptionValueSets("Current")(OptionIndex).Length - 1

                    Dim ctl As Control = Nothing
                    If RecreateControls Then
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
                                ud.Increment = IIf(od.constraint.range.quant <> 0, od.constraint.range.quant, 1)
                                ud.DecimalPlaces = 0
                                ctl = ud
                            Case SANE_API.SANE_Constraint_Type.SANE_CONSTRAINT_WORD_LIST
                                Dim cb As New ComboBox
                                cb.Top = vOffs
                                vOffs += cb.Height + BorderHeight
                                For k As Integer = 0 To od.constraint.word_list.Length - 1
                                    cb.Items.Add(od.constraint.word_list(k))
                                Next
                                cb.DropDownStyle = ComboBoxStyle.DropDownList
                                ctl = cb
                            Case Else
                                MsgBox("Unexpected constraint type '" & od.constraint_type.ToString & "' for value type '" & od.type.ToString & "'")
                        End Select
                        ctl.Name = "ctl_" & od.name & "_" & j.ToString
                        ctl.Enabled = SANE.SANE_OPTION_IS_ACTIVE(od.cap) And SANE.SANE_OPTION_IS_SETTABLE(od.cap)
                        Me.OptionValueControls(j) = ctl
                    Else
                        ctl = Me.OptionValueControls(j)
                    End If

                    If SANE.CurrentDevice.OptionValueSets("Current") IsNot Nothing Then
                        If SANE.CurrentDevice.OptionValueSets("Current")(OptionIndex) IsNot Nothing Then
                            If SANE.CurrentDevice.OptionValueSets("Current")(OptionIndex).length - 1 >= j Then
                                If SANE.CurrentDevice.OptionValueSets("Current")(OptionIndex)(j) IsNot Nothing Then
                                    If ctl.GetType = GetType(NumericUpDown) Then
                                        Dim ud As NumericUpDown = DirectCast(ctl, NumericUpDown)
                                        If (SANE.CurrentDevice.OptionValueSets("Current")(OptionIndex)(j) < ud.Minimum) Then
                                            Logger.Warn("Current option value '{0}' is below the minimum of '{1}' specified in the option constraint; changing value to constraint minimum.", SANE.CurrentDevice.OptionValueSets("Current")(OptionIndex)(j), ud.Minimum)
                                            SANE.CurrentDevice.OptionValueSets("Current")(OptionIndex)(j) = ud.Minimum
                                        End If
                                        If (SANE.CurrentDevice.OptionValueSets("Current")(OptionIndex)(j) > ud.Maximum) Then
                                            Logger.Warn("Current option value '{0}' is above the maximum of '{1}' specified in the option constraint; changing value to constraint maximum.", SANE.CurrentDevice.OptionValueSets("Current")(OptionIndex)(j), ud.Maximum)
                                            SANE.CurrentDevice.OptionValueSets("Current")(OptionIndex)(j) = ud.Maximum
                                        End If
                                        ud.Value = SANE.CurrentDevice.OptionValueSets("Current")(OptionIndex)(j)
                                    Else
                                        ctl.Text = SANE.CurrentDevice.OptionValueSets("Current")(OptionIndex)(j).ToString
                                    End If
                                End If
                            End If
                        End If
                    End If

                    If RecreateControls Then
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

                        'MS bug: TextChanged event does not fire on a combobox when DropDownStyle is set to DropDownList.
                        If ctl.GetType = GetType(ComboBox) Then
                            Dim cb As ComboBox = DirectCast(ctl, ComboBox)
                            AddHandler cb.SelectedIndexChanged, AddressOf OptionControl_TextChanged
                        Else
                            AddHandler ctl.TextChanged, AddressOf OptionControl_TextChanged
                        End If
                        AddHandler ctl.Leave, AddressOf OptionControl_Leave
                        PanelOpt.Controls.Add(ctl)
                        PanelOpt.Controls.Add(ulbl)
                    End If

                Next

                If RecreateControls Then
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
                End If
                End If

            Case SANE_API.SANE_Value_Type.SANE_TYPE_STRING
                'XXX could there be an array of strings?
                Dim ctl As Control = Nothing
                If RecreateControls Then
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
                            cb.DropDownStyle = ComboBoxStyle.DropDownList
                            ctl = cb
                        Case Else
                            MsgBox("Unexpected constraint type '" & od.constraint_type.ToString & "' for value type '" & od.type.ToString & "'")
                    End Select
                    ctl.Name = "ctl_" & od.name
                    ctl.Enabled = SANE.SANE_OPTION_IS_ACTIVE(od.cap) And SANE.SANE_OPTION_IS_SETTABLE(od.cap)
                    Me.OptionValueControls(0) = ctl
                Else
                    ctl = Me.OptionValueControls(0)
                End If

                If SANE.CurrentDevice.OptionValueSets("Current") IsNot Nothing Then
                    If SANE.CurrentDevice.OptionValueSets("Current")(OptionIndex) IsNot Nothing Then
                        If SANE.CurrentDevice.OptionValueSets("Current")(OptionIndex).length - 1 >= 0 Then
                            If SANE.CurrentDevice.OptionValueSets("Current")(OptionIndex)(0) IsNot Nothing Then
                                ctl.Text = SANE.CurrentDevice.OptionValueSets("Current")(OptionIndex)(0).ToString
                            End If
                        End If
                    End If
                End If

                If RecreateControls Then
                    'MS bug: TextChanged event does not fire on a combobox when DropDownStyle is set to DropDownList.
                    If ctl.GetType = GetType(ComboBox) Then
                        Dim cb As ComboBox = DirectCast(ctl, ComboBox)
                        AddHandler cb.SelectedIndexChanged, AddressOf OptionControl_TextChanged
                    Else
                        AddHandler ctl.TextChanged, AddressOf OptionControl_TextChanged
                    End If
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
                End If
        End Select
        Me.PanelOpt.Invalidate()
    End Sub

    Private Function GetTextWidth(ByVal Text As String, ByVal DestinationControl As Control) As Integer
        Dim g As Graphics = Graphics.FromHwnd(DestinationControl.Handle)
        Dim f As SizeF = g.MeasureString(Text, DestinationControl.Font)
        Return f.Width
    End Function

    Private Sub OptionControl_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs)
        Me.PanelOptIsDirty = True
        'for comboboxes and checkboxes set the option immediately.  
        'This allows the GUI to immediately show whether 'scan continuously' will be enabled or not when the ADF option changes.
        Select Case sender.GetType
            Case GetType(ComboBox), GetType(CheckBox)
                Try
                    Dim ControlName As String = sender.Name
                    SetOption()
                    Me.PanelOpt.Controls(ControlName).Focus()
                Catch ex As Exception

                End Try
        End Select
        ButtonSaveOptionValues.Enabled = True
    End Sub

    Private Sub OptionControl_Leave(ByVal sender As System.Object, ByVal e As System.EventArgs)
        Dim ControlName As String = Nothing
        Try
            ControlName = sender.Name
        Catch
        End Try
        LastOptionControlName = ControlName
        Try
            If Me.PanelOptIsDirty Then
                Debug.Print(ControlName)
                SetOption()
            End If
        Catch ex As Exception
            Logger.Error(ex, ex.Message)
        End Try
    End Sub

    Private Sub SaveCurrentOptionValues(FileName As String)
        Dim f As String = FileName
        Try
            'Write the current settings as defaults
            If f IsNot Nothing Then
                For i As Integer = 1 To SANE.CurrentDevice.OptionDescriptors.Count - 1 'skip the first option, which is just the option count
                    If Not String.IsNullOrEmpty(SANE.CurrentDevice.OptionDescriptors(i).name) Then
                        Dim Value As String = Nothing
                        Select Case SANE.CurrentDevice.OptionDescriptors(i).type
                            Case SANE_API.SANE_Value_Type.SANE_TYPE_GROUP, SANE_API.SANE_Value_Type.SANE_TYPE_BUTTON
                                'no need to save these options
                            Case Else
                                If SANE.SANE_OPTION_IS_ACTIVE(SANE.CurrentDevice.OptionDescriptors(i).cap) And SANE.SANE_OPTION_IS_SETTABLE(SANE.CurrentDevice.OptionDescriptors(i).cap) Then
                                    For j As Integer = 0 To SANE.CurrentDevice.OptionValueSets("Current")(i).Length - 1
                                        Dim s As String = SANE.CurrentDevice.OptionValueSets("Current")(i)(j).ToString
                                        If Not String.IsNullOrEmpty(s) Then s = s.Replace(",", "\,")
                                        If Not String.IsNullOrEmpty(Value) Then Value += ","
                                        Value += s
                                    Next
                                End If
                        End Select
                        WriteIni(f, "Option." & SANE.CurrentDevice.OptionDescriptors(i).name, "DefaultValue", Value)
                    End If
                Next
            End If
        Catch ex As Exception
            Logger.Error(ex, "Error saving option values")
        End Try
    End Sub

    Friend Sub ApplyUserSettings(Optional ByVal OptValSetName As String = Nothing)
        'read all DefaultValue= settings from the backend.ini and set live values
        Debug.Print("ApplyUserSettings '" & OptValSetName & "'")
        Logger.Debug("begin")
        If (OptValSetName IsNot Nothing) AndAlso (OptValSetName.ToUpper = "LOCAL DEFAULTS") Then OptValSetName = Nothing 'Use legacy file names and behavior
        Dim SetDefaults As Boolean = False
        If SANE.CurrentDevice.OptionValueSets IsNot Nothing Then
            '"Local Defaults" option value set will be created the first time this sub is called.
            'If it exists, some values may have been changed from "Backend Defaults".
            'If we're applying settings from a file, it will likely only contain a subset of the values, 
            '   so we'll need to set "Backend Defaults" for the remaining values.
            SetDefaults = SANE.CurrentDevice.OptionValueSets.ContainsKey("Local Defaults")
        End If

        Dim PreferredFileName As String = Nothing 'Use values from this file if present
        Dim AlternateFileName As String = Nothing 'Use values from this file if not present in the preferred file
        'If OptValSetName Is Nothing Then OptValSetName = "Local Defaults"
        Select Case OptValSetName
            'Case "Local Defaults"
            Case Nothing
                PreferredFileName = CurrentSettings.SANE.Hosts(CurrentSettings.SANE.CurrentHostIndex).DeviceINI.User
                AlternateFileName = CurrentSettings.SANE.Hosts(CurrentSettings.SANE.CurrentHostIndex).DeviceINI.Shared
            Case Else
                PreferredFileName = CurrentSettings.GetDeviceConfigFileName(SharedSettings.ConfigFileScope.User, OptValSetName, True, True)
                AlternateFileName = Nothing 'Unspecified settings will be taken from "Backend Defaults" in memory
        End Select

        SANE.CurrentDevice.SupportedPageSizes = New ArrayList
        Me.ComboBoxPageSize.Items.Clear()

        Me.ImageCurve_KeyPoints.Clear()

        If (PreferredFileName IsNot Nothing) Or (AlternateFileName IsNot Nothing) Then
            CurrentSettings.ScanContinuously = False
            CurrentSettings.ScanContinuouslyUserConfigured = False
            Try
                Dim opt As String = CurrentSettings.GetINIKeyValue("General", "ScanContinuously", PreferredFileName, AlternateFileName)
                If opt IsNot Nothing Then
                    If opt.Length Then
                        CurrentSettings.ScanContinuously = Convert.ToBoolean(opt)
                        CurrentSettings.ScanContinuouslyUserConfigured = True
                    End If
                End If
            Catch ex As Exception
                Logger.Error(ex, "Exception reading ScanContinuously setting from backend.ini")
            End Try
            Logger.Info("ScanContinuously = '{0}'", CurrentSettings.ScanContinuously)
            Me.CheckBoxBatchMode.Checked = CurrentSettings.ScanContinuously 'TWAIN_VB handles CheckBox_Checked event to set CAP_FEEDERENABLED.

            For i As Integer = 1 To SANE.CurrentDevice.OptionDescriptors.Count - 1 'skip the first option, which is just the option count
                Select Case SANE.CurrentDevice.OptionDescriptors(i).type
                    Case SANE_API.SANE_Value_Type.SANE_TYPE_GROUP, SANE_API.SANE_Value_Type.SANE_TYPE_BUTTON
                        'no need to map these options
                    Case Else
                        Dim Values() As Object = Nothing
                        Dim ValuesAreDefault As Boolean = True
                        If SetDefaults Then Values = SANE.CurrentDevice.OptionValueSets("Backend Defaults")(i)
                        ''Use values from memory if available
                        'If (OptValSetName IsNot Nothing) AndAlso SANE.CurrentDevice.OptionValueSets.ContainsKey(OptValSetName) Then
                        '    Values = SANE.CurrentDevice.OptionValueSets(OptValSetName)(i)
                        '    ValuesAreDefault = False
                        'End If
                        'As a last resort, read values from disk
                        If ValuesAreDefault Then
                            Dim optval As String = CurrentSettings.GetINIKeyValue("Option." & SANE.CurrentDevice.OptionDescriptors(i).name, "DefaultValue", PreferredFileName, AlternateFileName)
                            If optval IsNot Nothing Then
                                Logger.Debug("importing setting '{0}' = '{1}'", SANE.CurrentDevice.OptionDescriptors(i).name, optval)
                                optval = optval.Replace("\,", Chr(16))
                                Values = optval.Split(",")
                                For j = 0 To Values.Length - 1
                                    If Not String.IsNullOrEmpty(Values(j)) Then Values(j) = Values(j).Replace(Chr(16), ",")
                                Next
                            End If
                        End If
                        If (Not Values Is Nothing) AndAlso (Not Values.Contains(Nothing)) Then
                            If SANE.SANE_OPTION_IS_ACTIVE(SANE.CurrentDevice.OptionDescriptors(i).cap) And SANE.SANE_OPTION_IS_SETTABLE(SANE.CurrentDevice.OptionDescriptors(i).cap) Then
                                SetOpt(i, Values) 'sets value for both SANE and TWAIN
                            Else
                                Logger.Warn("Option '{0}' is not currently settable", SANE.CurrentDevice.OptionDescriptors(i).title)
                            End If
                        End If
                End Select
            Next
            'Dim CurrentOptionIndex As Integer = LastOptionIndexDisplayed
            LastOptionIndexDisplayed = -1 'This will cause the controls to be recreated in the correct style for the new values.
            Try
                'If CurrentOptionIndex > -1 Then Me.DisplayOption(PanelOpt, CurrentOptionIndex)
                If Me.TreeViewOptions.SelectedNode IsNot Nothing Then
                    If Me.TreeViewOptions.SelectedNode.Tag IsNot Nothing Then
                        DisplayOption(PanelOpt, TreeViewOptions.SelectedNode.Tag)
                    End If
                Else
                    ClearPanelControls()
                End If
            Catch ex As Exception
                Logger.Error(ex)
            End Try

            'Set user-specified maximum page dimensions.
            'It's important to do this after iterating through the options and setting user-specified defaults because some backends have their own 
            '   page size options that constrain br-x and br-y values, which would prevent us from setting br-x and br-y to their maximum supported sizes.
            '   In that case the backend.ini should set the page size options to their maximum values so br-x and br-y can be set to their maximums.
            Dim MaxWidth As Double = 0
            Dim MaxHeight As Double = 0
            Try
                Dim opt As String = CurrentSettings.GetINIKeyValue("General", "MaxPaperWidth", PreferredFileName, AlternateFileName)
                If opt IsNot Nothing Then
                    If opt.Length Then
                        If Not Double.TryParse(opt, MaxWidth) Then
                            Throw New Exception("Unable to interpret '" & opt & "' as a decimal number")
                        End If
                    End If
                End If
            Catch ex As Exception
                Logger.Error(ex, "Exception reading MaxPaperWidth setting from backend.ini")
            End Try
            Try
                Dim opt As String = CurrentSettings.GetINIKeyValue("General", "MaxPaperHeight", PreferredFileName, AlternateFileName)
                If opt IsNot Nothing Then
                    If opt.Length Then
                        If Not Double.TryParse(opt, MaxHeight) Then
                            Throw New Exception("Unable to interpret '" & opt & "' as a decimal number")
                        End If
                    End If
                End If
            Catch ex As Exception
                Logger.Error(ex, "Exception reading MaxPaperHeight setting from backend.ini")
            End Try
            If (MaxWidth = 0) And (MaxHeight = 0) Then
                Get_Current_Device_Physical_Size_In_Inches(MaxWidth, MaxHeight)
            End If

            If (MaxWidth > 0) And (MaxHeight > 0) Then
                Dim MaxHeightRounded As Single = Math.Round(MaxHeight, 1, MidpointRounding.AwayFromZero)
                Dim MaxWidthRounded As Single = Math.Round(MaxWidth, 1, MidpointRounding.AwayFromZero)
                Logger.Debug("Device or User Maximum Height=" & MaxHeight.ToString & ", rounded to " & MaxHeightRounded.ToString)
                Logger.Debug("Device or User Maximum Width=" & MaxWidth.ToString & ", rounded to " & MaxWidthRounded.ToString)
                For Each ps As PageSize In CurrentSettings.PageSizes
                    Select Case ps.TWAIN_TWSS 'We may not be using TWAIN, but this is a more reliable property than .Name.
                        Case TWAIN_VB.TWSS.TWSS_NONE, TWAIN_VB.TWSS.TWSS_MAXSIZE
                            ps.Width = MaxWidth
                            ps.Height = MaxHeight
                            Logger.Info("Supported page size: '{0}'", ps.Name)
                            SANE.CurrentDevice.SupportedPageSizes.Add(ps)
                            Me.ComboBoxPageSize.Items.Add(ps.Name)
                        Case Else
                            Logger.Debug("Evaluating page size '{0}':", ps.Name)
                            Dim PageHeightRounded As Single = Math.Round(ps.Height, 1, MidpointRounding.AwayFromZero)
                            Dim PageWidthRounded As Single = Math.Round(ps.Width, 1, MidpointRounding.AwayFromZero)
                            Logger.Debug(vbTab & "Height={0}, rounded to {1}", ps.Height.ToString, PageHeightRounded.ToString)
                            Logger.Debug(vbTab & "Width={0}, rounded to {1}", ps.Width.ToString, PageWidthRounded.ToString)
                            If PageWidthRounded <= MaxWidthRounded Then
                                If PageHeightRounded <= MaxHeightRounded Then
                                    Logger.Info("Supported page size: '{0}'", ps.Name)
                                    SANE.CurrentDevice.SupportedPageSizes.Add(ps)
                                    Me.ComboBoxPageSize.Items.Add(ps.Name)
                                Else
                                    Logger.Debug(vbTab & "Height oversized: {0} > {1}", PageHeightRounded.ToString, MaxHeightRounded.ToString)
                                End If
                            Else
                                Logger.Debug(vbTab & "Width oversized: {0} > {1}", PageWidthRounded.ToString, MaxWidthRounded.ToString)
                            End If
                    End Select
                Next
                If TWAIN_Is_Active Then
                    Me.TWAINInstance.InitPageSizes()
                    Me.TWAINInstance.SetCap(TWAIN_VB.CAP.ICAP_PHYSICALWIDTH, MaxWidth, TWAIN_VB.DS_Entry_Pump.SetCapScope.BothValues, TWAIN_VB.DS_Entry_Pump.RequestSource.SANE)
                    Me.TWAINInstance.SetCap(TWAIN_VB.CAP.ICAP_PHYSICALHEIGHT, MaxHeight, TWAIN_VB.DS_Entry_Pump.SetCapScope.BothValues, TWAIN_VB.DS_Entry_Pump.RequestSource.SANE)
                End If
            Else
                Logger.Warn("'MaxPaperWidth' and 'MaxPaperHeight' are not configured in backend.ini and backend doesn't properly support 'br-x' and 'br-y'; unable to determine which page sizes are supported.")
            End If

            Try
                Dim opt As String = CurrentSettings.GetINIKeyValue("General", "DefaultPaperSize", PreferredFileName, AlternateFileName)
                If opt IsNot Nothing Then
                    If opt.Length Then
                        Dim Found_Default As Boolean = False
                        For Each ps As PageSize In SANE.CurrentDevice.SupportedPageSizes
                            If ps.Name.ToUpper.Trim = opt.ToUpper.Trim Then
                                Found_Default = True
                                Logger.Info("Setting default page size to '{0}'", opt.Trim)
                                Me.ComboBoxPageSize.SelectedItem = ps.Name
                                If TWAIN_Is_Active Then
                                    Me.TWAINInstance.SetCap(TWAIN_VB.CAP.ICAP_SUPPORTEDSIZES, ps.TWAIN_TWSS, TWAIN_VB.DS_Entry_Pump.SetCapScope.DefaultValue, TWAIN_VB.DS_Entry_Pump.RequestSource.SANE)
                                End If
                                Exit For
                            End If
                        Next
                        If Not Found_Default Then
                            Logger.Warn("Default page size '{0}' is not in the list of supported sizes for this device", opt.Trim)
                        End If
                    End If
                End If
            Catch ex As Exception
                Logger.Error(ex, "Exception while attempting to set DefaultPaperSize value")
            End Try
            If Me.ComboBoxPageSize.SelectedItem Is Nothing Then
                If Me.ComboBoxPageSize.Items.Contains("Maximum") Then
                    Me.ComboBoxPageSize.SelectedItem = "Maximum"
                End If
            End If
            '
            If SANE.CurrentDevice.OptionValueSets IsNot Nothing Then
                With SANE.CurrentDevice.OptionValueSets
                    If Not .ContainsKey("Local Defaults") Then
                        'Local Defaults is the result of merging the shared and user copies of backend.ini, with the user copy having priority.
                        'We should get here only during initialization of a newly selected device.
                        .Add("Local Defaults", CloneOptionValueSet(.Item("Current")))
                    End If
                    'Following lines removed to prevent storing user value sets in memory.  Always read them from file.
                    'If (OptValSetName IsNot Nothing) AndAlso (Not .ContainsKey(OptValSetName)) Then
                    '    .Add(OptValSetName, CloneOptionValueSet(.Item("Current")))
                    'End If
                End With
            End If
        Else
            Logger.Warn("Backend configuration file uninitialized; backend-specific values were not configured")
        End If
        Logger.Debug("end")
    End Sub

    Private Sub SaveUserSettings(Optional ByVal OptionValueSetName As String = Nothing)
        If OptionValueSetName IsNot Nothing AndAlso OptionValueSetName.ToUpper = "LOCAL DEFAULTS" Then OptionValueSetName = Nothing 'Use legacy ini file names and behavior
        Dim f As String = CurrentSettings.GetDeviceConfigFileName(SharedSettings.ConfigFileScope.User, OptionValueSetName, True, True)
        Try
            Me.Cursor = Cursors.WaitCursor
            If CurrentSettings.SANE.Hosts(CurrentSettings.SANE.CurrentHostIndex).DeviceINI.User Is Nothing Then
                If (OptionValueSetName Is Nothing) And (Not String.IsNullOrEmpty(f)) Then
                    CurrentSettings.SANE.Hosts(CurrentSettings.SANE.CurrentHostIndex).DeviceINI.User = f
                End If
            End If
            SaveCurrentOptionValues(f)
            If CurrentSettings.ScanContinuouslyUserConfigured Then WriteIni(f, "General", "ScanContinuously", CurrentSettings.ScanContinuously.ToString)
            If Not String.IsNullOrWhiteSpace(ComboBoxPageSize.Text) Then WriteIni(f, "General", "DefaultPaperSize", ComboBoxPageSize.Text)
        Catch ex As Exception
            Logger.Error(ex)
        Finally
            Me.Cursor = Cursors.Default
        End Try
    End Sub

    Friend Sub Get_Current_Device_Physical_Size_In_Inches(ByRef Width As Double, ByRef Height As Double)
        Dim minx, maxx, miny, maxy As Double
        'Dim tlx, tly, brx, bry As Double
        Dim xyunit As SANE_API.SANE_Unit = SANE_API.SANE_Unit.SANE_UNIT_NONE

        Width = 0
        Height = 0

        Try
            For i As Integer = 1 To SANE.CurrentDevice.OptionDescriptors.Count - 1 'skip the first option, which is just the option count
                If (SANE.CurrentDevice.OptionDescriptors(i).type <> SANE_API.SANE_Value_Type.SANE_TYPE_GROUP) And (SANE.CurrentDevice.OptionDescriptors(i).type <> SANE_API.SANE_Value_Type.SANE_TYPE_BUTTON) Then
                    If SANE.SANE_OPTION_IS_READABLE(SANE.CurrentDevice.OptionDescriptors(i).cap) Then
                        Select Case SANE.CurrentDevice.OptionDescriptors(i).name.ToLower
                            Case "tl-x"
                                xyunit = SANE.CurrentDevice.OptionDescriptors(i).unit
                                'tlx = SANE.CurrentDevice.OptionValueSets("Current")(i)(0)
                            Case "tl-y"
                                xyunit = SANE.CurrentDevice.OptionDescriptors(i).unit
                                'tly = SANE.CurrentDevice.OptionValueSets("Current")(i)(0)
                            Case "br-x"
                                xyunit = SANE.CurrentDevice.OptionDescriptors(i).unit
                                'brx = SANE.CurrentDevice.OptionValueSets("Current")(i)(0)
                                Select Case SANE.CurrentDevice.OptionDescriptors(i).constraint_type
                                    Case SANE_API.SANE_Constraint_Type.SANE_CONSTRAINT_RANGE
                                        Dim n As Integer = SANE.CurrentDevice.OptionDescriptors(i).constraint.range.min
                                        minx = IIf(SANE.CurrentDevice.OptionDescriptors(i).type = SANE_API.SANE_Value_Type.SANE_TYPE_FIXED, SANE.SANE_UNFIX(n), n)
                                        n = SANE.CurrentDevice.OptionDescriptors(i).constraint.range.max
                                        maxx = IIf(SANE.CurrentDevice.OptionDescriptors(i).type = SANE_API.SANE_Value_Type.SANE_TYPE_FIXED, SANE.SANE_UNFIX(n), n)
                                    Case SANE_API.SANE_Constraint_Type.SANE_CONSTRAINT_WORD_LIST
                                        Dim Words(SANE.CurrentDevice.OptionDescriptors(i).constraint.word_list.Length - 1) As Integer
                                        Array.Copy(SANE.CurrentDevice.OptionDescriptors(i).constraint.word_list, Words, Words.Length)
                                        Array.Sort(Words)
                                        minx = IIf(SANE.CurrentDevice.OptionDescriptors(i).type = SANE_API.SANE_Value_Type.SANE_TYPE_FIXED, SANE.SANE_UNFIX(Words(0)), Words(0))
                                        maxx = IIf(SANE.CurrentDevice.OptionDescriptors(i).type = SANE_API.SANE_Value_Type.SANE_TYPE_FIXED, SANE.SANE_UNFIX(Words(Words.Length - 1)), Words(Words.Length - 1))
                                End Select
                            Case "br-y"
                                xyunit = SANE.CurrentDevice.OptionDescriptors(i).unit
                                'bry = SANE.CurrentDevice.OptionValueSets("Current")(i)(0)
                                Select Case SANE.CurrentDevice.OptionDescriptors(i).constraint_type
                                    Case SANE_API.SANE_Constraint_Type.SANE_CONSTRAINT_RANGE
                                        Dim n As Integer = SANE.CurrentDevice.OptionDescriptors(i).constraint.range.min
                                        miny = IIf(SANE.CurrentDevice.OptionDescriptors(i).type = SANE_API.SANE_Value_Type.SANE_TYPE_FIXED, SANE.SANE_UNFIX(n), n)
                                        n = SANE.CurrentDevice.OptionDescriptors(i).constraint.range.max
                                        maxy = IIf(SANE.CurrentDevice.OptionDescriptors(i).type = SANE_API.SANE_Value_Type.SANE_TYPE_FIXED, SANE.SANE_UNFIX(n), n)
                                    Case SANE_API.SANE_Constraint_Type.SANE_CONSTRAINT_WORD_LIST
                                        Dim Words(SANE.CurrentDevice.OptionDescriptors(i).constraint.word_list.Length - 1) As Integer
                                        Array.Copy(SANE.CurrentDevice.OptionDescriptors(i).constraint.word_list, Words, Words.Length)
                                        Array.Sort(Words)
                                        miny = IIf(SANE.CurrentDevice.OptionDescriptors(i).type = SANE_API.SANE_Value_Type.SANE_TYPE_FIXED, SANE.SANE_UNFIX(Words(0)), Words(0))
                                        maxy = IIf(SANE.CurrentDevice.OptionDescriptors(i).type = SANE_API.SANE_Value_Type.SANE_TYPE_FIXED, SANE.SANE_UNFIX(Words(Words.Length - 1)), Words(Words.Length - 1))
                                End Select
                        End Select
                    End If
                End If
            Next
            'Dim xyunit As SANE_API.SANE_Unit = MyForm.GetSANEOptionUnit("tl-x")
            If (maxx > 0) And (maxy > 0) And ((xyunit = SANE_API.SANE_Unit.SANE_UNIT_PIXEL) Or (xyunit = SANE_API.SANE_Unit.SANE_UNIT_MM)) Then
                Dim PhysicalWidth As Double = maxx '- minx
                Dim PhysicalLength As Double = maxy '- miny
                'internally we want to store everything as inches for TWAIN
                Select Case xyunit
                    Case SANE_API.SANE_Unit.SANE_UNIT_MM
                        PhysicalWidth = MMToInches(PhysicalWidth)
                        PhysicalLength = MMToInches(PhysicalLength)
                    Case SANE_API.SANE_Unit.SANE_UNIT_PIXEL
                        'XXX no way to test this without a backend that reports dimensions in pixels
                        Dim res_dpi As Double = GetSANEOption("resolution")(0)
                        If res_dpi > 0 Then
                            PhysicalWidth = PhysicalWidth / res_dpi
                            PhysicalLength = PhysicalLength / res_dpi
                        Else
                            Logger.Warn("Unable to convert pixels to inches because dpi is unknown; using defaults instead")
                            PhysicalWidth = 8.5
                            PhysicalLength = 11
                        End If
                End Select
                Logger.Debug("physical size = {0} x {1} inches", PhysicalWidth, PhysicalLength)
                Width = PhysicalWidth
                Height = PhysicalLength
            End If
        Catch ex As Exception
            Logger.Error(ex, ex.Message)
        End Try
    End Sub

    Friend Sub SetTWAINCaps(ByVal OptionDescriptor As SANE_API.SANE_Option_Descriptor, ByVal OptionValues() As Object, ByVal SetDefaultValue As Boolean)
        If OptionValues.Length > 1 Then Logger.Warn("Only the first value in the array will be evaluated for option '{0}'", OptionDescriptor.title)
        If (CurrentSettings.SANE.Hosts(CurrentSettings.SANE.CurrentHostIndex).DeviceINI.User IsNot Nothing) Or (CurrentSettings.SANE.Hosts(CurrentSettings.SANE.CurrentHostIndex).DeviceINI.Shared IsNot Nothing) Then
            If OptionValues.Length > 0 Then
                If OptionValues(0) IsNot Nothing Then
                    'Dim s As String = CurrentSettings.SANE.Hosts(CurrentSettings.SANE.CurrentHostIndex).DeviceINI.GetKeyValue("Option." & OptionDescriptor.name, "TWAIN." & OptionValues(0).ToString.Replace(" ", ""))
                    Dim s As String = CurrentSettings.GetINIKeyValue("Option." & OptionDescriptor.name, "TWAIN." & OptionValues(0).ToString.Replace(" ", ""), CurrentSettings.SANE.Hosts(CurrentSettings.SANE.CurrentHostIndex).DeviceINI.User, CurrentSettings.SANE.Hosts(CurrentSettings.SANE.CurrentHostIndex).DeviceINI.Shared)
                    'If there wasn't a TWAIN mapping for the specific value that was set, look for a general mapping.
                    'If (s Is Nothing) OrElse (s.Length = 0) Then s = CurrentSettings.SANE.Hosts(CurrentSettings.SANE.CurrentHostIndex).DeviceINI.GetKeyValue("Option." & OptionDescriptor.name, "TWAIN")
                    If (s Is Nothing) OrElse (s.Length = 0) Then s = CurrentSettings.GetINIKeyValue("Option." & OptionDescriptor.name, "TWAIN", CurrentSettings.SANE.Hosts(CurrentSettings.SANE.CurrentHostIndex).DeviceINI.User, CurrentSettings.SANE.Hosts(CurrentSettings.SANE.CurrentHostIndex).DeviceINI.Shared)
                    If s IsNot Nothing AndAlso s.Length Then
                        Dim caps() As String = s.Split(";")
                        Dim capName(caps.Length - 1) As String
                        Dim capVal(caps.Length - 1) As String
                        For i = 0 To caps.Length - 1
                            Dim ss() As String = caps(i).Split(",")
                            If ss.Length = 2 Then
                                capName(i) = ss(0).Trim.ToUpper
                                capVal(i) = ss(1).Trim.ToUpper
                                Logger.Debug("TWAIN Capability = '{0}', Value = '{1}'", capName(i), capVal(i))
                                If Me.TWAINInstance IsNot Nothing Then
                                    Dim n As TWAIN_VB.CAP
                                    If [Enum].TryParse(capName(i), True, n) Then
                                        If capVal(i) = "#" Then capVal(i) = OptionValues(0)
                                        Me.TWAINInstance.SetCap(n, capVal(i), IIf(SetDefaultValue, TWAIN_VB.DS_Entry_Pump.SetCapScope.BothValues, TWAIN_VB.DS_Entry_Pump.SetCapScope.CurrentValue), TWAIN_VB.DS_Entry_Pump.RequestSource.SANE)
                                    Else
                                        Logger.Warn("Unknown TWAIN capability '{0}'", capName(i))
                                    End If
                                End If
                            Else
                                Logger.Warn("Malformed TWAIN capability mapping: '{0}'", caps(i))
                            End If
                        Next
                    End If
                End If
            End If
        Else
            Logger.Warn("Backend configuration file uninitialized; SANE to TWAIN capability mappings were not configured")
        End If
    End Sub

    Friend Function GetSANEOptionUnit(ByVal OptionName As String) As Integer
        If OptionName Is Nothing Then OptionName = String.Empty
        For Index As Integer = 1 To SANE.CurrentDevice.OptionDescriptors.Length - 1
            Dim od As SANE_API.SANE_Option_Descriptor = SANE.CurrentDevice.OptionDescriptors(Index)
            If Not String.IsNullOrEmpty(od.name) Then
                If od.name.ToUpper.Trim = OptionName.ToUpper.Trim Then
                    Return SANE.CurrentDevice.OptionDescriptors(Index).unit
                End If
            End If
        Next
        Return SANE_API.SANE_Unit.SANE_UNIT_NONE
    End Function

    Friend Function GetSANEOption(ByVal OptionName As String) As Object()
        If OptionName Is Nothing Then OptionName = String.Empty
        For Index As Integer = 0 To SANE.CurrentDevice.OptionDescriptors.Length - 1
            Dim od As SANE_API.SANE_Option_Descriptor = SANE.CurrentDevice.OptionDescriptors(Index)
            If Not String.IsNullOrEmpty(od.name) Then
                If od.name.ToUpper.Trim = OptionName.ToUpper.Trim Then
                    Return SANE.CurrentDevice.OptionValueSets("Current")(Index)
                End If
            End If
        Next
        Return Nothing
    End Function

    Public Function SetSANEOption(ByVal OptionName As String, ByVal Values() As Object) As Boolean
        Return (SetOpt(OptionName, Values) = SANE_API.SANE_Status.SANE_STATUS_GOOD)
    End Function

    Private Function SetOpt(ByVal OptionName As String, ByVal Values() As Object) As SANE_API.SANE_Status
        If OptionName Is Nothing Then OptionName = String.Empty
        For Index As Integer = 1 To SANE.CurrentDevice.OptionDescriptors.Length - 1 'skip the first option, which is just the option count
            Dim od As SANE_API.SANE_Option_Descriptor = SANE.CurrentDevice.OptionDescriptors(Index)
            If Not String.IsNullOrEmpty(od.name) Then
                If od.name.ToUpper.Trim = OptionName.ToUpper.Trim Then
                    Return SetOpt(Index, Values)
                End If
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

                Logger.Debug("Value Type = '{0}', Value = '{1}'", Values(i).GetType, Values(i))
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
            Dim Status As SANE_API.SANE_Status
            'The following variables and related code were added because of the lossy conversion between SANE_TYPE_FIXED and Double (SANE_FIX and SANE_UNFIX).
            'Because SANE_UNFIX rounds up, we should only ever need to decrease the value to avoid errors.
            Dim FixedFudgeCount As Integer = 0 'The number of times we've decreased a SANE_TYPE_FIXED value because the backend refused it.
            Dim MaxFixedFudgeCount As Integer = 3 'The maximum number of times we'll try to fudge a SANE_TYPE_FIXED value.
            Dim FixedFudgeIncrement As Double = 0.0001R 'The amount to subtract from a SANE_TYPE_FIXED value to see if the backend will accept it.
            '
            Do
                Status = SANE.Net_Control_Option(ControlClient, OptReq, OptReply,
                         CurrentSettings.SANE.Hosts(CurrentSettings.SANE.CurrentHostIndex).Username,
                         CurrentSettings.SANE.Hosts(CurrentSettings.SANE.CurrentHostIndex).Password)
                If Status = SANE_API.SANE_Status.SANE_STATUS_GOOD Then
                    Array.Copy(OptReply.values, SANE.CurrentDevice.OptionValueSets("Current")(OptionIndex), OptReply.values.Length)
                    'If ComboBoxOptionValueSet.Text IsNot Nothing Then
                    '    If SANE.CurrentDevice.OptionValueSets.ContainsKey(ComboBoxOptionValueSet.Text) Then
                    '        Array.Copy(OptReply.values, SANE.CurrentDevice.OptionValueSets(ComboBoxOptionValueSet.Text)(OptionIndex), OptReply.values.Length)
                    '    End If
                    'End If
                    If OptReply.info And SANE_API.SANE_INFO_RELOAD_OPTIONS Then GetOpts(False)
                    If Me.TWAIN_Is_Active Then
                        SetTWAINCaps(od, Values, False)
                    End If
                    Exit Do
                ElseIf Status = SANE_API.SANE_Status.SANE_STATUS_ACCESS_DENIED Then
                    Dim PwdBox As New FormSANEAuth
                    PwdBox.UsernameTextBox.Text = CurrentSettings.SANE.Hosts(CurrentSettings.SANE.CurrentHostIndex).Username
                    If PwdBox.ShowDialog = Windows.Forms.DialogResult.Cancel Then Exit Do
                    CurrentSettings.SANE.Hosts(CurrentSettings.SANE.CurrentHostIndex).Username = PwdBox.UsernameTextBox.Text
                    CurrentSettings.SANE.Hosts(CurrentSettings.SANE.CurrentHostIndex).Password = PwdBox.PasswordTextBox.Text
                Else
                    Dim msg As String = "Error setting '" & od.type.ToString & "' option '" & od.title & "': " & Status.ToString
                    Logger.Warn(msg)
                    If OptReq.value_type = SANE_API.SANE_Value_Type.SANE_TYPE_FIXED AndAlso FixedFudgeCount < MaxFixedFudgeCount Then
                        FixedFudgeCount += 1
                        Logger.Warn("Fudging SANE_TYPE_FIXED value -0.0001 (attempt " & FixedFudgeCount.ToString & " of " & MaxFixedFudgeCount.ToString & ")")
                        For j As Integer = 0 To OptReq.values.Length - 1
                            OptReq.values(j) -= FixedFudgeIncrement
                        Next
                    Else
                        MsgBox(msg)
                        Exit Do
                    End If
                End If
            Loop

            Me.PanelOptIsDirty = False

            If Not String.IsNullOrWhiteSpace(ComboBoxOptionValueSet.Text) Then
                ButtonSaveOptionValues.Enabled = True
            End If

            Return Status
        Catch ex As ApplicationException
            Logger.Warn(ex.Message, ex)
            Throw
        Catch ex As Exception
            Logger.Error(ex, ex.Message)
            Throw
        End Try

    End Function

    Private Sub SetOption(sender As Object, e As EventArgs)
        SetOption()
    End Sub

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
                Case GetType(ImageCurve)
                    Dim ic As ImageCurve = DirectCast(ctl, ImageCurve)
                    ReDim Values(ic.LevelValue.Length - 1)
                    ic.LevelValue.CopyTo(Values, 0)
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
            Logger.Error(ex, "Unable to set option value")
        End Try
    End Sub

    Private Sub FormMain_Resize(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Resize
        If Me.TreeViewOptions.SelectedNode IsNot Nothing Then
            If Me.TreeViewOptions.SelectedNode.Tag IsNot Nothing Then
                Try
                    DisplayOption(PanelOpt, TreeViewOptions.SelectedNode.Tag)
                Catch ex As Exception
                    Logger.Error(ex)
                End Try
            End If
        Else
            ClearPanelControls()
        End If
    End Sub

    Private Sub TreeViewOptions_AfterSelect(ByVal sender As Object, ByVal e As System.Windows.Forms.TreeViewEventArgs) Handles TreeViewOptions.AfterSelect
        Try
            DisplayOption(PanelOpt, e.Node.Tag)
        Catch ex As Exception
            Debug.Print(ex.Message)
        End Try
    End Sub

    Private Sub FormMain_Shown(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Shown
        Update_Host_GUI()
        If Me.TreeViewOptions.SelectedNode IsNot Nothing Then
            Me.TreeViewOptions_AfterSelect(Me, New TreeViewEventArgs(Me.TreeViewOptions.SelectedNode))
        End If
    End Sub

    Private Sub Update_Host_GUI()
        Debug.Print("Update_Host_GUI")
        If (CurrentSettings.SANE.CurrentHostIndex > -1) AndAlso (CurrentSettings.SANE.CurrentHostIndex < CurrentSettings.SANE.Hosts.Length) _
            AndAlso CurrentSettings.SANE.Hosts(CurrentSettings.SANE.CurrentHostIndex).Open Then
            Me.TextBoxHost.Text = CurrentSettings.SANE.Hosts(CurrentSettings.SANE.CurrentHostIndex).NameOrAddress
            Me.TextBoxPort.Text = CurrentSettings.SANE.Hosts(CurrentSettings.SANE.CurrentHostIndex).Port
            Me.TextBoxDevice.Text = CurrentSettings.SANE.Hosts(CurrentSettings.SANE.CurrentHostIndex).Device

            PopulateOptionValueSetNames()
            If String.IsNullOrWhiteSpace(ComboBoxOptionValueSet.Text) Then ComboBoxOptionValueSet.Text = "Local Defaults"
            Me.CheckBoxSaveOnExit.Checked = CurrentSettings.SaveDefaultsOnExit

        Else
            'Me.TextBoxHost.Text = Nothing
            'Me.TextBoxPort.Text = Nothing
            'Me.TextBoxDevice.Text = Nothing

            Me.ButtonOK.Enabled = False
            ClearPanelControls()
            Me.TreeViewOptions.Nodes.Clear()
            'Me.ComboBoxPageSize.Enabled = False
        End If
    End Sub

    Private Sub ButtonCancel_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ButtonCancel.Click
        'TWAIN_VB will capture the FormClosing event and cancel it, sending MSG_CLOSEDSREQ instead.
        Me.Result = UIResult.Cancel
        Me.Close()
    End Sub

    Private Sub CheckBoxBatchMode_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles CheckBoxBatchMode.CheckedChanged
        CurrentSettings.ScanContinuously = CheckBoxBatchMode.Checked
        If Not String.IsNullOrWhiteSpace(ComboBoxOptionValueSet.Text) Then
            ButtonSaveOptionValues.Enabled = True
        End If
    End Sub

    Private Sub ButtonHost_Click(sender As Object, e As EventArgs) Handles ButtonHost.Click

        If TWAIN_Is_Active Then
            Dim result As MsgBoxResult = MsgBox("The TWAIN data source has already been initialized using information from the current scanner." _
                                              & "  If you change to a different scanner, you should close and reopen your TWAIN application to avoid" _
                                              & " mismatched parameters and unexpected behavior.  Are you sure you want to continue?" _
                                              , MsgBoxStyle.Critical + MsgBoxStyle.YesNo, "Dangerous Device Change")
            If result <> MsgBoxResult.Yes Then Exit Sub
        End If

        Close_SANE()
        Close_Net()
        If SANE Is Nothing Then SANE = New SANE_API
        Dim f As New FormSANEHostWizard
        If f.ShowDialog <> Windows.Forms.DialogResult.OK Then
            Logger.Debug("User cancelled SANE host wizard")
        Else
            Try_Init_SANE(False)
        End If
        Update_Host_GUI()
    End Sub

    Private Sub CheckBoxBatchMode_Click(sender As Object, e As EventArgs) Handles CheckBoxBatchMode.Click
        CurrentSettings.ScanContinuouslyUserConfigured = True

    End Sub

    Private Sub ComboBoxPageSize_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ComboBoxPageSize.SelectedIndexChanged
        Dim br_x, br_y As Double
        Try
            Dim ps As PageSize = Nothing
            For i As Integer = 0 To SANE.CurrentDevice.SupportedPageSizes.Count - 1
                If SANE.CurrentDevice.SupportedPageSizes(i).Name = Me.ComboBoxPageSize.SelectedItem Then
                    ps = SANE.CurrentDevice.SupportedPageSizes(i)
                    Exit For
                End If
            Next

            If ps Is Nothing Then Throw New Exception("Selected page size is missing from the collection")

            If TWAIN_Is_Active Then
                'Setting this cap will cause tl-x, tl-y, br-x, and br-y to be set.
                Me.TWAINInstance.SetCap(TWAIN_VB.CAP.ICAP_SUPPORTEDSIZES, ps.TWAIN_TWSS, TWAIN_VB.DS_Entry_Pump.SetCapScope.CurrentValue, TWAIN_VB.DS_Entry_Pump.RequestSource.TWAIN)
            Else
                br_x = ps.Width
                br_y = ps.Height

                Select Case GetSANEOptionUnit("br-x")
                    Case SANE_API.SANE_Unit.SANE_UNIT_MM
                        br_x = InchesToMM(br_x)
                        br_y = InchesToMM(br_y)
                    Case SANE_API.SANE_Unit.SANE_UNIT_PIXEL
                        Dim res_dpi As Double = CDbl(GetSANEOption("resolution")(0))
                        br_x = br_x * res_dpi
                        br_y = br_y * res_dpi
                    Case Else
                        Throw New Exception("Page coordinates must use unit SANE_UNIT_MM or SANE_UNIT_PIXEL")
                End Select

                If SetSANEOption("tl-x", {0}) And
                    SetSANEOption("tl-y", {0}) And
                    SetSANEOption("br-x", {br_x}) And
                    SetSANEOption("br-y", {br_y}) Then
                    'ok
                Else
                    Throw New Exception("Unable to set SANE option")
                End If
            End If
        Catch ex As Exception
            Logger.Error(ex, "Error setting page dimensions")
        End Try
    End Sub

    Friend Sub ClearPanelControls()
        Try
            For Each ctl As Control In Me.PanelOpt.Controls
                Select Case ctl.GetType
                    Case GetType(CheckBox)
                        Dim chk As CheckBox = DirectCast(ctl, CheckBox)
                        RemoveHandler chk.CheckedChanged, AddressOf OptionControl_TextChanged
                    Case GetType(ComboBox)
                        Dim cb As ComboBox = DirectCast(ctl, ComboBox)
                        RemoveHandler cb.SelectedIndexChanged, AddressOf OptionControl_TextChanged
                    Case GetType(Button)
                        Dim bt As Button = DirectCast(ctl, Button)
                        RemoveHandler bt.Click, AddressOf SetOption
                    Case GetType(ImageCurve)
                        Dim ic As ImageCurve = DirectCast(ctl, ImageCurve)
                        RemoveHandler ic.ImageLevelChanged, AddressOf imageCurve_ImageLevelChanged
                End Select
                If ctl.GetType IsNot GetType(Label) Then
                    RemoveHandler ctl.Leave, AddressOf OptionControl_Leave
                    RemoveHandler ctl.TextChanged, AddressOf OptionControl_TextChanged
                End If
                'ctl.Dispose()
                ctl = Nothing
            Next
        Catch ex As Exception
            Logger.Error(ex, "Error removing event handlers from panel controls")
        End Try
        Me.PanelOpt.Controls.Clear()
    End Sub

    Private Sub imageCurve_ImageLevelChanged(sender As Object, e As ImageLevelEventArgs)
        Me.PanelOptIsDirty = True
        ButtonSaveOptionValues.Enabled = True
        If Not String.IsNullOrEmpty(sender.Name) Then
            Dim Keypoints As List(Of System.Drawing.Point) = DirectCast(sender.KeyPt, List(Of System.Drawing.Point))
            Dim NewList As List(Of System.Drawing.Point) = Keypoints.ToList()
            If Me.ImageCurve_KeyPoints.ContainsKey(sender.Name) Then
                Me.ImageCurve_KeyPoints(sender.Name) = NewList
            Else
                Me.ImageCurve_KeyPoints.Add(sender.Name, NewList)
            End If
        End If
    End Sub

    Private Sub PanelOpt_Paint(sender As Object, e As PaintEventArgs) Handles PanelOpt.Paint
        MyBase.OnPaint(e)

        For Each ctl As Control In PanelOpt.Controls
            If ctl.GetType() = GetType(ImageCurve) Then

                Try
                    Dim BorderColor As Color = System.Drawing.SystemColors.ControlText
                    If Not ctl.Enabled Then BorderColor = System.Drawing.SystemColors.GrayText

                    Dim ic As ImageCurve = DirectCast(ctl, ImageCurve)
                    Dim imCurveRect As Rectangle = New Rectangle(ic.Left - 2, ic.Top - 2, ic.Width + 4, ic.Height + 4)

                    Dim g As Graphics = e.Graphics

                    ' draw X ruler
                    Dim x0 As Integer = 0
                    Dim x2 As Integer = ic.MaxX
                    Dim unitX As Single = CSng(ic.Width) / CSng(ic.MaxX)

                    Dim incr As Integer = GetImageCurveScaleIncrement(x2)

                    For i As Integer = x0 To x2
                        If i Mod incr = 0 Then
                            g.DrawLine(New Pen(BorderColor), New PointF((i - x0) * unitX + ic.Left, imCurveRect.Bottom), New PointF((i - x0) * unitX + ic.Left, imCurveRect.Bottom + 5))
                        End If
                        ' ruler line
                        If i Mod (incr * 5) = 0 Then
                            g.DrawLine(New Pen(BorderColor, 2.0F), New PointF((i - x0) * unitX + ic.Left, imCurveRect.Bottom), New PointF((i - x0) * unitX + ic.Left, imCurveRect.Bottom + 12))
                            Dim stringSize As SizeF = g.MeasureString(i.ToString(), Me.Font)
                            Dim stringLoc As New PointF((i - x0) * unitX + ic.Left - stringSize.Width / 2, imCurveRect.Bottom + 12)
                            g.DrawString(i.ToString(), Me.Font, New SolidBrush(BorderColor), stringLoc)
                        End If
                    Next

                    ' draw Y ruler
                    Dim y0 As Integer = 0
                    Dim y2 As Integer = ic.MaxY
                    Dim unitY As Single = CSng(ic.Height) / CSng(y2)

                    incr = GetImageCurveScaleIncrement(y2)

                    For i As Integer = y0 To y2
                        If i Mod incr = 0 Then
                            g.DrawLine(New Pen(BorderColor), New PointF(imCurveRect.Left - 5, ic.Bottom - (i - y0) * unitY), New PointF(imCurveRect.Left, ic.Bottom - (i - y0) * unitY))
                        End If
                        ' ruler line
                        If i Mod (incr * 5) = 0 Then
                            g.DrawLine(New Pen(BorderColor, 2.0F), New PointF(imCurveRect.Left - 10, ic.Bottom - (i - y0) * unitY), New PointF(imCurveRect.Left, ic.Bottom - (i - y0) * unitY))
                            Dim stringSize As SizeF = g.MeasureString(i.ToString(), Me.Font)
                            Dim stringLoc As New PointF(imCurveRect.Left - 10 - stringSize.Width, ic.Bottom - (i - y0) * unitY - stringSize.Height / 2)

                            g.DrawString(i.ToString(), Me.Font, New SolidBrush(BorderColor), stringLoc)
                        End If
                    Next

                    g.DrawRectangle(New Pen(BorderColor, 2), imCurveRect)
                Catch ex As Exception
                    Logger.Error(ex, "Error painting ImageCurve rulers")
                End Try

                Exit For
            End If
        Next
    End Sub

    Private Function GetImageCurveScaleIncrement(ByVal MaxVal As Integer) As Integer
        Dim incr As Integer = MaxVal \ 25
        Dim power As Integer = 0
        Do
            If incr < 10 Then Exit Do
            If incr > 0 Then power += 1 Else Exit Do
            incr = incr \ 10
        Loop
        Return (incr * 10 ^ power)
    End Function

    Private Sub ImageFrameProgress(PercentComplete As Integer)
        If Me.ShowScanProgress And (Not TWAIN_Is_Active) Then
            If frmProgress IsNot Nothing Then frmProgress.ShowFrameProgress(PercentComplete)
        End If
        RaiseEvent ImageProgress(PercentComplete)
    End Sub

    Friend Sub SetControlsEnabled(NewState As Boolean)
        'Disable or enable all controls and controlboxes while preserving their original state, and display a waitcursor if disabled.
        Static Initialized As Boolean
        Static CurrentState As Boolean
        Static OriginalControlState() As Boolean
        If Not Initialized Then CurrentState = True 'True is the normal, enabled state
        Try
            Select Case NewState
                Case False
                    If CurrentState = True Then
                        Me.Cursor = Cursors.WaitCursor
                        ReDim OriginalControlState(Me.Controls.Count - 1)
                        For i As Integer = 0 To Me.Controls.Count - 1
                            OriginalControlState(i) = Me.Controls(i).Enabled
                            Me.Controls(i).Enabled = False
                        Next
                        Me.ControlBox = False
                    End If
                Case True
                    If CurrentState = False Then
                        Me.Cursor = Cursors.Default
                        Me.ControlBox = True
                        For i As Integer = 0 To Me.Controls.Count - 1
                            Me.Controls(i).Enabled = OriginalControlState(i)
                        Next
                    End If
            End Select
        Catch ex As Exception
            Logger.Error(ex)
        End Try
    End Sub

    Public Shadows Sub Show()
        Me.Result = UIResult.None
        MyBase.Show()
    End Sub
    Public Shadows Sub Show(ByVal Owner As IWin32Window)
        Me.Result = UIResult.None
        MyBase.Show(Owner)
    End Sub

    Public Shadows Function ShowDialog() As UIResult
        Me.Result = UIResult.None
        MyBase.ShowDialog()
        Return Me.Result
    End Function

    Private Function CloneOptionValueSet(CloneFrom As Object()) As Object()
        'Create a deep clone of an option value set.  Array.clone is a shallow clone.
        If CloneFrom IsNot Nothing Then
            Dim CloneTo(CloneFrom.Count - 1) As Object
            For i As Integer = 0 To CloneFrom.Count - 1
                If CloneFrom(i) IsNot Nothing Then
                    Dim FromVals As Object() = CloneFrom(i)
                    Dim ToVals(FromVals.Count - 1) As Object
                    For j As Integer = 0 To FromVals.Count - 1
                        ToVals(j) = FromVals(j)
                    Next
                    CloneTo(i) = ToVals
                End If
            Next
            Return CloneTo
        End If
        Return Nothing
    End Function

    Private Sub PopulateOptionValueSetNames()
        Debug.Print("PopulateOptionValueSetNames")
        Me.ComboBoxOptionValueSet.Items.Clear()
        For Each s As String In SANE.CurrentDevice.OptionValueSets.Keys
            Select Case s
                Case "Current", "Backend Defaults"
                    'Exclude from list
                Case Else
                    Me.ComboBoxOptionValueSet.Items.Add(s)
            End Select
        Next
        Dim Names As ArrayList = CurrentSettings.GetSavedOptionValueSetNames
        For Each s As String In Names
            If Not Me.ComboBoxOptionValueSet.Items.Contains(s) Then Me.ComboBoxOptionValueSet.Items.Add(s)
        Next
    End Sub

    Private Sub ComboBoxOptionValueSet_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ComboBoxOptionValueSet.SelectedIndexChanged
        Debug.Print("ComboBoxOptionValueSet_SelectedIndexChanged")
        Select Case ComboBoxOptionValueSet.SelectedItem
            Case "Current", "Backend Defaults"
                'Do nothing.  These should never be in the list.
            Case Else
                If Me.Initialized Then 'Prevent applying defaults twice on initial startup
                    ApplyUserSettings(ComboBoxOptionValueSet.SelectedItem)
                End If
        End Select
        ButtonSaveOptionValues.Enabled = False
    End Sub

    Private Sub ButtonSaveOptionValues_Click(sender As Object, e As EventArgs) Handles ButtonSaveOptionValues.Click
        Dim s As String = ComboBoxOptionValueSet.Text
        If String.IsNullOrWhiteSpace(s) Then
            MsgBox("Please specify a name for the option value set", MsgBoxStyle.Exclamation + MsgBoxStyle.OkOnly, "Invalid Name")
            Exit Sub
        Else
            Dim InvalidChars As Char() = System.IO.Path.GetInvalidFileNameChars
            For Each c As Char In s
                If InvalidChars.Contains(c) Then
                    MsgBox("Name contains invalid characters", MsgBoxStyle.OkOnly + MsgBoxStyle.Exclamation, "Invalid Name")
                    Exit Sub
                End If
            Next
        End If
        Try
            Me.Cursor = Cursors.WaitCursor
            SaveUserSettings(s)
            PopulateOptionValueSetNames()
            ComboBoxOptionValueSet.Text = s
        Catch ex As Exception
            Logger.Error(ex)
        Finally
            Me.Cursor = Cursors.Default
        End Try
    End Sub

    Private Sub ComboBoxOptionValueSet_TextChanged(sender As Object, e As EventArgs) Handles ComboBoxOptionValueSet.TextChanged
        Debug.Print("ComboBoxOptionValueSet_TextChanged")
        If Not String.IsNullOrWhiteSpace(ComboBoxOptionValueSet.Text) Then ButtonSaveOptionValues.Enabled = True
    End Sub

    Private Sub CheckBoxSaveOnExit_CheckedChanged(sender As Object, e As EventArgs) Handles CheckBoxSaveOnExit.CheckedChanged
        CurrentSettings.SaveDefaultsOnExit = CheckBoxSaveOnExit.Checked
    End Sub
End Class

