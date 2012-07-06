/// <reference path="jquery-1.6.2-vsdoc.js />
$(document).ready(function () {
    var itemlist = $(".AutoComplete");
    for (var i = 0; i < itemlist.length; i++) {
        AutoCompleteRender(itemlist[i].id, null);
    };
    $(".Articles").draggable();
});

function AutoCompleteRender(id, cascadeParams) {
    //Get data from service
    var serviceURL = $("#" + id).attr("ServiceURL");
    $("#" + id).unautocomplete();
    $("#" + id).flushCache();

    var response = $.post($("#" + id).attr("ServiceURL"), cascadeParams, function (data) {
        var data = data.toString().split("|");
        if ($("#" + id).attr("StartsWith") != undefined) {
            $("#" + id).autocomplete(data, { mustMatch: true });
        }
        else if (!$("#" + id).hasClass("MatchNotRequired")) {
            $("#" + id).autocomplete(data, { matchContains: true });
        }
        else {
            $("#" + id).autocomplete(data, { mustMatch: true, matchContains: false });
        }
    });
    response.error(function (jqXHR) {
        ErrorMessage($(jqXHR.responseText).find().prevObject[2].innerHTML);
    });
}


$(".AutoComplete[cascadeLoad]").live('blur', function () {
    AutoCompleteRender($(this).attr("cascadeLoad"), { parentValue: $(this).val() });
});