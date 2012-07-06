setup_wmd({
    input: "wmdMarkup",
    button_bar: "Body-button-bar",
    preview: "Body-preview"
});
$("#SubmitButton").live("click", function(){
    $("#Body").val($("#Body-preview").html());
});