﻿// Write your Javascript code.
$(document).on("blur", "input.decimal", function (e) {
    $(this).val($(this).val().replace('.', ","));
});

$(document).ready(function () {
    $('.table-pagination').DataTable({
        /* No ordering applied by DataTables during initialisation */
        "order": []
    });
});