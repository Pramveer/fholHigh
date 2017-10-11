/**
 * 
 * @author: Pramveer
 * @date: 09th Oct 2017
 * @desc: file for the custom javascript functions
 */

let userIDGlobal = null;

$(document).ready(function () {
    $('[data-toggle="popover"]').popover();   
    initDatePicker();

    bindApplyEventForDateRange();

    bindPillClickEvent();
    

    // remove the credits from the highcharts.
    if (Highcharts) {
        Highcharts.defaultOptions.credits.enabled = false;
    }

    let dateRange = $('input[name="physician_datePicker"]').val();
    dateRange = dateRange.split(' ');

    let minDate = dateRange[0], maxDate = dateRange[2];

    let params = {
        minDate: minDate,
        maxDate: maxDate,
        userID: getUserName($("#UserName").val()) 
    };
    userIDGlobal = params.userID;   
    getChartsData(params);

});

// initialize the date picker 
let initDatePicker = () => {
    let start = new Date('01/01/2017');
    let end = new Date();

    //initilise date picker
    $('input[name="physician_datePicker"]').daterangepicker({
        startDate: start,
        endDate: end,
        ranges: {
            'Today': [moment(), moment()],
            'Yesterday': [moment().subtract(1, 'days'), moment().subtract(1, 'days')],
            'Last 7 Days': [moment().subtract(6, 'days'), moment()],
            'Last 30 Days': [moment().subtract(29, 'days'), moment()],
            'This Month': [moment().startOf('month'), moment().endOf('month')],
            'Last 3 Month': [moment().subtract(3, 'month').startOf('month'), moment().subtract(3, 'month').endOf('month')]

        }
    });
};

// bind the apply btn click of date range picker
let bindApplyEventForDateRange = () => {
    $('input[name="physician_datePicker"]').on('apply.daterangepicker', function (ev, picker) {

        let params = {
            minDate: picker.startDate.format('YYYY-MM-DD'),
            maxDate: picker.endDate.format('YYYY-MM-DD'),
            userID: userIDGlobal
        };

        changeChartDataAfterDateChange(params);
    });
};

// bind click event for the pills count
let bindPillClickEvent = () => {
    $('.prescribedValue').on('click', function () {
        handleRxAndActiveChartClick(null, true);
    });

    $('.enrolledValue').on('click', function () {
        handleEnrolledChartClick(null, true);
    });

    $('.activatedValue').on('click', function () {
        handleCummilativeChartClick(null, true);
    });
};

// function to get charts data
let getChartsData = (params) => {
    getEnrolledPatientStatusData(params);
    getRxTrendAndActivatedData(params);
    getActivePatientsData(params); 
    getPatientCompliance(params);
    getPatientComplianceComparative();
    getProviderAlerts(params);
};

// function to render the charts after date change
let changeChartDataAfterDateChange = (params) => {
    getEnrolledPatientStatusData(params);
    getRxTrendAndActivatedData(params);
    getActivePatientsData(params);
};

// ajax call to get the enrolled patients data
let getEnrolledPatientStatusData = (params) => {
    let paramStr = prepareParamForChart(params);

    $.ajax({
        type: "POST",
        url: "PhysicianDashBoard.aspx/getEnrolledPatientStatusData",
        data: JSON.stringify({ dataParams: paramStr }),
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (response) {
            chartLoadComplete('enrolledPatientsChart');
            renderEnrolledStatusChart(response);
        }
    });
};

let getUserName = (user) => {
    var guserID = 4222; 
    if (user != "")
    {
        $.ajax({
            type: "POST",
            url: "PhysicianDashBoard.aspx/getUserID",
            data: JSON.stringify({ user: user }),
            async: false,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (response) {
                guserID = response.d; 
            }
        });
    }
    return parseInt(guserID);
};


// ajax call to get the active patients data
let getActivePatientsData = (params) => {
    let paramStr = prepareParamForChart(params);
    $.ajax({
        type: "POST",
        url: "PhysicianDashBoard.aspx/getActivePatientsData",
        data: JSON.stringify({ dataParams: paramStr }),
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (response) {
            chartLoadComplete('activePatientsChart');
            renderActivePatientsChart(response);
        }
    });
};


// ajax call for rx Trend and Activated  data
let getRxTrendAndActivatedData = (params) => {
    let paramStr = prepareParamForChart(params);

    $.ajax({
        type: "POST",
        url: "PhysicianDashBoard.aspx/getRxTrendAndActivatedData",
        data: JSON.stringify({ dataParams: paramStr }),
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (response) {
            chartLoadComplete('rxTrendAndActivatedChart');
            renderRxTrendAndActivatedChart(response);
        }
    });
};

