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
Imports System.Runtime.InteropServices
Class DIB
    Private Shared Logger As NLog.Logger = NLog.LogManager.GetCurrentClassLogger()
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
            Logger.Error(ex, ex.Message)
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
            Logger.Error(ex, ex.Message)
            Throw
        End Try
    End Function

    Friend Sub CopyLinesToUnmanagedBuffer(ByVal LineOffset As Integer, ByVal LineCount As UInt32, ByVal AlignedBytesPerLine As UInt32, ByVal pBuffer As IntPtr)
        Try
            Dim B((AlignedBytesPerLine * LineCount) - 1) As Byte
            MyStream.Seek(MyStream.Length - (AlignedBytesPerLine * (MyBitmapInfo.bmiHeader.biHeight - LineOffset - LineCount)), IO.SeekOrigin.Begin) 'ok to seek past end of stream
            Dim PreviousLineOffset As Integer = MyStream.Position
            For i As Integer = 0 To LineCount - 1
                Dim Line(AlignedBytesPerLine - 1) As Byte
                PreviousLineOffset -= AlignedBytesPerLine
                MyStream.Seek(PreviousLineOffset, IO.SeekOrigin.Begin)
                MyStream.Read(Line, 0, AlignedBytesPerLine)
                If MyBitmapInfo.bmiHeader.bitCount = 24 Then RGBtoBGR(Line, 8)
                If MyBitmapInfo.bmiHeader.bitCount = 1 Then WBtoBW(Line)
                Array.Copy(Line, 0, B, i * Line.Length, Line.Length)
            Next
            Marshal.Copy(B, 0, pBuffer, B.Length)
            Array.Resize(B, 0)
        Catch ex As Exception
            Logger.Error(ex, ex.Message)
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

            Logger.Debug("Size = {0}", BIH.biSize)
            Logger.Debug("Width = {0}", BIH.biWidth)
            Logger.Debug("Height = {0}", BIH.biHeight)
            Logger.Debug("Planes = {0}", BIH.biPlanes)
            Logger.Debug("Bits Per Pixel = {0}", BIH.bitCount)
            Logger.Debug("Compression = {0}", BIH.biCompression)
            Logger.Debug("Image Size = {0}", BIH.biSizeImage)
            Logger.Debug("X Pixels Per Meter = {0}", BIH.biXPelsPerMeter)
            Logger.Debug("Y Pixels Per Meter = {0}", BIH.biYPelsPerMeter)
            Logger.Debug("Colors in Palette = {0}", BIH.biClrUsed)
            Logger.Debug("Important Colors = {0}", BIH.biClrImportant)

            Return BIH
        Catch ex As Exception
            Logger.Error(ex, ex.Message)
            Throw
        End Try
    End Function
End Class
