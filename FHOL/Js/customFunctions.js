/**
 * 
 * @author: Pramveer
 * @date: 06th Oct 2017
 * @desc: file for the custom javascript functions
 */
$(document).ready(function () {
    getEnrolledPatientStatusData();
});


// function to handle the click event for the Rx Trend & Active patients Chart
let handleRxTrendActivePatientsClick = (obj) => {
    // alert('Hello World');
    console.log(obj);
};

// function to handle the click event for Active patients chart
let handleActivePatientsClick = (obj) => {

    console.log(obj);
};


let getEnrolledPatientStatusData = () => {
    var aa = 'Test';
    $.ajax({
        type: "POST",
        url: "PhysicianDashBoard.aspx/getEnrolledPatientStatusData",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: OnSuccess
    });

    function OnSuccess(response) {
        console.log(response);
        renderEnrolledStatusChart(response);
    }
}


let renderEnrolledStatusChart = (dataObj) => {
    let container = "enrolledPatientsChart";

    HighCharts.chart({
        chart: {
            type: 'pie'
        },
        title: {
            text: ''
        },
        series: [{
            name: 'Enrolled Status',
            colorByPoint: true,
            data: chartData
        }]
    });
};