$(function () {
    // disable jquery date validation. see: http://stackoverflow.com/questions/5966244/jquery-datepicker-chrome
    $.validator.methods.date = function (value, element) {
        return this.optional(element) || moment(value, 'YYYY').isValid();
    };

    $('.year-datepicker').parent('.input-group.date').datetimepicker({
        viewMode: 'years',
        format: 'YYYY',
        showTodayButton: true,
        ignoreReadonly: true,
        useCurrent: false,
    });
});