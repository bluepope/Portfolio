var AjaxCommonError = function (xhr) {
    var titleDiv = "<div class=\"titleerror\">";
    if (xhr.statusText === "abort")
        return;
    else if (xhr.responseType == "json") {
        alert(xhr.responseText);
    }
    else if (xhr.responseText.indexOf(titleDiv) > -1) {
        var message = xhr.responseText.substring(xhr.responseText.indexOf(titleDiv) + titleDiv.length, xhr.responseText.indexOf("</div>"));
        var div = document.createElement("div");
        div.innerHTML = message;
        alert(div.innerText);
    }
    else
        alert(xhr.statusText);
};
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
var Hello = /** @class */ (function () {
    function Hello(name) {
        if (name) {
            this.name = name;
        }
        else {
            this.name = "no data";
        }
    }
    Hello.prototype.sayHello = function () {
        return "Hello, " + this.name;
    };
    return Hello;
}());
var DragDrop = /** @class */ (function () {
    function DragDrop() {
    }
    DragDrop.SetFileDropZone = function (cssSelector, callBack) {
        var dropZoneList = document.querySelectorAll(cssSelector);
        Array.prototype.slice.call(dropZoneList).forEach(function (dropZone) {
            dropZone.addEventListener('dragover', function (e) {
                e.stopPropagation();
                e.preventDefault();
                e.dataTransfer.dropEffect = 'copy';
            });
            // Get file data on drop
            dropZone.addEventListener('drop', function (e) {
                e.stopPropagation();
                e.preventDefault();
                callBack(e, e.dataTransfer.files);
            });
        });
    };
    return DragDrop;
}());
//# sourceMappingURL=site.js.map