// ajax call to get the patient list data
let getPatientsList = (options, chartName) => {

    $.ajax({
        type: "POST",
        url: "PhysicianDashBoard.aspx/getPatientsListData",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (response) {
            // chartLoadComplete('rxTrendAndActivatedChart');
            renderPatientListOnPopup(response);
        }
    });
};


// function to prepare the param obj for charts
let prepareParamForChart = (paramObj) => {
    return paramObj.userID + "##" + paramObj.minDate + "##" + paramObj.maxDate;
};

// function to render the enrolled status chart
let renderEnrolledStatusChart = (dataObj) => {
    let container = "enrolledPatientsChart";

    dataObj = JSON.parse(dataObj.d);
    dataObj = dataObj[0];
    let allCount = 0;

    if (!dataObj['allCount']) {
        showNoDataFoundSection("enrolledPatientsChart");
        $('.enrolledValue').html(0);
        return false;
    }

    let chartData = [];
   
    for (let keys in dataObj) {
        if (keys !== 'allCount') {
            let obj = {};
            // obj.name = getRefinedKeyNames(keys);
            obj.name = keys;
            obj.y = dataObj[keys];
            
            switch (keys) {
                case "Active Patients":
                    obj.color ="#70ad47";
                    break;
                case "Baseline Progress":
                    obj.color = "#5b9bd5";
                    break;
                case "CEBL":
                    obj.color = "#e96e62";
                    break;
                case "Never Tested":
                    obj.color = "#ffb600";
                    break;
                default:

            }
             
            allCount += dataObj[keys];

            chartData.push(obj);
        }
    }

    Highcharts.chart(container, {
        chart: {
            type: 'pie'
        },
        title: {
            text: ''
        },
        plotOptions: {
            pie: {
                allowPointSelect: true,
                cursor: 'pointer',
                dataLabels: {
                    enabled: true,
                    format: '{point.percentage:.1f} %'
                },
                showInLegend: true
            },
            series: {
                cursor: 'pointer',
                events: {
                    click: function (event) {
                        handleEnrolledChartClick(event.point);
                    }
                }
            }
        },
        series: [{
            name: 'Enrolled Status',
            colorByPoint: true,
            data: chartData
        }]
    });

    // set the totalCount in the HeaderBar
    $('.enrolledValue').html(allCount);


    function getRefinedKeyNames(keyName) {
        let keysData = {
            "bpCount": "BaseLine Progress",
            "neverTestedCount": "Never Tested",
            "isCeblCount": "CEBL",
            "actPCount": "Active Patients"
        };

        return keysData[keyName];
    }
};

// function to render the Active patients charts
let renderActivePatientsChart = (dataObj) => {
    let container = "activePatientsChart";

    dataObj = JSON.parse(dataObj.d);

    if (dataObj.length < 1) {
        showNoDataFoundSection("activePatientsChart");
        $('.activatedValue').html(0);
        return false;
    }

    let categories = [], chartData = [];
    let actCount = dataObj[dataObj.length - 1].pCount;

    for (let i = 0; i < dataObj.length; i++) {
        let currObj = dataObj[i];
        let currMonth = getMonthName(currObj.MonthNo).name;

        categories.push(currMonth);
        chartData.push({ name: currMonth, y: currObj.pCount });

    }

    // render chart
    Highcharts.chart(container, {
        chart: {
            type: 'line'
        },
        title: {
            text: ''
        },
        xAxis: {
            categories: categories
        },
        yAxis: {
            title: {
                text: '# of Patients'
            }
        },
        plotOptions: {
            line: {
                dataLabels: {
                    enabled: true
                }
            },
            series: {
                cursor: 'pointer',
                point: {
                    events: {
                        click: function (event) {
                            handleCummilativeChartClick(event.point);
                        }
                    }
                }
            }

        },
        series: [
            {
                name: 'Active',
                data: chartData
            }
        ]

    });

    $('.activatedValue').html(actCount);
};

