function ObjectToFormDataArray(obj, prefix) {
    var formDataList = [];
    var name;

    if (obj == null)
        return formDataList;

    if (Array.isArray(obj)) {
        var idx = 0;

        for (var i = 0; i < obj.length; i++) {
            var item = obj[i];
            var keys = Object.keys(item);

            if (item != null && typeof (item) === "object") {
                for (var k = 0; k < keys.length; k++) {
                    var key = keys[k];

                    if (prefix == null) {
                        name = key + "[" + idx + "]";
                    }
                    else {
                        name = prefix + "[" + idx + "]" + key;
                    }

                    if (Array.isArray(item[key])) {
                        var subList = ObjectToFormDataArray(item[key], prefix);
                        for (var x = 0; x < subList.length; x++) {
                            formDataList.push(subList[x]);
                        }
                    }
                    else {
                        formDataList.push({ key: name, value: item[key] });
                    }
                }
            }
            else {
                if (prefix == null) {
                    name = "";
                }
                else {
                    name = prefix + "[" + idx + "]";
                }
                formDataList.push({ key: name, value: item });
            }
            idx++;
        }
    }
    else {
        var keys = Object.keys(obj);

        for (var i = 0; i < keys.length; i++) {
            var key = keys[i];

            if (prefix == null) {
                name = key;
            }
            else {
                name = prefix + "[" + key + "]";
            }

            if (obj[key] != null) {
                if (Array.isArray(obj[key]) || typeof (obj[key]) === "object") {
                    var subList = ObjectToFormDataArray(obj[key], name);
                    for (var x = 0; x < subList.length; x++) {
                        formDataList.push(subList[x]);
                    }
                }
                else {
                    formDataList.push({ key: name, value: obj[key] });
                }
            }
        }
    }

    return formDataList;
}

$.fn.reset = function () {
    return this.each(function () {
        $(this).replaceWith($(this).val("").clone(true));
    });
};

$.fn.setAddCommas = function () {
    return this.each(function () {
        $(this).focus(function () {
            $(this).attr("before-value", $(this).val());
            $(this).val($.trim($(this).val().replace(/,/g, "")));
        });

        $(this).blur(function () {
            $(this).val(AddCommas($.trim($(this).val())));
        });

        $(this).trigger("blur");
    });
};

$.fn.setDatePicker = function () {
    return this.each(function () {
        if ($(this).is("input") && $(this).parent().hasClass("input-group") === false) {

            var obj = $(this);

            obj.removeClass("date");

            if (obj.hasClass("form-control") === false)
                obj.addClass("form-control");

            obj.wrapAll("<span class=\"input-group date\" />");
            obj.parent().append("<div class=\"input-group-append\"><span class=\"input-group-text\"><i class=\"fa fa-calendar\"></i></span></div>");

            if (obj.hasClass("date-nobtn")) {
                obj.children(".input-group-append").hide();
            }

            obj.datepicker("destroy");
            obj.attr("maxlength", 10);

            obj.parent().datepicker();
        }
        else {
            $(this).datepicker();
            $(this).children("input").attr("maxlength", 10);
        }
    });
};

$.fn.setAjaxFileDownload = function (url, jsonObj) {
    return this.each(function () {
        $(this).off("click").on("click", function () {
            var $obj = $(this);

            $obj.Loading();

            $.fileDownload(url, {
                httpMethod: "POST",
                dataType: "json",
                contentType: "application/json",
                data: jsonObj,
                successCallback: function (url) { //쿠키에 추가할 것 Response.AddHeader("Set-Cookie", "fileDownload=true; path=/");
                    $obj.Restore();
                },
                failCallback: function (html, url) {
                    $obj.Restore();

                    alert("파일이 삭제되었거나 잘못된 접근입니다");
                    //alert(html);
                }
            });
        });
    });
};

$.fn.Loading = function (text) {
    if (typeof (text) === "undefined" || text === null)
        text = "";

    return this.each(function () {
        var obj = $(this);
        //obj.width(obj.width());
        //obj.attr("data-origintext", obj.text());
        obj.attr("disabled", true);
        //obj.html("<i class='fa fa-spinner fa-spin'></i> " + text);
    });
};

$.fn.Restore = function () {
    return this.each(function () {
        var obj = $(this);
        obj.attr("disabled", false);
        //obj.text(obj.attr("data-origintext"));
    });
};

var SwalConfirm = function (text) {
    if (typeof (text) === "undefined" || text === null)
        text = "진행하시겠습니까?";

    return Swal.fire({
        text: text,
        icon: "question",
        showCancelButton: true,
        confirmButtonText: "확인",
        cancelButtonText: "취소",
        reverseButtons: true
    });
};

var SwalConfirmDelete = function (text) {
    if (typeof (text) === "undefined" || text === null)
        text = "삭제하시겠습니까?";

    return Swal.fire({
        text: text,
        icon: "error",
        showCancelButton: true,
        confirmButtonColor: "#d33",
        confirmButtonText: "삭제",
        cancelButtonText: "취소",
        reverseButtons: true
    });
};

var SwalAlert = function (text) {
    return Swal.fire({
        text: text,
        icon: "info"
    });
};

$.summernote.options.lang = "ko-KR";
$.summernote.options.height = 500;

$(document).ready(function () {
    $.fn.modal.Constructor.Default.backdrop = "static";
    $.fn.modal.Constructor.Default.keyboard = false;
    $(window).on('shown.bs.modal', function (e) {
        var $obj = $(e.target);
        $obj.find("input.date").setDatePicker();
        $obj.find('[data-toggle="tooltip"]').tooltip();

        $(window).resize();
    });

    $(".date").setDatePicker();
    $('[data-toggle="tooltip"]').tooltip();
});
