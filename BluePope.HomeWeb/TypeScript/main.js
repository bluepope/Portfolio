jQuery.ajaxSetup({ cache: false }); //ajax 캐시 사용안함
var AjaxCommonError = function (xhr) {
    if (xhr.statusText === "abort")
        return;
    else if (xhr.responseText != null) {
        var message = xhr.responseText;
        var div = document.createElement("div");
        div.innerHTML = message;
        alert(div.innerText);
    }
    else {
        alert(xhr.statusText);
    }
};
//# sourceMappingURL=main.js.map