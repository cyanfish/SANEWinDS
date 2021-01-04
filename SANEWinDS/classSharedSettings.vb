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
    Public Enum ConfigFileScope As Integer
        [Shared] = 0
        User = 1
    End Enum
    Public Structure ConfigFile
        Dim [Shared] As String
        Dim User As String
    End Structure
    Public Structure HostInfo
        Dim NameOrAddress As String
        Dim UseTSClientIP As Boolean 'Are we currently using the TS client ip as the NameOrAddress?
        Dim Port As Integer
        Dim Username As String
        Dim Password As String
        Dim TCP_Timeout_ms As Integer
        Dim Image_Timeout_s As Integer
        Dim Open As Boolean
        Dim Device As String
        'Dim DeviceINI As IniFile.IniFile
        Dim DeviceINI As ConfigFile
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
    Private Const Current_INI_Ver = 0.8

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
        Dim UserSettingsFileName As String = Me.GetUserConfigFileName

        WriteIni(UserSettingsFileName, "General", "INI_Version", Current_INI_Ver)
        WriteIni(UserSettingsFileName, "General", "Version", GetType(SANE_API).Assembly.GetName.Version.ToString)

        Dim HostList As String = Nothing
        For idx As Integer = 0 To MAX_HOSTS
            Dim SectionName As String = "Host." & idx.ToString
            RemoveINISection(UserSettingsFileName, SectionName)
            If idx < SANE.Hosts.Length Then
                With SANE.Hosts(idx)
                    WriteIni(UserSettingsFileName, SectionName, "NameOrAddress", .NameOrAddress)
                    WriteIni(UserSettingsFileName, SectionName, "UseTSClientIP", .UseTSClientIP.ToString)
                    WriteIni(UserSettingsFileName, SectionName, "Port", .Port.ToString)
                    WriteIni(UserSettingsFileName, SectionName, "Username", .Username)
                    Dim crypto As New SimpleCrypto
                    Try
                        WriteIni(UserSettingsFileName, SectionName, "Password", crypto.Encrypt(.Password))
                    Catch ex As System.ArgumentNullException
                    Catch ex As Exception
                        Logger.Error(ex, ex.Message)
                    End Try
                    WriteIni(UserSettingsFileName, SectionName, "TCP_Timeout_ms", .TCP_Timeout_ms.ToString)
                    WriteIni(UserSettingsFileName, SectionName, "Image_Timeout_s", .Image_Timeout_s)
                    WriteIni(UserSettingsFileName, SectionName, "Device", .Device)

                    If String.IsNullOrEmpty(.AutoLocateDevice) Then
                        If Not String.IsNullOrEmpty(.Device) Then
                            Dim p As Integer = .Device.IndexOf(":")
                            If p Then
                                .AutoLocateDevice = .Device.Substring(0, p)
                            End If
                        End If
                    End If

                    WriteIni(UserSettingsFileName, SectionName, "AutoLocateDevice", .AutoLocateDevice)
                End With
            End If
        Next

        WriteIni(UserSettingsFileName, "SANE", "DefaultHost", CurrentSettings.SANE.CurrentHostIndex)
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
            Logger.Log(NLog.LogLevel.Error, "Failed to create common configuration folder '" & Me.SharedConfigDirectory & "'", ex)
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


        Dim INI_Version As String = ReadIni(UserSettingsFileName, "General", "INI_Version")
        Dim INI_Ver As Double = 0
        Double.TryParse(INI_Version, INI_Ver)

        Dim SuppressWarning As Boolean = False
        Boolean.TryParse(ReadIni(UserSettingsFileName, "General", "Suppress_Startup_Messages"), SuppressWarning)

        If Not SuppressWarning Then
            Dim App_Version As String = ReadIni(UserSettingsFileName, "General", "Version")
            If App_Version <> GetType(SANE_API).Assembly.GetName.Version.ToString Then
                Dim r As MsgBoxResult = MsgBox("SANEWinDS is hosted by SourceForge.  If you downloaded it from any other site you probably don't have the most recent version.  " _
                    & "The current version is available at http://sourceforge.net/projects/sanewinds/ along with configuration instructions and a forum for " _
                    & "bug reporting, feature requests, and backend.ini contributions.", MsgBoxStyle.Information + MsgBoxStyle.OkOnly, "SANEWinDS is hosted by SourceForge!")
            End If
        End If

        SANE.Hosts = GetSANEHostsFromINI(UserSettingsFileName)

        'increase the timeout for image acquisition.  1200 is the default beginning with INI version 0.8.
        If INI_Ver < 0.8 Then
            For i As Integer = 0 To SANE.Hosts.Length - 1
                SANE.Hosts(i).Image_Timeout_s = 1200
            Next
        End If

        SANE.CurrentHostIndex = -1
        Dim CurrentHostIndex As Integer = -1
        Dim s As String = ReadIni(UserSettingsFileName, "SANE", "DefaultHost")
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

    End Sub

    Public Function ResolveHost(ByRef Host As HostInfo) As Boolean
        With Host
            Logger.Debug("UseTSClientIP is " & .UseTSClientIP.ToString)
            If .UseTSClientIP Then
                Try
                    Dim ts As New TSAPI
                    .NameOrAddress = ts.GetCurrentSessionIP
                    ts = Nothing
                    Logger.Debug("TS Client IP is '" & .NameOrAddress & "'")
                Catch ex As Exception
                    Logger.Error("Error getting terminal server client IP address: " & ex.Message, ex)
                    Return False
                End Try
            End If
            Try
                Dim IPs() As System.Net.IPAddress = System.Net.Dns.GetHostAddresses(.NameOrAddress)
                Logger.Debug("Returning " & (IPs.Length > 0).ToString)
                Return (IPs.Length > 0)
            Catch ex As Exception
                Logger.Error("Error resolving host '" & .NameOrAddress & "': " & ex.Message, ex)
                Return False
            End Try
        End With
    End Function

    Public Function HostIsValid(ByVal Host As HostInfo) As Boolean
        With Host
            If .NameOrAddress IsNot Nothing AndAlso .NameOrAddress.Length Then
                If .Port > 0 Then
                    If .TCP_Timeout_ms > 1000 Then
                        'If .Username IsNot Nothing AndAlso .Username.Length Then
                        Logger.Log(NLog.LogLevel.Debug, "Returning True")
                        Return True
                        'End If
                    End If
                End If
            End If
        End With
        Logger.Log(NLog.LogLevel.Debug, "Returning False")
        Return False
    End Function

    Public Function DeviceIsValid(ByVal Host As HostInfo) As Boolean
        With Host
            Dim Result As Boolean = (Not String.IsNullOrWhiteSpace(Host.Device)) Or (Not String.IsNullOrWhiteSpace(Host.AutoLocateDevice))
            Logger.Debug("Returning " & Result.ToString)
            Return Result
        End With
    End Function

    Private Function GetSANEHostsFromINI(INI As String) As HostInfo()
        Dim hi(-1) As HostInfo
        If INI IsNot Nothing Then
            For idx As Integer = 0 To MAX_HOSTS - 1
                Dim SectionName As String = "Host." & idx.ToString
                If ReadIniSections(INI).Contains(SectionName) Then
                    Dim NameOrAddress As String = ReadIni(INI, SectionName, "NameOrAddress")
                    Dim UseTSClientIP As Boolean
                    Boolean.TryParse(ReadIni(INI, SectionName, "UseTSClientIP"), UseTSClientIP)
                    Dim Port As Integer = 0
                    Integer.TryParse(ReadIni(INI, SectionName, "Port"), Port)
                    If Port = 0 Then Port = 6566
                    If Port > 0 Then
                        ReDim Preserve hi(hi.Count)
                        With hi(hi.Count - 1)
                            If NameOrAddress IsNot Nothing Then .NameOrAddress = NameOrAddress.Trim
                            .UseTSClientIP = UseTSClientIP
                            .Port = Port
                            Integer.TryParse(ReadIni(INI, SectionName, "TCP_Timeout_ms"), .TCP_Timeout_ms)
                            If .TCP_Timeout_ms = 0 Then .TCP_Timeout_ms = 30000
                            If .TCP_Timeout_ms < 1000 Then .TCP_Timeout_ms = 1000

                            Integer.TryParse(ReadIni(INI, SectionName, "Image_Timeout_s"), .Image_Timeout_s)
                            If .Image_Timeout_s = 0 Then .Image_Timeout_s = 1200 '20 minutes
                            If .Image_Timeout_s < 5 Then .Image_Timeout_s = 5 '5 seconds

                            .Username = ReadIni(INI, SectionName, "Username")
                            .Password = ReadIni(INI, SectionName, "Password")
                            Try
                                Dim crypto As New SimpleCrypto
                                If crypto.IsEncrypted(.Password) Then .Password = crypto.Decrypt(.Password)
                            Catch ex As SimpleCryptoExceptions.SuppliedStringNotEncryptedException
                                Logger.Warn(ex.Message, ex)
                            Catch ex As Exception
                                Logger.Error(ex, ex.Message)
                            End Try
                            .Device = ReadIni(INI, SectionName, "Device")
                            .AutoLocateDevice = ReadIni(INI, SectionName, "AutoLocateDevice")
                        End With
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

    Public Function GetDeviceConfigFileName(Scope As ConfigFileScope) As String
        Dim BackEnd As String = modGlobals.SANE.CurrentDevice.Name
        Dim p As Integer = BackEnd.IndexOf(":")
        If p Then BackEnd = BackEnd.Substring(0, p)
        Dim f As String = CurrentSettings.UserConfigDirectory & "\" & BackEnd & ".ini"
        Dim ff As String = CurrentSettings.SharedConfigDirectory & "\" & BackEnd & ".ini"
        If Scope = ConfigFileScope.User Then
            If My.Computer.FileSystem.FileExists(f) Then
                Return f
            Else
                If My.Computer.FileSystem.FileExists(ff) Then
                    Return Nothing 'We have a shared backend.ini we can use.  No need to auto-create a user backend.ini.
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
                            Logger.Error(msg, ex)
                            MsgBox(msg, MsgBoxStyle.Critical)
                        End Try
                    End If
                    Return f
                End If
            End If
        Else
            If My.Computer.FileSystem.FileExists(ff) Then
                Return ff
            Else
                Return Nothing
            End If
        End If
    End Function

    Private Function SANE_Option_Defined(OptionName As String) As Boolean
        For i As Integer = 1 To modGlobals.SANE.CurrentDevice.OptionDescriptors.Count - 1 'skip the first option, which is just the option count
            If Not String.IsNullOrEmpty(modGlobals.SANE.CurrentDevice.OptionDescriptors(i).name) Then
                If modGlobals.SANE.CurrentDevice.OptionDescriptors(i).name.ToLower.Trim = OptionName.ToLower.Trim Then
                    Return True
                End If
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
            fs.WriteLine(";DefaultPaperSize is the name of the paper size as displayed in the SANEWinDS GUI.")
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
            Logger.Log(NLog.LogLevel.Error, "Error writing '{0}'", ex)
        Finally
            If fs IsNot Nothing Then fs.Close()
        End Try
        '
    End Sub

    Public Function GetINIKeyValue(Section As String, Key As String, Preferred_INIFile As String, Alternate_INIFile As String) As String
        Dim Result As String = Nothing
        If Preferred_INIFile IsNot Nothing Then
            Result = ReadIni(Preferred_INIFile, Section, Key)
        End If
        If Result IsNot Nothing Then 'The user may have intentionally specified an empty string as a value, so look elsewhere only if it's Nothing.
            Return Result
        Else
            If Alternate_INIFile IsNot Nothing Then
                Result = ReadIni(Alternate_INIFile, Section, Key)
                If Result IsNot Nothing Then Return Result
            End If
        End If
        Return Nothing
    End Function

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

            .Add(New PageSize(1682 / 25.4, 2378 / 25.4, "4A0", TWAIN_VB.TWSS.TWSS_4A0))
            .Add(New PageSize(1189 / 25.4, 1682 / 25.4, "2A0", TWAIN_VB.TWSS.TWSS_2A0))
            .Add(New PageSize(841 / 25.4, 1189 / 25.4, "A0", TWAIN_VB.TWSS.TWSS_A0))
            .Add(New PageSize(594 / 25.4, 841 / 25.4, "A1", TWAIN_VB.TWSS.TWSS_A1))
            .Add(New PageSize(420 / 25.4, 594 / 25.4, "A2", TWAIN_VB.TWSS.TWSS_A2))
            .Add(New PageSize(297 / 25.4, 420 / 25.4, "A3", TWAIN_VB.TWSS.TWSS_A3))
            .Add(New PageSize(210 / 25.4, 297 / 25.4, "A4", TWAIN_VB.TWSS.TWSS_A4))
            .Add(New PageSize(148 / 25.4, 210 / 25.4, "A5", TWAIN_VB.TWSS.TWSS_A5))
            .Add(New PageSize(105 / 25.4, 148 / 25.4, "A6", TWAIN_VB.TWSS.TWSS_A6))
            .Add(New PageSize(74 / 25.4, 105 / 25.4, "A7", TWAIN_VB.TWSS.TWSS_A7))
            .Add(New PageSize(52 / 25.4, 74 / 25.4, "A8", TWAIN_VB.TWSS.TWSS_A8))
            .Add(New PageSize(37 / 25.4, 52 / 25.4, "A9", TWAIN_VB.TWSS.TWSS_A9))
            .Add(New PageSize(26 / 25.4, 37 / 25.4, "A10", TWAIN_VB.TWSS.TWSS_A10))

            .Add(New PageSize(1000 / 25.4, 1414 / 25.4, "B0", TWAIN_VB.TWSS.TWSS_ISOB0))
            .Add(New PageSize(707 / 25.4, 1000 / 25.4, "B1", TWAIN_VB.TWSS.TWSS_ISOB1))
            .Add(New PageSize(500 / 25.4, 707 / 25.4, "B2", TWAIN_VB.TWSS.TWSS_ISOB2))
            .Add(New PageSize(353 / 25.4, 500 / 25.4, "B3", TWAIN_VB.TWSS.TWSS_ISOB3))
            .Add(New PageSize(250 / 25.4, 353 / 25.4, "B4", TWAIN_VB.TWSS.TWSS_ISOB4))
            .Add(New PageSize(176 / 25.4, 250 / 25.4, "B5", TWAIN_VB.TWSS.TWSS_ISOB5))
            .Add(New PageSize(125 / 25.4, 176.25 / 4, "B6", TWAIN_VB.TWSS.TWSS_ISOB6))
            .Add(New PageSize(88 / 25.4, 125 / 25.4, "B7", TWAIN_VB.TWSS.TWSS_ISOB7))
            .Add(New PageSize(62 / 25.4, 88 / 25.4, "B8", TWAIN_VB.TWSS.TWSS_ISOB8))
            .Add(New PageSize(44 / 25.4, 62 / 25.4, "B9", TWAIN_VB.TWSS.TWSS_ISOB9))
            .Add(New PageSize(31 / 25.4, 44 / 25.4, "B10", TWAIN_VB.TWSS.TWSS_ISOB10))

            .Add(New PageSize(917 / 25.4, 1297 / 25.4, "C0", TWAIN_VB.TWSS.TWSS_C0))
            .Add(New PageSize(648 / 25.4, 917 / 25.4, "C1", TWAIN_VB.TWSS.TWSS_C1))
            .Add(New PageSize(458 / 25.4, 648 / 25.4, "C2", TWAIN_VB.TWSS.TWSS_C2))
            .Add(New PageSize(324 / 25.4, 458 / 25.4, "C3", TWAIN_VB.TWSS.TWSS_C3))
            .Add(New PageSize(229 / 25.4, 324 / 25.4, "C4", TWAIN_VB.TWSS.TWSS_C4))
            .Add(New PageSize(162 / 25.4, 229 / 25.4, "C5", TWAIN_VB.TWSS.TWSS_C5))
            .Add(New PageSize(114 / 25.4, 162 / 25.4, "C6", TWAIN_VB.TWSS.TWSS_C6))
            .Add(New PageSize(81 / 25.4, 114 / 25.4, "C7", TWAIN_VB.TWSS.TWSS_C7))
            .Add(New PageSize(57 / 25.4, 81 / 25.4, "C8", TWAIN_VB.TWSS.TWSS_C8))
            .Add(New PageSize(40 / 25.4, 57 / 25.4, "C9", TWAIN_VB.TWSS.TWSS_C9))
            .Add(New PageSize(28 / 25.4, 40 / 25.4, "C10", TWAIN_VB.TWSS.TWSS_C10))

            .Add(New PageSize(1030 / 25.4, 1456 / 25.4, "JIS B0", TWAIN_VB.TWSS.TWSS_JISB0))
            .Add(New PageSize(728 / 25.4, 1030 / 25.4, "JIS B1", TWAIN_VB.TWSS.TWSS_JISB1))
            .Add(New PageSize(515 / 25.4, 728 / 25.4, "JIS B2", TWAIN_VB.TWSS.TWSS_JISB2))
            .Add(New PageSize(364 / 25.4, 515 / 25.4, "JIS B3", TWAIN_VB.TWSS.TWSS_JISB3))
            .Add(New PageSize(257 / 25.4, 364 / 25.4, "JIS B4", TWAIN_VB.TWSS.TWSS_JISB4))
            .Add(New PageSize(182 / 25.4, 257 / 25.4, "JIS B5", TWAIN_VB.TWSS.TWSS_JISB5))
            .Add(New PageSize(128 / 25.4, 182 / 25.4, "JIS B6", TWAIN_VB.TWSS.TWSS_JISB6))
            .Add(New PageSize(91 / 25.4, 128 / 25.4, "JIS B7", TWAIN_VB.TWSS.TWSS_JISB7))
            .Add(New PageSize(64 / 25.4, 91 / 254.4, "JIS B8", TWAIN_VB.TWSS.TWSS_JISB8))
            .Add(New PageSize(45 / 25.4, 64 / 25.4, "JIS B9", TWAIN_VB.TWSS.TWSS_JISB9))
            .Add(New PageSize(32 / 25.4, 45 / 25.4, "JIS B10", TWAIN_VB.TWSS.TWSS_JISB10))

        End With
    End Sub
End Class
