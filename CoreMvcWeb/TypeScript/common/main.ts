var AjaxCommonError = function (xhr: XMLHttpRequest) {
    let titleDiv = "<div class=\"titleerror\">";

    if (xhr.statusText === "abort")
        return;
    else if (xhr.responseType == "json") {
        alert(xhr.responseText);
    }
    else if (xhr.responseText.indexOf(titleDiv) > -1) {
        let message = xhr.responseText.substring(xhr.responseText.indexOf(titleDiv) + titleDiv.length, xhr.responseText.indexOf("</div>"));
        let div = document.createElement("div");
        div.innerHTML = message;

        alert(div.innerText);
    }
    else
        alert(xhr.statusText);
}

/*
 * $.ajax({
            type: "POST",
            url: "/login/login",
            dataType: "json",
            data: {
                reception_seq: 1
            },
            success: function (data, status, xhr) {
                _d = data;
                alert(data.msg);
            },
            error: function (xhr, textStatus, thrownError) {

            }
        });
*/