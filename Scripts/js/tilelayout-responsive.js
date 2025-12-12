/**
 * Common Responsive Handler for Kendo TileLayout
 * Automatically adjusts TileLayout columns based on screen size
 */

(function ($) {
    'use strict';

    // Default breakpoints
    var BREAKPOINTS = {
        mobile: 576,
        tablet: 768,
        desktop: 992,
        large: 1200
    };

    /**
     * Calculate optimal columns based on screen width and current configuration
     * @param {number} currentColumns - Current number of columns
     * @param {number} screenWidth - Current screen width
     * @returns {number} Optimal number of columns
     */
    function calculateOptimalColumns(currentColumns, screenWidth) {
        if (screenWidth <= BREAKPOINTS.mobile) {
            // Mobile: Always 1 column
            return 1;
        } else if (screenWidth <= BREAKPOINTS.tablet) {
            // Tablet: Max 2 columns
            return Math.min(currentColumns, 2);
        } else if (screenWidth <= BREAKPOINTS.desktop) {
            // Small desktop: Max 3 columns
            return Math.min(currentColumns, 3);
        } else if (screenWidth <= BREAKPOINTS.large) {
            // Large desktop: Max 4 columns
            return Math.min(currentColumns, 4);
        }
        // Extra large: Use original columns (up to 5)
        return currentColumns;
    }

    /**
     * Adjust TileLayout for current screen size
     * @param {jQuery} $tileLayout - jQuery object of the TileLayout element
     */
    function adjustTileLayout($tileLayout) {
        var tileLayout = $tileLayout.data('kendoTileLayout');
        if (!tileLayout) {
            return;
        }

        var screenWidth = $(window).width();
        var currentOptions = tileLayout.options;
        var originalColumns = currentOptions.columns || 2;
        
        // Store original columns if not already stored
        if (!$tileLayout.data('originalColumns')) {
            $tileLayout.data('originalColumns', originalColumns);
        } else {
            originalColumns = $tileLayout.data('originalColumns');
        }

        var optimalColumns = calculateOptimalColumns(originalColumns, screenWidth);
        var currentColumns = currentOptions.columns || originalColumns;

        // Only update if columns need to change
        if (optimalColumns !== currentColumns) {
            try {
                tileLayout.setOptions({
                    columns: optimalColumns,
                    columnsWidth: "100%"
                });
            } catch (e) {
                console.warn('Error adjusting TileLayout:', e);
            }
        }
    }

    /**
     * Initialize responsive behavior for a TileLayout
     * @param {string|jQuery} selector - Selector or jQuery object for TileLayout
     */
    function initTileLayoutResponsive(selector) {
        var $tileLayout = $(selector);
        
        if ($tileLayout.length === 0) {
            return;
        }

        // Wait for TileLayout to be initialized
        var checkInitialized = setInterval(function () {
            var tileLayout = $tileLayout.data('kendoTileLayout');
            if (tileLayout) {
                clearInterval(checkInitialized);
                
                // Initial adjustment
                adjustTileLayout($tileLayout);
                
                // Handle window resize with debounce
                var resizeTimer;
                $(window).on('resize.tileLayoutResponsive', function () {
                    clearTimeout(resizeTimer);
                    resizeTimer = setTimeout(function () {
                        adjustTileLayout($tileLayout);
                    }, 150);
                });
            }
        }, 100);

        // Clear interval after 5 seconds if TileLayout not found
        setTimeout(function () {
            clearInterval(checkInitialized);
        }, 5000);
    }

    /**
     * Initialize all TileLayouts on the page
     */
    function initAllTileLayouts() {
        // Find TileLayouts by checking elements with IDs containing "tileLayout" (case insensitive)
        $('[id]').each(function () {
            var $element = $(this);
            var id = ($element.attr('id') || '').toLowerCase();
            if (id.indexOf('tilelayout') !== -1 && !$element.data('tileLayoutInitialized')) {
                initTileLayoutResponsive($element);
            }
        });
        
        // Also check for elements that already have the Kendo TileLayout widget
        // This catches TileLayouts with different ID patterns
        $('div, span').filter(function() {
            return $(this).data('kendoTileLayout') !== undefined;
        }).each(function() {
            var $element = $(this);
            if (!$element.data('tileLayoutInitialized')) {
                $element.data('tileLayoutInitialized', true);
                initTileLayoutResponsive($element);
            }
        });
    }

    // Initialize when document is ready
    $(document).ready(function () {
        // Wait a bit for Kendo to initialize all widgets
        setTimeout(function() {
            initAllTileLayouts();
        }, 300);
        
        // Also try after a longer delay in case some TileLayouts are initialized later
        setTimeout(function() {
            initAllTileLayouts();
        }, 1500);
    });

    // Expose function globally for manual initialization
    window.initTileLayoutResponsive = initTileLayoutResponsive;
    window.adjustTileLayout = adjustTileLayout;

})(jQuery);

