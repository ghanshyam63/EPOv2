var isActive = false;
    var isCapexSwitched = false;
    var currCapexId = 0;

function EditItem(object) {
    var itemID = object.value;
    $("#Dashboard #SelectedItem").val(itemID);
    if (event.ctrlKey) {
        $('#Dashboard').attr('target', '_blank').attr("Action", "EditCapex").submit();
    } else {
        $('#Dashboard').attr('target', '_self').attr("Action", "EditCapex").submit();
    }

}

function ViewItem(object) {
    var itemID = object.value;
    $("#Dashboard #SelectedItem").val(itemID);
    if (event.ctrlKey) {
        $('#Dashboard').attr('target', '_blank').attr("Action", "ViewCapex").submit();
    } else {
        $('#Dashboard').attr('target', '_self').attr("Action", "ViewCapex").submit();
    }

}

function SendNotification(object) {
    var itemID = object.value;
    $("#Dashboard #SelectedItem").val(itemID);
    
    $('#Dashboard').attr('target', '_self').attr("Action", "SendNotification").submit();
    

}

function DeleteItem(object) {
    bootbox.confirm("Are you sure you want to Delete capex?", function(result) {
        if (result === true) {
            var itemID = object.value;
            $("#Dashboard #SelectedItem").val(itemID);
            $('#Dashboard').attr('target', '_self').attr("Action", "DeleteCapex").submit();
        }
    });
}

function ApproveItem(object) {
    var itemID = object.value;
    
    $("#DashboardApproval #SelectedItem").val(itemID);
    if (event.ctrlKey) {
        $('#DashboardApproval').attr('target', '_blank').attr("Action", "ApproveCapex").submit();
    } else {
        $('#DashboardApproval').attr('target', '_self').attr("Action", "ApproveCapex").submit();
    }
}

function ShowTransactions(object) {
    var capexID = object.value;
    if (currCapexId !== capexID) {
        isCapexSwitched = true;
        currCapexId = capexID;
    } else {
        isCapexSwitched = false;
    }
    if (isCapexSwitched) {
        if (isActive) {
            $('.material').removeClass('active');
            isActive = false;
        }

        //currOrderId = orderID;
        var btnTop = $(object).offset().top - $(window).scrollTop();;
        var left = $(object).offset().left-230;
        var modTop = 0;
        if (CalculateModalTop(0, btnTop)) modTop = btnTop - 207;
        else {
            modTop = btnTop + 23;
        }

        $.ajax({
            type: 'GET',
            url: 'ShowTransactions',
            cache: false,
            data: {
                capexId: capexID
            },
            error: function (xhr) {
                $(".alert").html(xhr.responseText);
            },
            success: function (result) {
                $('#my-dialog').html(result);
                $('.material').css({ top: modTop, bottom: '', left: left });
                $('.material').addClass('active');
                isActive = true;
            }
        });
    } else {
        isActive = false;
        $('.material').toggleClass('active');
    }
}

function CalculateModalTop(winTop, btnTop) {
    var tT = winTop + btnTop;
    if (tT >= 307) {
        $('.material').css({ 'transform-origin': 'bottom left' });
        return true;
    }
    else {
        $('.material').css({ 'transform-origin': 'top left' });
        return false;
    }
}

