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

<Serializable()>
Public Class EmptyFrameException
    Inherits System.Exception
    Public Sub New()
        MyBase.New("An empty image frame was received from the SANE server")
    End Sub
    Public Sub New(ByVal message As String)
        MyBase.New(message)
    End Sub
    Public Sub New(ByVal message As String, ByVal innerException As System.Exception)
        MyBase.New(message, innerException)
    End Sub
End Class

Public Class ServerDisconnectedException
    Inherits System.Exception
    Public Sub New()
        MyBase.New("The server has disconnected")
    End Sub
    Public Sub New(ByVal message As String)
        MyBase.New(message)
    End Sub
    Public Sub New(ByVal message As String, ByVal innerException As System.Exception)
        MyBase.New(message, innerException)
    End Sub
End Class

Class SANE_API
#Region "SANE.h"
    Friend Const SANE_FALSE = 0
    Friend Const SANE_TRUE = 1

    Friend Const SANE_CURRENT_MAJOR = 1
    Friend Const SANE_CURRENT_MINOR = 0
    Friend Const SANE_CURRENT_BUILD = 3
    Friend ReadOnly VersionCode As Int32 = Me.SANE_VERSION_CODE(SANE_CURRENT_MAJOR, SANE_CURRENT_MINOR, SANE_CURRENT_BUILD)

    Friend Const SANE_MAX_USERNAME_LEN = 128
    Friend Const SANE_MAX_PASSWORD_LEN = 128

    Private Const SANE_FIXED_SCALE_SHIFT = 16

    Friend Const SANE_CAP_SOFT_SELECT = (1 << 0)
    Friend Const SANE_CAP_HARD_SELECT = (1 << 1)
    Friend Const SANE_CAP_SOFT_DETECT = (1 << 2)
    Friend Const SANE_CAP_EMULATED = (1 << 3)
    Friend Const SANE_CAP_AUTOMATIC = (1 << 4)
    Friend Const SANE_CAP_INACTIVE = (1 << 5)
    Friend Const SANE_CAP_ADVANCED = (1 << 6)

    Friend Function SANE_OPTION_IS_READABLE(ByVal cap As Int32) '!!! My code; not from sane.h.
        If Not SANE_OPTION_IS_ACTIVE(cap) Then Return False
        If (cap And SANE_CAP_HARD_SELECT) And Not (cap And SANE_CAP_SOFT_DETECT) Then Return False
        Return True
    End Function

    Friend Function SANE_OPTION_IS_ACTIVE(ByVal cap As Int32)
        Return ((cap And SANE_CAP_INACTIVE) = 0)
    End Function

    Friend Function SANE_OPTION_IS_SETTABLE(ByVal cap As Int32)
        Return ((cap And SANE_CAP_SOFT_SELECT) <> 0)
    End Function

    Friend Const SANE_INFO_INEXACT = (1 << 0)
    Friend Const SANE_INFO_RELOAD_OPTIONS = (1 << 1)
    Friend Const SANE_INFO_RELOAD_PARAMS = (1 << 2)

    Friend Enum SANE_Constraint_Type
        SANE_CONSTRAINT_NONE = 0
        SANE_CONSTRAINT_RANGE = 1
        SANE_CONSTRAINT_WORD_LIST = 2
        SANE_CONSTRAINT_STRING_LIST = 3
    End Enum

    Friend Structure SANE_Range
        Dim min As Int32        '/* minimum (element) value */
        Dim max As Int32        '/* maximum (element) value */
        Dim quant As Int32      '/* quantization value (0 if none) */
    End Structure

    Friend Structure SANEConstraintUnion
        Dim string_list() As String
        Dim word_list() As Int32
        Dim range As SANE_Range
    End Structure
    Friend Structure SANE_Option_Descriptor
        Dim name As String          '/* name of this option (command-line name) */
        Dim title As String         '/* title of this option (single-line) */
        Dim desc As String          '/* description of this option (multi-line) */
        Dim type As SANE_Value_Type '/* how are values interpreted? */
        Dim unit As SANE_Unit       '/* what is the (physical) unit? */
        Dim size As Int32
        Dim cap As Int32            '/* capabilities */
        Dim constraint_type As SANE_Constraint_Type
        Dim constraint As SANEConstraintUnion
    End Structure

    Friend Enum SANE_Action
        SANE_ACTION_GET_VALUE = 0
        SANE_ACTION_SET_VALUE = 1
        SANE_ACTION_SET_AUTO = 2
    End Enum

    Friend Enum SANE_Frame
        SANE_FRAME_GRAY = 0     '/* band covering human visual range */
        SANE_FRAME_RGB = 1      '/* pixel-interleaved red/green/blue bands */
        SANE_FRAME_RED = 2      '/* red band only */
        SANE_FRAME_GREEN = 3    '/* green band only */
        SANE_FRAME_BLUE = 4     '/* blue band only */
    End Enum

    Friend Structure SANE_Parameters
        Dim format As SANE_Frame
        Dim last_frame As Boolean
        Dim bytes_per_line As Int32
        Dim pixels_per_line As Int32
        Dim lines As Int32
        Dim depth As Int32
    End Structure

    Private Function SANE_VERSION_CODE(ByVal major As Int32, ByVal minor As Int32, ByVal build As Int32) As Int32
        Return ((major And &HFF) << 24) Or ((minor And &HFF) << 16) Or ((build And &HFFFF) << 0)
    End Function

    Public Function SANE_VERSION_MAJOR(ByVal code As Int32) As Int32
        Return (code >> 24) And &HFF
    End Function

    Friend Function SANE_VERSION_MINOR(ByVal code As Int32) As Int32
        Return ((code >> 16) And &HFF)
    End Function

    Friend Function SANE_VERSION_BUILD(ByVal code As Int32) As Int32
        Return ((code >> 0) And &HFFFF)
    End Function

    Friend Function SANE_FIX(ByVal d As Double) As Int32
        Try
            Return CInt(d * (1 << SANE_FIXED_SCALE_SHIFT))
        Catch ofe As OverflowException
            Logger.Warn("The number '{0}' had to be adjusted to avoid an overflow.", d)
            If d > 0 Then
                Return Int32.MaxValue
            Else
                Return Int32.MinValue
            End If
        End Try
    End Function

    Friend Function SANE_UNFIX(ByVal w As Int32) As Double
        Return Math.Round(CDbl(w) / CDbl(1 << SANE_FIXED_SCALE_SHIFT), 4, MidpointRounding.AwayFromZero)
    End Function

    Friend Enum SANE_Status
        SANE_STATUS_GOOD = 0
        SANE_STATUS_UNSUPPORTED = 1
        SANE_STATUS_CANCELLED = 2
        SANE_STATUS_DEVICE_BUSY = 3
        SANE_STATUS_INVAL = 4
        SANE_STATUS_EOF = 5
        SANE_STATUS_JAMMED = 6
        SANE_STATUS_NO_DOCS = 7
        SANE_STATUS_COVER_OPEN = 8
        SANE_STATUS_IO_ERROR = 9
        SANE_STATUS_NO_MEM = 10
        SANE_STATUS_ACCESS_DENIED = 11
    End Enum

    Friend Enum SANE_Value_Type
        SANE_TYPE_BOOL = 0
        SANE_TYPE_INT = 1
        SANE_TYPE_FIXED = 2
        SANE_TYPE_STRING = 3
        SANE_TYPE_BUTTON = 4
        SANE_TYPE_GROUP = 5
    End Enum

    Friend Enum SANE_Unit
        SANE_UNIT_NONE = 0          '/* the value is unit-less (e.g., # of scans) */
        SANE_UNIT_PIXEL = 1         '/* value is number of pixels */
        SANE_UNIT_BIT = 2           '/* value is number of bits */
        SANE_UNIT_MM = 3            '/* value is millimeters */
        SANE_UNIT_DPI = 4           '/* value is resolution in dots/inch */
        SANE_UNIT_PERCENT = 5       '/* value is a percentage */
        SANE_UNIT_MICROSECOND = 6   '/* value is micro seconds */
    End Enum

    Friend Structure SANE_Device
        Dim name As String          '/* unique device name */
        Dim vendor As String        '/* device vendor string */
        Dim model As String     '/* device model name */
        Dim type As String          '/* device type (e.g., "flatbed scanner") */
    End Structure
