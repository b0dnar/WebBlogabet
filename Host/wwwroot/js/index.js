document.getElementById("percent").value = getSavedValue("percent");    // set the value to this input
document.getElementById("countBet").value = getSavedValue("countBet");   // set the value to this input
document.getElementById("percentColor").value = getSavedValue("percentColor");    // set the value to this input
document.getElementById("countBetColor").value = getSavedValue("countBetColor");   // set the value to this input
/* Here you can add more inputs to set value. if it's saved */

//Save the value function - save it to localStorage as (ID, VALUE)
function saveValue(e) {
    var id = e.id;  // get the sender's id to save it . 
    var val = e.value; // get the value. 
    localStorage.setItem(id, val);// Every time user writing something, the localStorage's value will override . 
}

function removeValue(e) {
    if (e == "filter") {
        localStorage.setItem("percent", "");
        localStorage.setItem("countBet", "");
    } else if (e == "filterColor") {
        localStorage.setItem("percentColor", "");
        localStorage.setItem("countBetColor", "");
    }
}

//get the saved value function - return the value of "v" from localStorage. 
function getSavedValue(v) {
    if (!localStorage.getItem(v)) {
        return "";// You can change this to your defualt value. 
    }
    return localStorage.getItem(v);
}


$("#button_filter").click(function () {
    var data0 = { Percent: $("#percent").val(), CountBet: $("#countBet").val() };

    var json = JSON.stringify(data0);

    $.ajax({
        type: "POST",
        url: "/api/forecast/filter",
        data: json,
        contentType: "application/json; charset=utf-8",
        dataType: "json"
    });
});

$("#button_filter_color").click(function () {
    localStorage.setItem("percentColor", document.getElementById("percentColor").value);
    localStorage.setItem("countBetColor", document.getElementById("countBetColor").value);
});

$("#button_no_filter").click(function () {
    $.get("/api/forecast/nofilter");
    removeValue("filter");
});

$("#button_no_filter_color").click(function () {
    removeValue("filterColor");
});

setInterval(function () {
    $.get("/api/forecast/update", function (data) {
        var forecastObj = JSON.parse(data);
        var table_body = '<table align="center"  border="1" id="example"><thead><tr align="center" height="20"><th width="150" >Время</th><th width="90" >Процент %</th><th width="60" >Cтавки</th><th width="450" >Информация</th></tr></thead><tbody>';

        var sortList = getSortList(forecastObj);

        for (var i = 0; i < sortList.length; i++) {
            table_body += '<tr align="center" height="30" bgcolor="' + sortList[i].Color + '">';

            table_body += '<td <p>' + sortList[i].TimeBet + '</p> </td>';
            table_body += '<td <p>' + sortList[i].Percent + '</p> </td>';
            table_body += '<td <p>' + sortList[i].CountBet + '</p> </td>';
            table_body += '<td <p>' + sortList[i].HtmlPayload + '</p> </td>';
        }

        table_body += '</tbody></table>';
        $('#forecastId').html(table_body);
    });
}, 7000);

function getSortList(list) {
    var myList = [];

    var colorPercent = parseInt(document.getElementById("percentColor").value);
    var colorCountBet = parseInt(document.getElementById("countBetColor").value);

    for (var i = 0; i < list.length; i++) {
        var myObj = {};

        myObj.TimeBet = list[i].TimeBet.slice(0, 16).replace('T', ' ');
        myObj.Percent = list[i].Percent;
        myObj.CountBet = list[i].CountBet;
        myObj.HtmlPayload = list[i].HtmlPayload;

        if (colorPercent != NaN && colorCountBet != NaN && colorPercent <= parseInt(myObj.Percent) && colorCountBet <= parseInt(myObj.CountBet)) {
            myObj.Color = "yellow";
        } else if (colorPercent != NaN && colorCountBet == NaN && colorPercent <= parseInt(myObj.Percent)) {
            myObj.Color = "yellow";
        } else if (colorPercent == NaN && colorCountBet != NaN && colorCountBet <= parseInt(myObj.CountBet)) {
            myObj.Color = "yellow";
        } else {
            myObj.Color = "white";
        }        

        myList.push(myObj);
    }

    return myList;
}