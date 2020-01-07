jQuery.ajaxSetup({ cache: false }); //ajax 캐시 사용안함

var AjaxCommonError = function (xhr: XMLHttpRequest) {
    if (xhr.statusText === "abort")
        return;
    else if (xhr.responseText != null) {
        let message = xhr.responseText;
        let div = document.createElement("div");
        div.innerHTML = message;

        alert(div.innerText);
    }
    else {
        alert(xhr.statusText);
    }
}