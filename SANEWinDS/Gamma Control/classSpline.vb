Imports System.Collections.Generic
Imports System.Drawing

Namespace YLScsDrawing.Geometry
    Public Class Spline
        Private m_dataPoint As Vector()
        Private controlPoint As Vector()
        Private m_splinePoint As New List(Of Vector)()
        Private m_precision As Double = 0.1
        Private isXcalibrated As Boolean = True

        Public Property Precision() As Double
            Get
                Return m_precision
            End Get
            Set(value As Double)
                m_precision = value
            End Set
        End Property

        Public WriteOnly Property IsXaxisCalibrated() As Boolean
            Set(value As Boolean)
                isXcalibrated = value
            End Set
        End Property

        Public WriteOnly Property DataPointF() As PointF()
            Set(value As PointF())
                Dim n As Integer = value.Length
                m_dataPoint = New Vector(n - 1) {}
                For i As Integer = 0 To n - 1
                    m_dataPoint(i) = New Vector(value(i))
                Next
            End Set
        End Property

        Public WriteOnly Property DataPoint() As Point()
            Set(value As Point())
                Dim n As Integer = value.Length
                m_dataPoint = New Vector(n - 1) {}
                For i As Integer = 0 To n - 1
                    m_dataPoint(i) = New Vector(value(i))
                Next
            End Set
        End Property

        Public WriteOnly Property ListDataPointF() As List(Of PointF)
            Set(value As List(Of PointF))
                Dim n As Integer = value.Count
                m_dataPoint = New Vector(n - 1) {}
                For i As Integer = 0 To n - 1
                    m_dataPoint(i) = New Vector(value(i))
                Next
            End Set
        End Property

        Public WriteOnly Property ListDataPoint() As List(Of Point)
            Set(value As List(Of Point))
                Dim n As Integer = value.Count
                m_dataPoint = New Vector(n - 1) {}
                For i As Integer = 0 To n - 1
                    m_dataPoint(i) = New Vector(value(i))
                Next
            End Set
        End Property

        Public ReadOnly Property SplinePoint() As Point()
            Get
                getSplinePoints()
                Dim pts As Point() = New Point(m_splinePoint.Count - 1) {}
                For i As Integer = 0 To pts.Length - 1
                    pts(i) = m_splinePoint(i).ToPoint()
                Next
                Return pts
            End Get
        End Property

        Public ReadOnly Property SplinePointF() As PointF()
            Get
                getSplinePoints()
                Dim pts As PointF() = New PointF(m_splinePoint.Count - 1) {}
                For i As Integer = 0 To pts.Length - 1
                    pts(i) = m_splinePoint(i).ToPointF()
                Next
                Return pts
            End Get
        End Property

        Private Sub getSplinePoints()
            m_splinePoint.Clear()
            If m_dataPoint.Length = 1 Then
                m_splinePoint.Add(m_dataPoint(0))
            End If

            If m_dataPoint.Length = 2 Then
                Dim n As Integer = 1
                If isXcalibrated Then
                    n = CInt((m_dataPoint(1).X - m_dataPoint(0).X) / m_precision)
                Else
                    n = CInt((m_dataPoint(1).Y - m_dataPoint(0).Y) / m_precision)
                End If
                If n = 0 Then
                    n = 1
                End If
                If n < 0 Then
                    n = -n
                End If
                For j As Integer = 0 To n - 1
                    Dim t As Double = CDbl(j) / CDbl(n)

                    m_splinePoint.Add((1 - t) * m_dataPoint(0) + t * m_dataPoint(1))
                Next
            End If

            If m_dataPoint.Length > 2 Then
                getControlPoints()

                'draw bezier curves using Bernstein Polynomials
                For i As Integer = 0 To controlPoint.Length - 2
                    Dim b1 As Vector = controlPoint(i) * 2.0 / 3.0 + controlPoint(i + 1) / 3.0
                    Dim b2 As Vector = controlPoint(i) / 3.0 + controlPoint(i + 1) * 2.0 / 3.0

                    Dim n As Integer = 1
                    If isXcalibrated Then
                        n = CInt((m_dataPoint(i + 1).X - m_dataPoint(i).X) / m_precision)
                    Else
                        n = CInt((m_dataPoint(i + 1).Y - m_dataPoint(i).Y) / m_precision)
                    End If
                    If n = 0 Then
                        n = 1
                    End If
                    If n < 0 Then
                        n = -n
                    End If
                    For j As Integer = 0 To n - 1
                        Dim t As Double = CDbl(j) / CDbl(n)
                        Dim v As Vector = (1 - t) * (1 - t) * (1 - t) * m_dataPoint(i) + 3 * (1 - t) * (1 - t) * t * b1 + 3 * (1 - t) * t * t * b2 + t * t * t * m_dataPoint(i + 1)
                        m_splinePoint.Add(v)
                    Next
                Next
            End If
        End Sub

        Private Sub getControlPoints()
            If m_dataPoint IsNot Nothing AndAlso m_dataPoint.Length = 3 Then
                controlPoint = New Vector(2) {}
                controlPoint(0) = m_dataPoint(0)
                controlPoint(1) = (6 * m_dataPoint(1) - m_dataPoint(0) - m_dataPoint(2)) / 4
                controlPoint(2) = m_dataPoint(2)
            End If

            If m_dataPoint IsNot Nothing AndAlso m_dataPoint.Length > 3 Then
                Dim n As Integer = m_dataPoint.Length
                controlPoint = New Vector(n - 1) {}
                Dim diag As Double() = New Double(n - 1) {}
                ' tridiagonal matrix a(i , i)
                Dim [sub] As Double() = New Double(n - 1) {}
                ' tridiagonal matrix a(i , i-1)
                Dim sup As Double() = New Double(n - 1) {}
                ' tridiagonal matrix a(i , i+1)
                For i As Integer = 0 To n - 1
                    controlPoint(i) = m_dataPoint(i)
                    diag(i) = 4
                    [sub](i) = 1
                    sup(i) = 1
                Next

                controlPoint(1) = 6 * controlPoint(1) - controlPoint(0)
                controlPoint(n - 2) = 6 * controlPoint(n - 2) - controlPoint(n - 1)

                For i As Integer = 2 To n - 3
                    controlPoint(i) = 6 * controlPoint(i)
                Next

                ' Gaussian elimination fron row 1 to n-2
                For i As Integer = 2 To n - 2
                    [sub](i) = [sub](i) / diag(i - 1)
                    diag(i) = diag(i) - [sub](i) * sup(i - 1)
                    controlPoint(i) = controlPoint(i) - [sub](i) * controlPoint(i - 1)
                Next

                controlPoint(n - 2) = controlPoint(n - 2) / diag(n - 2)

                For i As Integer = n - 3 To 1 Step -1
                    controlPoint(i) = (controlPoint(i) - sup(i) * controlPoint(i + 1)) / diag(i)
                Next
            End If
        End Sub
    End Class
End Namespace

