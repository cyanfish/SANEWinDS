Imports System.Collections.Generic
Imports System.ComponentModel
Imports System.Drawing
Imports System.Drawing.Drawing2D
Imports System.Windows.Forms

'Namespace YLScsDrawing.Controls
''' <summary>
''' For image curve control
''' </summary>

Public Class ImageLevelEventArgs
    Inherits EventArgs
    Private m_levelValue As Integer()

    Public Sub New(LevelValue As Integer())
        m_levelValue = LevelValue
    End Sub

    Public ReadOnly Property LevelValue() As Integer()
        Get
            Return m_levelValue
        End Get
    End Property

End Class

'Declare a delegate 
Public Delegate Sub ImageLevelChangedEventHandler(sender As Object, e As ImageLevelEventArgs)

Public Class ImageCurve
    Inherits UserControl
    Public Sub New()
        ' Set the value of the double-buffering style bits to true.
        Me.SetStyle(ControlStyles.AllPaintingInWmPaint Or ControlStyles.UserPaint Or ControlStyles.ResizeRedraw Or ControlStyles.UserPaint Or ControlStyles.DoubleBuffer, True)
    End Sub
    Public Sub New(MaxX As Integer, MaxY As Integer)
        Me.New()
        Me.Init(MaxX, MaxY)
    End Sub
    Public Sub Init(MaxX As Integer, MaxY As Integer)
        _MaxX = MaxX - 1
        _MaxY = MaxY - 1
        ReDim level(_MaxX)
    End Sub

    Private _MaxX, _MaxY As Integer
    Public ReadOnly Property MaxX As Integer
        Get
            Return _MaxX
        End Get
    End Property
    Public ReadOnly Property MaxY As Integer
        Get
            Return _MaxY
        End Get
    End Property
    Public keyPt As New List(Of Point)()
    Private level() As Integer

    Public Event ImageLevelChanged As ImageLevelChangedEventHandler

    Protected Overridable Sub OnLevelChanged(e As ImageLevelEventArgs)
        ' Make sure there are methods to execute.
        RaiseEvent ImageLevelChanged(Me, e)
        ' Raise the event.
    End Sub

    Public ReadOnly Property LevelValue() As Integer()
        Get
            getImageLevel()
            Return level
        End Get
    End Property

    Private Sub getImageLevel()
        Dim pts As Point() = New Point(keyPt.Count - 1) {}
        For i As Integer = 0 To pts.Length - 1
            pts(i).X = keyPt(i).X * MaxX \ Me.Width
            pts(i).Y = MaxY - keyPt(i).Y * MaxY \ Me.Height
        Next

        For i As Integer = 0 To pts(0).X - 1
            level(i) = pts(0).Y
        Next
        For i As Integer = pts(pts.Length - 1).X To MaxX
            level(i) = pts(pts.Length - 1).Y
        Next

        Dim sp As New YLScsDrawing.Geometry.Spline()
        sp.DataPoint = pts
        sp.Precision = 1.0
        Dim spt As Point() = sp.SplinePoint
        For i As Integer = 0 To spt.Length - 1
            Dim n As Integer = spt(i).Y
            If n < 0 Then
                n = 0
            End If
            If n > MaxY Then
                n = MaxY
            End If
            level(pts(0).X + i) = n
        Next
    End Sub

    Private ww As Integer, hh As Integer
    Protected Overrides Sub OnLoad(e As EventArgs)
        MyBase.OnLoad(e)
        ww = Me.Width
        hh = Me.Height
        keyPt.Clear()
        keyPt.Add(New Point(0, hh))
        keyPt.Add(New Point(ww, 0))

        Invalidate()
    End Sub

    Protected Overrides Sub OnResize(e As EventArgs)
        MyBase.OnResize(e)
        For i As Integer = 0 To keyPt.Count - 1
            keyPt(i) = New Point(keyPt(i).X * Me.Width \ ww, keyPt(i).Y * Me.Height \ hh)
        Next
        ww = Me.Width
        hh = Me.Height

        Invalidate()
    End Sub

    Private moveflag As Integer
    Private drag As Boolean = False
    Protected Overrides Sub OnMouseDown(e As MouseEventArgs)
        MyBase.OnMouseDown(e)
        If e.Button = Windows.Forms.MouseButtons.Left Then
            For i As Integer = 1 To keyPt.Count - 1
                If e.X > keyPt(i - 1).X + 20 AndAlso e.Y > 0 AndAlso e.X < keyPt(i).X - 20 AndAlso e.Y < Me.Height Then
                    keyPt.Insert(i, e.Location)
                    drag = True
                    moveflag = i
                    Me.Cursor = Cursors.Hand
                    Invalidate()
                End If
            Next
        End If

        Dim r As New Rectangle(e.X - 20, e.Y - 20, 40, 40)
        For i As Integer = 0 To keyPt.Count - 1
            If r.Contains(keyPt(i)) Then
                If e.Button = Windows.Forms.MouseButtons.Left Then
                    drag = True
                    moveflag = i
                Else
                    If i > 0 And i < keyPt.Count - 1 Then
                        keyPt.RemoveAt(i)
                        Invalidate()
                    End If
                End If
                Exit For
            End If
        Next
    End Sub

    Protected Overrides Sub OnMouseMove(e As MouseEventArgs)
        MyBase.OnMouseMove(e)

        'mouse cursor
        Dim handCursor As Boolean = False
        For i As Integer = 0 To keyPt.Count - 1
            Dim r As New Rectangle(keyPt(i).X - 2, keyPt(i).Y - 2, 4, 4)
            If r.Contains(e.Location) Then
                handCursor = True
            End If
        Next
        If handCursor Then
            Me.Cursor = Cursors.Hand
        Else
            Me.Cursor = Cursors.[Default]
        End If

        ' move the picked point
        If Me.ClientRectangle.Contains(e.Location) Then
            If drag AndAlso moveflag > 0 AndAlso moveflag < keyPt.Count - 1 Then
                If e.X > keyPt(moveflag - 1).X + 20 AndAlso e.X < keyPt(moveflag + 1).X - 20 Then
                    keyPt(moveflag) = e.Location
                Else

                    'keyPt.RemoveAt(moveflag)
                    'drag = False
                End If
            End If

            If drag AndAlso moveflag = 0 AndAlso e.X < keyPt(1).X - 20 Then
                keyPt(0) = e.Location
            End If

            If drag AndAlso moveflag = keyPt.Count - 1 AndAlso e.X > keyPt(keyPt.Count - 2).X + 20 Then
                keyPt(moveflag) = e.Location
            End If

            Invalidate()
        End If
    End Sub

    Protected Overrides Sub OnMouseUp(e As MouseEventArgs)
        MyBase.OnMouseUp(e)
        If drag Then
            drag = False
            getImageLevel()
            OnLevelChanged(New ImageLevelEventArgs(level))
        End If
    End Sub


    Protected Overrides Sub OnPaint(e As PaintEventArgs)
        MyBase.OnPaint(e)

        Dim g As Graphics = e.Graphics

        ' draw ruler

        ' draw curve
        g.DrawLine(New Pen(Color.Black), New Point(0, keyPt(0).Y), keyPt(0))
        g.DrawLine(New Pen(Color.Black), New Point(Me.Width, keyPt(keyPt.Count - 1).Y), keyPt(keyPt.Count - 1))

        Dim spline As New YLScsDrawing.Geometry.Spline()
        spline.ListDataPoint = keyPt
        spline.Precision = 5
        Dim splinePt As Point() = spline.SplinePoint
        g.DrawLines(New Pen(Color.Black), splinePt)
        g.DrawLine(New Pen(Color.Black), keyPt(keyPt.Count - 1), splinePt(splinePt.Length - 1))

        For Each pt As Point In keyPt
            Dim pts As Point() = New Point() {New Point(pt.X, pt.Y - 3), New Point(pt.X - 3, pt.Y), New Point(pt.X, pt.Y + 3), New Point(pt.X + 3, pt.Y)}
            g.FillPolygon(New SolidBrush(Color.Red), pts)
        Next
    End Sub
End Class
'End Namespace

