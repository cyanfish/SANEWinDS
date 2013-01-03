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
Public Class SharedSettings

    Private Logger As NLog.Logger = NLog.LogManager.GetCurrentClassLogger()
    'Public Enum UserSettingsLocations As Integer
    '    LocalAppData = 0
    '    RoamingAppData = 1
    'End Enum
    Public Structure HostInfo
        Dim NameOrAddress As String
        Dim UseTSClientIP As Boolean 'Are we currently using the TS client ip as the NameOrAddress?
        Dim Port As Integer
        Dim Username As String
        Dim Password As String
        Dim TCP_Timeout_ms As Integer
        Dim Open As Boolean
        Dim Device As String
        Dim DeviceINI As IniFile.IniFile
        Dim AutoLocateDevice As String 'SANE backend name of device to auto-choose from list of devices on CurrentHost (example: "canon_dr")
    End Structure
    Public Structure SANESettings
        Dim Hosts() As HostInfo
        'Dim CurrentHost As HostInfo
        Dim CurrentHostIndex As Integer
        'Dim CurrentDevice As String    'Full SANE device name on CurrentHost (example: "canon_dr:libusb:008:005")
        'Dim CurrentDeviceINI As IniFile
        'Dim AutoLocateDevice As String 'SANE backend name of device to auto-choose from list of devices on CurrentHost (example: "canon_dr")
    End Structure
    'Public Class SANESettings
    '    Public Hosts() As HostInfo
    '    'Dim CurrentHost As HostInfo
    '    Public CurrentHostIndex As Integer
    '    Public CurrentDevice As String    'Full SANE device name on CurrentHost (example: "canon_dr:libusb:008:005")
    '    Public CurrentDeviceINI As IniFile
    '    Public AutoLocateDevice As String 'SANE backend name of device to auto-choose from list of devices on CurrentHost (example: "canon_dr")
    '    Public ReadOnly Property CurrentHost As HostInfo
    '        Get
    '            If (Me.CurrentHostIndex >= 0) And (Me.CurrentHostIndex < Me.Hosts.Length) Then
    '                Return Me.Hosts(Me.CurrentHostIndex)
    '            Else
    '                Return Nothing
    '            End If
    '        End Get
    '    End Property
    'End Class

    Public Structure TWAINSettings
        'Dim Enabled As Boolean

    End Structure
    Public UserConfigDirectory As String
    Public SharedConfigDirectory As String
    Public LogDirectory As String

    Public ProductName As System.Reflection.AssemblyName = System.Reflection.Assembly.GetExecutingAssembly.GetName

    Private Initialized As Boolean
    Private UserSettingsFolder As String
    Public UseRoamingAppData As Boolean
    Public ScanContinuously As Boolean
    Public ScanContinuouslyUserConfigured As Boolean
    Public SANE As SANESettings
    Public TWAIN As TWAINSettings

    Public PageSizes As New ArrayList
    Private Const MAX_HOSTS As Integer = 50

    Public Sub New(ByVal _UseRoamingAppData As Boolean)
        If Not Initialized Then
            UseRoamingAppData = _UseRoamingAppData
            ReadSettings()
            InitPageSizes()
            Initialized = True
        End If
    End Sub

    Public Sub Save()
        Me.WriteSettings()
    End Sub

    Private Sub WriteSettings()
        'XXX The INIFile class writes the settings out of order
        Dim INI As New IniFile.IniFile
        Dim UserSettingsFileName As String = Me.GetUserConfigFileName
        INI.Load(UserSettingsFileName)

        If INI.GetSection("Log") Is Nothing Then
            INI.AddSection("Log")
            INI.Save(UserSettingsFileName)
        End If
        'XXX this value isn't currently saved in memory anywhere.
        'If String.IsNullOrEmpty(INI.GetKeyValue("Log", "RetainDays")) Then
        '    INI.SetKeyValue("Log", "RetainDays", CurrentSettings.)
        '    INI.Save(UserSettingsFileName)
        'End If
        If INI.GetSection("SANE") Is Nothing Then
            INI.AddSection("SANE")
            INI.Save(UserSettingsFileName)
        End If
        'If String.IsNullOrEmpty(INI.GetKeyValue("SANE", "Hosts")) Then
        '    INI.SetKeyValue("SANE", "Hosts", " ")
        '    INI.Save(UserSettingsFileName)
        'End If

        'If Me.SANE.Hosts(CurrentSettings.SANE.CurrentHostIndex).NameOrAddress IsNot Nothing Then
        '    If Me.SANE.Hosts(CurrentSettings.SANE.CurrentHostIndex).Port > 0 Then
        '        Dim CurrentHostString As String = Me.SANE.Hosts(CurrentSettings.SANE.CurrentHostIndex).NameOrAddress.Trim & ":" & Me.SANE.Hosts(CurrentSettings.SANE.CurrentHostIndex).Port.ToString
        '        Dim Current_Host_Is_In_The_List As Boolean = False
        '        Dim hostlist As String = INI.GetKeyValue("SANE", "Hosts")
        '        If Not String.IsNullOrEmpty(hostlist) Then
        '            Dim hosts() As HostInfo = Me.GetSANEHostsFromString(hostlist)
        '            For Each host As HostInfo In hosts
        '                If host.NameOrAddress.ToLower = Me.SANE.Hosts(CurrentSettings.SANE.CurrentHostIndex).NameOrAddress.ToLower Then
        '                    If host.Port = Me.SANE.Hosts(CurrentSettings.SANE.CurrentHostIndex).Port Then
        '                        Current_Host_Is_In_The_List = True
        '                        Exit For
        '                    End If
        '                End If
        '            Next
        '        End If
        '        If Not Current_Host_Is_In_The_List Then

        '        End If
        '    End If
        'End If

        Dim HostList As String = Nothing
        For idx As Integer = 0 To MAX_HOSTS
            Dim SectionName As String = "Host." & idx.ToString
            INI.RemoveSection(SectionName)
            If idx < SANE.Hosts.Length Then
                INI.AddSection(SectionName)
                With SANE.Hosts(idx)
                    INI.SetKeyValue(SectionName, "NameOrAddress", .NameOrAddress)
                    INI.SetKeyValue(SectionName, "UseTSClientIP", .UseTSClientIP.ToString)
                    INI.SetKeyValue(SectionName, "Port", .Port.ToString)
                    INI.SetKeyValue(SectionName, "Username", .Username)
                    INI.SetKeyValue(SectionName, "Password", .Password)
                    INI.SetKeyValue(SectionName, "Timeout_ms", .TCP_Timeout_ms.ToString)
                    INI.SetKeyValue(SectionName, "Device", .Device)
                    INI.SetKeyValue(SectionName, "AutoLocateDevice", .AutoLocateDevice)
                End With
            End If
        Next

        INI.SetKeyValue("SANE", "DefaultHost", CurrentSettings.SANE.CurrentHostIndex)

        'If String.IsNullOrEmpty(INI.GetKeyValue("SANE", "DefaultHost")) Then
        '    INI.SetKeyValue("SANE", "DefaultHost", "0")
        '    INI.Save(UserSettingsFileName)
        'End If
        'If String.IsNullOrEmpty(INI.GetKeyValue("SANE", "Device")) Then
        '    INI.SetKeyValue("SANE", "Device", " ")
        '    INI.Save(UserSettingsFileName)
        'End If
        'If String.IsNullOrEmpty(INI.GetKeyValue("SANE", "AutoLocateDevice")) Then
        '    INI.SetKeyValue("SANE", "AutoLocateDevice", " ")
        '    INI.Save(UserSettingsFileName)
        'End If

        INI.Save(UserSettingsFileName)

    End Sub

    Private Sub ReadSettings()
        SANE = New SANESettings
        TWAIN = New TWAINSettings

        If UseRoamingAppData Then
            UserSettingsFolder = System.Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData) & "\" & Me.ProductName.Name
        Else
            UserSettingsFolder = System.Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData) & "\" & Me.ProductName.Name
        End If
        If Not My.Computer.FileSystem.DirectoryExists(UserSettingsFolder) Then My.Computer.FileSystem.CreateDirectory(UserSettingsFolder)

        Me.UserConfigDirectory = UserSettingsFolder
        Me.LogDirectory = Me.UserConfigDirectory 'XXX

        Try
            Me.SharedConfigDirectory = System.Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) & "\" & Me.ProductName.Name
            If Not My.Computer.FileSystem.DirectoryExists(Me.SharedConfigDirectory) Then My.Computer.FileSystem.CreateDirectory(Me.SharedConfigDirectory)
        Catch ex As Exception
            Logger.LogException(NLog.LogLevel.Error, "Failed to create common configuration folder '" & Me.SharedConfigDirectory & "'", ex)
        End Try

        Dim UserSettingsFileName As String = Me.GetUserConfigFileName
        If Not My.Computer.FileSystem.FileExists(UserSettingsFileName) Then
            Dim f As New System.IO.StreamWriter(UserSettingsFileName)
            f.WriteLine("[Log]")
            f.WriteLine("RetainDays=3")
            f.WriteLine("")
            f.Close()
            f = Nothing
        End If

        Dim INI As New IniFile.IniFile
        INI.Load(UserSettingsFileName)

        SANE.Hosts = GetSANEHostsFromINI(INI)
        SANE.CurrentHostIndex = -1
        Dim CurrentHostIndex As Integer = -1
        Dim s As String = INI.GetKeyValue("SANE", "DefaultHost")
        If IsNumeric(s) Then Integer.TryParse(s, CurrentHostIndex)
        If (CurrentHostIndex > -1) And (CurrentHostIndex < SANE.Hosts.Length) Then
            SANE.CurrentHostIndex = CurrentHostIndex
        Else
            If SANE.Hosts.Length > 0 Then SANE.CurrentHostIndex = 0
        End If

        If (SANE.CurrentHostIndex > -1) AndAlso (SANE.CurrentHostIndex < SANE.Hosts.Length) Then
            Logger.Log(NLog.LogLevel.Info, "Current host is '{0}' port '{1}'", SANE.Hosts(SANE.CurrentHostIndex).NameOrAddress, SANE.Hosts(SANE.CurrentHostIndex).Port)
            Logger.Log(NLog.LogLevel.Info, "Default device is '{0}'", SANE.Hosts(SANE.CurrentHostIndex).Device)
            Logger.Log(NLog.LogLevel.Info, "AutoLocateDevice is '{0}'", SANE.Hosts(SANE.CurrentHostIndex).AutoLocateDevice)
        Else
            Logger.Log(NLog.LogLevel.Warn, "No hosts are configured")
        End If
        ''clean up expired log files
        'Dim DebugLogMaxAgeDays As Integer = 0 'never delete
        'Try
        '    s = INI.GetKeyValue("Log", "RetainDays")
        '    If s Is Nothing OrElse s.Length < 1 Then Throw New Exception("RetainDays value is not configured in the [Log] section")
        '    If Integer.TryParse(s, DebugLogMaxAgeDays) Then
        '        If DebugLogMaxAgeDays = 0 Then
        '            Logger.Log(NLog.LogLevel.Info, "Retaining logs forever")
        '        Else
        '            Logger.Log(NLog.LogLevel.Info, "Retaining logs for '{0}'", DebugLogMaxAgeDays)
        '        End If
        '        Logger.Delete_Expired_Logs(DebugLogMaxAgeDays)
        '    Else
        '        Throw New Exception("Unable to interpret '" & s & "' as an integer")
        '    End If
        'Catch ex As Exception
        '    Logger.Write(DebugLogger.Level.Error_, False, "Error reading RetainDays value from '" & UserSettingsFileName & "': " & ex.Message)
        'End Try

    End Sub

    Public Function HostIsValid(ByVal Host As HostInfo) As Boolean
        With Host
            If .NameOrAddress IsNot Nothing AndAlso .NameOrAddress.Length Then
                If .Port > 0 Then
                    If .TCP_Timeout_ms > 1000 Then
                        If .Username IsNot Nothing AndAlso .Username.Length Then
                            Logger.Log(NLog.LogLevel.Debug, "Returning True")
                            Return True
                        End If
                    End If
                End If
            End If
        End With
        Logger.Log(NLog.LogLevel.Debug, "Returning False")
        Return False
    End Function

    Private Function GetSANEHostsFromINI(INI As IniFile.IniFile) As HostInfo()
        Dim hi(-1) As HostInfo
        If INI IsNot Nothing Then
            For idx As Integer = 0 To MAX_HOSTS - 1
                Dim SectionName As String = "Host." & idx.ToString
                If INI.GetSection(SectionName) IsNot Nothing Then
                    Dim NameOrAddress As String = INI.GetKeyValue(SectionName, "NameOrAddress")
                    Dim UseTSClientIP As Boolean
                    Boolean.TryParse(INI.GetKeyValue(SectionName, "UseTSClientIP"), UseTSClientIP)
                    If UseTSClientIP Then
                        Try
                            Dim ts As New TSAPI
                            NameOrAddress = ts.GetCurrentSessionIP
                            ts = Nothing
                        Catch ex As Exception
                            Logger.ErrorException("Error getting terminal server client IP address: " & ex.Message, ex)
                        End Try
                    End If
                    If Not String.IsNullOrEmpty(NameOrAddress) Then
                        Dim Port As Integer = 0
                        Integer.TryParse(INI.GetKeyValue(SectionName, "Port"), Port)
                        If Port = 0 Then Port = 6566
                        If Port > 0 Then
                            ReDim Preserve hi(hi.Count)
                            With hi(hi.Count - 1)
                                .NameOrAddress = NameOrAddress.Trim
                                .UseTSClientIP = UseTSClientIP
                                .Port = Port
                                Integer.TryParse(INI.GetKeyValue(SectionName, "Timeout_ms"), .TCP_Timeout_ms)
                                If .TCP_Timeout_ms = 0 Then .TCP_Timeout_ms = 5000
                                If .TCP_Timeout_ms < 1000 Then .TCP_Timeout_ms = 1000
                                .Username = INI.GetKeyValue(SectionName, "Username")
                                .Password = INI.GetKeyValue(SectionName, "Password")
                                .Device = INI.GetKeyValue(SectionName, "Device")
                                .AutoLocateDevice = INI.GetKeyValue(SectionName, "AutoLocateDevice")
                            End With
                        End If
                    End If
                Else
                    'Exit For
                End If
            Next
        End If
        Return hi
    End Function

    Public Function GetUserConfigFileName() As String
        Return Me.UserConfigDirectory & "\" & Me.ProductName.Name & ".ini"
    End Function

    Public Function GetDeviceConfigFileName() As String
        Dim BackEnd As String = modGlobals.SANE.CurrentDevice.Name
        Dim p As Integer = BackEnd.IndexOf(":")
        If p Then BackEnd = BackEnd.Substring(0, p)
        Dim f As String = CurrentSettings.UserConfigDirectory & "\" & BackEnd & ".ini"
        If My.Computer.FileSystem.FileExists(f) Then
            Return f
        Else
            Dim ff As String = CurrentSettings.SharedConfigDirectory & "\" & BackEnd & ".ini"
            If My.Computer.FileSystem.FileExists(ff) Then
                Return ff
            Else
                CreateDeviceConfigFile(f)
                Dim r As MsgBoxResult = MsgBox("The configuration file '" & BackEnd & ".ini' for the '" & BackEnd & "' backend was not found." _
                    & "  A file containing reasonable defaults has been created in the folder '" & CurrentSettings.UserConfigDirectory & "'." _
                    & "  You will most likely need to modify this new file to take full advantage of your backend, particularly if" _
                    & " you intend to use it through TWAIN.  Once you have tested your configuration, please help other users" _
                    & " by submitting '" & BackEnd & ".ini' back to the project at https://sourceforge.net/p/sanewinds/discussion/backend-ini/." _
                    & "  Would you like to open the backend forum now?" _
                    , MsgBoxStyle.Exclamation + MsgBoxStyle.YesNo)
                If r = MsgBoxResult.Yes Then
                    Try
                        Process.Start("https://sourceforge.net/p/sanewinds/discussion/backend-ini/")
                    Catch ex As Exception
                        Dim msg As String = "Unable to open web page: " & ex.Message
                        Logger.ErrorException(msg, ex)
                        MsgBox(msg, MsgBoxStyle.Critical)
                    End Try
                End If
                Return f
            End If
        End If
    End Function

    Private Function SANE_Option_Defined(OptionName As String) As Boolean
        For i As Integer = 1 To modGlobals.SANE.CurrentDevice.OptionDescriptors.Count - 1 'skip the first option, which is just the option count
            If modGlobals.SANE.CurrentDevice.OptionDescriptors(i).name.ToLower.Trim = OptionName.ToLower.Trim Then
                Return True
            End If
        Next
        Return False
    End Function

    Private Sub CreateDeviceConfigFile(ByVal FileName As String)
        'Create .INI file to hold all settings for SANE backend
        Dim fs As System.IO.StreamWriter = Nothing
        Try
            fs = New System.IO.StreamWriter(FileName)
            fs.WriteLine("[General]")
            fs.WriteLine(";ScanContinuously is a boolean value that determines whether to scan a single page or continue until the ADF is empty.")
            fs.WriteLine(";In most cases the correct value will be guessed automatically.")
            fs.WriteLine(";ScanContinuously=True")
            fs.WriteLine("")
            fs.WriteLine(";MaxPaperWidth and MaxPaperHeight values are in inches and determine the ICAP_SUPPORTEDSIZES values for TWAIN.")
            fs.WriteLine(";These values will be taken from the default br-x and br-y values if not specified here.")
            fs.WriteLine(";MaxPaperWidth=8.5")
            fs.WriteLine(";MaxPaperHeight=14")
            fs.WriteLine("")
            fs.WriteLine(";DefaultPaperSize is the name of the paper size as displayed in the SANEWin GUI.")
            fs.WriteLine(";DefaultPaperSize=Letter")
            fs.WriteLine("")
            For i As Integer = 1 To modGlobals.SANE.CurrentDevice.OptionDescriptors.Count - 1 'skip the first option, which is just the option count
                Select Case modGlobals.SANE.CurrentDevice.OptionDescriptors(i).type
                    Case SANE_API.SANE_Value_Type.SANE_TYPE_GROUP, SANE_API.SANE_Value_Type.SANE_TYPE_BUTTON
                        'no need to map these options
                    Case Else
                        fs.WriteLine("[Option." & modGlobals.SANE.CurrentDevice.OptionDescriptors(i).name & "]")
                        fs.WriteLine(";Name: " & modGlobals.SANE.CurrentDevice.OptionDescriptors(i).name)
                        fs.WriteLine(";Title: " & modGlobals.SANE.CurrentDevice.OptionDescriptors(i).title)
                        fs.WriteLine(";Description: " & modGlobals.SANE.CurrentDevice.OptionDescriptors(i).desc)
                        fs.WriteLine(";Unit: " & modGlobals.SANE.CurrentDevice.OptionDescriptors(i).unit.ToString)
                        fs.WriteLine(";Type: " & modGlobals.SANE.CurrentDevice.OptionDescriptors(i).type.ToString)
                        fs.WriteLine(";Constraint Type: " & modGlobals.SANE.CurrentDevice.OptionDescriptors(i).constraint_type.ToString)

                        Select Case modGlobals.SANE.CurrentDevice.OptionDescriptors(i).constraint_type
                            Case SANE_API.SANE_Constraint_Type.SANE_CONSTRAINT_NONE
                            Case SANE_API.SANE_Constraint_Type.SANE_CONSTRAINT_RANGE
                                fs.WriteLine(";Constraint Values: ")
                                fs.WriteLine(";" & vbTab & "min: " & IIf(modGlobals.SANE.CurrentDevice.OptionDescriptors(i).type = SANE_API.SANE_Value_Type.SANE_TYPE_FIXED, modGlobals.SANE.SANE_UNFIX(modGlobals.SANE.CurrentDevice.OptionDescriptors(i).constraint.range.min).ToString, modGlobals.SANE.CurrentDevice.OptionDescriptors(i).constraint.range.min.ToString))
                                fs.WriteLine(";" & vbTab & "max: " & IIf(modGlobals.SANE.CurrentDevice.OptionDescriptors(i).type = SANE_API.SANE_Value_Type.SANE_TYPE_FIXED, modGlobals.SANE.SANE_UNFIX(modGlobals.SANE.CurrentDevice.OptionDescriptors(i).constraint.range.max).ToString, modGlobals.SANE.CurrentDevice.OptionDescriptors(i).constraint.range.max.ToString))
                                fs.WriteLine(";" & vbTab & "step: " & IIf(modGlobals.SANE.CurrentDevice.OptionDescriptors(i).type = SANE_API.SANE_Value_Type.SANE_TYPE_FIXED, modGlobals.SANE.SANE_UNFIX(modGlobals.SANE.CurrentDevice.OptionDescriptors(i).constraint.range.quant).ToString, modGlobals.SANE.CurrentDevice.OptionDescriptors(i).constraint.range.quant.ToString))
                            Case SANE_API.SANE_Constraint_Type.SANE_CONSTRAINT_STRING_LIST
                                fs.WriteLine(";Constraint Values: ")
                                For j As Integer = 0 To modGlobals.SANE.CurrentDevice.OptionDescriptors(i).constraint.string_list.Count - 1
                                    fs.WriteLine(";" & vbTab & modGlobals.SANE.CurrentDevice.OptionDescriptors(i).constraint.string_list(j))
                                Next
                            Case SANE_API.SANE_Constraint_Type.SANE_CONSTRAINT_WORD_LIST
                                fs.WriteLine(";Constraint Values: ")
                                For j As Integer = 0 To modGlobals.SANE.CurrentDevice.OptionDescriptors(i).constraint.word_list.Count - 1
                                    fs.WriteLine(";" & vbTab & IIf(modGlobals.SANE.CurrentDevice.OptionDescriptors(i).type = SANE_API.SANE_Value_Type.SANE_TYPE_FIXED, modGlobals.SANE.SANE_UNFIX(modGlobals.SANE.CurrentDevice.OptionDescriptors(i).constraint.word_list(j)).ToString, modGlobals.SANE.CurrentDevice.OptionDescriptors(i).constraint.word_list(j).ToString))
                                Next
                        End Select
                        'fs.WriteLine(SANE.CurrentDevice.OptionDescriptors(i).name & "=")
                        fs.WriteLine("DefaultValue=")

                        'Configure TWAIN capability mappings for SANE well-known options
                        Select Case modGlobals.SANE.CurrentDevice.OptionDescriptors(i).name.ToLower
                            Case "resolution"
                                fs.WriteLine("TWAIN=ICAP_XRESOLUTION,#;ICAP_YRESOLUTION,#")
                                If Not SANE_Option_Defined("x-resolution") Then
                                    fs.WriteLine("")
                                    fs.WriteLine("[TWAIN.ICAP_XRESOLUTION]")
                                    fs.WriteLine("SANE=resolution,#")
                                End If
                                If Not SANE_Option_Defined("y-resolution") Then
                                    fs.WriteLine("")
                                    fs.WriteLine("[TWAIN.ICAP_YRESOLUTION]")
                                    fs.WriteLine("SANE=resolution,#")
                                End If
                            Case "x-resolution" 'SANE 2.0 draft
                                fs.WriteLine("TWAIN=ICAP_XRESOLUTION,#")
                                fs.WriteLine("")
                                fs.WriteLine("[TWAIN.ICAP_XRESOLUTION]")
                                fs.WriteLine("SANE=x-resolution,#")
                            Case "y-resolution" 'SANE 2.0 draft
                                fs.WriteLine("TWAIN=ICAP_YRESOLUTION,#")
                                fs.WriteLine("")
                                fs.WriteLine("[TWAIN.ICAP_YRESOLUTION]")
                                fs.WriteLine("SANE=y-resolution,#")
                            Case "preview"
                                'XXX
                            Case "tl-x"
                                'XXX
                            Case "tl-y"
                                'XXX
                            Case "br-x"
                                'XXX
                            Case "br-y"
                                'XXX
                            Case "depth" 'SANE 2.0 draft
                                fs.WriteLine("TWAIN=ICAP_BITDEPTH,#")
                                fs.WriteLine("")
                                fs.WriteLine("[TWAIN.ICAP_BITDEPTH]")
                                fs.WriteLine("SANE=depth,#")
                            Case "mode" 'SANE 2.0 draft
                                If modGlobals.SANE.CurrentDevice.OptionDescriptors(i).constraint_type = SANE_API.SANE_Constraint_Type.SANE_CONSTRAINT_STRING_LIST Then
                                    If modGlobals.SANE.CurrentDevice.OptionDescriptors(i).constraint.string_list.Contains("Lineart") Then
                                        fs.WriteLine("TWAIN.Lineart=ICAP_PIXELTYPE,TWPT_BW;ICAP_BITDEPTH,1")
                                    End If
                                    If modGlobals.SANE.CurrentDevice.OptionDescriptors(i).constraint.string_list.Contains("Gray") Then
                                        If SANE_Option_Defined("depth") Then
                                            fs.WriteLine("TWAIN.Gray=ICAP_PIXELTYPE,TWPT_GRAY")
                                        Else
                                            fs.WriteLine("TWAIN.Gray=ICAP_PIXELTYPE,TWPT_GRAY;ICAP_BITDEPTH,8")
                                        End If
                                    End If
                                    If modGlobals.SANE.CurrentDevice.OptionDescriptors(i).constraint.string_list.Contains("Color") Then
                                        If SANE_Option_Defined("depth") Then
                                            fs.WriteLine("TWAIN.Color=ICAP_PIXELTYPE,TWPT_RGB")
                                        Else
                                            fs.WriteLine("TWAIN.Color=ICAP_PIXELTYPE,TWPT_RGB;ICAP_BITDEPTH,8")
                                        End If
                                    End If
                                    'XXX 'Halftone' is possible also.

                                    fs.WriteLine("")
                                    fs.WriteLine("[TWAIN.ICAP_PIXELTYPE]")
                                    If modGlobals.SANE.CurrentDevice.OptionDescriptors(i).constraint.string_list.Contains("Lineart") Then
                                        fs.WriteLine("SANE.TWPT_BW=mode,Lineart")
                                    End If
                                    If modGlobals.SANE.CurrentDevice.OptionDescriptors(i).constraint.string_list.Contains("Gray") Then
                                        fs.WriteLine("SANE.TWPT_GRAY=mode,Gray")
                                    End If
                                    If modGlobals.SANE.CurrentDevice.OptionDescriptors(i).constraint.string_list.Contains("Color") Then
                                        fs.WriteLine("SANE.TWPT_RGB=mode,Color")
                                    End If
                                    'XXX 'Halftone' is possible also.

                                End If
                            Case "source" 'SANE 2.0 draft
                                If modGlobals.SANE.CurrentDevice.OptionDescriptors(i).constraint_type = SANE_API.SANE_Constraint_Type.SANE_CONSTRAINT_STRING_LIST Then
                                    If modGlobals.SANE.CurrentDevice.OptionDescriptors(i).constraint.string_list.Contains("Flatbed") Then
                                        fs.WriteLine("TWAIN.Flatbed=CAP_FEEDERENABLED,False")
                                    End If
                                    If modGlobals.SANE.CurrentDevice.OptionDescriptors(i).constraint.string_list.Contains("Transparency Adapter") Then
                                        'XXX how does a Transparency Adapter behave?
                                        fs.WriteLine("TWAIN.TransparencyAdapter=CAP_FEEDERENABLED,False")
                                    End If
                                    If modGlobals.SANE.CurrentDevice.OptionDescriptors(i).constraint.string_list.Contains("Automatic Document Feeder") Then
                                        fs.WriteLine("TWAIN.AutomaticDocumentFeeder=CAP_FEEDERENABLED,True")
                                    End If
                                    fs.WriteLine("")
                                    fs.WriteLine("[TWAIN.CAP_FEEDERENABLED]")
                                    If modGlobals.SANE.CurrentDevice.OptionDescriptors(i).constraint.string_list.Contains("Flatbed") Then
                                        fs.WriteLine("SANE.0=source,Flatbed")
                                    End If
                                    'If modGlobals.SANE.CurrentDevice.OptionDescriptors(i).constraint.string_list.Contains("Transparency Adapter") Then
                                    '    'XXX how does a Transparency Adapter behave?
                                    '    fs.WriteLine("SANE.0=source,Flatbed")
                                    'End If
                                    If modGlobals.SANE.CurrentDevice.OptionDescriptors(i).constraint.string_list.Contains("Automatic Document Feeder") Then
                                        fs.WriteLine("SANE.1=source,Automatic Document Feeder")
                                    End If
                                End If
                            Case Else
                                'XXX there are lots of other new well-known options in the SANE 2.0 draft

                        End Select
                End Select
                fs.WriteLine("")
            Next
        Catch ex As Exception
            Logger.LogException(NLog.LogLevel.Error, "Error writing '{0}'", ex)
        Finally
            If fs IsNot Nothing Then fs.Close()
        End Try
        '
    End Sub

    Private Sub InitPageSizes()
        Me.PageSizes.Clear()
        With Me.PageSizes
            .Add(New PageSize(0, 0, "None", TWAIN_VB.TWSS.TWSS_NONE))
            .Add(New PageSize(-1, -1, "Maximum", TWAIN_VB.TWSS.TWSS_MAXSIZE))

            .Add(New PageSize(8.5, 11.0, "Letter", TWAIN_VB.TWSS.TWSS_USLETTER))
            .Add(New PageSize(8.5, 14.0, "Legal", TWAIN_VB.TWSS.TWSS_USLEGAL))
            .Add(New PageSize(11.0, 17.0, "Ledger", TWAIN_VB.TWSS.TWSS_USLEDGER))
            .Add(New PageSize(7.25, 10.5, "Executive", TWAIN_VB.TWSS.TWSS_USEXECUTIVE))
            .Add(New PageSize(5.5, 8.5, "Statement", TWAIN_VB.TWSS.TWSS_USSTATEMENT))
            .Add(New PageSize(2.0, 3.5, "Business Card", TWAIN_VB.TWSS.TWSS_BUSINESSCARD))

            .Add(New PageSize(33.11, 46.81, "A0", TWAIN_VB.TWSS.TWSS_A0))
            .Add(New PageSize(23.39, 33.11, "A1", TWAIN_VB.TWSS.TWSS_A1))
            .Add(New PageSize(16.54, 23.39, "A2", TWAIN_VB.TWSS.TWSS_A2))
            .Add(New PageSize(11.69, 16.54, "A3", TWAIN_VB.TWSS.TWSS_A3))
            .Add(New PageSize(8.27, 11.69, "A4", TWAIN_VB.TWSS.TWSS_A4))
            .Add(New PageSize(5.83, 8.27, "A5", TWAIN_VB.TWSS.TWSS_A5))
            .Add(New PageSize(4.13, 5.83, "A6", TWAIN_VB.TWSS.TWSS_A6))
            .Add(New PageSize(2.91, 4.13, "A7", TWAIN_VB.TWSS.TWSS_A7))
            .Add(New PageSize(2.05, 2.91, "A8", TWAIN_VB.TWSS.TWSS_A8))
            .Add(New PageSize(1.46, 2.05, "A9", TWAIN_VB.TWSS.TWSS_A9))
            .Add(New PageSize(1.02, 1.46, "A10", TWAIN_VB.TWSS.TWSS_A10))

            .Add(New PageSize(39.37, 55.67, "B0", TWAIN_VB.TWSS.TWSS_ISOB0))
            .Add(New PageSize(27.83, 39.37, "B1", TWAIN_VB.TWSS.TWSS_ISOB1))
            .Add(New PageSize(19.69, 27.83, "B2", TWAIN_VB.TWSS.TWSS_ISOB2))
            .Add(New PageSize(13.9, 19.69, "B3", TWAIN_VB.TWSS.TWSS_ISOB3))
            .Add(New PageSize(9.84, 13.9, "B4", TWAIN_VB.TWSS.TWSS_ISOB4))
            .Add(New PageSize(6.93, 9.84, "B5", TWAIN_VB.TWSS.TWSS_ISOB5))
            .Add(New PageSize(4.92, 6.93, "B6", TWAIN_VB.TWSS.TWSS_ISOB6))
            .Add(New PageSize(3.46, 4.92, "B7", TWAIN_VB.TWSS.TWSS_ISOB7))
            .Add(New PageSize(2.44, 3.46, "B8", TWAIN_VB.TWSS.TWSS_ISOB8))
            .Add(New PageSize(1.73, 2.44, "B9", TWAIN_VB.TWSS.TWSS_ISOB9))
            .Add(New PageSize(1.22, 1.73, "B10", TWAIN_VB.TWSS.TWSS_ISOB10))

            .Add(New PageSize(36.1, 51.06, "C0", TWAIN_VB.TWSS.TWSS_C0))
            .Add(New PageSize(25.51, 36.1, "C1", TWAIN_VB.TWSS.TWSS_C1))
            .Add(New PageSize(18.03, 25.51, "C2", TWAIN_VB.TWSS.TWSS_C2))
            .Add(New PageSize(12.76, 18.03, "C3", TWAIN_VB.TWSS.TWSS_C3))
            .Add(New PageSize(9.02, 12.76, "C4", TWAIN_VB.TWSS.TWSS_C4))
            .Add(New PageSize(6.38, 9.02, "C5", TWAIN_VB.TWSS.TWSS_C5))
            .Add(New PageSize(4.49, 6.38, "C6", TWAIN_VB.TWSS.TWSS_C6))
            .Add(New PageSize(3.19, 4.49, "C7", TWAIN_VB.TWSS.TWSS_C7))
            .Add(New PageSize(2.24, 3.19, "C8", TWAIN_VB.TWSS.TWSS_C8))
            .Add(New PageSize(1.57, 2.24, "C9", TWAIN_VB.TWSS.TWSS_C9))
            .Add(New PageSize(1.1, 1.57, "C10", TWAIN_VB.TWSS.TWSS_C10))

            .Add(New PageSize(40.55, 57.32, "JIS B0", TWAIN_VB.TWSS.TWSS_JISB0))
            .Add(New PageSize(28.66, 40.55, "JIS B1", TWAIN_VB.TWSS.TWSS_JISB1))
            .Add(New PageSize(20.28, 28.66, "JIS B2", TWAIN_VB.TWSS.TWSS_JISB2))
            .Add(New PageSize(14.33, 20.28, "JIS B3", TWAIN_VB.TWSS.TWSS_JISB3))
            .Add(New PageSize(10.12, 14.33, "JIS B4", TWAIN_VB.TWSS.TWSS_JISB4))
            .Add(New PageSize(7.17, 10.12, "JIS B5", TWAIN_VB.TWSS.TWSS_JISB5))
            .Add(New PageSize(5.04, 7.17, "JIS B6", TWAIN_VB.TWSS.TWSS_JISB6))
            .Add(New PageSize(3.58, 5.04, "JIS B7", TWAIN_VB.TWSS.TWSS_JISB7))
            .Add(New PageSize(2.52, 3.58, "JIS B8", TWAIN_VB.TWSS.TWSS_JISB8))
            .Add(New PageSize(1.77, 2.52, "JIS B9", TWAIN_VB.TWSS.TWSS_JISB9))
            .Add(New PageSize(1.26, 1.77, "JIS B10", TWAIN_VB.TWSS.TWSS_JISB10))

            .Add(New PageSize(66.22, 93.62, "4A0", TWAIN_VB.TWSS.TWSS_4A0))
            .Add(New PageSize(46.81, 66.22, "2A0", TWAIN_VB.TWSS.TWSS_2A0))
        End With
    End Sub
End Class
