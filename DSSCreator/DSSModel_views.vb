Imports System.Xml.Serialization
Imports System.ComponentModel
Imports System.IO

Partial Public Class views
    Implements INotifyPropertyChanged

    Public Function AddView() As viewsView
        Dim newViewID = 1

        If Me.view.Length > 0 Then
            newViewID = Me.view.Max(Function(v) v.id) + 1
        End If

        Dim newV As New viewsView With {.fields = New viewsViewField() {}, .parameters = New viewsViewParameter() {}, .relationship = New viewsViewRelationship(), .title = "<none>", .id = newViewID}
        Dim viewList = Me.view.ToList()
        viewList.Add(newV)
        Me.view = viewList.ToArray()

        RaisePropertyChanged("view")
        RaisePropertyChanged("ChildViews")
        Return newV

    End Function

    Public Sub RemoveView(ByVal view As viewsView)
        Dim viewList = Me.view.ToList()
        If viewList.Contains(view) Then
            viewList.Remove(view)
            Me.view = viewList.ToArray()
            RaisePropertyChanged("view")
            RaisePropertyChanged("ChildViews")
        End If
    End Sub

    Public Sub CopyParametersFromParent(ByVal childView As viewsView)
        Dim parentView = view.FirstOrDefault(Function(x) x.relationship.viewId = childView.id)

        Dim childPara = childView.parameters.ToList()


        Dim xmlS As XmlSerializer = New XmlSerializer(GetType(viewsViewParameter))
        For Each pPara In parentView.parameters.Reverse()
            Dim ms As New MemoryStream()
            xmlS.Serialize(ms, pPara)
            ms.Position = 0
            Dim copy As viewsViewParameter = CType(xmlS.Deserialize(ms), viewsViewParameter)
            childPara.Insert(0, copy)
            ms.Flush()
            ms.Dispose()
        Next

        childView.parameters = childPara.ToArray()

    End Sub

    Public Sub CopyFieldsFromParent(ByVal childView As viewsView)
        Dim parentView = view.FirstOrDefault(Function(x) x.relationship.viewId = childView.id)
        If parentView Is Nothing Then
            Return
        End If

        Dim l = New List(Of viewsViewField)

        Dim xmlS As XmlSerializer = New XmlSerializer(GetType(viewsViewField))
        For Each pPara In parentView.fields
            Dim ms As New MemoryStream()
            xmlS.Serialize(ms, pPara)
            ms.Position = 0
            Dim copy As viewsViewField = CType(xmlS.Deserialize(ms), viewsViewField)
            l.Add(copy)
            ms.Flush()
            ms.Dispose()
        Next

        childView.fields = l.ToArray()
        childView.RaisePropertyChanged("fields")
    End Sub

    <XmlIgnoreAttribute()>
    Public ReadOnly Property ChildViews() As Dictionary(Of String, String)
        Get
            Dim d As New Dictionary(Of String, String)
            d.Add(String.Empty, "<No Child View>")
            Me.view.ToList.ForEach(Sub(v) d.Add(v.id.ToString(), v.title))
            Return d
        End Get
    End Property

    Private Sub RaisePropertyChanged(ByVal e As String)
        RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(e))
    End Sub

    Public Event PropertyChanged(sender As Object, e As PropertyChangedEventArgs) Implements INotifyPropertyChanged.PropertyChanged

End Class
