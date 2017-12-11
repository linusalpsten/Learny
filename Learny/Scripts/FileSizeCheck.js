$("input[type=file]").bind('change', function () {
    var _span = $("#fileErrorMessage")[0];
    var _maxFileSize = $("#maxFileSize");
    var _maxFileSizeKB = $("#maxFileSizeKB");
    if ((this.files[0].size / 1024) <= parseInt(_maxFileSizeKB.val())) {
        _span.innerHTML = "";
        $("#submit")[0].disabled = false;
    } else {
        _span.innerHTML = "Filen får inte vara större än "+_maxFileSize.val();
        $("#submit")[0].disabled = true;
    }
});
//$("#fileErrorMessage")[0].innerText = "test";