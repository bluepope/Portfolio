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
//# sourceMappingURL=JQueryAjaxProgress.js.map