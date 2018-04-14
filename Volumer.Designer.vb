<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class VolumerForm
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()>
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        Me.SuspendLayout()
        '
        'VolumerForm
        '
        Me.BackColor = System.Drawing.Color.Fuchsia
        Me.ClientSize = New System.Drawing.Size(0, 0)
        Me.ControlBox = False
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "VolumerForm"
        Me.Opacity = 0R
        Me.ShowIcon = False
        Me.ShowInTaskbar = False
        Me.TransparencyKey = System.Drawing.Color.Fuchsia
        Me.ResumeLayout(False)

    End Sub
End Class
