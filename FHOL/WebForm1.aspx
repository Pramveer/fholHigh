<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="WebForm1.aspx.cs" Inherits="FHOL.WebForm1" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <style type="text/css">
        .auto-style1 {
            width: 202px;
            font-size: small;
        }

        .pc-table-title {
            background-color: #f3f3f3;
            width: calc(100% - 12px);
            float: left;
            padding: 6px;
            font-size: 13px;
        }

        .pc-table-content {
            text-align: center;
            padding-top: 20px;
            padding: 10px 0;
            float: left;
            width: 100%;
        }

        .pc-table tr td {
            border: 1px solid #ccc;
        }

        .widthof33 {
            width: calc(33% - 13px);
            float: left;
            margin-right: 20px;
        }

            .widthof33:last-child {
                margin: 0;
            }

        .fullwidth {
            width: 100%;
            float: left;
            margin-bottom: 20px;
        }

        .widthof40 {
            width: 40%;
            float: left;
        }

        .widthof60 {
            width: 60%;
            float: left;
        }

        .widthof100 {
            width: 100%;
            float: left;
            margin-bottom: 15px;
        }

        .widthof50 {
            width: calc(50% - 10px);
            float: left;
            margin-right: 20px;
        }

            .widthof50:last-child {
                margin: 0;
            }

        .patient_Continuum_Shapshot_list {
            width: auto;
            float: right;
            padding: 0;
            margin: 0;
        }

            .patient_Continuum_Shapshot_list li {
                display: inline-block;
                float: left;
                width: auto;
                font-size: 20px;
                margin-right: 20px;
            }

        .pcsl_title {
            float: left;
            width: auto;
            padding: 15px 30px;
            background: #f2f2f2;
        }

        .pcsl_content {
            float: left;
            width: auto;
            padding: 15px 30px;
        }

        .pscl_yellow {
            background: #ffb600;
        }

        .pscl_red {
            background: #e96e62;
        }

        .pscl_blue {
            background: #5b9bd5;
        }

        .pscl_green {
            background: #70ad47;
        }

        .patient_Continuum_Shapshot_title {
            width: auto;
            float: left;
            font-size: 22px;
            padding: 10px 0;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <div class="fullwidth">
                <div class="patient_Continuum_Shapshot_title">
                    <b>Time Duration</b>
                    <asp:DropDownList ID="t" runat="server" Width="150px" Height="25px">
                        <asp:ListItem Text="2017"  Value="2017"  />
                      
                    </asp:DropDownList>
                </div>
                <ul class="patient_Continuum_Shapshot_list">
                    <li class="">
                        <div class="pcsl_title">Prescribed</div>
                        <div class="pcsl_content pscl_yellow">302</div>
                    </li>
                    <li class="">
                        <div class="pcsl_title">Enrolled</div>
                        <div class="pcsl_content pscl_red">302</div>
                    </li>
                    <li class="">
                        <div class="pcsl_title">Activated</div>
                        <div class="pcsl_content pscl_blue">302</div>
                    </li>
                    <li class="">
                        <div class="pcsl_title">Open Alerts</div>
                        <div class="pcsl_content pscl_green">4</div>
                    </li>
                </ul>
            </div>

            <div class="fullwidth">
                <div class="widthof50">
                    <table class="pc-table" style="table-layout: fixed; border-spacing: inherit; width: 100%">
                        <thead>
                            <tr>
                                <th style="background-color: #f0f0f0; padding: 10px"><b> Rx Trend and Activated Patient by Month</b></th>
                            </tr>
                        </thead>
                        <tbody style="border: 1px solid #ccc; border-right: none;">
                            <tr>
                                <td>
                                    <img src="Trend.png" style="width: 100%; height: 280px;" />
                                </td>
                            </tr>
                        </tbody>
                    </table>
                </div>
                <div class="widthof50">
                     <table class="pc-table" style="table-layout: fixed; border-spacing: inherit; width: 100%">
                        <thead>
                            <tr>
                                <th style="background-color: #f0f0f0; padding: 10px"><b>Enrolled Patient Status</b></th>
                            </tr>
                        </thead>
                        <tbody style="border: 1px solid #ccc; border-right: none;">
                            <tr>
                                <td>
                                    <img src="enrolled.png" style="width: 100%; height: 272px;" />
                                </td>
                            </tr>
                        </tbody>
                    </table>
                   
                </div>
            </div>
            <div class="fullwidth">
                <div class="widthof33">
                 <table class="pc-table" style="table-layout: fixed; border-spacing: inherit; width: 100%">
                        <thead>
                            <tr>
                                <th style="background-color: #f0f0f0; padding: 10px"><b>Active Patients</b></th>
                            </tr>
                        </thead>
                        <tbody style="border: 1px solid #ccc; border-right: none;">
                            <tr>
                                <td>
                                    <img src="activepatients.png" style="width: 100%; height: 280px;" />
                                </td>
                            </tr>
                        </tbody>
                    </table>

                </div>
                <div class="widthof33">
                    <table class="pc-table" style="table-layout: fixed; border-spacing: inherit; width: 100%">
                        <thead>
                            <tr>
                                <th style="background-color: #f0f0f0; padding: 10px"><b>Patient Compliance<em> (Last 30 days)</em></b></th>
                            </tr>
                        </thead>
                        <tbody style="border: 1px solid #ccc; border-right: none;">
                            <tr>
                                <td>
                                    <img src="tests.png" style="width: 100%; height: 272px;" />
                                </td>
                            </tr>
                        </tbody>
                    </table>

                </div>
                <div class="widthof33">
                    <table class="pc-table" style="table-layout: fixed; border-spacing: inherit; width: 100%">
                        <thead>
                            <tr>
                                <th style="background-color: #f0f0f0; padding: 10px"><b>Comparative Baseline Patient Complaince<em> (Last 30 days)</em></b></th>
                            </tr>
                        </thead>
                        <tbody style="border: 1px solid #ccc; border-right: none;">
                            <tr>
                                <td>
                                    <img src="tests.png" style="width: 100%; height: 272px;" />
                                </td>
                            </tr>
                        </tbody>
                    </table>
                </div>
            </div>
        </div>
    </form>
</body>
</html>
