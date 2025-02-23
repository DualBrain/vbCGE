Imports VbConsoleGameEngine.PixelType
Imports VbConsoleGameEngine.Color

Module Program
  Sub Main()
    Dim game As New Demo
    game.ConstructConsole(160, 100, 4, 4)
    game.Start()
  End Sub

End Module

Class Demo
  Inherits ConsoleGameEngine

  Private playerX As Single
  Private playerY As Single

  Public Overrides Function OnUserCreate() As Boolean

    playerX = 10
    playerY = 10

    Return True

  End Function

  Public Overrides Function OnUserUpdate(elapsedTime As Single) As Boolean

    ' ------
    ' Random fill
    ' ------

    'For x = 0 To ScreenWidth() - 1
    '  For y = 0 To ScreenHeight() - 1
    '    Draw(x, y, AscW("#"), Rand.Next(0, 15))
    '  Next
    'Next

    ' ------
    ' Color Swatch
    ' ------

    'Fill(0, 0, ScreenWidth, ScreenHeight, AscW(" "), 0)
    'For c = 0 To 15
    '  Fill(0, c * 6, 5, c * 6 + 5, QUARTER, c)
    '  Fill(6, c * 6, 11, c * 6 + 5, HALF, c)
    '  Fill(12, c * 6, 17, c * 6 + 5, THREEQUARTERS, c)
    '  Fill(18, c * 6, 23, c * 6 + 5, SOLID, c)

    '  Fill(24, c * 6, 29, c * 6 + 5, THREEQUARTERS, c Or BgWHITE)
    '  Fill(30, c * 6, 35, c * 6 + 5, HALF, c Or BgWHITE)
    '  Fill(36, c * 6, 41, c * 6 + 5, QUARTER, c Or BgWHITE)

    'Next

    ' ------
    ' Simple character movement
    ' ------

    'If m_keys(VK_LEFT).bHeld Then playerX -= 15 * elapsedTime
    'If m_keys(VK_RIGHT).bHeld Then playerX += 15 * elapsedTime
    'If m_keys(VK_UP).bHeld Then playerY -= 15 * elapsedTime
    'If m_keys(VK_DOWN).bHeld Then playerY += 15 * elapsedTime

    'Fill(0, 0, ScreenWidth, ScreenHeight, AscW(" "), 0)
    'Fill(playerX, playerY, playerX + 5, playerY + 5)

    ' ------
    ' Mouse - https://youtu.be/tdqc9hZhHxM
    ' ------

    Cls()

    Draw(m_mousePosX - 1, m_mousePosY)
    Draw(m_mousePosX, m_mousePosY)
    Draw(m_mousePosX + 1, m_mousePosY)
    Draw(m_mousePosX, m_mousePosY - 1)
    Draw(m_mousePosX, m_mousePosY + 1)

    If m_mouse(0).Held Then
      Fill(20, 20, 50, 50)
    End If

    Return True

  End Function

End Class