checkCheckbox();

function checkCheckbox() {
    var checktxtSerch = document.getElementById("ckTxtSearch").checked;
    if (checktxtSerch) {
        document.getElementById("txtSearch").hidden = false
    }
    else {
        document.getElementById("txtSearch").hidden = true;
    }
}

function onchangeSearch() {
    var input, filter, found, table, tr, td, i, j;
    input = document.getElementById("txtSearch");
    filter = input.value.toUpperCase();
    table = document.getElementById("dataTable");
    tr = table.getElementsByTagName("tr");

    console.log(input);

    for (i = 0; i < tr.length; i++) {
        td = tr[i].getElementsByTagName("td");
        for (j = 0; j < td.length; j++) {
            if (td[j].innerHTML.toUpperCase().indexOf(filter) > -1) {
                found = true;
            }
        }
        if (found) {
            tr[i].style.display = "";
            found = false;
        } else {
            if (tr[i].id != 'tableHeader') { tr[i].style.display = "none"; }
            //tr[i].style.display = "none";
        }
    }
    //tableHeadertableRow
}

function onchangeSearchAgent() {
    var input, filter, found, table, tr, td, i, j;
    input = document.getElementById("txtAgentSearch");
    filter = input.value.toUpperCase();
    table = document.getElementById("tblAgents");
    tr = table.getElementsByTagName("tr");
    //
    console.log(input);

    for (i = 0; i < tr.length; i++) {
        td = tr[i].getElementsByTagName("td");
        for (j = 0; j < td.length; j++) {
            if (td[j].innerHTML.toUpperCase().indexOf(filter) > -1) {
                found = true;
            }
        }
        if (found) {
            tr[i].style.display = "";
            found = false;
        } else {
            if (tr[i].id != 'tableHeader') { tr[i].style.display = "none"; }
            //tr[i].style.display = "none";
        }
    }
    //tableHeadertableRow
}
function onCheckChange() {
    checkCheckbox();
    document.getElementById("txtSearch").value = "";
}

function onSelect(name, code) {
    alert(name + " " + code);

}

