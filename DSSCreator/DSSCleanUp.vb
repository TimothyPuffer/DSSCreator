Partial Public Class views

    Public Sub CleanUp()

        If view IsNot Nothing Then
            Me.view.Where(Function(x) x.fields.Count > 0).ToList().ForEach(Sub(x) x.xAxisLabel = x.fields(0).displayName)
        End If

        Dim newOrder As New List(Of viewsView)
        Dim firstpass = True
        Dim view_id = Nothing

        While (True)

            If firstpass Then
                firstpass = False

                Dim childviewIDs = From v In Me.view
                      Where Not String.IsNullOrEmpty(v.relationship.viewId)
                      Select v.relationship.viewId

                Dim head = Me.view.FirstOrDefault(Function(v) Not childviewIDs.Contains(v.id.ToString()))

                If head Is Nothing Then
                    Exit While
                End If

                newOrder.Add(head)
                view_id = head.relationship.viewId
            End If

            Dim nextView = Me.view.FirstOrDefault(Function(v) v.id.ToString = view_id)

            If nextView Is Nothing Then
                Exit While
            End If
            newOrder.Add(nextView)
            view_id = nextView.relationship.viewId
        End While

        If newOrder.Count = Me.view.Count Then
            Me.view = newOrder.ToArray()
        End If

        Dim lastView As viewsView = Nothing
        Dim id As Byte = 0

        For Each v In Me.view
            lastView = v

            lastView.id = id
            id = id + 1
            lastView.relationship.viewId = id.ToString()
        Next

        lastView.relationship.fieldName = Nothing
        lastView.relationship.viewId = Nothing

        For Each v In Me.view
            For Each f In v.fields
                f.label = f.displayName
            Next
        Next

        For Each v In Me.view
            If String.IsNullOrWhiteSpace(v.showChart) Then
                v.showChart = "false"
            End If

            For Each c In v.fields
                If Not String.IsNullOrWhiteSpace(c.formatting) Then
                    c.calcTotals = True
                End If
            Next

        Next

    End Sub




End Class
