(function ($) {

    echo = function (whatever) {
        if (opts.debug) console.log(whatever); return whatever;
    }

    // grid plugin default options
    var knockoutSessionTimeoutDefaults = {
        debug: false,
        checkSessionUrl: '/SessionHandler/CheckSession',
        timeoutUrl: '/SessionHandler/Timeout'
    }
    var template = '<script type="text/html" id="sessionTimeoutControlTemplate">    \
<div class="bootstrap sessionExpireWarning"> \
    Your session will expire in a minute or less.  Click <a data-bind="click:keepSession">here</a> to reactivate it. \
</div> \
</script>';

    var model = null;

    var methods = {
        init: function (options) {

            var $placeHolderElement = this;
            $placeHolderElement.append(template);
            $placeHolderElement.css('display', 'none');

            var $timeout = $('<div>');
            $timeout.attr('data-bind', 'template: { name: "sessionTimeoutControlTemplate", data: $data }');
            $placeHolderElement.append($timeout);
            
            // Merge passed options with defaults
            var opts = $.extend({}, knockoutSessionTimeoutDefaults, options);

            function Model() {
                var self = this;
                self.keepSession = function () {

                    $.getJSON(opts.checkSessionUrl, { keepAlive: true }, function (data) {

                        if (data.Success) {

                            $('#sessionExpireWarning').slideToggle(500);
                            model.nearTimedOut(false);
                        }
                    });
                };
                self.nearTimedOut = ko.observable(false);
            }

            model = new Model();
            ko.applyBindings(model, $timeout[0]);

            (function checkTimeout(data) {

                $.getJSON(opts.checkSessionUrl, { keepAlive: false }, function (data) {

                    if (opts.debug) console.log('Now={0} ExpireDate={1} NearExpireDate={2}'.format(data.Now, data.ExpireDate, data.NearExpireDate));

                    if (data.TimedOut) window.location = opts.timeoutUrl;
                    else
                        if (data.NearTimedOut) {
                            if (!model.nearTimedOut()) {
                                var retans = window.confirm("Your session is about to Expire");
                                if (retans) {
                                    $.getJSON(opts.checkSessionUrl, { keepAlive: true }, function (data) {
                                        if (data.Success) {
                                            $('#sessionExpireWarning').slideToggle(500);
                                            model.nearTimedOut(false);
                                        }
                                    });
                                }
                                //                                $('#sessionExpireWarning').slideToggle(500);
                                model.nearTimedOut(true);
                            }
                        }
                    setTimeout(checkTimeout, 5000, data);
                });
            })();
        }
    };

    $.fn.knockoutSessionTimeout = function (method) {
        // Method calling logic
        if (methods[method]) {
            return methods[method].apply(this, Array.prototype.slice.call(arguments, 1));
        } else if (typeof method === 'object' || !method) {
            return methods.init.apply(this, arguments);
        } else {
            $.error('Method ' + method + ' does not exist on jQuery.knockoutSessionTimeout');
        }
    };
})(jQuery);



