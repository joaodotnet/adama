// Write your Javascript code.
$(document).on("blur", "input.decimal", function (e) {
    $(this).val($(this).val().replace('.', ","));
});