// function to render the Rx trend and activated chart
let renderRxTrendAndActivatedChart = (dataObj) => {

    let container = "rxTrendAndActivatedChart";

    dataObj = JSON.parse(dataObj.d);

    if (dataObj.length < 1) {
        showNoDataFoundSection("rxTrendAndActivatedChart");
        $('.prescribedValue').html(0);
        return false;
    }

    let categories = [], rxData = [], activeData = [];
    let rxtotal = 0;

    for (let i = 0; i < dataObj.length; i++) {
        let currObj = dataObj[i];
        let currMonth = getMonthName(currObj.Month).name;

        categories.push(currMonth);
        rxData.push({ name: currMonth, y: currObj.rxCount });
        activeData.push({ name: currMonth, y: currObj.newAct });

        rxtotal += currObj.rxCount;
    }

    // render chart
    Highcharts.chart(container, {
        chart: {
            type: 'line'
        },
        title: {
            text: ''
        },
        xAxis: {
            categories: categories
        },
        yAxis: {
            title: {
                text: '# of Patients'
            }
        },
        plotOptions: {
            line: {
                dataLabels: {
                    enabled: true
                }
            },
            series: {
                cursor: 'pointer',
                point: {
                    events: {
                        click: function (event) {
                            handleRxAndActiveChartClick(event.point);
                        }
                    }
                }
            }
        },
        series: [
            {
                name: 'Rx',
                data: rxData
            },
            {
                name: 'Active',
                data: activeData
            }
        ]

    });

    $('.prescribedValue').html(rxtotal);
};


// function to handle the chart click for the enrolled status chart
let handleEnrolledChartClick = (pointObj, isFromPill) => {

    let paramStr = getbaseFiltersForChartClick();

    if (isFromPill) {
        paramStr += '##' + "NA";
    }
    else {
        // append point name which is clicked
        paramStr += '##' + pointObj.options.name;
    }

    $.ajax({
        type: "POST",
        url: "PhysicianDashBoard.aspx/getPatientListForEnrolledChart",
        data: JSON.stringify({ dataParams: paramStr }),
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (response) {
            renderPatientListOnPopup(response);
        }
    });
};

// function to handle the chart click for the active and rx trend chart
let handleRxAndActiveChartClick = (pointObj, isFromPill) => {
    let paramStr = getbaseFiltersForChartClick();

    if (isFromPill) {
        paramStr += '##' + "Rx";
        paramStr += '##' + "NA";
    }
    else {
        // append point name which is clicked
        paramStr += '##' + pointObj.series.name;

        // append month Id on which is clicked
        paramStr += '##' + getMonthName(null, pointObj.options.name);
    }

    $.ajax({
        type: "POST",
        url: "PhysicianDashBoard.aspx/getPatientListForRxAndActive",
        data: JSON.stringify({ dataParams: paramStr }),
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (response) {
            renderPatientListOnPopup(response);
        }
    });
};

// function to handle the chart click for the active cummilative cahrt
let handleCummilativeChartClick = (pointObj, isFromPill) => {
    let paramStr = getbaseFiltersForChartClick();

    if (isFromPill) {
        paramStr += '##' + "NA";
    }
    else {
        // append month Id on which is clicked
        paramStr += '##' + getMonthName(null, pointObj.options.name);
    }

    $.ajax({
        type: "POST",
        url: "PhysicianDashBoard.aspx/getPatientListForCummilative",
        data: JSON.stringify({ dataParams: paramStr }),
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (response) {
            renderPatientListOnPopup(response);
        }
    });
        
}

// function to get base filters for chart click
let getbaseFiltersForChartClick = () => {
    let finalStr = null;
    let daterrange = $('input[name="physician_datePicker"]').val().split(' ');

    finalStr = userIDGlobal + '##' + daterrange[0] + '##' + daterrange[2];

    return finalStr;
    
};

// function to get the MonthName From Month Id
let getMonthName = (monthId, monName) => {
    let monthsArray = [
        { id: 1, name: 'Jan', fullName: 'January' },
        { id: 2, name: 'Feb', fullName: 'February' },
        { id: 3, name: 'Mar', fullName: 'March' },
        { id: 4, name: 'Apr', fullName: 'April' },
        { id: 5, name: 'May', fullName: 'May' },
        { id: 6, name: 'Jun', fullName: 'June' },
        { id: 7, name: 'July', fullName: 'July' },
        { id: 8, name: 'Aug', fullName: 'August' },
        { id: 9, name: 'Sep', fullName: 'September' },
        { id: 10, name: 'Oct', fullName: 'October' },
        { id: 11, name: 'Nov', fullName: 'November' },
        { id: 12, name: 'Dec', fullName: 'December' }
    ];

    if (monName) {
        return _.where(monthsArray, { name: monName})[0].id;
    }
    else {
        return monthsArray[monthId - 1];
    }
};

// function to toggle the loading for the dataloading of chart
let chartLoadComplete = (chartId) => {
    $('#' + chartId + '-NoData').hide();
    $('#' + chartId + '-Loading').hide();
    $('#' + chartId).show();
};


