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

    'Public Enum UserSettingsLocations As Integer
    '    LocalAppData = 0
    '    RoamingAppData = 1
    'End Enum
    Public Structure HostInfo
        Dim NameOrAddress As String
        Dim Port As Integer
        Dim Username As String
        Dim TCP_Timeout_ms As Integer
        Dim Open As Boolean
    End Structure
    Public Structure SANESettings
        Dim Hosts() As HostInfo
        Dim CurrentHost As HostInfo
        Dim CurrentDevice As String    'Full SANE device name on CurrentHost (example: "canon_dr:libusb:008:005")
        Dim CurrentDeviceINI As IniFile
        Dim AutoLocateDevice As String 'SANE backend name of device to auto-choose from list of devices on CurrentHost (example: "canon_dr")
    End Structure
    Public Structure TWAINSettings
        'Dim Enabled As Boolean

    End Structure
    Public ConfigDirectory As String
    Public LogDirectory As String

    Public ProductName As System.Reflection.AssemblyName = System.Reflection.Assembly.GetExecutingAssembly.GetName

    Private Initialized As Boolean
    Private UserSettingsFolder As String
    Public UseRoamingAppData As Boolean
    Public ScanContinuously As Boolean
    Public ScanContinuouslyUserConfigured As Boolean
    Public SANE As SANESettings
    Public TWAIN As TWAINSettings

    Public Sub New(ByVal _UseRoamingAppData As Boolean)
        If Not Initialized Then
            UseRoamingAppData = _UseRoamingAppData
            ReadSettings()
            Initialized = True
        End If
    End Sub

    Private Sub WriteSettings()

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

        Me.ConfigDirectory = UserSettingsFolder
        Me.LogDirectory = Me.ConfigDirectory 'XXX

        Dim UserSettingsFileName As String = Me.ConfigDirectory & "\" & Me.ProductName.Name & ".ini"
        If Not My.Computer.FileSystem.FileExists(UserSettingsFileName) Then
            Dim f As New System.IO.StreamWriter(UserSettingsFileName)
            f.WriteLine("[Log]")
            f.WriteLine("RetainDays=3")
            f.WriteLine("")
            f.WriteLine("[SANE]")
            f.WriteLine("Hosts=")
            f.WriteLine("DefaultHost=0")
            f.WriteLine("Device=")
            f.WriteLine("AutoLocateDevice=")
            f.Close()
            f = Nothing
        End If

        Dim INI As New IniFile
        INI.Load(UserSettingsFileName)

        'Set defaults for new ini file
        'XXX The INIFile class writes the settings out of order
        'If INI.GetSection("Log") Is Nothing Then
        '    INI.AddSection("Log")
        '    INI.Save(UserSettingsFileName)
        'End If
        'If String.IsNullOrEmpty(INI.GetKeyValue("Log", "RetainDays")) Then
        '    INI.SetKeyValue("Log", "RetainDays", "3")
        '    INI.Save(UserSettingsFileName)
        'End If
        'If INI.GetSection("SANE") Is Nothing Then
        '    INI.AddSection("SANE")
        '    INI.Save(UserSettingsFileName)
        'End If
        'If String.IsNullOrEmpty(INI.GetKeyValue("SANE", "Hosts")) Then
        '    INI.SetKeyValue("SANE", "Hosts", " ")
        '    INI.Save(UserSettingsFileName)
        'End If
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

        Dim Hosts() As HostInfo = GetSANEHostsFromString(INI.GetKeyValue("SANE", "Hosts"))
        Dim CurrentHostIndex As Integer = -1
        Dim s As String = INI.GetKeyValue("SANE", "DefaultHost")
        Integer.TryParse(s, CurrentHostIndex)
        If (CurrentHostIndex > -1) And (CurrentHostIndex < Hosts.Length) Then
            'SANESettings.CurrentHost = Hosts(CurrentHostIndex)
            SANE.CurrentHost.NameOrAddress = Hosts(CurrentHostIndex).NameOrAddress
            SANE.CurrentHost.Port = Hosts(CurrentHostIndex).Port
            SANE.CurrentHost.TCP_Timeout_ms = Hosts(CurrentHostIndex).TCP_Timeout_ms
            SANE.CurrentHost.Username = Hosts(CurrentHostIndex).Username
        End If
        Logger.Write(DebugLogger.Level.Info, False, "CurrentHost is '" & SANE.CurrentHost.NameOrAddress & "' port '" & SANE.CurrentHost.Port & "'")
        SANE.CurrentDevice = INI.GetKeyValue("SANE", "Device")
        Logger.Write(DebugLogger.Level.Info, False, "CurrentDevice is '" & SANE.CurrentDevice & "'")
        SANE.AutoLocateDevice = INI.GetKeyValue("SANE", "AutoLocateDevice").Trim
        Logger.Write(DebugLogger.Level.Info, False, "AutoLocateDevice is '" & SANE.AutoLocateDevice & "'")

        'clean up expired log files
        Dim DebugLogMaxAgeDays As Integer = 0 'never delete
        Try
            s = INI.GetKeyValue("Log", "RetainDays")
            If s Is Nothing OrElse s.Length < 1 Then Throw New Exception("RetainDays value is not configured in the [Log] section")
            If Integer.TryParse(s, DebugLogMaxAgeDays) Then
                If DebugLogMaxAgeDays = 0 Then
                    Logger.Write(DebugLogger.Level.Info, False, "Retaining logs forever")
                Else
                    Logger.Write(DebugLogger.Level.Info, False, "Retaining logs for '" & DebugLogMaxAgeDays.ToString & "' days")
                End If
                Logger.Delete_Expired_Logs(DebugLogMaxAgeDays)
            Else
                Throw New Exception("Unable to interpret '" & s & "' as an integer")
            End If
        Catch ex As Exception
            Logger.Write(DebugLogger.Level.Error_, False, "Error reading RetainDays value from '" & UserSettingsFileName & "': " & ex.Message)
        End Try

    End Sub

    Public Function HostIsValid(ByVal Host As HostInfo) As Boolean
        With Host
            If .NameOrAddress IsNot Nothing AndAlso .NameOrAddress.Length Then
                If .Port > 0 Then
                    If .TCP_Timeout_ms > 1000 Then
                        If .Username IsNot Nothing AndAlso .Username.Length Then
                            Logger.Write(DebugLogger.Level.Debug, False, "returning True")
                            Return True
                        End If
                    End If
                End If
            End If
        End With
        Logger.Write(DebugLogger.Level.Debug, False, "returning False")
        Return False
    End Function

    Private Function GetSANEHostsFromString(ByVal str As String) As HostInfo()
        'str should be in the format "host0:port0;timeout_ms0;username0,host1:port1;timeout_ms1;username1,..."
        Dim Hosts(-1) As HostInfo
        If str IsNot Nothing Then
            Dim h() As String = str.Split(",")
            For Each s As String In h
                'Try
                If s IsNot Nothing AndAlso s.Length > 0 Then
                    Dim p As Integer = s.IndexOf(":")
                    If p > 0 And p < (s.Length - 1) Then
                        ReDim Preserve Hosts(Hosts.Count)
                        Hosts(Hosts.Count - 1).NameOrAddress = System.Environment.ExpandEnvironmentVariables(s.Substring(0, p))

                        If Hosts(Hosts.Count - 1).NameOrAddress.ToUpper = "TSCLIENTIP" Then
                            Try
                                Dim ts As New TSAPI
                                Hosts(Hosts.Count - 1).NameOrAddress = ts.GetCurrentSessionIP
                                ts = Nothing
                            Catch ex As Exception
                                Logger.Write(DebugLogger.Level.Debug, False, "Error getting terminal server client IP address: " & ex.Message)
                            End Try
                        End If

                        Hosts(Hosts.Count - 1).Port = s.Substring(p + 1)
                        'XXX parse these out from the ini
                        Hosts(Hosts.Count - 1).TCP_Timeout_ms = 5000
                        Hosts(Hosts.Count - 1).Username = ProductName.Name
                        '
                    End If
                End If
                'Catch ex As Exception
                'End Try
            Next
        End If
        Return Hosts
    End Function

    Public Function GetDeviceConfigFileName() As String
        Dim BackEnd As String = modGlobals.SANE.CurrentDevice.Name
        Dim p As Integer = BackEnd.IndexOf(":")
        If p Then BackEnd = BackEnd.Substring(0, p)
        Dim f As String = CurrentSettings.ConfigDirectory & "\" & BackEnd & ".ini"
        If Not My.Computer.FileSystem.FileExists(f) Then CreateDeviceConfigFile(f)
        Return f
    End Function

    Private Sub CreateDeviceConfigFile(ByVal FileName As String)
        'Create .INI file to hold all settings for SANE backend
        Dim fs As System.IO.StreamWriter = Nothing
        Try
            'If My.Computer.FileSystem.FileExists(FileName) Then
            '    'XXX read file
            'Else

            'Logger.Write(DebugLogger.Level.Debug, False, "1")

            fs = New System.IO.StreamWriter(FileName)
            'Logger.Write(DebugLogger.Level.Debug, False, "1")
            fs.WriteLine("[General]")
            fs.WriteLine(";ScanContinuously=True")
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
                        'fs.WriteLine("TWAIN-capability=" & TWAINcap)
                End Select
                fs.WriteLine("")
            Next
            'End If
        Catch ex As Exception
            Logger.Write(DebugLogger.Level.Error_, False, "Error writing '" & FileName & "': " & ex.Message)
        Finally
            If fs IsNot Nothing Then fs.Close()
        End Try
        '

    End Sub
End Class
