<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="PhysicianWebPage.aspx.cs" Inherits="FHOL.PhysicianWebPage" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        

        <div><b>Open Alert</b></div> 
        <div><asp:LinkButton ID="lblopenalert" runat="server" OnClick="lblopenalert_Click"></asp:LinkButton> </div>
        <div>         
          <%--  OnPageIndexChanging="OnPageIndexChanging"--%>
            <asp:GridView ID="grdAlerts" runat="server" AutoGenerateColumns="false" 
        PageSize="2" AllowPaging="true">
        <Columns>
            <asp:BoundField DataField="AlertID" HeaderText="Customer Id" ItemStyle-Width="80" />
             
        </Columns>
    </asp:GridView>

        </div>
    </div>
    </form>
</body>
</html>
