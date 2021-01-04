'
'   Copyright 2011, 2019 Alec Skelly
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
    '*** PLEASE NOTE ***
    'This class is only intended to protect against casual viewing of sensitive information.
    'Anyone with access to this source code can very easily decrypt the encrypted data.
    '*******************

    Private lbtVector8() As Byte = {240, 3, 45, 29, 0, 76, 173, 59}
    Private lbtVector16() As Byte = {89, 44, 209, 1, 34, 3, 109, 52, 17, 24, 9, 9, 13, 67, 50, 0}
    Private lscryptoKey As String = "_S@N3W1nDS!"

    Public Enum EncryptionMethod As Integer
        None = 0
        TripleDES_MD5 = 1
        AES_SHA256 = 2
    End Enum

    Public Function IsEncrypted(sQueryString As String) As Boolean
        If String.IsNullOrEmpty(sQueryString) Then Return False
        If sQueryString.Length < 7 Then Return False
        Return (Left(sQueryString, 3) = "{-!" AndAlso Right(sQueryString, 3) = "!-}") Or (Left(sQueryString, 3) = "{=!" AndAlso Right(sQueryString, 3) = "!=}")
    End Function

    Public Function GetEncryptionMethod(sQueryString As String) As EncryptionMethod
        If (Left(sQueryString, 3) = "{-!" AndAlso Right(sQueryString, 3) = "!-}") Then
            Return EncryptionMethod.TripleDES_MD5
        ElseIf (Left(sQueryString, 3) = "{=!" AndAlso Right(sQueryString, 3) = "!=}") Then
            Return EncryptionMethod.AES_SHA256
        Else
            Return EncryptionMethod.None
        End If
    End Function

    Public Function Decrypt(ByVal sQueryString As String) As String
        If String.IsNullOrEmpty(sQueryString) Then Throw New ArgumentNullException("sQueryString")
        Dim buffer() As Byte
        Select Case GetEncryptionMethod(sQueryString)
            Case EncryptionMethod.TripleDES_MD5
                sQueryString = sQueryString.Substring(3, sQueryString.Length - 6)
                Dim loCryptoClass As New TripleDESCryptoServiceProvider
                Dim loCryptoProvider As New MD5CryptoServiceProvider
                Try
                    buffer = Convert.FromBase64String(sQueryString)
                    loCryptoClass.Key = loCryptoProvider.ComputeHash(ASCIIEncoding.ASCII.GetBytes(lscryptoKey))
                    loCryptoClass.IV = lbtVector8
                    Return Encoding.ASCII.GetString(loCryptoClass.CreateDecryptor().TransformFinalBlock(buffer, 0, buffer.Length()))
                Catch ex As Exception
                    Throw
                Finally
                    loCryptoClass.Clear()
                    loCryptoProvider.Clear()
                    loCryptoClass = Nothing
                    loCryptoProvider = Nothing
                End Try
            Case EncryptionMethod.AES_SHA256
                sQueryString = sQueryString.Substring(3, sQueryString.Length - 6)
                Dim loCryptoClass As New AesCryptoServiceProvider
                Dim loCryptoProvider As New SHA256CryptoServiceProvider
                Try
                    buffer = Convert.FromBase64String(sQueryString)
                    loCryptoClass.Key = loCryptoProvider.ComputeHash(ASCIIEncoding.ASCII.GetBytes(lscryptoKey))
                    loCryptoClass.IV = lbtVector16
                    Return Encoding.ASCII.GetString(loCryptoClass.CreateDecryptor().TransformFinalBlock(buffer, 0, buffer.Length()))
                Catch ex As Exception
                    Throw
                Finally
                    loCryptoClass.Clear()
                    loCryptoProvider.Clear()
                    loCryptoClass = Nothing
                    loCryptoProvider = Nothing
                End Try
            Case Else
                Throw New SimpleCryptoExceptions.SuppliedStringNotEncryptedException
        End Select
    End Function

    Public Function Encrypt(ByVal sInputVal As String) As String
        If String.IsNullOrEmpty(sInputVal) Then Throw New ArgumentNullException("sInputVal")
        Dim loCryptoClass As New AesCryptoServiceProvider
        Dim loCryptoProvider As New SHA256CryptoServiceProvider
        Dim lbtBuffer() As Byte
        Try
            lbtBuffer = System.Text.Encoding.ASCII.GetBytes(sInputVal)
            loCryptoClass.Key = loCryptoProvider.ComputeHash(ASCIIEncoding.ASCII.GetBytes(lscryptoKey))
            loCryptoClass.IV = lbtVector16
            sInputVal = Convert.ToBase64String(loCryptoClass.CreateEncryptor().TransformFinalBlock(lbtBuffer, 0, lbtBuffer.Length()))
            Return "{=!" & sInputVal & "!=}"
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
