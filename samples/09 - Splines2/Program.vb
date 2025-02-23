' Inspired by: "Programming & Using Splines - Part#2" -- @javidx9
' https://youtu.be/DzjtU4WLYNs

Imports VbConsoleGameEngine
Imports VbConsoleGameEngine.PixelType
Imports VbConsoleGameEngine.Color

Module Program

  Sub Main()
    Dim game As New Splines2
    game.ConstructConsole(160, 100, 8, 8)
    game.Start()
  End Sub

End Module

Class Splines2
  Inherits ConsoleGameEngine

  Private ReadOnly m_path As New Spline
  Private m_selectedPoint As Integer
  Private m_marker As Single

  Private m_modelCar As List(Of (Single, Single))

  Public Overrides Function OnUserCreate() As Boolean

    For i = 0 To 9
      m_path.Points.Add(New Point2D(30.0F * CSng(Math.Sin(i / 10.0F * 3.14159F * 2.0F)) + ScreenWidth() / 2.0F,
                                         30.0F * CSng(Math.Cos(i / 10.0F * 3.14159F * 2.0F)) + ScreenHeight() / 2.0F))
    Next

    m_modelCar = New List(Of (Single, Single)) From {(1, 1), (1, 3), (3, 0), (0, -3), (-3, 0), (-1, 3), (-1, 1)}

    Return True

  End Function

  Public Overrides Function OnUserUpdate(elapsedTime As Single) As Boolean

    ' Clear Screen
    Cls()

    ' Handle input
    If m_keys(AscW("X")).Released Then
      m_selectedPoint += 1
      If m_selectedPoint > m_path.Points.Count - 1 Then
        m_selectedPoint = 0
      End If
    End If
    If m_keys(AscW("Z")).Released Then
      m_selectedPoint -= 1
      If m_selectedPoint < 0 Then
        m_selectedPoint = m_path.Points.Count - 1
      End If
    End If
    If m_keys(VK_LEFT).Held Then m_path.Points(m_selectedPoint).X -= 30 * elapsedTime
    If m_keys(VK_RIGHT).Held Then m_path.Points(m_selectedPoint).X += 30 * elapsedTime
    If m_keys(VK_UP).Held Then m_path.Points(m_selectedPoint).Y -= 30 * elapsedTime
    If m_keys(VK_DOWN).Held Then m_path.Points(m_selectedPoint).Y += 30 * elapsedTime
    If m_keys(AscW("A")).Held Then m_marker -= 20.0F * elapsedTime
    If m_keys(AscW("S")).Held Then m_marker += 20.0F * elapsedTime

    If m_marker > m_path.TotalSplineLength Then m_marker -= m_path.TotalSplineLength
    If m_marker < 0 Then m_marker += m_path.TotalSplineLength

    ' Draw Spline
    For t = 0.0F To m_path.Points.Count Step 0.005F
      Dim pos = m_path.GetSplinePoint(t, True)
      Draw(pos.X, pos.Y)
    Next

    m_path.TotalSplineLength = 0.0

    ' Draw Control Points
    For i = 0 To m_path.Points.Count - 1
      m_path.Points(i).Length = m_path.CalculateSegmentLength(i, True)
      m_path.TotalSplineLength += m_path.Points(i).Length
      Fill(m_path.Points(i).X - 1, m_path.Points(i).Y - 1, m_path.Points(i).X + 2, m_path.Points(i).Y + 2, Solid, FgRed)
      DrawString(m_path.Points(i).X, m_path.Points(i).Y, CStr(i))
      DrawString(m_path.Points(i).X + 3, m_path.Points(i).Y, CStr(m_path.Points(i).Length))
    Next

    ' Highlight control point
    Fill(m_path.Points(m_selectedPoint).X - 1, m_path.Points(m_selectedPoint).Y - 1, m_path.Points(m_selectedPoint).X + 2, m_path.Points(m_selectedPoint).Y + 2, Solid, FgYellow)
    DrawString(m_path.Points(m_selectedPoint).X, m_path.Points(m_selectedPoint).Y, CStr(m_selectedPoint))

    ' Draw agent to demonstrate gradient
    Dim offset = m_path.GetNormalisedOffset(m_marker)
    Dim p1 = m_path.GetSplinePoint(offset, True)
    Dim g1 = m_path.GetSplineGradient(offset, True)
    Dim r = Math.Atan2(-g1.Y, g1.X)
    DrawLine(5.0F * Math.Sin(r) + p1.X, 5.0F * Math.Cos(r) + p1.Y, -5.0F * Math.Sin(r) + p1.X, -5.0F * Math.Cos(r) + p1.Y, Solid, FgBlue)

    DrawWireFrameModel(m_modelCar, CSng(p1.X), CSng(p1.Y), CSng(-r + (3.14159F / 2.0F)), 5.0F, FgCyan)

    DrawString(2, 2, CStr(offset))
    DrawString(2, 4, CStr(m_marker))

    Return True

  End Function

