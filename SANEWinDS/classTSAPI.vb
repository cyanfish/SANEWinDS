Option Explicit On
Option Strict On
Imports System.Runtime.InteropServices

Public Class TSAPI
    Public Const WTS_CURRENT_SERVER_HANDLE As Int32 = 0
    Public Const WTS_CURRENT_SESSION As Int32 = -1

    Private Enum WTS_CONNECTSTATE_CLASS
        WTSActive
        WTSConnected
        WTSConnectQuery
        WTSShadow
        WTSDisconnected
        WTSIdle
        WTSListen
        WTSReset
        WTSDown
        WTSInit
    End Enum

    Private Enum WTS_INFO_CLASS
        WTSInitialProgram
        WTSApplicationName
        WTSWorkingDirectory
        WTSOEMId
        WTSSessionId
        WTSUserName
        WTSWinStationName
        WTSDomainName
        WTSConnectState
        WTSClientBuildNumber
        WTSClientName
        WTSClientDirectory
        WTSClientProductId
        WTSClientHardwareId
        WTSClientAddress
        WTSClientDisplay
        WTSClientProtocolType
        WTSIdleTime
        WTSLogonTime
        WTSIncomingBytes
        WTSOutgoingBytes
        WTSIncomingFrames
        WTSOutgoingFrames
        WTSClientInfo
        WTSSessionInfo
    End Enum

    Private Enum ADDRESS_FAMILY
        AF_UNSPEC = 0
        AF_INET = 2
        AF_IPX = 6
        AF_NETBIOS = 17
    End Enum

    <StructLayout(LayoutKind.Sequential, CharSet:=CharSet.Auto)> _
    Private Structure WTS_SESSION_INFO
        Dim SessionID As Int32 'DWORD integer
        <MarshalAs(UnmanagedType.LPStr)>
        Dim pWinStationName As String ' integer LPTSTR - Pointer to a null-terminated string containing the name of the WinStation for this session
        Dim State As WTS_CONNECTSTATE_CLASS
    End Structure

    <StructLayout(LayoutKind.Sequential, CharSet:=CharSet.Auto)> _
    Private Structure WTS_CLIENT_ADDRESS
        Dim iAddressFamily As Int32
        <MarshalAs(UnmanagedType.ByValArray, SizeConst:=20)>
        Dim bAddress() As Byte
    End Structure

    <DllImport("wtsapi32.dll", _
    BestFitMapping:=True, _
    CallingConvention:=CallingConvention.StdCall, _
    CharSet:=CharSet.Auto, _
    EntryPoint:="WTSQuerySessionInformation", _
    SetLastError:=True, _
    ThrowOnUnmappableChar:=True)> _
    Private Shared Function WTSQuerySessionInformation( _
        ByVal hServer As IntPtr, _
        <MarshalAs(UnmanagedType.U4)> ByVal SessionId As Int32, _
        ByVal InfoClass As WTS_INFO_CLASS, _
        ByRef ppBuffer As IntPtr, _
       <MarshalAs(UnmanagedType.U4)> ByRef pCount As Int32) As Int32
    End Function

    <DllImport("wtsapi32.dll")> _
    Private Shared Sub WTSFreeMemory(ByVal pMemory As IntPtr)
    End Sub

    Public Function GetCurrentSessionIP() As String
        Dim FRetVal As Int32
        Dim ppBuffer As IntPtr = IntPtr.Zero
        Dim BufferLen As Int32 = 0
        FRetVal = WTSQuerySessionInformation(CType(WTS_CURRENT_SERVER_HANDLE, IntPtr), WTS_CURRENT_SESSION, WTS_INFO_CLASS.WTSClientAddress, ppBuffer, BufferLen)
        If FRetVal <> 0 Then
            Dim Address As WTS_CLIENT_ADDRESS = CType(Marshal.PtrToStructure(ppBuffer, GetType(WTS_CLIENT_ADDRESS)), WTS_CLIENT_ADDRESS)
            WTSFreeMemory(ppBuffer)
            If Address.iAddressFamily = ADDRESS_FAMILY.AF_INET Then
                Return Address.bAddress(2).ToString & "." & Address.bAddress(3).ToString & "." & Address.bAddress(4).ToString & "." & Address.bAddress(5).ToString
            Else
                Throw New ApplicationException("Address Family '" & CType(Address.iAddressFamily, ADDRESS_FAMILY).ToString & "' is not currently supported.")
            End If
        Else
            Throw New ApplicationException("Call to WTSQuerySessionInformation failed.")
        End If
            Return Nothing
    End Function
End Class