#End Region

    Private Shared Logger As NLog.Logger = NLog.LogManager.GetCurrentClassLogger()

    Friend Event FrameProgress(PercentComplete As Integer)

    Friend Function UnitString(ByVal Unit As SANE_Unit) As String
        Select Case Unit
            Case SANE_Unit.SANE_UNIT_NONE
                Return Nothing
            Case SANE_Unit.SANE_UNIT_PIXEL
                Return "pixels"
            Case SANE_Unit.SANE_UNIT_BIT
                Return "bits"
            Case SANE_Unit.SANE_UNIT_MM
                Return "mm"
            Case SANE_Unit.SANE_UNIT_DPI
                Return "dpi"
            Case SANE_Unit.SANE_UNIT_PERCENT
                Return "%"
            Case SANE_Unit.SANE_UNIT_MICROSECOND
                Return "ms"
            Case Else
                Return Nothing
        End Select
    End Function

    Private Enum SANE_Net_Procedure_Number
        SANE_NET_INIT = 0
        SANE_NET_GET_DEVICES = 1
        SANE_NET_OPEN = 2
        SANE_NET_CLOSE = 3
        SANE_NET_GET_OPTION_DESCRIPTORS = 4
        SANE_NET_CONTROL_OPTION = 5
        SANE_NET_GET_PARAMETERS = 6
        SANE_NET_START = 7
        SANE_NET_CANCEL = 8
        SANE_NET_AUTHORIZE = 9
        SANE_NET_EXIT = 10
    End Enum

    Friend Enum SANE_Net_Byte_Order
        SANE_NET_LITTLE_ENDIAN = &H1234
        SANE_NET_BIG_ENDIAN = &H4321
    End Enum

    Friend Enum Sane_DataType
        SANE_Word = 0
        SANE_Byte = 1
        SANE_Char = 2
        SANE_String = 3
        SANE_Boolean = 4

    End Enum

    Friend Structure CurrentDeviceInfo
        Dim Name As String
        Dim Handle As Integer
        Dim Open As Boolean
        Dim OptionDescriptors() As SANE_Option_Descriptor
        Dim OptionValues() As Object
        Dim ScanUntilError As Boolean 'Use ADF
        Dim SupportedPageSizes As ArrayList
    End Structure
    'Friend TCP_Timeout_ms As Integer = 5000
    Friend CurrentDevice As CurrentDeviceInfo

    Friend Structure SANEImageFrame
        Dim Params As SANE_API.SANE_Parameters
        Dim Data As Byte()
    End Structure

    Friend Structure SANEImage
        Dim Frames() As SANEImageFrame
    End Structure

    Friend Structure SANENetControlOption_Request
        Dim handle As Int32
        Dim option_ As Int32
        Dim action As SANE_Action
        Dim value_type As SANE_Value_Type
        Dim value_size As Int32
        Dim values() As Object
    End Structure
    Friend Structure SANENetControlOption_Reply
        Dim status As SANE_Status
        Dim info As Int32
        Dim value_type As SANE_Value_Type
        Dim value_size As Int32
        Dim values() As Object
        Dim resource As String
    End Structure

    Public Sub New()
        Dim UseRoamingAppData As Boolean = False
        Try
            UseRoamingAppData = My.Settings.UseRoamingAppData
        Catch ex As Exception
        End Try

        Logger.Debug("")

        If CurrentSettings Is Nothing Then CurrentSettings = New SharedSettings(UseRoamingAppData)

    End Sub

    Friend Function OptionValueArrayLength(ByVal Option_Size As Int32, ByVal Option_Type As SANE_Value_Type) As Int32
        Select Case Option_Type
            Case SANE_Value_Type.SANE_TYPE_BUTTON, SANE_Value_Type.SANE_TYPE_GROUP
                Return 0
            Case SANE_Value_Type.SANE_TYPE_BOOL
                Return Option_Size \ 4
            Case SANE_Value_Type.SANE_TYPE_FIXED
                Return Option_Size \ 4
            Case SANE_Value_Type.SANE_TYPE_INT
                Return Option_Size \ 4
            Case SANE_Value_Type.SANE_TYPE_STRING
                'XXX string arrays?!
                Return 1
            Case Else
                'XXX
                Return 0
        End Select
    End Function

    Friend Sub Serialize(ByVal Data As Object, ByRef Buffer() As Byte, ByRef Offset As Integer, ByVal DataType As Sane_DataType)
        Select Case DataType
            Case Sane_DataType.SANE_Word
                If Data.GetType = GetType(Int32) Then
                    Dim sane_word As Int32 = Me.SwapEndian(CType(Data, Int32))
                    If (Buffer Is Nothing) OrElse (Offset + 3 > Buffer.Length - 1) Then ReDim Preserve Buffer(Offset + 3)
                    BitConverter.GetBytes(sane_word).CopyTo(Buffer, Offset)
                    Offset += 4
                Else
                    Throw New ArgumentException("data type mismatch")
                End If
            Case Sane_DataType.SANE_Byte
                If Data.GetType = GetType(Byte) Then
                    If (Buffer Is Nothing) OrElse (Offset > Buffer.Length - 1) Then ReDim Preserve Buffer(Offset)
                    Buffer(Offset) = Data
                    Offset += 1
                Else
                    Throw New ArgumentException("data type mismatch")
                End If
            Case Sane_DataType.SANE_Char
                If Data.GetType = GetType(Char) Then
                    If (Buffer Is Nothing) OrElse (Offset > Buffer.Length - 1) Then ReDim Preserve Buffer(Offset)
                    Buffer(Offset) = Asc(Data)
                    Offset += 1
                Else
                    Throw New ArgumentException("data type mismatch")
                End If
            Case Sane_DataType.SANE_String
                If Data.GetType = GetType(String) Then
                    Dim s As String = DirectCast(Data, String)
                    Dim strlen As Int32 = s.Length
                    Me.Serialize(strlen + 1, Buffer, Offset, Sane_DataType.SANE_Word) 'add 1 to strlen for the terminating null
                    For j As Integer = 0 To strlen - 1
                        Me.Serialize(CChar(s.Substring(j, 1)), Buffer, Offset, Sane_DataType.SANE_Char)
                    Next
                    Me.Serialize(Chr(0), Buffer, Offset, Sane_DataType.SANE_Char)
                Else
                    Throw New ArgumentException("data type mismatch")
                End If
            Case Sane_DataType.SANE_Boolean
                If Data.GetType = GetType(Boolean) Then
                    Dim int As Int32 = 0
                    If Data = True Then int = SANE_TRUE Else int = SANE_FALSE
                    Me.Serialize(int, Buffer, Offset, Sane_DataType.SANE_Word)
                Else
                    Throw New ArgumentException("data type mismatch")
                End If
            Case Else
                Throw New ArgumentException("Unimplemented Sane_DataType: " & DataType)
        End Select
    End Sub

    Friend Sub Serialize(ByVal Data As Object, ByRef Buffer() As Byte, ByVal DataType As Sane_DataType)
        If Buffer Is Nothing Then ReDim Preserve Buffer(-1)
        Me.Serialize(Data, Buffer, Buffer.Length, DataType)
    End Sub

    Friend Function DeSerialize(ByRef Stream As System.Net.Sockets.NetworkStream, ByRef Buffer As System.IO.MemoryStream, ByVal DataType As Sane_DataType) As Object
        Select Case DataType
            Case Sane_DataType.SANE_Word
                Dim int As Int32 = 0
                If CheckBuffer(Stream, Buffer, 4) Then
                    Dim bytes(3) As Byte
                    Buffer.Read(bytes, 0, 4)
                    int = BitConverter.ToInt32(bytes, 0)
                    Return Me.SwapEndian(int)
                Else
                    Throw New Exception("Server sent incomplete data")
                End If
            Case Sane_DataType.SANE_Byte
                If CheckBuffer(Stream, Buffer, 1) Then
                    Return Buffer.ReadByte
                Else
                    Throw New Exception("Server sent incomplete data")
                End If
            Case Sane_DataType.SANE_Char
                Dim c As String
                If CheckBuffer(Stream, Buffer, 1) Then
                    c = BytesToString(Buffer, 1)
                    Return c
                Else
                    Throw New Exception("Server sent incomplete data")
                End If
            Case Sane_DataType.SANE_String
                Dim strlen As Int32
                Dim str As String
                If CheckBuffer(Stream, Buffer, 4) Then
                    strlen = Me.DeSerialize(Stream, Buffer, Sane_DataType.SANE_Word)
                    If CheckBuffer(Stream, Buffer, strlen) Then
                        str = BytesToString(Buffer, strlen)
                        If str IsNot Nothing Then str = str.Substring(0, str.Length - 1) 'ignore the terminating null
                        Return str
                    Else
                        Throw New Exception("Server sent incomplete data")
                    End If
                Else
                    Throw New Exception("Server sent incomplete data")
                End If
            Case Sane_DataType.SANE_Boolean
                Dim bool As Boolean
                bool = CBool(Me.DeSerialize(Stream, Buffer, Sane_DataType.SANE_Word))
                Return bool
            Case Else
                Throw New ArgumentException("Unimplemented Sane_DataType: " & DataType)
        End Select
    End Function

    Private Function CheckBuffer(ByRef Stream As System.Net.Sockets.NetworkStream, ByRef Buffer As System.IO.MemoryStream, ByVal DataLen As Integer) As Boolean
        If Buffer.Position + DataLen <= Buffer.Length Then
            Return True
        Else
            StreamReadAndCopy(Stream, Buffer)
            Return CBool(Buffer.Position + DataLen < Buffer.Length)
        End If
    End Function

    Private Function BytesToString(ByRef buffer As System.IO.MemoryStream, ByVal length As Integer) As String
        Dim s As String = Nothing
        For i = 0 To length - 1
            s += Chr(buffer.ReadByte)
        Next
        Return s
    End Function

    Private Sub StreamReadAndCopy(ByRef NetStream As System.Net.Sockets.NetworkStream, ByRef MemStream As System.IO.MemoryStream)
        StreamReadAndCopy(NetStream, MemStream, 8192)
    End Sub
    Private Sub StreamReadAndCopy(ByRef NetStream As System.Net.Sockets.NetworkStream, ByRef MemStream As System.IO.MemoryStream, ByVal BytesToRead As Integer)
        Dim OriginalPosition As Integer = MemStream.Position
        Dim buf(BytesToRead - 1) As Byte
        Dim Bytes As Integer = NetStream.Read(buf, 0, buf.Length)
        MemStream.Position = MemStream.Length
        MemStream.Write(buf, 0, Bytes)
        MemStream.Position = OriginalPosition
    End Sub

    Private Function ReadEntireStream(ByRef stream As System.Net.Sockets.NetworkStream, ByRef rstream As System.IO.MemoryStream) As Integer
        Do
            StreamReadAndCopy(stream, rstream)
        Loop While stream.DataAvailable
        Return rstream.Length
    End Function

    Private Function ReadWord(ByRef stream As System.Net.Sockets.NetworkStream, ByRef rstream As System.IO.MemoryStream) As Integer
        StreamReadAndCopy(stream, rstream, 4)
        Return rstream.Length
    End Function

    Friend Function Net_Init(ByRef TCPClient As System.Net.Sockets.TcpClient, ByVal Username As String) As SANE_Status
        Logger.Debug("")
        Dim stream As System.Net.Sockets.NetworkStream = Nothing
        Dim rstream As New System.IO.MemoryStream
        Try
            stream = TCPClient.GetStream

            Dim sbuf() As Byte = Nothing
            Dim RPCCode As SANE_Net_Procedure_Number = SANE_Net_Procedure_Number.SANE_NET_INIT
            Me.Serialize(CInt(RPCCode), sbuf, Sane_DataType.SANE_Word)
            Dim ver As Int32 = Me.VersionCode
            Me.Serialize(ver, sbuf, Sane_DataType.SANE_Word)
            Me.Serialize(Username, sbuf, Sane_DataType.SANE_String)

            If stream.CanWrite Then
                stream.Write(sbuf, 0, sbuf.Length)
            Else
                Throw New Exception("Stream does not support writing")
            End If

            If stream.CanRead Then
                Dim ReadBytes As Integer = Me.ReadEntireStream(stream, rstream)
                Dim Status As Int32 = Me.DeSerialize(stream, rstream, Sane_DataType.SANE_Word)
                Dim Version As Int32 = Me.DeSerialize(stream, rstream, Sane_DataType.SANE_Word)
                Dim status_code As SANE_Status = Status
                Logger.Info("My version = {0}.{1}.{2}", Me.SANE_VERSION_MAJOR(Me.VersionCode), Me.SANE_VERSION_MINOR(Me.VersionCode), Me.SANE_VERSION_BUILD(Me.VersionCode))
                Logger.Info("Server version = {0}.{1}.{2}", Me.SANE_VERSION_MAJOR(Version), Me.SANE_VERSION_MINOR(Version), Me.SANE_VERSION_BUILD(Version))
                If Version <> Me.VersionCode Then
                    Logger.Warn("Version mismatch!")
                End If
                Return Status
            Else
                Throw New Exception("Stream does not support reading")
            End If
        Catch ex As Exception
            Logger.Error(ex.Message, ex)
            Throw
        Finally
            stream = Nothing
        End Try
    End Function

    Friend Function Net_Get_Devices(ByRef TCPClient As System.Net.Sockets.TcpClient, ByRef DeviceList() As SANE_Device) As SANE_Status
        Logger.Debug("")

        Dim stream As System.Net.Sockets.NetworkStream = Nothing
        Dim rstream As New System.IO.MemoryStream
        Try
            stream = TCPClient.GetStream
            Dim RPCCode As SANE_Net_Procedure_Number = SANE_Net_Procedure_Number.SANE_NET_GET_DEVICES
            Dim sbuf(0 To 3) As Byte
            Me.Serialize(CInt(RPCCode), sbuf, 0, Sane_DataType.SANE_Word)

            If stream.CanWrite Then
                stream.Write(sbuf, 0, 4)
            Else
                Throw New Exception("Stream does not support writing")
            End If

            If stream.CanRead Then
                Dim bytes As Integer = Me.ReadEntireStream(stream, rstream)

                Dim Status As Int32 = Me.DeSerialize(stream, rstream, Sane_DataType.SANE_Word)
                If Status = SANE_Status.SANE_STATUS_GOOD Then
                    Dim arraylen As Int32
                    arraylen = Me.DeSerialize(stream, rstream, Sane_DataType.SANE_Word)

                    ReDim DeviceList(0 To arraylen - 1 - 1) 'array ends with a null record
                    For i As Integer = 0 To arraylen - 1
                        Dim empty_record As Boolean
                        empty_record = Me.DeSerialize(stream, rstream, Sane_DataType.SANE_Boolean)
                        If Not empty_record Then
                            DeviceList(i) = New SANE_Device
                            DeviceList(i).name = Me.DeSerialize(stream, rstream, Sane_DataType.SANE_String)
                            DeviceList(i).vendor = Me.DeSerialize(stream, rstream, Sane_DataType.SANE_String)
                            DeviceList(i).model = Me.DeSerialize(stream, rstream, Sane_DataType.SANE_String)
                            DeviceList(i).type = Me.DeSerialize(stream, rstream, Sane_DataType.SANE_String)
                        End If
                    Next
                End If
                Return Status
            Else
                Throw New Exception("Stream does not support reading")
            End If
        Catch ex As Exception
            Logger.Error(ex.Message, ex)
            Throw
        Finally
            stream = Nothing
        End Try

    End Function

    Friend Function Net_Open(ByRef TCPClient As System.Net.Sockets.TcpClient, ByVal DeviceName As String, ByRef DeviceHandle As Int32, ByVal Username As String, ByVal Password As String) As SANE_Status
        Logger.Debug("")

        If DeviceName Is Nothing Then DeviceName = ""
        If Username Is Nothing Then Username = ""
        If Password Is Nothing Then Password = ""

        Dim stream As System.Net.Sockets.NetworkStream = Nothing
        Dim rstream As New System.IO.MemoryStream
        Try
            stream = TCPClient.GetStream
            Dim RPCCode As SANE_Net_Procedure_Number = SANE_Net_Procedure_Number.SANE_NET_OPEN
            Dim sbuf(-1) As Byte
            Me.Serialize(CInt(RPCCode), sbuf, Sane_DataType.SANE_Word)
            Me.Serialize(DeviceName, sbuf, Sane_DataType.SANE_String)

            If stream.CanWrite Then
                stream.Write(sbuf, 0, sbuf.Length)
            Else
                Throw New Exception("Stream does not support writing")
            End If

            If stream.CanRead Then
                Dim bytes As Integer = Me.ReadEntireStream(stream, rstream)
                Dim Status As Int32 = Me.DeSerialize(stream, rstream, Sane_DataType.SANE_Word)
                DeviceHandle = Me.DeSerialize(stream, rstream, Sane_DataType.SANE_Word)
                Dim AuthResource As String = Me.DeSerialize(stream, rstream, Sane_DataType.SANE_String)

                'Authenticate if required
                If Not String.IsNullOrEmpty(AuthResource) Then
                    Net_Authorize(TCPClient, AuthResource, Username, Password)
                    rstream = New System.IO.MemoryStream
                    bytes = Me.ReadEntireStream(stream, rstream)
                    Status = Me.DeSerialize(stream, rstream, Sane_DataType.SANE_Word)
                    DeviceHandle = Me.DeSerialize(stream, rstream, Sane_DataType.SANE_Word)
                    AuthResource = Me.DeSerialize(stream, rstream, Sane_DataType.SANE_String)
                End If
                '

                Return Status
            Else
                Throw New Exception("Stream does not support reading")
            End If
        Catch ex As Exception
            Logger.Error(ex.Message, ex)
            Throw
        Finally
            stream = Nothing
        End Try
    End Function

    Friend Sub Net_Cancel(ByRef TCPClient As System.Net.Sockets.TcpClient, ByVal DeviceHandle As Int32)
        Logger.Debug("")
        Dim stream As System.Net.Sockets.NetworkStream = Nothing
        Dim rstream As New System.IO.MemoryStream
        Try
            stream = TCPClient.GetStream
            Dim RPCCode As SANE_Net_Procedure_Number = SANE_Net_Procedure_Number.SANE_NET_CANCEL
            Dim sbuf(-1) As Byte
            Me.Serialize(CInt(RPCCode), sbuf, Sane_DataType.SANE_Word)
            Me.Serialize(DeviceHandle, sbuf, Sane_DataType.SANE_Word)

            If stream.CanWrite Then
                stream.Write(sbuf, 0, sbuf.Length)
            Else
                Throw New Exception("Stream does not support writing")
            End If

            If stream.CanRead Then
                Dim bytes As Integer = Me.ReadEntireStream(stream, rstream)
                Dim Dummy As Int32 = Me.DeSerialize(stream, rstream, Sane_DataType.SANE_Word)
            Else
                Throw New Exception("Stream does not support reading")
            End If
        Catch ex As Exception
            Logger.Error(ex.Message, ex)
            Throw
        Finally
            stream = Nothing
        End Try
    End Sub

    Friend Sub Net_Close(ByRef TCPClient As System.Net.Sockets.TcpClient, ByVal DeviceHandle As Int32)
        Logger.Debug("")
        Dim stream As System.Net.Sockets.NetworkStream = Nothing
        Dim rstream As New System.IO.MemoryStream
        Try
            stream = TCPClient.GetStream
            Dim RPCCode As SANE_Net_Procedure_Number = SANE_Net_Procedure_Number.SANE_NET_CLOSE
            Dim sbuf(-1) As Byte
            Me.Serialize(CInt(RPCCode), sbuf, Sane_DataType.SANE_Word)
            Me.Serialize(DeviceHandle, sbuf, Sane_DataType.SANE_Word)

            If stream.CanWrite Then
                stream.Write(sbuf, 0, sbuf.Length)
            Else
                Throw New Exception("Stream does not support writing")
            End If

            If stream.CanRead Then
                Dim bytes As Integer = Me.ReadEntireStream(stream, rstream)
                Dim Dummy As Int32 = Me.DeSerialize(stream, rstream, Sane_DataType.SANE_Word)
            Else
                Throw New Exception("Stream does not support reading")
            End If
        Catch ex As Exception
            Logger.Error(ex.Message, ex)
            Throw
        Finally
            stream = Nothing
        End Try
    End Sub

    Friend Function Net_Get_Option_Descriptors(ByRef TCPClient As System.Net.Sockets.TcpClient, ByVal DeviceHandle As Int32) As SANE_Option_Descriptor()
        Logger.Debug("")
        Dim stream As System.Net.Sockets.NetworkStream = Nothing
        Dim rstream As New System.IO.MemoryStream
        Try
            stream = TCPClient.GetStream

            Dim RPCCode As SANE_Net_Procedure_Number = SANE_Net_Procedure_Number.SANE_NET_GET_OPTION_DESCRIPTORS
            Dim sbuf(-1) As Byte
            Me.Serialize(CInt(RPCCode), sbuf, Sane_DataType.SANE_Word)
            Me.Serialize(DeviceHandle, sbuf, Sane_DataType.SANE_Word)

            If stream.CanWrite Then
                stream.Write(sbuf, 0, sbuf.Length)
            Else
                Throw New Exception("Stream does not support writing")
            End If

            If stream.CanRead Then
                Dim ReadBytes As Integer = Me.ReadEntireStream(stream, rstream)

                Dim ArrayLen As Int32 = Me.DeSerialize(stream, rstream, Sane_DataType.SANE_Word)
                Dim Descriptors(ArrayLen - 1) As SANE_Option_Descriptor

                For i As Integer = 0 To Descriptors.Length - 1
                    Dim empty_record As Boolean = Me.DeSerialize(stream, rstream, Sane_DataType.SANE_Boolean)
                    If Not empty_record Then

                        Descriptors(i) = New SANE_Option_Descriptor
                        Descriptors(i).name = Me.DeSerialize(stream, rstream, Sane_DataType.SANE_String)
                        Descriptors(i).title = Me.DeSerialize(stream, rstream, Sane_DataType.SANE_String)
                        Descriptors(i).desc = Me.DeSerialize(stream, rstream, Sane_DataType.SANE_String)
                        Descriptors(i).type = Me.DeSerialize(stream, rstream, Sane_DataType.SANE_Word)
                        Descriptors(i).unit = Me.DeSerialize(stream, rstream, Sane_DataType.SANE_Word)
                        Descriptors(i).size = Me.DeSerialize(stream, rstream, Sane_DataType.SANE_Word)
                        Descriptors(i).cap = Me.DeSerialize(stream, rstream, Sane_DataType.SANE_Word)
                        Descriptors(i).constraint_type = Me.DeSerialize(stream, rstream, Sane_DataType.SANE_Word)
                        Select Case Descriptors(i).constraint_type
                            Case SANE_Constraint_Type.SANE_CONSTRAINT_NONE
                                'do nothing
                            Case SANE_Constraint_Type.SANE_CONSTRAINT_STRING_LIST
                                Dim ListLength As Int32 = Me.DeSerialize(stream, rstream, Sane_DataType.SANE_Word)
                                ReDim Descriptors(i).constraint.string_list(ListLength - 1 - 1) 'string array ends with a null
                                For j = 0 To ListLength - 1
                                    Dim str As String = Me.DeSerialize(stream, rstream, Sane_DataType.SANE_String)
                                    If str IsNot Nothing Then Descriptors(i).constraint.string_list(j) = str
                                Next
                                'This is a workaround for the buggy hp3500 backend, which defines the 'mode' option length as 4 bytes but uses 8 bytes.
                                Dim LongestLength As Integer = -1
                                For j = 0 To Descriptors(i).constraint.string_list.Length - 1
                                    LongestLength = Math.Max(LongestLength, Descriptors(i).constraint.string_list(j).Length)
                                Next
                                Descriptors(i).size = Math.Max(Descriptors(i).size, LongestLength + 1) '+1 for the trailing null
                                '
                            Case SANE_Constraint_Type.SANE_CONSTRAINT_WORD_LIST
                                Dim ListLength As Int32 = Me.DeSerialize(stream, rstream, Sane_DataType.SANE_Word) 'the following word is this - 1
                                ListLength = Me.DeSerialize(stream, rstream, Sane_DataType.SANE_Word) 'first element is the count (again, now 1 less)
                                ReDim Descriptors(i).constraint.word_list(ListLength - 1)
                                For j As Integer = 0 To ListLength - 1
                                    Descriptors(i).constraint.word_list(j) = Me.DeSerialize(stream, rstream, Sane_DataType.SANE_Word)
                                Next
                            Case SANE_Constraint_Type.SANE_CONSTRAINT_RANGE
                                empty_record = Me.DeSerialize(stream, rstream, Sane_DataType.SANE_Boolean)
                                If Not empty_record Then
                                    Descriptors(i).constraint.range.min = Me.DeSerialize(stream, rstream, Sane_DataType.SANE_Word)
                                    Descriptors(i).constraint.range.max = Me.DeSerialize(stream, rstream, Sane_DataType.SANE_Word)
                                    Descriptors(i).constraint.range.quant = Me.DeSerialize(stream, rstream, Sane_DataType.SANE_Word)
                                End If
                        End Select
                    Else
                        'it's an empty record
                        'XXX
                    End If
                Next
                Return Descriptors
            Else
                Throw New Exception("Stream does not support reading")
            End If

        Catch ex As Exception
            Logger.Error(ex.Message, ex)
            Throw
        Finally
            stream = Nothing
        End Try

    End Function

    Friend Function Net_Control_Option(ByRef TCPClient As System.Net.Sockets.TcpClient, ByVal Request As SANENetControlOption_Request, ByRef Reply As SANENetControlOption_Reply, ByVal Username As String, ByVal Password As String) As SANE_Status
        Logger.Debug("Option={0}, Action={1}", CurrentDevice.OptionDescriptors(Request.option_).name, Request.action)
        Select Case Request.value_type
            Case SANE_Value_Type.SANE_TYPE_GROUP
                'do nothing; these types don't have values
                Return SANE_Status.SANE_STATUS_INVAL
            Case Else

                Dim stream As System.Net.Sockets.NetworkStream = Nothing
                Dim rstream As New System.IO.MemoryStream

                If Request.values IsNot Nothing Then
                    For i As Integer = 0 To Request.values.Length - 1
                        If Request.values(i) IsNot Nothing Then
                            Logger.Debug("  New value({0}) = '{1}'", i, Request.values(i))
                        Else
                            Logger.Debug("  New value({0}) = 'Nothing'", i)
                        End If
                    Next
                End If

                Try
                    stream = TCPClient.GetStream
                    Dim RPCCode As SANE_Net_Procedure_Number = SANE_Net_Procedure_Number.SANE_NET_CONTROL_OPTION
                    Dim sbuf(-1) As Byte
                    Me.Serialize(CInt(RPCCode), sbuf, Sane_DataType.SANE_Word)
                    Me.Serialize(Request.handle, sbuf, Sane_DataType.SANE_Word)
                    Me.Serialize(Request.option_, sbuf, Sane_DataType.SANE_Word)
                    Me.Serialize(CInt(Request.action), sbuf, Sane_DataType.SANE_Word)
                    Me.Serialize(CInt(Request.value_type), sbuf, Sane_DataType.SANE_Word)
                    Me.Serialize(Request.value_size, sbuf, Sane_DataType.SANE_Word)
                    Dim ReqArrayLen As Int32 = Me.OptionValueArrayLength(Request.value_size, Request.value_type)
                    'Dim Value(Request.value_size - 1) As Byte
                    Select Case Request.value_type
                        Case SANE_Value_Type.SANE_TYPE_BOOL
                            'Dim ArrayLen As Int32 = Request.value_size \ 4
                            Me.Serialize(ReqArrayLen, sbuf, Sane_DataType.SANE_Word)
                            If Request.values Is Nothing Then ReDim Request.values(ReqArrayLen - 1)
                            For i As Integer = 0 To ReqArrayLen - 1
                                Me.Serialize(IIf(Request.values(i) Is Nothing, False, CBool(Request.values(i))), sbuf, Sane_DataType.SANE_Boolean)
                            Next
                        Case SANE_Value_Type.SANE_TYPE_BUTTON
                            Me.Serialize(ReqArrayLen, sbuf, Sane_DataType.SANE_Word)
                        Case SANE_Value_Type.SANE_TYPE_FIXED
                            'Dim ArrayLen As Int32 = Request.value_size \ 4
                            Me.Serialize(ReqArrayLen, sbuf, Sane_DataType.SANE_Word)
                            If Request.values Is Nothing Then ReDim Request.values(ReqArrayLen - 1)
                            For i As Integer = 0 To ReqArrayLen - 1
                                Me.Serialize(IIf(Request.values(i) Is Nothing, 0, SANE_FIX(Request.values(i))), sbuf, Sane_DataType.SANE_Word)
                            Next
                        Case SANE_Value_Type.SANE_TYPE_INT
                            'Dim ArrayLen As Int32 = Request.value_size \ 4
                            Me.Serialize(ReqArrayLen, sbuf, Sane_DataType.SANE_Word)
                            If Request.values Is Nothing Then ReDim Request.values(ReqArrayLen - 1)
                            For i As Integer = 0 To ReqArrayLen - 1
                                Me.Serialize(IIf(Request.values(i) Is Nothing, 0, Request.values(i)), sbuf, Sane_DataType.SANE_Word)
                            Next
                        Case SANE_Value_Type.SANE_TYPE_STRING
                            'XXX could there be an array of strings?
                            If Request.values Is Nothing Then ReDim Request.values(0)
                            Dim s As String = Request.values(0)
                            If s Is Nothing Then s = ""
                            For i As Integer = s.Length To Request.value_size - 1 - 1 'leave room for the terminating null
                                s += Chr(0)
                            Next
                            Me.Serialize(s, sbuf, Sane_DataType.SANE_String)

                    End Select

                    If stream.CanWrite Then
                        stream.Write(sbuf, 0, sbuf.Length)
                    Else
                        Throw New Exception("Stream does not support writing")
                    End If

                    If stream.CanRead Then
                        Dim ReadBytes As Integer = Me.ReadEntireStream(stream, rstream)
                        Reply.status = Me.DeSerialize(stream, rstream, Sane_DataType.SANE_Word)
                        Reply.info = Me.DeSerialize(stream, rstream, Sane_DataType.SANE_Word)
                        Reply.value_type = Me.DeSerialize(stream, rstream, Sane_DataType.SANE_Word)
                        Reply.value_size = Me.DeSerialize(stream, rstream, Sane_DataType.SANE_Word)
                        GetControlOptionReplyValues(Reply, stream, rstream)
                        Reply.resource = Me.DeSerialize(stream, rstream, Sane_DataType.SANE_String)

                        'Authenticate if required
                        If Not String.IsNullOrEmpty(Reply.resource) Then
                            Net_Authorize(TCPClient, Reply.resource, Username, Password)
                            rstream = New System.IO.MemoryStream
                            ReadBytes = Me.ReadEntireStream(stream, rstream)
                            Reply.status = Me.DeSerialize(stream, rstream, Sane_DataType.SANE_Word)
                            Reply.info = Me.DeSerialize(stream, rstream, Sane_DataType.SANE_Word)
                            Reply.value_type = Me.DeSerialize(stream, rstream, Sane_DataType.SANE_Word)
                            Reply.value_size = Me.DeSerialize(stream, rstream, Sane_DataType.SANE_Word)
                            GetControlOptionReplyValues(Reply, stream, rstream)
                            Reply.resource = Me.DeSerialize(stream, rstream, Sane_DataType.SANE_String)
                        End If
                        '

                        Return Reply.status
                    Else
                        Throw New Exception("Stream does not support reading")
                    End If
                Catch ex As Exception
                    Throw
                Finally
                    stream = Nothing
                End Try
        End Select
    End Function

    Private Sub GetControlOptionReplyValues(ByRef Reply As SANENetControlOption_Reply, ByRef stream As System.Net.Sockets.NetworkStream, ByRef rstream As System.IO.MemoryStream)
        Select Case Reply.value_type
            Case SANE_Value_Type.SANE_TYPE_BOOL
                Dim ArrayLen As Int32 = Me.DeSerialize(stream, rstream, Sane_DataType.SANE_Word)
                ReDim Reply.values(ArrayLen - 1)
                For i As Integer = 0 To ArrayLen - 1
                    Reply.values(i) = Me.DeSerialize(stream, rstream, Sane_DataType.SANE_Boolean)
                Next
            Case SANE_Value_Type.SANE_TYPE_BUTTON
                Dim ArrayLen As Int32 = Me.DeSerialize(stream, rstream, Sane_DataType.SANE_Word)
                ReDim Reply.values(ArrayLen - 1)
            Case SANE_Value_Type.SANE_TYPE_FIXED
                Dim ArrayLen As Int32 = Me.DeSerialize(stream, rstream, Sane_DataType.SANE_Word)
                ReDim Reply.values(ArrayLen - 1)
                For i As Integer = 0 To ArrayLen - 1
                    Reply.values(i) = SANE_UNFIX(Me.DeSerialize(stream, rstream, Sane_DataType.SANE_Word))
                Next
            Case SANE_Value_Type.SANE_TYPE_INT
                Dim ArrayLen As Int32 = Me.DeSerialize(stream, rstream, Sane_DataType.SANE_Word)
                ReDim Reply.values(ArrayLen - 1)
                For i As Integer = 0 To ArrayLen - 1
                    Reply.values(i) = Me.DeSerialize(stream, rstream, Sane_DataType.SANE_Word)
                Next
            Case SANE_Value_Type.SANE_TYPE_STRING
                'XXX could there be an array of strings?
                ReDim Reply.values(0)
                Reply.values(0) = Me.DeSerialize(stream, rstream, Sane_DataType.SANE_String)
                If Reply.values(0) IsNot Nothing Then Reply.values(0) = Reply.values(0).Replace(Chr(0), "")
        End Select
    End Sub

    Friend Function Net_Get_Parameters(ByRef TCPClient As System.Net.Sockets.TcpClient, ByVal DeviceHandle As Int32, ByRef Params As SANE_Parameters) As SANE_Status
        Logger.Debug("")
        Dim stream As System.Net.Sockets.NetworkStream = Nothing
        Dim rstream As New System.IO.MemoryStream
        Try
            stream = TCPClient.GetStream
            Dim RPCCode As SANE_Net_Procedure_Number = SANE_Net_Procedure_Number.SANE_NET_GET_PARAMETERS
            Dim sbuf(-1) As Byte
            Me.Serialize(CInt(RPCCode), sbuf, Sane_DataType.SANE_Word)
            Me.Serialize(DeviceHandle, sbuf, Sane_DataType.SANE_Word)

            If stream.CanWrite Then
                stream.Write(sbuf, 0, sbuf.Length)
            Else
                Throw New Exception("Stream does not support writing")
            End If

            If stream.CanRead Then
                Dim bytes As Integer = Me.ReadEntireStream(stream, rstream)

                Dim Status As SANE_Status = Me.DeSerialize(stream, rstream, Sane_DataType.SANE_Word)

                Params.format = Me.DeSerialize(stream, rstream, Sane_DataType.SANE_Word)
                Params.last_frame = CBool(Me.DeSerialize(stream, rstream, Sane_DataType.SANE_Word))
                Params.bytes_per_line = Me.DeSerialize(stream, rstream, Sane_DataType.SANE_Word)
                Params.pixels_per_line = Me.DeSerialize(stream, rstream, Sane_DataType.SANE_Word)
                Params.lines = Me.DeSerialize(stream, rstream, Sane_DataType.SANE_Word)
                Params.depth = Me.DeSerialize(stream, rstream, Sane_DataType.SANE_Word)

                Return Status
            Else
                Throw New Exception("Stream does not support reading")
            End If
        Catch ex As Exception
            Throw
        Finally
            stream = Nothing
        End Try

    End Function

    Friend Function Net_Start(ByRef TCPClient As System.Net.Sockets.TcpClient, ByVal DeviceHandle As Int32, ByRef Port As Int32, ByRef ByteOrder As SANE_Net_Byte_Order, ByVal Username As String, ByVal Password As String) As SANE_Status
        Logger.Debug("")
        Dim stream As System.Net.Sockets.NetworkStream = Nothing
        Dim rstream As New System.IO.MemoryStream
        Try
            stream = TCPClient.GetStream
            Dim RPCCode As SANE_Net_Procedure_Number = SANE_Net_Procedure_Number.SANE_NET_START
            Dim sbuf(-1) As Byte
            Me.Serialize(CInt(RPCCode), sbuf, Sane_DataType.SANE_Word)
            Me.Serialize(DeviceHandle, sbuf, Sane_DataType.SANE_Word)

            If stream.CanWrite Then
                stream.Write(sbuf, 0, sbuf.Length)
            Else
                Throw New Exception("Stream does not support writing")
            End If

            If stream.CanRead Then
                Dim bytes As Integer = Me.ReadEntireStream(stream, rstream)
                Dim Status As SANE_Status = Me.DeSerialize(stream, rstream, Sane_DataType.SANE_Word)
                Port = Me.DeSerialize(stream, rstream, Sane_DataType.SANE_Word)
                ByteOrder = Me.DeSerialize(stream, rstream, Sane_DataType.SANE_Word)
                Dim AuthResource As String = Me.DeSerialize(stream, rstream, Sane_DataType.SANE_String)

                'Authenticate if required
                If Not String.IsNullOrEmpty(AuthResource) Then
                    Net_Authorize(TCPClient, AuthResource, Username, Password)
                    rstream = New System.IO.MemoryStream
                    bytes = Me.ReadEntireStream(stream, rstream)
                    Status = Me.DeSerialize(stream, rstream, Sane_DataType.SANE_Word)
                    Port = Me.DeSerialize(stream, rstream, Sane_DataType.SANE_Word)
                    ByteOrder = Me.DeSerialize(stream, rstream, Sane_DataType.SANE_Word)
                    AuthResource = Me.DeSerialize(stream, rstream, Sane_DataType.SANE_String)
                End If
                '

                Return Status
            Else
                Throw New Exception("Stream does not support reading")
            End If
        Catch ex As Exception
            Throw
        Finally
            stream = Nothing
        End Try
    End Function

    Friend Sub Net_Authorize(ByRef TCPClient As System.Net.Sockets.TcpClient, ByVal Resource As String, ByVal Username As String, ByVal Password As String)
        Logger.Debug("")
        If TCPClient Is Nothing Then Throw New ArgumentNullException("TCPClient")
        If Resource Is Nothing Then Throw New ArgumentNullException("Resource")
        'If Username Is Nothing Then Throw New ArgumentNullException("Username")
        If Username Is Nothing Then Username = ""
        'If Password Is Nothing Then Throw New ArgumentNullException("Password")
        If Password Is Nothing Then Password = ""

        If Not String.IsNullOrEmpty(Resource) Then
            Dim MD5_String As String = Nothing
            If Resource.ToUpper.Contains("$MD5$") Then
                Dim p As Integer = Resource.ToUpper.IndexOf("$MD5$")
                If (p > 0) And ((p + 5) < Resource.Length) Then
                    MD5_String = Resource.Substring(p + 5)
                    If MD5_String.Length <= 128 Then
                        'Resource = Resource.Substring(0, p)
                        Dim md5 As System.Security.Cryptography.MD5 = System.Security.Cryptography.MD5.Create()
                        Dim data As Byte() = md5.ComputeHash(System.Text.Encoding.Default.GetBytes(MD5_String & Password))
                        Dim sBuilder As New System.Text.StringBuilder()
                        For i As Integer = 0 To data.Length - 1
                            sBuilder.Append(data(i).ToString("x2"))
                        Next
                        Password = "$MD5$" & sBuilder.ToString()
                    Else
                        Throw New Exception("The MD5 portion of the resource string is too long")
                    End If
                Else
                    Throw New Exception("The resource string is malformed")
                End If
            End If

            Dim stream As System.Net.Sockets.NetworkStream = Nothing
            Dim rstream As New System.IO.MemoryStream
            Try
                stream = TCPClient.GetStream
                Dim RPCCode As SANE_Net_Procedure_Number = SANE_Net_Procedure_Number.SANE_NET_AUTHORIZE
                Dim sbuf(-1) As Byte
                Me.Serialize(CInt(RPCCode), sbuf, Sane_DataType.SANE_Word)
                Me.Serialize(Resource, sbuf, Sane_DataType.SANE_String)
                Me.Serialize(Username, sbuf, Sane_DataType.SANE_String)
                Me.Serialize(Password, sbuf, Sane_DataType.SANE_String)

                If stream.CanWrite Then
                    stream.Write(sbuf, 0, sbuf.Length)
                Else
                    Throw New Exception("Stream does not support writing")
                End If

                If stream.CanRead Then
                    'Dim bytes As Integer = Me.ReadEntireStream(stream, rstream)
                    Dim bytes As Integer = Me.ReadWord(stream, rstream)
                    Dim Dummy As Int32 = Me.DeSerialize(stream, rstream, Sane_DataType.SANE_Word) 'Dummy reply is unused and meaningless
                Else
                    Throw New Exception("Stream does not support reading")
                End If

            Catch ex As Exception
                Throw
            Finally
                stream = Nothing
            End Try
        Else
            Throw New Exception("Resource cannot be Nothing")
        End If
    End Sub

    Friend Sub Net_Exit(ByRef TCPClient As System.Net.Sockets.TcpClient)
        Logger.Debug("")
        Dim stream As System.Net.Sockets.NetworkStream = Nothing
        Try
            stream = TCPClient.GetStream
            Dim RPCCode As SANE_Net_Procedure_Number = SANE_Net_Procedure_Number.SANE_NET_EXIT
            Dim buf(0 To 3) As Byte
            Me.Serialize(CInt(RPCCode), buf, 0, Sane_DataType.SANE_Word)

            If stream.CanWrite Then
                stream.Write(buf, 0, buf.Length)
            Else
                Throw New Exception("Stream does not support writing")
            End If
        Catch ex As Exception
            Throw
        Finally
            stream = Nothing
        End Try
    End Sub

    Friend Function AcquireFrame(ByRef Control_TCPClient As System.Net.Sockets.TcpClient, ByVal Port As Integer, ByVal ByteOrder As SANE_Net_Byte_Order, ByVal TCP_Timeout_ms As Integer) As SANEImageFrame
        Dim EndPoint As System.Net.IPEndPoint = Control_TCPClient.Client.RemoteEndPoint
        Dim HostIP As System.Net.IPAddress = EndPoint.Address

        TCP_Timeout_ms = Math.Max(TCP_Timeout_ms, 5000) 'Make sure it's at least 5 seconds

        Logger.Debug("Image data port = " & Port.ToString)
        Logger.Debug("Byte order = " & ByteOrder.ToString)
        Logger.Debug("Receive timeout = " & TCP_Timeout_ms & "ms")

        Dim Data_TCPClient As System.Net.Sockets.TcpClient = New Net.Sockets.TcpClient
        Data_TCPClient.ReceiveTimeout = TCP_Timeout_ms
        Data_TCPClient.ReceiveBufferSize = 32768
        Data_TCPClient.SendBufferSize = 32768
        Data_TCPClient.Connect(HostIP, Port) 'must connect on the data port before calling Net_Get_Parameters on the control port!

        Dim Frame As New SANEImageFrame
        Dim Status As Int32 = Me.Net_Get_Parameters(Control_TCPClient, Me.CurrentDevice.Handle, Frame.Params)

        Logger.Debug("Beginning image transfer")

        Dim stream As System.Net.Sockets.NetworkStream = Nothing
        Dim ImageStream As System.IO.MemoryStream = Nothing
        Try
            Dim StartTime As DateTime = Now
            Dim TransferredBytes As UInt32 = 0

            stream = Data_TCPClient.GetStream

            ImageStream = New System.IO.MemoryStream
            Dim ImageBuf(Data_TCPClient.ReceiveBufferSize - 1) As Byte

            Dim Expected_Total_Bytes As UInt32 = 0
            If Frame.Params.lines > 0 Then
                Expected_Total_Bytes = Frame.Params.lines * Frame.Params.bytes_per_line
                Logger.Debug("Expecting {0} lines of {1} bytes each, for a total of {2} bytes", Frame.Params.lines, Frame.Params.bytes_per_line, Expected_Total_Bytes)
            Else
                Logger.Debug("The backend doesn't know how many lines this frame will have")
            End If

            Dim ReportProgressAt As Integer = 1 'The next percentage point at which to report progress
            Dim LastReportedProgress As Date = Date.MinValue

            Dim LastGoodRead As Date = Now
            Do
                If Client_Is_Connected(Data_TCPClient) Then

                    Dim buf(3) As Byte
                    Dim bytes_read As Integer = 0
                    Do
                        Dim n As Integer = stream.Read(buf, bytes_read, 1)
                        bytes_read += n
                        If n > 0 Then
                            LastGoodRead = Now
                        Else
                            If Not Client_Is_Connected(Data_TCPClient) Then Throw New ServerDisconnectedException("Server disconnected during frame transmission")
                        End If
                        If Now > LastGoodRead.AddMilliseconds(TCP_Timeout_ms) Then Throw New Exception("Timeout waiting for image data")
                    Loop While bytes_read < 4

                    Dim datalen As UInt32 = BitConverter.ToUInt32(buf, 0)
                    datalen = Me.SwapEndian(datalen)
                    If datalen = CUInt(&HFFFFFFFFUI) Then
                        Logger.Debug("Server sent EOF")
                        If (Expected_Total_Bytes > 0) AndAlso (TransferredBytes < Expected_Total_Bytes) Then
                            Logger.Debug("Server underran the expected byte count (" & TransferredBytes.ToString & " < " & Expected_Total_Bytes.ToString & ")")
                        End If
                        'canondr (not canon_dr) backend from Canon's web site never returns SANE_STATUS_NO_DOCS but instead returns empty frames.
                        'avision backend sometimes returns empty frames as well.
                        If TransferredBytes = 0 Then Throw New EmptyFrameException
                        Exit Do
                    ElseIf (Expected_Total_Bytes > 0) AndAlso (TransferredBytes >= Expected_Total_Bytes) Then
                        Logger.Debug("Server overran the expected byte count without sending EOF; aborting")
                        Exit Do
                    Else
                        Logger.Debug("Server said to expect " & datalen.ToString & " bytes")
                    End If

                    'datalen is typically ReceiveBufferSize - 4 (the length of datalen itself).
                    If (datalen < Data_TCPClient.ReceiveBufferSize) Then
                        Dim TotalBytes As UInt32 = 0
                        Do
                            Dim ImageBytes As UInt32 = stream.Read(ImageBuf, 0, datalen - TotalBytes)
                            Logger.Debug("Received " & ImageBytes.ToString & " bytes")
                            ImageStream.Write(ImageBuf, 0, ImageBytes)
                            TotalBytes += ImageBytes
                            If ImageBytes > 0 Then
                                LastGoodRead = Now
                            Else
                                If Not Client_Is_Connected(Data_TCPClient) Then Throw New ServerDisconnectedException("Server disconnected during frame transmission")
                            End If
                            If Now > LastGoodRead.AddMilliseconds(TCP_Timeout_ms) Then Throw New Exception("Timeout waiting for image data")
                        Loop While TotalBytes < datalen
                        TransferredBytes += TotalBytes
                        Logger.Debug("Received a total of " & TransferredBytes.ToString & " bytes so far")
                    Else
                        Logger.Warn("The value of the data length field looks bogus: &H" & Hex(datalen) & "; aborting transfer")
                        'If this happens the remaining image data will be filled with zeros below.  
                        Exit Do
                    End If
                Else
                    Throw New ServerDisconnectedException("Server disconnected during frame transmission")
                End If

                'Update progress for a GUI to consume
                If Expected_Total_Bytes > 0 Then
                    Dim Progress As Integer = (TransferredBytes / Expected_Total_Bytes) * 100
                    If Progress > ReportProgressAt Then
                        If Now > LastReportedProgress.AddMilliseconds(2000) Then 'don't report progress any more frequently than every 2 seconds.
                            RaiseEvent FrameProgress(Progress)
                            LastReportedProgress = Now
                            ReportProgressAt = Progress + 10
                        End If
                    End If
                Else 'we don't know how many bytes to expect (hand scanner).  Report -1 progress only once.  GUI should use a marquee-style progress bar.
                    If ReportProgressAt = 1 Then
                        ReportProgressAt += 1
                        RaiseEvent FrameProgress(-1)
                    End If
                End If
                '
            Loop

            If Expected_Total_Bytes > 0 Then RaiseEvent FrameProgress(100)

            'This was a workaround for a bug that has been fixed.  It might still be useful in some unforseen circumstance.
            If TransferredBytes < Expected_Total_Bytes Then
                Dim Filler(Expected_Total_Bytes - TransferredBytes) As Byte
                ImageStream.Write(Filler, 0, Filler.Length)
                ReDim Filler(-1)
            End If

            Dim TransferredMbits As UInt64 = (TransferredBytes * 8UL) / 1024 / 1024
            Dim ElapsedSeconds As Double = (Now - StartTime).TotalSeconds
            If ElapsedSeconds < 0.001 Then ElapsedSeconds = 0.001 'avoid divide by zero
            Logger.Debug("Transferred " & TransferredBytes.ToString & " bytes in " & ElapsedSeconds.ToString("0.00") & " seconds with a throughput of " & (TransferredMbits / ElapsedSeconds).ToString("0.00") & " Mbps")

            ReDim Frame.Data(ImageStream.Length - 1)
            Frame.Data = ImageStream.ToArray

            Return Frame

        Catch ex As EmptyFrameException
            Throw
            'Catch ex As System.Net.Sockets.SocketException
            '    Logger.Error("Error acquiring image frame: " & ex.SocketErrorCode, ex)
            '    Throw
            'Catch ex As ServerDisconnectedException
            '    Logger.Error("Error acquiring image frame: " & ex.Message, ex)
            '    Throw
        Catch ex As Exception
            Logger.Error("Error acquiring image frame: " & ex.Message, ex)
            Throw
        Finally
            Try
                If ImageStream IsNot Nothing Then ImageStream.Close()
            Catch ex As Exception
                Logger.Debug(ex.Message, ex)
            End Try
            'If stream IsNot Nothing Then
            '    Dim buf(Data_TCPClient.ReceiveBufferSize - 1) As Byte
            '    Do While stream.DataAvailable 'hopefully prevent the server from staying in SANE_STATUS_BUSY
            '        stream.Read(buf, 0, buf.Length)
            '    Loop
            '    stream.Close()
            'End If
            Try
                If stream IsNot Nothing Then stream.Close()
            Catch ex As Exception
                Logger.Debug(ex.Message, ex)
            End Try
            Try
                If Data_TCPClient IsNot Nothing Then Data_TCPClient.Client.Shutdown(Net.Sockets.SocketShutdown.Both)
            Catch ex As Exception
                Logger.Debug(ex.Message, ex)
            End Try
            Try
                If Data_TCPClient IsNot Nothing Then Data_TCPClient.Close()
            Catch ex As Exception
                Logger.Debug(ex.Message, ex)
            End Try
            ImageStream = Nothing
            stream = Nothing
            Data_TCPClient = Nothing
        End Try

    End Function

    Friend Function SwapEndian(ByVal _UInt32 As UInt32) As UInt32
        Dim n As UInt32
        n = (_UInt32 And &HFF) << 24
        n += (_UInt32 And &HFF00) << 8
        n += (_UInt32 And &HFF0000) >> 8
        n += (_UInt32 And &HFF000000) >> 24
        Return n
    End Function

    Friend Function SwapEndian(ByVal _Int32 As Int32) As Int32
        Return System.Net.IPAddress.NetworkToHostOrder(_Int32)
    End Function

End Class