End Class

Class Point2D

  Public Property X As Single
  Public Property Y As Single
  Public Property Length As Single

  Public Sub New()
  End Sub

  Public Sub New(x As Single, y As Single)
    Me.X = x
    Me.Y = y
  End Sub

End Class

Class Spline

  Public Property Points As New List(Of Point2D)
  Public Property TotalSplineLength As Single

  Public Function GetSplinePoint(t As Single, Optional looped As Boolean = False) As Point2D

    Dim p0, p1, p2, p3 As Integer

    If Not looped Then
      p1 = CInt(Fix(t)) + 1
      p2 = p1 + 1
      p3 = p2 + 1
      p0 = p1 - 1
    Else
      p1 = CInt(Fix(t)) Mod Points.Count
      p2 = (p1 + 1) Mod Points.Count
      p3 = (p2 + 1) Mod Points.Count
      p0 = If(p1 >= 1, p1 - 1, Points.Count - 1)
    End If
    t -= CInt(Fix(t))

    Dim tt = t * t
    Dim ttt = tt * t

    Dim q1 = -ttt + 2.0F * tt - t
    Dim q2 = 3.0F * ttt - 5.0F * tt + 2.0F
    Dim q3 = -3.0F * ttt + 4.0F * tt + t
    Dim q4 = ttt - tt

    Dim tx = 0.5F * (Points(p0).X * q1 + Points(p1).X * q2 + Points(p2).X * q3 + Points(p3).X * q4)
    Dim ty = 0.5F * (Points(p0).Y * q1 + Points(p1).Y * q2 + Points(p2).Y * q3 + Points(p3).Y * q4)

    Return New Point2D(tx, ty)

  End Function

  Public Function GetSplineGradient(t As Single, Optional looped As Boolean = False) As Point2D

    Dim p0, p1, p2, p3 As Integer

    If Not looped Then
      p1 = CInt(Fix(t)) + 1
      p2 = p1 + 1
      p3 = p2 + 1
      p0 = p1 - 1
    Else
      p1 = CInt(Fix(t)) Mod Points.Count
      p2 = (p1 + 1) Mod Points.Count
      p3 = (p2 + 1) Mod Points.Count
      p0 = If(p1 >= 1, p1 - 1, Points.Count - 1)
    End If

    t -= CInt(Fix(t))

    Dim tt = t * t
    Dim ttt = tt * t

    Dim q1 = -3.0F * tt + 4.0F * t - 1.0F
    Dim q2 = 9.0F * tt - 10.0F * t
    Dim q3 = -9.0F * tt + 8.0F * t + 1.0F
    Dim q4 = 3.0F * tt - 2.0F * t

    Dim tx = 0.5F * (Points(p0).X * q1 + Points(p1).X * q2 + Points(p2).X * q3 + Points(p3).X * q4)
    Dim ty = 0.5F * (Points(p0).Y * q1 + Points(p1).Y * q2 + Points(p2).Y * q3 + Points(p3).Y * q4)

    Return New Point2D(tx, ty)

  End Function

  Public Function CalculateSegmentLength(node As Integer, Optional looped As Boolean = False) As Single

    Dim length = 0.0F
    Dim stepSize = 0.005F
    Dim oldPoint, newPoint As Point2D

    oldPoint = GetSplinePoint(node, looped)

    For t = 0.0F To 1.0F - stepSize Step stepSize
      newPoint = GetSplinePoint(node + t, looped)
      length += CSng(Math.Sqrt((newPoint.X - oldPoint.X) * (newPoint.X - oldPoint.X) + (newPoint.Y - oldPoint.Y) * (newPoint.Y - oldPoint.Y)))
      oldPoint = newPoint
    Next

    Return length

  End Function

  Public Function GetNormalisedOffset(p As Single) As Single
    ' Which node is the base?
    Dim i = 0
    While p > Points(i).Length
      p -= Points(i).Length
      i += 1
    End While
    ' The fractional is the offset 
    Return i + (p / Points(i).Length)
  End Function

End Class