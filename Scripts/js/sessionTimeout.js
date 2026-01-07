/**
 * Session Timeout Manager
 * Handles session timeout tracking, countdown display, and automatic redirect on expiration.
 * 
 * Features:
 * - 10-minute session timeout with idle detection
 * - Visual countdown timer display
 * - Automatic session refresh on user activity
 * - Redirect to login page on expiration
 * - Warning notifications before expiration
 */
(function ($) {
    'use strict';

    var SessionTimeout = {
        // Configuration
        config: {
            timeoutMinutes: 10,           // Session timeout in minutes
            warningMinutes: 2,            // Show warning 2 minutes before expiration
            checkInterval: 1000,         // Check every second
            keepAliveInterval: 60000,     // Keep-alive ping every minute (60 seconds)
            keepAliveUrl: '/Account/KeepAlive', // Keep-alive endpoint
            loginUrl: '/Account/Login'     // Login page URL
        },

        // State
        state: {
            lastActivity: null,
            warningShown: false,
            expired: false,
            keepAliveTimer: null,
            countdownTimer: null,
            countdownElement: null
        },

        /**
         * Initialize the session timeout manager
         */
        init: function () {
            // Only initialize if user is authenticated
            if (!this.isAuthenticated()) {
                return;
            }

            // Verify session is still valid before initializing
            var self = this;
            this.verifySession(function(isValid) {
                if (!isValid) {
                    // Session is expired, redirect to login
                    self.handleExpiration();
                    return;
                }

                // Set initial last activity time
                self.state.lastActivity = Date.now();

                // Create countdown display element
                self.createCountdownDisplay();

                // Start tracking user activity
                self.trackActivity();

                // Start countdown timer
                self.startCountdown();

                // Start keep-alive ping
                self.startKeepAlive();

                // Handle visibility change (tab switching)
                self.handleVisibilityChange();
            });
        },

        /**
         * Verify if the session is still valid
         */
        verifySession: function(callback) {
            var self = this;
            
            // Check session by calling keep-alive endpoint
            fetch(this.config.keepAliveUrl, {
                method: 'POST',
                credentials: 'same-origin',
                headers: {
                    'X-Requested-With': 'XMLHttpRequest'
                }
            })
            .then(function (response) {
                if (response.ok) {
                    // Session is valid
                    callback(true);
                } else if (response.status === 401 || response.status === 403) {
                    // Session expired
                    callback(false);
                } else {
                    // Unknown error, assume valid to avoid blocking
                    callback(true);
                }
            })
            .catch(function (error) {
                console.error('Session verification error:', error);
                // On network error, assume valid to avoid blocking
                callback(true);
            });
        },

        /**
         * Check if user is authenticated
         */
        isAuthenticated: function () {
            // Check if we're on the login page
            var currentPath = window.location.pathname.toLowerCase();
            if (currentPath.indexOf('/account/login') !== -1) {
                return false;
            }

            // Check for authentication indicator (you may need to adjust this based on your app)
            return true; // Assume authenticated if not on login page
        },

        /**
         * Create countdown display element in the header
         */
        createCountdownDisplay: function () {
            // Check if element already exists
            if ($('#session-timeout-display').length > 0) {
                this.state.countdownElement = $('#session-timeout-display');
                return;
            }

            // Create countdown display
            var $display = $('<div id="session-timeout-display" class="session-timeout-display">' +
                '<span class="k-icon k-i-clock"></span>' +
                '<span id="session-timeout-text" class="session-timeout-text">Session expires in: <strong id="session-countdown">10:00</strong></span>' +
                '</div>');

            // Insert into AppBar container (right side, immediately before user menu)
            var $container = $('#session-timeout-container');
            if ($container.length > 0) {
                // Insert into the dedicated container in AppBar (right side, before user menu)
                $display.appendTo($container);
            } else {
                // Fallback: try to find AppBar and insert before user menu
                var $appBar = $('#appBar');
                if ($appBar.length > 0) {
                    var $itemsContainer = $appBar.find('.k-appbar-items');
                    if ($itemsContainer.length > 0) {
                        // Find the user menu and insert before it
                        var $userMenu = $itemsContainer.find('.appbar-user-menu').closest('[data-role="appbar-item"]');
                        if ($userMenu.length > 0) {
                            $display.insertBefore($userMenu);
                        } else {
                            // If user menu not found, append to items container
                            $display.appendTo($itemsContainer);
                        }
                    } else {
                        $display.appendTo($appBar);
                    }
                } else {
                    // Fallback: add to body
                    $('body').prepend($display);
                }
            }

            this.state.countdownElement = $display;
        },

        /**
         * Track user activity (mouse, keyboard, touch, scroll)
         */
        trackActivity: function () {
            var self = this;
            var activityEvents = 'mousedown touchstart keydown scroll wheel';

            $(document).on(activityEvents, function () {
                self.state.lastActivity = Date.now();
                self.state.warningShown = false;

                // Refresh session on activity (debounced)
                clearTimeout(self.refreshTimer);
                self.refreshTimer = setTimeout(function () {
                    self.refreshSession();
                }, 5000); // Refresh 5 seconds after last activity
            });
        },

        /**
         * Start countdown timer
         */
        startCountdown: function () {
            var self = this;
            
            this.state.countdownTimer = setInterval(function () {
                self.updateCountdown();
            }, this.config.checkInterval);
        },

        /**
         * Update countdown display
         */
        updateCountdown: function () {
            if (this.state.expired) {
                return;
            }

            var now = Date.now();
            var elapsed = now - this.state.lastActivity;
            var elapsedMinutes = elapsed / (1000 * 60);
            var remainingMinutes = this.config.timeoutMinutes - elapsedMinutes;
            var remainingSeconds = Math.floor((remainingMinutes % 1) * 60);
            var remainingMinutesInt = Math.floor(remainingMinutes);

            // Check if session expired
            if (remainingMinutes <= 0) {
                this.handleExpiration();
                return;
            }

            // Update countdown display
            var minutes = Math.max(0, remainingMinutesInt);
            var seconds = Math.max(0, remainingSeconds);
            var displayText = this.padTime(minutes) + ':' + this.padTime(seconds);

            $('#session-countdown').text(displayText);

            // Show warning if approaching expiration
            if (remainingMinutes <= this.config.warningMinutes && !this.state.warningShown) {
                this.showWarning(remainingMinutes);
            }

            // Update display color based on remaining time
            var $display = this.state.countdownElement;
            if ($display) {
                $display.removeClass('session-warning session-critical');
                if (remainingMinutes <= 1) {
                    $display.addClass('session-critical');
                } else if (remainingMinutes <= this.config.warningMinutes) {
                    $display.addClass('session-warning');
                }
            }
        },

        /**
         * Pad time value with leading zero
         */
        padTime: function (value) {
            return value < 10 ? '0' + value : value.toString();
        },

        /**
         * Show warning notification
         */
        showWarning: function (remainingMinutes) {
            this.state.warningShown = true;

            // Show Kendo notification
            if (typeof kendo !== 'undefined' && kendo.ui && kendo.ui.Notification) {
                var notification = $('#session-warning-notification').data('kendoNotification');
                if (!notification) {
                    notification = $('<div id="session-warning-notification"></div>')
                        .appendTo('body')
                        .kendoNotification({
                            position: {
                                top: 20,
                                right: 20
                            },
                            stacking: 'down',
                            hideOnClick: true,
                            autoHideAfter: 0
                        })
                        .data('kendoNotification');
                }

                var message = 'Your session will expire in ' + Math.ceil(remainingMinutes) + 
                    ' minute' + (remainingMinutes > 1 ? 's' : '') + '. Please save your work.';
                
                notification.show({
                    message: message,
                    type: 'warning'
                }, 'warning');
            } else {
                // Fallback alert
                alert('Warning: Your session will expire in ' + Math.ceil(remainingMinutes) + ' minute(s).');
            }
        },

        /**
         * Handle session expiration
         */
        handleExpiration: function () {
            if (this.state.expired) {
                return;
            }

            this.state.expired = true;

            // Clear timers
            this.stopTimers();

            // Show expiration message
            if (typeof kendo !== 'undefined' && kendo.ui && kendo.ui.Notification) {
                var notification = $('#session-warning-notification').data('kendoNotification');
                if (notification) {
                    notification.show({
                        message: 'Your session has expired. You will be redirected to the login page.',
                        type: 'error'
                    }, 'error');
                }
            }

            // Redirect to login page after short delay (without returnUrl to go to home after login)
            setTimeout(function () {
                window.location.href = SessionTimeout.config.loginUrl;
            }, 2000);
        },

        /**
         * Start keep-alive ping to refresh session
         */
        startKeepAlive: function () {
            var self = this;

            // Initial keep-alive
            this.refreshSession();

            // Set up periodic keep-alive
            this.state.keepAliveTimer = setInterval(function () {
                // Only ping if user has been active recently (within last 8 minutes)
                var now = Date.now();
                var elapsed = now - self.state.lastActivity;
                var elapsedMinutes = elapsed / (1000 * 60);

                if (elapsedMinutes < 8) {
                    self.refreshSession();
                }
            }, this.config.keepAliveInterval);
        },

        /**
         * Refresh session by calling keep-alive endpoint
         */
        refreshSession: function () {
            var self = this;

            // Use fetch API for keep-alive
            fetch(this.config.keepAliveUrl, {
                method: 'POST',
                credentials: 'same-origin',
                headers: {
                    'X-Requested-With': 'XMLHttpRequest'
                }
            })
            .then(function (response) {
                if (response.ok) {
                    // Session refreshed successfully
                    self.state.lastActivity = Date.now();
                } else if (response.status === 401 || response.status === 403) {
                    // Session expired on server
                    self.handleExpiration();
                }
            })
            .catch(function (error) {
                console.error('Session keep-alive error:', error);
                // Don't expire on network errors, but log them
            });
        },

        /**
         * Handle visibility change (tab switching)
         */
        handleVisibilityChange: function () {
            var self = this;

            $(document).on('visibilitychange', function () {
                if (!document.hidden) {
                    // Tab became visible - verify session is still valid
                    self.verifySession(function(isValid) {
                        if (!isValid) {
                            // Session expired while tab was hidden
                            self.handleExpiration();
                        } else {
                            // Session is valid, check elapsed time
                            var now = Date.now();
                            var elapsed = now - self.state.lastActivity;
                            var elapsedMinutes = elapsed / (1000 * 60);

                            if (elapsedMinutes >= self.config.timeoutMinutes) {
                                self.handleExpiration();
                            } else {
                                // Refresh session when tab becomes visible
                                self.refreshSession();
                            }
                        }
                    });
                }
            });
        },

        /**
         * Stop all timers
         */
        stopTimers: function () {
            if (this.state.countdownTimer) {
                clearInterval(this.state.countdownTimer);
                this.state.countdownTimer = null;
            }

            if (this.state.keepAliveTimer) {
                clearInterval(this.state.keepAliveTimer);
                this.state.keepAliveTimer = null;
            }

            if (this.refreshTimer) {
                clearTimeout(this.refreshTimer);
                this.refreshTimer = null;
            }
        },

        /**
         * Destroy session timeout manager
         */
        destroy: function () {
            this.stopTimers();
            $(document).off('mousedown touchstart keydown scroll wheel visibilitychange');
            if (this.state.countdownElement) {
                this.state.countdownElement.remove();
            }
        }
    };

    // Initialize when DOM is ready
    $(document).ready(function () {
        // Small delay to ensure all elements are loaded
        setTimeout(function () {
            SessionTimeout.init();
        }, 500);
    });

    // Expose to global scope for manual control if needed
    window.SessionTimeout = SessionTimeout;

})(jQuery);

