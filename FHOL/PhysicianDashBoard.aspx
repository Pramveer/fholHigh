<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="PhysicianDashBoard.aspx.cs" Inherits="FHOL.PhysicianDashBoard" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Physician DashBoard</title>
    <script type="text/javascript" src="Js/jquery-1.5.1.min.js" ></script>
    <script type="text/javascript" src="Js/jquery-ui.js" ></script>
    <script type="text/javascript" src="https://netdna.bootstrapcdn.com/bootstrap/3.0.0/js/bootstrap.min.js"></script>

    <script type="text/javascript" src="https://code.highcharts.com/highcharts.js"></script>
    <script type="text/javascript" src="https://code.highcharts.com/highcharts.src.js"></script>
    <script type="text/javascript" src="https://code.highcharts.com/highcharts-more.js"></script>

    <script type="text/javascript" src="Js/customFunctions.js" ></script>
    <link rel="stylesheet" href="https://netdna.bootstrapcdn.com/bootstrap/3.0.0/css/bootstrap.min.css" />
    <link rel="stylesheet" href="Css/physicianDashboard.css" />
</head>
<body>
    <div class="container" style="max-width: 95%;">

         <!-- Physician Dashboard Header -->
        <div class="physicianDashboardHeader">
            
            <!-- Time Filter -->
            <div class="col-md-4 timeFilter">
                Time Filter
            </div>
            
            <!-- prescribed count section -->
            <div class="col-md-2 physicianDash-CountPill">
                <div class="col-md-6 pillCountLabel">Prescribed</div>
                <div class="col-md-6 pillCountValue prescribedValue">186</div>
            </div>

            <!-- Enrolled count section -->
            <div class="col-md-2 physicianDash-CountPill">
                <div class="col-md-6 pillCountLabel">Enrolled</div>
                <div class="col-md-6 pillCountValue enrolledValue">102</div>
            </div>

            <!-- Activated count section -->
            <div class="col-md-2 physicianDash-CountPill">
                <div class="col-md-6 pillCountLabel">Activated</div>
                <div class="col-md-6 pillCountValue activatedValue">58</div>
            </div>

            <!-- Open Alerts count section -->
            <div class="col-md-2 physicianDash-CountPill">
                <div class="col-md-6 pillCountLabel">Open Alerts</div>
                <div class="col-md-6 pillCountValue openAlertsValue">5</div>
            </div>

        </div>

        <!-- Chart Sections -->
        <div class="physicianDash-ChartSection">
            
            <!-- Rx trend and new Active Chart -->
            <div class="col-lg-7 chartContainer">
                <div class="chartTitle">Rx Trend and Activated Patients by Months</div>
                <div class="chartArea" id="rxTrendAndActivatedChart"></div>
            </div>

            <!-- Rx trend and new Active Chart -->
            <div class="col-lg-5 chartContainer">
                <div class="chartTitle">Enrolled Patients Status</div>
                <div class="chartArea" id="enrolledPatientsChart"></div>
            </div>

             <!-- Rx trend and new Active Chart -->
            <div class="col-lg-4 chartContainer">
                <div class="chartTitle">Active Patients</div>
                <div class="chartArea" id="activePatientsChart"></div>
            </div>

            <!-- Rx trend and new Active Chart -->
            <div class="col-lg-4 chartContainer">
                <div class="chartTitle">Patient Compliance (Last 30 days)</div>
                <div class="chartArea" id="patientComplianceChart"></div>
            </div>

            <!-- Rx trend and new Active Chart -->
            <div class="col-lg-4 chartContainer">
                <div class="chartTitle">Comparative Baseline Compliance (Last 30 days)</div>
                <div class="chartArea" id="comparitiveBaselineChart"></div>
            </div>

        </div>
    </div>
</body>
</html>
