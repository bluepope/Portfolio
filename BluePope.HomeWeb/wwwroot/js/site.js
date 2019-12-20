var ButtonProgress = /** @class */ (function () {
    function ButtonProgress(target) {
        this._targetObject = target;
        this.InitProgress();
    }
    ButtonProgress.prototype.InitProgress = function () {
        this._initFlag = 0;
        this._progressPercent = 0;
        this._color = window.getComputedStyle(this._targetObject, "").backgroundColor;
        //console.log(this._color);
        if (this._color == "transparent" || this._color == "rgba(0, 0, 0, 0)") {
            this._color = "#007BFF";
        }
        this._targetObject.style.color = "black";
        this._targetObject.style.background = "linear-gradient(to right, " + this._color + " " + this._progressPercent.toString() + "%, transparent 0%)";
        if (this._targetObject.tagName == "BUTTON") {
            this._targetObject.setAttribute("disabled", "true");
        }
    };
    ButtonProgress.prototype.SetProgress = function (percent) {
        this._initFlag = 1;
        if (this._progressPercent != percent) {
            this._progressPercent = percent;
            this._targetObject.style.background = "linear-gradient(to right, " + this._color + " " + this._progressPercent.toString() + "%, transparent 0%)";
            if (this._progressPercent >= 100) {
                this._targetObject.classList.add("blinking");
            }
        }
    };
    ButtonProgress.prototype.EndProgress = function () {
        this._initFlag = 2;
        this._targetObject.style.color = "";
        this._targetObject.style.background = "";
        this._targetObject.classList.remove("blinking");
        if (this._targetObject.tagName == "BUTTON") {
            this._targetObject.removeAttribute("disabled");
        }
    };
    return ButtonProgress;
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
var JQueryAjaxProgress = /** @class */ (function () {
    function JQueryAjaxProgress() {
    }
    JQueryAjaxProgress.prototype.SetButtonSendProgress = function (buttonProgressObject) {
        this._buttonProgressObject = buttonProgressObject;
        this._buttonProgressObject.InitProgress();
    };
    JQueryAjaxProgress.prototype.GetXhr = function () {
        var _this = this;
        var customXhr = $.ajaxSettings.xhr();
        if (customXhr.upload) { // check if upload property exists
            if (this.sendProgressEvent || this._buttonProgressObject) {
                customXhr.upload.addEventListener('progress', function (evt) {
                    if (_this._buttonProgressObject) {
                        if (evt.lengthComputable) {
                            var percentComplete = Math.round((evt.loaded / evt.total) * 100);
                            _this._buttonProgressObject.SetProgress(percentComplete);
                        }
                    }
                    if (_this.sendProgressEvent) {
                        _this.sendProgressEvent(evt);
                    }
                }, false); // for handling the progress of the upload
            }
            if (this.sendAbortEvent) {
                customXhr.upload.addEventListener('abort', this.sendAbortEvent); // for handling the progress of the upload
            }
            if (this.sendCompleteEvent || this._buttonProgressObject) {
                customXhr.upload.addEventListener('load', function (evt) {
                    if (_this._buttonProgressObject) {
                        _this._buttonProgressObject.EndProgress();
                    }
                    if (_this.sendCompleteEvent) {
                        _this.sendCompleteEvent(evt);
                    }
                }); // for handling the progress of the upload
            }
        }
        return customXhr;
    };
    return JQueryAjaxProgress;
}());
//# sourceMappingURL=site.js.map