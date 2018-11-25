



$(document).ready(function () {

    function validateInput() {
        var time = document.getElementById("timepicker").value;
        if (time.toString() == '') {
            alert("Please select pickup time");
            return false;
        }
        else {
            return true;
        }
    }

    var curDt = new Date();
    //12pm-10pm
    var minTime = "";
    if (curDt.getHours() < 11) {
        minTime = '12:00pm';
    }
    else {
        if (curDt.getMinutes() < 30) {
            minTime = (curDt.getHours() + 1).toString() + ":30pm";
        }
        else {
            minTime = (curDt.getHours() + 2).toString() + ":00pm";
        }
    }

    if (curDt.getHours() > 20) {
        //No More orders
        $('timepicker').prop('disabled', true);
        $('btnPlaceOrder').prop('disabled', true);
        $('lblShopClosed').show();
    }

    $('#timepicker').timepicker({
        'minTime': minTime,
        'maxTime': '20:00pm'
    });

    //$('#timepicker').prop('width:100px;')


    $("#btnCoupon").click(function () {
        //var couponCode = $("#txtCouponCode").val();
        //var orderTotal = $("#txtOrderTotal").val();

        var couponCode = document.getElementById("txtCouponCode").value;
        var orderTotal = document.getElementById("txtOrderTotal").value;

        $.ajax({
            url: '/Api/CouponApi?orderTotal=' + orderTotal + '&couponCode=' + couponCode,
            type: 'GET',
            dataType: 'text',
            success: function (data, textStatus, xhr) {
                var splitData = data.split(":");

                if (splitData[1] == 'E') {
                    // error
                    alert("Coupon is invalid or do not meet the criteria")
                }
                else {
                    $("#txtOrderTotal").attr('value', splitData[0]);

                    $("#txtCouponCode").readOnly = true;
                    document.getElementById('btnCoupon').style.display = 'none';
                    document.getElementById('btnRemoveCoupon').style.display = '';

                }
            }
        });
    });

});
