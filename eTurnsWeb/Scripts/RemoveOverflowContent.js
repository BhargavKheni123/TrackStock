/*
* RandomClass - 	A jQuery plugin to remove any containing elements that overflow the selected element.
*					Perfect for elements with a fixed size and dynamic content.
* 
* Copyright (c) 2010 Fredi Bach
* www.fredibach.ch
*
* Usage:

$("#box").removeOverflow();
	
* Without recursion:

$("#box").removeOverflow( { recursion: false } );
 
* Plugin page: http://fredibach.ch/jquery-plugins/removeoverflow.php
*
*/
(function ($) {

    $.fn.removeOverflow = function (settings) {
        var defaults = {
            recursion: true,
            excludedTags: []
        };
        var s = $.extend(defaults, settings);

        var b = getBounds(this);
        removeElements(this);

        function removeElements(el) {
            $(el).children().each(function () {
                if (s.recursion) {
                    removeElements(this);
                }
                var eb = getBounds(this);
                if (eb.top < b.top || eb.left < b.left || eb.right > b.right || eb.bottom > b.bottom) {
                    if ($.inArray(this.tagName, s.excludedTags) == -1) {
                        $(this).remove();
                    }
                }
            });
        }

        function getBounds(el) {
            var offset = $(el).offset();
            var x1 = offset.left;
            var y1 = offset.top;
            var x2 = offset.left + $(el).width();
            var y2 = offset.top + $(el).height();
            return { top: y1, left: x1, bottom: y2, right: x2 };
        }

        return this;
    };

})(jQuery);