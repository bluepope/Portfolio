class JQueryAjaxProgress {

    public sendProgressEvent: EventListener;
    public sendAbortEvent: EventListener;
    public sendCompleteEvent: EventListener;
    public loadProgressEvent: EventListener;
    public loadCompleteEvent: EventListener;

    private _buttonProgressObject: ButtonProgress;

    constructor() {
    }

    SetButtonSendProgress(buttonProgressObject: ButtonProgress) {
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
