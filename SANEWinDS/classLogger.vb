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
Public Class DebugLogger
    Public CurrentLevel As Level = Level.None
    Public FileName As String = Nothing
    Private UserSettingsFolder As String
    Private ProductName As System.Reflection.AssemblyName = System.Reflection.Assembly.GetExecutingAssembly.GetName

    Public Enum Level As Integer
        None = 0
        Error_ = 1
        Warn = 2
        Debug = 16
        Info = 32
    End Enum

    Public Sub New(ByVal UseRoamingAppData As Boolean)
        Me.CurrentLevel = Level.Info 'XXX
        If UseRoamingAppData Then
            UserSettingsFolder = System.Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData) & "\" & Me.ProductName.Name
        Else
            UserSettingsFolder = System.Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData) & "\" & Me.ProductName.Name
        End If
        If Not My.Computer.FileSystem.DirectoryExists(UserSettingsFolder) Then My.Computer.FileSystem.CreateDirectory(UserSettingsFolder)
        Me.FileName = UserSettingsFolder & "\" & Me.ProductName.Name & "_" & Now.ToString("yyyyMMdd") & ".log"
        If Not My.Computer.FileSystem.FileExists(FileName) Then
            Dim f As New System.IO.StreamWriter(FileName)
            f.Close()
            f = Nothing
        End If
    End Sub

    Public Sub Write(ByVal LogLevel As Level, ByVal NotifyUser As Boolean, ByVal Message As String)
        Dim caller As String = Nothing
        Dim callertype As String = Nothing
        If Me.CurrentLevel >= LogLevel Then
            Try
                Dim stackTrace As StackTrace = New StackTrace
                caller = stackTrace.GetFrame(1).GetMethod.Name
                callertype = stackTrace.GetFrame(1).GetMethod.ReflectedType.Name
            Catch ex As Exception
                Debug.Print("Exception thrown while logging: " & ex.Message)
            End Try
            Try
                If NotifyUser Then MsgBox(Message, MsgBoxStyle.OkOnly, "Message")
                Dim sw As New System.IO.StreamWriter(Me.FileName, True)
                Dim ClientName As String = System.Environment.ExpandEnvironmentVariables("%ClientName%")
                If ClientName = "%ClientName%" Then ClientName = System.Environment.ExpandEnvironmentVariables("%ComputerName%")
                sw.WriteLine(Now.ToString("yyyyMMdd HH:mm:ss.fff") & vbTab & System.Environment.ExpandEnvironmentVariables("'%UserName%' on '%ComputerName%' from '" & ClientName & "'") & vbTab & LogLevel.ToString.Replace("_", "") & vbTab & "[" & callertype & ":" & caller & "]" & vbTab & Message)
                sw.Close()
            Catch ex As Exception
                Debug.Print("Exception thrown while logging: " & ex.Message)
            End Try
        End If
    End Sub

    Public Sub Delete_Expired_Logs(ByVal OlderThanDays As Integer)
        Dim LogPath As String = Me.UserSettingsFolder
        If LogPath IsNot Nothing AndAlso LogPath.Length > 0 Then
            If OlderThanDays > 0 Then
                For Each FileName As String In My.Computer.FileSystem.GetFiles(LogPath, FileIO.SearchOption.SearchTopLevelOnly, {Me.ProductName.Name & "_*.log"})
                    Dim span As TimeSpan = Nothing
                    Try
                        span = Now.Subtract(My.Computer.FileSystem.GetFileInfo(FileName).CreationTime)
                        If span.Days > OlderThanDays Then
                            My.Computer.FileSystem.DeleteFile(FileName)
                            Me.Write(Level.Debug, False, "Deleted expired log file '" & FileName & "'")
                        End If
                    Catch ex As Exception
                        Me.Write(Level.Error_, False, "Unable to delete expired log file '" & FileName & "': " & ex.Message)
                    Finally
                        span = Nothing
                    End Try
                Next
            End If
        Else
            Me.Write(Level.Warn, False, "LogPath not configured; cleanup aborted")
        End If
    End Sub
End Class
