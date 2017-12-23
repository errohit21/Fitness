var OrderManagement = (function () {
    var self = this;
    var map;
    var addressMarker;

    this.searchOrders = function () {
        var searchText = GetSearchTextBoxText('TextBox_Main_SearchBox');
        $('#div_searchresults').html($("#DIV_InnerPageContent_Loading").html());
        $('#div_searchresults').load("/Order/SearchOrderList?searchText=" + searchText +
            "&filterPending=" + $('#chk_seachorder_onlypendingapprovals').is(':checked'), new function () {

            });
    };

    this.advanceSearchOrders = function () {
        var searchText = GetSearchTextBoxText('TextBox_Main_SearchBox');
        var searchModel = {
            SearchText: GetSearchTextBoxText('TextBox_Main_SearchBox'),
            Customer: $("#TextBox_Main_SearchCustomer").val(),
            Supplier: $("#TextBox_Main_SearchSupplier").val(),
            OrderStatus: $("#DDL_Main_SearchStatus").val(),
            FilterPending: $('#chk_seachorder_onlypendingapprovals').is(':checked')
        };

        $('#div_searchresults').load("/Order/AdvanceSearchOrderList", searchModel);
    };

    this.findOrders = function (txt) {
        initFunctionDelayedCall(OrderManagement.SearchOrders);
    };

    this.loadAddEditForm = function (id) {
        $('#DIV_PageInner_OperationDetails').html($('#DIV_InnerPageContent_Loading').html());
        $('#DIV_PageInner_OperationDetails').load("/Order/AddEditOrder?id=" + id, new function () {

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
            google.maps.event.trigger(SupplierManagement.map, 'resize');
        }, 500);
    };

    this.updateActiveStatus = function () {
        var activeStautsUpdateModel = {
            Id: $("#supplier_id").val(),
            IsActive: $('#Supplier_ActiveStatusChk').is(':checked')
        };

        azyncLockPost("../Api/Supplier/UpdateActiveStatus", activeStautsUpdateModel, SupplierManagement.SaveSucsussfull, ConnectionError);
    }

    this.cancelAddEdit = function (id) {
        $('#DIV_PageInner_OperationDetails').html($('#PageInnerSubFormContent').html());
        CourseManagement.ResetHeight();
    };

    this.resetHeight = function () {
        var h = $(window).height();
        var h2 = h - 60;
        var h3 = h - 195;
        $(".main-search-result").css('height', h3 + 'px');
        $(".page-action-container").css('height', h2 + 'px');
    };

    this.saveAddEditCourse = function () {

        var course = {
            Id: $("#Course_Id").val(),
            Code: $("#TextBox_Course_CourseCode").val(),
            Name: $("#TextBox_Course_CourseName").val(),
            Description: $("#TextBox_Course_Description").val(),
            NoOfSessions: $("#TextBox_Course_NoSessions").val(),
            MaxNoOfStudents: $("#TextBox_Course_MaxParticipants").val(),
            LogoUrl: $("#TextBox_Course_LogoUrl").val(),
            BadgeUrl: $("#TextBox_Course_BadgeUrl").val(),
            CompletionType: $("#DDL_Course_CompletionType").val(),
            OwnerId: $("#DDL_Course_Owner").val(),
            UserName: $("#System_Logged_UserName").val()
        };

        if (course.Code.replace(/\s/g, "") == "" || course.Name.replace(/\s/g, "") == "" ||
            course.NoOfSessions.replace(/\s/g, "") == "") {
            ShowAlert("Please fill all the required fields and try again.");
            return;
        }


        azyncLockPost("../Api/Course/SaveCourse", course, CourseManagement.SaveSucsussfull, ConnectionError);
    };

    this.saveSucsussfull = function (id) {
        SupplierManagement.SearchSuppliers();
        if ($('#Supplier_ActiveStatusChk').is(':checked')) {
            $('#Supplier_active_status').removeClass('label-danger').addClass('label-success').html('Active');
        }
        else {
            $('#Supplier_active_status').removeClass('label-success').addClass('label-danger').html('Inactive');
        }
    };

    this.approveSupplier = function () {
        if (confirm('Are you sure you want to approve this supplier?')) {
            var activeStautsUpdateModel = {
                Id: $("#supplier_id").val()
            };

            azyncLockPost("../Api/Supplier/ApproveSupplier", activeStautsUpdateModel, SupplierManagement.ApproveSucsussfull, ConnectionError);
        }
    }

    this.approveSucsussfull = function (id) {
        SupplierManagement.SearchSuppliers();
        SupplierManagement.LoadAddEditForm($("#supplier_id").val());
    };

    this.loadOrderList = function (username) {
        var searchModel = {
            SupplierUsername: username
        };

        azyncLockPost("../Api/Supplier/GetSupplierOrders", searchModel, SupplierManagement.LoadOrdersSucsussfull, ConnectionError);
    }

    this.loadOrdersSucsussfull = function (d) {
        $('#Supplier_OrdersLoading').hide();
        $('#Supplier_OrderList').show().DataTable({
            aaData: d,
            aoColumns: [
                { title: "Order Date", data: "orderDateDisplay", "asSorting": ["desc"] },
                { title: "Customer", data: "customerWithTag" },
                { title: "Status", data: "orderStatus" },
                { title: "Washing", data: "loadsForWashing" },
                { title: "Ironing", data: "clothesForIroning" },
                { title: "Total", data: "total" }
            ],
            "columnDefs": [
                { className: "dt-right", "targets": [3, 4, 5] },
                { targets: 5, render: $.fn.dataTable.render.number(',', '.', 2, '$') }
            ]
        });
    };

    this.showhideAdvanceSearch = function () {
        $("#DIV_OrderSearch_Advance").toggle(500);
    }

    this.updateOurderStatus = function () {
        var updateModel = {
            OrderNumber: $("#order_number").val(),
            OrderStatus: $("#DDL_Update_OrderStatus").val(),
            Notes: $("#Txt_UpdateOrder_Notes").val()
        };

        if (updateModel.OrderStatus.replace(/\s/g, "") == "" || updateModel.Notes.replace(/\s/g, "") == "") {
            ShowAlert("Please fill all the fields and try again.");
            return;
        }

        azyncLockPost("../Api/Order/AdminUpdateOrderStatus", updateModel, OrderManagement.UpdateOrdersSucsussfull, ConnectionError);
    }

    this.updateOrdersSucsussfull = function () {
        ShowAlert('Order updated successfully.');
        OrderManagement.LoadAddEditForm($("#order_id").val())
    }


    return {
        SearchOrders: searchOrders,
        AdvanceSearchOrders: advanceSearchOrders,
        FindOrders: findOrders,
        ResetHeight: resetHeight,
        LoadAddEditForm: loadAddEditForm,
        DrawAddressMap: drawAddressMap,
        CancelAddEdit: cancelAddEdit,
        SaveAddEditCourse: saveAddEditCourse,
        SaveSucsussfull: saveSucsussfull,
        UpdateOurderStatus: updateOurderStatus,
        UpdateOrdersSucsussfull: updateOrdersSucsussfull,
        
        UpdateActiveStatus: updateActiveStatus,
        ApproveSupplier: approveSupplier,
        ApproveSucsussfull: approveSucsussfull,
        LoadOrderList: loadOrderList,
        LoadOrdersSucsussfull: loadOrdersSucsussfull,
        ShowhideAdvanceSearch: showhideAdvanceSearch
    };

})();
