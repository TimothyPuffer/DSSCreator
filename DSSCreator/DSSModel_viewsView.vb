Imports System.Xml.Serialization
Imports System.ComponentModel

Partial Public Class viewsView
    Implements INotifyPropertyChanged

    Public Function AddField() As viewsViewField

        Dim newField As New viewsViewField With {.name = "<none>"}
        Dim vvfList = Me.fields.ToList()
        vvfList.Add(newField)
        Me.fields = vvfList.ToArray

        RaisePropertyChanged("fields")

        Return newField
    End Function

    Public Sub RemoveField(ByVal removeField As viewsViewField)
        Dim vvfList = Me.fields.ToList()
        If (vvfList.Contains(removeField)) Then
            vvfList.Remove(removeField)
            Me.fields = vvfList.ToArray

            RaisePropertyChanged("fields")
        End If
    End Sub

    Public Sub RemoveParameter(ByVal removeParameter As viewsViewParameter)
        Dim vpList = Me.parameters.ToList()
        If vpList.Contains(removeParameter) Then
            vpList.Remove(removeParameter)
            Me.parameters = vpList.ToArray()
        End If
    End Sub

    Public Function AddParameter() As viewsViewParameter
        Dim vpList = Me.parameters.ToList()
        Dim addPara As New viewsViewParameter With {.name = "<none>"}
        vpList.Add(addPara)
        Me.parameters = vpList.ToArray()
        Return addPara
    End Function

    Public Sub MoveColumnUp(ByVal field As viewsViewField, ByVal upDirection As Boolean)
        If Not Me.fields.Contains(field) Or field Is Nothing Then
            Return
        End If
        Dim colIndex As Integer = Array.FindIndex(Me.fields, Function(f) field.Equals(f))
        Dim swapIndex As Integer

        If upDirection Then
            swapIndex = colIndex - 1
            If colIndex <= 0 Then
                Return
            End If
        Else
            swapIndex = colIndex + 1
            If colIndex >= Me.fields.Count - 1 Then
                Return
            End If
        End If

        Dim tempHold = Me.fields(colIndex)
        Me.fields(colIndex) = Me.fields(swapIndex)
        Me.fields(swapIndex) = tempHold

        Me.fields = Me.fields.ToList.ToArray

        RaisePropertyChanged("fields")

    End Sub

    <XmlIgnoreAttribute()>
    Public Property ShowChartBar() As Boolean
        Get
            Return ShowChartsContains("bar")
        End Get
        Set(value As Boolean)
            UpdateShowCharts("bar", value)
        End Set
    End Property

    <XmlIgnoreAttribute()> _
    Public Property ShowChartStacked() As Boolean
        Get
            Return ShowChartsContains("stacked")
        End Get
        Set(value As Boolean)
            UpdateShowCharts("stacked", value)
        End Set
    End Property

    <XmlIgnoreAttribute()>
    Public Property ShowChartPie() As Boolean
        Get
            Return ShowChartsContains("pie")
        End Get
        Set(value As Boolean)
            UpdateShowCharts("pie", value)
        End Set
    End Property

    <XmlIgnoreAttribute()>
    Public Property ShowChartLine() As Boolean
        Get
            Return ShowChartsContains("line")
        End Get
        Set(value As Boolean)
            UpdateShowCharts("line", value)
        End Set
    End Property

    Private Sub UpdateShowCharts(ByVal chartName As String, ByVal includeChart As Boolean)
        Dim chartList As New List(Of String)
        If ShowChartBar Then
            chartList.Add("bar")
        End If
        If ShowChartLine Then
            chartList.Add("line")
        End If
        If ShowChartPie Then
            chartList.Add("pie")
        End If
        If ShowChartStacked Then
            chartList.Add("stacked")
        End If

        If includeChart And Not chartList.Contains(chartName) Then
            chartList.Add(chartName)
        End If

        If Not includeChart And chartList.Contains(chartName) Then
            chartList.Remove(chartName)
        End If

        If chartList.Count = 0 Then
            Me.showChart = "false"
        Else
            Me.showChart = String.Join(",", chartList)
        End If
    End Sub

    Private Function ShowChartsContains(ByVal chartName As String) As Boolean
        If Me.showChart Is Nothing Then
            Return False
        End If
        Return Me.showChart.ToLower.Contains(chartName)
    End Function

    Public Event PropertyChanged(sender As Object, e As PropertyChangedEventArgs) Implements INotifyPropertyChanged.PropertyChanged

    Public Sub RaisePropertyChanged(ByVal e As String)
        RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(e))
    End Sub

End Class