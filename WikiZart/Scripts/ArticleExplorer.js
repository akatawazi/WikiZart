/// <reference path="jquery-1.6.2-vsdoc.js />

$(".ViewHistory").live("click", function () {
    $(".HideHistory").show();
    $(".ViewHistory").hide();
    $(".History").show();
});

$(".HideHistory").live("click", function () {
    $(".HideHistory").hide();
    $(".ViewHistory").show();
    $(".History").hide();
});

$("#Search").keyup(function () {
    var postData = { searchTerm: $("#Search").val() };
    $.ajax({
        type: "POST",
        url: "/wiki/search",
        data: postData,
        success: function (data) {
            $(".WikiExplorer").html(data);
        },
        dataType: "html",
        traditional: true,
        error: function (jqXHR) {
            alert($(jqXHR.responseText));
        }
    });
});