// function to append values to popup
let renderPatientListOnPopup = (dataList) => {
    let html = ``;

    dataList = JSON.parse(dataList.d);

    if (dataList.length) {
        for (let i = 0; i < dataList.length; i++) {
            let currPat = dataList[i];

            html += `<div class="col-md-12 patientListRow">
                <div class="col-md-4">${currPat.PatientID}</div>
                <div class="col-md-4">${getPatientName(currPat.FirstName, currPat.LastName)}</div>
                <div class="col-md-4">${getFormattedDate(currPat.DateOfBirth)}</div>
                </div>`;
        }
    }
    else {
        html = `<div class="noPatientsFound">No Patients Data Found for Current Selection!</div>`;
    }

    showPopup('Patient Details (N = ' + dataList.length +')', html);
};

// function to open popup 
let showPopup = (title, htmlText) => {
    $("#patientListPopupContent").html(htmlText);

    $("#patientListDialog").dialog({
        title: title,
        width: 650,
        buttons: {
            Ok: function () {
                $(this).dialog('close');
            }
        },
        modal: true
    });
};


// function to open popup 
let showAlertPopup = (title, htmlText) => {
    $("#patientAlertListContent").html(htmlText);

    $("#patientAlertListDialog").dialog({
        title: title,
        width: 650,
        buttons: {
            Ok: function () {
                $(this).dialog('close');
            }
        },
        modal: true
    });
};


// function to open popup 
let showCompliancePopup = (title, htmlText) => {
    $("#patientComplianceListContent").html(htmlText);

    $("#patientComplianceListDialog").dialog({
        title: title,
        width: 650,
        buttons: {
            Ok: function () {
                $(this).dialog('close');
            }
        },
        modal: true
    });
};

// function to get patient name
let getPatientName = (fName, lName) => {
    return fName + ' ' + lName;
};

// function to get formatted date
let getFormattedDate = (date) => {
    date = new Date(date);

    return date.getMonth() + '/' + date.getDate() + '/' + date.getFullYear();
}


// ajax call to get the patient compliance
let getPatientCompliance = (data) => {
    $.ajax({
        type: "POST",
        url: "PhysicianDashBoard.aspx/getPatientCompliance",
        data: JSON.stringify({ userID: data.userID }),
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (response) {
            chartLoadComplete('patientComplianceChart');
            renderPatientCompliance(response);
        }
    });
};

// ajax call to get the patient compliance
let getProviderAlerts = (data) => {
    $.ajax({
        type: "POST",
        url: "PhysicianDashBoard.aspx/getProviderAlerts",
        data: JSON.stringify({ userID: data.userID }),
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (response) {
            //  console.log(response.d);
            $(".openAlertsValue").html(response.d);
        }
    });
};

let ShowOpenAlerts = () => {
    $.ajax({
        type: "POST",
        url: "PhysicianDashBoard.aspx/getProviderAlertsDrillDown",
        data: JSON.stringify({ userID: userIDGlobal }),
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (response) {
           let html = ``;

            dataList = JSON.parse(response.d);

            for (let i = 0; i < dataList.length; i++) {
                let currPat = dataList[i];

                html += `<div class="col-md-12 patientListRow">               
                <div class="col-md-3">${getPatientName(currPat.FirstName, currPat.LastName)}</div>
                <div class="col-md-3">${currPat.DOB}</div>
                <div class="col-md-3">${currPat.StudyName}</div>
                <div class="col-md-3">${getFormattedDate(currPat.AlertDate)}</div>
                </div>`;
            }
            //console.log(html);
            showAlertPopup('Patient Details (N = ' + dataList.length + ')', html); 
        }
    });
} 

// ajax call to get the enrolled patients data
let getPatientComplianceComparative = () => {
    $.ajax({
        type: "POST",
        url: "PhysicianDashBoard.aspx/getPatientComplianceComparative",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (response) {
            chartLoadComplete('comparitiveBaselineChart');
            renderPatientComplianceComparative(response);
        }
    });
};

