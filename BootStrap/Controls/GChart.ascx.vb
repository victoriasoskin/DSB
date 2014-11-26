Imports System.Data.SqlClient
Imports System.Linq
Imports System.Web.UI.DataVisualization.Charting
Imports System.Dynamic
Imports System.Drawing
Imports System.Reflection
Imports System.Xml.Linq

Namespace BootStrap
    Partial Class Controls_GChart
        Inherits System.Web.UI.UserControl
#Region "Properties And Fields"
        Private XMLDataSourceValue As String
        Public Property XMLDataSource() As String
            Get
                Return XMLDataSourceValue
            End Get
            Set(ByVal value As String)
                XMLDataSourceValue = value
            End Set
        End Property
        Private ReportIDValue As Integer
        Public Property ReportID() As Integer
            Get
                Return ReportIDValue
            End Get
            Set(ByVal value As Integer)
                ReportIDValue = value
            End Set
        End Property
        Private SubReportValue As String
        Public Property SubReport() As String
            Get
                Return SubReportValue
            End Get
            Set(ByVal value As String)
                SubReportValue = value
            End Set
        End Property
        Private ParamsValue As String
        Public Property Params() As String
            Get
                Return ParamsValue
            End Get
            Set(ByVal value As String)
                ParamsValue = value
            End Set
        End Property
        Private WidthValue As String
        Public Property Width() As String
            Get
                Return WidthValue
            End Get
            Set(ByVal value As String)
                WidthValue = value
                Chart1.Width = WidthValue
            End Set
        End Property
        Private HeightValue As String
        Public Property Height() As String
            Get
                Return HeightValue
            End Get
            Set(ByVal value As String)
                HeightValue = value
                Chart1.Height = HeightValue
            End Set
        End Property

        Dim sName As String = vbNullString
        Dim sQuery As String = vbNullString
        Dim sLegendLabels As String = vbNullString
        Dim sXlabels As String = vbNullString
        Dim sYvalues As String = vbNullString
        Dim sChrtType As String = vbNullString
        Dim sDataSourceID As String = vbNullString
        Dim sSubtitle As String = vbNullString
        Dim sUrl As String = vbNullString
        Dim sTarget As String = vbNullString

        Dim sEntity As String = vbNullString
        Dim sMember As String = vbNullString
        Dim sEnum As String = vbNullString
        Dim sProperty As String = vbNullString
        Dim sPropertyType As String = vbNullString
        Dim sDataType As String = vbNullString
        Dim sValue As String = vbNullString

        Dim cLabels As New Collection
        Dim cSlabel As New Collection
        Dim cValues As New Collection
        Dim cSeries As New Collection
        Dim cLegend As New Collection
        Dim cUrls As New Collection
        Dim srs As Series
        Dim xE As XElement
#End Region

#Region "binding Event"
        Protected Sub Page_DataBinding(sender As Object, e As System.EventArgs) Handles Me.DataBinding
            ShowChart(ReportID, SubReport)
        End Sub
#End Region

#Region "Show Chart"
        Sub ShowChart(iRepID As Integer, sSub As String)

            ' Read XML Definition of Query

            readCharXML(iRepID, sSub)

            '  If ReportID Is empty exit

            If sName = vbNullString Then
                xE = Nothing
                Exit Sub
            End If

            ' Update Query and Name With Parameters

            updateQueryParameters()

            ' Read Query From Data Base And prepare For the chart

            readChartData()

            ' Add Missing points

            addMissingPoints()

            'build Chart

            Dim Chrt As Chart = Chart1
            buildChart(Chrt)

            'Chart PreSetting fORMATING

            ChartPreSettings(sChrtType)

            ' Chart Post Setting Formating

            ChartPostSettings(iRepID, sSub)

        End Sub
