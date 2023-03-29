function InitTable() {
    $("#myTable").DataTable();
}

function DataTablesAdd(table) {
    $(function () {
        $(table).DataTable();
    });

}

function DataTablesRemove(table) {
    $(function () {
        $(table).DataTable().destroy();
    });
}
