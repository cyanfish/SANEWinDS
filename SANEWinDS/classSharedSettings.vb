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
Public Class SharedSettings

    Private Logger As NLog.Logger = NLog.LogManager.GetCurrentClassLogger()

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
        Dim DeviceINI As ConfigFile
        Dim AutoLocateDevice As String 'SANE backend name of device to auto-choose from list of devices on CurrentHost (example: "canon_dr")
    End Structure
    Public Structure SANESettings
        Dim Hosts() As HostInfo
        Dim CurrentHostIndex As Integer
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
    Public SaveDefaultsOnExit As Boolean
    Public SANE As SANESettings
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
        WriteIni(UserSettingsFileName, "SANE", "DefaultHost", Me.SANE.CurrentHostIndex)
        WriteIni(UserSettingsFileName, "General", "SaveDefaultsOnExit", Me.SaveDefaultsOnExit)
    End Sub

    Private Sub ReadSettings()
        SANE = New SANESettings

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
            Logger.Error(ex, "Failed to create common configuration folder '" & Me.SharedConfigDirectory & "'")
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

        Boolean.TryParse(ReadIni(UserSettingsFileName, "General", "SaveDefaultsOnExit"), Me.SaveDefaultsOnExit)

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
                    Logger.Error(ex, "Error getting terminal server client IP address")
                    Return False
                End Try
            End If
            Try
                Dim IPs() As System.Net.IPAddress = System.Net.Dns.GetHostAddresses(.NameOrAddress)
                Logger.Debug("Returning " & (IPs.Length > 0).ToString)
                Return (IPs.Length > 0)
            Catch ex As Exception
                Logger.Error(ex, "Error resolving host '" & .NameOrAddress & "'")
                Return False
            End Try
        End With
    End Function

    Public Function HostIsValid(ByVal Host As HostInfo) As Boolean
        With Host
            If .NameOrAddress IsNot Nothing AndAlso .NameOrAddress.Length Then
                If .Port > 0 Then
                    If .TCP_Timeout_ms > 1000 Then
                        Logger.Log(NLog.LogLevel.Debug, "Returning True")
                        Return True
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
                    If NameOrAddress Is Nothing Then NameOrAddress = "localhost"
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

    Public Function GetSavedOptionValueSetNames() As ArrayList
        Dim SetNames As New ArrayList
        Try
            Dim BackEnd As String = modGlobals.SANE.CurrentDevice.Name
            Dim p As Integer = BackEnd.IndexOf(":")
            If p Then BackEnd = BackEnd.Substring(0, p)
            Dim s As String = BackEnd & ".*.ini"
            For Each f As String In My.Computer.FileSystem.GetFiles(Me.UserConfigDirectory, FileIO.SearchOption.SearchTopLevelOnly, s)
                Dim ff As String = My.Computer.FileSystem.GetName(f)
                Dim ss As String = ff.Replace(BackEnd & ".", "")
                Dim SetName As String = Strings.Replace(ss, ".ini", "",,, CompareMethod.Text)
                If Not String.IsNullOrWhiteSpace(SetName) Then SetNames.Add(SetName)
            Next
        Catch ex As Exception
            Logger.Error(ex)
        End Try
        Return SetNames
    End Function

    Public Function GetUserConfigFileName() As String
        Return Me.UserConfigDirectory & "\" & Me.ProductName.Name & ".ini"
    End Function

    Public Function GetDeviceConfigFileName(Scope As ConfigFileScope) As String
        Return GetDeviceConfigFileName(Scope, Nothing, False, False)
    End Function
    Public Function GetDeviceConfigFileName(Scope As ConfigFileScope, OptionValueSetName As String, ForceCreateUserConfig As Boolean, SuppressNotifications As Boolean) As String
        'ForceCreateUserConfig=True will create a user config file even if a shared config file exists.
        Dim BackEnd As String = modGlobals.SANE.CurrentDevice.Name
        Dim p As Integer = BackEnd.IndexOf(":")
        If p Then BackEnd = BackEnd.Substring(0, p)
        Dim f As String = Me.UserConfigDirectory & "\" & BackEnd & IIf(OptionValueSetName Is Nothing, "", "." & OptionValueSetName) & ".ini"
        Dim ff As String = Me.SharedConfigDirectory & "\" & BackEnd & ".ini"
        If Scope = ConfigFileScope.User Then
            If My.Computer.FileSystem.FileExists(f) Then
                Return f
            Else
                If My.Computer.FileSystem.FileExists(ff) And (Not ForceCreateUserConfig) Then
                    Return Nothing 'We have a shared backend.ini we can use.  No need to auto-create a user backend.ini.
                Else
                    CreateDeviceConfigFile(f, My.Computer.FileSystem.FileExists(ff)) 'if we have a shared backend.ini, create the user backend.ini with all values commented out.
                    If (Not SuppressNotifications) And (OptionValueSetName Is Nothing) Then
                        Dim r As MsgBoxResult = MsgBox("The configuration file '" & BackEnd & ".ini' for the '" & BackEnd & "' backend was not found." _
                            & "  A file containing reasonable defaults has been created in the folder '" & Me.UserConfigDirectory & "'." _
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
                                Logger.Error(ex, msg)
                                MsgBox(msg, MsgBoxStyle.Critical)
                            End Try
                        End If
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

    Private Sub WriteDeviceConfigLine(ByRef Writer As System.IO.StreamWriter, ByVal Text As String)
        WriteDeviceConfigLine(Writer, Text, False)
    End Sub
    Private Sub WriteDeviceConfigLine(ByRef Writer As System.IO.StreamWriter, ByVal Text As String, ByVal WriteAsComment As Boolean)
        If Not ((Not String.IsNullOrEmpty(Text)) AndAlso Text.Substring(0, 1) = ";") Then
            If WriteAsComment Then Text = ";" & Text
        End If
        Writer.WriteLine(Text)
    End Sub

    Private Sub CreateDeviceConfigFile(ByVal FileName As String)
        CreateDeviceConfigFile(FileName, False)
    End Sub
    Private Sub CreateDeviceConfigFile(ByVal FileName As String, ByVal WriteValuesAsComments As Boolean)
        'Create .INI file to hold all settings for SANE backend
        Dim fs As System.IO.StreamWriter = Nothing
        Try
            fs = New System.IO.StreamWriter(FileName)
            WriteDeviceConfigLine(fs, "[General]")
            WriteDeviceConfigLine(fs, ";ScanContinuously is a boolean value that determines whether to scan a single page or continue until the ADF is empty.")
            WriteDeviceConfigLine(fs, ";In most cases the correct value will be guessed automatically.")
            WriteDeviceConfigLine(fs, ";ScanContinuously=True")
            WriteDeviceConfigLine(fs, "")
            WriteDeviceConfigLine(fs, ";MaxPaperWidth and MaxPaperHeight values are in inches and determine the ICAP_SUPPORTEDSIZES values for TWAIN.")
            WriteDeviceConfigLine(fs, ";These values will be taken from the default br-x and br-y values if not specified here.")
            WriteDeviceConfigLine(fs, ";MaxPaperWidth=8.5")
            WriteDeviceConfigLine(fs, ";MaxPaperHeight=14")
            WriteDeviceConfigLine(fs, "")
            WriteDeviceConfigLine(fs, ";DefaultPaperSize is the name of the paper size as displayed in the SANEWinDS GUI.")
            WriteDeviceConfigLine(fs, ";DefaultPaperSize=Letter")
            WriteDeviceConfigLine(fs, "")
            For i As Integer = 1 To modGlobals.SANE.CurrentDevice.OptionDescriptors.Count - 1 'skip the first option, which is just the option count
                Select Case modGlobals.SANE.CurrentDevice.OptionDescriptors(i).type
                    Case SANE_API.SANE_Value_Type.SANE_TYPE_GROUP, SANE_API.SANE_Value_Type.SANE_TYPE_BUTTON
                        'no need to map these options
                    Case Else
                        WriteDeviceConfigLine(fs, "[Option." & modGlobals.SANE.CurrentDevice.OptionDescriptors(i).name & "]")
                        WriteDeviceConfigLine(fs, ";Name: " & modGlobals.SANE.CurrentDevice.OptionDescriptors(i).name)
                        WriteDeviceConfigLine(fs, ";Title: " & modGlobals.SANE.CurrentDevice.OptionDescriptors(i).title)
                        WriteDeviceConfigLine(fs, ";Description: " & modGlobals.SANE.CurrentDevice.OptionDescriptors(i).desc)
                        WriteDeviceConfigLine(fs, ";Unit: " & modGlobals.SANE.CurrentDevice.OptionDescriptors(i).unit.ToString)
                        WriteDeviceConfigLine(fs, ";Type: " & modGlobals.SANE.CurrentDevice.OptionDescriptors(i).type.ToString)
                        WriteDeviceConfigLine(fs, ";Constraint Type: " & modGlobals.SANE.CurrentDevice.OptionDescriptors(i).constraint_type.ToString)

                        Select Case modGlobals.SANE.CurrentDevice.OptionDescriptors(i).constraint_type
                            Case SANE_API.SANE_Constraint_Type.SANE_CONSTRAINT_NONE
                            Case SANE_API.SANE_Constraint_Type.SANE_CONSTRAINT_RANGE
                                WriteDeviceConfigLine(fs, ";Constraint Values: ")
                                WriteDeviceConfigLine(fs, ";" & vbTab & "min: " & IIf(modGlobals.SANE.CurrentDevice.OptionDescriptors(i).type = SANE_API.SANE_Value_Type.SANE_TYPE_FIXED, modGlobals.SANE.SANE_UNFIX(modGlobals.SANE.CurrentDevice.OptionDescriptors(i).constraint.range.min).ToString, modGlobals.SANE.CurrentDevice.OptionDescriptors(i).constraint.range.min.ToString))
                                WriteDeviceConfigLine(fs, ";" & vbTab & "max: " & IIf(modGlobals.SANE.CurrentDevice.OptionDescriptors(i).type = SANE_API.SANE_Value_Type.SANE_TYPE_FIXED, modGlobals.SANE.SANE_UNFIX(modGlobals.SANE.CurrentDevice.OptionDescriptors(i).constraint.range.max).ToString, modGlobals.SANE.CurrentDevice.OptionDescriptors(i).constraint.range.max.ToString))
                                WriteDeviceConfigLine(fs, ";" & vbTab & "step: " & IIf(modGlobals.SANE.CurrentDevice.OptionDescriptors(i).type = SANE_API.SANE_Value_Type.SANE_TYPE_FIXED, modGlobals.SANE.SANE_UNFIX(modGlobals.SANE.CurrentDevice.OptionDescriptors(i).constraint.range.quant).ToString, modGlobals.SANE.CurrentDevice.OptionDescriptors(i).constraint.range.quant.ToString))
                            Case SANE_API.SANE_Constraint_Type.SANE_CONSTRAINT_STRING_LIST
                                WriteDeviceConfigLine(fs, ";Constraint Values: ")
                                For j As Integer = 0 To modGlobals.SANE.CurrentDevice.OptionDescriptors(i).constraint.string_list.Count - 1
                                    WriteDeviceConfigLine(fs, ";" & vbTab & modGlobals.SANE.CurrentDevice.OptionDescriptors(i).constraint.string_list(j))
                                Next
                            Case SANE_API.SANE_Constraint_Type.SANE_CONSTRAINT_WORD_LIST
                                WriteDeviceConfigLine(fs, ";Constraint Values: ")
                                For j As Integer = 0 To modGlobals.SANE.CurrentDevice.OptionDescriptors(i).constraint.word_list.Count - 1
                                    WriteDeviceConfigLine(fs, ";" & vbTab & IIf(modGlobals.SANE.CurrentDevice.OptionDescriptors(i).type = SANE_API.SANE_Value_Type.SANE_TYPE_FIXED, modGlobals.SANE.SANE_UNFIX(modGlobals.SANE.CurrentDevice.OptionDescriptors(i).constraint.word_list(j)).ToString, modGlobals.SANE.CurrentDevice.OptionDescriptors(i).constraint.word_list(j).ToString))
                                Next
                        End Select

                        WriteDeviceConfigLine(fs, "DefaultValue=", WriteValuesAsComments)

                        'Configure TWAIN capability mappings for SANE well-known options
                        Select Case modGlobals.SANE.CurrentDevice.OptionDescriptors(i).name.ToLower
                            Case "resolution"
                                WriteDeviceConfigLine(fs, "TWAIN=ICAP_XRESOLUTION,#;ICAP_YRESOLUTION,#", WriteValuesAsComments)
                                If Not SANE_Option_Defined("x-resolution") Then
                                    WriteDeviceConfigLine(fs, "")
                                    WriteDeviceConfigLine(fs, "[TWAIN.ICAP_XRESOLUTION]")
                                    WriteDeviceConfigLine(fs, "SANE=resolution,#", WriteValuesAsComments)
                                End If
                                If Not SANE_Option_Defined("y-resolution") Then
                                    WriteDeviceConfigLine(fs, "")
                                    WriteDeviceConfigLine(fs, "[TWAIN.ICAP_YRESOLUTION]")
                                    WriteDeviceConfigLine(fs, "SANE=resolution,#", WriteValuesAsComments)
                                End If
                            Case "x-resolution" 'SANE 2.0 draft
                                WriteDeviceConfigLine(fs, "TWAIN=ICAP_XRESOLUTION,#", WriteValuesAsComments)
                                WriteDeviceConfigLine(fs, "")
                                WriteDeviceConfigLine(fs, "[TWAIN.ICAP_XRESOLUTION]")
                                WriteDeviceConfigLine(fs, "SANE=x-resolution,#", WriteValuesAsComments)
                            Case "y-resolution" 'SANE 2.0 draft
                                WriteDeviceConfigLine(fs, "TWAIN=ICAP_YRESOLUTION,#", WriteValuesAsComments)
                                WriteDeviceConfigLine(fs, "")
                                WriteDeviceConfigLine(fs, "[TWAIN.ICAP_YRESOLUTION]")
                                WriteDeviceConfigLine(fs, "SANE=y-resolution,#", WriteValuesAsComments)
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
                                WriteDeviceConfigLine(fs, "TWAIN=ICAP_BITDEPTH,#", WriteValuesAsComments)
                                WriteDeviceConfigLine(fs, "")
                                WriteDeviceConfigLine(fs, "[TWAIN.ICAP_BITDEPTH]")
                                WriteDeviceConfigLine(fs, "SANE=depth,#", WriteValuesAsComments)
                            Case "mode" 'SANE 2.0 draft
                                If modGlobals.SANE.CurrentDevice.OptionDescriptors(i).constraint_type = SANE_API.SANE_Constraint_Type.SANE_CONSTRAINT_STRING_LIST Then
                                    If modGlobals.SANE.CurrentDevice.OptionDescriptors(i).constraint.string_list.Contains("Lineart") Then
                                        WriteDeviceConfigLine(fs, "TWAIN.Lineart=ICAP_PIXELTYPE,TWPT_BW;ICAP_BITDEPTH,1", WriteValuesAsComments)
                                    End If
                                    If modGlobals.SANE.CurrentDevice.OptionDescriptors(i).constraint.string_list.Contains("Gray") Then
                                        If SANE_Option_Defined("depth") Then
                                            WriteDeviceConfigLine(fs, "TWAIN.Gray=ICAP_PIXELTYPE,TWPT_GRAY", WriteValuesAsComments)
                                        Else
                                            WriteDeviceConfigLine(fs, "TWAIN.Gray=ICAP_PIXELTYPE,TWPT_GRAY;ICAP_BITDEPTH,8", WriteValuesAsComments)
                                        End If
                                    End If
                                    If modGlobals.SANE.CurrentDevice.OptionDescriptors(i).constraint.string_list.Contains("Color") Then
                                        If SANE_Option_Defined("depth") Then
                                            WriteDeviceConfigLine(fs, "TWAIN.Color=ICAP_PIXELTYPE,TWPT_RGB", WriteValuesAsComments)
                                        Else
                                            WriteDeviceConfigLine(fs, "TWAIN.Color=ICAP_PIXELTYPE,TWPT_RGB;ICAP_BITDEPTH,8", WriteValuesAsComments)
                                        End If
                                    End If
                                    'XXX 'Halftone' is possible also.

                                    WriteDeviceConfigLine(fs, "")
                                    WriteDeviceConfigLine(fs, "[TWAIN.ICAP_PIXELTYPE]")
                                    If modGlobals.SANE.CurrentDevice.OptionDescriptors(i).constraint.string_list.Contains("Lineart") Then
                                        WriteDeviceConfigLine(fs, "SANE.TWPT_BW=mode,Lineart", WriteValuesAsComments)
                                    End If
                                    If modGlobals.SANE.CurrentDevice.OptionDescriptors(i).constraint.string_list.Contains("Gray") Then
                                        WriteDeviceConfigLine(fs, "SANE.TWPT_GRAY=mode,Gray", WriteValuesAsComments)
                                    End If
                                    If modGlobals.SANE.CurrentDevice.OptionDescriptors(i).constraint.string_list.Contains("Color") Then
                                        WriteDeviceConfigLine(fs, "SANE.TWPT_RGB=mode,Color", WriteValuesAsComments)
                                    End If
                                    'XXX 'Halftone' is possible also.

                                End If
                            Case "source" 'SANE 2.0 draft
                                If modGlobals.SANE.CurrentDevice.OptionDescriptors(i).constraint_type = SANE_API.SANE_Constraint_Type.SANE_CONSTRAINT_STRING_LIST Then
                                    If modGlobals.SANE.CurrentDevice.OptionDescriptors(i).constraint.string_list.Contains("Flatbed") Then
                                        WriteDeviceConfigLine(fs, "TWAIN.Flatbed=CAP_FEEDERENABLED,False", WriteValuesAsComments)
                                    End If
                                    If modGlobals.SANE.CurrentDevice.OptionDescriptors(i).constraint.string_list.Contains("Transparency Adapter") Then
                                        'XXX how does a Transparency Adapter behave?
                                        WriteDeviceConfigLine(fs, "TWAIN.TransparencyAdapter=CAP_FEEDERENABLED,False", WriteValuesAsComments)
                                    End If
                                    If modGlobals.SANE.CurrentDevice.OptionDescriptors(i).constraint.string_list.Contains("Automatic Document Feeder") Then
                                        WriteDeviceConfigLine(fs, "TWAIN.AutomaticDocumentFeeder=CAP_FEEDERENABLED,True", WriteValuesAsComments)
                                    End If
                                    WriteDeviceConfigLine(fs, "")
                                    WriteDeviceConfigLine(fs, "[TWAIN.CAP_FEEDERENABLED]")
                                    If modGlobals.SANE.CurrentDevice.OptionDescriptors(i).constraint.string_list.Contains("Flatbed") Then
                                        WriteDeviceConfigLine(fs, "SANE.0=source,Flatbed", WriteValuesAsComments)
                                    End If

                                    If modGlobals.SANE.CurrentDevice.OptionDescriptors(i).constraint.string_list.Contains("Automatic Document Feeder") Then
                                        WriteDeviceConfigLine(fs, "SANE.1=source,Automatic Document Feeder", WriteValuesAsComments)
                                    End If
                                End If
                            Case Else
                                'XXX there are lots of other new well-known options in the SANE 2.0 draft

                        End Select
                End Select
                WriteDeviceConfigLine(fs, "")
            Next
        Catch ex As Exception
            Logger.Error(ex, "Error writing '{0}'")
        Finally
            If fs IsNot Nothing Then fs.Close()
        End Try
        '
    End Sub

    Public Function GetINIKeyValue(Section As String, Key As String, Preferred_INIFile As String, Optional Alternate_INIFile As String = Nothing) As String
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

            .Add(New PageSize(MMToInches(1682), MMToInches(2378), "4A0", TWAIN_VB.TWSS.TWSS_4A0))
            .Add(New PageSize(MMToInches(1189), MMToInches(1682), "2A0", TWAIN_VB.TWSS.TWSS_2A0))
            .Add(New PageSize(MMToInches(841), MMToInches(1189), "A0", TWAIN_VB.TWSS.TWSS_A0))
            .Add(New PageSize(MMToInches(594), MMToInches(841), "A1", TWAIN_VB.TWSS.TWSS_A1))
            .Add(New PageSize(MMToInches(420), MMToInches(594), "A2", TWAIN_VB.TWSS.TWSS_A2))
            .Add(New PageSize(MMToInches(297), MMToInches(420), "A3", TWAIN_VB.TWSS.TWSS_A3))
            .Add(New PageSize(MMToInches(210), MMToInches(297), "A4", TWAIN_VB.TWSS.TWSS_A4))
            .Add(New PageSize(MMToInches(148), MMToInches(210), "A5", TWAIN_VB.TWSS.TWSS_A5))
            .Add(New PageSize(MMToInches(105), MMToInches(148), "A6", TWAIN_VB.TWSS.TWSS_A6))
            .Add(New PageSize(MMToInches(74), MMToInches(105), "A7", TWAIN_VB.TWSS.TWSS_A7))
            .Add(New PageSize(MMToInches(52), MMToInches(74), "A8", TWAIN_VB.TWSS.TWSS_A8))
            .Add(New PageSize(MMToInches(37), MMToInches(52), "A9", TWAIN_VB.TWSS.TWSS_A9))
            .Add(New PageSize(MMToInches(26), MMToInches(37), "A10", TWAIN_VB.TWSS.TWSS_A10))

            .Add(New PageSize(MMToInches(1000), MMToInches(1414), "B0", TWAIN_VB.TWSS.TWSS_ISOB0))
            .Add(New PageSize(MMToInches(707), MMToInches(1000), "B1", TWAIN_VB.TWSS.TWSS_ISOB1))
            .Add(New PageSize(MMToInches(500), MMToInches(707), "B2", TWAIN_VB.TWSS.TWSS_ISOB2))
            .Add(New PageSize(MMToInches(353), MMToInches(500), "B3", TWAIN_VB.TWSS.TWSS_ISOB3))
            .Add(New PageSize(MMToInches(250), MMToInches(353), "B4", TWAIN_VB.TWSS.TWSS_ISOB4))
            .Add(New PageSize(MMToInches(176), MMToInches(250), "B5", TWAIN_VB.TWSS.TWSS_ISOB5))
            .Add(New PageSize(MMToInches(125), MMToInches(176), "B6", TWAIN_VB.TWSS.TWSS_ISOB6))
            .Add(New PageSize(MMToInches(88), MMToInches(125), "B7", TWAIN_VB.TWSS.TWSS_ISOB7))
            .Add(New PageSize(MMToInches(62), MMToInches(88), "B8", TWAIN_VB.TWSS.TWSS_ISOB8))
            .Add(New PageSize(MMToInches(44), MMToInches(62), "B9", TWAIN_VB.TWSS.TWSS_ISOB9))
            .Add(New PageSize(MMToInches(31), MMToInches(44), "B10", TWAIN_VB.TWSS.TWSS_ISOB10))

            .Add(New PageSize(MMToInches(917), MMToInches(1297), "C0", TWAIN_VB.TWSS.TWSS_C0))
            .Add(New PageSize(MMToInches(648), MMToInches(917), "C1", TWAIN_VB.TWSS.TWSS_C1))
            .Add(New PageSize(MMToInches(458), MMToInches(648), "C2", TWAIN_VB.TWSS.TWSS_C2))
            .Add(New PageSize(MMToInches(324), MMToInches(458), "C3", TWAIN_VB.TWSS.TWSS_C3))
            .Add(New PageSize(MMToInches(229), MMToInches(324), "C4", TWAIN_VB.TWSS.TWSS_C4))
            .Add(New PageSize(MMToInches(162), MMToInches(229), "C5", TWAIN_VB.TWSS.TWSS_C5))
            .Add(New PageSize(MMToInches(114), MMToInches(162), "C6", TWAIN_VB.TWSS.TWSS_C6))
            .Add(New PageSize(MMToInches(81), MMToInches(114), "C7", TWAIN_VB.TWSS.TWSS_C7))
            .Add(New PageSize(MMToInches(57), MMToInches(81), "C8", TWAIN_VB.TWSS.TWSS_C8))
            .Add(New PageSize(MMToInches(40), MMToInches(57), "C9", TWAIN_VB.TWSS.TWSS_C9))
            .Add(New PageSize(MMToInches(28), MMToInches(40), "C10", TWAIN_VB.TWSS.TWSS_C10))

            .Add(New PageSize(MMToInches(1030), MMToInches(1456), "JIS B0", TWAIN_VB.TWSS.TWSS_JISB0))
            .Add(New PageSize(MMToInches(728), MMToInches(1030), "JIS B1", TWAIN_VB.TWSS.TWSS_JISB1))
            .Add(New PageSize(MMToInches(515), MMToInches(728), "JIS B2", TWAIN_VB.TWSS.TWSS_JISB2))
            .Add(New PageSize(MMToInches(364), MMToInches(515), "JIS B3", TWAIN_VB.TWSS.TWSS_JISB3))
            .Add(New PageSize(MMToInches(257), MMToInches(364), "JIS B4", TWAIN_VB.TWSS.TWSS_JISB4))
            .Add(New PageSize(MMToInches(182), MMToInches(257), "JIS B5", TWAIN_VB.TWSS.TWSS_JISB5))
            .Add(New PageSize(MMToInches(128), MMToInches(182), "JIS B6", TWAIN_VB.TWSS.TWSS_JISB6))
            .Add(New PageSize(MMToInches(91), MMToInches(128), "JIS B7", TWAIN_VB.TWSS.TWSS_JISB7))
            .Add(New PageSize(MMToInches(64), MMToInches(91), "JIS B8", TWAIN_VB.TWSS.TWSS_JISB8))
            .Add(New PageSize(MMToInches(45), MMToInches(64), "JIS B9", TWAIN_VB.TWSS.TWSS_JISB9))
            .Add(New PageSize(MMToInches(32), MMToInches(45), "JIS B10", TWAIN_VB.TWSS.TWSS_JISB10))

        End With
    End Sub
End Class
