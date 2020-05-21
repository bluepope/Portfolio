class ButtonProgress {
    constructor(target) {
        this._targetObject = target;
        this.InitProgress();
    }
    InitProgress() {
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
    }
    SetProgress(percent) {
        this._initFlag = 1;
        if (this._progressPercent != percent) {
            this._progressPercent = percent;
            this._targetObject.style.background = "linear-gradient(to right, " + this._color + " " + this._progressPercent.toString() + "%, transparent 0%)";
            if (this._progressPercent >= 100) {
                this._targetObject.classList.add("blinking");
            }
        }
    }
    EndProgress() {
        this._initFlag = 2;
        this._targetObject.style.color = "";
        this._targetObject.style.background = "";
        this._targetObject.classList.remove("blinking");
        if (this._targetObject.tagName == "BUTTON") {
            this._targetObject.removeAttribute("disabled");
        }
    }
}
class DragDrop {
    static SetFileDropZone(cssSelector, callBack) {
        let dropZoneList = document.querySelectorAll(cssSelector);
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
    }
}
class JQueryAjaxProgress {
    constructor() {
    }
    SetButtonSendProgress(buttonProgressObject) {
        this._buttonProgressObject = buttonProgressObject;
        this._buttonProgressObject.InitProgress();
    }
    GetXhr() {
        let customXhr = $.ajaxSettings.xhr();
        if (customXhr.upload) { // check if upload property exists
            if (this.sendProgressEvent || this._buttonProgressObject) {
                customXhr.upload.addEventListener('progress', (evt) => {
                    if (this._buttonProgressObject) {
                        if (evt.lengthComputable) {
                            var percentComplete = Math.round((evt.loaded / evt.total) * 100);
                            this._buttonProgressObject.SetProgress(percentComplete);
                        }
                    }
                    if (this.sendProgressEvent) {
                        this.sendProgressEvent(evt);
                    }
                }, false); // for handling the progress of the upload
            }
            if (this.sendAbortEvent) {
                customXhr.upload.addEventListener('abort', this.sendAbortEvent); // for handling the progress of the upload
            }
            if (this.sendCompleteEvent || this._buttonProgressObject) {
                customXhr.upload.addEventListener('load', (evt) => {
                    if (this._buttonProgressObject) {
                        this._buttonProgressObject.EndProgress();
                    }
                    if (this.sendCompleteEvent) {
                        this.sendCompleteEvent(evt);
                    }
                }); // for handling the progress of the upload
            }
        }
        return customXhr;
    }
}
jQuery.ajaxSetup({ cache: false }); //ajax 캐시 사용안함
var AjaxCommonError = function (xhr) {
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
};
//# sourceMappingURL=site.js.map