
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
        Dim iTextDocument As iTextSharp.text.Document
        Dim iTextWriter As iTextSharp.text.pdf.PdfWriter
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
    Private INI As New IniFile.IniFile
    Private UserConfigDirectory As String
    Private SharedConfigDirectory As String
    Private LogDirectory As String
    Private OutputDirectory As String
    'Private CurrentPDF As PDFInfo
    'Private CurrentTIFF As TIFFInfo
    Private CurrentImage As ImageInfo

    Private frmStatus As Form

    Private GUIForm_Shown As Boolean = False

    Private Sub FormStartup_FormClosed(sender As Object, e As FormClosedEventArgs) Handles Me.FormClosed
        If Me.INI IsNot Nothing Then
            If Me.INIFileName IsNot Nothing AndAlso Me.INIFileName.Trim.Length > 0 Then
                Me.INI.Save(Me.INIFileName)
            End If
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

        'Me.Visible = False
        'Me.Text = Application.ProductName & " " & Application.ProductVersion 'Assembly File Version
        Me.Text = My.Application.Info.ProductName & " " & My.Application.Info.Version.ToString & " Alpha" 'Assembly Version

        Me.MinimumSize = Me.Size

        Dim UseRoamingAppData As Boolean = True
        Dim UserSettingsFolder As String

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

        Try
            If Not My.Computer.FileSystem.FileExists(INIFileName) Then Me.INI.Save(INIFileName) 'create new file
            Me.INI.Load(INIFileName)
            If Me.INI.GetSection("Output") Is Nothing Then Me.INI.AddSection("Output")
            Dim OutputFolder As String = INI.GetKeyValue("Output", "DefaultOutputFolder")
            If String.IsNullOrEmpty(OutputFolder) Then
                Me.OutputDirectory = My.Computer.FileSystem.SpecialDirectories.Temp & "\" & Me.ProductName
                Me.INI.SetKeyValue("Output", "DefaultOutputFolder", "")
            Else
                Me.OutputDirectory = OutputFolder.Trim
            End If
        Catch ex As Exception
            MsgBox("Error loading '" & INIFileName & "': " & ex.Message)
        End Try

        Try
            If Not My.Computer.FileSystem.DirectoryExists(Me.OutputDirectory) Then My.Computer.FileSystem.CreateDirectory(Me.OutputDirectory)
        Catch ex As Exception
            MsgBox("Error creating output folder: " & ex.Message)
        End Try

        With Me.ComboBoxOutputFormat.Items
            .Clear()
            For Each f In [Enum].GetNames(GetType(ImageType))
                .Add(f)
            Next
        End With
        'MsgBox("[" & ImageType.PDF.ToString & "]")
        Dim s As String = ImageType.PDF.ToString
        Me.ComboBoxOutputFormat.SelectedItem = ImageType.PDF.ToString

        Me.ComboBoxOutputFolderName.Items.Add(Me.OutputDirectory)
        Me.ComboBoxOutputFolderName.SelectedItem = Me.OutputDirectory

        AddHandler GUIForm.FormClosing, AddressOf Me.GUIForm_FormClosing

    End Sub

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

    Private Sub ShowStatus(ByVal Text As String)
        'Show a status form with progress bar during image acquisition.
        Try
            If frmStatus Is Nothing OrElse frmStatus.IsDisposed Then
                frmStatus = New Form
                frmStatus.Height = 180
                frmStatus.Width = 500

                Dim lbl As New Label
                lbl.Name = "Status"
                lbl.Text = Text
                lbl.Top = 10
                lbl.Width = frmStatus.ClientSize.Width
                lbl.Height = 40
                lbl.Anchor = AnchorStyles.Top + AnchorStyles.Left + AnchorStyles.Right
                lbl.Font = New Font("Arial", 14)
                lbl.TextAlign = ContentAlignment.MiddleCenter
                lbl.Parent = frmStatus

                Dim pbar As New ProgressBar
                pbar.Name = "Progress"
                pbar.Top = lbl.Bottom + 10
                pbar.Width = frmStatus.ClientSize.Width - 40
                pbar.Left = (frmStatus.ClientSize.Width \ 2) - (pbar.Width \ 2)
                pbar.Parent = frmStatus

                Dim btn As New Button
                btn.Name = "Cancel"
                btn.Text = "Cancel"
                btn.Parent = frmStatus
                btn.Top = pbar.Bottom + 20
                btn.Width = 60
                btn.Left = (frmStatus.ClientSize.Width \ 2) - (btn.Width \ 2)
                btn.Anchor = AnchorStyles.Top
                AddHandler btn.Click, AddressOf Me.CancelScan_Click

                frmStatus.Text = Me.Text
                frmStatus.Icon = Me.Icon
                frmStatus.FormBorderStyle = Windows.Forms.FormBorderStyle.FixedSingle
                frmStatus.StartPosition = FormStartPosition.CenterScreen
                frmStatus.Show()
                frmStatus.BringToFront()
            Else
                frmStatus.Controls("Status").Text = Text
            End If
        Catch ex As Exception
            Debug.Print(ex.Message)
        End Try
    End Sub

    Private Sub CancelScan_Click(sender As Object, e As System.EventArgs)
        If GUIForm IsNot Nothing Then
            GUIForm.CancelScan()
        End If
    End Sub

    Private Sub OnBatchStarted() Handles GUIForm.BatchStarted
        'GUIForm.Visible = False

        ShowStatus("Acquiring page 1...")

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
            frmStatus.Close()
            If Pages > 0 Then
                If fname IsNot Nothing Then
                    'ShowStatus("Opening PDF document in default viewer...")
                    Process.Start(fname)
                End If
            Else
                MsgBox("No pages were acquired.  The Automatic Document Feeder may be empty or the job may have been cancelled.")
            End If

        Catch ex As Exception
            MsgBox(ex.Message)
        Finally
            If frmStatus IsNot Nothing AndAlso (Not frmStatus.IsDisposed) Then frmStatus.Close()
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
        frmStatus.Close()

        MsgBox("Error acquiring page " & PageNumber.ToString & ": " & Message)
    End Sub

    Private Sub OnImageAcquired(ByVal PageNumber As Integer, ByVal bmp As Bitmap) Handles GUIForm.ImageAcquired
        Try
            If Not My.Computer.FileSystem.DirectoryExists(Me.ComboBoxOutputFolderName.Text) Then My.Computer.FileSystem.CreateDirectory(Me.ComboBoxOutputFolderName.Text)
            Me.INI.SetKeyValue("Output", "DefaultOutputFolder", Me.ComboBoxOutputFolderName.Text)
        Catch ex As Exception
            MsgBox("Error creating directory '" & Me.ComboBoxOutputFolderName.Text & "': " & ex.Message)
        End Try

        Dim FileNameBase As String = My.Computer.FileSystem.CombinePath(Me.ComboBoxOutputFolderName.Text, Me.ProductName & Now.ToString("_MMddyyyy_HHmmss_fff"))
        Dim OutputFormat As ImageType = [Enum].Parse(GetType(ImageType), Me.ComboBoxOutputFormat.Text)
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

                            'guess page size:
                            '8.5/11 = .773
                            '11/8.5 = 1.294
                            '8.5/14 = .607
                            '14/8.5 = 1.647
                            Dim AspectRatio As Single = bmp.Width / bmp.Height
                            Dim PageSize As iTextSharp.text.Rectangle
                            Select Case AspectRatio
                                Case Is < 0.69
                                    PageSize = iTextSharp.text.PageSize.LEGAL
                                Case Is < 1.034
                                    PageSize = iTextSharp.text.PageSize.LETTER
                                Case Is < 1.471
                                    PageSize = iTextSharp.text.PageSize.LETTER_LANDSCAPE
                                Case Else
                                    PageSize = iTextSharp.text.PageSize.LEGAL_LANDSCAPE
                            End Select

                            Me.CurrentImage.FileName = FileNameBase & ".pdf"
                            Me.OpenPDF(PageSize)
                        End If

                        If CurrentImage.FileName IsNot Nothing Then
                            'ShowStatus("Adding page " & PageNumber.ToString & " to PDF document...")
                            Me.AddPDFPage(bmp)
                        End If

                    End If
                Catch ex As Exception
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
                        'ShowStatus("Creating TIFF image...")

                        Me.CurrentImage.FileName = FileNameBase & ".tif"

                        Me.CurrentImage.TIFF.EncoderParameters.Param(0) = New System.Drawing.Imaging.EncoderParameter(Me.CurrentImage.TIFF.SaveEncoder, System.Drawing.Imaging.EncoderValue.MultiFrame)
                        Me.CurrentImage.TIFF.FirstPage = bmp
                        Me.CurrentImage.TIFF.FirstPage.Save(Me.CurrentImage.FileName, Me.CurrentImage.TIFF.ImageCodecInfo, Me.CurrentImage.TIFF.EncoderParameters)
                    Else
                        'ShowStatus("Adding page " & PageNumber.ToString & " to TIFF image...")
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
                    MsgBox(ex.Message)
                End Try
            Case ImageType.PNG
                Try
                    Dim fname As String = FileNameBase & "_Page"
                    fname += PageNumber.ToString
                    fname += ".png"
                    Me.CurrentImage.FileName = fname
                    bmp.Save(fname, Imaging.ImageFormat.Png)
                Catch ex As Exception
                    MsgBox("Error saving file: " & ex.Message)
                 End Try
            Case ImageType.JPEG
                Try
                    Dim fname As String = FileNameBase & "_Page"
                    fname += PageNumber.ToString
                    fname += ".jpg"
                    Me.CurrentImage.FileName = fname
                    bmp.Save(fname, Imaging.ImageFormat.Jpeg)
                Catch ex As Exception
                    MsgBox("Error saving file: " & ex.Message)
                 End Try
            Case ImageType.GIF
                Try
                    Dim fname As String = FileNameBase & "_Page"
                    fname += PageNumber.ToString
                    fname += ".gif"
                    Me.CurrentImage.FileName = fname
                    bmp.Save(fname, Imaging.ImageFormat.Gif)
                Catch ex As Exception
                    MsgBox("Error saving file: " & ex.Message)
                End Try
            Case ImageType.WMF
                Try
                    Dim fname As String = FileNameBase & "_Page"
                    fname += PageNumber.ToString
                    fname += ".wmf"
                    Me.CurrentImage.FileName = fname
                    bmp.Save(fname, Imaging.ImageFormat.Wmf)
                Catch ex As Exception
                    MsgBox("Error saving file: " & ex.Message)
                End Try
            Case ImageType.EMF
                Try
                    Dim fname As String = FileNameBase & "_Page"
                    fname += PageNumber.ToString
                    fname += ".emf"
                    Me.CurrentImage.FileName = fname
                    bmp.Save(fname, Imaging.ImageFormat.Emf)
                Catch ex As Exception
                    MsgBox("Error saving file: " & ex.Message)
                End Try
            Case ImageType.BMP
                Try
                    Dim fname As String = FileNameBase & "_Page"
                    fname += PageNumber.ToString
                    fname += ".bmp"
                    Me.CurrentImage.FileName = fname
                    bmp.Save(fname, Imaging.ImageFormat.Bmp)
                Catch ex As Exception
                    MsgBox("Error saving file: " & ex.Message)
                End Try

        End Select

        If bmp IsNot Nothing Then
            bmp.Dispose()
            bmp = Nothing
        End If

        If PageNumber = 1 Then Me.CurrentImage.FileNamePage1 = Me.CurrentImage.FileName
        ShowStatus("Acquiring page " & (PageNumber + 1).ToString & "...")

    End Sub

    Private Sub OpenPDF(ByVal PageSize As iTextSharp.text.Rectangle)
        With Me.CurrentImage.PDF
            '.FileName = Me.OutputDirectory & "\" & Me.ProductName & Now.ToString("_MMddyyyy_HHmmss_fff") & ".pdf"
            .FileStream = New System.IO.FileStream(Me.CurrentImage.FileName, IO.FileMode.Create, IO.FileAccess.Write, IO.FileShare.None)
            .iTextDocument = New iTextSharp.text.Document(PageSize, 0, 0, 0, 0)
            .iTextWriter = iTextSharp.text.pdf.PdfWriter.GetInstance(.iTextDocument, .FileStream)
            .iTextDocument.Open()
        End With
    End Sub

    Private Sub AddPDFPage(ByRef bmp As Bitmap)
        With Me.CurrentImage.PDF
            .iTextDocument.NewPage()

            'ShowStatus("Creating iText image...")
            'Dim img As iTextSharp.text.Image = iTextSharp.text.Image.GetInstance(bmp, iTextSharp.text.BaseColor.WHITE) 'Unbearably slow!
            Dim ms As New System.IO.MemoryStream
            bmp.Save(ms, System.Drawing.Imaging.ImageFormat.Png)
            Dim img As iTextSharp.text.Image = iTextSharp.text.Image.GetInstance(ms.ToArray)

            'ShowStatus("Scaling image...")
            img.ScaleToFit(.iTextDocument.PageSize.Width, .iTextDocument.PageSize.Height)

            'ShowStatus("Adding image to document...")
            .iTextDocument.Add(img)
        End With
    End Sub

    Private Sub ClosePDF()
        With Me.CurrentImage.PDF
            Try
                If .iTextDocument IsNot Nothing Then If .iTextDocument.IsOpen Then .iTextDocument.Close()
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
        Try
            Me.GUIForm_Shown = True
            Me.Cursor = Cursors.WaitCursor
            Dim DialogResult As DialogResult = GUIForm.ShowDialog
        Catch ex As Exception
            MsgBox(ex.Message & vbCrLf & ex.StackTrace)
            Me.ClosePDF()
            Me.CloseTIFF(0)
        Finally
            Me.Cursor = Cursors.Default
        End Try
    End Sub

    Private Sub GUIForm_FormClosing(sender As Object, e As FormClosingEventArgs) 'Handles GUIForm.FormClosing
        If e.CloseReason = CloseReason.UserClosing Then
            e.Cancel = True
            GUIForm.Hide()
        End If
    End Sub

    Private Sub GUIForm_ImageProgress(PercentComplete As Integer) Handles GUIForm.ImageProgress
        If Me.InvokeRequired Then
            Dim d As New dSetImageProgress(AddressOf SetImageProgress)
            Me.Invoke(d, New Object() {PercentComplete})
        Else
            Me.SetImageProgress(PercentComplete)
        End If
    End Sub

    Private Delegate Sub dSetImageProgress(ByVal Progress As Integer)
    Private Sub SetImageProgress(ByVal Progress As Integer)
        Try
            Dim pbar As ProgressBar = frmStatus.Controls("Progress")
            If Progress < 0 Then
                pbar.Style = ProgressBarStyle.Marquee
                pbar.MarqueeAnimationSpeed = 30
             Else
                pbar.Style = ProgressBarStyle.Continuous
                pbar.Value = Progress
            End If
        Catch ex As Exception
        End Try
    End Sub
End Class