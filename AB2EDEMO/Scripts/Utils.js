//<summary>
//The Script file has been made to define various jQuery functions //
//Here, the code for varios actions have been written as various methods of jQuery Framework//
//this code file is conroller specific rather than being a generic class applicable to all the
//</summary>

//<summary>
//The Following is the code to render the datepicker on the date fields on the flat screen (only for ADD/CHANGE mode of operation)
//</summary>
function addDatePicker() {
    var selectedOption = $("#SelectedOption").text();
    if (selectedOption != "Delete" && selectedOption != "Display") {
        $(".datepicker").datepicker({
            changeMonth: true,
            changeYear: true,
            dateFormat: "dd/mm/yy"
        });
    }
}
function appendHeaderInfo(title) {
    if ($("title").html() != title && $("title").html() == "X-Redo Login Page") {
        $("#outer").append("<div>Hello " + $("#userName").text().trim() + " </div>");
        $("#outer").append("<div id=\"ServerTime\"></div>");
        $("#outer").append("<div class=\"rightAlign\"><a href=\"../../Login/Index\" class=\"logout\">Logout</a></div>");
        $.get("/Utils/GetServerTime", function (datetime) { $("#ServerTime").html(datetime); });
        $("Title").html(title);
    }
    else if ($("title").html() != title && $("title").html() != "X-Redo Login Page" && title != "index") {
        $("Title").html(title);
    }
    else if (title == "index") {
        $("Title").html("X-Redo Login Page");
        $("#outer").html("");
        $("#outer").append("<div class=\"companysiteLinkContainer\"><a id=\"companysite\" target=\"_blank\" href=\"http://www.databorough.com\" ></a></div>");
    }
}

//<summary>
//The following code is written to fetch
//</summary>
function loadPopupValue(str,contr,cls) {
    gifLoad();
    var selectedValCl = "."+cls+"_" + str.trim() + "_txt";
    var selectedVal = $(selectedValCl).val();
    $.get("/"+contr+"/GetPopupData", { keyValue: selectedVal, operationType: str }, function (data) {
            var arr = data.split(',');
            $(selectedValCl).val(arr[0]);
            $("#" + str.toUpperCase() + "txtbox").val(arr[1]);        
    });
    gifUnload();
}

function gifLoad() {
    $("#ExternalDiv").show();
    $("#InternalDiv").fadeIn("slow");

}
function gifUnload() {
    $("#InternalDiv").fadeOut("slow");
    $("#ExternalDiv").hide();
}


$(document).ready(function () {
    window.setInterval(function () {
        $.get("/Utils/GetServerTime", function (datetime) { $("#ServerTime").html(datetime); });
    }, 60000);
});


/// <summary>
///   Method to apply some properties according to screen mode.
/// </summary>
function SetOperationType() {
    var selectedOption = $("#SelectedOption").text();
    if (selectedOption == "Display") {
        $("#Save").attr('style', 'visibility:hidden');
        readonlyFields();
    }
    else if (selectedOption == "Delete") {
        $("#Save").attr('Value', 'Delete');
        readonlyFields();
    }
    //else if (selectedOption == "Edit") {
    //    $(".CustomerDetailMaintenanceEntryPanel_customer_txt").attr('readonly', 'readonly');
    //    $(".CustomerDetailMaintenanceEntryPanel_customer_txt").addClass(' readonlyinp');
    //}
    $('input').each(function () {
        this.value = this.value.replace(/^[ \t]+|[ \t]+$/, '');
    });
    $("input:text:visible:first").focus();
}


//Code for achieving dropdown llike functionality at the panel screen
function loadPopup(imgId, className, methodName) {
    gifLoad();
    $('#popupdata').removeClass().addClass(className);
    $.get(methodName, function (data) {
        $('#popupdata').html(data);
        $('#popupdata').slideDown(600);
        $('#popup').show(); gifUnload();
    });
}

function ChkBoxChecked(e) {
    $('input[type="checkbox"]').removeAttr("checked");
    e.checked = true;
}


function POPUPSelect(obj, value) {
    $('input[type="checkbox"]').each(function () {
        if (this.checked == true) {
            var temp = "updtGridgrid" + value.toString().trim() + this.name;
            $("#" + ($(obj).attr("Id").substring(0, $(obj).attr("Id").indexOf("Select")).toUpperCase()) + "txtbox").val($("#" + temp).val().trim());
            temp = "updtGridgrid" + ($(obj).attr("Id").substring(0, $(obj).attr("Id").indexOf("Select")).toLowerCase()) + this.name;
            $("#updt_" + ($(obj).attr("Id").substring(0, $(obj).attr("Id").indexOf("Select")).toLowerCase())).val($("#" + temp).val());
        }
    });
    $('#popup').hide();
    $('#popupdata').html("");
    $('#popupdata').hide();
}


function POPUPCancel() {
    $('#popup').hide();
    $('#popupdata').html("");
    $('#popupdata').hide();
}

function nextGifclick() {
    gifLoad();
    var cp = parseInt($("#currentPosition").text());
    var p2 = parseInt($("#position2").text());
    var page = $("#popupPage").text().trim();
    $.get(('/' + page.split('/')[0].toString() + '/' + page.split('/')[1].toString() + '/?currntposition=' + cp.toString().trim() + '&position2=' + p2.toString().trim()), function (data) {
        $('#popupdata').html(data); gifUnload();
    });

}
function previousGifclick() {
    gifLoad();
    var cp = parseInt($("#currentPosition").text()) - 20;
    var p2 = parseInt($("#position2").text());
    var page = $("#popupPage").text().trim();
    $.get(('/' + page.split('/')[0].toString() + '/' + page.split('/')[1].toString() + '/?currntposition=' + cp.toString().trim() + '&position2=' + p2.toString().trim()), function (data) {
        $('#popupdata').html(data); gifUnload();
    });

}