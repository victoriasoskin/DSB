<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="grf.aspx.cs" Inherits="grf.grf" %>

<%@ Register Assembly="System.Web.DataVisualization, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" Namespace="System.Web.UI.DataVisualization.Charting" TagPrefix="asp" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
     <script src="/Scripts/Services.js"></script>
</head>
<body>
    <form id="form1" runat="server" >
        <div>
            <asp:Chart ID="Chart1" runat="server"  RenderType="ImageTag">
            </asp:Chart>
        </div>
    </form>
</body>
</html>
