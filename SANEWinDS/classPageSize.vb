Public Class PageSize
    Public Width As Single 'inches
    Public Height As Single 'inches
    Public Name As String
    Public TWAIN_TWSS As TWAIN_VB.TWSS
    Public Sub New()
    End Sub
    Public Sub New(Width As Single, Height As Single)
        Me.Width = Width
        Me.Height = Height
    End Sub
    Public Sub New(Width As Single, Height As Single, Name As String)
        Me.Width = Width
        Me.Height = Height
        Me.Name = Name
    End Sub
    Public Sub New(Width As Single, Height As Single, Name As String, TWSS As TWAIN_VB.TWSS)
        Me.Width = Width
        Me.Height = Height
        Me.Name = Name
        Me.TWAIN_TWSS = TWSS
    End Sub
End Class

