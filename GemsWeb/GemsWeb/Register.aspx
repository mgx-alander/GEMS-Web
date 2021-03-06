﻿<%@ Page Title="" Language="C#" MasterPageFile="~/Main.Master" AutoEventWireup="true"
    CodeBehind="Register.aspx.cs" Inherits="GemsWeb.Register" %>

<%@ Register Assembly="MSCaptcha" Namespace="MSCaptcha" TagPrefix="cc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphCENTER" runat="server">
    <div style="margin-left: 10%;">
        <h3>
            <a href="#">Event Name:
                <label runat="server" id="lblEventName">
                </label>
            </a>
        </h3>
        <asp:PlaceHolder ID="phRegister" runat="server"></asp:PlaceHolder>
        <table>
            <tr>
                <td>
                    <asp:Label ID="lblFieldName" Width="150px" runat="server" Text="Captcha"></asp:Label>
                    &nbsp; &nbsp; &nbsp;
                </td>
                <td>
                    <cc1:CaptchaControl ID="ccJoin" runat="server" Height="50px" CaptchaBackgroundNoise="Low"
                        Width="180px" CaptchaLength="5" BackColor="White" EnableViewState="False" />
                    <asp:TextBox ID="txtCaptcha" runat="server" MaxLength="5"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ErrorMessage="Verification Required"
                        ControlToValidate="txtCaptcha" Display="Static" Visible="true"> * </asp:RequiredFieldValidator>
                </td>
            </tr>
        </table>
        <br />
        <asp:Button ID="btnSignUp" runat="server" Text="Sign Up Now" OnClick="btnSignUp_Click" />

    </div>
</asp:Content>
