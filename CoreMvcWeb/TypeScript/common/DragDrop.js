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
//# sourceMappingURL=DragDrop.js.map