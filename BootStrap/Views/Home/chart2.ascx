<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="chart2.ascx.cs" Inherits="BootStrap.Views.Home.chart2" %>
<asp:Chart ID="Chart1" runat="server">
            <Series>
                <asp:Series Name="Series1">
                    <Points>
                        <asp:DataPoint AxisLabel="44" YValues="33" />
                        <asp:DataPoint AxisLabel="34" YValues="35" />
                        <asp:DataPoint AxisLabel="7" YValues="12" />
                    </Points>
                </asp:Series>
            </Series>
            <ChartAreas>
                <asp:ChartArea Name="ChartArea1"></asp:ChartArea>
            </ChartAreas>
        </asp:Chart>
<%--<asp:SqlDataSource ID="SqlDataSource1" runat="server" ConnectionString="<%$ ConnectionStrings:masterConnectionString %>" SelectCommand="SELECT
s.servicename area, count(a.[CustomerID]) clientId   	
FROM (
SELECT distinct CustomerId,CustFrameId,MAX(CUstEventDate) OVER(Partition by CustomerId,CustFrameId) Last 
FROM (
SELECT distinct  CustomerId,CustFrameId,CustEventDate,CustEventTypeId
FROM  book10_21.dbo.CustEventList
WHERE CustEventTypeId = 1
UNION ALL
SELECT CustomerId,CustFrameId,CustEventDate,CustEventTypeId
FROM book10_21.dbo.custEventList
where CustEventTypeId = 2) a
) a
LEFT OUTER JOIN  book10_21.dbo.CustEventList b ON a.CustomerID=b.CustomerId AND a.CustFrameID=b.CustFrameID and a.Last=b.CustEventDate
left outer join  book10_21.dbo.Framelist f on f.FrameId=a.CustFrameId
left outer join  book10_21.dbo.customerlist c on c.customerid= a.customerid
left outer join  book10_21.dbo.servicelist s on s.serviceid=f.serviceid
where CustEventtypeId in (1) and a.CustFrameId not in(122) and f.serviceid not in (16,18)
group by s.servicename"></asp:SqlDataSource>--%>