// function to render the enrolled status chart
let renderPatientCompliance = (dataObj) => {
    let container = "patientComplianceChart";
  //  console.log(dataObj);
    dataObj = JSON.parse(dataObj.d);
   // console.log(dataObj);
 
      let chartData = [];
  
      for (let keys in dataObj) {
          
          let obj = {};
              obj.name = dataObj[keys].Name.toString();
              obj.y = dataObj[keys].Y;
              obj.color = dataObj[keys].Color;  
              obj.yValue = dataObj[keys].yvalue;  
              chartData.push(obj);         
      }
   
      Highcharts.chart(container, {
          chart: {
              type: 'pie'
          },
          title: {
              text: ''
          },
          tooltip: {
              formatter: function () {
                  if (this.point.name =="<8") 
                      return 'Patient counts with <b> &lt;8 </b> tests: <b>' + this.point.yValue + '</b>';
                  else 
                      return 'Patient counts with <b> >=8 </b> tests: <b>' + this.point.yValue + '</b>';
              }  
            //  pointFormat: '{point.name}: <b>{point.yValue}</b>'
             
          },
          plotOptions: {
              pie: {
                  allowPointSelect: true,
                  cursor: 'pointer',
                  dataLabels: {
                      enabled: true,
                      format: '{point.percentage:.1f} %'
                  },
                  showInLegend: true
              },
              series: {
                  cursor: 'pointer',
                  point: {
                      events: {
                          click: function (e) {
                              bindPatientCompliance(userIDGlobal,this.name);
                          }
                      }
                  }
              }             
          },
          series: [{
              name: 'Test Counts',             
              data: chartData
          }]
      });

    // set the totalCount in the HeaderBar

};


// function to render the enrolled status chart
let renderPatientComplianceComparative = (dataObj) => {
    let container = "comparitiveBaselineChart";
    //  console.log(dataObj);
    dataObj = JSON.parse(dataObj.d);
    // console.log(dataObj);

    let chartData = [];

    for (let keys in dataObj) {

        let obj = {};
        obj.name = dataObj[keys].Name.toString();
        obj.y = dataObj[keys].Y;
        obj.color = dataObj[keys].Color;
        obj.yValue = dataObj[keys].yvalue;
        chartData.push(obj);
    }

    Highcharts.chart(container, {
        chart: {
            type: 'pie'
        },
        title: {
            text: ''
        },
        tooltip: {
            formatter: function () {
                if (this.point.name == "<8")
                    return 'Patient counts with <b> &lt;8 </b> tests: <b>' + this.point.yValue + '</b>';
                else
                    return 'Patient counts with <b> >=8 </b> tests: <b>' + this.point.yValue + '</b>';
            }
        },
        plotOptions: {
            pie: {
                allowPointSelect: true,
                cursor: 'pointer',
                dataLabels: {
                    enabled: true,
                    format: '{point.percentage:.1f} %'
                },
                showInLegend: true
            },
            series: {
                cursor: 'pointer',
                point: {
                    events: {
                        click: function (e) {
                            bindPatientComplianceComparative(this.name);
                        }
                    }
                }
            }
        },
        series: [{
            name: 'Test Counts',
            data: chartData
        }]
    });

    // set the totalCount in the HeaderBar

};

let bindPatientCompliance = (dataUserId,pointoption) => {
    $.ajax({
        type: "POST",
        url: "PhysicianDashBoard.aspx/getPatientComplianceDrillDown",
        data: JSON.stringify({ userID: dataUserId, pointtype: pointoption  }),
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (response) {
            let html = ``;

            dataList = JSON.parse(response.d);

            for (let i = 0; i < dataList.length; i++) {
                let currPat = dataList[i];

                html += `<div class="col-md-12 patientListRow">
                <div class="col-md-4">${currPat.Patient}</div>
                <div class="col-md-3">${currPat.DOB}</div>
                <div class="col-md-2">${currPat.testnum}</div>
                <div class="col-md-3">${currPat.TestDate}</div>
                </div>`;
            }
           // console.log(html);
            showCompliancePopup('Patient Details (N = ' + dataList.length + ')', html);
        }
    });

}


let bindPatientComplianceComparative = (pointoption) => {
    $.ajax({
        type: "POST",
        url: "PhysicianDashBoard.aspx/getPatientComplianceComparativeDrillDown",
        data: JSON.stringify({ pointtype: pointoption }),
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (response) {
            let html = ``;

            dataList = JSON.parse(response.d);

            for (let i = 0; i < dataList.length; i++) {
                let currPat = dataList[i];

                html += `<div class="col-md-12 patientListRow">
                <div class="col-md-4">${currPat.Patient}</div>
                <div class="col-md-3">${currPat.DOB}</div>
                <div class="col-md-2">${currPat.testnum}</div>
                <div class="col-md-3">${currPat.TestDate}</div>
                </div>`;
            }
            //console.log(html);
            showCompliancePopup('Patient Details (N = ' + dataList.length + ')', html);
        }
    });

};

// function to append msg for no data found for charts
let showNoDataFoundSection = (baseContainer) => {
    let html = `<div class="noDataFoundForChart">No data found for chart for the selection !</div>`;
    $('#' + baseContainer + '-NoData').html(html);

    $('#' + baseContainer).hide();
    $('#' + baseContainer + '-NoData').show();
};
