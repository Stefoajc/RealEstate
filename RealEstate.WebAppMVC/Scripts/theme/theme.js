﻿(function() {

	"use strict";

	var Core = {

		initialized: false,

		initialize: function() {

			if (this.initialized) {return;}
			this.initialized = true;

			this.build();

		},

		build: function() {
			
			// Scroll to Top Button.
			$.scrollToTop();
			
			// Owl pgl-main-slide
			this.owlCarousel();
			
			// dropdown menu
			this.dropdownhover();
			
			//flexslider
			this.flexslider();
			
			//choosenselect
			this.choosenselect();
			
			//masonrygrid
			this.masonrygrid();
			
		},


		owlCarousel: function(options) {

			var total = $("div.owl-carousel:not(.manual)").length,
				count = 0;

			$("div.owl-carousel:not(.manual)").each(function() {

				var slider = $(this);

                var defaults = {
					 // Most important owl features
					items : 4,
					itemsCustom : false,
					itemsDesktop : [1199,2],
					itemsDesktopSmall : [979,1],
					itemsTablet: [768,2],
					itemsTabletSmall: false,
					itemsMobile : [479,1],
					singleItem : true,
					itemsScaleUp : false,

					//Basic Speeds
					slideSpeed : 200,
					paginationSpeed : 800,
					rewindSpeed : 1000,

					//Autoplay
					autoPlay : false,
					stopOnHover : false,

					// Navigation
					navigation : true,
					navigationText : ["<i class=\"icons icon-left\"></i>","<i class=\"icons icon-right\"></i>"],
					rewindNav : true,
					scrollPerPage : false,

					//Pagination
					pagination : true,
					paginationNumbers: false,

					// Responsive
					responsive: true,
					responsiveRefreshRate : 200,
					responsiveBaseWidth: window,

					// CSS Styles
					baseClass : "owl-carousel",
					theme : "owl-theme",

					//Lazy load
					lazyLoad : false,
					lazyFollow : true,
					lazyEffect : "fade",

					//Auto height
					autoHeight : false,

					//JSON
					jsonPath : false,
					jsonSuccess : false,

					//Mouse Events
					dragBeforeAnimFinish : true,
					mouseDrag : true,
					touchDrag : true,

					//Transitions
					transitionStyle : false,

					// Other
					addClassActive : false,

					//Callbacks
					beforeUpdate : false,
					afterUpdate : false,
					beforeInit: false,
					afterInit: false,
					beforeMove: false,
					afterMove: false,
					afterAction: false,
					startDragging : false,
					afterLazyLoad : false
				};

				var config = $.extend({}, defaults, options, slider.data("plugin-options"));

				// Initialize Slider
				slider.owlCarousel(config).addClass("owl-carousel-init");

			});

		},

		dropdownhover: function(options) {
		
			/** Extra script for smoother navigation effect **/
			if ($(window).width() > 992) {
				$('.pgl-navbar-main .dropdown-toggle').addClass('disabled');
				$('.navbar .dropdown').hover(function () {
					"use strict";
					$(this).addClass('open').find('.dropdown-menu').first().stop(true, true).delay(150).slideDown();
				}, function () {
					"use strict";
					var na = $(this);
					na.find('.dropdown-menu').first().stop(true, true).delay(100).slideUp('fast', function () {
						na.removeClass('open');
					});
				});
			} else {return;}
			
		},
		
		flexslider: function(options) {
			// The slider being synced must be initialized first
			$('#carousel').flexslider({
				animation: "slide",
				controlNav: false,
				animationLoop: false,
				slideshow: false,
				itemWidth: 148,
				itemMargin: 10,
				asNavFor: '#slider'
			});
			$('#slider').flexslider({
				animation: "slide",
				controlNav: false,
				animationLoop: false,
				slideshow: false,
				sync: '#carousel'
			});
			
		},
		
		choosenselect: function(options) {           

	        var config = {
                '.chosen-select': { no_results_text: "Няма резултат с", placeholder_text_single: "Изберете" },
	            '.chosen-select-deselect': { allow_single_deselect: true },
	            '.chosen-select-no-single': { disable_search_threshold: 10 },
	            '.chosen-select-no-results': { no_results_text: 'Няма резултат с' },
	            '.chosen-select-width': { width: "95%" }
	        };

			for (var selector in config) {
	    	  $(selector).chosen(config[selector]);
			}
			
		},
		
		masonrygrid: function(options) {
		
			// Masonry for desktop
			if($(window).width() < 361) return;
					
			var $container = $('.masonry-desk');
			// initialize
			
			$container.imagesLoaded( function() {
				$container.masonry();
			});
			var $items = document.querySelectorAll('.masonry-item');
			imagesLoaded( $items, function() {
				$container.masonry({
					itemSelector: '.masonry-item'
				});
			});
			
		}

	};

	Core.initialize();

})();