Imports System.Xml.Serialization
Imports System.ComponentModel
Imports System.IO

Public Class DSSRules

    Public Function TestForErrors(ByVal viewToTest As views) As List(Of String)

        Dim listRet As New List(Of String)

        For Each v As viewsView In viewToTest.view
            For Each testFunc As Func(Of views, viewsView, String) In RulesviewsView
                Dim errorString = testFunc(viewToTest, v)
                If Not String.IsNullOrEmpty(errorString) Then
                    listRet.Add(errorString)
                End If
            Next

            For Each field In v.fields
                For Each testFuncField In RulesviewsViewField
                    Dim errorString1 = testFuncField(viewToTest, v, field)
                    If Not String.IsNullOrEmpty(errorString1) Then
                        listRet.Add(errorString1)
                    End If
                Next
            Next
        Next

        Return listRet

    End Function

    Public ReadOnly Property RulesviewsView As List(Of Func(Of views, viewsView, String))
        Get
            Dim listRet As New List(Of Func(Of views, viewsView, String))
            listRet.Add(AddressOf ViewParametersMatchParentParameters)
            listRet.Add(AddressOf ViewDefaultSortMustEndWith_ASC_or_DESC)
            listRet.Add(AddressOf ViewMustHaveAtLeastOneColumnPickForYAxis)
            listRet.Add(AddressOf FirstFieldCanNotBeOnYAxis)
            listRet.Add(AddressOf DefaultSortMustBeAColumn)
            listRet.Add(AddressOf ViewSQLHasRptVarSiteID)
            listRet.Add(AddressOf ViewParametersForParentRelationalFieldName)
            listRet.Add(AddressOf ViewHasDuplicateDisplayColumns)
            Return listRet
        End Get
    End Property

    Public ReadOnly Property RulesviewsViewField As List(Of Func(Of views, viewsView, viewsViewField, String))
        Get
            Dim listRet As New List(Of Func(Of views, viewsView, viewsViewField, String))
            listRet.Add(AddressOf SQLContainsColumn)
            Return listRet
        End Get
    End Property

    Public Function SQLContainsColumn(ByVal dssMain As views, ByVal childview As viewsView, ByVal field As viewsViewField) As String
        Dim errorString = String.Format("8 - ({0} - {1}) - SQL does not contain the field", childview.title, field.name)

        If childview.sql IsNot Nothing Then
            If childview.sql.IndexOf(field.name, StringComparison.OrdinalIgnoreCase) < 0 Then
                Return errorString
            End If
        End If


        Return Nothing
    End Function

    Public Function DefaultSortMustBeAColumn(ByVal dssMain As views, ByVal childview As viewsView) As String
        Dim errorString = String.Format("5 - ({0}) - Default Sort Must be a column", childview.title)

        If ViewDefaultSortMustEndWith_ASC_or_DESC(dssMain, childview) Is Nothing Then
            Dim s As String = childview.defaultSort.Substring(0, childview.defaultSort.IndexOf(Chr(32)))
            If childview.fields.FirstOrDefault(Function(c) c.name = s) Is Nothing Then
                Return errorString
            End If
        End If

        Return Nothing
    End Function

    Public Function FirstFieldCanNotBeOnYAxis(ByVal dssMain As views, ByVal childview As viewsView) As String
        Dim errorString = String.Format("4 - ({0}) - First Field can not the Y Axis", childview.title)

        If childview.fields.Count() > 0 Then
            If (childview.fields(0).axisY Or childview.fields(0).axisY2 Or childview.fields(0).axisY3 Or childview.fields(0).axisY4 Or childview.fields(0).axisY5) Then
                Return errorString
            End If
        End If

        Return Nothing
    End Function

    Public Function ViewMustHaveAtLeastOneColumnPickForYAxis(ByVal dssMain As views, ByVal childview As viewsView) As String
        Dim errorString = String.Format("3 - ({0}) - Does not have a Field Pick to be on the Y Axis", childview.title)

        If (String.IsNullOrEmpty(childview.relationship.viewId)) Then
            Return Nothing
        End If

        If childview.fields.Count(Function(c) c.valueField) > 0 Then
            Return Nothing
        End If

        Return errorString
    End Function

    Public Function ViewDefaultSortMustEndWith_ASC_or_DESC(ByVal dssMain As views, ByVal childview As viewsView) As String
        Dim errorString = String.Format("2 - ({0}) - Default sort must end in ASC or DESC and can only have one space and must be CAPITAL and it may only have one space inbetween", childview.title)

        If childview.defaultSort Is Nothing Then
            Return errorString
        End If

        If childview.defaultSort.Count(Function(c) c = Chr(32)) <> 1 Then
            Return errorString
        End If


        Dim s As String = childview.defaultSort.Substring(childview.defaultSort.IndexOf(Chr(32)) + 1)
        If (New String() {"ASC", "DESC"}).Contains(s) Then
            Return Nothing
        End If
        Return errorString
    End Function


    Public Function ViewParametersMatchParentParameters(ByVal dssMain As views, ByVal childview As viewsView) As String
        Dim errorString = String.Format("1 - ({0}) - Parameters don't match.  Make sure all of your parent View’s parameters match exactly in the same order. User the Copy parent parameters.", childview.title)

        Dim parentView = dssMain.view.FirstOrDefault(Function(x) x.relationship.viewId = childview.id.ToString)
        If parentView Is Nothing Then
            Return Nothing
        End If

        If parentView.parameters.Length > childview.parameters.Length Then
            Return errorString
        End If

        Dim xmlS As XmlSerializer = New XmlSerializer(GetType(viewsViewParameter))
        For i As Integer = 0 To parentView.parameters.Count - 1
            Dim first As New MemoryStream()
            Dim second As New MemoryStream()
            xmlS.Serialize(first, parentView.parameters(i))
            xmlS.Serialize(second, childview.parameters(i))

            If Not Enumerable.SequenceEqual(first.ToArray(), second.ToArray()) Then
                Return errorString
            End If
        Next

        Return Nothing
    End Function

    Public Function ViewParametersForParentRelationalFieldName(ByVal dssMain As views, ByVal childview As viewsView) As String
        Dim parentView = dssMain.view.FirstOrDefault(Function(x) x.relationship.viewId = childview.id.ToString)
        If parentView Is Nothing Then
            Return Nothing
        End If
        If String.IsNullOrWhiteSpace(parentView.relationship.fieldName) Then
            Return Nothing
        End If

        Dim errorString = String.Format("9 - ({0}) - Most have a Parameter with the Column Name ({1})", childview.title, parentView.relationship.fieldName)

        If childview.parameters.Count(Function(p) p.columnName = parentView.relationship.fieldName) = 0 Then
            If childview.parameters.Where(Function(p) p.type.ToUpper = "DYNAMICMULTILIST").Count(Function(p) p.columnName.ToUpper() = parentView.relationship.fieldName.ToUpper().Replace("_ID", "")) = 0 Then
                Return errorString
            End If
            Return Nothing
        Else
            Return Nothing
        End If

    End Function

    Public Function ViewSQLHasRptVarSiteID(ByVal dssMain As views, ByVal childview As viewsView) As String
        Dim errorString = String.Format("6 - ({0}) - SQL must include RptVarSiteID", childview.title)
        If childview.sql IsNot Nothing Then

            If Not childview.sql.ToUpper().Contains("RPTVARSITEID") Then
                Return errorString
            End If
        End If

        Return Nothing
    End Function

    Public Function ViewHasDuplicateDisplayColumns(ByVal dssMain As views, ByVal childview As viewsView) As String

        Dim dups = From f In childview.fields _
                   Group f By f.displayName Into g = Group _
                   Where g.Count() > 1
                   Select g.First().displayName

        For Each f In dups
            Return String.Format("10 - ({0}) - Has Duplicate Display Name ({1})", childview.title, f)
        Next

        Return Nothing
    End Function

End Class
