jQuery.ajaxSetup({ cache: false }); //ajax 캐시 사용안함

var AjaxCommonError = function (xhr: XMLHttpRequest) {
    let titleDiv = "<div class=\"titleerror\">";

    if (xhr.statusText === "abort")
        return;
    else if (xhr.responseType == "json") {
        alert(xhr.responseText);
    }
    else if (xhr.responseText != null && xhr.responseText.indexOf(titleDiv) > -1) {
        let message = xhr.responseText.substring(xhr.responseText.indexOf(titleDiv) + titleDiv.length, xhr.responseText.indexOf("</div>"));
        let div = document.createElement("div");
        div.innerHTML = message;

        alert(div.innerText);
    }
    else
        alert(xhr.statusText);
}