#Region "Helpers"
        Private Sub readCharXML(iRepID As Integer, sSub As String)
            'Open Source if not already openned

            If xE Is Nothing Then xE = XElement.Load(HttpContext.Current.Server.MapPath("~/" & XMLDataSource))

            ' Read Chart Definition

            If iRepID = 0 Then Exit Sub

            Dim q = From l In xE.Descendants("Query") _
                Where l.Parent.Attribute("ID").Value = iRepID And l.Parent.Attribute("SubID").Value = sSub _
                    Select New With { _
                        .DSid = l.Attribute("DataSourceID").Value, _
                        .nam = l.Parent.Attribute("Name").Value, _
                        .Subt = If(l.Parent.Attribute("SubTitle") Is Nothing, vbNullString, l.Parent.Attribute("SubTitle").Value), _
                .Query = If(l IsNot Nothing, l.Value, vbNullString), _
                .LegendLabels = If(l.Attribute("LegandLabels") IsNot Nothing, l.Attribute("LegandLabels").Value, vbNullString), _
                .XLabels = If(l.Attribute("XLabels") IsNot Nothing, l.Attribute("XLabels").Value, vbNullString), _
                .YValues = If(l.Attribute("YValues") IsNot Nothing, l.Attribute("YValues").Value, vbNullString), _
                .ChartType = If(l.Attribute("ChartType") IsNot Nothing, l.Attribute("ChartType").Value, vbNullString), _
                        .Url = If(l.Attribute("Url") IsNot Nothing, l.Attribute("Url").Value, vbNullString), _
                        .Trgt = If(l.Attribute("Target") IsNot Nothing, l.Attribute("Target").Value, vbNullString)}
            For Each l In q
                sName = l.nam
                sQuery = If(l.Query Is Nothing, vbNullString, l.Query)
                sLegendLabels = If(l.LegendLabels Is Nothing, vbNullString, l.LegendLabels)
                sXlabels = If(l.XLabels Is Nothing, vbNullString, l.XLabels)
                sYvalues = If(l.YValues Is Nothing, vbNullString, l.YValues)
                sChrtType = If(l.ChartType Is Nothing, vbNullString, l.ChartType)
                sDataSourceID = If(l.DSid Is Nothing, vbNullString, l.DSid)
                sSubtitle = l.Subt
                sUrl = l.Url
                sTarget = l.Trgt
                Exit For
            Next
        End Sub
        Private Sub readChartData()

            'Read Data And Fill Points

            Dim connStr As String = ConfigurationManager.ConnectionStrings(sDataSourceID).ConnectionString
            Dim dbConnection As System.Data.IDbConnection = New System.Data.SqlClient.SqlConnection(connStr)
            Dim cD As New SqlCommand(sQuery, dbConnection)
            dbConnection.Open()
            Dim dr As SqlDataReader
            Try
                dr = cD.ExecuteReader
            Catch ex As Exception
                Response.Write(ex.Message & "<br /><br />" & cD.CommandText)
                'dr.Close()
                dbConnection.Close()
                Response.End()
            End Try

            If sLegendLabels <> vbNullString Then
                Chart1.Legends.Add("L")
            End If

            While (dr.Read)
                Dim seriesName As String = "A" & If(sLegendLabels = vbNullString, vbNullString, dr(sLegendLabels))
                Try
                    cSeries.Add(seriesName, seriesName)
                    cLegend.Add(dr(sLegendLabels), seriesName)
                Catch ex As Exception
                End Try
                If Not IsDBNull(dr(sXlabels)) Then
                    Dim sLbl As String = dr(sXlabels)
                    If sLbl <> vbNullString Then
                        Try
                            cLabels.Add(sLbl, sLbl)
                        Catch ex As Exception
                        End Try
                        Try
                            cSlabel.Add(sLbl, sLbl & seriesName)
                            cValues.Add(If(IsDBNull(dr(sYvalues)), 0, dr(sYvalues)), sLbl & seriesName)
                            If sUrl <> vbNullString Then cUrls.Add(dr(sUrl), sLbl & seriesName)
                        Catch ex As Exception
                        End Try
                    End If
                End If
            End While

            dr.Close()
            dbConnection.Close()

        End Sub
        Private Sub updateQueryParameters()

            sQuery = sQuery.Replace("{UserID}", Format(CInt(Session("UserID")), "000"))

            If Params <> vbNullString Then
                Dim p0() As String = Params.Split("|")
                For i = 0 To p0.Length - 1
                    Dim p1() As String = p0(i).Split("=")
                    sQuery = sQuery.Replace("{" & p1(0) & "}", p1(1))
                    sName = sName.Replace("{" & p1(0) & "}", p1(1))
                    sSubtitle = sSubtitle.Replace("{" & p1(0) & "}", p1(1))
                Next
            End If
        End Sub
        Private Sub addMissingPoints()
            For Each sSer As String In cSeries
                For Each sLbl In cLabels
                    Try
                        Dim x As Double = cValues(sLbl & sSer)
                    Catch ex As Exception
                        cSlabel.Add(sLbl, sLbl & sSer)
                        cValues.Add(0, sLbl & sSer)
                    End Try
                Next
            Next
        End Sub
        Private Sub buildChart(Chrt As Chart)
            Chrt.ChartAreas.Clear()
            Chrt.Series.Clear()
            Chrt.Legends.Clear()

            Chrt.ChartAreas.Add("A")

            Chrt.Titles.Add(sName)
            If sSubtitle <> vbNullString Then Chrt.Titles.Add(sSubtitle)
            If sLegendLabels <> vbNullString Then Chrt.Legends.Add("L")


            For Each sSer In cSeries
                Chrt.Series.Add(sSer)
                srs = Chrt.Series(sSer)
                If sLegendLabels <> vbNullString Then
                    Try
                        srs.LegendText = cLegend(sSer)
                    Catch ex As Exception
                    End Try
                End If

                srs.ChartTypeName = sChrtType
                For Each sLbl As String In cLabels
                    srs.Points.AddXY(sLbl, cValues(sLbl & sSer))
                    If sUrl <> vbNullString Then
                        setUrl(sUrl, sLbl, sSer, srs.Points.Last)
                    End If
                Next

            Next
        End Sub
        Sub ChartPreSettings(sChartType As String)

            '   These in all ChartTypes

            Chart1.ChartAreas(0).Area3DStyle.Enable3D = True
            Chart1.ChartAreas(0).Area3DStyle.Perspective = 90%
            Chart1.ChartAreas(0).Area3DStyle.IsRightAngleAxes = True
            Chart1.ChartAreas(0).Area3DStyle.Inclination = 60
            Chart1.Titles(0).Font = New Font("Arial", 12, FontStyle.Bold)

            Select Case sChartType
                Case "Pie"
                    Chart1.ChartAreas(0).BackColor = Drawing.Color.Transparent
                    For Each Me.srs In Chart1.Series
                        srs.Label = "#VALX" & vbCrLf & "#VALY"
                    Next
                Case "Column"
                    Chart1.ChartAreas(0).Area3DStyle.Inclination = 15
                    Chart1.ChartAreas(0).Area3DStyle.Rotation = -15
                    Chart1.ChartAreas(0).Area3DStyle.IsClustered = True
                    Chart1.ChartAreas(0).AxisX.MajorGrid.LineDashStyle = ChartDashStyle.Dot
                    Chart1.ChartAreas(0).AxisY.MajorGrid.LineDashStyle = ChartDashStyle.Dot
                    'Chart1.ChartAreas(0).AxisX.LabelStyle.Angle = -45
                    Chart1.ChartAreas(0).AxisX.Interval = 1
                    Chart1.Legends(0).Alignment = StringAlignment.Far
                    Chart1.Legends(0).Docking = Docking.Top
                    For Each Me.srs In Chart1.Series
                        srs.IsValueShownAsLabel = True
                    Next

            End Select
        End Sub
        Sub ChartPostSettings(iRepID As Integer, sSub As String)
            Dim qP = From l In xE.Descendants("Style") _
     Where l.Parent.Parent.Attribute("ID").Value = iRepID And l.Parent.Parent.Attribute("SubID").Value = sSub _
           Select New With { _
               .Ent = l.Attribute("Entity").Value, _
               .Mbr = l.Attribute("Member").Value, _
               .Enum = l.Attribute("Enum").Value, _
               .Prop = l.Attribute("Property").Value, _
               .PropType = l.Attribute("PropertyType").Value, _
               .DataType = l.Attribute("DataType").Value, _
               .Val = l.Attribute("Value").Value}
            For Each l In qP
                sEntity = l.Ent
                sMember = l.Mbr
                sEnum = l.Enum
                sProperty = l.Prop
                sPropertyType = l.PropType
                sDataType = l.DataType
                sValue = l.Val
                updateSettings(sEntity, sMember, sProperty, sPropertyType, sDataType, sValue, sEnum)
            Next
        End Sub
