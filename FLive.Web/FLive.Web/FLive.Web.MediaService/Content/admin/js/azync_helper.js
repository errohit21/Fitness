
// Maintain the lock for the operation.
// will be released after the completion of previous request.
var _systemUpdateOperationLock = false;

function azyncLockPost(URL, dataObj, succesFnc, errorFnc, beforSendFnc) {

    // Check if the lock is on, If its locked then return without 
    // calling the server function.
    if (_systemUpdateOperationLock == true) {
        alert("Current operation is in progress. Please wait.");
        return;
    }

    var accesstoken = sessionStorage.getItem('accessToken');

    var authHeaders = {};
    if (accesstoken) {
        authHeaders.Authorization = 'Bearer ' + accesstoken;
    }
    console.clear();
    console.log(accesstoken);
    // Server call with jQuery
    $.ajax({
        url: URL,
        data: JSON.stringify(dataObj),
        type: 'POST',
        contentType: "application/json; charset=utf-8",
        dataType: 'json',
        headers : authHeaders,
        beforeSend: function (xhr) {
            // Locking the operation to avoid conflicts.
            if (beforSendFnc != undefined)
                beforSendFnc(xhr);
            _systemUpdateOperationLock = true;
            $("#SystemMainAzyncLoadingDiv").css("display", "block");
        },
        statusCode: {
            200: function (result) {
                // Comment by Harinda Dias
                // Releasing the lock for enable other operations.
                _systemUpdateOperationLock = false;
                $("#SystemMainAzyncLoadingDiv").css("display", "none");
                // Calling the sucesess function 
                succesFnc(result);
            },
            401: function (err) {
                // Releasing the lock for enable other operations.
                _systemUpdateOperationLock = false;
                // Calling the function to be handled when an error occured.
                $("#SystemMainAzyncLoadingDiv").css("display", "none");
                ShowAlert('You are not authorized.');
            },
            406: function (err) {
                // Releasing the lock for enable other operations.
                _systemUpdateOperationLock = false;
                // Calling the function to be handled when an error occured.
                $("#SystemMainAzyncLoadingDiv").css("display", "none");
                console.log(err);
                ShowAlert(err.responseText);
            },
            404: function (err) {
                _systemUpdateOperationLock = false;
                $("#SystemMainAzyncLoadingDiv").css("display", "none");
                ShowAlert('Requested resource cannot be found.');
            },
            500: function (xhr, desc, err) {
                // Releasing the lock for enable other operations.
                _systemUpdateOperationLock = false;
                console.log(xhr);
                console.log("Desc: " + desc + "\nErr:" + err);
                // Calling the function to be handled when an error occured.
                $("#SystemMainAzyncLoadingDiv").css("display", "none");
                errorFnc(err, xhr);
            }
            
        }
    });
}

function azyncPost(URL, dataObj, succesFnc, errorFnc, beforSendFnc) {

    // Check if the lock is on, If its locked then return without 
    // calling the server function.
    if (_systemUpdateOperationLock == true) {
        alert("Current operation is in progress. Please wait.");
        return;
    }

    // Server call with jQuery
    $.ajax({
        url: URL,
        data: JSON.stringify(dataObj),
        type: 'POST',
        contentType: 'application/json; charset=utf-8',
        dataType: 'json',
        beforeSend: function () {
            // Locking the operation to avoid conflicts.
            beforSendFnc(xhr);
            _systemUpdateOperationLock = true;
            $("#SystemMainAzyncLoadingDiv").css("display", "block");
        },
        success: function (result) {
            // Comment by Harinda Dias
            // Releasing the lock for enable other operations.
            _systemUpdateOperationLock = false;
            $("#SystemMainAzyncLoadingDiv").css("display", "none");
            // Calling the sucesess function 
            succesFnc(result);
        },
        error: function (xhr, desc, err) {
            // Releasing the lock for enable other operations.
            _systemUpdateOperationLock = false;
            console.log(xhr);
            console.log("Desc: " + desc + "\nErr:" + err);
            $("#SystemMainAzyncLoadingDiv").css("display", "none");
            // Calling the function to be handled when an error occured.
            errorFnc(err);
        }
    });
}

function azyncGet(URL, dataObj, succesFnc, errorFnc, beforSendFnc) {

    // Server call with jQuery
    $.ajax({
        url: URL,
        data: JSON.stringify(dataObj),
        type: 'POST',
        contentType: 'application/json; charset=utf-8',
        dataType: 'json',
        beforeSend: function () {
            $("#SystemMainAzyncLoadingDiv").css("display", "block");
        },
        success: function (result) {
            // Calling the sucesess function 
            $("#SystemMainAzyncLoadingDiv").css("display", "none");
            succesFnc(result);
        },
        error: function (xhr, desc, err) {
            console.log(xhr);
            console.log("Desc: " + desc + "\nErr:" + err);
            $("#SystemMainAzyncLoadingDiv").css("display", "none");
            // Calling the function to be handled when an error occured.
            errorFnc(err);
        }
    });
}


function azyncXMLLockPost(URL, dataObj, succesFnc, errorFnc, beforSendFnc) {

    // Check if the lock is on, If its locked then return without 
    // calling the server function.
    if (_systemUpdateOperationLock == true) {
        alert("Current operation is in progress. Please wait.");
        return;
    }

    // Server call with jQuery

    $.ajax({
        url: URL,
        data: dataObj,
        type: 'POST',
        contentType: 'application/xml; charset=utf-8',
        dataType: 'xml',
        beforeSend: function (xhr) {
            // Locking the operation to avoid conflicts.
            if (beforSendFnc != undefined)
                beforSendFnc(xhr);
            _systemUpdateOperationLock = true;
        },
        success: function (result) {
            // Comment by Harinda Dias
            // Releasing the lock for enable other operations.
            _systemUpdateOperationLock = false;
            // Calling the sucesess function 
            succesFnc(result);
        },
        error: function (xhr, desc, err) {
            // Releasing the lock for enable other operations.
            _systemUpdateOperationLock = false;
            console.log(xhr);
            console.log("Desc: " + desc + "\nErr:" + err);
            // Calling the function to be handled when an error occured.
            errorFnc(err, xhr);
        },
        statusCode: {
            500: function (err) {
                var err = eval("(" + err.responseText + ")");
                bootbox.alert(err.Message, function () { });
            }

        },
    });
}