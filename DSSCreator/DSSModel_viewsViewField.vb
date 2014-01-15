Imports System.ComponentModel

Partial Public Class viewsViewField
    Implements INotifyPropertyChanged

    Public Property OnY1Axis As Boolean
        Get
            Return valueField And Not (axisY2 Or axisY3 Or axisY4 Or axisY5)
        End Get
        Set(value As Boolean)

            valueField = value

            axisY = False
            axisY2 = False Or OnY2Axis
            axisY3 = False
            axisY4 = False
            axisY5 = False

            RaisePropertyChanged("OnY1Axis")
            RaisePropertyChanged("OnY2Axis")
        End Set
    End Property

    Public Property OnY2Axis As Boolean
        Get
            Return axisY2 = True
        End Get
        Set(value As Boolean)
            valueField = value


            axisY = False
            axisY2 = value
            axisY3 = False
            axisY4 = False
            axisY5 = False

            RaisePropertyChanged("OnY1Axis")
            RaisePropertyChanged("OnY2Axis")
        End Set
    End Property

    Private Sub RaisePropertyChanged(ByVal e As String)
        RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(e))
    End Sub

    Public Event PropertyChanged(sender As Object, e As PropertyChangedEventArgs) Implements INotifyPropertyChanged.PropertyChanged

End Class
