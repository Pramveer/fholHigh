<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="PhysicianDashBoard.aspx.cs" Inherits="FHOL.PhysicianDashBoard" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">

    <title>Physician DashBoard</title>
    <!-- <script type="text/javascript" src="Js/jquery-1.5.1.min.js" ></script> -->
    <script type="text/javascript" src="https://code.jquery.com/jquery-2.2.4.min.js" ></script>

    <script type="text/javascript" src="https://code.jquery.com/ui/1.12.1/jquery-ui.min.js" ></script>
    <script type="text/javascript" src="https://netdna.bootstrapcdn.com/bootstrap/3.0.0/js/bootstrap.min.js"></script>

    <script type="text/javascript" src="Js/highcharts.src.js"></script>
    <script src="https://code.highcharts.com/modules/exporting.js"></script>

    <link rel="stylesheet" href="https://netdna.bootstrapcdn.com/bootstrap/3.0.0/css/bootstrap.min.css" />

    <!-- Date Range Picker (http://www.daterangepicker.com/) --> 
    <!-- <script type="text/javascript" src="//cdn.jsdelivr.net/jquery/1/jquery.min.js"></script> -->
    <script type="text/javascript" src="//cdn.jsdelivr.net/momentjs/latest/moment.min.js"></script>
    <script type="text/javascript" src="//cdn.jsdelivr.net/bootstrap.daterangepicker/2/daterangepicker.js"></script>
    <link rel="stylesheet" type="text/css" href="//cdn.jsdelivr.net/bootstrap.daterangepicker/2/daterangepicker.css" />
    
    <script type="text/javascript" src="Js/customFunctions.js" ></script>    
  
    <link rel="stylesheet" href="https://code.jquery.com/ui/1.12.1/themes/excite-bike/jquery-ui.css" />  
    <link rel="stylesheet" href="Css/physicianDashboard.css" />
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/4.7.0/css/font-awesome.min.css" />
</head>
<body>
    <div class="physicianDashboardContainer">
       
         <!-- Physician Dashboard Header -->
        <div class="physicianDashboardHeader">
            
            <!-- Time Filter -->
            <div class="col-md-4 timeFilter">
                Time Duration
                <input type="text" name="physician_datePicker" class="dateRangeField" value="01/01/2017 - 10/09/2017" />

            </div>
            
            <!-- prescribed count section -->
            <div class="col-md-2 physicianDash-CountPill">
                <div class="col-md-6 pillCountLabel">Prescribed</div>
                <div class="col-md-6 pillCountValue prescribedValue">0</div>
            </div>

            <!-- Enrolled count section -->
            <div class="col-md-2 physicianDash-CountPill">
                <div class="col-md-6 pillCountLabel">Enrolled</div>
                <div class="col-md-6 pillCountValue enrolledValue">0</div>
            </div>

            <!-- Activated count section -->
            <div class="col-md-2 physicianDash-CountPill">
                <div class="col-md-6 pillCountLabel">Activated</div>
                <div class="col-md-6 pillCountValue activatedValue">0</div>
            </div>

            <!-- Open Alerts count section -->
            <div class="col-md-2 physicianDash-CountPill">
                <div class="col-md-8 pillCountLabel">Open Alerts</div>
                <div onclick="ShowOpenAlerts();" class="col-md-6 pillCountValue openAlertsValue" style="cursor:pointer"> </div>
            </div>

        </div>

        <!-- Chart Sections -->
        <div class="physicianDash-ChartSection">
            
            <!-- Rx trend and new Active Chart -->
            <div class="col-lg-7 chartContainer">
                <div class="chartTitle">Rx Trend and Activated Patients by Months</div>
                <div class="loadingSection" id="rxTrendAndActivatedChart-Loading">Loading...</div>
                <div class="chartArea" id="rxTrendAndActivatedChart"></div>
            </div>

            <!-- Rx trend and new Active Chart -->
            <div class="col-lg-5 chartContainer">
                <div class="chartTitle">Enrolled Patients Status</div>
                <div class="loadingSection" id="enrolledPatientsChart-Loading">Loading...</div>
                <div class="chartArea" id="enrolledPatientsChart"></div>
            </div>

             <!-- Rx trend and new Active Chart -->
            <div class="col-lg-4 chartContainer">
                <div class="chartTitle">Active Patients</div>
                <div class="loadingSection" id="activePatientsChart-Loading">Loading...</div>
                <div class="chartArea" id="activePatientsChart"></div>
            </div>

            <!-- Rx trend and new Active Chart -->
            <div class="col-lg-4 chartContainer">
                  
         
                <div class="chartTitle">Patient Compliance (Last 30 days) <a href="#" class="infoicon" title="Patient Compliance" data-toggle="popover" data-trigger="hover" data-content="This shows the rate of Patients who did >=8 test and <8 for last 30 days for the logged in Physician"  data-placement="left"> <i class="fa fa-info-circle"></i></a>  </div>
                <div class="loadingSection" id="patientComplianceChart-Loading">Loading...</div>
                <div class="chartArea" id="patientComplianceChart"></div>
            </div>

            <!-- Rx trend and new Active Chart -->
           <div class="col-lg-4 chartContainer">                
                <div class="chartTitle">Comparative Baseline Compliance (Last 30 days) <a href="#" class="infoicon" title="Comparative Baseline Complaince" data-toggle="popover" data-trigger="hover" data-content="This shows the rate of Patients who did >=8 and <8 for the last 30 days"  data-placement="left"> <i class="fa fa-info-circle"></i></a>  
         </div>
                <div class="loadingSection" id="comparitiveBaselineChart-Loading">Loading...</div>
                <div class="chartArea" id="comparitiveBaselineChart"></div>
            </div>

        </div>

        <!-- Popup Section -->
        <div id="patientListDialog" class="patientListDialog">
            <div class="col-md-12 patientsListHeader">
                <div class="col-md-4">Patient ID</div>
                <div class="col-md-4">Name</div>
                <div class="col-md-4">DOB</div>
            </div>
            <div id="patientListPopupContent" class="patientListContent"></div>
        </div>

         <!-- Popup Section Patient Compliance-->
        <div id="patientComplianceListDialog" class="patientListDialog">
            <div class="col-md-12 patientsListHeader">
                <div class="col-md-4">Patient</div>
                <div class="col-md-3">DOB</div>
                <div class="col-md-2">Counts</div>
                <div class="col-md-3">Last Test Date</div>
            </div>
            <div id="patientComplianceListContent" class="patientListContent"></div>
        </div>

        <!-- Popup Section Patient Compliance-->
        <div id="patientAlertListDialog" class="patientListDialog">
            <div class="col-md-12 patientsListHeader">
                <div class="col-md-3">Patient</div>
                <div class="col-md-3">DOB</div>
                <div class="col-md-3">Study</div>
                <div class="col-md-3">Alert Date</div>
            </div>
            <div id="patientAlertListContent" class="patientListContent"></div>
        </div>

    </div>
</body>
</html>
