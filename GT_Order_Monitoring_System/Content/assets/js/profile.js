var openFile = function (file) {
    var input = file.target;

    var reader = new FileReader();
    reader.onload = function () {
        var dataURL = reader.result;
        var output = document.getElementById('avatar');
        output.src = dataURL;
    };
    reader.readAsDataURL(input.files[0]);
};


var openFile2 = function (file) {
    var input = file.target;

    var reader = new FileReader();
    reader.onload = function () {
        var dataURL = reader.result;
        var output = document.getElementById('signature');
        console.log(output);
        output.src = dataURL;
    };
    reader.readAsDataURL(input.files[0]);
};

function saveImage()
{
    // Checking whether FormData is available in browser  
    if (window.FormData !== undefined) {  
  
        var fileUpload = $("#FileUpload1").get(0);  
        var files = fileUpload.files;  
              
        // Create FormData object  
        var fileData = new FormData();  
  
        // Looping over all files and add it to FormData object  
        for (var i = 0; i < files.length; i++) {  
            fileData.append(files[i].name, files[i]);  
        }  
              
        // Adding one more key to FormData object  
        //fileData.append('username', ‘Manas’);  
  
        $.ajax({  
            url: '/Home/UploadProfile',  
            type: "POST",  
            contentType: false, // Not to set any content header  
            processData: false, // Not to process data  
            data: fileData,  
            success: function (result) {  
                alert(result);  
            },  
            error: function (err) {  
                alert(err.statusText +"\n Tips: Please select smaller image size and Check Image Name. \nNot Saved!");  
            }  
        });  
    } else {  
        alert("FormData is not supported.");  
    }  
}