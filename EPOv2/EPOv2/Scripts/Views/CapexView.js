function selectEntity(object) {
    var entityID = object.value;
    var ownerID = $('select[id=SelectedOwner]').val();
    var capexType = $('select[id=SelectedCapexType]').val();
    if (entityID != null && entityID != '') {
        $.ajax({
            type: 'GET',
            url: 'FetchCostCentre',
            cache: false,
            data: {
                entityId: entityID
            },
            error: function (xhr, status, error) {
                $(".alert").html(xhr.responseText);
            },
            success: function (result) {
                $('#div-capexcompanybox').html(result);
                $('select[id=SelectedOwner]').val(ownerID);
                $('select[id=SelectedOwner]').removeAttr("disabled");
                $('select[id=SelectedCostCentre]').removeAttr("disabled");
                $('select[id=SelectedCapexType]').val(capexType);
                $('input[id=EntityId]').val(entityID);
            }
        });
    }
}

function editEntity(object) {
    var entityID = object.value;
    var ownerID = $('select[id=SelectedOwner]').val();
    var capexType = $('select[id=SelectedCapexType]').val();
    if (entityID != null && entityID !== '') {
        $.ajax({
            type: 'GET',
            url: 'FetchCostCentre',
            cache: false,
            data: {
                entityId: entityID
            },
            error: function (xhr, status, error) {
                $(".alert").html(xhr.responseText);
            },
            success: function (result) {
                $('#div-capexcompanybox').html(result);
                $('select[id=SelectedOwner]').val(ownerID);
                $('select[id=SelectedCapexType]').val(capexType);
                $('input[id=EntityId]').val(entityID);
            }
        });
    }
}

function selectCostCenter(object) {
    var ccID = object.value;
    $('input[id=CostCentreId]').val(ccID);
}

function selectOwner(object) {
    $('input[id=OwnerId]').val(object.value);
}

function selectType(object) {
    $('input[id=CapexType]').val(object.value);
}

function isNumberKey(evt) {
    var charCode = (evt.which) ? evt.which : event.keyCode;
    if (charCode != 46 && charCode > 31
        && (charCode < 48 || charCode > 57))
        return false;
    return true;
}

function SubmitCapex() {

   
    //$("#CapexForm > #Title").val($("#div-capexdetailbox #Title").val());
    //$("#CapexForm > #Description").val($("#div-capexdetailbox #Description").val());
    //$("#FilePath").val($("#SelectedFilePath").val());
    // alert($("#FilePath").val());
    // $("#CapexForm > #TotalExGST").val($("#div-capexdetailbox #TotalExGST").val());
    //var file = $('#SelectedFilePath').val();
   
    //    $("#CapexForm").validate({
    //        rules: {
    //            SelectedFilePath: {
    //                required: true,
    //                accept: 'application/pdf'
    //            }
    //        }
    //    });
    //alert();
    $('#CapexForm').submit();
}

function DeclineCapex() {
    //bootbox.confirm("Are you sure you want to Decline capex?", function(result) {
    //    if (result === true) {
    //        $('#CapexForm').attr('target', '_self').attr("Action", "DeclineCapex").submit();
    //    }
    //});

    //bootbox.prompt("Are you sure you want to Decline capex?", function (result) {
    //    if (result === null) {
            
    //    } else {
    //        //Example.show("Hi <b>" + result + "</b>");
    //        $("#Comment").val(result);
    //    }
    //});
    bootbox.dialog({
        title: "Are you sure you want to Decline capex?",
        message: '<div class="row">  ' +
            '<div class="col-md-12"> ' +
            '<form class="form-horizontal"> ' +
            '<div class="form-group"> ' +
            '<label class="col-md-2 control-label" for="name">Comment</label> ' +
            '<div class="col-md-10"> ' +
            '<textarea id="declineComment" name="declineComment" placeholder="Comment" class="form-control" cols="90" rows="4"></textarea> ' +
            '</form> </div>  </div>',
        buttons: {
            danger: {
                label: "Cancel",
                className: "btn-default",
                callback: function () {
                   
                }
            },
            main: {
                label: "Ok",
                className: "btn-primary",
                callback: function () {
                    $("#Comment").val($("#declineComment").val());
                    $('#CapexForm').attr('target', '_self').attr("Action", "DeclineCapex").submit();
                }
            }
           
        }
    }
        );

}
function AuthoriseCapex() {
    bootbox.confirm("Are you sure you want to Authorise capex?", function(result) {
        if (result === true) {
            $('#CapexForm').attr("Action", "AuthoriseCapex");
            $('#CapexForm').attr('target', '_self');
            $('#CapexForm').submit();
        }
    });
}

function ViewDocument(object) {
    var docId = object.value;
    $("#CapexId").val(docId);
   
    //if (event.ctrlKey) {
    $('#DocumentViewForm').attr('target', '_blank').submit();
    //}
    //$('#DocumentViewForm').submit();
}

function changeCapexFile(object) {
    $('.capex-file-reference').addClass("hidden");
}