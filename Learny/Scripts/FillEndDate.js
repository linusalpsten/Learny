$("#StartDate").change(function () {
    $("#EndDate").val($("#StartDate").val());
    console.log("test")
});