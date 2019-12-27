class ButtonProgress {
    private _initFlag: number; 
    private _color: string;
    private _progressPercent: number;
    private _targetObject: HTMLElement;

    constructor(target: HTMLElement) {
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

    SetProgress(percent: number) {
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


