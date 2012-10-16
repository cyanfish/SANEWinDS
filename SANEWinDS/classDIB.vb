Imports System.Runtime.InteropServices
Class DIB
    Private MyStream As System.IO.MemoryStream
    Private MyBitmapInfo As BITMAPINFO
    Public Structure BITMAPINFOHEADER
        Dim biSize As UInt32
        Dim biWidth As Int32
        Dim biHeight As Int32
        Dim biPlanes As UInt16
        Dim bitCount As UInt16
        Dim biCompression As UInt32
        Dim biSizeImage As UInt32
        Dim biXPelsPerMeter As Int32
        Dim biYPelsPerMeter As Int32
        Dim biClrUsed As UInt32
        Dim biClrImportant As UInt32
    End Structure
    Public Structure RGBQUAD
        Dim rgbBlue As Byte
        Dim rgbGreen As Byte
        Dim rgbRed As Byte
        Const rgbReserved As Byte = 0
    End Structure
    Public Structure BITMAPINFO
        Dim bmiHeader As BITMAPINFOHEADER
        Dim bmiColors() As RGBQUAD
    End Structure
    Friend ReadOnly Property DIBInfo As BITMAPINFO
        Get
            DIBInfo = MyBitmapInfo
        End Get
    End Property
    Friend ReadOnly Property Stream As System.IO.MemoryStream
        Get
            Stream = MyStream
        End Get
    End Property

    Sub New(ByRef Bmp As System.Drawing.Bitmap)
        Try
            MyStream = New System.IO.MemoryStream
            Bmp.Save(MyStream, System.Drawing.Imaging.ImageFormat.Bmp)
            Dim B() As Byte = MyStream.ToArray
            MyStream.Dispose()
            Array.Copy(B, 14, B, 0, B.Length - 14) 'FILEINFOHEADER is 14 bytes long
            Array.Resize(B, B.Length - 14)
            MyStream = New System.IO.MemoryStream(B) 'this is now the DIB without the FILEINFOHEADER.  It still contains the BITMAPINFOHEADER and the color palette.

            MyBitmapInfo.bmiHeader = GetBitmapInfoHeader()

            ReDim MyBitmapInfo.bmiColors(MyBitmapInfo.bmiHeader.biClrUsed - 1)
            For i As Integer = 0 To MyBitmapInfo.bmiColors.Count - 1
                Dim BaseIndex As Integer = i * 4
                MyBitmapInfo.bmiColors(i).rgbBlue = B(MyBitmapInfo.bmiHeader.biSize + BaseIndex)
                MyBitmapInfo.bmiColors(i).rgbGreen = B(MyBitmapInfo.bmiHeader.biSize + BaseIndex + 1)
                MyBitmapInfo.bmiColors(i).rgbRed = B(MyBitmapInfo.bmiHeader.biSize + BaseIndex + 2)
            Next

            Array.Resize(B, 0)

        Catch ex As Exception
            Logger.Write(DebugLogger.Level.Error_, True, ex.Message)
            Throw
        End Try
    End Sub

    Friend Function CreateUnmanagedCopy() As IntPtr
        'returns a handle to an unmanaged copy of the DIB
        Try
            Dim B() As Byte = MyStream.ToArray
            Dim hBmp As IntPtr = WinAPI.GlobalAlloc(WinAPI.GlobalAllocFlags.GHND, B.Length)
            Marshal.Copy(B, 0, hBmp, B.Length)
            Array.Resize(B, 0)
            Return hBmp
        Catch ex As Exception
            Logger.Write(DebugLogger.Level.Error_, True, ex.Message)
            Throw
        End Try
    End Function

    Friend Sub CopyLinesToUnmanagedBuffer(ByVal LineOffset As Integer, ByVal LineCount As UInt32, ByVal AlignedBytesPerLine As UInt32, ByVal pBuffer As IntPtr)
        Try
            'Logger.Write(DebugLogger.Level.Debug, False, "LineOffset=" & LineOffset.ToString & ", LineCount=" & LineCount.ToString & ", AlignedBytesPerLine=" & AlignedBytesPerLine.ToString)

            Dim B((AlignedBytesPerLine * LineCount) - 1) As Byte
            'Logger.Write(DebugLogger.Level.Debug, False, "Copying " & B.Length.ToString & " bytes into buffer")

            'Logger.Write(DebugLogger.Level.Debug, False, "Data stream length = " & MyStream.Length)

            MyStream.Seek(MyStream.Length - (AlignedBytesPerLine * LineOffset), IO.SeekOrigin.Begin) 'ok to seek past end of stream
            'Logger.Write(DebugLogger.Level.Debug, False, "Seek to " & MyStream.Position.ToString)
            Dim PreviousLineOffset As Integer = MyStream.Position
            For i As Integer = 0 To LineCount - 1
                Dim Line(AlignedBytesPerLine - 1) As Byte

                PreviousLineOffset -= AlignedBytesPerLine

                MyStream.Seek(PreviousLineOffset, IO.SeekOrigin.Begin)
                'Logger.Write(DebugLogger.Level.Debug, False, "Seek to " & MyStream.Position.ToString)

                MyStream.Read(Line, 0, AlignedBytesPerLine)
                'Logger.Write(DebugLogger.Level.Debug, False, "Read " & AlignedBytesPerLine.ToString)

                If MyBitmapInfo.bmiHeader.bitCount = 24 Then RGBtoBGR(Line, 8) 'XXX what about other bitCounts?

                Array.Copy(Line, 0, B, i * Line.Length, Line.Length)
            Next

            Marshal.Copy(B, 0, pBuffer, B.Length)
            Array.Resize(B, 0)

        Catch ex As Exception
            Logger.Write(DebugLogger.Level.Error_, True, ex.Message)
            Throw
        End Try
    End Sub

    'Function GetPaletteSize() As Integer
    '    Dim PaletteSize As Integer = MyBitmapInfo.bmiColors.Count * 4
    '    'XXX there are special rules for the size of the palette when bmiColors.Count = 0
    '    'see http://www.herdsoft.com/ti/davincie/imex3j8i.htm

    '    Return PaletteSize
    'End Function

    Function GetBitmapInfoHeader() As BITMAPINFOHEADER
        Try
            Dim BIH As BITMAPINFOHEADER
            Dim BR As New System.IO.BinaryReader(MyStream)
            BIH.biSize = BR.ReadUInt32
            BIH.biWidth = BR.ReadInt32
            BIH.biHeight = BR.ReadInt32
            BIH.biPlanes = BR.ReadUInt16
            BIH.bitCount = BR.ReadUInt16
            BIH.biCompression = BR.ReadUInt32
            BIH.biSizeImage = BR.ReadUInt32
            BIH.biXPelsPerMeter = BR.ReadInt32
            BIH.biYPelsPerMeter = BR.ReadInt32
            BIH.biClrUsed = BR.ReadUInt32
            BIH.biClrImportant = BR.ReadUInt32

            Logger.Write(DebugLogger.Level.Debug, False, "Size = " & BIH.biSize.ToString)
            Logger.Write(DebugLogger.Level.Debug, False, "Width = " & BIH.biWidth.ToString)
            Logger.Write(DebugLogger.Level.Debug, False, "Height = " & BIH.biHeight.ToString)
            Logger.Write(DebugLogger.Level.Debug, False, "Planes = " & BIH.biPlanes.ToString)
            Logger.Write(DebugLogger.Level.Debug, False, "Bits Per Pixel = " & BIH.bitCount.ToString)
            Logger.Write(DebugLogger.Level.Debug, False, "Compression = " & BIH.biCompression.ToString)
            Logger.Write(DebugLogger.Level.Debug, False, "Image Size = " & BIH.biSizeImage.ToString)
            Logger.Write(DebugLogger.Level.Debug, False, "X Pixels Per Meter = " & BIH.biXPelsPerMeter.ToString)
            Logger.Write(DebugLogger.Level.Debug, False, "Y Pixels Per Meter = " & BIH.biYPelsPerMeter.ToString)
            Logger.Write(DebugLogger.Level.Debug, False, "Colors in Palette = " & BIH.biClrUsed.ToString)
            Logger.Write(DebugLogger.Level.Debug, False, "Important Colors = " & BIH.biClrImportant.ToString)

            Return BIH
        Catch ex As Exception
            Logger.Write(DebugLogger.Level.Error_, True, ex.Message)
            Throw
        End Try
    End Function
End Class
