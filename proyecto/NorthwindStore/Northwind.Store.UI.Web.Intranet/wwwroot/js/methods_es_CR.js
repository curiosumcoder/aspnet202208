/*! Soporte para la validación de formatos de número de Costa Rica*/
(function (factory) {
    if (typeof define === "function" && define.amd) {
        define(["jquery", "../jquery.validate"], factory);
    } else if (typeof module === "object" && module.exports) {
        module.exports = factory(require("jquery"));
    } else {
        factory(jQuery);
    }
}(function ($) {

    /*
     * Localized default methods for the jQuery validation plugin.
     * Locale: ES_CR
     */
    $.extend($.validator.methods, {
        number: function (value, element) {
            return this.optional(element) || /^-?(?:\d+|\d{1,3}(?:\.\d{3})+)(?:,\d+)?$/.test(value);
        }
    });

}));