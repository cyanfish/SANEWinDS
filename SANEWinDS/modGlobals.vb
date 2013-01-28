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
Imports System.Drawing

Module modGlobals

    Private Logger As NLog.Logger = NLog.LogManager.GetCurrentClassLogger()

    Public CurrentSettings As SharedSettings
    Public SANE As SANE_API
    Public net As System.Net.Sockets.TcpClient

    Public Function InchesToMM(ByVal Inches As Double) As Double
        Return Inches * 25.4
    End Function

    Public Function MMToInches(ByVal mm As Double) As Double
        Return mm / 25.4
    End Function

    Public Function InchesToCM(ByVal Inches As Double) As Double
        Return Inches * 2.54
    End Function

    Public Function CMToInches(ByVal cm As Double) As Double
        Return cm / 2.54
    End Function

    Public Function AcquireImage(ByRef bmp As Bitmap) As SANE_API.SANE_Status
        Logger.Debug("")
        Dim Status As SANE_API.SANE_Status = 0
        Dim Port As Int32 = 0
        Dim ByteOrder As SANE_API.SANE_Net_Byte_Order
        Dim AuthResource As String = Nothing

        Dim SANEImage As New SANE_API.SANEImage
        Dim CurrentFrame As Integer = 0
        net.ReceiveTimeout = CurrentSettings.SANE.Hosts(CurrentSettings.SANE.CurrentHostIndex).Image_Timeout_s * 1000
        Do
            Status = SANE.Net_Start(net, SANE.CurrentDevice.Handle, Port, ByteOrder, _
                                    CurrentSettings.SANE.Hosts(CurrentSettings.SANE.CurrentHostIndex).Username, _
                                    CurrentSettings.SANE.Hosts(CurrentSettings.SANE.CurrentHostIndex).Password)
            If Status = SANE_API.SANE_Status.SANE_STATUS_GOOD Then
                ReDim Preserve SANEImage.Frames(CurrentFrame)
                Try
                    SANEImage.Frames(CurrentFrame) = SANE.AcquireFrame(net, Port, ByteOrder, net.ReceiveTimeout) 'CurrentSettings.SANE.Hosts(CurrentSettings.SANE.CurrentHostIndex).TCP_Timeout_ms)
                Catch ex As Exception
                    Logger.ErrorException("AcquireFrame returned exception: " & ex.Message, ex)
                    Status = SANE_API.SANE_Status.SANE_STATUS_IO_ERROR
                    Exit Do
                End Try
                If SANEImage.Frames(CurrentFrame).Params.last_frame Then
                    Exit Do
                Else
                    CurrentFrame += 1
                End If
            ElseIf Status = SANE_API.SANE_Status.SANE_STATUS_ACCESS_DENIED Then
                Dim PwdBox As New FormSANEAuth
                PwdBox.UsernameTextBox.Text = CurrentSettings.SANE.Hosts(CurrentSettings.SANE.CurrentHostIndex).Username
                If PwdBox.ShowDialog = Windows.Forms.DialogResult.Cancel Then Exit Do
                CurrentSettings.SANE.Hosts(CurrentSettings.SANE.CurrentHostIndex).Username = PwdBox.UsernameTextBox.Text
                CurrentSettings.SANE.Hosts(CurrentSettings.SANE.CurrentHostIndex).Password = PwdBox.PasswordTextBox.Text
            End If
        Loop While (Status = SANE_API.SANE_Status.SANE_STATUS_GOOD) Or (Status = SANE_API.SANE_Status.SANE_STATUS_ACCESS_DENIED)
        net.ReceiveTimeout = CurrentSettings.SANE.Hosts(CurrentSettings.SANE.CurrentHostIndex).TCP_Timeout_ms

        If Status = SANE_API.SANE_Status.SANE_STATUS_GOOD Then

            'XXX need to switch away from GDI+ and use something else.  WPF?  FreeImage?  Qt?

            'Dim bmp As Bitmap = Nothing
            bmp = Nothing
            Dim bmp_data As Imaging.BitmapData
            Try
                'create image from SANE frames
                If SANEImage.Frames.Length > 1 Then SANEImage.Frames = CombineImageFrames(SANEImage.Frames) 'combine frames for 3-pass scanners

                Dim h As Integer = SANEImage.Frames(0).Params.lines
                Dim w As Integer = SANEImage.Frames(0).Params.pixels_per_line
                Dim Stride As Integer = SANEImage.Frames(0).Params.bytes_per_line

                If h < 0 Then '-1 if the number of lines is unknown, e.g. a hand scanner.
                    h = SANEImage.Frames(0).Data.Length \ Stride
                End If

                Dim bounds As Rectangle = New Rectangle(0, 0, w, h)
                Dim PixelFormat As Imaging.PixelFormat = Nothing
                Dim Palette As Imaging.ColorPalette = Nothing

                Select Case SANEImage.Frames(0).Params.format
                    Case SANE_API.SANE_Frame.SANE_FRAME_GRAY
                        Select Case SANEImage.Frames(0).Params.depth
                            Case 1
                                PixelFormat = Imaging.PixelFormat.Format1bppIndexed
                                Palette = Get1bitGrayScalePalette()
                            Case 8
                                PixelFormat = Imaging.PixelFormat.Format8bppIndexed
                                Palette = Get8bitGrayScalePalette()
                            Case 16
                                If ByteOrder = SANE_API.SANE_Net_Byte_Order.SANE_NET_BIG_ENDIAN Then SwapImageBytes(SANEImage.Frames(0).Data)
                                PixelFormat = Imaging.PixelFormat.Format16bppGrayScale
                        End Select
                    Case SANE_API.SANE_Frame.SANE_FRAME_RGB
                        RGBtoBGR(SANEImage.Frames(0).Data, SANEImage.Frames(0).Params.depth)
                        Select Case SANEImage.Frames(0).Params.depth
                            Case 1
                                MsgBox("1bpp color images are not currently supported")
                                'XXX
                                'Exit Do
                                Status = SANE_API.SANE_Status.SANE_STATUS_INVAL
                            Case 8
                                PixelFormat = Imaging.PixelFormat.Format24bppRgb
                            Case 16
                                If ByteOrder = SANE_API.SANE_Net_Byte_Order.SANE_NET_BIG_ENDIAN Then SwapImageBytes(SANEImage.Frames(0).Data)
                                PixelFormat = Imaging.PixelFormat.Format48bppRgb
                        End Select
                End Select
                If Status = SANE_API.SANE_Status.SANE_STATUS_GOOD Then
                    Try
                        bmp = New Bitmap(w, h, PixelFormat)
                    Catch ex As ArgumentException
                        MsgBox("GDI+ was unable to allocate enough memory to store the image", MsgBoxStyle.Critical, "Error")
                        Throw
                    End Try
                    If Palette IsNot Nothing Then bmp.Palette = Palette
                    bmp_data = bmp.LockBits(bounds, Imaging.ImageLockMode.ReadWrite, PixelFormat)
                    If Stride < bmp_data.Stride Then
                        For r = 0 To h - 1
                            System.Runtime.InteropServices.Marshal.Copy(SANEImage.Frames(0).Data, Stride * r, bmp_data.Scan0 + (bmp_data.Stride * r), Stride)
                        Next
                    ElseIf Stride > bmp_data.Stride Then
                        For r = 0 To h - 1
                            System.Runtime.InteropServices.Marshal.Copy(SANEImage.Frames(0).Data, Stride * r, bmp_data.Scan0 + (bmp_data.Stride * r), bmp_data.Stride)
                        Next
                    Else
                        System.Runtime.InteropServices.Marshal.Copy(SANEImage.Frames(0).Data, 0, bmp_data.Scan0, SANEImage.Frames(0).Data.Length)
                    End If
                    bmp.UnlockBits(bmp_data)
                End If
            Catch ex As Exception
                Logger.ErrorException("Error creating image: " & ex.Message, ex)
                Status = SANE_API.SANE_Status.SANE_STATUS_IO_ERROR
            Finally
                'ReDim SANEImage.Frames(0).Data(-1)
                SANEImage = Nothing
                bmp_data = Nothing
                'bmp = Nothing
            End Try
        Else
            Dim msg As String = "Got status '" & Status.ToString & "' while acquiring image frames"
            Logger.Debug(msg)
            If Status <> SANE_API.SANE_Status.SANE_STATUS_NO_DOCS Then MsgBox(msg)
        End If
        Return Status
    End Function

    Private Function CombineImageFrames(Frames() As SANE_API.SANEImageFrame) As SANE_API.SANEImageFrame()
        Dim CombinedFrame As SANE_API.SANEImageFrame = Nothing
        With CombinedFrame.Params
            .format = SANE_API.SANE_Frame.SANE_FRAME_RGB
            .last_frame = True
            .lines = Frames(0).Params.lines
            .pixels_per_line = Frames(0).Params.pixels_per_line
            .bytes_per_line = Frames(0).Params.bytes_per_line * 3
            .depth = Frames(0).Params.depth
        End With
        Dim CombinedStream As New System.IO.MemoryStream
        Dim RedStream As System.IO.MemoryStream = Nothing
        Dim GreenStream As System.IO.MemoryStream = Nothing
        Dim BlueStream As System.IO.MemoryStream = Nothing
        Try
            For FrameIndex = 0 To Frames.Length - 1
                Select Case Frames(FrameIndex).Params.format
                    Case SANE_API.SANE_Frame.SANE_FRAME_RED
                        RedStream = New System.IO.MemoryStream(Frames(FrameIndex).Data)
                    Case SANE_API.SANE_Frame.SANE_FRAME_GREEN
                        GreenStream = New System.IO.MemoryStream(Frames(FrameIndex).Data)
                    Case SANE_API.SANE_Frame.SANE_FRAME_BLUE
                        BlueStream = New System.IO.MemoryStream(Frames(FrameIndex).Data)
                End Select
            Next
            If RedStream Is Nothing Or GreenStream Is Nothing Or BlueStream Is Nothing Then Throw New Exception("One or more frames is missing from the image data")
            Dim samplebytes As Integer = 1 'this should work for 1bpp and 8bpp
            If CombinedFrame.Params.depth = 16 Then samplebytes = 2
            For sample As UInt32 = 0 To (RedStream.Length - 1) \ samplebytes
                For samplebyte As Byte = 1 To samplebytes
                    CombinedStream.WriteByte(RedStream.ReadByte)
                Next
                For samplebyte As Byte = 1 To samplebytes
                    CombinedStream.WriteByte(GreenStream.ReadByte)
                Next
                For samplebyte As Byte = 1 To samplebytes
                    CombinedStream.WriteByte(BlueStream.ReadByte)
                Next
            Next
            CombinedFrame.Data = CombinedStream.ToArray
            Dim Result(0) As SANE_API.SANEImageFrame
            Result(0) = CombinedFrame
            Return Result
        Catch ex As Exception
            Throw
        Finally
            If CombinedStream IsNot Nothing Then CombinedStream.Close()
            If RedStream IsNot Nothing Then RedStream.Close()
            If GreenStream IsNot Nothing Then GreenStream.Close()
            If BlueStream IsNot Nothing Then BlueStream.Close()
            CombinedStream = Nothing
            RedStream = Nothing
            GreenStream = Nothing
            BlueStream = Nothing
        End Try
        Return Nothing
    End Function

    Public Sub RGBtoBGR(ByRef bytes() As Byte, ByVal BitsPerColor As Integer)
        Select Case BitsPerColor
            Case 1
                'XXX
                Throw New Exception("1bpp images are not yet supported")
            Case 8
                For i As UInt32 = 0 To bytes.Length - 1 Step 3
                    If i < bytes.Length - 2 Then
                        Dim R As Byte = bytes(i)
                        bytes(i) = bytes(i + 2)
                        bytes(i + 2) = R
                    End If
                Next
            Case 16
                For i As UInt32 = 0 To bytes.Length - 1 Step 6
                    If i < bytes.Length - 5 Then
                        Dim R1 As Byte = bytes(i)
                        Dim R2 As Byte = bytes(i + 1)
                        bytes(i) = bytes(i + 4)
                        bytes(i + 1) = bytes(i + 5)
                        bytes(i + 4) = R1
                        bytes(i + 5) = R2
                    End If
                Next
        End Select
    End Sub

    Private Sub SwapImageBytes(ByRef bytes() As Byte)
        For i As UInteger = 0 To bytes.Length - 1 Step 2
            If i < bytes.Length - 1 Then
                Dim b As Byte = bytes(i)
                bytes(i) = bytes(i + 1)
                bytes(i + 1) = b
            End If
        Next
    End Sub

    Private Function Get1bitGrayScalePalette()
        Dim bmp As New Bitmap(1, 1, Imaging.PixelFormat.Format1bppIndexed)
        Dim palette As Imaging.ColorPalette = bmp.Palette
        palette.Entries(0) = Color.White
        palette.Entries(1) = Color.Black
        Return palette
    End Function

    Private Function Get8bitGrayScalePalette()
        Dim bmp As New Bitmap(1, 1, Imaging.PixelFormat.Format8bppIndexed)
        Dim palette As Imaging.ColorPalette = bmp.Palette
        For i As Integer = 0 To palette.Entries.Length - 1
            palette.Entries(i) = Color.FromArgb(i, i, i)
        Next
        Return palette
    End Function

    Public Function Device_Appears_To_Have_ADF() As Boolean
        For i As Integer = 1 To SANE.CurrentDevice.OptionDescriptors.Count - 1 'index 0 is the array length
            If SANE.CurrentDevice.OptionDescriptors(i).name IsNot Nothing AndAlso SANE.CurrentDevice.OptionDescriptors(i).name.ToLower = "source" Then
                Select Case SANE.CurrentDevice.OptionDescriptors(i).type
                    Case SANE_API.SANE_Value_Type.SANE_TYPE_STRING
                        Select Case SANE.CurrentDevice.OptionDescriptors(i).constraint_type
                            Case SANE_API.SANE_Constraint_Type.SANE_CONSTRAINT_STRING_LIST
                                For j As Integer = 0 To SANE.CurrentDevice.OptionDescriptors(i).constraint.string_list.Count - 1
                                    Dim s As String = SANE.CurrentDevice.OptionDescriptors(i).constraint.string_list(j).ToLower
                                    If (s.Contains("adf") Or s.Contains("automatic document feeder")) Then
                                        Return True
                                    End If
                                Next
                        End Select
                End Select
            ElseIf SANE.CurrentDevice.OptionDescriptors(i).type = SANE_API.SANE_Value_Type.SANE_TYPE_BOOL Then
                If SANE.CurrentDevice.OptionDescriptors(i).name IsNot Nothing Then
                    Dim s As String = SANE.CurrentDevice.OptionDescriptors(i).name.ToLower
                    If (s.Contains("adf") Or s.Contains("automatic document feeder")) Then
                        Return True
                    End If
                End If
            End If
        Next
        Return False
    End Function
    Public Function Device_Appears_To_Have_ADF_Enabled() As Boolean
        For i As Integer = 1 To SANE.CurrentDevice.OptionDescriptors.Count - 1 'index 0 is the array length
            If SANE.CurrentDevice.OptionDescriptors(i).name IsNot Nothing AndAlso SANE.CurrentDevice.OptionDescriptors(i).name.ToLower = "source" Then
                Select Case SANE.CurrentDevice.OptionDescriptors(i).type
                    Case SANE_API.SANE_Value_Type.SANE_TYPE_STRING
                        Select Case SANE.CurrentDevice.OptionDescriptors(i).constraint_type
                            Case SANE_API.SANE_Constraint_Type.SANE_CONSTRAINT_STRING_LIST
                                For j As Integer = 0 To SANE.CurrentDevice.OptionDescriptors(i).constraint.string_list.Count - 1
                                    Dim s As String = SANE.CurrentDevice.OptionDescriptors(i).constraint.string_list(j).ToLower
                                    If (s.Contains("adf") Or s.Contains("automatic document feeder")) Then
                                        Return SANE.CurrentDevice.OptionValues(i)(0).ToLower = s.ToLower
                                    End If
                                Next
                        End Select
                End Select
            ElseIf SANE.CurrentDevice.OptionDescriptors(i).type = SANE_API.SANE_Value_Type.SANE_TYPE_BOOL Then
                If SANE.CurrentDevice.OptionDescriptors(i).name IsNot Nothing Then
                    Dim s As String = SANE.CurrentDevice.OptionDescriptors(i).name.ToLower
                    If (s.Contains("adf") Or s.Contains("automatic document feeder")) Then
                        Return SANE.CurrentDevice.OptionValues(i)(0)
                    End If
                End If
            End If
        Next
        Return False
    End Function

    Public Function Device_Appears_To_Have_Duplex() As Boolean
        For i As Integer = 1 To SANE.CurrentDevice.OptionDescriptors.Count - 1 'index 0 is the array length
            If SANE.CurrentDevice.OptionDescriptors(i).name IsNot Nothing AndAlso SANE.CurrentDevice.OptionDescriptors(i).name.ToLower = "source" Then
                Select Case SANE.CurrentDevice.OptionDescriptors(i).type
                    Case SANE_API.SANE_Value_Type.SANE_TYPE_STRING
                        Select Case SANE.CurrentDevice.OptionDescriptors(i).constraint_type
                            Case SANE_API.SANE_Constraint_Type.SANE_CONSTRAINT_STRING_LIST
                                For j As Integer = 0 To SANE.CurrentDevice.OptionDescriptors(i).constraint.string_list.Count - 1
                                    Dim s As String = SANE.CurrentDevice.OptionDescriptors(i).constraint.string_list(j).ToLower
                                    If s.Contains("duplex") Then
                                        Return True
                                    End If
                                Next
                        End Select
                End Select
            ElseIf SANE.CurrentDevice.OptionDescriptors(i).type = SANE_API.SANE_Value_Type.SANE_TYPE_BOOL Then
                If SANE.CurrentDevice.OptionDescriptors(i).name IsNot Nothing Then
                    Dim s As String = SANE.CurrentDevice.OptionDescriptors(i).name.ToLower
                    If s.Contains("duplex") Then
                        Return True
                    End If
                End If
            End If
        Next
        Return False
    End Function

    Public Sub Close_SANE()
        Try
            If SANE.CurrentDevice.Open Then
                SANE.CurrentDevice.Open = False
                SANE.Net_Close(net, SANE.CurrentDevice.Handle)
            End If
            If (CurrentSettings.SANE.CurrentHostIndex > -1) AndAlso (CurrentSettings.SANE.CurrentHostIndex < CurrentSettings.SANE.Hosts.Length) Then
                If CurrentSettings.SANE.Hosts(CurrentSettings.SANE.CurrentHostIndex).Open Then
                    CurrentSettings.SANE.Hosts(CurrentSettings.SANE.CurrentHostIndex).Open = False
                    SANE.Net_Exit(net)
                End If
            End If
        Catch exx As Exception
        End Try
    End Sub

    Public Sub Close_Net()
        Try
            If (CurrentSettings.SANE.CurrentHostIndex > -1) AndAlso (CurrentSettings.SANE.CurrentHostIndex < CurrentSettings.SANE.Hosts.Length) Then
                If Not CurrentSettings.SANE.Hosts(CurrentSettings.SANE.CurrentHostIndex).Open Then
                    If net IsNot Nothing Then
                        If net.Connected Then
                            Dim stream As System.Net.Sockets.NetworkStream = net.GetStream
                            stream.Close()
                            stream = Nothing
                        End If
                        If net.Connected Then net.Close()
                        net = Nothing
                    End If
                End If
            End If
        Catch ex As Exception
            Logger.ErrorException(ex.Message, ex)
        End Try
    End Sub
End Module
