jQuery.ajaxSetup({ cache: false }); //ajax 캐시 사용안함
var AjaxCommonError = function (xhr) {
    var titleDiv = "<div class=\"titleerror\">";
    if (xhr.statusText === "abort")
        return;
    else if (xhr.responseType == "json") {
        alert(xhr.responseText);
    }
    else if (xhr.responseText != null && xhr.responseText.indexOf(titleDiv) > -1) {
        var message = xhr.responseText.substring(xhr.responseText.indexOf(titleDiv) + titleDiv.length, xhr.responseText.indexOf("</div>"));
        var div = document.createElement("div");
        div.innerHTML = message;
        alert(div.innerText);
    }
    else
        alert(xhr.statusText);
};
//# sourceMappingURL=main.js.map