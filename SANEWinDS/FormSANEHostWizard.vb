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
Public Class FormSANEHostWizard

    Private Shared Logger As NLog.Logger = NLog.LogManager.GetCurrentClassLogger()
    Private TSClientIP As String
    Private Const DefaultPortString As String = "6566"
    Private Const DefaultTimeoutString As String = "30000"
    Private OriginalHostIndex As Integer

    Private Sub ButtonNext_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ButtonNext.Click
        If Me.PanelHost.Visible Then
            If Me.ComboBoxHost.Text IsNot Nothing AndAlso Me.ComboBoxHost.Text.Length Then
                If Me.TextBoxPort.Text IsNot Nothing AndAlso Me.TextBoxPort.Text.Length Then
                    Dim n As Integer
                    If Integer.TryParse(Me.TextBoxPort.Text, n) Then
                        If n > 0 Then
                            'Dim net As System.Net.Sockets.TcpClient
                            Dim Host As New SharedSettings.HostInfo
                            Dim tmout As Integer = 0
                            Integer.TryParse(Me.TextBoxTimeout.Text, tmout)
                            If tmout < 1000 Then tmout = 1000
                            With Host
                                .UseTSClientIP = Me.CheckBoxUseTSClientIP.Checked
                                .NameOrAddress = Me.ComboBoxHost.Text
                                .Port = n
                                .TCP_Timeout_ms = tmout
                                '.Username = Me.TextBoxUserName.Text
                            End With
                            If CurrentSettings.ResolveHost(Host) Then
                                Try

                                    Close_SANE()
                                    Close_Net()

                                    If ControlClient Is Nothing Then ControlClient = New System.Net.Sockets.TcpClient
                                    If ControlClient IsNot Nothing Then
                                        ControlClient.ReceiveTimeout = Host.TCP_Timeout_ms
                                        ControlClient.SendTimeout = Host.TCP_Timeout_ms
                                        Logger.Debug("TCPClient Send buffer length is {0}", ControlClient.SendBufferSize)
                                        Logger.Debug("TCPClient Receive buffer length is {0}", ControlClient.ReceiveBufferSize)
                                        Me.Cursor = Windows.Forms.Cursors.WaitCursor
                                        ControlClient.Connect(Host.NameOrAddress, Host.Port)
                                        'Dim Status As SANE_API.SANE_Status = SANE.Net_Init(net, Host.Username)
                                        Dim Status As SANE_API.SANE_Status = SANE.Net_Init(ControlClient, Environment.UserName)
                                        Logger.Debug("Net_Init returned status '{0}'", Status)
                                        If Status = SANE_API.SANE_Status.SANE_STATUS_GOOD Then
                                            Host.Open = True
                                            Dim Devices(-1) As SANE_API.SANE_Device
                                            Me.ComboBoxDevices.Items.Clear()
                                            Status = SANE.Net_Get_Devices(ControlClient, Devices)
                                            If Status = SANE_API.SANE_Status.SANE_STATUS_GOOD Then
                                                For i As Integer = 0 To Devices.Length - 1
                                                    'XXX MsgBox("Name: " & Devices(i).name & vbCr & "Vendor: " & Devices(i).vendor & vbCr & "Model: " & Devices(i).model & vbCr & "Type: " & Devices(i).type)
                                                    Me.ComboBoxDevices.Items.Add(Devices(i).name)
                                                Next
                                                If Me.ComboBoxDevices.Items.Count > 0 Then Me.ComboBoxDevices.SelectedIndex = 0
                                                Dim Host_Index As Integer = -1
                                                For idx = 0 To CurrentSettings.SANE.Hosts.Count - 1
                                                    Dim hi As SharedSettings.HostInfo = CurrentSettings.SANE.Hosts(idx)
                                                    If hi.NameOrAddress.Trim = Host.NameOrAddress.Trim Then
                                                        If hi.Port = Host.Port Then
                                                            Host_Index = idx
                                                            Exit For
                                                        End If
                                                    End If
                                                Next
                                                If Host_Index > -1 Then
                                                    CurrentSettings.SANE.CurrentHostIndex = Host_Index
                                                    Host.Username = CurrentSettings.SANE.Hosts(CurrentSettings.SANE.CurrentHostIndex).Username 'preserve existing username
                                                    Host.Password = CurrentSettings.SANE.Hosts(CurrentSettings.SANE.CurrentHostIndex).Password 'preserve existing password
                                                    'XXX
                                                    Host.Image_Timeout_s = CurrentSettings.SANE.Hosts(CurrentSettings.SANE.CurrentHostIndex).Image_Timeout_s 'this should be added to the GUI
                                                    '
                                                    CurrentSettings.SANE.Hosts(CurrentSettings.SANE.CurrentHostIndex) = Host
                                                Else
                                                    'XXX
                                                    Host.Image_Timeout_s = 1200 '20 minutes
                                                    '
                                                    ReDim Preserve CurrentSettings.SANE.Hosts(CurrentSettings.SANE.Hosts.Count)
                                                    CurrentSettings.SANE.Hosts(CurrentSettings.SANE.Hosts.Count - 1) = Host
                                                    CurrentSettings.SANE.CurrentHostIndex = CurrentSettings.SANE.Hosts.Count - 1
                                                    Me.ComboBoxHost.Items.Add(Host.NameOrAddress)
                                                End If
                                            Else
                                                Throw New ApplicationException("SANE server returned status '" & Status.ToString & "'")
                                            End If
                                        Else
                                            Throw New ApplicationException("SANE server returned status '" & Status.ToString & "'")
                                        End If
                                    End If

                                    Me.PanelDevice.Top = Me.PanelHost.Top
                                    Me.PanelDevice.Left = Me.PanelHost.Left
                                    Me.PanelDevice.Height = Me.PanelHost.Height
                                    Me.PanelDevice.Width = Me.PanelHost.Width
                                    Me.PanelDevice.Anchor = Me.PanelHost.Anchor

                                    Me.PanelHost.Visible = False
                                    Me.PanelDevice.Visible = True
                                    Me.ButtonNext.Text = "OK"
                                Catch ex As Exception
                                    MsgBox(ex.Message)
                                Finally
                                    Close_SANE()
                                    Close_Net()
                                    Me.Cursor = Windows.Forms.Cursors.Default
                                End Try
                            Else
                                MsgBox("Unable to resolve host '" & Host.NameOrAddress & "'", MsgBoxStyle.Exclamation)
                            End If
                            Else
                                MsgBox("Please specify a valid port number", MsgBoxStyle.Exclamation)
                            End If
                        Else
                            MsgBox("Please specify a valid port number", MsgBoxStyle.Exclamation)
                        End If
                    Else
                        MsgBox("Please specify a valid port number", MsgBoxStyle.Exclamation)
                    End If
                Else
                    MsgBox("Please specify a hostname or IP address", MsgBoxStyle.Exclamation)
                End If

            ElseIf Me.PanelDevice.Visible Then
                If Me.ComboBoxDevices.Text IsNot Nothing Then
                    If Me.ComboBoxDevices.Text.Trim.Length > 0 Then
                        With CurrentSettings.SANE.Hosts(CurrentSettings.SANE.CurrentHostIndex)
                            .NameOrAddress = Me.ComboBoxHost.Text
                            .Port = CInt(Me.TextBoxPort.Text)
                            .TCP_Timeout_ms = CInt(Me.TextBoxTimeout.Text)
                            '.Username = Me.TextBoxUserName.Text
                        End With
                        CurrentSettings.SANE.Hosts(CurrentSettings.SANE.CurrentHostIndex).Device = Me.ComboBoxDevices.Text.Trim
                        Me.DialogResult = Windows.Forms.DialogResult.OK
                        Me.Close()
                    End If
                End If
            End If

            Me.SetControlStatus()

    End Sub

    Private Sub FormSANEHostWizard_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Me.PanelHost.Visible = True
        Me.PanelDevice.Visible = False

        Me.OriginalHostIndex = CurrentSettings.SANE.CurrentHostIndex

        Try
            Dim ts As New TSAPI
            Me.TSClientIP = ts.GetCurrentSessionIP
        Catch ex As Exception
            Logger.Debug(ex.Message, ex)
        End Try
        Me.CheckBoxUseTSClientIP.Enabled = Not String.IsNullOrWhiteSpace(Me.TSClientIP)

        For Each host As SharedSettings.HostInfo In CurrentSettings.SANE.Hosts
            Me.ComboBoxHost.Items.Add(host.NameOrAddress)
        Next
        If (CurrentSettings.SANE.CurrentHostIndex > -1) AndAlso (CurrentSettings.SANE.CurrentHostIndex < CurrentSettings.SANE.Hosts.Length) Then
            With CurrentSettings.SANE.Hosts(CurrentSettings.SANE.CurrentHostIndex)
                Me.ComboBoxHost.SelectedItem = .NameOrAddress
                Me.CheckBoxUseTSClientIP.Checked = (.UseTSClientIP) And (Not String.IsNullOrWhiteSpace(Me.TSClientIP))
                If .Port > 0 Then Me.TextBoxPort.Text = .Port.ToString Else Me.TextBoxPort.Text = DefaultPortString
                If .TCP_Timeout_ms > 1000 Then Me.TextBoxTimeout.Text = .TCP_Timeout_ms.ToString Else Me.TextBoxTimeout.Text = DefaultTimeoutString
            End With
         Else
            Me.TextBoxPort.Text = DefaultPortString
            Me.TextBoxTimeout.Text = DefaultTimeoutString
            'Me.TextBoxUserName.Text = CurrentSettings.ProductName.Name
            'Me.TextBoxUserName.Text = Environment.UserName
        End If

        'With CurrentSettings.SANE.Hosts(CurrentSettings.SANE.CurrentHostIndex)
        '    If .NameOrAddress IsNot Nothing Then Me.ComboBoxHost.Text = .NameOrAddress
        '    If .Port > 0 Then Me.TextBoxPort.Text = .Port.ToString Else Me.TextBoxPort.Text = "6566"
        '    If .TCP_Timeout_ms > 1000 Then Me.TextBoxTimeout.Text = .TCP_Timeout_ms.ToString Else Me.TextBoxTimeout.Text = "5000"
        '    If .Username IsNot Nothing AndAlso .Username.Trim.Length Then Me.TextBoxUserName.Text = .Username.Trim Else Me.TextBoxUserName.Text = CurrentSettings.ProductName.Name
        'End With


        Me.SetControlStatus()

    End Sub

    Private Sub ButtonPrevious_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ButtonPrevious.Click
        If Me.PanelDevice.Visible Then
            Me.PanelDevice.Visible = False
            Me.PanelHost.Visible = True
            'Me.ButtonPrevious.Enabled = False
            'Me.ButtonNext.Text = "Next"
        End If

        Close_SANE()
        Close_Net()

        Me.SetControlStatus()

    End Sub

    Private Sub ButtonCancel_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ButtonCancel.Click
        CurrentSettings.SANE.CurrentHostIndex = Me.OriginalHostIndex
        Me.DialogResult = Windows.Forms.DialogResult.Cancel
        Me.Close()
    End Sub

    Private Sub SetControlStatus()
        Me.ButtonNext.Text = IIf(Me.PanelHost.Visible, "Next", "OK")
        Me.ButtonPrevious.Enabled = Me.PanelDevice.Visible

    End Sub


    Private Sub ComboBoxHost_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ComboBoxHost.SelectedIndexChanged

        If (ComboBoxHost.SelectedIndex > -1) AndAlso (ComboBoxHost.SelectedIndex < CurrentSettings.SANE.Hosts.Count) Then
            CurrentSettings.SANE.CurrentHostIndex = ComboBoxHost.SelectedIndex
        End If

        If (CurrentSettings.SANE.CurrentHostIndex > -1) AndAlso (CurrentSettings.SANE.CurrentHostIndex < CurrentSettings.SANE.Hosts.Count) Then
            With CurrentSettings.SANE.Hosts(CurrentSettings.SANE.CurrentHostIndex)
                'If .NameOrAddress IsNot Nothing Then Me.ComboBoxHost.Text = .NameOrAddress
                If .Port > 0 Then Me.TextBoxPort.Text = .Port.ToString Else Me.TextBoxPort.Text = DefaultPortString
                If .TCP_Timeout_ms > 1000 Then Me.TextBoxTimeout.Text = .TCP_Timeout_ms.ToString Else Me.TextBoxTimeout.Text = DefaultTimeoutString
                'If .Username IsNot Nothing AndAlso .Username.Trim.Length Then Me.TextBoxUserName.Text = .Username.Trim Else Me.TextBoxUserName.Text = CurrentSettings.ProductName.Name
            End With
        End If
    End Sub

    Private Sub CheckBoxUseTSClientIP_CheckedChanged(sender As Object, e As EventArgs) Handles CheckBoxUseTSClientIP.CheckedChanged
        If Me.CheckBoxUseTSClientIP.Checked Then Me.ComboBoxHost.Text = Me.TSClientIP
        Me.ComboBoxHost.Enabled = Not Me.CheckBoxUseTSClientIP.Checked
    End Sub

    Private Sub ButtonDefaultPort_Click(sender As Object, e As EventArgs) Handles ButtonDefaultPort.Click
        Me.TextBoxPort.Text = DefaultPortString
    End Sub

    Private Sub ButtonDefaultTimeout_Click(sender As Object, e As EventArgs) Handles ButtonDefaultTimeout.Click
        Me.TextBoxTimeout.Text = DefaultTimeoutString
    End Sub
End Class