#Region "Help The Helpers"
        Private Sub setUrl(sUrl As String, sLbl As String, sSer As String, p As DataPoint)
            Try
                sUrl = cUrls(sLbl & sSer)
                sUrl = sUrl.Replace("~", Request.Url.Scheme & "://" & Request.Url.Authority & Request.ApplicationPath)
                If sTarget = vbNullString Then
                    p.Url = sUrl
                Else
                    p.Url = "javascript:void(0);"
                    sUrl = "onclick = ""window.open('" & sUrl & "','" & sTarget & "');"""
                    p.MapAreaAttributes = sUrl
                End If
            Catch ex As Exception
            End Try
        End Sub
        Sub updateSettings(sEntity As String, sMember As String, sProperty As String, sPropertyType As String, sDataType As String, sValue As String, sEnum As String)
            Dim chrt As Chart = Chart1
            Dim srs As Series
            Dim LGD As New Legend()
            Select Case sEntity
                Case "Chart"
                    Select Case sPropertyType
                        Case "Text"
                        Case "Method"
                            Select Case sMember
                                Case "Titles"
                                    Select Case sProperty
                                        Case "Docking"
                                            Select Case sValue
                                                Case "Top"
                                                    chrt.Titles(CInt(sEnum)).Docking = Docking.Top
                                                Case "Bottom"
                                                    chrt.Titles(CInt(sEnum)).Docking = Docking.Bottom
                                                Case "Left"
                                                    chrt.Titles(CInt(sEnum)).Docking = Docking.Left
                                                Case "Right"
                                                    chrt.Titles(CInt(sEnum)).Docking = Docking.Right
                                            End Select
                                    End Select
                                Case "Width"
                                    Select Case sValue
                                        Case "Window"
                                            If IsNumeric(Request.QueryString("width")) Then chrt.Width = Request.QueryString("width") / 3 + 30
                                        Case Else
                                            If IsNumeric(sValue) Then
                                                chrt.Width = sValue
                                            End If
                                    End Select
                                Case "Height"
                                    Select Case sValue
                                        Case "Window"
                                            '    chrt.Height = hdnheight.Value
                                        Case Else
                                            If IsNumeric(sValue) Then
                                                chrt.Height = sValue
                                            End If
                                    End Select
                                Case "ChartAreas"
                                    Select Case sProperty
                                        Case "Interval"
                                            chrt.ChartAreas(0).AxisX.Interval = sValue
                                        Case "Angle"
                                            Chart1.ChartAreas(0).AxisX.LabelStyle.Angle = CInt(sValue)
                                    End Select
                            End Select
                    End Select
                Case "Series"
                    srs = chrt.Series(CInt(sEnum))
                    Select Case sPropertyType
                        Case "Text"
                            srs = chrt.Series(CInt(sEnum))
                            srs(sProperty) = sValue
                        Case "Method"
                            Select Case sMember
                                Case Else
                                    Select Case sProperty
                                        Case "Url"
                                            srs.Url = sValue
                                    End Select
                            End Select
                    End Select

                Case "Point"
            End Select
            ''   srs = Chart1.Series(CInt(sSubEntityID))
            'If sPropertyType = "Text" Then
            '    srs(sProperty) = sValue
            'Else
            '    Dim typ As Type = srs.GetType()
            '    Dim prop As System.Reflection.PropertyInfo = typ.GetProperty(sProperty)
            '    Dim propType As Type = prop.GetType()

            '    prop.SetValue(srs, [Enum].Parse(propType, sValue), Nothing)

            '    'Dim prop As System.Reflection.PropertyInfo = Chrt.Titles(0).GetType().GetProperty(sProperty)
            '    'Dim propType As Type = prop.PropertyType()
            '    'Dim o As Object = Convert.ChangeType(sValue, propType.GetType())
            '    'prop.SetValue(Chrt.Titles(), o, Nothing)
            'End If

            ''       srs = Chart1.Series(CInt(sSubEntityID))
            'Dim typ As Type = srs.Points(0).GetType()
            'Dim prop As System.Reflection.PropertyInfo = typ.GetProperty(sProperty)
            'Dim propType As Type = prop.GetType()
            'For Each p In srs.Points
            '    If sPropertyType = "Text" Then
            '        p(sProperty) = sValue
            '    Else
            '        prop.SetValue(p, [Enum].Parse(propType, sValue), Nothing)
            '    End If
            'Next

        End Sub
#End Region
#End Region
#End Region
    End Class
End Namespace
