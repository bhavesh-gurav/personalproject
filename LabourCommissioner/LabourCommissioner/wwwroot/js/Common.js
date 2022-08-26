
const ResponseMsgType = {
    success: 1,
    error: 2,
    warning: 3,
    info: 4
}

function ShowDynamicSwalAlert(title, msg) {
    debugger;
    const myArray = msg.split("||");
    msg = myArray[0];
    var type = myArray[1];
    if (msg != null && msg != '') {
        if (type.toString() == ResponseMsgType.success.toString()) {
            swal({
                title: title,
                text: msg,
                type: "info",
                showCancelButtonClass: "btn-primary",
                confirmButtonText: 'OK'
            });
        }
        else if (type.toString() == ResponseMsgType.error.toString()) {
            swal({
                title: title,
                text: msg,
                type: "error",
                showCancelButtonClass: "btn-primary",
                confirmButtonText: 'OK'
            });
        }
        else if (type.toString() == ResponseMsgType.warning.toString()) {
            swal({
                title: title,
                text: msg,
                type: "warning",
                showCancelButtonClass: "btn-primary",
                confirmButtonText: 'OK'
            });
        }
        else if (type.toString() == ResponseMsgType.info.toString()) {
            debugger;
            swal({
                title: title,
                text: msg,
                type: "info",
                showCancelButtonClass: "btn-primary",
                confirmButtonText: 'OK'
            });
        }
    }
}

function LoadSchemePartialView(controllerName, actionName, ServiceId, ApplicationId,isFilled) {
    debugger
    $.ajax({
        type: "GET",
        url: "/" + controllerName + "/" + actionName,
        data: { ServiceId: ServiceId, ApplicationId: ApplicationId, isFilled: isFilled },
        contentType: "application/json; charset=utf-8",
        dataType: "html",
        /*dataType: "html",*/
        /* dataType: 'JSON',*/
        success: function (response) {
            debugger;
            $('#dvTabPartialView').html('');
            $('#dvTabPartialView').html(response);
            $("form").removeData("validator").removeData("unobtrusiveValidation");
            $.validator.unobtrusive.parse($("form"));
        },
        failure: function (response) {
            alert(response.responseText);
        },
        error: function (response) {
            alert(response.responseText);
        }
    });
}


