var RateManagement = (function () {
    var self = this;

    this.saveRates = function () {

        var rate = {
            IroningRate: $("#TextBox_IroningRate").val(),
            WashingRate: $("#TextBox_WashingRate").val()
        };

        if (rate.WashingRate.replace(/\s/g, "") == "" || rate.IroningRate.replace(/\s/g, "") == "") {
            ShowAlert("Please fill all the required fields and try again.");
            return;
        }

        if (!isNumeric(rate.WashingRate) || !isNumeric(rate.IroningRate))
        {
            ShowAlert("Washing rate and ironing rate must be number.");
            return;
        }

        azyncLockPost("../Api/Rate/SaveRate", rate, RateManagement.SaveSucsussfull, ConnectionError);
    };

    this.saveSucsussfull = function (id) {
        ShowAlert("Rates updated Successfully");
    };

    return {
        SaveRates: saveRates,
        SaveSucsussfull: saveSucsussfull,
        ResetHeight: resetHeight
    };

})();
