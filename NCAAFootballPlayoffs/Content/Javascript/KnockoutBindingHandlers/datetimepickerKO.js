ko.bindingHandlers.datetimepicker = {
    init: function (element, valueAccessor, allBindingsAccessor) {
        var options = allBindingsAccessor().dateTimePickerOptions || {};
        $(element).datetimepicker(options, {
            format: false
        })

        //when a user changes the date, update the view model
        ko.utils.registerEventHandler(element, "dp.change", function (event) {
            var value = valueAccessor();
            if (ko.isObservable(value)) {
                if (event.date != null && !(event.date instanceof Date)) {
                    value(event.date.toDate());
                } else {
                    value(event.date);
                }
            }
        });

        ko.utils.domNodeDisposal.addDisposeCallback(element, function () {
            var picker = $(element).data("DateTimePicker");
            if (picker) {
                picker.destroy();
            }
        });
    },
    update: function (element, valueAccessor, allBindings, viewModel, bindingContext) {

        var picker = $(element).data("DateTimePicker");
        //when the view model is updated, update the widget
        if (picker) {
            //var koDate = ko.utils.unwrapObservable(valueAccessor());

            ////in case return from server datetime i am get in this form for example /Date(93989393)/ then fomat this
            //koDate = (typeof (koDate) !== 'object') ? new Date(koDate) : '';

            var koDate = ko.utils.unwrapObservable(valueAccessor());

            if (typeof koDate === "string") {
                // convert c# string to momentjs object
                koDate = moment(koDate);
            }

            //newDate = moment(koDate).format("YYYY-MM-DD HH:mm:ss")
            //picker.date(newDate);
            picker.date(koDate);
        }
    }
};

ko.bindingHandlers.datepicker = {
    init: function (element, valueAccessor, allBindingsAccessor) {
        var options = allBindingsAccessor().datepickerOptions || {};
        $(element).datepicker(options).on("changeDate", function (ev) {
            var observable = valueAccessor();
            observable(ev.date);
        });
    },
    update: function (element, valueAccessor) {
        var value = ko.utils.unwrapObservable(valueAccessor());
        $(element).datepicker("setValue", value);
    }
};