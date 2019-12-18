type EventDataTransfer = Event & { dataTransfer?: DataTransfer };
type FileDropCallBack = (evt: EventDataTransfer, fileList: FileList) => Function;

class DragDrop {

    static SetFileDropZone(cssSelector: string, callBack: FileDropCallBack) {
        let dropZoneList = document.querySelectorAll(cssSelector);

        Array.prototype.slice.call(dropZoneList).forEach(function (dropZone: Element) {
            dropZone.addEventListener('dragover', function (e: EventDataTransfer) {
                e.stopPropagation();
                e.preventDefault();
                e.dataTransfer.dropEffect = 'copy';
            });

            // Get file data on drop
            dropZone.addEventListener('drop', function (e: EventDataTransfer) {
                e.stopPropagation();
                e.preventDefault();

                callBack(e, e.dataTransfer.files);
            });
        });
    }
}
