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
                                .NameOrAddress = Me.ComboBoxHost.Text
                                .Port = n
                                .TCP_Timeout_ms = tmout
                                .Username = Me.TextBoxUserName.Text
                            End With
                            Try

                                Close_SANE()
                                Close_Net()

                                If net Is Nothing Then net = New System.Net.Sockets.TcpClient
                                If net IsNot Nothing Then
                                    net.ReceiveTimeout = Host.TCP_Timeout_ms
                                    net.SendTimeout = Host.TCP_Timeout_ms
                                    Logger.Write(DebugLogger.Level.Debug, False, "TCPClient Send buffer length is " & net.SendBufferSize)
                                    Logger.Write(DebugLogger.Level.Debug, False, "TCPClient Receive buffer length is " & net.ReceiveBufferSize)
                                    Me.Cursor = Windows.Forms.Cursors.WaitCursor
                                    net.Connect(Host.NameOrAddress, Host.Port)
                                    Dim Status As SANE_API.SANE_Status = SANE.Net_Init(net, Host.Username)
                                    Logger.Write(DebugLogger.Level.Debug, False, "Net_Init returned status '" & Status.ToString & "'")
                                    If Status = SANE_API.SANE_Status.SANE_STATUS_GOOD Then
                                        Host.Open = True
                                        Dim Devices(-1) As SANE_API.SANE_Device
                                        Me.ComboBoxDevices.Items.Clear()
                                        Status = SANE.Net_Get_Devices(net, Devices)
                                        If Status = SANE_API.SANE_Status.SANE_STATUS_GOOD Then
                                            For i As Integer = 0 To Devices.Length - 1
                                                'XXX MsgBox("Name: " & Devices(i).name & vbCr & "Vendor: " & Devices(i).vendor & vbCr & "Model: " & Devices(i).model & vbCr & "Type: " & Devices(i).type)
                                                Me.ComboBoxDevices.Items.Add(Devices(i).name)
                                            Next
                                            If Me.ComboBoxDevices.Items.Count > 0 Then Me.ComboBoxDevices.SelectedIndex = 0
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
                    With CurrentSettings.SANE.CurrentHost
                        .NameOrAddress = Me.ComboBoxHost.Text
                        .Port = CInt(Me.TextBoxPort.Text)
                        .TCP_Timeout_ms = CInt(Me.TextBoxTimeout.Text)
                        .Username = Me.TextBoxUserName.Text
                    End With
                    CurrentSettings.SANE.CurrentDevice = Me.ComboBoxDevices.Text.Trim
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
        With CurrentSettings.SANE.CurrentHost
            If .NameOrAddress IsNot Nothing Then Me.ComboBoxHost.Text = .NameOrAddress
            If .Port > 0 Then Me.TextBoxPort.Text = .Port.ToString Else Me.TextBoxPort.Text = "6566"
            If .TCP_Timeout_ms > 1000 Then Me.TextBoxTimeout.Text = .TCP_Timeout_ms.ToString Else Me.TextBoxTimeout.Text = "5000"
            If .Username IsNot Nothing AndAlso .Username.Trim.Length Then Me.TextBoxUserName.Text = .Username.Trim Else Me.TextBoxUserName.Text = CurrentSettings.ProductName.Name
        End With

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
        Me.DialogResult = Windows.Forms.DialogResult.Cancel
        Me.Close()
    End Sub

    Private Sub SetControlStatus()
        Me.ButtonNext.Text = IIf(Me.PanelHost.Visible, "Next", "OK")
        Me.ButtonPrevious.Enabled = Me.PanelDevice.Visible

    End Sub


End Class