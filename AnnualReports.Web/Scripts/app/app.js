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

    $('.monthyear-datepicker').parent('.input-group.date').datetimepicker({
        viewMode: 'years',
        format: 'MM/YYYY',
        showTodayButton: true,
        ignoreReadonly: true,
    });

    $('.time-datepicker').parent('.input-group.date').datetimepicker({
        format: 'HH:mm:ss',
        useCurrent: false,
        ignoreReadonly: true,
    });

    $('.date-datepicker').parent('.input-group.date').datetimepicker({
        format: 'MM/DD/YYYY',
        showTodayButton: true,
        ignoreReadonly: true,
        useCurrent: false,
    });
});