var REGEX_EMAIL = '([a-z0-9!#$%&\'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&\'*+/=?^_`{|}~-]+)*@' +
                  '(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)';

function AddWatermark(inputTextboxId, watermark) {
    $('#' + inputTextboxId).blur(function () {
        if ($(this).val().length == 0)
            $(this).val(watermark).addClass('watermark');
    }).focus(function () {
        if ($(this).val() == watermark)
            $(this).val('').removeClass('watermark');
    }).val(watermark).addClass('watermark');
}

function show(dataObj, fn) {
    $("#" + dataObj).slideDown(500, fn);
}

function hide(dataObj, fn) {
    $("#" + dataObj).slideUp(500, fn);
}

function ToggleElement(objId) {
    if (document.getElementById(objId).style.display == 'none')
        show(objId);
    else
        hide(objId);
}

function ConnectionError(err) {

}

function CustomAlert(msg) {
    alert(msg);
}

function BeforeSend(xhr) {
    authToken = getCookie("AUTH_TOKEN");
    xhr.setRequestHeader("Authorization", "JWT " + authToken);
}

function GetDateFromText(txtID) {
    var dateStrArr = $("#" + txtID).val().split("/");
    return new Date(dateStrArr[2], dateStrArr[1] - 1, dateStrArr[0]);
}

function MonthDiff(d1, d2) {
    // Months between years.
    var months = (d2.getFullYear() - d1.getFullYear()) * 12;

    // Months between... months.
    months += d2.getMonth() - d1.getMonth();

    // Subtract one month if b's date is less that a's.
    if (d2.getDate() < d1.getDate()) {
        months--;
    }
    return months;
}

function setCookie(c_name, value, exdays) {
    var exdate = new Date();
    exdate.setDate(exdate.getDate() + exdays);
    var c_value = escape(value) + ((exdays == null) ? "" : "; expires=" + exdate.toUTCString());
    document.cookie = c_name + "=" + c_value;
}

function getCookie(c_name) {
    var c_value = document.cookie;
    var c_start = c_value.indexOf(" " + c_name + "=");
    if (c_start == -1) {
        c_start = c_value.indexOf(c_name + "=");
    }
    if (c_start == -1) {
        c_value = null;
    }
    else {
        c_start = c_value.indexOf("=", c_start) + 1;
        var c_end = c_value.indexOf(";", c_start);
        if (c_end == -1) {
            c_end = c_value.length;
        }
        c_value = unescape(c_value.substring(c_start, c_end));
    }
    return c_value;
}


function GetSearchTextBoxText(txt) {
    var className = $('#' + txt).attr('class');
    if (className.indexOf("watermark") != -1)
        return "";
    else
        return $('#' + txt).val();
}

function ShowAlert(msg) {
    CustomAlert(msg, function () { });
}

function CustomSuccessMessage(msg) {
    CustomAlert(msg, function () { });
}

function KeepMeAlive() {
    setInterval(KeepAliveCheck, 30000);//180000
}

function guid() {
    function s4() {
        return Math.floor((1 + Math.random()) * 0x10000)
          .toString(16)
          .substring(1);
    }
    return s4() + s4() + '-' + s4() + '-' + s4() + '-' +
      s4() + '-' + s4() + s4() + s4();
}

function KeepAliveCheck(parameters) {
    $.ajax({
        url: '../Home/KeepMeAlive',
        type: 'POST',
        contentType: 'application/xml; charset=utf-8',
        dataType: 'xml'
    });
}

function ShowPageAsDialog(pageUrl,title) {
    $.colorbox({ html: '<iframe style="border: 0px; " src="' + pageUrl + '" width="100%" height="100%"></iframe>', title: title, width: '50%', height: '50%', scrolling:false });
}

function isNumeric(n) {
    return !isNaN(parseFloat(n)) && isFinite(n);
}

function ProfileTagSelector(optionsList, inputControlId) {
    $('#' + inputControlId).selectize({
        persist: false,
        maxItems: null,
        valueField: 'email',
        labelField: 'name',
        searchField: ['name', 'email'],
        options: optionsList,
        render: {
            item: function (item, escape) {
                return '<div>' +
                    (item.name ? '<span class="name">' + escape(item.name) + '</span>' : '') +
                    (item.email ? '<span class="email">' + escape(item.email) + '</span>' : '') +
                '</div>';
            },
            option: function (item, escape) {
                var label = item.name || item.email;
                var caption = item.name ? item.email : null;
                return '<div>' +
                    '<span class="label">' + escape(label) + '</span>' +
                    (caption ? '<span class="caption">' + escape(caption) + '</span>' : '') +
                '</div>';
            }
        },
        createFilter: function (input) {
            var match, regex;

            // email@address.com
            regex = new RegExp('^' + REGEX_EMAIL + '$', 'i');
            match = input.match(regex);
            if (match) return !this.options.hasOwnProperty(match[0]);

            // name <email@address.com>
            regex = new RegExp('^([^<]*)\<' + REGEX_EMAIL + '\>$', 'i');
            match = input.match(regex);
            if (match) return !this.options.hasOwnProperty(match[2]);

            return false;
        },
        create: function (input) {
            if ((new RegExp('^' + REGEX_EMAIL + '$', 'i')).test(input)) {
                return { email: input };
            }
            var match = input.match(new RegExp('^([^<]*)\<' + REGEX_EMAIL + '\>$', 'i'));
            if (match) {
                return {
                    email: match[2],
                    name: $.trim(match[1])
                };
            }
            alert('Invalid email address.');
            return false;
        }
    });
}