Imports System.Text
Imports System.Collections.Specialized
Imports System.Security.Cryptography

Namespace SimpleCryptoExceptions
    Public Class SuppliedStringNotEncryptedException
        Inherits System.ApplicationException
        Public Sub New()
            MyBase.New("The supplied string is not encrypted.")
        End Sub
    End Class
End Namespace

Public Class SimpleCrypto

    Private lbtVector() As Byte = {240, 3, 45, 29, 0, 76, 173, 59}
    Private lscryptoKey As String = "_S@N3W1nDS!"

    Public Function IsEncrypted(sQueryString As String) As Boolean
        If String.IsNullOrEmpty(sQueryString) Then Return False
        If sQueryString.Length < 7 Then Return False
        Return (Left(sQueryString, 3) = "{-!" AndAlso Right(sQueryString, 3) = "!-}")
    End Function

    Public Function Decrypt(ByVal sQueryString As String) As String
        If String.IsNullOrEmpty(sQueryString) Then Throw New ArgumentNullException("sQueryString")
        Dim buffer() As Byte
        Dim loCryptoClass As New TripleDESCryptoServiceProvider
        Dim loCryptoProvider As New MD5CryptoServiceProvider
        If IsEncrypted(sQueryString) Then
            sQueryString = Replace(sQueryString, "{-!", "")
            sQueryString = Replace(sQueryString, "!-}", "")
        Else
            Throw New SimpleCryptoExceptions.SuppliedStringNotEncryptedException
        End If
        Try
            buffer = Convert.FromBase64String(sQueryString)
            loCryptoClass.Key = loCryptoProvider.ComputeHash(ASCIIEncoding.ASCII.GetBytes(lscryptoKey))
            loCryptoClass.IV = lbtVector
            Return Encoding.ASCII.GetString(loCryptoClass.CreateDecryptor().TransformFinalBlock(buffer, 0, buffer.Length()))
        Catch ex As Exception
            Throw
        Finally
            loCryptoClass.Clear()
            loCryptoProvider.Clear()
            loCryptoClass = Nothing
            loCryptoProvider = Nothing
        End Try
    End Function

    Public Function Encrypt(ByVal sInputVal As String) As String
        If String.IsNullOrEmpty(sInputVal) Then Throw New ArgumentNullException("sInputVal")
        Dim loCryptoClass As New TripleDESCryptoServiceProvider
        Dim loCryptoProvider As New MD5CryptoServiceProvider
        Dim lbtBuffer() As Byte
        Try
            lbtBuffer = System.Text.Encoding.ASCII.GetBytes(sInputVal)
            loCryptoClass.Key = loCryptoProvider.ComputeHash(ASCIIEncoding.ASCII.GetBytes(lscryptoKey))
            loCryptoClass.IV = lbtVector
            sInputVal = Convert.ToBase64String(loCryptoClass.CreateEncryptor().TransformFinalBlock(lbtBuffer, 0, lbtBuffer.Length()))
            Encrypt = "{-!" & sInputVal & "!-}"
        Catch ex As CryptographicException
            Throw
        Catch ex As FormatException
            Throw
        Catch ex As Exception
            Throw
        Finally
            loCryptoClass.Clear()
            loCryptoProvider.Clear()
            loCryptoClass = Nothing
            loCryptoProvider = Nothing
        End Try
    End Function
End Class
