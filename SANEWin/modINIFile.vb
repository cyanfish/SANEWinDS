'
'   Copyright 2013-2021 Alec Skelly
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
Module modINIFile
    Private Logger As NLog.Logger = NLog.LogManager.GetCurrentClassLogger

    Public Function ReadIniSections(ByVal FileName As String) As List(Of String)
        Dim Result As New List(Of String)

        Dim sr As System.IO.StreamReader = Nothing
        Try
            Dim CurrentSection As String = Nothing

            sr = New System.IO.StreamReader(FileName)
            Dim line As String = sr.ReadLine
            Do While line IsNot Nothing
                line = line.Trim
                If line.Length > 0 Then
                    If line.Substring(0, 1) = "[" Then 'section header
                        Dim p As Integer = line.IndexOf("]")
                        If p > 1 Then 'section title is at least 1 char long
                            CurrentSection = line.Substring(1, p - 1)
                            CurrentSection = CurrentSection.Trim

                            If Not Result.Contains(CurrentSection) Then Result.Add(CurrentSection)

                        End If
                    End If
                End If
                line = sr.ReadLine
            Loop

        Catch ex As Exception
            Logger.Warn(ex.Message)
        Finally
            If sr IsNot Nothing Then sr.Close()
        End Try

        Return Result
    End Function

    Public Function ReadIni(ByVal FileName As String, ByVal strSection As String, ByVal strKey As String) As String
        Dim sr As System.IO.StreamReader = Nothing
        Try

            If strSection Is Nothing Then Throw New ArgumentException("Section name not supplied")
            If strKey Is Nothing Then Throw New ArgumentException("Key name not supplied")

            strSection = strSection.Trim
            strKey = strKey.Trim

            Dim CurrentSection As String = Nothing

            sr = New System.IO.StreamReader(FileName)
            Dim line As String = sr.ReadLine
            Do While line IsNot Nothing
                line = line.Trim
                If line.Length > 0 Then
                    If line.Substring(0, 1) = "[" Then 'section header
                        Dim p As Integer = line.IndexOf("]")
                        If p > 1 Then 'section title is at least 1 char long
                            CurrentSection = line.Substring(1, p - 1)
                            CurrentSection = CurrentSection.Trim
                        End If
                    ElseIf line.Substring(0, 1) = ";" Then 'comment
                    ElseIf line.Contains("=") Then 'value
                        If CurrentSection.ToUpper = strSection.ToUpper Then
                            Dim p As Integer = line.IndexOf("=")
                            If p > 0 Then 'the key name is at least 1 char long
                                If p < (line.Length - 1) Then 'the value is at least 1 char long
                                    Dim name As String = line.Substring(0, p)
                                    name = name.Trim
                                    If name.ToUpper = strKey.ToUpper Then
                                        Dim value As String = line.Substring(p + 1)
                                        value = value.Trim
                                        If value.Substring(0, 1) = Chr(34) Then
                                            If value.Substring(value.Length - 1, 1) = Chr(34) Then 'it's a quoted value; remove the quotes
                                                value = value.Substring(1, value.Length - 2)
                                            End If
                                        End If
                                        Return value
                                    End If
                                End If
                            End If
                        End If
                    End If
                End If
                line = sr.ReadLine
            Loop

        Catch ex As Exception
            Logger.Warn(ex.Message)
        Finally
            If sr IsNot Nothing Then sr.Close()
        End Try

        Return Nothing
    End Function

    Public Sub WriteIni(ByVal FileName As String, ByVal strSection As String, ByVal strKey As String, ByVal strValue As String)
        Dim sr As System.IO.StreamReader = Nothing
        Dim sw As System.IO.StreamWriter = Nothing

        Try
            If strSection Is Nothing Then Throw New ArgumentException("Section name not supplied")
            If strKey Is Nothing Then Throw New ArgumentException("Key name not supplied")
            If strValue Is Nothing Then strValue = ""
            strValue = strValue.Trim
            If strValue.Contains(" ") Then strValue = Chr(34) & strValue & Chr(34)

            strSection = strSection.Trim
            strKey = strKey.Trim

            Dim SectionExists As Boolean = False
            Dim KeyExists As Boolean = False

            Dim CurrentSection As String = Nothing

            Dim lines As New List(Of String) 'temporary buffer for ini lines

            If Not My.Computer.FileSystem.FileExists(FileName) Then
                'create new .ini file
                sw = New IO.StreamWriter(FileName)
                sw.Close()
                sw = Nothing
            End If

            sr = New System.IO.StreamReader(FileName)

            Dim line As String = sr.ReadLine
            Do While line IsNot Nothing
                line = line.Trim
                If line.Length > 0 Then
                    If line.Substring(0, 1) = "[" Then 'section header
                        Dim p As Integer = line.IndexOf("]")
                        If p > 1 Then 'section title is at least 1 char long
                            CurrentSection = line.Substring(1, p - 1)
                            CurrentSection = CurrentSection.Trim
                            If CurrentSection.ToUpper = strSection.ToUpper Then
                                SectionExists = True
                            Else
                                If SectionExists And (Not KeyExists) Then 'we just moved out of our target section without having found the key, so create a new key.
                                    lines.Add(strKey & "=" & strValue)
                                    KeyExists = True
                                End If
                            End If
                        End If
                        lines.Add(line)
                    ElseIf line.Substring(0, 1) = ";" Then 'comment
                        lines.Add(line)
                    ElseIf line.Contains("=") Then 'value
                        If CurrentSection.ToUpper = strSection.ToUpper Then
                            Dim p As Integer = line.IndexOf("=")
                            If p > 0 Then 'the key name is at least 1 char long
                                Dim name As String = line.Substring(0, p)
                                name = name.Trim
                                If name.ToUpper = strKey.ToUpper Then
                                    KeyExists = True
                                    lines.Add(strKey & "=" & strValue)
                                Else
                                    lines.Add(line)
                                End If
                            Else
                                'this line starts with "=", which is invalid.  Remove it.
                            End If
                        Else
                            lines.Add(line) 'not in our target section.
                        End If
                    Else
                        lines.Add(line) 'something we don't understand, so preserve it.
                    End If
                Else 'empty line
                    lines.Add("")
                End If
                line = sr.ReadLine
            Loop
            If Not SectionExists Then lines.Add("[" & strSection & "]")
            If Not KeyExists Then lines.Add(strKey & "=" & strValue) 'in case our section was last in the file and the key didn't already exist.

            sr.Close()
            sr = Nothing
            sw = New System.IO.StreamWriter(FileName)
            For Each s As String In lines
                sw.WriteLine(s)
            Next

        Catch ex As Exception
            Logger.Error(ex)
        Finally
            If sr IsNot Nothing Then sr.Close()
            If sw IsNot Nothing Then sw.Close()
        End Try

    End Sub

    Public Sub RemoveINISection(ByVal FileName As String, ByVal Section As String)
        Dim sr As System.IO.StreamReader = Nothing
        Dim sw As System.IO.StreamWriter = Nothing

        Try
            If Section Is Nothing Then Throw New ArgumentException("Section name not supplied")
            Section = Section.Trim

            'Dim SectionExists As Boolean = False

            Dim CurrentSection As String = Nothing

            Dim lines As New List(Of String) 'temporary buffer for ini lines

            If Not My.Computer.FileSystem.FileExists(FileName) Then
                'create new .ini file
                sw = New IO.StreamWriter(FileName)
                sw.Close()
                sw = Nothing
            End If

            sr = New System.IO.StreamReader(FileName)

            Dim line As String = sr.ReadLine
            Do While line IsNot Nothing
                line = line.Trim
                If line.Length > 0 Then
                    If line.Substring(0, 1) = "[" Then 'section header
                        Dim p As Integer = line.IndexOf("]")
                        If p > 1 Then 'section title is at least 1 char long
                            CurrentSection = line.Substring(1, p - 1)
                            CurrentSection = CurrentSection.Trim
                            If Not (CurrentSection.ToUpper Like Section.ToUpper) Then
                                lines.Add(line)
                            End If
                        End If
                    ElseIf line.Substring(0, 1) = ";" Then 'comment
                        lines.Add(line)
                    ElseIf line.Contains("=") Then 'value
                        If Not (CurrentSection.ToUpper Like Section.ToUpper) Then
                            Dim p As Integer = line.IndexOf("=")
                            If p > 0 Then 'the key name is at least 1 char long
                                lines.Add(line)
                            Else
                                'this line starts with "=", which is invalid.  Remove it.
                            End If
                        Else
                            'lines.Add(line) 'not in our target section.
                        End If
                    Else
                        lines.Add(line) 'something we don't understand, so preserve it.
                    End If
                Else 'empty line
                    lines.Add("")
                End If
                line = sr.ReadLine
            Loop

            sr.Close()
            sr = Nothing
            sw = New System.IO.StreamWriter(FileName)
            For Each s As String In lines
                sw.WriteLine(s)
            Next

        Catch ex As Exception
            Logger.Error(ex)
        Finally
            If sr IsNot Nothing Then sr.Close()
            If sw IsNot Nothing Then sw.Close()
        End Try

    End Sub
End Module

