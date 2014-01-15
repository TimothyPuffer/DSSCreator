Imports System.Xml.Serialization
Imports System.IO
Imports Microsoft.Win32
Class MainWindow

#Region "File Open Save"
    Dim _currentView As views = Nothing
    Dim _currentOpenFile As String = Nothing
    Private Sub Button_Click_1(sender As Object, e As RoutedEventArgs)

        Dim d As New OpenFileDialog
        If d.ShowDialog() Then
            Dim fStream As Stream = d.OpenFile
            _currentOpenFile = d.FileName
            Dim xmlS As XmlSerializer = New XmlSerializer(GetType(views))
            _currentView = xmlS.Deserialize(fStream)
            MainContent.DataContext = Nothing
            MainContent.DataContext = _currentView
            fStream.Dispose()
        End If
        lbErrors.ItemsSource = Nothing
    End Sub

    Private Sub Button_Click_2(sender As Object, e As RoutedEventArgs)

        Dim d As New SaveFileDialog
        d.AddExtension = True
        d.DefaultExt = ".xml"
        If d.ShowDialog() And _currentView IsNot Nothing Then
            WriteDSSToFile(d.FileName)
        End If

    End Sub

    Private Sub Button_Click_3(sender As Object, e As RoutedEventArgs)
        WriteDSSToFile(_currentOpenFile)
    End Sub

    Private Sub WriteDSSToFile(ByVal filepath As String)

        _currentView.CleanUp()
        If filepath Is Nothing Or _currentView Is Nothing Then
            Return
        End If
        Dim ff As New StreamWriter(filepath, False)
        Dim xmlS As XmlSerializer = New XmlSerializer(GetType(views))

        xmlS.Serialize(ff, _currentView)
        ff.Flush()
        ff.Dispose()
    End Sub
#End Region

    Private Sub Button_Click_Add_field(sender As Object, e As RoutedEventArgs)
        Dim currentView As viewsView = CType(lbViews.SelectedItem, viewsView)
        If (currentView IsNot Nothing) Then
            lbColumns.SelectedItem = currentView.AddField()
        End If
    End Sub

    Private Sub Button_Click_Remove_field(sender As Object, e As RoutedEventArgs)
        Dim currentView As viewsView = CType(lbViews.SelectedItem, viewsView)
        Dim currentField As viewsViewField = CType(lbColumns.SelectedItem, viewsViewField)
        If currentField IsNot Nothing And currentView IsNot Nothing Then
            currentView.RemoveField(currentField)
            lbColumns.SelectedItem = Nothing
        End If
    End Sub

    Private Sub Button_Click_Add_View(sender As Object, e As RoutedEventArgs)
        If _currentView IsNot Nothing Then
            lbViews.SelectedItem = _currentView.AddView()
        End If
    End Sub

    Private Sub Button_Click_Remove_View(sender As Object, e As RoutedEventArgs)
        Dim currentView As viewsView = CType(lbViews.SelectedItem, viewsView)
        If currentView IsNot Nothing And _currentView IsNot Nothing Then
            _currentView.RemoveView(currentView)
            lbViews.SelectedItem = Nothing
        End If
    End Sub

    Private Sub Button_Click_Move_Up(sender As Object, e As RoutedEventArgs)
        MoveFieldUp(True)
    End Sub

    Private Sub Button_Click_Move_Down(sender As Object, e As RoutedEventArgs)
        MoveFieldUp(False)
    End Sub

    Private Sub MoveFieldUp(ByVal upMove As Boolean)
        Dim currentView As viewsView = CType(lbViews.SelectedItem, viewsView)
        Dim currentField As viewsViewField = CType(lbColumns.SelectedItem, viewsViewField)
        If currentField IsNot Nothing And currentView IsNot Nothing Then
            currentView.MoveColumnUp(currentField, upMove)
            lbColumns.SelectedItem = currentField
        End If
    End Sub

    Private Sub Button_Click_Copy_Parent_Parameters(sender As Object, e As RoutedEventArgs)
        Dim currentView As viewsView = CType(lbViews.SelectedItem, viewsView)

        If currentView IsNot Nothing And _currentView IsNot Nothing Then
            _currentView.CopyParametersFromParent(currentView)
        End If
    End Sub

    Private Sub Button_Click_Overwrite_Fields(sender As Object, e As RoutedEventArgs)
        Dim currentView As viewsView = CType(lbViews.SelectedItem, viewsView)

        If currentView IsNot Nothing And _currentView IsNot Nothing Then
            _currentView.CopyFieldsFromParent(currentView)
        End If
    End Sub

    Private Sub Button_Click_Add_Parameter(sender As Object, e As RoutedEventArgs)
        Dim currentView As viewsView = CType(lbViews.SelectedItem, viewsView)
        If currentView IsNot Nothing Then
            lbParameters.SelectedItem = currentView.AddParameter()
        End If
    End Sub

    Private Sub Button_Click_Remove_Parameter(sender As Object, e As RoutedEventArgs)
        Dim currentView As viewsView = CType(lbViews.SelectedItem, viewsView)
        Dim currentParameter As viewsViewParameter = CType(lbParameters.SelectedItem, viewsViewParameter)
        If currentView IsNot Nothing And currentParameter IsNot Nothing Then
            currentView.RemoveParameter(currentParameter)
            lbParameters.SelectedItem = Nothing
        End If

    End Sub

    Private Sub Button_Click_Error_Test(sender As Object, e As RoutedEventArgs)
        If _currentView IsNot Nothing Then
            Dim dssR As New DSSRules
            lbErrors.ItemsSource = dssR.TestForErrors(_currentView)
        End If

    End Sub

End Class
