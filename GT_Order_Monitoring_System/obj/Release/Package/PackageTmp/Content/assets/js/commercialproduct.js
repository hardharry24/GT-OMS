var testForm = document.getElementById('form-create');
testForm.onsubmit = function (event) {
    event.preventDefault();

    var errorMsg = document.getElementById("errortxt").innerHTML;
    if (errorMsg == "Code Name Valid!") {
        var request = new XMLHttpRequest();
        // POST to any url
        request.open('POST', "/Product/Create", false);

        var formData = new FormData(document.getElementById('form-create'));
        console.log(formData);
        request.send(formData);

        var resObj = JSON.parse(request.response);

        if (resObj.code == 1) {
            swal("Successfully Saved!", {
                icon: "success",
                closeOnClickOutside: false
            }).then((ok) => {
                if (ok) {
                    window.location.href = "/Product/CommercialProducts";
                }
            });
        }
        else {
            swal(resObj.message, {
                icon: "error",
                closeOnClickOutside: false
            })
        }

    }
    else {
        swal("Please enter valid code!", {
            icon: "error",
            closeOnClickOutside: false
        });
    }

}

var myFormUpdate = document.getElementById('update-product');
myFormUpdate.onsubmit = function (event) {
    event.preventDefault();

    var request = new XMLHttpRequest();
    // POST to any url
    request.open('POST', "/Product/Update", false);

    var counter = myFormUpdate.className;

    var formDataUpdate = new FormData();

    var code = document.getElementById("code" + counter).value;
    var price = document.getElementById("description" + counter).value;
    var pricekg = document.getElementById("unit_price" + counter).value;
    var pricepck = document.getElementById("unit_pricekg" + counter).value;
    var pricepck = document.getElementById("unit_pricepck" + counter).value
    var priceth = document.getElementById("unit_priceth" + counter).value


    formDataUpdate.append('code', code);
    formDataUpdate.append('description', price);
    formDataUpdate.append('unit_price', pricekg);
    formDataUpdate.append('unit_pricekg', pricepck);
    formDataUpdate.append('unit_pricepck', pricepck);
    formDataUpdate.append('unit_priceth', priceth);

    request.send(formDataUpdate);

    var resObj = JSON.parse(request.response);

    if (resObj.code == 1) {
        swal("Successfully Updated!", {
            icon: "success",
            closeOnClickOutside: false
        }).then((ok) => {
            if (ok) {
                window.location.href = "/Product/CommercialProducts";
            }
        });
    }
    else {
        swal(resObj.message, {
            icon: "error",
            closeOnClickOutside: false
        })
    }


}

function validateCode() {
    var input = document.getElementById("ComProdcode").value;
    //var code = new Object();
    //code.code = code;

    const param = {
        code: input

    }

    var url = "/Product/ValidateCode";

    var request = new XMLHttpRequest();
    request.open('POST', url);
    request.setRequestHeader('Content-type', 'application/json');

    request.onreadystatechange = function () {
        if (this.readyState == 4 && this.status == 200) {
            console.log(this.responseText);
            //console.log(request.responseText);

            var resObj = JSON.parse(this.responseText);


            if (resObj.code == 1) {
                $('#ComProdcode').css('border-color', 'green');
                document.getElementById("errortxt").innerHTML = "Code Name Valid!";
                $('#errortxt').css({ 'color': 'green' });
            }
            else {
                $('#ComProdcode').css('border-color', 'red');
                document.getElementById("errortxt").innerHTML = "Code Name Not Valid!";
                $('#errortxt').css({ 'color': 'red' });

            }
            //{ 'color': 'red', 'font-size': '150%' }

        }
    };

    request.send(JSON.stringify(param));


}



function deleteProd(counter) {
    var test = "lblCode" + counter;
    var input = document.getElementById(test).value;

    const param = {
        code: input

    }

    var request = new XMLHttpRequest();
    request.open('POST', "/Product/Delete");
    request.setRequestHeader('Content-type', 'application/json');

    request.onreadystatechange = function () {
        if (this.readyState == 4 && this.status == 200) {
            console.log(this.responseText);
            //console.log(request.responseText);

            var resObj = JSON.parse(this.responseText);


            if (resObj.code == 1) {
                swal("Successfully Deleted!", {
                    icon: "success",
                    closeOnClickOutside: false
                }).then((ok) => {
                    if (ok) {
                        window.location.href = "/Product/CommercialProducts";
                    }
                });
            }
            else {
                swal(resObj.message, {
                    icon: "error",
                    closeOnClickOutside: false
                })

            }
        }
    };

    request.send(JSON.stringify(param));

}