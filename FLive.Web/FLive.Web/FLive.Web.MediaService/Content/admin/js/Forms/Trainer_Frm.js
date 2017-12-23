var TrainerManagement = (function () {
    var self = this;
    var map;
    var addressMarker;

    this.searchTrainers = function () {
        console.log('came');
        var searchText = GetSearchTextBoxText('TextBox_Main_SearchBox');
        $('#div_searchresults').html($("#DIV_InnerPageContent_Loading").html());
        $('#div_searchresults').load("/Trainers/SearchTrainerList?searchText=" + searchText, new function () {

        });
    };

    this.findTrainers = function (txt) {
        initFunctionDelayedCall(TrainerManagement.SearchTrainers);
    };

    this.loadAddEditForm = function (id) {
        $('#DIV_PageInner_OperationDetails').html($('#DIV_InnerPageContent_Loading').html());
        $('#DIV_PageInner_OperationDetails').load("/Trainers/AddEditTrainer?id=" + id, new function () {

        });
    };

    this.drawAddressMap = function (lat, lng) {
        var mapCenter = [lat + 0.07, lng];
        var addressLatLng = new google.maps.LatLng(lat, lng);
        var mapOptions = {
            zoom: 10,
            mapTypeControl: false,
            center: new google.maps.LatLng(mapCenter[0], mapCenter[1]),
            styles: [{ "featureType": "administrative", "elementType": "labels.text.fill", "stylers": [{ "color": "#6195a0" }] }, { "featureType": "administrative.province", "elementType": "geometry.stroke", "stylers": [{ "visibility": "off" }] }, { "featureType": "landscape", "elementType": "geometry", "stylers": [{ "lightness": "0" }, { "saturation": "0" }, { "color": "#f5f5f2" }, { "gamma": "1" }] }, { "featureType": "landscape.man_made", "elementType": "all", "stylers": [{ "lightness": "-3" }, { "gamma": "1.00" }] }, { "featureType": "landscape.natural.terrain", "elementType": "all", "stylers": [{ "visibility": "off" }] }, { "featureType": "poi", "elementType": "all", "stylers": [{ "visibility": "off" }] }, { "featureType": "poi.park", "elementType": "geometry.fill", "stylers": [{ "color": "#bae5ce" }, { "visibility": "on" }] }, { "featureType": "road", "elementType": "all", "stylers": [{ "saturation": -100 }, { "lightness": 45 }, { "visibility": "simplified" }] }, { "featureType": "road.highway", "elementType": "all", "stylers": [{ "visibility": "simplified" }] }, { "featureType": "road.highway", "elementType": "geometry.fill", "stylers": [{ "color": "#fac9a9" }, { "visibility": "simplified" }] }, { "featureType": "road.highway", "elementType": "labels.text", "stylers": [{ "color": "#4e4e4e" }] }, { "featureType": "road.arterial", "elementType": "labels.text.fill", "stylers": [{ "color": "#787878" }] }, { "featureType": "road.arterial", "elementType": "labels.icon", "stylers": [{ "visibility": "off" }] }, { "featureType": "transit", "elementType": "all", "stylers": [{ "visibility": "simplified" }] }, { "featureType": "transit.station.airport", "elementType": "labels.icon", "stylers": [{ "hue": "#0a00ff" }, { "saturation": "-77" }, { "gamma": "0.57" }, { "lightness": "0" }] }, { "featureType": "transit.station.rail", "elementType": "labels.text.fill", "stylers": [{ "color": "#43321e" }] }, { "featureType": "transit.station.rail", "elementType": "labels.icon", "stylers": [{ "hue": "#ff6c00" }, { "lightness": "4" }, { "gamma": "0.75" }, { "saturation": "-68" }] }, { "featureType": "water", "elementType": "all", "stylers": [{ "color": "#eaf6f8" }, { "visibility": "on" }] }, { "featureType": "water", "elementType": "geometry.fill", "stylers": [{ "color": "#c7eced" }] }, { "featureType": "water", "elementType": "labels.text.fill", "stylers": [{ "lightness": "-49" }, { "saturation": "-53" }, { "gamma": "0.79" }] }]
        };


        map = new google.maps.Map(document.getElementById("map"), mapOptions);

        if (addressMarker != null)
            addressMarker.setMap(null);

        addressMarker = new google.maps.Marker({
            position: addressLatLng,
            map: map,
            draggable: false
        });

        setTimeout(function () {

            google.maps.event.trigger(TrainerManagement.map, 'resize');
        }, 500);
    };

    this.updateActiveStatus = function () {
        var activeStautsUpdateModel = {
            Id: $("#trainer_id").val(),
            IsActive: $('#Trainer_ActiveStatusChk').is(':checked')
        };

        if (confirm('Are you sure you want to update the status of this trainer ?')) {
            azyncLockPost("../Api/Trainer/UpdateActiveStatus", activeStautsUpdateModel, TrainerManagement.SaveSucsussfull, ConnectionError);
        }
        else {
            $('#Trainer_ActiveStatusChk').prop('checked', !activeStautsUpdateModel.IsActive);
        }
    }

    this.cancelAddEdit = function (id) {
        $('#DIV_PageInner_OperationDetails').html($('#PageInnerSubFormContent').html());
        CourseManagement.ResetHeight();
    };

    this.resetHeight = function () {
        var h = $(window).height();
        var h2 = h - 60;
        var h3 = h - 160;
        $(".main-search-result").css('height', h3 + 'px');
        $(".page-action-container").css('height', h2 + 'px');
    };

    this.saveSucsussfull = function (id) {
        TrainerManagement.SearchTrainers();
        TrainerManagement.LoadAddEditForm($("#trainer_id").val());
        if ($('#Trainer_ActiveStatusChk').is(':checked')) {
            $('#Trainer_active_status').removeClass('label-danger').addClass('label-success').html('Active');
        }
        else {
            $('#Trainer_active_status').removeClass('label-success').addClass('label-danger').html('Inactive');
        }
    };
    
    return {
        SearchTrainers: searchTrainers,
        FindTrainers: findTrainers,
        LoadAddEditForm: loadAddEditForm,
        DrawAddressMap: drawAddressMap,
        CancelAddEdit: cancelAddEdit,
        ResetHeight: resetHeight,
        UpdateActiveStatus: updateActiveStatus,
        SaveSucsussfull: saveSucsussfull
    };

})();
