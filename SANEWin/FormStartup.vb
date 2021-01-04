
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
Imports SANEWinDS
Public Class FormStartup
    Private WithEvents GUIForm As New SANEWinDS.FormMain

    Private Structure PDFInfo
        'Dim FileName As String
        Dim FileStream As System.IO.FileStream
        Dim iTextDocument As iText.Layout.Document
        Dim iTextPDFDocument As iText.Kernel.Pdf.PdfDocument
        Dim iTextWriter As iText.Kernel.Pdf.PdfWriter
        Dim iTextPageSize As iText.Kernel.Geom.Rectangle
    End Structure
    Public Enum TIFFCompressionMethod As Byte
        None = 0
        CCITT4 = 1
        RLE = 2
        LZW = 3
    End Enum
    Private Structure TIFFInfo
        Dim FirstPage As Bitmap
        'Dim FileName As String
        'Dim FileStream As System.IO.FileStream
        Dim ImageCodecInfo As System.Drawing.Imaging.ImageCodecInfo
        Dim SaveEncoder As System.Drawing.Imaging.Encoder
        Dim CompressionEncoder As System.Drawing.Imaging.Encoder
        Dim EncoderParameters As System.Drawing.Imaging.EncoderParameters
        Dim CompressionMethod As TIFFCompressionMethod
    End Structure
    Public Enum ImageType As Byte
        BMP = 0
        PDF = 1
        TIFF = 2
        JPEG = 3
        GIF = 4
        WMF = 5
        EMF = 6
        PNG = 7
    End Enum
    Private Structure ImageInfo
        Dim FileName As String
        Dim FileNamePage1 As String
        Dim ImageType As ImageType
        Dim TIFF As TIFFInfo
        Dim PDF As PDFInfo
    End Structure
    'Public ProductName As System.Reflection.AssemblyName = System.Reflection.Assembly.GetExecutingAssembly.GetName

    Private INIFileName As String
    'Private INI As New IniFile.IniFile
    Private UserConfigDirectory As String
    Private SharedConfigDirectory As String
    Private LogDirectory As String
    Private CurrentImage As ImageInfo
    Private GUIForm_Shown As Boolean = False
    Private OutputFolderMRU As New SortedList(Of DateTime, String)
    Private Sub FormStartup_FormClosed(sender As Object, e As FormClosedEventArgs) Handles Me.FormClosed
        If Me.INIFileName IsNot Nothing Then

            If Not String.IsNullOrWhiteSpace(Me.ComboBoxOutputFolderName.Text) Then
                WriteIni(Me.INIFileName, "Output", "DefaultOutputFolder", Me.ComboBoxOutputFolderName.Text)
            End If
            If Not String.IsNullOrWhiteSpace(Me.TextBoxOutputFileNameBase.Text) Then
                WriteIni(Me.INIFileName, "Output", "DefaultOutputFileNameBase", Me.TextBoxOutputFileNameBase.Text)
            End If
            WriteIni(Me.INIFileName, "Output", "OverwriteExistingFile", Me.CheckBoxOverwriteOutputFile.Checked.ToString)
            WriteIni(Me.INIFileName, "Output", "Format", Me.ComboBoxOutputFormat.Text)
            WriteIni(Me.INIFileName, "Output", "Compression", Me.ComboBoxCompression.Text)
            WriteIni(Me.INIFileName, "Output", "ViewAfterAcquire", Me.CheckBoxViewAfterAcquire.Checked.ToString)

            Dim OutputMRU As String = ""
            Dim i As Integer = 0
            For Each kvp As KeyValuePair(Of DateTime, String) In Me.OutputFolderMRU.Reverse
                OutputMRU += kvp.Key.ToString & "," & kvp.Value & ";"
                i += 1
                If i > 9 Then Exit For 'only save the most recent 10 folders
            Next
            WriteIni(Me.INIFileName, "Output", "FolderMRU", OutputMRU)

            WriteIni(Me.INIFileName, "Window", "Top", Me.Top)
            WriteIni(Me.INIFileName, "Window", "Left", Me.Left)
            WriteIni(Me.INIFileName, "Window", "Height", Me.Height)
            WriteIni(Me.INIFileName, "Window", "Width", Me.Width)

        End If
    End Sub

    Private Sub FormStartup_FormClosing(sender As Object, e As FormClosingEventArgs) Handles Me.FormClosing
        RemoveHandler GUIForm.FormClosing, AddressOf Me.GUIForm_FormClosing 'This handler hides the form instead of closing it
        If Me.GUIForm_Shown Then
            'XXX MS bug: The FormClosing and FormClosed events don't fire when the form is hidden.
            '            Make the form as inconspicuous as possible before showing it.
            GUIForm.Controls.Clear()
            GUIForm.ControlBox = False
            GUIForm.FormBorderStyle = Windows.Forms.FormBorderStyle.None
            GUIForm.Size = New Size(0, 0)
            GUIForm.Show()
            '
            GUIForm.Close()
        End If
    End Sub

    Private Sub FormStartup_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        Me.Text = My.Application.Info.ProductName & " " & My.Application.Info.Version.ToString & " Beta" 'Assembly Version

        Me.MinimumSize = Me.Size

        Dim UseRoamingAppData As Boolean = True
        Dim UserSettingsFolder As String = Nothing

        If UseRoamingAppData Then
            UserSettingsFolder = System.Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData) & "\" & Me.ProductName
        Else
            UserSettingsFolder = System.Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData) & "\" & Me.ProductName
        End If
        If Not My.Computer.FileSystem.DirectoryExists(UserSettingsFolder) Then My.Computer.FileSystem.CreateDirectory(UserSettingsFolder)

        Me.UserConfigDirectory = UserSettingsFolder
        Me.LogDirectory = Me.UserConfigDirectory 'XXX

        Try
            Me.SharedConfigDirectory = System.Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) & "\" & Me.ProductName
            If Not My.Computer.FileSystem.DirectoryExists(Me.SharedConfigDirectory) Then My.Computer.FileSystem.CreateDirectory(Me.SharedConfigDirectory)
        Catch ex As Exception
            'Logger.Write(DebugLogger.Level.Error_, True, "Failed to create common configuration folder '" & Me.SharedConfigDirectory & "': " & ex.Message)
        End Try

        Me.INIFileName = Me.UserConfigDirectory & "\" & Me.ProductName & ".ini"

        With Me.ComboBoxOutputFormat.Items
            .Clear()
            For Each f In [Enum].GetNames(GetType(ImageType))
                .Add(f)
            Next
        End With

        Try
            Dim OutputFolder As String = ReadIni(Me.INIFileName, "Output", "DefaultOutputFolder")
            If String.IsNullOrWhiteSpace(OutputFolder) Then
                OutputFolder = My.Computer.FileSystem.SpecialDirectories.Temp & "\" & Me.ProductName
                WriteIni(Me.INIFileName, "Output", "DefaultOutputFolder", "")
            Else
                OutputFolder = String.Join("-", OutputFolder.Split(IO.Path.GetInvalidPathChars)).Trim
            End If
            Try
                If Not My.Computer.FileSystem.DirectoryExists(OutputFolder) Then My.Computer.FileSystem.CreateDirectory(OutputFolder)
            Catch ex As Exception
                MsgBox("Error creating output folder: " & ex.Message)
            End Try

            'FolderMRU format is "Folder1,Timestamp1;Folder2,Timestamp2;Folder3,Timestamp3"...
            Me.OutputFolderMRU.Reverse()
            Dim FolderMRU() As String = Nothing
            Dim s As String = ReadIni(Me.INIFileName, "Output", "FolderMRU")
            If Not String.IsNullOrWhiteSpace(s) Then
                FolderMRU = s.Split(";")
                For Each Folder As String In FolderMRU
                    Dim Values() As String = Folder.Split(",")
                    If Values.Length = 2 Then
                        If Not String.IsNullOrWhiteSpace(Values(1)) Then
                            Dim FolderName As String = Values(1).Trim
                            If Not String.IsNullOrWhiteSpace(Values(0)) Then
                                Dim Timestamp As DateTime
                                If DateTime.TryParse(Values(0), Timestamp) Then
                                    Me.OutputFolderMRU.Add(Timestamp, FolderName)
                                End If
                            End If
                        End If
                    End If
                Next
            End If
            Dim idx As Integer = Me.OutputFolderMRU.IndexOfValue(OutputFolder)
            If idx > -1 Then
                Me.OutputFolderMRU.RemoveAt(idx)
            End If
            Me.OutputFolderMRU.Add(Now, OutputFolder)

            Me.ReloadComboBoxOutputFolderNameList()
            Me.ComboBoxOutputFolderName.SelectedItem = OutputFolder

            Dim OutputFileNameBase As String = ReadIni(Me.INIFileName, "Output", "DefaultOutputFileNameBase")
            If String.IsNullOrWhiteSpace(OutputFileNameBase) Then
                OutputFileNameBase = Me.ProductName & "_%MMddyyyy_HHmmss_fff%"
                WriteIni(Me.INIFileName, "Output", "DefaultOutputFileNameBase", "")
            Else
                OutputFileNameBase = OutputFileNameBase.Trim
            End If
            Me.TextBoxOutputFileNameBase.Text = OutputFileNameBase

            Dim Overwrite As String = ReadIni(Me.INIFileName, "Output", "OverwriteExistingFile")
            Boolean.TryParse(Overwrite, Me.CheckBoxOverwriteOutputFile.Checked)

            s = ReadIni(Me.INIFileName, "Output", "Format")
            Dim ImageFormat As ImageType
            If [Enum].TryParse(Of ImageType)(s, ImageFormat) Then
                Me.ComboBoxOutputFormat.SelectedItem = s
            Else
                Me.ComboBoxOutputFormat.SelectedItem = ImageType.PDF.ToString
            End If

            If ImageFormat = ImageType.TIFF Then
                s = ReadIni(Me.INIFileName, "Output", "Compression")
                Dim ImageCompression As TIFFCompressionMethod
                If [Enum].TryParse(Of TIFFCompressionMethod)(s, ImageCompression) Then
                    Me.ComboBoxCompression.SelectedItem = s
                End If
            End If

            s = ReadIni(Me.INIFileName, "Output", "ViewAfterAcquire")
            Boolean.TryParse(s, Me.CheckBoxViewAfterAcquire.Checked)

            s = ReadIni(Me.INIFileName, "Window", "Top")
            Dim Top As Integer = 0
            Integer.TryParse(s, Top)

            s = ReadIni(Me.INIFileName, "Window", "Left")
            Dim Left As Integer = 0
            Integer.TryParse(s, Left)

            s = ReadIni(Me.INIFileName, "Window", "Height")
            Dim Height As Integer = 0
            Integer.TryParse(s, Height)

            s = ReadIni(Me.INIFileName, "Window", "Width")
            Dim Width As Integer = 0
            Integer.TryParse(s, Width)

            For Each Monitor As Screen In Screen.AllScreens
                If Monitor.WorkingArea.Contains(New Point(Left, Top)) Then
                    Me.Top = Top
                    Me.Left = Left
                    Exit For
                End If
            Next
            Me.Height = Math.Max(Height, Me.Height)
            Me.Width = Math.Max(Width, Me.Width)
        Catch ex As Exception
            MsgBox("Error setting preferences from '" & INIFileName & "': " & ex.Message)
        End Try

        AddHandler GUIForm.FormClosing, AddressOf Me.GUIForm_FormClosing

    End Sub

    Private Sub ReloadComboBoxOutputFolderNameList()
        Me.ComboBoxOutputFolderName.Items.Clear()
        For Each Folder As String In Me.OutputFolderMRU.Values.Reverse
            Me.ComboBoxOutputFolderName.Items.Add(Folder)
        Next
    End Sub

    Private Function ExpandDateTimeVariables(ByVal StringContainingVariables As String) As String
        Dim OutputString As String = Nothing
        Dim s As String = StringContainingVariables
        Dim p As Integer = s.IndexOf("%")
        Dim CurrentTime As DateTime = Now
        Do While s.Length
            If p >= 0 Then
                OutputString += s.Substring(0, p)
                If p + 1 < s.Length Then
                    Dim pp As Integer = s.IndexOf("%", p + 1)
                    If pp >= 0 Then
                        If (pp - p) > 1 Then OutputString += CurrentTime.ToString(s.Substring(p + 1, (pp - p - 1)))
                        s = s.Substring(pp + 1)
                        p = s.IndexOf("%")
                    Else
                        OutputString += s.Substring(p)
                        s = ""
                    End If
                 End If
            Else
                OutputString += s
                s = ""
            End If
        Loop
        Return OutputString
    End Function

    Private Sub OnCompressionMethodChanged(sender As Object, e As EventArgs) Handles ComboBoxCompression.TextChanged
        If Not String.IsNullOrEmpty(Me.ComboBoxCompression.Text) Then
            Me.CurrentImage.TIFF.CompressionMethod = [Enum].Parse(GetType(TIFFCompressionMethod), Me.ComboBoxCompression.Text)
        End If
    End Sub

    Private Sub OnOutputFormatChanged(sender As Object, e As EventArgs) Handles ComboBoxOutputFormat.TextChanged
        Me.ComboBoxCompression.Items.Clear()
        Me.ComboBoxCompression.Enabled = False
        Dim NewValue As ImageType = [Enum].Parse(GetType(ImageType), Me.ComboBoxOutputFormat.Text)
        If Me.ComboBoxOutputFormat.Text IsNot Nothing Then
            Select Case NewValue
                Case ImageType.TIFF
                    For Each c In [Enum].GetNames(GetType(TIFFCompressionMethod))
                        Me.ComboBoxCompression.Items.Add(c)
                    Next
                    Me.ComboBoxCompression.SelectedItem = TIFFCompressionMethod.LZW.ToString
                    ComboBoxCompression.Enabled = True
                Case Else
                    Me.ComboBoxCompression.Items.Add(TIFFCompressionMethod.None.ToString)
                    Me.ComboBoxCompression.SelectedItem = TIFFCompressionMethod.None.ToString
            End Select
        End If
    End Sub

    Private Sub OnBatchStarted() Handles GUIForm.BatchStarted

    End Sub

    Private Sub OnBatchCompleted(ByVal Pages As Integer) Handles GUIForm.BatchCompleted
        Me.GUIForm.Visible = True
        Dim fname As String = Me.CurrentImage.FileNamePage1
        Dim OutputFormat As ImageType = [Enum].Parse(GetType(ImageType), Me.ComboBoxOutputFormat.Text)
        Try
            Select Case OutputFormat
                Case ImageType.PDF
                    Me.ClosePDF()
                Case ImageType.TIFF
                    Me.CloseTIFF(Pages)
                Case Else
                    'MsgBox("Successfully acquired " & Pages.ToString & " pages")
            End Select
            If Pages > 0 Then
                If fname IsNot Nothing Then
                    If Me.CheckBoxViewAfterAcquire.Checked Then
                        Process.Start(fname)
                    End If
                End If
            Else
                MsgBox("No pages were acquired.  The Automatic Document Feeder may be empty or the job may have been cancelled.")
            End If

        Catch ex As Exception
            MsgBox(ex.Message)
        Finally
            Me.CurrentImage.FileName = Nothing
            Me.CurrentImage.FileNamePage1 = Nothing
            GUIForm.Hide()
        End Try
    End Sub

    Private Sub OnImageError(ByVal PageNumber As Integer, ByVal Message As String) Handles GUIForm.ImageError
        Me.GUIForm.Visible = True
        Dim OutputFormat As ImageType = [Enum].Parse(GetType(ImageType), Me.ComboBoxOutputFormat.Text)
        Select Case OutputFormat
            Case ImageType.PDF
                Me.ClosePDF()
            Case ImageType.TIFF
                Me.CloseTIFF(PageNumber)
        End Select

        MsgBox("Error acquiring page " & PageNumber.ToString & ": " & Message)
    End Sub

    Private Sub OnImageAcquired(ByVal PageNumber As Integer, ByVal bmp As Bitmap) Handles GUIForm.ImageAcquired
        Try
            If Not My.Computer.FileSystem.DirectoryExists(Me.ComboBoxOutputFolderName.Text) Then My.Computer.FileSystem.CreateDirectory(Me.ComboBoxOutputFolderName.Text)
            WriteIni(Me.INIFileName, "Output", "DefaultOutputFolder", Me.ComboBoxOutputFolderName.Text)
        Catch ex As Exception
            MsgBox("Error creating directory '" & Me.ComboBoxOutputFolderName.Text & "': " & ex.Message)
        End Try

        Dim OutputFormat As ImageType = [Enum].Parse(GetType(ImageType), Me.ComboBoxOutputFormat.Text)
        Me.CurrentImage.FileName = BuildFileName(OutputFormat, PageNumber)

        Select Case OutputFormat
            'Case "SCREEN"
            '    If bmp IsNot Nothing Then
            '        Dim fs As System.IO.FileStream
            '        Dim ms As System.IO.MemoryStream
            '        Try
            '            Dim frm As New Form
            '            frm.Text = "Page " & PageNumber.ToString
            '            Dim pb As New PictureBox
            '            pb.Name = "PictureBox1"
            '            pb.Parent = frm
            '            pb.Dock = DockStyle.Fill
            '            'pb.SizeMode = PictureBoxSizeMode.AutoSize
            '            'pb.SizeMode = PictureBoxSizeMode.StretchImage
            '            pb.SizeMode = PictureBoxSizeMode.Zoom

            '            frm.Show()
            '            pb.BorderStyle = BorderStyle.None
            '            pb.Visible = True
            '            pb.Show()

            '            'pb.Image = bmp

            '            'XXX we have to save as a PNG here or 16bit color images get jacked up.
            '            Dim fname As String = FileNameBase & "_Page"
            '            fname += PageNumber.ToString
            '            fname += ".png"
            '            bmp.Save(fname, Imaging.ImageFormat.Png)
            '            bmp = Nothing
            '            fs = New System.IO.FileStream(fname, System.IO.FileMode.Open)
            '            ms = New System.IO.MemoryStream
            '            Dim b(fs.Length - 1) As Byte
            '            fs.Read(b, 0, fs.Length)
            '            ms.Write(b, 0, fs.Length)
            '            fs.Close()
            '            pb.Image = Image.FromStream(ms)
            '            ms.Close()

            '            ResizeImageForm(frm)
            '            'AddHandler frm.Resize, AddressOf ImageForm_OnResize

            '            Application.DoEvents() 'Without this the pictureboxes don't get drawn until all pages are finished.
            '        Catch ex As Exception
            '            MsgBox("Error creating image: " & ex.Message)
            '        Finally
            '            fs = Nothing
            '            ms = Nothing
            '            'bmp_data = Nothing
            '            bmp = Nothing
            '        End Try
            '    End If
            Case ImageType.PDF
                Try
                    If bmp IsNot Nothing Then
                        If PageNumber = 1 Then
                            'ShowStatus("Creating PDF document...")
                            If bmp.Tag IsNot Nothing Then
                                If bmp.Tag.GetType = GetType(SANEWinDS.PageSize) Then
                                    Try
                                        Dim SANEWinDS_PageSize As SANEWinDS.PageSize = DirectCast(bmp.Tag, SANEWinDS.PageSize)
                                        Dim iText_Default_dpi As Single = 72.0
                                        Dim iText_PageWidth As Integer = iText_Default_dpi * SANEWinDS_PageSize.Width
                                        Dim iText_PageHeight As Integer = iText_Default_dpi * SANEWinDS_PageSize.Height
                                        CurrentImage.PDF.iTextPageSize = New iText.Kernel.Geom.PageSize(iText_PageWidth, iText_PageHeight)
                                    Catch ex As Exception
                                        'Logger.Error(ex, "Error converting PageSize from SANEWinDS to iText")
                                    End Try
                                End If
                            End If
                            Me.OpenPDF()
                        End If

                        If CurrentImage.FileName IsNot Nothing Then
                            Me.AddPDFPage(bmp)
                        End If

                    End If
                Catch ex As Exception
                    Me.GUIForm.CancelScan()
                    MsgBox(ex.Message)
                    Me.ClosePDF()
                End Try
            Case ImageType.TIFF
                Try
                    If bmp IsNot Nothing Then

                        If Me.CurrentImage.TIFF.ImageCodecInfo Is Nothing Then
                            Dim Codecs() As System.Drawing.Imaging.ImageCodecInfo = System.Drawing.Imaging.ImageCodecInfo.GetImageEncoders()
                            For Each codec As System.Drawing.Imaging.ImageCodecInfo In Codecs
                                If codec.FormatDescription = "TIFF" Then
                                    Me.CurrentImage.TIFF.ImageCodecInfo = codec
                                    Exit For
                                End If
                            Next
                        End If
                        If Me.CurrentImage.TIFF.ImageCodecInfo Is Nothing Then Throw New Exception("Unable to find an instance of the TIFF codec")

                        If Me.CurrentImage.TIFF.SaveEncoder Is Nothing Then Me.CurrentImage.TIFF.SaveEncoder = System.Drawing.Imaging.Encoder.SaveFlag
                        If Me.CurrentImage.TIFF.CompressionEncoder Is Nothing Then Me.CurrentImage.TIFF.CompressionEncoder = System.Drawing.Imaging.Encoder.Compression
                        If Me.CurrentImage.TIFF.EncoderParameters Is Nothing Then Me.CurrentImage.TIFF.EncoderParameters = New System.Drawing.Imaging.EncoderParameters(2)

                        Select Case Me.CurrentImage.TIFF.CompressionMethod
                            Case TIFFCompressionMethod.None
                                Me.CurrentImage.TIFF.EncoderParameters.Param(1) = New System.Drawing.Imaging.EncoderParameter(Me.CurrentImage.TIFF.CompressionEncoder, System.Drawing.Imaging.EncoderValue.CompressionNone)
                            Case TIFFCompressionMethod.CCITT4
                                Me.CurrentImage.TIFF.EncoderParameters.Param(1) = New System.Drawing.Imaging.EncoderParameter(Me.CurrentImage.TIFF.CompressionEncoder, System.Drawing.Imaging.EncoderValue.CompressionCCITT4)
                                bmp = Me.ConvertToBW(bmp) 'CCITT4 is only for B&W images
                            Case TIFFCompressionMethod.RLE
                                Me.CurrentImage.TIFF.EncoderParameters.Param(1) = New System.Drawing.Imaging.EncoderParameter(Me.CurrentImage.TIFF.CompressionEncoder, System.Drawing.Imaging.EncoderValue.CompressionRle)
                            Case TIFFCompressionMethod.LZW
                                Me.CurrentImage.TIFF.EncoderParameters.Param(1) = New System.Drawing.Imaging.EncoderParameter(Me.CurrentImage.TIFF.CompressionEncoder, System.Drawing.Imaging.EncoderValue.CompressionLZW)
                        End Select

                    End If

                    If PageNumber = 1 Then
                        Me.CurrentImage.TIFF.EncoderParameters.Param(0) = New System.Drawing.Imaging.EncoderParameter(Me.CurrentImage.TIFF.SaveEncoder, System.Drawing.Imaging.EncoderValue.MultiFrame)
                        Me.CurrentImage.TIFF.FirstPage = bmp.Clone
                        Me.CurrentImage.TIFF.FirstPage.Save(Me.CurrentImage.FileName, Me.CurrentImage.TIFF.ImageCodecInfo, Me.CurrentImage.TIFF.EncoderParameters)
                    Else
                        Try
                            If Me.CurrentImage.TIFF.FirstPage IsNot Nothing Then
                                Me.CurrentImage.TIFF.EncoderParameters.Param(0) = New System.Drawing.Imaging.EncoderParameter(Me.CurrentImage.TIFF.SaveEncoder, System.Drawing.Imaging.EncoderValue.FrameDimensionPage)
                                Me.CurrentImage.TIFF.FirstPage.SaveAdd(bmp, Me.CurrentImage.TIFF.EncoderParameters)
                            Else
                                Throw New Exception("The first page of a multipage TIFF disappeared while the file was being written")
                            End If
                        Catch ex As Exception
                            Throw
                        End Try
                    End If

                Catch ex As Exception
                    GUIForm.CancelScan()
                    MsgBox(ex.Message)
                End Try
            Case ImageType.PNG
                Try
                    bmp.Save(CurrentImage.FileName, Imaging.ImageFormat.Png)
                Catch ex As Exception
                    MsgBox("Error saving file: " & ex.Message)
                End Try
            Case ImageType.JPEG
                Try
                    bmp.Save(CurrentImage.FileName, Imaging.ImageFormat.Jpeg)
                Catch ex As Exception
                    MsgBox("Error saving file: " & ex.Message)
                End Try
            Case ImageType.GIF
                Try
                    bmp.Save(CurrentImage.FileName, Imaging.ImageFormat.Gif)
                Catch ex As Exception
                    MsgBox("Error saving file: " & ex.Message)
                End Try
            Case ImageType.WMF
                Try
                    bmp.Save(CurrentImage.FileName, Imaging.ImageFormat.Wmf)
                Catch ex As Exception
                    MsgBox("Error saving file: " & ex.Message)
                End Try
            Case ImageType.EMF
                Try
                    bmp.Save(CurrentImage.FileName, Imaging.ImageFormat.Emf)
                Catch ex As Exception
                    MsgBox("Error saving file: " & ex.Message)
                End Try
            Case ImageType.BMP
                Try
                    bmp.Save(CurrentImage.FileName, Imaging.ImageFormat.Bmp)
                Catch ex As Exception
                    MsgBox("Error saving file: " & ex.Message)
                End Try

        End Select

        If bmp IsNot Nothing Then
            bmp.Dispose()
            bmp = Nothing
        End If

        If PageNumber = 1 Then Me.CurrentImage.FileNamePage1 = Me.CurrentImage.FileName

    End Sub

    Private Function BuildFileName(ImageFormat As ImageType, PageNumber As Integer)
        Dim FileNameBase As String = String.Join("-", ExpandDateTimeVariables(Me.TextBoxOutputFileNameBase.Text).Split(IO.Path.GetInvalidFileNameChars))
        FileNameBase = My.Computer.FileSystem.CombinePath(Me.ComboBoxOutputFolderName.Text, FileNameBase)
        Select Case ImageFormat
            Case ImageType.JPEG
                Dim Ext As String = ".JPG"
                If FileNameBase.ToUpper.EndsWith(Ext) Then FileNameBase = FileNameBase.Substring(0, FileNameBase.ToUpper.LastIndexOf(Ext))
                Dim fname As String = FileNameBase & "_Page"
                fname += PageNumber.ToString
                fname += Ext
                Return fname
            Case ImageType.TIFF
                Dim Ext As String = ".TIFF"
                If FileNameBase.ToUpper.EndsWith(Ext) Then FileNameBase = FileNameBase.Substring(0, FileNameBase.ToUpper.LastIndexOf(Ext))
                Ext = ".TIF"
                If FileNameBase.ToUpper.EndsWith(Ext) Then FileNameBase = FileNameBase.Substring(0, FileNameBase.ToUpper.LastIndexOf(Ext))
                Dim fname As String = FileNameBase
                fname += Ext
                Return fname
            Case ImageType.PDF
                Dim Ext As String = ".PDF"
                If FileNameBase.ToUpper.EndsWith(Ext) Then FileNameBase = FileNameBase.Substring(0, FileNameBase.ToUpper.LastIndexOf(Ext))
                Dim fname As String = FileNameBase
                fname += Ext
                Return fname
            Case Else
                Dim Ext As String = "." & ImageFormat.ToString
                If FileNameBase.ToUpper.EndsWith(Ext) Then FileNameBase = FileNameBase.Substring(0, FileNameBase.ToUpper.LastIndexOf(Ext))
                Dim fname As String = FileNameBase & "_Page"
                fname += PageNumber.ToString
                fname += Ext
                Return fname
        End Select
    End Function

    Private Sub CheckOverwrite(FileName As String)
        If Not Me.CheckBoxOverwriteOutputFile.Checked Then
            If My.Computer.FileSystem.FileExists(FileName) Then
                Throw New Exception("Output file '" & FileName & "' already exists")
            End If
        End If
    End Sub

    Private Sub OpenPDF()
        With Me.CurrentImage.PDF
            .FileStream = New System.IO.FileStream(Me.CurrentImage.FileName, IO.FileMode.Create, IO.FileAccess.Write, IO.FileShare.None)
            .iTextWriter = New iText.Kernel.Pdf.PdfWriter(.FileStream)
            .iTextPDFDocument = New iText.Kernel.Pdf.PdfDocument(.iTextWriter)
            .iTextDocument = New iText.Layout.Document(.iTextPDFDocument, .iTextPageSize)
            .iTextDocument.SetMargins(0, 0, 0, 0)
        End With
    End Sub

    Private Sub AddPDFPage(ByRef bmp As Bitmap)
        With Me.CurrentImage.PDF
            If .iTextPDFDocument.GetNumberOfPages > 0 Then .iTextDocument.Add(New iText.Layout.Element.AreaBreak(iText.Layout.Properties.AreaBreakType.NEXT_PAGE))

            Dim ms As New System.IO.MemoryStream
            bmp.Save(ms, System.Drawing.Imaging.ImageFormat.Png)
            Dim img As iText.Layout.Element.Image = New iText.Layout.Element.Image(iText.IO.Image.ImageDataFactory.Create(ms.ToArray))

            img.ScaleToFit(.iTextPageSize.GetWidth, .iTextPageSize.GetHeight)

            .iTextDocument.Add(img)
        End With
    End Sub

    Private Sub ClosePDF()
        With Me.CurrentImage.PDF
            Try
                If .iTextDocument IsNot Nothing Then .iTextDocument.Close()
            Catch
            End Try
            Try
                If .iTextWriter IsNot Nothing Then .iTextWriter.Close()
            Catch
            End Try
            Try
                If .FileStream IsNot Nothing Then .FileStream.Close()
            Catch
            End Try
            .iTextDocument = Nothing
            .iTextWriter = Nothing
            .FileStream = Nothing
        End With
        Me.CurrentImage.FileName = Nothing
    End Sub

    Private Sub CloseTIFF(Pages As Integer)
        With Me.CurrentImage.TIFF
            Try
                If Me.CurrentImage.TIFF.FirstPage IsNot Nothing Then
                    If Pages > 1 Then
                        ' Close the multiple-frame file.
                        Dim myEncoderParameter As System.Drawing.Imaging.EncoderParameter
                        myEncoderParameter = New System.Drawing.Imaging.EncoderParameter(Me.CurrentImage.TIFF.SaveEncoder, Fix(System.Drawing.Imaging.EncoderValue.Flush))
                        Me.CurrentImage.TIFF.EncoderParameters.Param(0) = myEncoderParameter
                        Me.CurrentImage.TIFF.FirstPage.SaveAdd(Me.CurrentImage.TIFF.EncoderParameters)
                    End If
                    Me.CurrentImage.TIFF.FirstPage.Dispose()
                    Me.CurrentImage.TIFF.FirstPage = Nothing
                End If
                Me.CurrentImage.FileName = Nothing
            Catch ex As Exception
                MsgBox(ex.Message)
            End Try
        End With
    End Sub

    Private Function ConvertToBW(ByVal Original As Bitmap) As Bitmap
        Dim Source As Bitmap = Nothing
        'If original bitmap is not already in 32 BPP, ARGB format, then convert
        If Original.PixelFormat <> System.Drawing.Imaging.PixelFormat.Format32bppArgb Then
            Source = New Bitmap(Original.Width, Original.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb)
            Source.SetResolution(Original.HorizontalResolution, Original.VerticalResolution)
            Using g As Graphics = Graphics.FromImage(Source)
                g.DrawImageUnscaled(Original, 0, 0)
            End Using
        Else
            Source = Original
        End If

        'Lock source bitmap in memory
        Dim sourceData As System.Drawing.Imaging.BitmapData = Source.LockBits(New Rectangle(0, 0, Source.Width, Source.Height), System.Drawing.Imaging.ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb)

        'Copy image data to binary array
        Dim imageSize As Integer = sourceData.Stride * sourceData.Height
        Dim sourceBuffer(imageSize - 1) As Byte

        System.Runtime.InteropServices.Marshal.Copy(sourceData.Scan0, sourceBuffer, 0, imageSize)

        'Unlock source bitmap
        Source.UnlockBits(sourceData)

        'Create destination bitmap
        Dim Destination As Bitmap = New Bitmap(Source.Width, Source.Height, System.Drawing.Imaging.PixelFormat.Format1bppIndexed)

        'Lock destination bitmap in memory
        Dim destinationData As System.Drawing.Imaging.BitmapData = Destination.LockBits(New Rectangle(0, 0, Destination.Width, Destination.Height), System.Drawing.Imaging.ImageLockMode.WriteOnly, System.Drawing.Imaging.PixelFormat.Format1bppIndexed)

        'Create destination buffer
        imageSize = destinationData.Stride * destinationData.Height
        Dim destinationBuffer(imageSize - 1) As Byte

        Dim sourceIndex As Integer = 0
        Dim destinationIndex As Integer = 0
        Dim pixelTotal As Integer = 0
        Dim destinationValue As Byte = 0
        Dim pixelValue As Integer = 128
        Dim height As Integer = Source.Height
        Dim width As Integer = Source.Width
        Dim threshold As Integer = 500


        'Iterate lines
        For y As Integer = 0 To height - 1

            sourceIndex = y * sourceData.Stride
            destinationIndex = y * destinationData.Stride
            destinationValue = 0
            pixelValue = 128

            'Iterate pixels
            For x As Integer = 0 To width - 1

                'Compute pixel brightness (i.e. total of Red, Green, and Blue values)
                pixelTotal = CInt(sourceBuffer(sourceIndex + 1)) + CInt(sourceBuffer(sourceIndex + 2)) + CInt(sourceBuffer(sourceIndex + 3))
                If (pixelTotal > threshold) Then
                    destinationValue += CByte(pixelValue)
                End If
                If pixelValue = 1 Then
                    destinationBuffer(destinationIndex) = destinationValue
                    destinationIndex += 1
                    destinationValue = 0
                    pixelValue = 128
                Else
                    pixelValue >>= 1
                End If
                sourceIndex += 4
            Next
            If pixelValue <> 128 Then
                destinationBuffer(destinationIndex) = destinationValue
            End If
        Next

        'Copy binary image data to destination bitmap
        System.Runtime.InteropServices.Marshal.Copy(destinationBuffer, 0, destinationData.Scan0, imageSize)

        'Unlock destination bitmap
        Destination.UnlockBits(destinationData)

        Return Destination
    End Function

    'Private Sub ImageForm_OnResize(sender As Object, e As EventArgs)
    '    ResizeImageForm(sender)
    'End Sub

    'Private Sub ResizeImageForm(frm As Form)

    '    Dim pb As PictureBox = frm.Controls("PictureBox1")

    '    Dim BordersWidth As Integer = frm.Width - frm.ClientSize.Width
    '    Dim BordersHeight As Integer = frm.Height - frm.ClientSize.Height
    '    Dim hzoom As Integer = Math.Abs(pb.PreferredSize.Height - pb.Height)
    '    Dim wzoom As Integer = Math.Abs(pb.PreferredSize.Width - pb.Width)
    '    If hzoom > wzoom Then
    '        frm.Height = ((pb.PreferredSize.Height / pb.PreferredSize.Width) * pb.Width) + BordersHeight
    '    Else
    '        frm.Width = ((pb.PreferredSize.Width / pb.PreferredSize.Height) * pb.Height) + BordersWidth
    '    End If

    'End Sub

    Private Sub ButtonAcquire_Click(sender As Object, e As EventArgs) Handles ButtonAcquire.Click
        Dim Folder As String = Me.ComboBoxOutputFolderName.Text
        If Not String.IsNullOrWhiteSpace(Folder) Then
            Dim idx As Integer = Me.OutputFolderMRU.IndexOfValue(Folder.Trim)
            If idx > -1 Then
                Me.OutputFolderMRU.RemoveAt(idx)
            End If
            Me.OutputFolderMRU.Add(Now, Folder.Trim)
        End If
        Me.ReloadComboBoxOutputFolderNameList()
        Me.ComboBoxOutputFolderName.SelectedItem = Folder

        Dim OutputFormat As ImageType = [Enum].Parse(GetType(ImageType), Me.ComboBoxOutputFormat.Text)
        Try
            Dim fname As String = BuildFileName(OutputFormat, 1)
            CheckOverwrite(fname)

            Try
                Me.GUIForm_Shown = True
                Me.Cursor = Cursors.WaitCursor
                GUIForm.ShowScanProgress = True
                GUIForm.ShowDialog()
             Catch ex As Exception
                MsgBox(ex.Message & vbCrLf & ex.StackTrace)
                Me.ClosePDF()
                Me.CloseTIFF(0)
            Finally
                Me.Cursor = Cursors.Default
            End Try

        Catch ex As Exception
            MsgBox(ex.Message, MsgBoxStyle.Exclamation, "Error")
        End Try
    End Sub

    Private Sub GUIForm_FormClosing(sender As Object, e As FormClosingEventArgs) 'Handles GUIForm.FormClosing
        If e.CloseReason = CloseReason.UserClosing Then
            e.Cancel = True
            GUIForm.Hide()
        End If
    End Sub

    Private Sub ButtonBrowseOutputFolder_Click(sender As Object, e As EventArgs) Handles ButtonBrowseOutputFolder.Click
        Dim fb As New FolderBrowserDialog
        fb.ShowNewFolderButton = True
        fb.Description = "Select a folder in which to place the scanned items."
        If fb.ShowDialog = Windows.Forms.DialogResult.OK Then
            Me.ComboBoxOutputFolderName.Text = fb.SelectedPath
        End If
    End Sub

    Private Sub ButtonOutputFileNameBaseFormatHelp_Click(sender As System.Object, e As System.EventArgs) Handles ButtonOutputFileNameBaseFormatHelp.Click
        Dim s As String = "The 'Output File Name Base' field can contain literal characters and date/time formatting variables surrounded by percent (%) characters."
        s += "  Variables can be any of the Standard or Custom variables described in the following Microsoft web pages:" & vbCr
        s += "http://msdn.microsoft.com/en-us/library/az4se3k1(v=vs.100).aspx" & vbCr
        s += "http://msdn.microsoft.com/en-us/library/8kb3ddd4(v=vs.100).aspx" & vbCr
        s += "Any character not allowed in a filename will be replaced by a dash (-) character and the appropriate file name extension will be added." & vbCr
        s += vbCr & "Example: 'MyFileName_%g%' with an output format of 'TIFF' will result in a file name similar to 'MyFileName_10-31-2014 11-46 AM.TIF'."
        s += "  The format may vary depending on your locale." & vbCr
        s += vbCr & "For historical compatibility the default 'Output File Name Base' is '" & Application.ProductName & "_%MMddyyyy_HHmmss_fff%'." & vbCr
        s += vbCr & "Would you like to open the format help pages now?"
        If MsgBox(s, MsgBoxStyle.YesNo + MsgBoxStyle.Information, "File Name Format Help") = MsgBoxResult.Yes Then
            Process.Start("http://msdn.microsoft.com/en-us/library/az4se3k1(v=vs.100).aspx")
            Process.Start("http://msdn.microsoft.com/en-us/library/8kb3ddd4(v=vs.100).aspx")
        End If
    End Sub

End Class