
Imports System.Runtime.InteropServices

Public Class VolumerForm

    <DllImport("user32.dll", SetLastError:=True, CharSet:=CharSet.Auto)> Private Shared Function SendMessage(ByVal hWnd As IntPtr, ByVal Msg As UInteger, ByVal wParam As IntPtr, ByVal lParam As IntPtr) As IntPtr
    End Function

    Dim ControlTimer As New Timer
    Dim cursorOnTaskbar As Boolean
    Dim TaskBarRect As Rectangle = Rectangle.Intersect(Screen.PrimaryScreen.WorkingArea, Screen.PrimaryScreen.Bounds)
    Dim CropRect As New Rectangle

    Private Sub VolumerForm_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        ' Checking where is your taskbak
        ' If it's width equals to your screen's width, it's horizontal
        ' If it's height equals to your screen's height, it's vertical
        If TaskBarRect.Width = Screen.PrimaryScreen.Bounds.Width Then
            ' If it's y position is not 0, it's on top
            ' If not, it's on bottom
            If TaskBarRect.Y <> 0 Then
                CropRect = New Rectangle(0, 0, TaskBarRect.Width, TaskBarRect.Y)
            Else
                CropRect = New Rectangle(0, TaskBarRect.Height, TaskBarRect.Width, Screen.PrimaryScreen.Bounds.Height)
            End If
        ElseIf TaskBarRect.Height = Screen.PrimaryScreen.Bounds.Height Then
            ' If it's x position is not 0, it's on left
            ' If not, it's on right
            If TaskBarRect.X <> 0 Then
                CropRect = New Rectangle(0, 0, TaskBarRect.X, TaskBarRect.Height)
            Else
                CropRect = New Rectangle(TaskBarRect.Width, 0, Screen.PrimaryScreen.Bounds.Width, TaskBarRect.Height)
            End If
        Else
            ' No taskbar found.
            Application.Exit()
        End If

        ' Set timer & start
        AddHandler ControlTimer.Tick, AddressOf ControlTimer_Tick
        ControlTimer.Interval = 1
        ControlTimer.Enabled = True

        ' Start global mouse hook
        MouseHook.Start()
        AddHandler MouseHook.MouseWheel, AddressOf Mouse_Wheel

    End Sub

    Private Sub VolumerForm_Closing(sender As Object, e As EventArgs) Handles Me.FormClosing
        MouseHook.Stop()
    End Sub

    Private Sub Mouse_Wheel(ByVal sender As Object, ByVal e As EventArgs)

        If (cursorOnTaskbar) Then
            If (CInt(MouseHook.MouseWheelInfo.ToString) > 0) Then
                SendMessage(Handle, &H319, &H30292, &HA * &H10000) ' Volume Up
            Else
                SendMessage(Handle, &H319, &H30292, &H9 * &H10000) ' Volume Down
            End If
        End If

    End Sub

    Private Sub ControlTimer_Tick(sender As Object, e As EventArgs)

        Dim posx = Cursor.Position.X
        Dim posy = Cursor.Position.Y

        ' Hide form
        If Me.Visible Then
            Me.Hide()
        End If

        ' Check if cursor is on taskbar
        If posx >= CropRect.X And posx <= CropRect.Width And posy >= CropRect.Y And posy <= CropRect.Height Then
            cursorOnTaskbar = True
        Else
            cursorOnTaskbar = False
        End If

    End Sub
End Class


Public NotInheritable Class MouseHook

    Public Shared Event MouseWheel As EventHandler
    Public Shared MouseWheelInfo As String

    Public Shared Sub Start()
        _hookID = SetHook(_proc)
    End Sub

    Public Shared Sub [Stop]()
        UnhookWindowsHookEx(_hookID)
    End Sub

    Private Shared _proc As LowLevelMouseProc = AddressOf HookCallback
    Private Shared _hookID As IntPtr = IntPtr.Zero

    Private Shared Function SetHook(ByVal proc As LowLevelMouseProc) As IntPtr
        Using curProcess As Process = Process.GetCurrentProcess()
            Using curModule As ProcessModule = curProcess.MainModule
                Dim hook As IntPtr = SetWindowsHookEx(14, proc, GetModuleHandle("user32"), 0)
                If hook = IntPtr.Zero Then
                    Throw New System.ComponentModel.Win32Exception()
                End If
                Return hook
            End Using
        End Using
    End Function

    Private Delegate Function LowLevelMouseProc(ByVal nCode As Integer, ByVal wParam As IntPtr, ByVal lParam As IntPtr) As IntPtr

    Private Shared Function HookCallback(ByVal nCode As Integer, ByVal wParam As IntPtr, ByVal lParam As IntPtr) As IntPtr
        If nCode >= 0 AndAlso MouseMessages.WM_MOUSEWHEEL = CType(wParam, MouseMessages) Then
            Dim hookStruct As MSLLHOOKSTRUCT = CType(Marshal.PtrToStructure(lParam, GetType(MSLLHOOKSTRUCT)), MSLLHOOKSTRUCT)
            Dim v As Integer = CInt((hookStruct.mouseData And &HFFFF0000) >> 16)
            If v > SystemInformation.MouseWheelScrollDelta Then v = v - (UShort.MaxValue + 1)
            MouseWheelInfo = v.ToString
            RaiseEvent MouseWheel(Nothing, New EventArgs())
        End If
        Return CallNextHookEx(_hookID, nCode, wParam, lParam)
    End Function

    Private Enum MouseMessages
        WM_MOUSEWHEEL = &H20A
    End Enum

    <StructLayout(LayoutKind.Sequential)>
    Private Structure POINT
        Public x As Integer
        Public y As Integer
    End Structure

    <StructLayout(LayoutKind.Sequential)>
    Private Structure MSLLHOOKSTRUCT
        Public pt As POINT
        Public mouseData As UInteger
        Public flags As UInteger
        Public time As UInteger
        Public dwExtraInfo As IntPtr
    End Structure

    <DllImport("user32.dll", CharSet:=CharSet.Auto, SetLastError:=True)>
    Private Shared Function SetWindowsHookEx(ByVal idHook As Integer, ByVal lpfn As LowLevelMouseProc, ByVal hMod As IntPtr, ByVal dwThreadId As UInteger) As IntPtr
    End Function

    <DllImport("user32.dll", CharSet:=CharSet.Auto, SetLastError:=True)>
    Private Shared Function UnhookWindowsHookEx(ByVal hhk As IntPtr) As <MarshalAs(UnmanagedType.Bool)> Boolean
    End Function

    <DllImport("user32.dll", CharSet:=CharSet.Auto, SetLastError:=True)>
    Private Shared Function CallNextHookEx(ByVal hhk As IntPtr, ByVal nCode As Integer, ByVal wParam As IntPtr, ByVal lParam As IntPtr) As IntPtr
    End Function

    <DllImport("kernel32.dll", CharSet:=CharSet.Auto, SetLastError:=True)>
    Private Shared Function GetModuleHandle(ByVal lpModuleName As String) As IntPtr
    End Function

End Class