var FreeWorkoutManagement = (function () {
    var self = this;
    
    this.searchFreeWorkouts = function () {
        var searchText = GetSearchTextBoxText('TextBox_Main_SearchBox');
        $('#div_searchresults').html($("#DIV_InnerPageContent_Loading").html() + "<br><br><br><br><br>");
        $('#div_searchresults').load("/FreeWorkouts/SearchFreeWorkoutList?searchText=" + searchText , new function () {

        });
    };

    this.findFreeWorkouts = function (txt) {
        initFunctionDelayedCall(FreeWorkoutManagement.SearchFreeWorkouts);
    };


    this.searchAddNewFreeWorkouts = function () {
        var searchText = GetSearchTextBoxText('TextBox_AddNewFreeVideo_SearchBox');
        $('#AddNew-FreeWorkout-List').html($("#DIV_InnerPageContent_Loading").html() + "<br><br><br><br><br>");
        $('#AddNew-FreeWorkout-List').load("/FreeWorkouts/SearchAddNewFreeWorkoutList?searchText=" + searchText, new function () {

        });
    };

    this.findAddNewFreeWorkouts = function (txt) {
        initFunctionDelayedCall(FreeWorkoutManagement.SearchAddNewFreeWorkouts);
    };


    this.loadAddEditForm = function () {
        $('#div_addnew_freevideo').modal('show');
        $('#TextBox_AddNewFreeVideo_SearchBox').val('');
        FreeWorkoutManagement.SearchAddNewFreeWorkouts();
    };
    
    this.addToFreeWorkout = function (id) {
        var activeStautsUpdateModel = {
            Id: id,
            IsActive: true
        };

        azyncLockPost("../Api/FreeWorkouts/UpdateWorkoutFreeStatus", activeStautsUpdateModel, FreeWorkoutManagement.AddToFreeSaveSucsussfull, ConnectionError);
    }

    this.addToFreeSaveSucsussfull = function (model) {
        FreeWorkoutManagement.SearchFreeWorkouts();
        $('#div_workout_' + model.Id).remove();
    };


    this.removeFromFreeWorkout = function (id) {
        var activeStautsUpdateModel = {
            Id: id,
            IsActive: false
        };

        azyncLockPost("../Api/FreeWorkouts/UpdateWorkoutFreeStatus", activeStautsUpdateModel, FreeWorkoutManagement.RemoveFromFreeSaveSucsussfull, ConnectionError);
    }

    this.removeFromFreeSaveSucsussfull = function (model) {
        FreeWorkoutManagement.SearchFreeWorkouts();
        $('#div_workout_' + model.Id).remove();
    };



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
    
    this.saveSucsussfull = function (id) {
        SupplierManagement.SearchSuppliers();
        if ($('#Supplier_ActiveStatusChk').is(':checked')) {
            $('#Supplier_active_status').removeClass('label-danger').addClass('label-success').html('Active');
        }
        else {
            $('#Supplier_active_status').removeClass('label-success').addClass('label-danger').html('Inactive');
        }
    };
    
    return {
        SearchFreeWorkouts: searchFreeWorkouts,
        FindFreeWorkouts: findFreeWorkouts,

        SearchAddNewFreeWorkouts: searchAddNewFreeWorkouts,
        FindAddNewFreeWorkouts: findAddNewFreeWorkouts,

        LoadAddEditForm: loadAddEditForm,
        CancelAddEdit: cancelAddEdit,
        SaveSucsussfull: saveSucsussfull,
        ResetHeight: resetHeight,
        AddToFreeWorkout: addToFreeWorkout,
        AddToFreeSaveSucsussfull: addToFreeSaveSucsussfull,

        RemoveFromFreeWorkout: removeFromFreeWorkout,
        RemoveFromFreeSaveSucsussfull: removeFromFreeSaveSucsussfull
    };

})();
