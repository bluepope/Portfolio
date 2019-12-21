$(document).ready(function ($) {
	"use strict";
	$('.equal-height').matchHeight({
		property: 'max-height'
	});

	// Menu Trigger
	$('#menuToggle').on('click', function (event) {
		var windowWidth = $(window).width();
		if (windowWidth < 1025) {
			$('body').removeClass('open');
			if (windowWidth > 768) {
				$('#left-panel').toggleClass('open-menu');
			} else {
				$('#left-panel').slideToggle(200);
			}
		} else {
			$('body').toggleClass('open');
			$('#left-panel').removeClass('open-menu');
		}

	});
	/*
	$(".menu-item-has-children.dropdown").each(function () {
		var $temp_text = $(this).children('.dropdown-toggle').html();
		$(this).children('.sub-menu').prepend('<li class="subtitle">' + $temp_text + '</li>');
	});
	*/
	// Load Resize 
	$(window).on("load resize", function (event) {
		var w = $(window).width();
		var h = $(window).height() - $('#header').height() - $('footer').height() - 2;

		if (w > 768) {
			$("#left-panel").show();
		}

		if (w < 1025) {
			$('body').addClass('small-device');
		} else {
			$('body').removeClass('small-device');
		}

		$('#right-panel > .content').css("min-height", h);
	});
});