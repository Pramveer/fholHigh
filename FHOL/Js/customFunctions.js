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
        userID: "4222"
    };
    userIDGlobal = params.userID;   
    getChartsData(params);

});

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

let bindApplyEventForDateRange = () => {
    $('input[name="physician_datePicker"]').on('apply.daterangepicker', function (ev, picker) {
        alert('date changed');
        console.log(picker.startDate.format('YYYY-MM-DD'));
        console.log(picker.endDate.format('YYYY-MM-DD'));

        let params = {
            minDate: picker.startDate.format('YYYY-MM-DD'),
            maxDate: picker.endDate.format('YYYY-MM-DD'),
            userID: userIDGlobal
        };

        changeChartDataAfterDateChange(params);
    });
};


let getChartsData = (params) => {
    getEnrolledPatientStatusData(params);
    getRxTrendAndActivatedData(params);
    getActivePatientsData(params); 
    getPatientCompliance(params);
    getPatientComplianceComparative();
    getProviderAlerts(params);
};

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
let getPatientsList = (options) => {

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

let prepareParamForChart = (paramObj) => {
    return paramObj.userID + "##" + paramObj.minDate + "##" + paramObj.maxDate;;
};

// function to render the enrolled status chart
let renderEnrolledStatusChart = (dataObj) => {
    let container = "enrolledPatientsChart";

    dataObj = JSON.parse(dataObj.d);
    dataObj = dataObj[0];
    let allCount = 0;

    let chartData = [];

    for (let keys in dataObj) {
        if (keys !== 'allCount') {
            let obj = {};
            // obj.name = getRefinedKeyNames(keys);
            obj.name = keys;
            obj.y = dataObj[keys];

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
                        console.log(event);
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
                        click: function () {
                            handleActivePatsChartClick(this);
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
                        click: function () {
                            handleRxAndActivePatsChartClick(this);
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


// function to get the MonthName From Month Id
let getMonthName = (monthId) => {
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

    return monthsArray[monthId - 1];
};

// function to toggle the loading for the dataloading of chart
let chartLoadComplete = (chartId) => {
    $('#' + chartId + '-Loading').hide();
    $('#' + chartId).show();
};

// function to handle the click on the active patients chart
let handleActivePatsChartClick = (pointObj) => {
    getPatientsList();
};

// function to handle the click on the rx & active patients chart
let handleRxAndActivePatsChartClick = (pointObj) => {
    getPatientsList();
};

// function to append values to popup
let renderPatientListOnPopup = (dataList) => {
    let html = ``;

    dataList = JSON.parse(dataList.d);

    for (let i = 0; i < dataList.length; i++) {
        let currPat = dataList[i];

        html += `<div class="col-md-12 patientListRow">
                <div class="col-md-4">${currPat.PatientID}</div>
                <div class="col-md-4">${getPatientName(currPat.FirstName, currPat.LastName)}</div>
                <div class="col-md-4">${getFormattedDate(currPat.DateOfBirth)}</div>
                </div>`;
    }

    showPopup('Patient Details', html);
};

// function to open popup 
let showPopup = (title, htmlText) => {
    $(".#patientListPopupContent").html(htmlText);

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
            showAlertPopup('Patient Details', html); 
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
            showCompliancePopup('Patient Details', html);
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
            showCompliancePopup('Patient Details', html);
        }
    });

}
