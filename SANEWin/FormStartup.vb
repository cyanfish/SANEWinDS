
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
    'Private Enum OutputFormat As Integer
    '    Screen = 0
    '    PDF = 1
    '    JPEG = 2
    'End Enum
    Private Structure PDFInfo
        Dim FileName As String
        Dim FileStream As System.IO.FileStream
        Dim iTextDocument As iTextSharp.text.Document
        Dim iTextWriter As iTextSharp.text.pdf.PdfWriter
    End Structure

    'Private OutputTo As OutputFormat = OutputFormat.PDF
    'Private OutputTo As String = Nothing

    Private CurrentPDF As PDFInfo
    Private frmStatus As Form

    Private Sub FormStartup_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Me.Visible = False
        Try
            With GUIForm
                '.AddOutputDestinationString("JPEG")
                .AddOutputDestinationString("PDF")
                .AddOutputDestinationString("Screen")
                .SetOutputDestinationString("PDF")
            End With
            Dim DialogResult As DialogResult = GUIForm.ShowDialog
        Catch ex As Exception
            MsgBox(ex.Message)
            Me.ClosePDF()
        End Try
        Me.Close()
    End Sub

    Private Sub ShowStatus(Text As String)
        If frmStatus Is Nothing OrElse frmStatus.IsDisposed Then frmStatus = New Form
        frmStatus.Controls.Clear()
        frmStatus.Cursor = Cursors.WaitCursor
        Dim lbl As New Label
        lbl.Name = "Status"
        lbl.Text = Text
        lbl.Dock = DockStyle.Fill
        lbl.Font = New Font("Arial", 14)
        lbl.TextAlign = ContentAlignment.MiddleCenter
        lbl.Parent = frmStatus
        frmStatus.Height = lbl.Font.Height * 5
        frmStatus.Width = frmStatus.Height * 3
        frmStatus.StartPosition = FormStartPosition.CenterScreen
        frmStatus.Show()
        frmStatus.BringToFront()
        Application.DoEvents()

    End Sub

    Private Sub OnBatchStarted() Handles GUIForm.BatchStarted
        'GUIForm.Visible = False

        ShowStatus("Acquiring page 1...")

    End Sub

    Private Sub OnBatchCompleted(ByVal Pages As Integer) Handles GUIForm.BatchCompleted
        Me.GUIForm.Visible = True
        Try
            'If Me.OutputTo = OutputFormat.PDF Then
            If Me.GUIForm.GetOutputDestinationString.ToUpper = "PDF" Then
                Dim fname As String = Me.CurrentPDF.FileName
                Me.ClosePDF()
                frmStatus.Close()
                If Pages > 0 Then
                    If fname IsNot Nothing Then
                        'ShowStatus("Opening PDF document in default viewer...")
                        Process.Start(fname)
                    End If
                Else
                    MsgBox("No pages were acquired.  The Automatic Document Feeder may be empty.")
                End If
            Else
                'MsgBox("Successfully acquired " & Pages.ToString & " pages")
            End If
        Catch ex As Exception
            MsgBox(ex.Message)
        Finally
            If frmStatus IsNot Nothing AndAlso (Not frmStatus.IsDisposed) Then frmStatus.Close()
        End Try
    End Sub

    Private Sub OnImageError(ByVal PageNumber As Integer, ByVal Message As String) Handles GUIForm.ImageError
        Me.GUIForm.Visible = True
        'If Me.OutputTo = OutputFormat.PDF Then
        If GUIForm.GetOutputDestinationString.ToUpper = "PDF" Then
            Me.ClosePDF()
        End If

        frmStatus.Close()

        MsgBox("Error acquiring page " & PageNumber.ToString & ": " & Message)
    End Sub

    Private Sub OnImageAcquired(ByVal PageNumber As Integer, ByVal bmp As Bitmap) Handles GUIForm.ImageAcquired
        Select Case GUIForm.GetOutputDestinationString.ToUpper
            'Case OutputFormat.Screen
            Case "SCREEN"
                Dim FileName As String = My.Computer.FileSystem.SpecialDirectories.Temp & "\SANEWin" 'XXX
                If bmp IsNot Nothing Then
                    Dim fs As System.IO.FileStream
                    Dim ms As System.IO.MemoryStream
                    Try
                        Dim frm As New Form
                        frm.Text = "Page " & PageNumber.ToString
                        Dim pb As New PictureBox
                        pb.Name = "PictureBox1"
                        pb.Parent = frm
                        pb.Dock = DockStyle.Fill
                        'pb.SizeMode = PictureBoxSizeMode.AutoSize
                        'pb.SizeMode = PictureBoxSizeMode.StretchImage
                        pb.SizeMode = PictureBoxSizeMode.Zoom

                        frm.Show()
                        pb.BorderStyle = BorderStyle.None
                        pb.Visible = True
                        pb.Show()

                        'pb.Image = bmp

                        'XXX we have to save as a PNG here or 16bit color images get jacked up.
                        Dim fname As String = FileName
                        fname += PageNumber.ToString
                        fname += ".png"
                        bmp.Save(fname, Imaging.ImageFormat.Png)
                        bmp = Nothing
                        fs = New System.IO.FileStream(fname, System.IO.FileMode.Open)
                        ms = New System.IO.MemoryStream
                        Dim b(fs.Length - 1) As Byte
                        fs.Read(b, 0, fs.Length)
                        ms.Write(b, 0, fs.Length)
                        fs.Close()
                        pb.Image = Image.FromStream(ms)
                        ms.Close()

                        ResizeImageForm(frm)
                        'AddHandler frm.Resize, AddressOf ImageForm_OnResize

                        Application.DoEvents() 'Without this the pictureboxes don't get drawn until all pages are finished.
                    Catch ex As Exception
                        MsgBox("Error creating image: " & ex.Message)
                    Finally
                        fs = Nothing
                        ms = Nothing
                        'bmp_data = Nothing
                        bmp = Nothing
                    End Try
                End If
                'Case OutputFormat.PDF
            Case "PDF"
                Try
                    If bmp IsNot Nothing Then
                        If PageNumber = 1 Then

                            ShowStatus("Creating PDF document...")

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

                            Me.OpenPDF(PageSize)
                        End If

                        If CurrentPDF.FileName IsNot Nothing Then
                            ShowStatus("Adding page " & PageNumber.ToString & " to PDF document...")
                            Me.AddPDFPage(bmp)
                        End If

                    End If
                Catch ex As Exception
                    MsgBox(ex.Message)
                    Me.ClosePDF()
                Finally
                    bmp.Dispose()
                    bmp = Nothing
                End Try
        End Select
        ShowStatus("Acquiring page " & (PageNumber + 1).ToString & "...")

    End Sub
    Private Sub OpenPDF(ByVal PageSize As iTextSharp.text.Rectangle)
        With Me.CurrentPDF
            .FileName = My.Computer.FileSystem.SpecialDirectories.Temp & "\SANEWin.PDF" 'XXX
            .FileStream = New System.IO.FileStream(.FileName, IO.FileMode.Create, IO.FileAccess.Write, IO.FileShare.None)
            .iTextDocument = New iTextSharp.text.Document(PageSize, 0, 0, 0, 0)
            .iTextWriter = iTextSharp.text.pdf.PdfWriter.GetInstance(.iTextDocument, .FileStream)
            .iTextDocument.Open()
        End With
    End Sub
    Private Sub AddPDFPage(ByRef bmp As Bitmap)
        With Me.CurrentPDF
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
        With Me.CurrentPDF
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
            .FileName = Nothing
        End With
    End Sub

    Private Sub ImageForm_OnResize(sender As Object, e As EventArgs)
        ResizeImageForm(sender)
    End Sub

    Private Sub ResizeImageForm(frm As Form)

        Dim pb As PictureBox = frm.Controls("PictureBox1")

        Dim BordersWidth As Integer = frm.Width - frm.ClientSize.Width
        Dim BordersHeight As Integer = frm.Height - frm.ClientSize.Height
        Dim hzoom As Integer = Math.Abs(pb.PreferredSize.Height - pb.Height)
        Dim wzoom As Integer = Math.Abs(pb.PreferredSize.Width - pb.Width)
        If hzoom > wzoom Then
            frm.Height = ((pb.PreferredSize.Height / pb.PreferredSize.Width) * pb.Width) + BordersHeight
        Else
            frm.Width = ((pb.PreferredSize.Width / pb.PreferredSize.Height) * pb.Height) + BordersWidth
        End If

    End Sub
End Class