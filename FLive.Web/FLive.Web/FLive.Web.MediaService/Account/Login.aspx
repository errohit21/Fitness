<%@ Page Title="Log in" Language="C#" MasterPageFile="~/SiteLogin.Master" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="FLive.Web.MediaService.Account.Login" Async="true" %>

<%@ Register Src="~/Account/OpenAuthProviders.ascx" TagPrefix="uc" TagName="OpenAuthProviders" %>

<asp:Content runat="server" ID="BodyContent" ContentPlaceHolderID="MainContent">
    <div class="row" style="height: 400px;">
        <div class="col-md-12 login-back">
            <div class="welcome-msg animated fadeInDown">

                <div class="welcome-heading">
                    Hello there!
                </div>
                <div class="sub">
                    Welcome to Flive, Sign in and start managing your users profiles.
                </div>
            </div>
        </div>
    </div>
    <div class="row">
        <div class="col-md-12">
            <section id="loginForm" style="margin-top: -15px; padding: 25px; width: 600px;">
                <div class="form-horizontal">
                    <table>
                        <tr>
                            <td>
                                <h4 style="font-size: 27px;">Sign in</h4>
                            </td>
                            <td>
                                <div style="color: #b6b7af; font-size: 13px; padding-left: 5px; padding-top: 11px;">
                                    Please enter your username and password to login.
                                </div>
                            </td>
                        </tr>
                    </table>
                    <hr style="margin-top: 5px; margin-bottom: 25px;" />

                    <asp:PlaceHolder runat="server" ID="ErrorMessage" Visible="false">
                        <p class="text-danger">
                            <asp:Literal runat="server" ID="FailureText" />
                        </p>
                    </asp:PlaceHolder>
                    <div class="form-group" style="margin-bottom: 0px; margin-top: 20px;">

                        <div class="col-sm-5">
                            <asp:TextBox runat="server" placeholder="Username" ID="Email" CssClass="login-text" TextMode="Email" />
                            
                        </div>
                        <div class="col-sm-5" style="margin-left: -15px;">
                            <asp:TextBox runat="server" ID="Password" placeholder="Password" TextMode="Password" CssClass="login-text" />
                            
                        </div>
                        <div class="col-md-2" style="margin-left: -15px;">
                            <asp:Button runat="server" OnClick="LogIn" Text="Sign in" CssClass="btn btn-default login-btn" />
                        </div>
                    </div>

                    <div class="form-group" style="margin-top:10px;">
                        <div class="col-sm-12">
                            <table style="width:100%">
                                <tr>
                                    <td style="width:175px;">
                                        <div class="checkbox" style="padding-top: 0px; display: ; width: 175px;">
                                            <asp:CheckBox runat="server" ID="RememberMe" CssClass="ios" />
                                            <div class="login-remember">Remember me?</div>
                                        </div>
                                    </td>
                                    <td style="color: #b6b7af; width:15px;">|</td>
                                    <td>
                                        <div style="color: #b6b7af; font-size: 13px; margin-top: 0px;">
                                Forgot your password?
                    <asp:HyperLink runat="server" ID="ForgotPasswordHyperLink" ViewStateMode="Disabled">Reset now</asp:HyperLink>
                            </div>
                                    </td>
                                </tr>
                            </table>

                            
                        </div>
                    </div>
                </div>
                <p style="display: none;">
                    <asp:HyperLink runat="server" ID="RegisterHyperLink" ViewStateMode="Disabled">Register as a new user</asp:HyperLink>
                </p>

            </section>
        </div>
    </div>
    <div style="display: none">
        <section id="socialLoginForm">
            <uc:OpenAuthProviders runat="server" ID="OpenAuthLogin" />
        </section>
    </div>
    <script type="text/javascript">
        jQuery(function ($) {
            $(".ios").iosCheckbox();
        });
    </script>
</asp:Content>
