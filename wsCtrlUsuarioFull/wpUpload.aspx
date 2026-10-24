<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="wpUpload.aspx.cs" Inherits="wsCtrlUsuarioFull.wpUpload" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <input id="oFile" type="file" runat="server" name="oFile" pattern="*.jpg,*.gif,*.png" />
            <br /><br />
            <asp:button id="btnUpload" type="submit" text="Upload" runat="server" OnClick="btnUpload_Click"></asp:button>
            <br /><br />
            <asp:Panel ID="frmConfirmation" Visible="False" Runat="server">
                <asp:Label id="lblUploadResult" Runat="server"></asp:Label>
            </asp:Panel>
            <br /><br />
        </div>
    </form>
</body>
</html>
