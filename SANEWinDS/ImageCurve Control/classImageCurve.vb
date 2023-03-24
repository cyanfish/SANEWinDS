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
        RaiseEvent ImageLevelChanged(Me, e)
    End Sub

    Public Property LevelValue() As Integer()
        Get
            getImageLevel()
            Return level
        End Get
        Set(value As Integer())
            If value.Length <> level.Length Then Throw New ArgumentOutOfRangeException("Incorrect array length supplied")
            For i As Integer = 0 To value.Length - 1
                If value(i) > _MaxY Then
                    level(i) = _MaxY
                ElseIf value(i) < 0 Then
                    level(i) = 0
                Else
                    level(i) = value(i)
                End If
            Next
            CalcKeyPtsFromLevels()
            OnLevelChanged(New ImageLevelEventArgs(level))
        End Set
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

    Private Sub CalcKeyPtsFromLevels()
        Dim level_pts As New List(Of Point)
        For i As Integer = 0 To Me.level.Length - 1
            level_pts.Add(New Point(i, Me.level(i)))
        Next

        level_pts = SimplifyPolylineAdaptive(level_pts, MaxY * 0.2, MaxY * 0.0005, MaxY)

        Dim keyPts As New List(Of Point)
        For Each lvl As Point In level_pts
            Dim key As New Point()
            key.X = lvl.X * Me.Width / MaxX
            key.Y = Me.Height - lvl.Y * Me.Height / MaxY
            keyPts.Add(key)
        Next

        keyPt = keyPts
    End Sub

    Function SimplifyPolyline(pts As List(Of Point), tolerance As Double) As List(Of Point)
        If pts.Count < 3 Then
            Return pts
        End If

        Dim dmax As Double = 0.0
        Dim index As Integer = 0
        Dim endpt As Integer = pts.Count - 1

        For i As Integer = 1 To endpt - 1
            Dim d As Double = LineToPointDistance(pts(i), pts(0), pts(endpt))
            If d > dmax Then
                index = i
                dmax = d
            End If
        Next

        If dmax > tolerance Then
            Dim left As List(Of Point) = SimplifyPolyline(pts.GetRange(0, index + 1), tolerance)
            Dim right As List(Of Point) = SimplifyPolyline(pts.GetRange(index, endpt - index + 1), tolerance)

            Dim result As New List(Of Point)(left.Take(left.Count - 1))
            result.AddRange(right)

            Return result
        Else
            Return New List(Of Point)({pts(0), pts(endpt)})
        End If
    End Function

    Function SimplifyPolylineAdaptive(pts As List(Of Point), tolerance As Double, minTolerance As Double, maxTolerance As Double) As List(Of Point)
        If pts.Count < 3 Then
            Return pts
        End If

        Dim dmax As Double = 0.0
        Dim index As Integer = 0
        Dim endpt As Integer = pts.Count - 1

        For i As Integer = 1 To endpt - 1
            Dim d As Double = LineToPointDistance(pts(i), pts(0), pts(endpt))
            If d > dmax Then
                index = i
                dmax = d
            End If
        Next

        Dim currentTolerance As Double = tolerance

        If dmax > currentTolerance Then
            If currentTolerance < maxTolerance Then
                currentTolerance = Math.Min(maxTolerance, currentTolerance * 2)
            Else
                currentTolerance = maxTolerance
            End If

            Dim left As List(Of Point) = SimplifyPolylineAdaptive(pts.GetRange(0, index + 1), currentTolerance, minTolerance, maxTolerance)
            Dim right As List(Of Point) = SimplifyPolylineAdaptive(pts.GetRange(index, endpt - index + 1), currentTolerance, minTolerance, maxTolerance)

            Dim result As New List(Of Point)(left.Take(left.Count - 1))
            result.AddRange(right)

            Return result
        Else
            If currentTolerance > minTolerance Then
                currentTolerance = Math.Max(minTolerance, currentTolerance / 2)
                Return SimplifyPolylineAdaptive(pts, currentTolerance, minTolerance, maxTolerance)
            Else
                Return New List(Of Point)({pts(0), pts(endpt)})
            End If
        End If
    End Function

    Function LineToPointDistance(pt As Point, lnA As Point, lnB As Point) As Double
        Dim v1 As New YLScsDrawing.Geometry.Vector(lnA, lnB)
        Dim v2 As New YLScsDrawing.Geometry.Vector(lnA, pt)
        v1 /= v1.Magnitude
        Return Math.Abs(v2.CrossProduct(v1))
    End Function

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
    Private removing_point As Boolean = False
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
                        removing_point = True
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
        If drag Or removing_point Then
            drag = False
            removing_point = False
            getImageLevel()
            '
            'Show calculated result immediately:
            'CalcKeyPtsFromLevels()
            'getImageLevel()
            '
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

