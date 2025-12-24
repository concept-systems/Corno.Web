/*
* TelerikReporting v18.3.24.1112 (https://www.telerik.com/products/reporting.aspx)
* Copyright 2024 Progress Software EAD. All rights reserved.
*
* Telerik Reporting commercial licenses may be obtained at
* https://www.telerik.com/purchase/license-agreement/reporting.aspx
* If you do not own a commercial license, this file shall be governed by the trial license terms.
*/
var telerikReportViewer = (function (exports) {
    'use strict';

    var stringFormatRegExp = /{(\w+?)}/g;
    var specialKeys = {
      DELETE: 46,
      BACKSPACE: 8,
      TAB: 9,
      ESC: 27,
      LEFT: 37,
      UP: 38,
      RIGHT: 39,
      DOWN: 40,
      END: 35,
      HOME: 36
    };
    function isSpecialKey() {
      var userAgent = window.navigator.userAgent.toLowerCase();
      if (userAgent.indexOf("firefox") > -1) {
        var specialKeysArray = Object.keys(specialKeys);
        var specialKeysLength = specialKeysArray.length;
        return function(keyCode) {
          for (var i = 0; i < specialKeysLength; i++) {
            if (specialKeys[specialKeysArray[i]] == keyCode) {
              return true;
            }
          }
        };
      }
      return function(keyCode) {
        return false;
      };
    }
    function toXhrErrorData(xhr, status, error) {
      return {
        xhr,
        status,
        error
      };
    }
    function rectangle(left, top, width, height) {
      return {
        left,
        top,
        width,
        height,
        right: function() {
          return left + width;
        },
        bottom: function() {
          return top + height;
        },
        union: function(other) {
          var newLeft = Math.min(left, other.left);
          var newTop = Math.min(top, other.top);
          var newWidth = Math.max(this.right(), other.right()) - newLeft;
          var newHeight = Math.max(this.bottom(), other.bottom()) - newTop;
          return rectangle(
            newLeft,
            newTop,
            newWidth,
            newHeight
          );
        }
      };
    }
    function generateGuidString() {
      return "xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx".replace(/[xy]/g, function(c) {
        var r = Math.random() * 16 | 0;
        var v = c == "x" ? r : r & 3 | 8;
        return v.toString(16);
      });
    }
    function trim(s, charlist) {
      return rTrim(lTrim(s, charlist), charlist);
    }
    function replaceAll(str, find, replace) {
      return str.replace(new RegExp(find, "g"), replace);
    }
    function lTrim(s, charlist) {
      if (charlist === void 0) {
        charlist = "s";
      }
      return s.replace(new RegExp("^[" + charlist + "]+"), "");
    }
    function rTrim(s, charlist) {
      if (charlist === void 0) {
        charlist = "s";
      }
      return s.replace(new RegExp("[" + charlist + "]+$"), "");
    }
    function stringFormat(template, data) {
      var isArray2 = Array.isArray(data);
      return template.replace(stringFormatRegExp, function($0, $1) {
        return data[isArray2 ? parseInt($1) : $1];
      });
    }
    function escapeHtml(str) {
      return $("<div>").text(str).html();
    }
    function tryParseInt(value) {
      if (/^(\-|\+)?([0-9]+)$/.test(value)) {
        return Number(value);
      }
      return NaN;
    }
    function tryParseFloat(value) {
      if (/^(\-|\+)?([0-9]+(\.[0-9]+)?)$/.test(value)) {
        return Number(value);
      }
      return NaN;
    }
    function parseToLocalDate(date) {
      if (date instanceof Date)
        return date;
      var isUtc = /Z|[\+\-]\d\d:?\d\d/i.test(date);
      if (!isUtc) {
        date += "Z";
      }
      return new Date(date);
    }
    function adjustTimezone(date) {
      return new Date(
        Date.UTC(
          date.getFullYear(),
          date.getMonth(),
          date.getDate(),
          date.getHours(),
          date.getMinutes(),
          date.getSeconds(),
          date.getMilliseconds()
        )
      );
    }
    function unadjustTimezone(date) {
      return new Date(
        date.getUTCFullYear(),
        date.getUTCMonth(),
        date.getUTCDate(),
        date.getUTCHours(),
        date.getUTCMinutes(),
        date.getUTCSeconds(),
        date.getUTCMilliseconds()
      );
    }
    function areEqual(v1, v2) {
      if (v1 instanceof Date && v2 instanceof Date) {
        if (v1.getTime() !== v2.getTime()) {
          return false;
        }
      } else if (v1 !== v2) {
        return false;
      }
      return true;
    }
    function reportSourcesAreEqual(rs1, rs2) {
      if (rs1 && rs2 && rs1.report === rs2.report) {
        var params1String = "";
        if (rs1.parameters) {
          params1String = JSON.stringify(rs1.parameters);
        }
        var params2String = "";
        if (rs2.parameters) {
          params2String = JSON.stringify(rs2.parameters);
        }
        return params1String === params2String;
      }
      return false;
    }
    function areEqualArrays(array1, array2) {
      if (array1 === null) {
        if (array2 !== null) {
          return false;
        }
        return true;
      }
      if (array2 === null) {
        return false;
      }
      if (array1.length !== array2.length) {
        return false;
      }
      for (var j = array1.length - 1; j >= 0; j--) {
        if (!areEqual(array1[j], array2[j])) {
          return false;
        }
      }
      return true;
    }
    function isSvgSupported() {
      var matches = /Version\/(\d+.\d+.\d+) Safari/.exec(navigator.userAgent);
      if (matches && matches.length > 1) {
        var version = parseFloat(matches[1]);
        return version >= 6;
      }
      return true;
    }
    function isInternalServerError(error) {
      if (error) {
        return replaceAll(error, " ", "").toLowerCase() === "internalservererror";
      }
      return false;
    }
    function isSystemArgumentException(xhr) {
      var exceptionShortName = "ArgumentException";
      var exceptionInstance = getExceptionInstance(xhr);
      return isExceptionInstanceOfType(exceptionInstance, exceptionShortName, "System." + exceptionShortName);
    }
    function isInvalidClientException(xhr) {
      var exceptionShortName = "InvalidClientException";
      var exceptionInstance = getExceptionInstance(xhr);
      return isExceptionInstanceOfType(exceptionInstance, exceptionShortName, "Telerik.Reporting.Services.Engine." + exceptionShortName);
    }
    function isApplicationException(xhr) {
      return isApplicationExceptionInstance(getExceptionInstance(xhr));
    }
    function isApplicationExceptionInstance(exception) {
      var exceptionShortName = "DrawingFactoryUnavailableException";
      return isExceptionInstanceOfType(exception, exceptionShortName, "Telerik.Drawing.Contract." + exceptionShortName);
    }
    function isExceptionOfType(xhr, exceptionType) {
      return isExceptionInstanceOfType(getExceptionInstance(xhr), exceptionType, exceptionType);
    }
    function isExceptionInstanceOfType(exceptionInstance, exceptionTypeShortName, exceptionTypeFullName) {
      return exceptionInstance && exceptionInstance.exceptionType && exceptionTypeNamesMatch(exceptionInstance.exceptionType, exceptionTypeShortName, exceptionTypeFullName);
    }
    function exceptionTypeNamesMatch(instanceTypeName, exceptionTypeShortName, exceptionTypeFullName) {
      return instanceTypeName && (instanceTypeName === exceptionTypeFullName || instanceTypeName.endsWith(exceptionTypeShortName));
    }
    function parseJSON(json) {
      try {
        return JSON.parse(
          json,
          function(key, value) {
            if (key && value) {
              var firstChar = key.charAt(0);
              if (firstChar === firstChar.toUpperCase()) {
                var newPropertyName = firstChar.toLowerCase() + key.slice(1);
                this[newPropertyName] = value;
              }
            }
            return value;
          }
        );
      } catch (e) {
        return null;
      }
    }
    function getExceptionInstance(xhr) {
      if (!xhr || !xhr.responseText) {
        return false;
      }
      return parseJSON(xhr.responseText);
    }
    function extend() {
      var copy;
      var name;
      var options;
      var target;
      var i = 0;
      var length = arguments.length;
      target = length > 1 ? arguments[i++] || {} : {};
      for (; i < length; i++) {
        if ((options = arguments[i]) != null) {
          for (name in options) {
            target[name];
            copy = options[name];
            if (target === copy) {
              continue;
            }
            if (copy !== void 0) {
              target[name] = copy;
            }
          }
        }
      }
      return target;
    }
    function each(obj, callback) {
      var length;
      var i = 0;
      if (isArray(obj)) {
        length = obj.length;
        for (; i < length; i++) {
          if (callback.call(obj[i], i, obj[i]) === false) {
            break;
          }
        }
      } else {
        for (i in obj) {
          if (callback.call(obj[i], i, obj[i]) === false) {
            break;
          }
        }
      }
      return obj;
    }
    function selector() {
      return document.querySelectorAll(arguments[0]);
    }
    function isArray(obj) {
      if (Array.isArray(obj))
        return true;
      var length = Boolean(obj) && "length" in obj && obj.length;
      if (typeof length === "number") {
        return true;
      }
      return false;
    }
    function loadScriptWithCallback(src, done, version) {
      var js = document.createElement("script");
      js.src = src;
      js.onload = function() {
        done(version);
      };
      js.onerror = function() {
        logError(new Error("Failed to load script " + src));
      };
      document.head.appendChild(js);
    }
    function loadScript(url) {
      var ajaxOptions = {
        dataType: "script",
        cache: true
      };
      return $ajax(url, ajaxOptions);
    }
    function filterUniqueLastOccurrence(array) {
      function onlyLastUnique(value, index, self) {
        return self.lastIndexOf(value) === index;
      }
      return array.filter(onlyLastUnique);
    }
    function logError(error) {
      var console = window.console;
      if (console && console.error) {
        console.error(error);
      }
    }
    function findElement(selectorChain) {
      if (selectorChain.constructor != Array) {
        selectorChain = [selectorChain];
      }
      var $area = $(selectorChain[0]);
      for (var i = 1; i < selectorChain.length; i++) {
        $area = $area.find(selectorChain[i]);
      }
      return $area;
    }
    function toRgbColor(hexColor) {
      if (hexColor && hexColor.length < 6) {
        var index = 1;
        var hexParts = hexColor.split("");
        if (hexParts[0] !== "#") {
          index = 0;
        }
        for (index; index < hexParts.length; index++) {
          hexParts[index] = hexParts[index] + hexParts[index];
        }
        hexColor = hexParts.join("");
      }
      var result = /^#?([a-f\d]{2})([a-f\d]{2})([a-f\d]{2})$/i.exec(hexColor);
      return result ? parseInt(result[1], 16) + ", " + parseInt(result[2], 16) + ", " + parseInt(result[3], 16) : null;
    }
    function isRgbColor(color) {
      if (!color) {
        return false;
      }
      return color.indexOf(",") > -1 ? true : false;
    }
    function getColorAlphaValue(color) {
      if (color.toLowerCase() === "transparent") {
        return 0;
      }
      if (!isRgbColor(color)) {
        return 1;
      }
      if (color.indexOf("#") !== -1) {
        color = toRgbColor(color);
      }
      var colorComponents = color.split(",").map(function(c) {
        return c.trim();
      });
      var alpha = colorComponents.length === 4 ? parseFloat((parseFloat(colorComponents[3].replace(/[()]/g, "")) / 255).toFixed(2)) : 1;
      return alpha;
    }
    function $ajax(url, ajaxSettings) {
      return new Promise(function(resolve, reject) {
        $.ajax(url, ajaxSettings).done(function(data) {
          return resolve(data);
        }).fail(function(xhr, status, error) {
          reject(toXhrErrorData(xhr, status, error));
        });
      });
    }
    function type(obj) {
      return Object.prototype.toString.call(obj).slice(8, -1);
    }
    function isEqual(a, b) {
      if (a === b) {
        return true;
      }
      if (a !== a && b !== b) {
        return true;
      }
      if (a !== a && b === b || a === a && b !== b) {
        return false;
      }
      const typeA = type(a);
      const typeB = type(b);
      if (typeA !== typeB) {
        return false;
      }
      if (typeA === "Array") {
        if (a.length !== b.length) {
          return false;
        }
        const length = a.length;
        for (let i = 0; i < length; i++) {
          if (!isEqual(a[i], b[i])) {
            return false;
          }
        }
        return true;
      }
      if (typeA === "Object") {
        const keysA = Object.keys(a);
        const keysB = Object.keys(b);
        if (keysA.length !== keysB.length) {
          return false;
        }
        const length = keysA.length;
        for (let i = 0; i < length; i++) {
          const key = keysA[i];
          if (!isEqual(a[key], b[key])) {
            return false;
          }
        }
        return true;
      }
      if (typeA === "Date") {
        return a.getTime() === b.getTime();
      }
      return false;
    }

    var utils = /*#__PURE__*/Object.freeze({
        __proto__: null,
        isSpecialKey: isSpecialKey,
        rectangle: rectangle,
        generateGuidString: generateGuidString,
        trim: trim,
        replaceAll: replaceAll,
        lTrim: lTrim,
        rTrim: rTrim,
        stringFormat: stringFormat,
        escapeHtml: escapeHtml,
        tryParseInt: tryParseInt,
        tryParseFloat: tryParseFloat,
        parseToLocalDate: parseToLocalDate,
        adjustTimezone: adjustTimezone,
        unadjustTimezone: unadjustTimezone,
        areEqual: areEqual,
        reportSourcesAreEqual: reportSourcesAreEqual,
        areEqualArrays: areEqualArrays,
        isSvgSupported: isSvgSupported,
        isInternalServerError: isInternalServerError,
        isSystemArgumentException: isSystemArgumentException,
        isInvalidClientException: isInvalidClientException,
        isApplicationException: isApplicationException,
        isApplicationExceptionInstance: isApplicationExceptionInstance,
        isExceptionOfType: isExceptionOfType,
        exceptionTypeNamesMatch: exceptionTypeNamesMatch,
        parseJSON: parseJSON,
        getExceptionInstance: getExceptionInstance,
        extend: extend,
        each: each,
        selector: selector,
        isArray: isArray,
        loadScriptWithCallback: loadScriptWithCallback,
        loadScript: loadScript,
        filterUniqueLastOccurrence: filterUniqueLastOccurrence,
        logError: logError,
        findElement: findElement,
        toRgbColor: toRgbColor,
        isRgbColor: isRgbColor,
        getColorAlphaValue: getColorAlphaValue,
        $ajax: $ajax,
        type: type,
        isEqual: isEqual
    });

    function toPixels(value) {
      return parseInt(value, 10) || 0;
    }
    function getMargins(dom) {
      var $target = $(dom);
      return {
        left: toPixels($target.css("marginLeft")),
        right: toPixels($target.css("marginRight")),
        top: toPixels($target.css("marginTop")),
        bottom: toPixels($target.css("marginBottom"))
      };
    }
    function getPadding(dom) {
      var $target = $(dom);
      return {
        left: toPixels($target.css("paddingLeft")),
        right: toPixels($target.css("paddingRight")),
        top: toPixels($target.css("paddingTop")),
        bottom: toPixels($target.css("paddingBottom"))
      };
    }
    function getBorderWidth(dom) {
      var $target = $(dom);
      return {
        left: toPixels($target.css("borderLeftWidth")),
        right: toPixels($target.css("borderRightWidth")),
        top: toPixels($target.css("borderTopWidth")),
        bottom: toPixels($target.css("borderBottomWidth"))
      };
    }
    function scale(dom, scaleX, scaleY, originX, originY) {
      scaleX = scaleX || 1;
      scaleY = scaleY || 1;
      originX = originX || 0;
      originY = originY || 0;
      var scale2 = stringFormat("scale({0}, {1})", [scaleX, scaleY]);
      var origin = stringFormat("{0} {1}", [originX, originY]);
      $(dom).css("transform", scale2).css("transform-origin", origin);
    }

    var domUtils = /*#__PURE__*/Object.freeze({
        __proto__: null,
        getMargins: getMargins,
        getPadding: getPadding,
        getBorderWidth: getBorderWidth,
        scale: scale
    });

    var _a;
    var sr$1 = {
      // warning and error string resources
      controllerNotInitialized: "Controller is not initialized.",
      noReportInstance: "No report instance.",
      missingTemplate: "!obsolete resource!",
      noReport: "No report.",
      noReportDocument: "No report document.",
      missingOrInvalidParameter: "There are missing or invalid parameter values. Please input valid data for the following parameters:\n",
      invalidParameter: "Please input a valid value.",
      invalidDateTimeValue: "Please input a valid date.",
      parameterIsEmpty: "Parameter value cannot be empty.",
      cannotValidateType: "Cannot validate parameter of type {type}.",
      loadingFormats: "Loading...",
      loadingReport: "Loading report...",
      loadingParameters: "Loading parameters...",
      autoRunDisabled: "Please validate the report parameter values and press Preview to generate the report.",
      preparingDownload: "Preparing document to download. Please wait...",
      preparingPrint: "Preparing document to print. Please wait...",
      errorLoadingTemplates: "Error loading the report viewer's templates. (templateUrl = '{0}').",
      errorServiceUrl: "Cannot access the Reporting REST service. (serviceUrl = '{0}'). Make sure the service address is correct and enable CORS if needed. (https://enable-cors.org)",
      errorServiceVersion: "The version of the Report Viewer '{1}' does not match the version of the Reporting REST Service '{0}'. Please make sure both are running same version.",
      loadingReportPagesInProgress: "{0} pages loaded so far...",
      loadedReportPagesComplete: "Done. Total {0} pages loaded.",
      noPageToDisplay: "No page to display.",
      errorDeletingReportInstance: "Error deleting report instance: '{0}'.",
      errorRegisteringViewer: "Error registering the viewer with the service.",
      noServiceClient: "No serviceClient has been specified for this controller.",
      errorRegisteringClientInstance: "Error registering client instance.",
      errorCreatingReportInstance: "Error creating report instance (Report = '{0}').",
      errorCreatingReportDocument: "Error creating report document (Report = '{0}'; Format = '{1}').",
      unableToGetReportParameters: "Unable to get report parameters.",
      errorObtainingAuthenticationToken: "Error obtaining authentication token.",
      clientExpired: "Click 'Refresh' to restore client session.",
      promisesChainStopError: "Error shown. Throwing promises chain stop error.",
      renderingCanceled: "Report processing was canceled.",
      tryReportPreview: "The report may now be previewed.",
      // viewer template string resources
      parameterEditorSelectNone: "clear selection",
      parameterEditorSelectAll: "select all",
      parametersAreaPreviewButton: "Preview",
      menuNavigateBackwardText: "Navigate Backward",
      menuNavigateBackwardTitle: "Navigate Backward",
      menuNavigateForwardText: "Navigate Forward",
      menuNavigateForwardTitle: "Navigate Forward",
      menuStopRenderingText: "Stop Rendering",
      menuStopRenderingTitle: "Stop Rendering",
      menuRefreshText: "Refresh",
      menuRefreshTitle: "Refresh",
      menuFirstPageText: "First Page",
      menuFirstPageTitle: "First Page",
      menuLastPageText: "Last Page",
      menuLastPageTitle: "Last Page",
      menuPreviousPageTitle: "Previous Page",
      menuNextPageTitle: "Next Page",
      menuPageNumberTitle: "Page Number Selector",
      menuDocumentMapTitle: "Toggle Document Map",
      menuParametersAreaTitle: "Toggle Parameters Area",
      menuZoomInTitle: "Zoom In",
      menuZoomOutTitle: "Zoom Out",
      menuPageStateTitle: "Toggle FullPage/PageWidth",
      menuPrintText: "Print...",
      menuContinuousScrollText: "Toggle Continuous Scrolling",
      menuSendMailText: "Send an email",
      menuPrintTitle: "Print",
      menuContinuousScrollTitle: "Toggle Continuous Scrolling",
      menuSendMailTitle: "Send an email",
      menuExportText: "Export",
      menuExportTitle: "Export",
      menuPrintPreviewText: "Toggle Print Preview",
      menuPrintPreviewTitle: "Toggle Print Preview",
      menuSearchText: "Search",
      menuSearchTitle: "Toggle Search",
      menuSideMenuTitle: "Toggle Side Menu",
      sendEmailFromLabel: "From:",
      sendEmailToLabel: "To:",
      sendEmailCCLabel: "CC:",
      sendEmailSubjectLabel: "Subject:",
      sendEmailFormatLabel: "Format:",
      sendEmailSendLabel: "Send",
      sendEmailCancelLabel: "Cancel",
      // accessibility string resources
      ariaLabelPageNumberSelector: "Page number selector. Showing page {0} of {1}.",
      ariaLabelPageNumberEditor: "Page number editor",
      ariaLabelExpandable: "Expandable",
      ariaLabelSelected: "Selected",
      ariaLabelParameter: "parameter",
      ariaLabelErrorMessage: "Error message",
      ariaLabelParameterInfo: "Contains {0} options",
      ariaLabelMultiSelect: "Multiselect",
      ariaLabelMultiValue: "Multivalue",
      ariaLabelSingleValue: "Single value",
      ariaLabelParameterDateTime: "DateTime",
      ariaLabelParameterString: "String",
      ariaLabelParameterNumerical: "Numerical",
      ariaLabelParameterBoolean: "Boolean",
      ariaLabelParametersAreaPreviewButton: "Preview the report",
      ariaLabelMainMenu: "Main menu",
      ariaLabelCompactMenu: "Compact menu",
      ariaLabelSideMenu: "Side menu",
      ariaLabelDocumentMap: "Document map area",
      ariaLabelDocumentMapSplitter: "Document map area splitbar.",
      ariaLabelParametersAreaSplitter: "Parameters area splitbar.",
      ariaLabelPagesArea: "Report contents area",
      ariaLabelSearchDialogArea: "Search area",
      ariaLabelSendEmailDialogArea: "Send email area",
      ariaLabelSearchDialogStop: "Stop search",
      ariaLabelSearchDialogOptions: "Search options",
      ariaLabelSearchDialogNavigateUp: "Navigate up",
      ariaLabelSearchDialogNavigateDown: "Navigate down",
      ariaLabelSearchDialogMatchCase: "Match case",
      ariaLabelSearchDialogMatchWholeWord: "Match whole word",
      ariaLabelSearchDialogUseRegex: "Use regex",
      ariaLabelMenuNavigateBackward: "Navigate backward",
      ariaLabelMenuNavigateForward: "Navigate forward",
      ariaLabelMenuStopRendering: "Stop rendering",
      ariaLabelMenuRefresh: "Refresh",
      ariaLabelMenuFirstPage: "First page",
      ariaLabelMenuLastPage: "Last page",
      ariaLabelMenuPreviousPage: "Previous page",
      ariaLabelMenuNextPage: "Next page",
      ariaLabelMenuPageNumber: "Page number selector",
      ariaLabelMenuDocumentMap: "Toggle document map",
      ariaLabelMenuParametersArea: "Toggle parameters area",
      ariaLabelMenuZoomIn: "Zoom in",
      ariaLabelMenuZoomOut: "Zoom out",
      ariaLabelMenuPageState: "Toggle FullPage/PageWidth",
      ariaLabelMenuPrint: "Print",
      ariaLabelMenuContinuousScroll: "Continuous scrolling",
      ariaLabelMenuSendMail: "Send an email",
      ariaLabelMenuExport: "Export",
      ariaLabelMenuPrintPreview: "Toggle print preview",
      ariaLabelMenuSearch: "Search in report contents",
      ariaLabelMenuSideMenu: "Toggle side menu",
      ariaLabelSendEmailFrom: "From email address",
      ariaLabelSendEmailTo: "Recipient email address",
      ariaLabelSendEmailCC: "Carbon Copy email address",
      ariaLabelSendEmailSubject: "Email subject:",
      ariaLabelSendEmailFormat: "Report format:",
      ariaLabelSendEmailSend: "Send email",
      ariaLabelSendEmailCancel: "Cancel sending email",
      // search dialog resources
      searchDialogTitle: "Search in report contents",
      searchDialogSearchInProgress: "searching...",
      searchDialogNoResultsLabel: "No results",
      searchDialogResultsFormatLabel: "Result {0} of {1}",
      searchDialogStopTitle: "Stop Search",
      searchDialogNavigateUpTitle: "Navigate Up",
      searchDialogNavigateDownTitle: "Navigate Down",
      searchDialogMatchCaseTitle: "Match Case",
      searchDialogMatchWholeWordTitle: "Match Whole Word",
      searchDialogUseRegexTitle: "Use Regex",
      searchDialogCaptionText: "Find",
      searchDialogPageText: "page",
      // Send Email dialog resources
      sendEmailDialogTitle: "Send Email",
      sendEmailValidationEmailRequired: "Email field is required",
      sendEmailValidationEmailFormat: "Email format is not valid",
      sendEmailValidationSingleEmail: "The field accepts a single email address only",
      sendEmailValidationFormatRequired: "Format field is required",
      errorSendingDocument: "Error sending report document (Report = '{0}')."
    };
    window.telerikReportViewer || (window.telerikReportViewer = {});
    (_a = window.telerikReportViewer).sr || (_a.sr = sr$1);

    var sr = sr || {};
    const userResources = (window.telerikReportViewer || {}).sr || {};
    const stringResources = $.extend({}, sr, userResources);

    var defaultOptions$7 = {};
    function Accessibility(options) {
      var controller;
      var pageInitialized = false;
      var areas;
      var lastArea;
      var keyMap = {
        CONFIRM_KEY: 13,
        // C
        CONTENT_AREA_KEY: 67,
        // D
        DOCUMENT_MAP_AREA_KEY: 68,
        // M
        MENU_AREA_KEY: 77,
        // P
        PARAMETERS_AREA_KEY: 80
      };
      options = $.extend({}, defaultOptions$7, options);
      controller = options.controller;
      if (!controller) {
        throw "No controller (telerikReporting.ReportViewerController) has been specified.";
      }
      controller.reportLoadComplete(function() {
        setAccessibilityUI();
        var content = findContentArea();
        if (content.length > 0) {
          content.focus();
        }
      }).pageReady(function(event, page) {
        initPage(page);
        pageInitialized = true;
      }).error(function(e, message) {
        focusOnErrorMessage();
        window.setTimeout(setAccessibilityUI, 500);
      });
      controller.updateUI(function(e) {
        if (pageInitialized) {
          setPageSelector();
          decorateMenuItems();
        }
      });
      function setAccessibilityUI() {
        if (!areas) {
          initAreas();
          $(document.body).off("keydown", processKeyDown);
          $(document.body).on("keydown", processKeyDown);
        }
      }
      function focusOnErrorMessage() {
        var $errMsg = $("div.trv-pages-area div.trv-error-message");
        if ($errMsg.length === 0) {
          return;
        }
        $errMsg.attr("tabIndex", 0);
        $errMsg.focus();
      }
      function initPage(page) {
        if (!page) {
          return;
        }
        setAccessibilityUI();
        var area = areas[keyMap.CONTENT_AREA_KEY];
        setContentAreaKeyDown(area);
      }
      function setPageSelector() {
        var pagers = document.querySelectorAll(".trv-report-pager");
        var pageNumber = controller.getCurrentPageNumber();
        var pageCount = controller.getPageCount();
        pagers.forEach((pager) => {
          pager.setAttribute("aria-label", stringFormat(stringResources.ariaLabelPageNumberSelector, [pageNumber, pageCount]));
          var pageInputs = pager.querySelectorAll("input[data-role=telerik_ReportViewer_PageNumberInput]");
          pageInputs.forEach((input) => {
            input.setAttribute("aria-label", stringResources.ariaLabelPageNumberEditor);
          });
        });
      }
      function initAreas() {
        areas = {};
        areas[keyMap.DOCUMENT_MAP_AREA_KEY] = findDocumentMapArea();
        areas[keyMap.MENU_AREA_KEY] = findMenuArea();
        areas[keyMap.CONTENT_AREA_KEY] = findContentArea();
        var parametersArea = findParametersArea();
        if (parametersArea) {
          areas[keyMap.PARAMETERS_AREA_KEY] = parametersArea;
          setParameterEditorsKeyDown(parametersArea);
        }
      }
      function findContentArea() {
        return $("div[data-role=telerik_ReportViewer_PagesArea]");
      }
      function findDocumentMapArea() {
        return $("div[data-role=telerik_ReportViewer_DocumentMapArea] div[data-role=treeview]");
      }
      function findMenuArea() {
        return $("ul[data-role=telerik_ReportViewer_MainMenu]");
      }
      function findParametersArea() {
        return $("div[data-role=telerik_ReportViewer_ParametersArea] div.trv-parameters-area-content");
      }
      function processKeyDown(event) {
        if (!areas) {
          return;
        }
        if (!(event.altKey && event.ctrlKey)) {
          return;
        }
        var currentArea = areas[event.which];
        if (!currentArea) {
          return;
        }
        if (!IsAreaContainerVisible(currentArea.parent())) {
          return;
        }
        var className = "k-focus";
        if (lastArea) {
          lastArea.removeClass(className);
        }
        currentArea.addClass(className);
        currentArea.focus();
        lastArea = currentArea;
        event.preventDefault();
      }
      function setParameterEditorsKeyDown(parametersAreaContent) {
        if (parametersAreaContent.length === 0) {
          return;
        }
        var $paramsArea = parametersAreaContent.parent("div[data-role=telerik_ReportViewer_ParametersArea]");
        if (!IsAreaContainerVisible($paramsArea)) {
          return;
        }
        Array.from(parametersAreaContent.children()).forEach((child) => {
          $(child).on("keydown", (event) => {
            if (event.which == keyMap.CONFIRM_KEY) {
              var paramsButton = $paramsArea.find("button.trv-parameters-area-preview-button");
              paramsButton.focus();
              event.preventDefault();
            }
          });
        });
      }
      function IsAreaContainerVisible(container) {
        return container && !(container.hasClass("k-collapsed") || container.hasClass("trv-hidden"));
      }
      function setContentAreaKeyDown(contentArea) {
        if (!contentArea) {
          return;
        }
        var actions = contentArea.find("div [data-reporting-action]");
        if (!actions.length > 0) {
          return;
        }
        Array.from(actions).forEach((action) => {
          var $action = $(action);
          $action.on("keydown", (event) => {
            if (event.which == keyMap.CONFIRM_KEY) {
              $action.trigger("click");
            }
          });
        });
      }
      function decorateMenuItems() {
        var menuAreas = areas[keyMap.MENU_AREA_KEY];
        if (!menuAreas) {
          return;
        }
        Array.from(menuAreas).forEach((menu) => {
          var $menu = $(menu);
          var menuItems = $menu.children("li.k-item");
          Array.from(menuItems).forEach((menuItem) => {
            var $menuItem = $(menuItem);
            if (!$menuItem.hasClass("trv-report-pager")) {
              var ariaLabel = $menuItem.attr("aria-label");
              var expandableSr = stringFormat(". {0}", [stringResources.ariaLabelExpandable]);
              var expandable = $menuItem.find("ul").length > 0 && ariaLabel.indexOf(expandableSr) < 0 ? expandableSr : "";
              var selectedSr = stringFormat(". {0}", [stringResources.ariaLabelSelected]);
              var selected = $menuItem.hasClass("k-selected") && ariaLabel.indexOf(selectedSr) < 0 ? selectedSr : "";
              var label = ariaLabel + expandable + selected;
              $menuItem.attr("aria-label", label);
              if ($menuItem.hasClass("k-disabled")) {
                $menuItem.attr("aria-disabled", "true");
              } else {
                $menuItem.removeAttr("aria-disabled");
              }
            }
          });
        });
      }
      function setKeyMap(keyMapValues) {
        keyMap = keyMapValues;
        areas = void 0;
      }
      function getKeyMap() {
        return keyMap;
      }
      return {
        getKeyMap,
        setKeyMap
      };
    }

    const GlobalSettings = {
      viewerInstances: []
    };

    class Binder {
      static bind($element, ...args) {
        const commands = args[0].commands;
        const viewerOptions = args[1];
        Binder.attachCommands($element, commands, viewerOptions);
        var plugins = $element.find('[data-role^="telerik_ReportViewer_"]');
        Array.from(plugins).forEach((element) => {
          var $element2 = $(element);
          var fn = $.fn[$element2.attr("data-role")];
          if (typeof fn === "function") {
            fn.apply($element2, args);
          }
        });
      }
      static attachCommands($element, commands, viewerOptions) {
        var elementSelector = '[data-command^="telerik_ReportViewer_"]';
        var customElementSelector = "[data-target-report-viewer]" + elementSelector;
        $element.on("click", elementSelector, commandHandler);
        if (!GlobalSettings.CommandHandlerAttached) {
          $(document.body).on("click", customElementSelector, customCommandHandler);
          GlobalSettings.CommandHandlerAttached = true;
        }
        Object.entries(commands).forEach(([key, command]) => {
          attachCommand(key, command, viewerOptions, $element);
        });
        function commandHandler(event) {
          var prefixedDataCommand = $(this).attr("data-command");
          if (prefixedDataCommand) {
            var dataCommand = prefixedDataCommand.substring("telerik_ReportViewer_".length);
            var cmd = commands[dataCommand];
            if (cmd && cmd.enabled()) {
              cmd.exec($(this).attr("data-command-parameter"));
            }
            event.preventDefault();
          }
        }
        function customCommandHandler(event) {
          var $this = $(this);
          var prefixedDataCommand = $this.attr("data-command");
          var reportViewerTarget = $this.attr("data-target-report-viewer");
          if (prefixedDataCommand && reportViewerTarget) {
            var dataCommand = prefixedDataCommand.substring("telerik_ReportViewer_".length);
            var reportViewer = $(reportViewerTarget).data("telerik_ReportViewer");
            var cmd = reportViewer.commands[dataCommand];
            if (cmd.enabled()) {
              cmd.exec($(this).attr("data-command-parameter"));
            }
            event.preventDefault();
          }
        }
      }
    }
    function attachCommand(dataCommand, cmd, viewerOptions, $element) {
      if (cmd) {
        var elementSelector = '[data-command="telerik_ReportViewer_' + dataCommand + '"]';
        var customElementSelector = '[data-target-report-viewer="' + viewerOptions.selector + '"]' + elementSelector;
        var $defaultElement = $element.find(elementSelector);
        var $customElement = $(customElementSelector);
        $(cmd).on("enabledChanged", function(event) {
          (cmd.enabled() ? $.fn.removeClass : $.fn.addClass).call($defaultElement.parent("li"), "k-disabled");
          (cmd.enabled() ? $.fn.removeClass : $.fn.addClass).call($customElement, viewerOptions.disabledButtonClass);
        }).on("checkedChanged", function(event) {
          var defaultTarget = $defaultElement.parent("li");
          (cmd.checked() ? $.fn.addClass : $.fn.removeClass).call(defaultTarget, getSelectedClassName(defaultTarget));
          (cmd.checked() ? $.fn.addClass : $.fn.removeClass).call($customElement, viewerOptions.checkedButtonClass);
        });
      }
    }
    function getSelectedClassName($element) {
      return $element.hasClass("trv-menu-toggleable") ? "k-selected !k-bg-primary" : "k-selected";
    }

    const ViewModes = {
      INTERACTIVE: "INTERACTIVE",
      PRINT_PREVIEW: "PRINT_PREVIEW"
    };
    const PrintModes = {
      AUTO_SELECT: "AUTO_SELECT",
      FORCE_PDF_PLUGIN: "FORCE_PDF_PLUGIN",
      FORCE_PDF_FILE: "FORCE_PDF_FILE"
    };
    const PageModes = {
      SINGLE_PAGE: "SINGLE_PAGE",
      CONTINUOUS_SCROLL: "CONTINUOUS_SCROLL"
    };
    const ScaleModes = {
      FIT_PAGE_WIDTH: "FIT_PAGE_WIDTH",
      FIT_PAGE: "FIT_PAGE",
      SPECIFIC: "SPECIFIC"
    };
    const ParameterTypes = {
      INTEGER: "System.Int64",
      FLOAT: "System.Double",
      STRING: "System.String",
      DATETIME: "System.DateTime",
      BOOLEAN: "System.Boolean"
    };
    const ParameterEditorTypes = {
      COMBO_BOX: "COMBO_BOX",
      LIST_VIEW: "LIST_VIEW"
    };
    const ParametersAreaPositions = {
      RIGHT: "RIGHT",
      LEFT: "LEFT",
      TOP: "TOP",
      BOTTOM: "BOTTOM"
    };
    const DocumentMapAreaPositions = {
      RIGHT: "RIGHT",
      LEFT: "LEFT"
    };

    function Command(execCallback) {
      var enabledState = true;
      var checkedState = false;
      var cmd = {
        enabled: function(state) {
          if (arguments.length === 0) {
            return enabledState;
          }
          var newState = Boolean(state);
          enabledState = newState;
          $(this).trigger("enabledChanged");
          return cmd;
        },
        checked: function(state) {
          if (arguments.length === 0) {
            return checkedState;
          }
          var newState = Boolean(state);
          checkedState = newState;
          $(this).trigger("checkedChanged");
          return cmd;
        },
        exec: execCallback
      };
      return cmd;
    }

    var scaleTransitionMap = {};
    scaleTransitionMap[ScaleModes.FIT_PAGE] = { scaleMode: ScaleModes.FIT_PAGE_WIDTH };
    scaleTransitionMap[ScaleModes.FIT_PAGE_WIDTH] = { scaleMode: ScaleModes.SPECIFIC, scale: 1 };
    scaleTransitionMap[ScaleModes.SPECIFIC] = { scaleMode: ScaleModes.FIT_PAGE };
    var scaleValues = [0.1, 0.25, 0.5, 0.75, 1, 1.5, 2, 4, 8];
    function CommandSet(options) {
      var controller = options.controller;
      if (!controller) {
        throw "No options.controller.";
      }
      var historyManager = options.history;
      if (!historyManager) {
        throw "No options.history.";
      }
      function getDocumentMapVisible() {
        var args = {};
        controller.getDocumentMapState(args);
        return Boolean(args.visible);
      }
      function getParametersAreaVisible() {
        var args = {};
        controller.getParametersAreaState(args);
        return Boolean(args.visible);
      }
      function getSideMenuVisible() {
        var args = {};
        controller.getSideMenuVisible(args);
        return Boolean(args.visible);
      }
      function getSearchDialogVisible() {
        var args = {};
        controller.getSearchDialogState(args);
        return Boolean(args.visible);
      }
      function getSendEmailDialogVisible() {
        var args = {};
        controller.getSendEmailDialogState(args);
        return Boolean(args.visible);
      }
      return {
        "historyBack": new Command(function() {
          historyManager.back();
        }),
        "historyForward": new Command(function() {
          historyManager.forward();
        }),
        "stopRendering": new Command(function() {
          controller.stopRendering();
        }),
        "goToPrevPage": new Command(function() {
          controller.navigateToPage(controller.getCurrentPageNumber() - 1);
        }),
        "goToNextPage": new Command(function() {
          controller.navigateToPage(controller.getCurrentPageNumber() + 1);
        }),
        "goToFirstPage": new Command(function() {
          controller.navigateToPage(1);
        }),
        "goToLastPage": new Command(function() {
          controller.navigateToPage(controller.getPageCount());
        }),
        "goToPage": new Command(function(pageNumber) {
          if (!isNaN(pageNumber)) {
            var pageCount = controller.getPageCount();
            if (pageNumber > pageCount) {
              pageNumber = pageCount;
            } else if (pageNumber < 1) {
              pageNumber = 1;
            }
            controller.navigateToPage(pageNumber);
            return pageNumber;
          }
        }),
        "refresh": new Command(
          function() {
            controller.refreshReport(true, false);
          }
        ),
        "export": new Command(function(format) {
          if (format) {
            controller.exportReport(format);
          }
        }),
        "print": new Command(function() {
          controller.printReport();
        }),
        "pageMode": new Command(function() {
          controller.setPageMode(
            controller.getPageMode() === PageModes.SINGLE_PAGE ? PageModes.CONTINUOUS_SCROLL : PageModes.SINGLE_PAGE
          );
        }),
        "togglePrintPreview": new Command(function() {
          controller.setViewMode(
            controller.getViewMode() === ViewModes.PRINT_PREVIEW ? ViewModes.INTERACTIVE : ViewModes.PRINT_PREVIEW
          );
        }),
        "toggleDocumentMap": new Command(function() {
          controller.setDocumentMapVisible({ visible: !getDocumentMapVisible() });
        }),
        "toggleParametersArea": new Command(function() {
          controller.setParametersAreaVisible({ visible: !getParametersAreaVisible() });
        }),
        "zoom": new Command(function(scale) {
          controller.setScale(scale);
        }),
        "zoomIn": new Command(function() {
          zoom(1);
        }),
        "zoomOut": new Command(function() {
          zoom(-1);
        }),
        "toggleSideMenu": new Command(function() {
          controller.setSideMenuVisible({ visible: !getSideMenuVisible() });
        }),
        "toggleZoomMode": new Command(function(e) {
          var scale = controller.getScale();
          var scaleMode = controller.getScaleMode();
          controller.setScale(scaleTransitionMap[scaleMode].scale || scale);
          controller.setScaleMode(scaleTransitionMap[scaleMode].scaleMode);
        }),
        "toggleSearchDialog": new Command(function() {
          controller.setSearchDialogVisible({ visible: !getSearchDialogVisible() });
        }),
        "toggleSendEmailDialog": new Command(function() {
          controller.setSendEmailDialogVisible({ visible: !getSendEmailDialogVisible() });
        })
      };
      function zoom(step) {
        controller.setScale(getZoomScale(controller.getScale(), step));
      }
      function getZoomScale(scale, steps) {
        var pos = -1;
        var length = scaleValues.length;
        for (var i = 0; i < length; i++) {
          var value = scaleValues[i];
          if (scale < value) {
            pos = i - 0.5;
            break;
          }
          if (scale === value) {
            pos = i;
            break;
          }
        }
        pos = pos + steps;
        if (steps >= 0) {
          pos = Math.round(pos - 0.49);
        } else {
          pos = Math.round(pos + 0.49);
        }
        if (pos < 0) {
          pos = 0;
        } else if (pos > length - 1) {
          pos = length - 1;
        }
        return scaleValues[pos];
      }
    }

    function IEHelper() {
      function getPdfPlugin() {
        var classIds = ["AcroPDF.PDF.1", "PDF.PdfCtrl.6", "PDF.PdfCtrl.5"];
        var plugin = null;
        $.each(classIds, function(index, classId) {
          try {
            plugin = new ActiveXObject(classId);
            if (plugin) {
              return false;
            }
          } catch (ex) {
          }
        });
        return plugin;
      }
      return {
        hasPdfPlugin: function() {
          return getPdfPlugin() !== null;
        }
      };
    }
    function FirefoxHelper() {
      function hasPdfPlugin2() {
        var matches = /Firefox[\/\s](\d+\.\d+)/.exec(navigator.userAgent);
        if (null !== matches && matches.length > 1) {
          var version = parseFloat(matches[1]);
          if (version >= 19) {
            return false;
          }
        }
        var pdfPlugins = navigator.mimeTypes["application/pdf"];
        var pdfPlugin = pdfPlugins !== null ? pdfPlugins.enabledPlugin : null;
        if (pdfPlugin) {
          var description = pdfPlugin.description;
          return description.indexOf("Adobe") !== -1 && (description.indexOf("Version") === -1 || parseFloat(description.split("Version")[1]) >= 6);
        }
        return false;
      }
      return {
        hasPdfPlugin: function() {
          return hasPdfPlugin2();
        }
      };
    }
    function ChromiumHelper(defaultPlugin) {
      function hasPdfPlugin2() {
        var navPlugins = navigator.plugins;
        var found = false;
        $.each(navPlugins, function(key, value) {
          if (navPlugins[key].name === defaultPlugin || navPlugins[key].name === "Adobe Acrobat") {
            found = true;
            return false;
          }
        });
        return found;
      }
      return {
        hasPdfPlugin: function() {
          return hasPdfPlugin2();
        }
      };
    }
    function OtherBrowserHelper() {
      return {
        hasPdfPlugin: function() {
          return false;
        }
      };
    }
    function selectBrowserHelper() {
      if (window.navigator) {
        var userAgent = window.navigator.userAgent.toLowerCase();
        if (userAgent.indexOf("msie") > -1 || userAgent.indexOf("mozilla") > -1 && userAgent.indexOf("trident") > -1)
          return IEHelper();
        else if (userAgent.indexOf("firefox") > -1)
          return FirefoxHelper();
        else if (userAgent.indexOf("edg/") > -1)
          return ChromiumHelper("Microsoft Edge PDF Plugin");
        else if (userAgent.indexOf("chrome") > -1)
          return ChromiumHelper("Chrome PDF Viewer");
        else if (userAgent.indexOf("safari") > -1)
          return ChromiumHelper("WebKit built-in PDF");
        return OtherBrowserHelper();
      }
      return null;
    }
    var helper = selectBrowserHelper();
    var hasPdfPlugin = helper ? helper.hasPdfPlugin() : false;
    var PrintManager = function() {
      var iframe;
      function printDesktop(src) {
        var sameOriginUrl = null;
        if (!iframe) {
          iframe = document.createElement("iframe");
          iframe.style.display = "none";
          iframe.onload = function() {
            try {
              iframe.contentDocument.execCommand("print", true, null);
            } catch (e) {
              logError(e);
            } finally {
              if (sameOriginUrl) {
                (window.URL || window.webkitURL).revokeObjectURL(sameOriginUrl);
              }
            }
          };
        }
        if (isSameOriginUrl(src) && useMsBlobHandling()) {
          iframe.src = src;
          document.body.appendChild(iframe);
          return;
        }
        var request = new XMLHttpRequest();
        request.open("GET", src, true);
        request.responseType = "arraybuffer";
        request.onload = function(e) {
          if (this.status === 200) {
            var localPdf = new Blob([this.response], { type: "application/pdf" });
            if (useMsBlobHandling()) {
              window.navigator.msSaveOrOpenBlob(localPdf);
            } else {
              sameOriginUrl = (window.URL || window.webkitURL).createObjectURL(localPdf);
              iframe.src = sameOriginUrl;
              document.body.appendChild(iframe);
            }
          } else {
            console.log("Could not retrieve remote PDF document.");
          }
        };
        request.send();
      }
      function useMsBlobHandling() {
        return window.navigator && window.navigator.msSaveOrOpenBlob;
      }
      function isSameOriginUrl(url) {
        var location = window.location;
        var anchor = document.createElement("a");
        anchor.setAttribute("href", url);
        if (anchor.host == "") {
          anchor.href = anchor.href;
        }
        return location.hostname === anchor.hostname && location.protocol === anchor.protocol && location.port === anchor.port;
      }
      function printMobile(src) {
        window.open(src, "_self");
      }
      var isMobile = /Android|webOS|iPhone|iPad|iPod|BlackBerry|IEMobile|Opera Mini/i.test(navigator.userAgent);
      var printFunc = isMobile ? printMobile : printDesktop;
      return {
        print: function(src) {
          printFunc(src);
        },
        getDirectPrintState: function() {
          return hasPdfPlugin;
        }
      };
    }();

    var ParameterValidators = function() {
      var validators = {};
      function validateParameter(parameter, value, validatorFunc, compareFunc) {
        var values = [].concat(value).map(function(value1) {
          return checkAvailableValues(parameter, validatorFunc(value1), compareFunc);
        });
        if (parameter.multivalue) {
          if (value == null || value.length == 0) {
            if (parameter.allowNull) {
              return value;
            }
            throw stringResources.invalidParameter;
          }
          return values;
        }
        return values[0];
      }
      function isNull(parameter, value) {
        return parameter.allowNull && -1 != [null, "", void 0].indexOf(value);
      }
      function checkAvailableValues(parameter, value, compareFunc) {
        if (parameter.availableValues) {
          var found = Array.from(parameter.availableValues).some(function(av) {
            return compareFunc(value, av.value);
          });
          if (!found) {
            if (parameter.allowNull && !value) {
              return value;
            }
            throw stringResources.invalidParameter;
          }
        }
        return value;
      }
      validators[ParameterTypes.STRING] = {
        validate: function(parameter, value) {
          return validateParameter(
            parameter,
            value,
            function(value2) {
              if (!value2) {
                if (parameter.allowNull) {
                  return null;
                }
                if (parameter.allowBlank) {
                  return "";
                }
                throw stringResources.parameterIsEmpty;
              }
              return value2;
            },
            function(s1, s2) {
              return s1 == s2;
            }
          );
        }
      };
      validators[ParameterTypes.FLOAT] = {
        validate: function(parameter, value) {
          return validateParameter(
            parameter,
            value,
            function(value2) {
              var num = tryParseFloat(value2);
              if (isNaN(num)) {
                if (isNull(parameter, value2)) {
                  return null;
                }
                throw stringResources.parameterIsEmpty;
              }
              return num;
            },
            function(f1, f2) {
              return tryParseFloat(f1) == tryParseFloat(f2);
            }
          );
        }
      };
      validators[ParameterTypes.INTEGER] = {
        validate: function(parameter, value) {
          return validateParameter(
            parameter,
            value,
            function(value2) {
              var num = tryParseInt(value2);
              if (isNaN(num)) {
                if (isNull(parameter, value2)) {
                  return null;
                }
                throw stringResources.parameterIsEmpty;
              }
              return num;
            },
            function(n1, n2) {
              return tryParseInt(n1) == tryParseFloat(n2);
            }
          );
        }
      };
      validators[ParameterTypes.DATETIME] = {
        validate: function(parameter, value) {
          return validateParameter(
            parameter,
            value,
            function(value2) {
              if (parameter.allowNull && (value2 === null || value2 === "" || value2 === void 0)) {
                return null;
              }
              if (!isNaN(Date.parse(value2))) {
                if (parameter.availableValues) {
                  return value2;
                }
                return parseToLocalDate(value2);
              }
              throw stringResources.invalidDateTimeValue;
            },
            function(d1, d2) {
              d1 = parseToLocalDate(d1);
              d2 = parseToLocalDate(d2);
              return d1.getTime() == d2.getTime();
            }
          );
        }
      };
      validators[ParameterTypes.BOOLEAN] = {
        validate: function(parameter, value) {
          return validateParameter(
            parameter,
            value,
            function(value2) {
              if (-1 != ["true", "false"].indexOf(String(value2).toLowerCase())) {
                return Boolean(value2);
              }
              if (isNull(parameter, value2)) {
                return null;
              }
              throw stringResources.parameterIsEmpty;
            },
            function(b1, b2) {
              return Boolean(b1) == Boolean(b2);
            }
          );
        }
      };
      return {
        validate: function(parameter, value) {
          var v = validators[parameter.type];
          if (!v) {
            throw stringFormat(stringResources.cannotValidateType, parameter);
          }
          return v.validate(parameter, value);
        }
      };
    }();

    function isNumber(value) {
      return typeof value === "number" && isFinite(value);
    }
    function secondsToMilliseconds(seconds) {
      return seconds * 1e3;
    }
    function ServiceClientSentinel(serviceClient, clientID, sessionTimeoutSeconds) {
      if (!serviceClient) {
        throw stringResources.noServiceClient;
      }
      if (!isNumber(sessionTimeoutSeconds)) {
        throw "sessionTimeoutSeconds must a number and must be finite";
      }
      var interval;
      var pingMilliseconds;
      if (sessionTimeoutSeconds <= 120) {
        pingMilliseconds = secondsToMilliseconds(sessionTimeoutSeconds) / 2;
      } else {
        pingMilliseconds = secondsToMilliseconds(sessionTimeoutSeconds - 60);
      }
      function start() {
        if (pingMilliseconds <= 0) {
          return;
        }
        pingMilliseconds = Math.min(pingMilliseconds, 2147483647);
        interval = setInterval(function() {
          serviceClient.keepClientAlive(clientID);
        }, pingMilliseconds);
      }
      function restart() {
        stop();
        start();
      }
      function stop() {
        if (interval) {
          clearInterval(interval);
          interval = null;
        }
      }
      return {
        start,
        restart,
        stop
      };
    }

    var __defProp$6 = Object.defineProperty;
    var __defNormalProp$6 = (obj, key, value) => key in obj ? __defProp$6(obj, key, { enumerable: true, configurable: true, writable: true, value }) : obj[key] = value;
    var __publicField$5 = (obj, key, value) => {
      __defNormalProp$6(obj, typeof key !== "symbol" ? key + "" : key, value);
      return value;
    };
    class EventEmitter extends EventTarget {
      constructor() {
        super();
        __publicField$5(this, "_events");
        __publicField$5(this, "_eventsCount");
        this._events = {};
        this._eventsCount = 0;
      }
      /**
       * @param {string} type
       * @param {(event: CustomEvent, ...args: any[]) => void} listener
       * @returns
       */
      addListener(type, listener) {
        if (typeof listener !== "function") {
          throw new TypeError("listener must be a function");
        }
        if (!this._events[type]) {
          this._events[type] = [];
        }
        function wrappedListener(event) {
          listener.call(this, event, ...event.detail);
        }
        wrappedListener.listener = listener;
        this._events[type].push(wrappedListener);
        this._eventsCount++;
        this.addEventListener(type, wrappedListener.bind(this));
        return this;
      }
      /**
       * @alias addListener
       * @param {string} type
       * @param {(event: CustomEvent, ...args: any[]) => void} listener
       * @returns
       */
      on(type, listener) {
        return this.addListener(type, listener);
      }
      /**
       * @param {string} type
       * @param {any[]} args
       * @returns
       */
      trigger(type, ...args) {
        if (!this._events[type]) {
          return void 0;
        }
        const event = new CustomEvent(type, {
          detail: args,
          cancelable: true
        });
        return this.dispatchEvent(event);
      }
      /**
       * @alias trigger
       * @param {string} type
       * @param {any[]} args
       * @returns
       */
      emit(type, ...args) {
        return this.trigger(type, ...args);
      }
      /**
       * @param {string} type
       * @param {(event: CustomEvent, ...args: any[]) => void} listener
       * @returns
       */
      removeListener(type, listener) {
        if (!this._events[type]) {
          return this;
        }
        this._events[type] = this._events[type].filter((wrappedListener) => {
          if (wrappedListener.listener !== listener) {
            return true;
          }
          this.removeEventListener(type, wrappedListener);
          return false;
        });
        if (this._events[type].length === 0) {
          delete this._events[type];
          this._eventsCount--;
        }
        return this;
      }
      /**
       * @param {string} type
       * @returns
       */
      removeAllListeners(type) {
        if (type === void 0) {
          Object.keys(this._events).forEach((eventType) => {
            this.removeAllListeners(eventType);
          });
          return this;
        }
        if (this._events[type]) {
          this._events[type].forEach((wrappedListener) => {
            this.removeEventListener(type, wrappedListener);
          });
          delete this._events[type];
          this._eventsCount--;
        }
        return this;
      }
      /**
       * @param {string} type
       * @param {(event: CustomEvent, ...args: any[]) => void} listener
       * @returns
       */
      off(type, listener) {
        if (type === void 0) {
          return this.removeAllListeners();
        }
        if (listener === void 0) {
          return this.removeAllListeners(type);
        }
        return this.removeListener(type, listener);
      }
    }

    var defaultOptions$6 = {
      pagePollIntervalMs: 500,
      documentInfoPollIntervalMs: 2e3
    };
    function ReportViewerController(options) {
      var controller = {};
      var clientId;
      var reportInstanceId;
      var reportDocumentId;
      var registerClientPromise;
      var registerInstancePromise;
      var documentFormatsPromise;
      var report;
      var parameterValues;
      var processedParameterValues;
      var currentPageNumber;
      var pageCount;
      var viewMode = ViewModes.INTERACTIVE;
      var pageMode = PageModes.CONTINUOUS_SCROLL;
      var loader;
      var printMode = PrintModes.AUTO_SELECT;
      var bookmarkNodes;
      var renderingExtensions;
      var clientHasExpired = false;
      var parameterValidators = ParameterValidators;
      var eventEmitter = new EventEmitter();
      var serviceClientSentinel;
      clearReportState();
      options = $.extend({}, defaultOptions$6, options);
      var settings = options.settings;
      if (typeof settings.getPrintMode === "function") {
        printMode = settings.getPrintMode();
      }
      var client = options.serviceClient;
      if (!client) {
        throw stringResources.noServiceClient;
      }
      clientId = settings.getClientId();
      initializeAndStartSentinel();
      function setClientId(id) {
        clientId = id;
        stopSentinel();
        settings.setClientId(clientId);
        initializeAndStartSentinel();
      }
      function clearClientId() {
        clientId = null;
        registerClientPromise = null;
        stopSentinel();
        serviceClientSentinel = null;
        settings.setClientId(null);
      }
      function initializeAndStartSentinel() {
        if (typeof settings.getKeepClientAlive !== "function") {
          throw new Error("Required settings.getKeepClientAlive() is not supplied for ReportViewerController");
        }
        if (settings.getKeepClientAlive() && clientId) {
          return client.getClientsSessionTimeoutSeconds().then(function(sessionTimeout) {
            serviceClientSentinel = new ServiceClientSentinel(client, clientId, sessionTimeout);
            serviceClientSentinel.start();
          });
        }
      }
      function stopSentinel() {
        if (settings.getKeepClientAlive() && !!serviceClientSentinel) {
          serviceClientSentinel.stop();
        }
      }
      function getFormat() {
        if (viewMode === ViewModes.PRINT_PREVIEW) {
          return "HTML5";
        }
        return "HTML5Interactive";
      }
      function handleRequestError(xhrData, localizedMessage, suppressErrorBubbling) {
        var errorMessage = "";
        if (!xhrData.xhr) {
          errorMessage = xhrData;
          controller.raiseError(formatXhrError({ "responseText": errorMessage }, null, null, null));
          throw errorMessage;
        }
        var xhr = xhrData.xhr;
        if (isInvalidClientException(xhr)) {
          onClientExpired();
        }
        var formattedError = formatXhrError(
          xhr,
          xhrData.status,
          isInternalServerError(xhrData.error) ? "" : xhrData.error,
          localizedMessage
        );
        controller.raiseError(formattedError);
        if (!suppressErrorBubbling) {
          if (isApplicationException(xhr)) {
            var exception = getExceptionInstance(xhr);
            if (exception) {
              throw exception;
            }
          }
          throw xhr.responseJSON && xhr.responseJSON.exceptionMessage ? xhr.responseJSON.exceptionMessage : stringResources.promisesChainStopError;
        }
      }
      function initializeClientAsync() {
        if (!registerClientPromise) {
          registerClientPromise = client.registerClient().catch(function(xhrErrorData) {
            handleRequestError(xhrErrorData, stringResources.errorRegisteringViewer);
          }).then(function(clientId2) {
            setClientId(clientId2);
          }).catch(clearClientId);
        }
        return registerClientPromise;
      }
      function registerInstanceAsync() {
        if (!registerInstancePromise) {
          registerInstancePromise = createReportInstanceAsync(report, parameterValues).then(function(instanceId) {
            reportInstanceId = instanceId;
          });
          registerInstancePromise.catch(function(errorMessage) {
            registerInstancePromise = null;
          });
        }
        return registerInstancePromise;
      }
      function clearReportState() {
        reportDocumentId = null;
        reportInstanceId = null;
        registerInstancePromise = null;
        resetPageNumbers();
      }
      function resetPageNumbers() {
        currentPageNumber = pageCount = 0;
      }
      function formatError(args) {
        var len = args.length;
        if (len === 1) {
          return args[0];
        }
        if (len > 1) {
          return stringFormat(args[0], Array.prototype.slice.call(args, 1));
        }
        return "";
      }
      function createReportInstanceAsync(report2, parameterValues2) {
        throwIfNotInitialized();
        return client.createReportInstance(clientId, report2, parameterValues2).catch(function(xhrErrorData) {
          handleRequestError(xhrErrorData, stringFormat(stringResources.errorCreatingReportInstance, [escapeHtml(report2)]));
        });
      }
      function registerDocumentAsync(format, deviceInfo, useCache, baseDocumentId, actionId) {
        throwIfNotInitialized();
        throwIfNoReportInstance();
        return client.createReportDocument(clientId, reportInstanceId, format, deviceInfo, useCache, baseDocumentId, actionId).catch(function(xhrErrorData) {
          handleRequestError(xhrErrorData, stringFormat(stringResources.errorCreatingReportDocument, [escapeHtml(report), escapeHtml(format)]));
        });
      }
      function sendDocumentAsync(documentId, args) {
        throwIfNotInitialized();
        throwIfNoReportInstance();
        return client.sendDocument(clientId, reportInstanceId, documentId, args).catch(function(xhrErrorData) {
          handleRequestError(xhrErrorData, stringFormat(stringResources.errorSendingDocument, [escapeHtml(report)]));
        });
      }
      function getDocumentInfoRecursive(clientId2, instanceId, documentId, options2) {
        if (!options2.isCanceled && instanceId === reportInstanceId) {
          return client.getDocumentInfo(clientId2, instanceId, documentId).catch(handleRequestError).then(function(info) {
            if (info && info.documentReady) {
              return info;
            }
            info["promise"] = new Promise(function(resolve, reject) {
              window.setTimeout(resolve, options2.documentInfoPollIntervalMs);
            }).then(function() {
              return getDocumentInfoRecursive(clientId2, instanceId, documentId, options2);
            });
            return info;
          });
        }
        return Promise.reject();
      }
      function ReportLoader(reportHost, useCache, baseDocumentId, actionId) {
        var loaderOptions = {};
        function onReportDocumentRegistered(id) {
          if (reportHost) {
            reportDocumentId = id;
            onBeginLoadReport();
            getReportDocumentReady();
          }
        }
        function onBeforeLoadReport(args) {
          loaderOptions.documentInfoPollIntervalMs = options.pagePollIntervalMs;
          if (reportHost) {
            reportHost.beforeLoadReport(args);
          }
        }
        function onBeginLoadReport() {
          if (reportHost) {
            reportHost.beginLoadReport();
          }
        }
        function onReportLoadComplete(info) {
          if (reportHost) {
            reportHost.onReportLoadComplete(info);
          }
        }
        function onReportLoadProgress(info) {
          if (reportHost) {
            pageCount = info.pageCount;
            reportHost.reportLoadProgress(info);
          }
        }
        function getReportDocumentReady() {
          throwIfNotInitialized();
          throwIfNoReportInstance();
          throwIfNoReportDocument();
          progressReportDocumentReady(getDocumentInfoRecursive(clientId, reportInstanceId, reportDocumentId, loaderOptions));
        }
        function progressReportDocumentReady(getDocumentReadyPromise) {
          getDocumentReadyPromise.then(function(info) {
            if (info.documentReady) {
              onReportLoadComplete(info);
            } else {
              onReportLoadProgress(info);
              progressReportDocumentReady(info.promise);
            }
          });
        }
        function getPageAsync(pageNo) {
          return new Promise(function(resolve, reject) {
            var fn = function() {
              client.getPage(clientId, reportInstanceId, reportDocumentId, pageNo).catch(handleRequestError).then(function(pageInfo) {
                if (pageInfo.pageReady) {
                  resolve(pageInfo);
                } else {
                  window.setTimeout(fn, options.pagePollIntervalMs);
                }
              });
            };
            fn();
          });
        }
        function onBeginLoadPage(pageNo) {
          if (reportHost) {
            reportHost.beginLoadPage(pageNo);
          }
        }
        var loadPromise;
        function loadAsync() {
          if (!loadPromise) {
            var format = getFormat();
            var deviceInfo = createPreviewDeviceInfo();
            onBeforeLoadReport({ deviceInfo });
            loadPromise = initializeClientAsync().then(registerInstanceAsync).then(function() {
              return registerDocumentAsync(format, deviceInfo, useCache, baseDocumentId, actionId);
            }).then(onReportDocumentRegistered);
          }
          return loadPromise;
        }
        function createPreviewDeviceInfo() {
          var deviceInfo = createDeviceInfo();
          deviceInfo.ContentOnly = true;
          deviceInfo.UseSVG = isSvgSupported();
          return deviceInfo;
        }
        return {
          beginLoad: function() {
            loadAsync();
          },
          beginGetPage: function(pageNo) {
            throwIfNotInitialized();
            loadAsync().then(function() {
              onBeginLoadPage(pageNo);
              return getPageAsync(pageNo);
            }).then(function(pageInfo) {
              loaderOptions.documentInfoPollIntervalMs = options.documentInfoPollIntervalMs;
              if (reportHost) {
                reportHost.pageReady(pageInfo);
              }
            });
          },
          getPageData: function(pageNo) {
            throwIfNotInitialized();
            return loadAsync().then(function() {
              return getPageAsync(pageNo);
            });
          },
          dispose: function() {
            reportHost = null;
          },
          cancel: function() {
            loaderOptions.isCanceled = true;
          }
        };
      }
      function createDeviceInfo() {
        var deviceInfo = {};
        if (settings.getEnableAccessibility()) {
          deviceInfo.enableAccessibility = true;
          deviceInfo.contentTabIndex = settings.contentTabIndex;
        }
        var args = {};
        controller.getSearchDialogState(args);
        var searchInitiated = args.visible;
        var searchMetadataOnDemand = settings.getSearchMetadataOnDemand();
        var enableSearch = !searchMetadataOnDemand || searchInitiated;
        deviceInfo.enableSearch = enableSearch;
        return deviceInfo;
      }
      function tryResolveClientErrorByExceptionType(exceptionType) {
        var parameterExceptionShortName = "InvalidParameterException";
        return exceptionTypeNamesMatch(exceptionType, parameterExceptionShortName, "Telerik.Reporting.Services.Engine." + parameterExceptionShortName) ? stringResources.missingOrInvalidParameter : "";
      }
      function formatXhrError(xhr, status, error, localizedMessage) {
        var parsedXhr = parseJSON(xhr.responseText);
        var result = "";
        if (parsedXhr) {
          var errorMessage = tryResolveClientErrorByExceptionType(parsedXhr.exceptionType || parsedXhr.error);
          if (errorMessage) {
            return errorMessage;
          }
          result = escapeHtml(parsedXhr.message);
          var exceptionMessage = escapeHtml(parsedXhr.exceptionMessage || parsedXhr.error_description);
          if (exceptionMessage) {
            if (result) {
              result += " " + exceptionMessage;
            } else {
              result = exceptionMessage;
            }
          }
        } else {
          result = escapeHtml(xhr.responseText);
        }
        if (localizedMessage || error) {
          if (result) {
            result = " " + result;
          }
          result = escapeHtml(localizedMessage ? localizedMessage : error) + result;
        }
        if (isInvalidClientException(xhr)) {
          result += "<br />" + stringResources.clientExpired;
        }
        return result;
      }
      function getReportPage(pageNo) {
        if (loader) {
          loader.beginGetPage(pageNo);
        }
      }
      function loadReportAsync(ignoreCache, baseDocumentId, actionId) {
        if (!report) {
          controller.raiseError(stringResources.noReport);
          return;
        }
        if (loader) {
          loader.dispose();
          loader = null;
        }
        clearReportState();
        loader = new ReportLoader(controller, !ignoreCache, baseDocumentId, actionId);
        loader.beginLoad();
      }
      function printReport() {
        throwIfNoReport();
        var deviceInfo = {
          ImmediatePrint: true
        };
        var printStartArgs = {
          deviceInfo,
          handled: false
        };
        controller.printStarted(printStartArgs);
        if (!printStartArgs.handled) {
          controller.setUIState({
            operationName: "PrintInProgress",
            inProgress: true
          });
          controller.showNotification({ stringResources: "preparingPrint" });
          var canUsePlugin = getCanUsePlugin();
          var contentDisposition = canUsePlugin ? "inline" : "attachment";
          var queryString = "response-content-disposition=" + contentDisposition;
          exportAsync("PDF", deviceInfo).then(
            function(info) {
              var url = client.formatDocumentUrl(info.clientId, info.instanceId, info.documentId, queryString);
              var printEndArgs = {
                url,
                handled: false
              };
              controller.printReady(printEndArgs);
              controller.hideNotification();
              controller.setUIState({
                operationName: "PrintInProgress",
                inProgress: false
              });
              if (!printEndArgs.handled) {
                PrintManager.print(url);
              }
            }
          );
        }
      }
      function getCanUsePlugin() {
        switch (printMode) {
          case PrintModes.FORCE_PDF_FILE:
          case false:
            return false;
          case PrintModes.FORCE_PDF_PLUGIN:
          case true:
            return true;
          default:
            return PrintManager.getDirectPrintState();
        }
      }
      function exportReport(format, deviceInfo) {
        throwIfNoReport();
        if (!deviceInfo) {
          deviceInfo = createDeviceInfo();
        }
        var exportStartArgs = {
          format,
          deviceInfo,
          handled: false
        };
        controller.exportStarted(exportStartArgs);
        if (!exportStartArgs.handled) {
          var queryString = "response-content-disposition=attachment";
          controller.setUIState({
            operationName: "ExportInProgress",
            inProgress: true
          });
          controller.showNotification({ stringResources: "preparingDownload" });
          exportAsync(format, exportStartArgs.deviceInfo).then(
            function(info) {
              var url = client.formatDocumentUrl(info.clientId, info.instanceId, info.documentId, queryString);
              var exportEndArgs = {
                url,
                format,
                handled: false,
                windowOpenTarget: "_self"
              };
              controller.exportReady(exportEndArgs);
              controller.hideNotification();
              controller.setUIState({
                operationName: "ExportInProgress",
                inProgress: false
              });
              if (!exportEndArgs.handled) {
                window.open(url, exportEndArgs.windowOpenTarget);
              }
            }
          );
        }
      }
      function sendReport(args) {
        throwIfNoReport();
        if (!args.deviceInfo) {
          args.deviceInfo = createDeviceInfo();
        }
        var sendEmailStartArgs = {
          deviceInfo: args.deviceInfo,
          handled: false,
          format: args.format
        };
        controller.sendEmailStarted(sendEmailStartArgs);
        var queryString = "response-content-disposition=attachment";
        if (!sendEmailStartArgs.handled) {
          exportAsync(args.format, args.deviceInfo).then(
            function(info) {
              var url = client.formatDocumentUrl(info.clientId, info.instanceId, info.documentId, queryString);
              args["url"] = url;
              args["handled"] = false;
              controller.sendEmailReady(args);
              delete args.deviceInfo;
              if (!args.handled) {
                sendDocumentAsync(info.documentId, args);
              }
            }
          );
        }
      }
      function exportAsync(format, deviceInfo) {
        throwIfNoReport();
        return initializeClientAsync().then(registerInstanceAsync).then(function() {
          return registerDocumentAsync(format, deviceInfo, true, reportDocumentId);
        }).then(function(documentId) {
          return waitReportDocumentReady(clientId, reportInstanceId, documentId, options);
        });
      }
      function waitReportDocumentReady(clientId2, reportInstanceId2, documentId, options2) {
        return new Promise(function(resolve, reject) {
          var fn = function(promise) {
            promise.then(function(info) {
              if (info.documentReady) {
                resolve({
                  clientId: clientId2,
                  instanceId: reportInstanceId2,
                  documentId
                });
              } else {
                fn(info.promise);
              }
            });
          };
          fn(getDocumentInfoRecursive(clientId2, reportInstanceId2, documentId, options2));
        });
      }
      function execServerAction(actionId) {
        throwIfNoReport();
        throwIfNoReportInstance();
        throwIfNoReportDocument();
        onServerActionStarted();
        controller.refreshReportCore(false, reportDocumentId, actionId);
      }
      function throwIfNotInitialized() {
        if (!clientId) {
          throw stringResources.controllerNotInitialized;
        }
      }
      function throwIfNoReportInstance() {
        if (!reportInstanceId) {
          throw stringResources.noReportInstance;
        }
      }
      function throwIfNoReportDocument() {
        if (!reportDocumentId) {
          throw stringResources.noReportDocument;
        }
      }
      function throwIfNoReport() {
        if (!report) {
          throw stringResources.noReport;
        }
      }
      function eventFactory(event, args) {
        if (typeof args[0] === "function") {
          eventEmitter.on(event, args[0]);
        } else {
          eventEmitter.trigger(event, ...args);
        }
        return controller;
      }
      function loadParametersAsync(report2, paramValues) {
        return initializeClientAsync().then(function() {
          return client.getParameters(clientId, report2, paramValues || parameterValues || {}).catch(function(xhrErrorData) {
            handleRequestError(xhrErrorData, stringResources.unableToGetReportParameters);
          });
        });
      }
      function getDocumentFormatsAsync() {
        if (renderingExtensions) {
          return Promise.resolve(renderingExtensions);
        }
        if (!documentFormatsPromise) {
          documentFormatsPromise = client.getDocumentFormats().catch(handleRequestError);
        }
        return documentFormatsPromise;
      }
      function getServiceVersionAsync() {
        return client.getServiceVersion().catch(handleRequestError);
      }
      function getPageForBookmark(nodes, id) {
        if (nodes) {
          for (var i = 0, len = nodes.length; i < len; i++) {
            var node = nodes[i];
            if (node.id === id) {
              return node.page;
            }
            var page = getPageForBookmark(node.items, id);
            if (page) {
              return page;
            }
          }
        }
        return null;
      }
      function fixDataContractJsonSerializer(arr) {
        var dict = {};
        if (Array.isArray(arr)) {
          arr.forEach(function(pair) {
            dict[pair.Key] = pair.Value;
          });
        }
        return dict;
      }
      function changeReportSource(rs) {
        setStateReportSource(rs);
        controller.reportSourceChanged();
      }
      function setStateReportSource(rs) {
        if (typeof settings.setReportSource === "function") {
          settings.setReportSource(rs);
        }
      }
      function changePageNumber(pageNr) {
        settings.setPageNumber(pageNr);
        controller.currentPageChanged();
      }
      var actionHandlers = {
        "sorting": function(action) {
          execServerAction(action.Id);
        },
        "toggleVisibility": function(action) {
          execServerAction(action.Id);
        },
        "navigateToReport": function(action) {
          var args = action.Value;
          onServerActionStarted();
          controller.setReportSource({
            report: args.Report,
            parameters: fixDataContractJsonSerializer(args.ParameterValues)
          });
          controller.refreshReport(false, true);
        },
        "navigateToUrl": function(action) {
          var args = action.Value;
          window.open(args.Url, args.Target);
        },
        "navigateToBookmark": function(action) {
          var id = action.Value;
          var page = getPageForBookmark(bookmarkNodes, id);
          controller.navigateToPage(page, {
            type: "bookmark",
            id
          });
        },
        "customAction": function(action) {
        }
      };
      function onInteractiveActionExecuting(interactiveActionArgs) {
        controller.interactiveActionExecuting(interactiveActionArgs);
      }
      function executeReportAction(interactiveActionArgs) {
        var action = interactiveActionArgs.action;
        var handler = actionHandlers[action.Type];
        if (typeof handler === "function") {
          window.setTimeout(function() {
            onInteractiveActionExecuting(interactiveActionArgs);
            if (!interactiveActionArgs.cancel) {
              handler(action);
            }
          }, 0);
        }
      }
      function onServerActionStarted() {
        controller.serverActionStarted();
      }
      function onReportActionEnter(args) {
        controller.interactiveActionEnter({
          action: args.action,
          element: args.element
        });
      }
      function onReportActionLeave(args) {
        controller.interactiveActionLeave({
          action: args.action,
          element: args.element
        });
      }
      function onClientExpired() {
        clientHasExpired = true;
        controller.clientExpired();
      }
      function onReportToolTipOpening(args) {
        controller.toolTipOpening(args);
      }
      function getSearchResultsAsync(args) {
        if (!args.searchToken || args.searchToken === "") {
          return Promise.resolve(null);
        }
        return client.getSearchResults(clientId, reportInstanceId, reportDocumentId, args.searchToken, args.matchCase, args.matchWholeWord, args.useRegex).catch(handleSearchResultsError);
      }
      function handleSearchResultsError(xhrData) {
        if (!isSystemArgumentException(xhrData.xhr)) {
          handleRequestError(xhrData, null, true);
          throw null;
        }
        var exceptionDetails = parseJSON(xhrData.xhr.responseText);
        throw exceptionDetails.exceptionMessage;
      }
      function appendInvalidReportParameterNames(invalidParameters) {
        var errorElement = document.querySelector(".trv-pages-area .trv-error-message");
        var invalidParametersHolder = document.createElement("ul");
        for (var index = 0; index < invalidParameters.length; index++) {
          var invalidParameterText = document.createElement("li");
          invalidParameterText.classList.add(invalidParameters[index].name);
          invalidParameterText.innerText = stringFormat("{0} ({1})", [invalidParameters[index].text, invalidParameters[index].name]);
          invalidParametersHolder.appendChild(invalidParameterText);
        }
        errorElement.appendChild(invalidParametersHolder);
      }
      controller.Events = {
        ERROR: "error",
        EXPORT_STARTED: "exportStarted",
        EXPORT_DOCUMENT_READY: "exportDocumentReady",
        PRINT_STARTED: "printStarted",
        PRINT_DOCUMENT_READY: "printDocumentReady",
        BEFORE_LOAD_PARAMETERS: "beforeLoadParameters",
        LOADED_REPORT_CHANGE: "loadedReportChange",
        BEFORE_LOAD_REPORT: "beforeLoadReport",
        BEGIN_LOAD_REPORT: "beginLoadReport",
        REPORT_LOAD_COMPLETE: "reportLoadComplete",
        REPORT_LOAD_PROGRESS: "reportLoadProgress",
        REPORT_LOAD_FAIL: "reportLoadFail",
        BEGIN_LOAD_PAGE: "beginLoadPage",
        PAGE_READY: "pageReady",
        VIEW_MODE_CHANGED: "viewModeChanged",
        PAGE_MODE_CHANGED: "pageModeChanged",
        PRINT_MODE_CHANGED: "printModeChanged",
        REPORT_SOURCE_CHANGED: "reportSourceChanged",
        NAVIGATE_TO_PAGE: "navigateToPage",
        CURRENT_PAGE_CHANGED: "currentPageChanged",
        GET_DOCUMENT_MAP_STATE: "trv.GET_DOCUMENT_MAP_STATE",
        SET_DOCUMENT_MAP_VISIBLE: "trv.SET_DOCUMENT_MAP_VISIBLE",
        GET_PARAMETER_AREA_STATE: "trv.GET_PARAMETER_AREA_STATE",
        SET_PARAMETER_AREA_VISIBLE: "trv.SET_PARAMETER_AREA_VISIBLE",
        SCALE_CHANGED: "scaleChanged",
        SCALE_MODE_CHANGED: "scaleModeChanged",
        SERVER_ACTION_STARTED: "serverActionStarted",
        SET_TOGGLE_SIDE_MENU: "trv.SET_TOGGLE_SIDE_MENU",
        GET_TOGGLE_SIDE_MENU: "trv.GET_TOGGLE_SIDE_MENU",
        UPDATE_UI: "trv.UPDATE_UI",
        CSS_LOADED: "cssLoaded",
        RELOAD_PARAMETERS: "reloadParameters",
        PARAMETERS_LOADED: "parametersLoaded",
        INTERACTIVE_ACTION_EXECUTING: "interactiveActionExecuting",
        INTERACTIVE_ACTION_ENTER: "interactiveActionEnter",
        INTERACTIVE_ACTION_LEAVE: "interactiveActionLeave",
        UPDATE_UI_INTERNAL: "trv.UPDATE_UI_INTERNAL",
        CLIENT_EXPIRED: "clientExpired",
        TOOLTIP_OPENING: "tooltipOpening",
        TOOLTIP_CLOSING: "tooltipClosing",
        PAGE_NUMBER: "trv.PAGE_NUMBER",
        PAGE_COUNT: "trv.PAGE_COUNT",
        GET_SEARCH_DIALOG_STATE: "trv.GET_SEARCH_DIALOG_STATE",
        GET_SEND_EMAIL_DIALOG_STATE: "trv.GET_SEND_EMAIL_DIALOG_STATE",
        SET_SEARCH_DIALOG_VISIBLE: "trv.SET_SEARCH_DIALOG_VISIBLE",
        SET_SEND_EMAIL_DIALOG_VISIBLE: "trv.SET_SEND_EMAIL_DIALOG_VISIBLE",
        SEND_EMAIL_STARTED: "sendEmailStarted",
        SEND_EMAIL_READY: "sendEmailDocumentReady",
        SHOW_NOTIFICATION: "trv.SHOW_NOTIFICATION",
        HIDE_NOTIFICATION: "trv.HIDE_NOTIFICATION",
        UI_STATE: "trv.UI_STATE",
        SCROLL_PAGE_READY: "scrollPageReady",
        UPDATE_SCROLL_PAGE_DIMENSIONS_READY: "trv.UPDATE_SCROLL_PAGE_DIMENSIONS_READY",
        MISSING_OR_INVALID_PARAMETERS: "missingOrInvalidParameters",
        RENDERING_STOPPED: "renderingStopped"
      };
      $.extend(
        controller,
        {
          getPageData: function(pageNumber) {
            if (loader) {
              return loader.getPageData(pageNumber);
            }
            return;
          },
          getReportSource: function() {
            if (report === null) {
              return null;
            }
            return {
              report,
              parameters: $.extend({}, parameterValues)
            };
          },
          setReportSource: function(rs) {
            if (rs === null) {
              report = parameterValues = null;
              clearReportState();
              changeReportSource(rs);
              return this;
            }
            report = rs.report;
            controller.setParameters(rs.parameters);
            changeReportSource(rs);
            return this;
          },
          updateSettings: function(settings2) {
            options.settings = $.extend({}, settings2, options.settings);
          },
          clearReportSource: function() {
            report = parameterValues = null;
            clearReportState();
            changeReportSource(void 0);
            return this;
          },
          getReportDocumentId: function() {
            return reportDocumentId;
          },
          setReportDocumentId: function(documentId) {
            reportDocumentId = documentId;
          },
          setParameters: function(paramValues) {
            parameterValues = paramValues;
            controller.parametersLoaded();
          },
          setProcessedParameter: function(processedParamValues) {
            processedParameterValues = processedParamValues;
          },
          getPageCount: function() {
            return pageCount;
          },
          getCurrentPageNumber: function() {
            return currentPageNumber;
          },
          setCurrentPageNumber: function(pageNo) {
            var num = tryParseInt(pageNo);
            if (num !== currentPageNumber) {
              currentPageNumber = num;
              changePageNumber(num);
            }
            return this;
          },
          getScale: function() {
            return settings.getScale();
          },
          setScale: function(scale) {
            if (settings.getScale() !== scale) {
              settings.setScale(scale);
              controller.scaleChanged(scale);
              controller.setScaleMode(ScaleModes.SPECIFIC);
            }
            return controller;
          },
          getScaleMode: function() {
            return settings.getScaleMode();
          },
          setScaleMode: function(scaleMode) {
            if (settings.getScaleMode() !== scaleMode) {
              settings.setScaleMode(scaleMode);
              controller.scaleModeChanged(scaleMode);
            }
            return controller;
          },
          getViewMode: function() {
            return viewMode;
          },
          setViewMode: function(vm) {
            if (viewMode !== vm) {
              viewMode = vm;
              controller.viewModeChanged(vm);
              if (report) {
                controller.refreshReportCore(false, reportDocumentId);
              }
            }
            return controller;
          },
          getPageMode: function() {
            return pageMode;
          },
          setPageMode: function(psm) {
            if (pageMode !== psm) {
              pageMode = psm;
              controller.pageModeChanged(psm);
              if (report) {
                controller.refreshReportCore(false, reportDocumentId);
              }
            }
            return controller;
          },
          getPrintMode: function() {
            return printMode;
          },
          setPrintMode: function(pm) {
            if (printMode !== pm) {
              printMode = pm;
              controller.printModeChanged(pm);
            }
            return controller;
          },
          previewReport: function(ignoreCache, baseDocumentId, actionId) {
            controller.loadedReportChange();
            controller.respectReportAutoRun = false;
            controller.refreshReportCore(ignoreCache, baseDocumentId, actionId);
          },
          refreshReportCore: function(ignoreCache, baseDocumentId, actionId) {
            loadReportAsync(ignoreCache, baseDocumentId, actionId);
          },
          stopRendering: function() {
            throwIfNoReport();
            throwIfNoReportInstance();
            throwIfNoReportDocument();
            client.deleteReportDocument(clientId, reportInstanceId, reportDocumentId).catch(handleRequestError).then(function() {
              if (loader) {
                loader.cancel();
              }
              resetPageNumbers();
              controller.renderingStopped();
            });
          },
          getReportParameters: function() {
            if (!parameterValues) {
              return [];
            }
            var paramsToBeExposed = {};
            for (var key in processedParameterValues) {
              var processedParam = processedParameterValues[key];
              var paramValue = parameterValues[key];
              if (processedParam && processedParam.availableValues) {
                if (processedParam.multivalue) {
                  paramsToBeExposed[key] = addMultiComboParam(processedParam, paramValue, key);
                } else {
                  paramsToBeExposed[key] = addSingleComboParam(processedParam, paramValue, key);
                }
              } else {
                paramsToBeExposed[key] = paramValue;
              }
            }
            return paramsToBeExposed;
          },
          refreshReport: function(ignoreCache, respectAutoRun, baseDocumentId, actionId) {
            controller.loadedReportChange();
            if (clientHasExpired) {
              clientHasExpired = false;
              clearClientId();
            }
            if (!report) {
              controller.raiseError(stringResources.noReport);
              return;
            }
            var loadParametersPromise = controller.loadParameters(null);
            loadParametersPromise.then(function(parameters) {
              var parameterValues2 = {};
              var invalidParameters = [];
              var hasError = false;
              Array.from(parameters || []).forEach((parameter) => {
                try {
                  var value = parameterValidators.validate(parameter, parameter.value);
                  parameterValues2[parameter.id] = value;
                } catch (e) {
                  hasError = true;
                  invalidParameters.push(parameter);
                  return;
                }
              });
              if (hasError) {
                controller.raiseError(stringResources.missingOrInvalidParameter);
                appendInvalidReportParameterNames(invalidParameters);
                controller.missingOrInvalidParameters();
              } else {
                controller.respectReportAutoRun = respectAutoRun;
                controller.setParameters(parameterValues2);
                if (controller.isReportAutoRun()) {
                  controller.refreshReportCore(ignoreCache, baseDocumentId, actionId);
                } else {
                  controller.showNotification({ stringResources: "autoRunDisabled" });
                }
              }
            });
            controller.reloadParameters(loadParametersPromise);
          },
          isReportAutoRun: function() {
            var _a;
            var reportAutoRunParameter = true;
            if (parameterValues) {
              reportAutoRunParameter = (_a = parameterValues.trv_AutoRun) != null ? _a : true;
            }
            return !controller.respectReportAutoRun || reportAutoRunParameter;
          },
          exportReport: function(format, deviceInfo) {
            exportReport(format, deviceInfo);
          },
          sendReport: function(args) {
            sendReport(args);
          },
          printReport: function() {
            printReport();
          },
          getReportPage: function(pageNumber) {
            getReportPage(pageNumber);
          },
          executeReportAction: function(interactiveActionArgs) {
            executeReportAction(interactiveActionArgs);
          },
          reportActionEnter: function(args) {
            onReportActionEnter(args);
          },
          reportActionLeave: function(args) {
            onReportActionLeave(args);
          },
          reportToolTipOpening: function(args) {
            onReportToolTipOpening(args);
          },
          loadParameters: function(paramValues) {
            if (report === null) {
              return {};
            }
            controller.beforeLoadParameters(paramValues === null);
            return loadParametersAsync(report, paramValues);
          },
          getDocumentFormats: function() {
            return getDocumentFormatsAsync();
          },
          getServiceVersion: function() {
            return getServiceVersionAsync();
          },
          setAuthenticationToken: function(token) {
            client.setAccessToken(token);
          },
          clientId: function() {
            return clientId;
          },
          onReportLoadComplete: function(info) {
            pageCount = info.pageCount;
            bookmarkNodes = info.bookmarkNodes;
            renderingExtensions = info.renderingExtensions;
            setStateReportSource(controller.getReportSource());
            controller.reportLoadComplete(info);
          },
          raiseError: function(...args) {
            var errorMessage = formatError(args);
            controller.error(errorMessage);
          },
          getSearchResults: function(args, results) {
            return getSearchResultsAsync(args);
          },
          // --- E V E N T S ---
          on: function(eventName, handler) {
            eventEmitter.on(eventName, handler);
            return controller;
          },
          trigger: function(eventName, ...args) {
            eventEmitter.trigger(eventName, ...args);
            return controller;
          },
          showNotification: function(...args) {
            return eventFactory(controller.Events.SHOW_NOTIFICATION, args);
          },
          hideNotification: function(...args) {
            return eventFactory(controller.Events.HIDE_NOTIFICATION, args);
          },
          setUIState: function(...args) {
            return eventFactory(controller.Events.UI_STATE, args);
          },
          error: function(...args) {
            return eventFactory(controller.Events.ERROR, args);
          },
          reloadParameters: function(...args) {
            return eventFactory(controller.Events.RELOAD_PARAMETERS, args);
          },
          parametersLoaded: function(...args) {
            return eventFactory(controller.Events.PARAMETERS_LOADED, args);
          },
          exportStarted: function(...args) {
            return eventFactory(controller.Events.EXPORT_STARTED, args);
          },
          exportReady: function(...args) {
            return eventFactory(controller.Events.EXPORT_DOCUMENT_READY, args);
          },
          sendEmailStarted: function(...args) {
            return eventFactory(controller.Events.SEND_EMAIL_STARTED, args);
          },
          sendEmailReady: function(...args) {
            return eventFactory(controller.Events.SEND_EMAIL_READY, args);
          },
          printStarted: function(...args) {
            return eventFactory(controller.Events.PRINT_STARTED, args);
          },
          printReady: function(...args) {
            return eventFactory(controller.Events.PRINT_DOCUMENT_READY, args);
          },
          beforeLoadParameters: function(...args) {
            return eventFactory(controller.Events.BEFORE_LOAD_PARAMETERS, args);
          },
          loadedReportChange: function(...args) {
            return eventFactory(controller.Events.LOADED_REPORT_CHANGE, args);
          },
          beforeLoadReport: function(...args) {
            return eventFactory(controller.Events.BEFORE_LOAD_REPORT, args);
          },
          beginLoadReport: function(...args) {
            return eventFactory(controller.Events.BEGIN_LOAD_REPORT, args);
          },
          reportLoadComplete: function(...args) {
            return eventFactory(controller.Events.REPORT_LOAD_COMPLETE, args);
          },
          reportLoadProgress: function(...args) {
            return eventFactory(controller.Events.REPORT_LOAD_PROGRESS, args);
          },
          reportLoadFail: function(...args) {
            return eventFactory(controller.Events.REPORT_LOAD_FAIL, args);
          },
          beginLoadPage: function(...args) {
            return eventFactory(controller.Events.BEGIN_LOAD_PAGE, args);
          },
          pageReady: function(...args) {
            return eventFactory(controller.Events.PAGE_READY, args);
          },
          viewModeChanged: function(...args) {
            return eventFactory(controller.Events.VIEW_MODE_CHANGED, args);
          },
          pageModeChanged: function(...args) {
            return eventFactory(controller.Events.PAGE_MODE_CHANGED, args);
          },
          printModeChanged: function(...args) {
            return eventFactory(controller.Events.PRINT_MODE_CHANGED, args);
          },
          reportSourceChanged: function(...args) {
            return eventFactory(controller.Events.REPORT_SOURCE_CHANGED, args);
          },
          navigateToPage: function(...args) {
            return eventFactory(controller.Events.NAVIGATE_TO_PAGE, args);
          },
          currentPageChanged: function(...args) {
            return eventFactory(controller.Events.CURRENT_PAGE_CHANGED, args);
          },
          getDocumentMapState: function(...args) {
            return eventFactory(controller.Events.GET_DOCUMENT_MAP_STATE, args);
          },
          setDocumentMapVisible: function(...args) {
            return eventFactory(controller.Events.SET_DOCUMENT_MAP_VISIBLE, args);
          },
          getParametersAreaState: function(...args) {
            return eventFactory(controller.Events.GET_PARAMETER_AREA_STATE, args);
          },
          setParametersAreaVisible: function(...args) {
            return eventFactory(controller.Events.SET_PARAMETER_AREA_VISIBLE, args);
          },
          setSideMenuVisible: function(...args) {
            return eventFactory(controller.Events.SET_TOGGLE_SIDE_MENU, args);
          },
          getSideMenuVisible: function(...args) {
            return eventFactory(controller.Events.GET_TOGGLE_SIDE_MENU, args);
          },
          scaleChanged: function(...args) {
            return eventFactory(controller.Events.SCALE_CHANGED, args);
          },
          scaleModeChanged: function(...args) {
            return eventFactory(controller.Events.SCALE_MODE_CHANGED, args);
          },
          serverActionStarted: function(...args) {
            return eventFactory(controller.Events.SERVER_ACTION_STARTED, args);
          },
          cssLoaded: function(...args) {
            return eventFactory(controller.Events.CSS_LOADED, args);
          },
          interactiveActionExecuting: function(...args) {
            return eventFactory(controller.Events.INTERACTIVE_ACTION_EXECUTING, args);
          },
          interactiveActionEnter: function(...args) {
            return eventFactory(controller.Events.INTERACTIVE_ACTION_ENTER, args);
          },
          interactiveActionLeave: function(...args) {
            return eventFactory(controller.Events.INTERACTIVE_ACTION_LEAVE, args);
          },
          updateUI: function(...args) {
            return eventFactory(controller.Events.UPDATE_UI, args);
          },
          updateUIInternal: function(...args) {
            return eventFactory(controller.Events.UPDATE_UI_INTERNAL, args);
          },
          toolTipOpening: function(...args) {
            return eventFactory(controller.Events.TOOLTIP_OPENING, args);
          },
          pageNumberChange: function(...args) {
            return eventFactory(controller.Events.PAGE_NUMBER, args);
          },
          pageCountChange: function(...args) {
            return eventFactory(controller.Events.PAGE_COUNT, args);
          },
          getSearchDialogState: function(...args) {
            return eventFactory(controller.Events.GET_SEARCH_DIALOG_STATE, args);
          },
          getSendEmailDialogState: function(...args) {
            return eventFactory(controller.Events.GET_SEND_EMAIL_DIALOG_STATE, args);
          },
          setSearchDialogVisible: function(...args) {
            return eventFactory(controller.Events.SET_SEARCH_DIALOG_VISIBLE, args);
          },
          setSendEmailDialogVisible: function(...args) {
            return eventFactory(controller.Events.SET_SEND_EMAIL_DIALOG_VISIBLE, args);
          },
          scrollPageReady: function(...args) {
            return eventFactory(controller.Events.SCROLL_PAGE_READY, args);
          },
          updatePageDimensionsReady: function(...args) {
            return eventFactory(controller.Events.UPDATE_SCROLL_PAGE_DIMENSIONS_READY, args);
          },
          missingOrInvalidParameters: function(...args) {
            return eventFactory(controller.Events.MISSING_OR_INVALID_PARAMETERS, args);
          },
          renderingStopped: function(...args) {
            return eventFactory(controller.Events.RENDERING_STOPPED, args);
          },
          clientExpired: function(...args) {
            return eventFactory(controller.Events.CLIENT_EXPIRED, args);
          },
          dispose: function() {
            stopSentinel();
            this.serviceClientSentinel = null;
          }
        }
      );
      return controller;
    }
    function addSingleComboParam(processedParam, paramValue, key) {
      try {
        var nameValuePair = processedParam.availableValues.find((obj) => {
          return obj["value"] === paramValue;
        });
        if (!nameValuePair) {
          throw new Error(`The available values of parameter ${key} do not contain Value property that equals ${paramValue}`);
        }
        return { valueMember: paramValue, displayMember: nameValuePair["name"] };
      } catch (e) {
        logError(e);
      }
    }
    function addMultiComboParam(processedParam, paramValue, key) {
      var paramArr = [];
      for (var i in paramValue) {
        paramArr.push(addSingleComboParam(processedParam, paramValue[i], key));
      }
      return paramArr;
    }

    const Events$1 = {
      EXPORT_BEGIN: "EXPORT_BEGIN",
      EXPORT_END: "EXPORT_END",
      PRINT_BEGIN: "PRINT_BEGIN",
      PRINT_END: "PRINT_END",
      RENDERING_BEGIN: "RENDERING_BEGIN",
      RENDERING_END: "RENDERING_END",
      PAGE_READY: "PAGE_READY",
      ERROR: "ERROR",
      UPDATE_UI: "UPDATE_UI",
      INTERACTIVE_ACTION_EXECUTING: "INTERACTIVE_ACTION_EXECUTING",
      INTERACTIVE_ACTION_ENTER: "INTERACTIVE_ACTION_ENTER",
      INTERACTIVE_ACTION_LEAVE: "INTERACTIVE_ACTION_LEAVE",
      VIEWER_TOOLTIP_OPENING: "VIEWER_TOOLTIP_OPENING",
      SEND_EMAIL_BEGIN: "SEND_EMAIL_BEGIN",
      SEND_EMAIL_END: "SEND_EMAIL_END",
      PARAMETERS_LOADED: "PARAMETERS_LOADED"
    };

    function HistoryManager(options) {
      var controller = options.controller;
      if (!controller) {
        throw "No controller (telerikReporting.reportViewerController) has been specified.";
      }
      var settings = options.settings;
      var history = settings.getHistory() || { records: [], position: -1 };
      controller.loadedReportChange(function() {
        addToHistory(true);
      }).currentPageChanged(function() {
        updatePageInfo();
      }).reportLoadComplete(function(event, args) {
        addToHistory(false);
      }).clientExpired(function() {
        var records = history.records;
        for (var i = 0; i < records.length; i++) {
          records[i].reportDocumentId = null;
        }
      });
      function getCurrentRecord() {
        var records = history.records;
        if (records.length > 0) {
          return records[history.position];
        }
        return null;
      }
      function pushRecord(rec) {
        var records = history.records;
        var position = history.position;
        records = Array.prototype.slice.call(records, 0, position + 1);
        records.push(rec);
        history.records = records;
        history.position = records.length - 1;
        saveSettings();
      }
      function saveSettings() {
        settings.setHistory(history);
      }
      function updatePageInfo() {
        var currentRecord = getCurrentRecord();
        if (currentRecord) {
          currentRecord.pageNumber = controller.getCurrentPageNumber();
          currentRecord.viewMode = controller.getViewMode();
          currentRecord.reportDocumentId = controller.getReportDocumentId();
          saveSettings();
        }
      }
      function addToHistory(temp) {
        removeTempRecordsFromHistory();
        var currentRecord = getCurrentRecord();
        var rs = controller.getReportSource();
        if (!currentRecord || !reportSourcesAreEqual(currentRecord.reportSource, rs)) {
          pushRecord({
            reportSource: rs,
            pageNumber: 1,
            temp
          });
        }
      }
      function exec(currentRecord, reportChanged) {
        controller.setReportDocumentId(currentRecord.reportDocumentId);
        controller.setViewMode(currentRecord.viewMode);
        controller.setReportSource(currentRecord.reportSource);
        controller.refreshReport(false, reportChanged, currentRecord.reportDocumentId);
        controller.navigateToPage(currentRecord.pageNumber);
      }
      function canMove(step) {
        var position = history.position;
        var length = history.records.length;
        var newPos = position + step;
        return 0 <= newPos && newPos < length;
      }
      function move(step) {
        var position = history.position;
        var length = history.records.length;
        var newPos = position + step;
        if (newPos < 0) {
          newPos = 0;
        } else if (newPos >= length) {
          newPos = length - 1;
        }
        if (newPos != position) {
          var currentRecord = getCurrentRecord();
          var currentReport = currentRecord.reportSource ? currentRecord.reportSource.report : "";
          history.position = newPos;
          saveSettings();
          var newRecord = getCurrentRecord();
          var newReport = newRecord.reportSource ? newRecord.reportSource.report : "";
          exec(newRecord, currentReport !== newReport);
        }
      }
      function removeTempRecordsFromHistory() {
        var lastIndex = history.records.length - 1;
        while (lastIndex >= 0) {
          if (history.records[lastIndex].temp === true) {
            history.records.splice(lastIndex, 1);
            if (history.position >= lastIndex) {
              history.position--;
            }
          } else {
            break;
          }
          lastIndex--;
        }
      }
      return {
        back: function() {
          move(-1);
        },
        forward: function() {
          move(1);
        },
        canMoveBack: function() {
          return canMove(-1);
        },
        canMoveForward: function() {
          return canMove(1);
        },
        loadCurrent: function() {
          var rec = getCurrentRecord();
          if (rec) {
            exec(rec, false);
          }
          return Boolean(rec);
        }
      };
    }

    function TouchBehavior(dom, options) {
      var startDistance;
      var ignoreTouch;
      init(dom);
      function init(element) {
        if (typeof $.fn.kendoTouch === "function") {
          try {
            $(element).mousedown(function() {
              ignoreTouch = true;
            }).mouseup(function() {
              ignoreTouch = false;
            }).kendoTouch({
              multiTouch: true,
              enableSwipe: true,
              swipe: function(e) {
                if (!ignoreTouch) {
                  onSwipe(e);
                }
              },
              gesturestart: function(e) {
                if (!ignoreTouch) {
                  onGestureStart(e);
                }
              },
              gestureend: function(e) {
                if (!ignoreTouch) {
                  onGestureEnd(e);
                }
              },
              gesturechange: function(e) {
                if (!ignoreTouch) {
                  onGestureChange(e);
                }
              },
              doubletap: function(e) {
                if (!ignoreTouch) {
                  onDoubleTap(e);
                }
              },
              touchstart: function(e) {
                if (!ignoreTouch) {
                  fire("touchstart");
                }
              }
            });
          } catch (e) {
            console.error("Instantiation of Kendo Touch threw an exception", e);
            throw e;
          }
        }
      }
      function onDoubleTap(e) {
        fire("doubletap", e);
      }
      function onGestureStart(e) {
        startDistance = kendo.touchDelta(e.touches[0], e.touches[1]).distance;
      }
      function onGestureEnd(e) {
      }
      function onGestureChange(e) {
        var current = kendo.touchDelta(e.touches[0], e.touches[1]).distance;
        onPinch({
          distance: current,
          lastDistance: startDistance
        });
        startDistance = current;
      }
      function onSwipe(e) {
        fire("swipe", e);
      }
      function onPinch(e) {
        fire("pinch", e);
      }
      function fire(func, args) {
        var handler = options[func];
        if (typeof handler === "function") {
          handler(args);
        }
      }
    }

    var Scroll = {
      controller: {},
      $placeholder: null,
      $pageContainer: null,
      pageContainer: null,
      $pageWrapper: null,
      pageWrapper: null,
      viewMode: null,
      loadedPage: {},
      scrollInProgress: false,
      enabled: false,
      pageCount: 0,
      additionalTopOffset: 130,
      pageDistance: 20,
      oldScrollTopPosition: 0,
      skeletonTemplate: '<div class="trv-report-page trv-skeleton-page trv-skeleton-{0}" style="{1}" data-page="{0}"><div class="trv-skeleton-wrapper" style="{2}"></div></div>',
      /**
       * Initialize the scroll object
       * Takes two parameters
       *  - placeholder - PageArea dom element
       *  - options - PageArea options
       */
      init: function init(placeholder, options) {
        var that = this;
        that.$placeholder = $("[data-selector='" + options.viewerSelector + "']").find(placeholder);
        that.$pageContainer = this.$placeholder.find(".trv-page-container");
        that.pageContainer = this.$pageContainer[0];
        that.$pageWrapper = this.$placeholder.find(".trv-page-wrapper");
        that.pageWrapper = this.$pageWrapper[0];
        that.controller = options.controller;
        that.viewMode = null;
        that.loadedPage = {};
        that.scrollInProgress = false;
        that.enabled = false;
        that.pageCount = 0;
        that.controller.scaleChanged(function(e, args) {
          if (that.enabled) {
            setTimeout(function() {
              that._loadMorePages();
              that._keepCurrentPageInToView();
            }, 100);
          }
        }).loadedReportChange(function(event) {
          if (that.enabled) {
            that.disable();
            if (event.type !== "loadedReportChange") {
              setTimeout(function() {
                that.controller.getPageData(1).then(function(newPage) {
                  that.renderPage(newPage);
                });
              });
            }
          }
        }).viewModeChanged(function(args) {
          if (that.enabled) {
            that.disable();
          }
        }).interactiveActionExecuting(function(e, args) {
          var actionType = args.action.Type;
          if (that.enabled && (actionType === "sorting" || actionType === "toggleVisibility")) {
            that.disable();
          }
        }).updatePageDimensionsReady(function(event, args) {
          if (that.enabled && that._currentPageNumber() > 0) {
            that._keepCurrentPageInToView();
          }
        }).pageCountChange(function(event, args) {
          if (that.enabled && that.pageCount !== args) {
            if (that._currentPageNumber() > 0 && !that.scrollInProgress) {
              that._loadMorePages();
            }
            if (args > 1) {
              that._initEvents();
            }
            that.pageCount = args;
          }
        });
      },
      /**
       * Returns true, when Telerik Report Viewer pageMode is set to CONTINUOUS_SCROLL or it is unset
       */
      isEnabled: function isEnabled() {
        return this.enabled;
      },
      disable: function disable() {
        this.$pageWrapper.empty();
        this.enabled = false;
        this.loadedPage = {};
        this.pageCount = 0;
        this.$placeholder.removeClass("scrollable");
        this._unbind();
      },
      enable: function() {
        this.enabled = true;
        this.$placeholder.addClass("scrollable");
        this._initEvents();
      },
      /**
       * Handle the page rendering called from the PageArea
       * Takes one parameter
       * page - the page object
       */
      renderPage: function renderPage(page) {
        var that = this;
        var pageViewMode = that.controller.getViewMode();
        var renderedPage = that.$placeholder.find('[data-page="' + page.pageNumber + '"]');
        if (!that.enabled) {
          that.enabled = true;
          that.$placeholder.addClass("scrollable");
          if (pageViewMode !== that.viewMode || !renderedPage.length) {
            that._updatePageArea(page);
          } else {
            that._render(page, true);
            this.$pageContainer.scrollTop(3);
            that._setCurrentPage(page.pageNumber);
          }
          that.viewMode = that.controller.getViewMode();
          that._loadMorePages();
        } else {
          if (pageViewMode !== that.viewMode || !renderedPage.length) {
            that._updatePageArea(page);
          } else {
            that._navigateToPage(page, renderedPage);
          }
          that.viewMode = that.controller.getViewMode();
        }
      },
      /**
       * Trigger scrolling animation to the specific element.
       */
      navigateToElement: function navigateToElement(offsetTop, pageNumber) {
        var that = this;
        that.scrollInProgress = true;
        if (that._isSkeletonScreen(null, pageNumber)) {
          that.controller.getPageData(pageNumber).then(function(newPage) {
            that._render(newPage, false);
            that.$pageContainer.animate({ scrollTop: offsetTop }, 500, function() {
              that._setCurrentPage(pageNumber);
              setTimeout(function() {
                that.scrollInProgress = false;
              }, 100);
            });
          });
        } else {
          that.$pageContainer.animate({ scrollTop: offsetTop }, 500, function() {
            that._setCurrentPage(pageNumber);
            setTimeout(function() {
              that.scrollInProgress = false;
            }, 100);
          });
        }
      },
      _setCurrentPage: function _setCurrentPage(pageNumber) {
        var that = this;
        if (pageNumber !== that._currentPageNumber()) {
          that.controller.setCurrentPageNumber(pageNumber);
        }
        if (that.controller.getPageCount() > 1) {
          that.$placeholder.find(".k-state-default").removeClass("k-state-default");
          that.$placeholder.find('[data-page="' + pageNumber + '"]').addClass("k-state-default");
        }
        that._loadNextPreviousPage(pageNumber);
      },
      _updatePageArea: function _updatePageArea(page) {
        var that = this;
        var scrollTo = 0;
        var pageNumber = page.pageNumber;
        that.scrollInProgress = true;
        if (pageNumber > 1) {
          that._generateSkeletonScreens(pageNumber);
        }
        that._render(page, false);
        that._setCurrentPage(page.pageNumber);
        setTimeout(function() {
          scrollTo = pageNumber > 1 ? that.$placeholder.find('[data-page="' + pageNumber + '"]').position().top : 0;
          that.$pageContainer.animate({ scrollTop: scrollTo }, 0, function() {
            that.scrollInProgress = false;
          });
        }, 100);
      },
      _navigateToPage: function _navigateToPage(page, renderedPage) {
        var that = this;
        that.scrollInProgress = true;
        var scrollTo = renderedPage.position().top;
        var pages = that.$placeholder.find(".trv-report-page");
        var pageNumber = page.pageNumber;
        $(pages[0]).height();
        if (that._isSkeletonScreen(renderedPage, pageNumber)) {
          that.controller.getPageData(pageNumber).then(function(newPage) {
            that._render(newPage, false);
            that.$pageContainer.animate({ scrollTop: scrollTo }, 500, function() {
              setTimeout(function() {
                that._setCurrentPage(newPage.pageNumber);
                that.scrollInProgress = false;
              });
            });
          });
        } else {
          that._updatePageContent(page, renderedPage);
          that.$pageContainer.animate({ scrollTop: scrollTo }, 500, function() {
            setTimeout(function() {
              that._setCurrentPage(page.pageNumber);
              that.scrollInProgress = false;
            });
          });
        }
      },
      _updatePageContent: function _updatePageContent(page, renderedPage) {
        this._updatePageStyle(page);
        var pageNumber = page.pageNumber;
        var wrapper = $($.parseHTML(page.pageContent));
        var $pageContent = wrapper.find("div.sheet");
        var $page = this.$placeholder.find('[data-page="' + pageNumber + '"]');
        $pageContent.css("margin", 0);
        $page.append($pageContent).append($('<div class="trv-page-overlay"></div>'));
        renderedPage.replaceWith($page);
        this.controller.scrollPageReady({ page, target: $page });
      },
      _currentPageNumber: function _currentPageNumber() {
        return this.controller.getCurrentPageNumber();
      },
      _isSkeletonScreen: function _isSkeletonScreen(page, pageNumber) {
        if (!page) {
          page = this.$placeholder.find('[data-page="' + pageNumber + '"]');
        }
        return page.hasClass("trv-skeleton-" + pageNumber);
      },
      _addSkeletonScreen: function _addSkeletonScreen(pageNumber, position) {
        var that = this;
        var pageStyleNumber = position ? parseInt(pageNumber + 1) : parseInt(pageNumber - 1);
        var pageStyleBaseDom = that.$placeholder.find('[data-page="' + pageStyleNumber + '"]');
        var pageStyle = pageStyleBaseDom.attr("style");
        var contentStyle = pageStyleBaseDom.find("sheet").attr("style");
        var skeletonEl = stringFormat(that.skeletonTemplate, [pageNumber, pageStyle, contentStyle]);
        if (position) {
          that.$pageWrapper.prepend(skeletonEl);
        } else {
          that.$pageWrapper.append(skeletonEl);
        }
      },
      _generateSkeletonScreens: function _generateSkeletonScreens(upToPageNumber) {
        var that = this;
        var skeletonEl = "";
        var pageStyleBaseDom = this.$placeholder.find('[data-page="1"]');
        var pageStyle = pageStyleBaseDom.attr("style");
        var contentStyle = pageStyleBaseDom.find("sheet").attr("style");
        var lastPage = that.$placeholder.find(".trv-report-page").last().attr("data-page");
        var index = lastPage ? parseInt(lastPage) + 1 : 1;
        for (index; index < upToPageNumber; index++) {
          skeletonEl = skeletonEl + stringFormat(that.skeletonTemplate, [index, pageStyle, contentStyle]);
        }
        that.$pageWrapper.append($(skeletonEl));
      },
      _loadMorePages: function _loadMorePages() {
        var that = this;
        var pageCount = that.controller.getPageCount();
        var isViewPortBiggerThanPageHeight = that.$pageContainer.innerHeight() > that.$pageWrapper.innerHeight();
        if (pageCount > 1) {
          if (isViewPortBiggerThanPageHeight) {
            that.scrollInProgress = true;
            var lastPage = parseInt(that.$placeholder.find(".trv-report-page").last().attr("data-page"));
            var nextPage = lastPage + 1;
            if (nextPage <= pageCount) {
              that.controller.getPageData(nextPage).then(function(newPage) {
                that._render(newPage, false);
                that._loadMorePages();
                that.scrollInProgress = false;
              });
            }
          } else {
            that._loadVisiblePages();
            that.scrollInProgress = false;
          }
        }
      },
      _loadVisiblePages: function _loadVisiblePages() {
        var that = this;
        var pages = that.$placeholder.find(".trv-report-page");
        Array.from(pages).forEach((value) => {
          var pageItem = $(value);
          var pageNumber = parseInt(pageItem.attr("data-page"));
          if (that._scrolledInToView(pageItem) && that._isSkeletonScreen(pageItem, pageNumber)) {
            that.controller.getPageData(pageNumber).then((newPage) => {
              that._render(newPage, false);
            });
          }
        });
      },
      _scrolledInToView: function _scrolledInToView(elem) {
        var pageCoords = elem[0].getBoundingClientRect();
        var parentCoords = elem.closest(".trv-pages-area")[0].getBoundingClientRect();
        var parentTop = parentCoords.top;
        var parentBottom = parentCoords.top + parentCoords.height;
        var pageTop = pageCoords.top;
        var pageBottom = pageTop + elem.outerHeight(true);
        var additionalTopOffset = this.additionalTopOffset + parentTop;
        var topVisible = pageTop > 0 && pageTop < parentBottom;
        var bottomVisible = pageBottom < parentBottom && pageBottom > additionalTopOffset;
        return topVisible || bottomVisible;
      },
      _render: function _render(page, empty) {
        var that = this;
        var pageNumber = page.pageNumber;
        var pageItem = that.$placeholder.find('[data-page="' + pageNumber + '"]');
        if (!empty && pageItem && pageItem.length && !that._isSkeletonScreen(pageItem, pageNumber)) {
          return;
        }
        that.loadedPage[pageNumber] = page;
        that._updatePageStyle(page);
        var wrapper = $($.parseHTML(page.pageContent));
        var $pageContent = wrapper.find("div.sheet");
        var $page = $('<div class="trv-report-page" data-page="' + pageNumber + '"></div>');
        $pageContent.css("margin", 0);
        $page.append($pageContent).append($('<div class="trv-page-overlay"></div>'));
        if (empty) {
          that.$pageWrapper.empty();
        }
        that.$pageWrapper.removeData().data("pageNumber", pageNumber);
        var $skeletonPage = that.$placeholder.find(".trv-skeleton-" + pageNumber);
        if ($skeletonPage.length) {
          $skeletonPage.replaceWith($page);
        } else {
          that.$pageWrapper.append($page);
        }
        that.controller.scrollPageReady({ page, target: $page });
      },
      _updatePageStyle: function _updatePageStyle(page) {
        var that = this;
        var lastLoadedPage = that.loadedPage[that._lastLoadedPage()] || page;
        var styleId = "trv-" + that.controller.clientId() + "-styles";
        var pageStyles;
        $("#" + styleId).remove();
        pageStyles = $("<style id=" + styleId + "></style>");
        pageStyles.append(lastLoadedPage.pageStyles);
        pageStyles.appendTo("head");
      },
      _lastLoadedPage: function _lastLoadedPage() {
        var that = this;
        var lastKey;
        for (var key in that.loadedPage) {
          if (that.loadedPage.hasOwnProperty(key)) {
            lastKey = key;
          }
        }
        return lastKey;
      },
      _loadNextPreviousPage: function _loadNextPreviousPage(pageNumber) {
        var that = this;
        var nextPage;
        var previousPage;
        var nextItem;
        var previousItem;
        if (pageNumber < that.controller.getPageCount()) {
          nextPage = pageNumber + 1;
          nextItem = that.$placeholder.find('[data-page="' + nextPage + '"]');
        }
        if (pageNumber > 1) {
          previousPage = pageNumber - 1;
          previousItem = that.$placeholder.find('[data-page="' + previousPage + '"]');
        }
        if (previousItem && previousItem.length && that._isSkeletonScreen(previousItem, previousPage)) {
          that.controller.getPageData(previousPage).then(function(newPage) {
            that._render(newPage, false);
          });
        }
        if (nextItem && nextItem.length && that._isSkeletonScreen(nextItem, nextPage)) {
          that.controller.getPageData(nextPage).then(function(newPage) {
            that._render(newPage, false);
          });
        }
      },
      _clickPage: function _clickPage(pageDom) {
        var that = this;
        var currentPage = that._currentPageNumber();
        var pageNumber = parseInt(pageDom.attr("data-page"));
        if (currentPage !== pageNumber) {
          if (that._isSkeletonScreen(pageDom, pageNumber)) {
            that.controller.getPageData(pageNumber).then(function(newPage) {
              that._render(newPage, false, true);
              that._setCurrentPage(newPage.pageNumber);
            });
          } else {
            that._setCurrentPage(pageNumber);
          }
        }
      },
      _initEvents: function _initEvents() {
        var that = this;
        that.$pageContainer.off("click", ".trv-report-page").on("click", ".trv-report-page", function(e) {
          that._clickPage($(e.currentTarget));
        });
        that.$pageContainer.scroll(function() {
          var pages = that.$placeholder.find(".trv-report-page");
          var scrollPosition = parseInt((that.$pageContainer.scrollTop() + that.$pageContainer.innerHeight()).toFixed(0));
          if (!that.scrollInProgress && that.oldScrollTopPosition !== scrollPosition) {
            if (that.oldScrollTopPosition > scrollPosition) {
              that._scrollUp(pages);
            } else {
              that._scrollDown(pages, scrollPosition);
            }
          }
          that.oldScrollTopPosition = scrollPosition;
        });
        that.$pageContainer.scroll(function() {
          var pages = that.$placeholder.find(".trv-report-page");
          var scrollPosition = parseInt((that.$pageContainer.scrollTop() + that.$pageContainer.innerHeight()).toFixed(0));
          if (!that.scrollInProgress && pages.length && that.oldScrollTopPosition !== scrollPosition) {
            that._advanceCurrentPage(pages);
          }
        });
      },
      _unbind: function() {
        var that = this;
        that.$pageContainer.off("click", ".trv-report-page");
        that.$pageContainer.off("scroll");
      },
      _advanceCurrentPage: function _advanceCurrentPage(pages) {
        var that = this;
        var newCurrentPage = that._findNewCurrentPage(pages);
        var pageNumber;
        var currentPageNumber = that._currentPageNumber();
        var currentPageIsInToView = that._scrolledInToView(that.$placeholder.find('[data-page="' + currentPageNumber + '"]'));
        if (newCurrentPage !== -1) {
          newCurrentPage = $(newCurrentPage);
          pageNumber = parseInt(newCurrentPage.attr("data-page"));
          if (currentPageNumber !== pageNumber && !currentPageIsInToView) {
            if (that._isSkeletonScreen(newCurrentPage, pageNumber)) {
              that.controller.getPageData(pageNumber).then(function(newPage) {
                that._render(newPage, false, true);
                that._setCurrentPage(newPage.pageNumber);
              });
            } else {
              that._setCurrentPage(pageNumber);
            }
          }
        } else {
          console.log("Page not found - ", newCurrentPage);
        }
      },
      // Binary search
      _findNewCurrentPage: function _findNewCurrentPage(pages) {
        var that = this;
        var middleIndex = Math.floor(pages.length / 2);
        var result = that._findPageInViewPort(middleIndex, pages);
        if (pages.length === 1) {
          return pages[0];
        }
        if (result === 0) {
          return pages[middleIndex];
        } else if (result < 0 && pages.length > 1) {
          return that._findNewCurrentPage(pages.splice(middleIndex, Number.MAX_VALUE));
        } else if (result > 0 && pages.length > 1) {
          return that._findNewCurrentPage(pages.splice(0, middleIndex));
        }
        return -1;
      },
      _findPageInViewPort: function _findPageInViewPort(index, pages) {
        var pageItem = this.$placeholder.find(pages[index]);
        var pageCoords = pageItem[0].getBoundingClientRect();
        var parentCoords = pageItem.closest(".trv-pages-area")[0].getBoundingClientRect();
        var parentTop = parentCoords.top;
        parentCoords.top + parentCoords.height;
        var pageTop = pageCoords.top;
        var pageBottom = pageTop + pageItem.outerHeight(true);
        var additionalTopOffset = this.additionalTopOffset + parentTop;
        var isCurentPage = pageTop <= additionalTopOffset && additionalTopOffset < pageBottom;
        if (isCurentPage) {
          return 0;
        }
        if (pageBottom < additionalTopOffset) {
          return -1;
        }
        return 1;
      },
      _scrollDown: function _scrollDown(pages, scrollPosition) {
        var that = this;
        if (scrollPosition >= that.pageContainer.scrollHeight - 5) {
          var lastPage = parseInt($(pages[pages.length - 1]).attr("data-page"));
          var nextPage = lastPage + 1;
          if (that._currentPageNumber() < nextPage && nextPage <= that.controller.getPageCount()) {
            that._addSkeletonScreen(nextPage, false);
            that.controller.getPageData(nextPage).then(function(newPage) {
              that._render(newPage, false);
            });
          }
        } else {
          that._advanceCurrentPage(pages);
          that._loadVisiblePages();
        }
      },
      _scrollUp: function _scrollUp(pages) {
        var that = this;
        if (that.$pageContainer.scrollTop() === 0) {
          var firstPage = $(pages[0]);
          var pageNumber = parseInt(firstPage.attr("data-page"));
          var previousPage = pageNumber - 1;
          if (that._currentPageNumber() > previousPage && previousPage >= 1) {
            that._addSkeletonScreen(previousPage, true);
            that.controller.getPageData(previousPage).then(function(newPage) {
              that._render(newPage, false);
              that.$pageContainer.scrollTop(3);
            });
          }
        } else {
          that._advanceCurrentPage(pages);
          that._loadVisiblePages();
        }
      },
      _keepCurrentPageInToView: function _keepCurrentPageInToView() {
        var that = this;
        var currentPage = that.$placeholder.find('[data-page="' + that._currentPageNumber() + '"]');
        var currentPagePosition = currentPage.position().top;
        var currentPageHeight = currentPage.innerHeight();
        var pageContainerHeight = that.$pageContainer.innerHeight();
        var emptyView;
        that.scrollInProgress = true;
        if (currentPageHeight < pageContainerHeight) {
          emptyView = (pageContainerHeight - currentPageHeight) / 2;
          currentPagePosition = parseInt(currentPagePosition - emptyView);
        }
        that.$pageContainer.animate({ scrollTop: currentPagePosition }, 0, function() {
          setTimeout(function() {
            that.scrollInProgress = false;
          }, 100);
        });
      }
    };

    var UIFreezeCoordinator = {
      $placeholder: null,
      $scrollableContainer: null,
      // Holds all items initial position per container
      itemsInitialState: {},
      // Holds the bounds of the frozen areas by X per container
      xFrozenAreasBounds: {},
      // Holds the bounds of the frozen areas by Y per container
      yFrozenAreasBounds: {},
      freezeMaxZIndex: {},
      zIndex: 1,
      // Holds default background-color value per container
      freezeBGColor: {},
      // Holds whether freezing has been applied per container.
      currentlyFrozenContainer: {
        vertical: {},
        horizontal: {}
      },
      isInitialize: false,
      scaleFactor: null,
      /**
       * Initialize the uiFreezeCoordinator object
       * Takes one parameter
       *  - $placeholder - PageArea jQuery DOM element
       */
      init: function($placeholder) {
        this.$placeholder = $placeholder;
        this.$scrollableContainer = $placeholder.find(".trv-page-container");
        if (this.isInitialize) {
          this.reset($placeholder);
        }
        this._attachToScrollEvent();
        this.isInitialize = true;
      },
      reset: function($placeholder) {
        this.$placeholder = $placeholder;
        this.$scrollableContainer = $placeholder.find(".trv-page-container");
        this.itemsInitialState = {};
        this.xFrozenAreasBounds = {};
        this.yFrozenAreasBounds = {};
        this.currentlyfreezedContainer = {
          vertical: {},
          horizontal: {}
        };
      },
      setScaleFactor: function(scale) {
        this.scaleFactor = scale;
      },
      /**
       * Initializing scroll listener
       */
      _attachToScrollEvent: function() {
        var thisInstance = this;
        this.$scrollableContainer.scroll(function updateFreezeUIOnScroll() {
          var $freezeItems = thisInstance.$scrollableContainer.find("div[data-sticky-id]");
          if ($freezeItems.length) {
            var tableIDs = $freezeItems.map(function(index2, $element) {
              return $($element).attr("data-sticky-id");
            }).get();
            var uniqueIDs = tableIDs.filter(function(item, index2) {
              return index2 === tableIDs.indexOf(item);
            });
            var scrollableContainerScrollTop = thisInstance.$scrollableContainer.scrollTop();
            var scrollableContainerScrollLeft = thisInstance.$scrollableContainer.scrollLeft();
            for (var index = 0; index < uniqueIDs.length; index++) {
              var freezeItemsContainerID = uniqueIDs[index];
              if (!thisInstance.itemsInitialState[freezeItemsContainerID]) {
                thisInstance._saveFreezeItemsInitialState(freezeItemsContainerID);
              }
              thisInstance._updateFreezeItemsOnScroll(
                freezeItemsContainerID,
                scrollableContainerScrollTop,
                scrollableContainerScrollLeft
              );
            }
          }
        });
      },
      _saveFreezeItemsInitialState: function(freezeItemsContainerID) {
        var $allFreezeItems = $("[data-sticky-direction][data-sticky-id='" + freezeItemsContainerID + "']");
        var $freezeActions = $("[data-reporting-action][data-sticky-id='" + freezeItemsContainerID + "']");
        var yAreaBounds;
        var xAreaBounds;
        this.itemsInitialState[freezeItemsContainerID] = {};
        this.freezeBGColor[freezeItemsContainerID] = $("[data-id='" + freezeItemsContainerID + "']").attr("data-sticky-bg-color");
        for (var index = 0; index < $allFreezeItems.length; index++) {
          var $item = $($allFreezeItems[index]);
          var scrollDirection = $item.attr("data-sticky-direction");
          var itemID = $item.attr("data-id");
          var itemPosition = $item.position();
          var scaledItemPosition = { top: itemPosition.top / this.scaleFactor, left: itemPosition.left / this.scaleFactor };
          var itemBounds = rectangle(scaledItemPosition.left, scaledItemPosition.top, $item.outerWidth(true) * this.scaleFactor, $item.outerHeight(true) * this.scaleFactor);
          switch (scrollDirection) {
            case "Vertical":
              yAreaBounds = yAreaBounds ? yAreaBounds.union(itemBounds) : itemBounds;
              break;
            case "Horizontal":
              xAreaBounds = xAreaBounds ? xAreaBounds.union(itemBounds) : itemBounds;
              break;
          }
          this._saveFreezeItemInitialState(freezeItemsContainerID, $item, itemID, scaledItemPosition);
        }
        this.freezeMaxZIndex[freezeItemsContainerID] = $freezeActions.length ? $freezeActions.css("zIndex") : this.zIndex;
        this.yFrozenAreasBounds[freezeItemsContainerID] = yAreaBounds;
        this.xFrozenAreasBounds[freezeItemsContainerID] = xAreaBounds;
      },
      /**
       * Save the freeze UI item initial position based on the wrapper element and current page
       *  - freezeItemsContainerID - string. A parent/wrapper element identifier
       *  - $item - JQuery DOM element of the freezed item
       *  - itemID - string. A ID of the freeze UI item
       *  - position - object. Contains the top and left values
       */
      _saveFreezeItemInitialState: function(freezeItemsContainerID, $item, itemID, position) {
        var itemBgColor = $item.css("background-color");
        var hasInitialBgColor = this._hasSetBgColor(itemBgColor);
        var itemState = {
          top: position.top,
          left: position.left,
          zIndex: $item.css("zIndex"),
          hasBgColor: hasInitialBgColor
        };
        this.itemsInitialState[freezeItemsContainerID][itemID] = itemState;
      },
      _updateFreezeItemsOnScroll: function(freezeItemsContainerID, scrollableContainerScrollTop, scrollableContainerScrollLeft) {
        var $elementWrapper = $("div[data-id='" + freezeItemsContainerID + "']");
        if (this._isInScrollVisibleArea($elementWrapper)) {
          var $pageContainer = $elementWrapper.closest(".trv-report-page");
          var pageContainerPosition = $pageContainer.position();
          var pageContainerMargin = parseFloat($pageContainer.css("margin-top"));
          var pageContainerTopOffset = parseFloat($pageContainer.css("padding-top"));
          var pageContainerLeftOffset = parseFloat($pageContainer.css("padding-left"));
          var pageContainerBorderTopWidth = parseFloat($pageContainer.css("border-top-width"));
          var pageContainerBorderLeftWidth = parseFloat($pageContainer.css("border-left-width"));
          var $rowHeaders = $("[data-sticky-direction*='Horizontal'][data-sticky-id='" + freezeItemsContainerID + "']");
          var $colHeaders = $("[data-sticky-direction*='Vertical'][data-sticky-id='" + freezeItemsContainerID + "']");
          var hasFixRow = $rowHeaders.length > 0;
          var hasFixColumn = $colHeaders.length > 0;
          var elementWrapperPosition = $elementWrapper.position();
          var elementWrapperTopPosition = elementWrapperPosition.top + pageContainerPosition.top + pageContainerMargin + pageContainerTopOffset + pageContainerBorderTopWidth;
          var elementWrapperLeftPosition = elementWrapperPosition.left + pageContainerLeftOffset + pageContainerBorderLeftWidth;
          var verticalMoveOffset = scrollableContainerScrollTop - elementWrapperTopPosition;
          var horizontalMoveOffset = scrollableContainerScrollLeft - elementWrapperLeftPosition;
          if (hasFixColumn && verticalMoveOffset > 0) {
            if (scrollableContainerScrollTop <= $elementWrapper.outerHeight() * this.scaleFactor + elementWrapperTopPosition - this.yFrozenAreasBounds[freezeItemsContainerID].height) {
              this.currentlyFrozenContainer.vertical[freezeItemsContainerID] = true;
              this._updateUIElementsPosition($colHeaders, "top", verticalMoveOffset / this.scaleFactor, freezeItemsContainerID);
            }
          } else {
            if (this.currentlyFrozenContainer.vertical[freezeItemsContainerID]) {
              delete this.currentlyFrozenContainer.vertical[freezeItemsContainerID];
              this._updateUIElementsPosition($colHeaders, "top", -1, freezeItemsContainerID);
            }
          }
          if (hasFixRow && horizontalMoveOffset > 0) {
            if (scrollableContainerScrollLeft <= $elementWrapper.outerWidth() * this.scaleFactor + elementWrapperLeftPosition - this.xFrozenAreasBounds[freezeItemsContainerID].width) {
              this.currentlyFrozenContainer.horizontal[freezeItemsContainerID] = true;
              this._updateUIElementsPosition($rowHeaders, "left", horizontalMoveOffset / this.scaleFactor, freezeItemsContainerID);
            }
          } else {
            if (this.currentlyFrozenContainer.horizontal[freezeItemsContainerID]) {
              delete this.currentlyFrozenContainer.horizontal[freezeItemsContainerID];
              this._updateUIElementsPosition($rowHeaders, "left", -1, freezeItemsContainerID);
            }
          }
        } else {
          if (this.currentlyFrozenContainer.horizontal[freezeItemsContainerID] || this.currentlyFrozenContainer.vertical[freezeItemsContainerID]) {
            this._resetToDefaultPosition(freezeItemsContainerID);
          }
        }
      },
      /**
       * Move all freeze items to their initial position
       * Takes one parameters
       *  - freezeItemsContainerID - string. A parent/wrapper element identifier
       */
      _resetToDefaultPosition: function(freezeItemsContainerID) {
        var $rowHeaders = $("[data-sticky-direction*='Horizontal'][data-sticky-id='" + freezeItemsContainerID + "']");
        var $colHeaders = $("[data-sticky-direction*='Vertical'][data-sticky-id='" + freezeItemsContainerID + "']");
        this._updateUIElementsPosition($colHeaders, "top", -1, freezeItemsContainerID);
        this._updateUIElementsPosition($rowHeaders, "left", -1, freezeItemsContainerID);
        delete this.currentlyFrozenContainer.horizontal[freezeItemsContainerID];
        delete this.currentlyFrozenContainer.vertical[freezeItemsContainerID];
      },
      /**
       * Update the freeze elements position
       * Takes four parameters
       *  - targetElements -Array. Collection of DOM element, that has a freeze attribute
       *  - position - string. Indicates which position property to be updated - top or left
       *  - offset - integer. The value of the increase that should be applied.
       *              If it is negative number, should not move the items and it should set their initial position
       *  - freezeItemsContainerID - string. A parent/wrapper element identifier
       */
      _updateUIElementsPosition: function(targetElements, position, offset, freezeItemsContainerID) {
        for (var index = 0; index < targetElements.length; index++) {
          var $item = $(targetElements[index]);
          var itemFreezeDirection = $item.attr("data-sticky-direction");
          var isFrozenBothDirection = itemFreezeDirection.indexOf(",") > 0;
          var itemID = $item.attr("data-id");
          var itemInitialState = this.itemsInitialState[freezeItemsContainerID][itemID];
          var itemNewPostion = itemInitialState[position];
          var initialZIndex = itemInitialState["zIndex"];
          var hasInitialBgColor = itemInitialState["hasBgColor"];
          var zIndexValue = 1;
          var maxZIndex = this.freezeMaxZIndex[freezeItemsContainerID] ? this.freezeMaxZIndex[freezeItemsContainerID] : zIndexValue;
          if (isFrozenBothDirection) {
            zIndexValue = initialZIndex !== "auto" ? initialZIndex : maxZIndex + 2;
          } else {
            zIndexValue = initialZIndex !== "auto" ? initialZIndex + 1 : maxZIndex;
          }
          var newStyleRules = {
            "z-index": zIndexValue
          };
          if (offset >= 0) {
            itemNewPostion = itemNewPostion + offset;
          } else {
            newStyleRules["z-index"] = initialZIndex;
          }
          if (!hasInitialBgColor) {
            this._applyBgColorOnScroll($item, isFrozenBothDirection, hasInitialBgColor, offset >= 0, freezeItemsContainerID);
          }
          newStyleRules[position] = itemNewPostion + "px";
          $item.css(newStyleRules);
        }
      },
      // eslint-disable-next-line max-params
      _applyBgColorOnScroll: function($item, isItemFrozenBothDirection, hasInitialBgColor, shouldApplyBGColor, freezeItemsContainerID) {
        if ($item.is("img")) {
          return true;
        }
        if (isItemFrozenBothDirection && this._isFrozen(freezeItemsContainerID) && !hasInitialBgColor) {
          $item.css("background-color", this.freezeBGColor[freezeItemsContainerID]);
          return true;
        }
        if (shouldApplyBGColor) {
          $item.css("background-color", this.freezeBGColor[freezeItemsContainerID]);
        } else {
          $item.css("background-color", "initial");
        }
      },
      _hasSetBgColor: function(bgColorValue) {
        return getColorAlphaValue(bgColorValue) > 0;
      },
      _isFrozen: function(freezeItemsContainerID) {
        return this.currentlyFrozenContainer.horizontal[freezeItemsContainerID] || this.currentlyFrozenContainer.vertical[freezeItemsContainerID];
      },
      /**
       * Checks if an UI element is in the visible part of the scrollable container
       * Takes one parameters
       *  - $element - JQuery DOM element
       */
      _isInScrollVisibleArea: function($element) {
        var $page = $element.closest(".trv-report-page");
        var elementPosition = $element.position();
        return this._isVisibleVertically($element, $page, elementPosition) && this._isVisibleHorizontally($element, $page, elementPosition);
      },
      _isVisibleHorizontally: function($element, $page, elementPosition) {
        var pageLeftOffset = parseFloat($page.css("padding-left"));
        var scrollableContainerLeftScrollPosition = this.$scrollableContainer.scrollLeft();
        var scrollableContainerWidth = this.$scrollableContainer.width();
        var elementWidth = $element.outerWidth(true) * this.scaleFactor;
        var elementLeftOffset = elementPosition.left + pageLeftOffset;
        return elementLeftOffset > scrollableContainerLeftScrollPosition - elementWidth && elementLeftOffset < scrollableContainerLeftScrollPosition + elementWidth + scrollableContainerWidth;
      },
      _isVisibleVertically: function($element, $page, elementPosition) {
        var pageTopOffset = parseFloat($page.css("padding-top"));
        var pagePosition = $page.position();
        var scrollableContainerTopScrollPosition = this.$scrollableContainer.scrollTop();
        var scrollableContainerHeight = this.$scrollableContainer.height();
        var elementHeight = $element.outerHeight(true) * this.scaleFactor;
        var elementTopOffset = elementPosition.top + pageTopOffset + pagePosition.top;
        return elementTopOffset > scrollableContainerTopScrollPosition - elementHeight && elementTopOffset < scrollableContainerTopScrollPosition + elementHeight + scrollableContainerHeight;
      }
    };

    var defaultOptions$5 = {};
    function PagesArea(placeholder, options, otherOptions) {
      options = $.extend({}, defaultOptions$5, options, otherOptions);
      var controller = options.controller;
      if (!controller)
        throw "No controller (telerikReportViewer.reportViewerController) has been specified.";
      var $placeholder = $(placeholder);
      var $pageContainer = $placeholder.find(".trv-page-container");
      var pageContainer = $pageContainer[0];
      var $pageWrapper = $placeholder.find(".trv-page-wrapper");
      var pageWrapper = $pageWrapper[0];
      var $errorMessage = $placeholder.find(".trv-error-message");
      var actions;
      var pendingElement;
      var pageScaleMode = controller.getScaleMode();
      var pageScale = controller.getScale();
      var minPageScale = 0.1;
      var maxPageScale = 8;
      var documentReady = true;
      var navigateToPageOnDocReady;
      var navigateToElementOnDocReady;
      var isNewReportSource;
      var showErrorTimeoutId;
      var showPageAreaImage = false;
      var reportPageIsLoaded = false;
      var pageAreaImageStyle = '.trv-page-container {background: #ffffff url("{0}") no-repeat center 50px}';
      var pageAreaImageID = "trv-initial-image-styles";
      var scroll = $.extend({}, Scroll);
      var uiFreezeCoordinator = null;
      init();
      if (scroll) {
        scroll.init(placeholder, options);
      }
      function init() {
        replaceStringResources($placeholder);
      }
      function replaceStringResources($pagesArea) {
        $pagesArea.attr("aria-label", stringResources[$pagesArea.attr("aria-label")]);
      }
      $(window).on("resize", function(event, args) {
        if (shouldAutosizePage()) {
          updatePageDimensions();
        }
      });
      enableTouch($placeholder);
      function clearPendingTimeoutIds() {
        if (showErrorTimeoutId) {
          window.clearTimeout(showErrorTimeoutId);
        }
      }
      function invalidateCurrentlyLoadedPage() {
        var page = findPage(navigateToPageOnDocReady);
        if (page) {
          pageNo(page, -1);
        }
      }
      function navigateWhenPageAvailable(pageNumber, pageCount) {
        if (pageNumber && pageNumber <= pageCount) {
          navigateToPage(pageNumber, navigateToElementOnDocReady);
        }
      }
      function navigateOnLoadComplete(pageNumber, pageCount) {
        if (pageNumber) {
          pageNumber = Math.min(pageNumber, pageCount);
          navigateToPage(pageNumber, navigateToElementOnDocReady);
        }
      }
      function clearPage() {
        clear(isNewReportSource);
        isNewReportSource = false;
      }
      controller.pageModeChanged(function() {
        if (controller.getPageMode() === PageModes.CONTINUOUS_SCROLL) {
          scroll.enable();
        } else {
          scroll.disable();
        }
        controller.refreshReport(true);
      }).reportSourceChanged(function() {
        isNewReportSource = true;
        navigateToPageOnDocReady = null;
        navigateToElementOnDocReady = null;
        documentReady = false;
      }).beforeLoadParameters(function(event, initial) {
        if (initial) {
          clearPendingTimeoutIds();
          showError(stringResources.loadingParameters);
        }
      }).beforeLoadReport(function() {
        documentReady = false;
        if (!navigateToPageOnDocReady) {
          navigateToPageOnDocReady = 1;
        }
        clearPendingTimeoutIds();
        clear();
        disablePagesArea(true);
        if (controller.isReportAutoRun()) {
          showError(stringResources.loadingReport);
        }
      }).beginLoadReport(function(event, args) {
        documentReady = true;
        invalidateCurrentlyLoadedPage();
      }).reportLoadProgress(function(event, args) {
        navigateWhenPageAvailable(navigateToPageOnDocReady, args.pageCount);
        showError(stringFormat(stringResources.loadingReportPagesInProgress, [args.pageCount]));
      }).reportLoadComplete(function(event, args) {
        if (0 === args.pageCount) {
          clearPage();
          showError(stringResources.noPageToDisplay);
        } else {
          navigateOnLoadComplete(navigateToPageOnDocReady, args.pageCount);
          showError(stringFormat(stringResources.loadedReportPagesComplete, [args.pageCount]));
          showErrorTimeoutId = window.setTimeout(showError, 2e3);
          enableInteractivity();
        }
        if (args.containsFrozenContent && !uiFreezeCoordinator) {
          uiFreezeCoordinator = $.extend({}, UIFreezeCoordinator);
          if (controller.getViewMode() === ViewModes.INTERACTIVE) {
            uiFreezeCoordinator.init($placeholder);
          }
        }
      }).navigateToPage(function(event, pageNumber, targetElement) {
        navigateToPage(pageNumber, targetElement);
      }).pageReady(function(event, page) {
        if (controller.getPageMode() === PageModes.SINGLE_PAGE) {
          if (scroll.isEnabled()) {
            scroll.disable();
          }
          setPageContent(page);
        } else {
          scroll.renderPage(page);
        }
        if (!reportPageIsLoaded) {
          reportPageIsLoaded = true;
        }
        if (showPageAreaImage) {
          clearPageAreaImage();
        }
        if (controller.getViewMode() === ViewModes.INTERACTIVE && uiFreezeCoordinator) {
          uiFreezeCoordinator.init($placeholder);
        }
        disablePagesArea(false);
      }).error(function(event, error) {
        disablePagesArea(false);
        clearPage();
        showError(error);
      }).showNotification(function(event, args) {
        showError(stringResources[args.stringResources]);
      }).hideNotification(function(event, args) {
        showError();
      }).scaleChanged(function(event, scale) {
        setPageScale({
          scale,
          scaleMode: controller.getScaleMode()
        });
      }).scaleModeChanged(function(event, scaleMode) {
        setPageScale({
          scale: controller.getScale(),
          scaleMode
        });
      }).serverActionStarted(function() {
        disablePagesArea(true);
        showError(stringResources.loadingReport);
      }).scrollPageReady(function(event, args) {
        setScrollablePage(args);
      }).missingOrInvalidParameters(function(event, args) {
        if (options.initialPageAreaImageUrl && !reportPageIsLoaded) {
          clearPage();
          setPageAreaImage();
        }
      }).renderingStopped(function() {
        clear(true);
        showError(stringResources.renderingCanceled);
      });
      controller.setDocumentMapVisible(function() {
        if (shouldAutosizePage()) {
          setTimeout(function() {
            updatePageDimensions();
          });
        }
      }).setParametersAreaVisible(function() {
        if (shouldAutosizePage()) {
          setTimeout(function() {
            updatePageDimensions();
          });
        }
      });
      function enableTouch(dom) {
        var allowSwipeLeft;
        var allowSwipeRight;
        TouchBehavior(
          dom,
          {
            swipe: function(e) {
              var pageNumber = controller.getCurrentPageNumber();
              if (allowSwipeLeft && e.direction === "left") {
                if (pageNumber < controller.getPageCount()) {
                  controller.navigateToPage(pageNumber + 1);
                }
              } else if (allowSwipeRight && e.direction === "right") {
                if (pageNumber > 1) {
                  controller.navigateToPage(pageNumber - 1);
                }
              }
            },
            pinch: function(e) {
              var page = getCurrentPage();
              var scale = $(page).data("pageScale") || pageScale;
              var f = e.distance / e.lastDistance;
              setPageScale({
                scale: scale * f,
                scaleMode: ScaleModes.SPECIFIC
              });
            },
            doubletap: function(e) {
              options.commands.toggleZoomMode.exec();
            },
            touchstart: function(e) {
              var el = pageWrapper;
              allowSwipeRight = 0 === el.scrollLeft;
              allowSwipeLeft = el.scrollWidth - el.offsetWidth === el.scrollLeft;
            }
          }
        );
      }
      function shouldAutosizePage() {
        return -1 !== [ScaleModes.FIT_PAGE, ScaleModes.FIT_PAGE_WIDTH].indexOf(pageScaleMode);
      }
      function updatePageDimensions() {
        for (var i = 0, children = $pageContainer.find(".trv-report-page"), len = children.length; i < len; i++) {
          var pageNumber = parseInt($(children[i]).attr("data-page"));
          setPageDimensions(children[i], pageScaleMode, pageScale, pageNumber);
        }
        controller.updatePageDimensionsReady();
      }
      function setPageScale({ scale, scaleMode }) {
        pageScale = scale || pageScale;
        pageScaleMode = scaleMode || pageScaleMode;
        pageScale = Math.max(minPageScale, Math.min(maxPageScale, pageScale));
        updatePageDimensions();
      }
      function clear(clearPageWrapper) {
        disableInteractivity();
        pendingElement = void 0;
        if (clearPageWrapper) {
          $pageWrapper.empty();
        }
        showError();
      }
      function getCurrentPage() {
        return findPage(controller.getCurrentPageNumber());
      }
      function findPage(pageNumber) {
        var result;
        var allPages = $pageContainer.find(".trv-report-page");
        if (controller.getPageMode() === PageModes.SINGLE_PAGE) {
          result = Array.from(allPages).find((page) => {
            return pageNo(page) === pageNumber;
          });
        } else {
          result = Array.from(allPages).find((page) => {
            var dataPageNumber = parseInt($(page).attr("data-page"));
            return dataPageNumber === pageNumber;
          });
        }
        return result;
      }
      function navigateToPage(pageNumber, targetElement) {
        if (documentReady) {
          navigateToPageCore(pageNumber, targetElement);
        } else {
          navigateToPageOnDocumentReady(pageNumber, targetElement);
        }
      }
      function navigateToPageOnDocumentReady(pageNumber, targetElement) {
        navigateToPageOnDocReady = pageNumber;
        navigateToElementOnDocReady = targetElement;
      }
      function navigateToPageCore(pageNumber, targetElement) {
        var page = findPage(pageNumber);
        if (page) {
          if (targetElement) {
            navigateToElement(targetElement, pageNumber);
          }
          if (scroll.isEnabled() && !targetElement) {
            scroll.navigateToElement(page.offsetTop, pageNumber);
          }
        } else {
          pendingElement = targetElement;
          beginLoadPage(pageNumber);
        }
      }
      function navigateToElement(targetElement, pageNumber) {
        if (targetElement) {
          var el = $pageContainer.find("[data-" + targetElement.type + "-id=" + targetElement.id + "]")[0];
          if (el) {
            if (options.enableAccessibility) {
              var $nextFocusable = findNextFocusableElement($(el));
              if ($nextFocusable) {
                $nextFocusable.focus();
              }
            }
            var container = $pageContainer[0];
            var offsetTop = 0;
            var offsetLeft = 0;
            while (el && el !== container) {
              if ($(el).is(".trv-page-wrapper")) {
                var scale = $(el).data("pageScale");
                if (typeof scale === "number") {
                  offsetTop *= scale;
                  offsetLeft *= scale;
                }
              }
              offsetTop += el.offsetTop;
              offsetLeft += el.offsetLeft;
              el = el.offsetParent;
            }
            if (scroll.isEnabled() && pageNumber) {
              scroll.navigateToElement(offsetTop, pageNumber);
            } else {
              container.scrollTop = offsetTop;
              container.scrollLeft = offsetLeft;
            }
          } else {
            if (scroll.isEnabled() && pageNumber) {
              scroll.navigateToElement($placeholder.find('[data-page="' + pageNumber + '"]')[0].offsetTop, pageNumber);
            }
          }
        }
      }
      function findNextFocusableElement(element) {
        if (!element || element.length === 0) {
          return null;
        }
        var num = tryParseInt(element.attr("tabindex"));
        if (!isNaN(num) && num > -1) {
          return element;
        }
        return findNextFocusableElement(element.next());
      }
      function disablePagesArea(disable) {
        (disable ? $.fn.addClass : $.fn.removeClass).call($placeholder, "trv-loading");
      }
      function showError(error) {
        $errorMessage.text(error);
        (error ? $.fn.addClass : $.fn.removeClass).call($placeholder, "trv-error");
      }
      function pageNo(page, no) {
        var $page = page.$ ? page : $(page);
        var dataKey = "pageNumber";
        if (no === void 0) {
          return $page.data(dataKey);
        }
        $page.data(dataKey, no);
        return page;
      }
      function beginLoadPage(pageNumber) {
        disablePagesArea(true);
        window.setTimeout(controller.getReportPage.bind(controller, pageNumber), 1);
        navigateToPageOnDocReady = null;
      }
      function setPageDimensions(page, scaleMode, scale$1, pageNumber) {
        var $target = $(page);
        var $page = pageNumber ? $target : $target.find("div.trv-report-page");
        var $pageContent = $page.find("div.sheet");
        var $pageSkeletonContent = $page.find("div.trv-skeleton-wrapper");
        var pageContent = $pageContent[0] || $pageSkeletonContent[0];
        var pageSkeletonContent = $pageSkeletonContent[0];
        if (!pageContent) {
          return;
        }
        var pageWidth;
        var pageHeight;
        var box = $target.data("box");
        if (!box) {
          var margins = getMargins($target);
          var borders = getBorderWidth($page);
          var padding = getPadding($page);
          box = {
            padLeft: margins.left + borders.left + padding.left,
            padRight: margins.right + borders.right + padding.right,
            padTop: margins.top + borders.top + padding.top,
            padBottom: margins.bottom + borders.bottom + padding.bottom
          };
          $target.data("box", box);
        }
        if ($target.data("pageWidth") === void 0) {
          pageWidth = pageContent.offsetWidth;
          pageHeight = pageContent.offsetHeight;
          $target.data("pageWidth", pageWidth);
          $target.data("pageHeight", pageHeight);
        } else {
          pageWidth = $target.data("pageWidth");
          pageHeight = $target.data("pageHeight");
        }
        var scrollBarV = pageHeight > pageWidth && scaleMode === ScaleModes.FIT_PAGE_WIDTH ? 20 : 0;
        var scaleW = (pageContainer.clientWidth - scrollBarV - box.padLeft - box.padRight) / pageWidth;
        var scaleH = (pageContainer.clientHeight - 1 - box.padTop - box.padBottom) / pageHeight;
        if (scaleMode === ScaleModes.FIT_PAGE_WIDTH) {
          scale$1 = scaleW;
        } else if (!scale$1 || scaleMode === ScaleModes.FIT_PAGE) {
          scale$1 = Math.min(scaleW, scaleH);
        }
        if (uiFreezeCoordinator) {
          uiFreezeCoordinator.setScaleFactor(scale$1);
        }
        $target.data("pageScale", scale$1);
        $page.data("pageScale", scale$1);
        if (!pageSkeletonContent) {
          scale($pageContent, scale$1, scale$1);
        }
        $page.css({
          "height": scale$1 * pageHeight,
          "width": scale$1 * pageWidth
        });
      }
      function enableInteractivity() {
        $pageContainer.on("click", "[data-reporting-action]", onInteractiveItemClick);
        $pageContainer.on("mouseenter", "[data-reporting-action]", onInteractiveItemEnter);
        $pageContainer.on("mouseleave", "[data-reporting-action]", onInteractiveItemLeave);
        $pageContainer.on("mouseenter", "[data-tooltip-title],[data-tooltip-text]", onToolTipItemEnter);
        $pageContainer.on("mouseleave", "[data-tooltip-title],[data-tooltip-text]", onToolTipItemLeave);
      }
      function disableInteractivity() {
        $pageContainer.off("click", "[data-reporting-action]", onInteractiveItemClick);
        $pageContainer.off("mouseenter", "[data-reporting-action]", onInteractiveItemEnter);
        $pageContainer.off("mouseleave", "[data-reporting-action]", onInteractiveItemLeave);
        $pageContainer.off("mouseenter", "[data-tooltip-title],[data-tooltip-text]", onToolTipItemEnter);
        $pageContainer.off("mouseleave", "[data-tooltip-title],[data-tooltip-text]", onToolTipItemLeave);
      }
      function onInteractiveItemClick(event) {
        var $eventTarget = $(this);
        var actionId = $eventTarget.attr("data-reporting-action");
        var action = getAction(actionId);
        if (action) {
          navigateToPageOnDocReady = getNavigateToPageOnDocReady(event, action.Type);
          controller.executeReportAction({
            element: event.currentTarget,
            action,
            cancel: false
          });
        }
        event.stopPropagation();
      }
      function getNavigateToPageOnDocReady(event, actionType) {
        if (scroll.isEnabled() && (actionType === "sorting" || actionType === "toggleVisibility")) {
          return $(event.target).closest(".trv-report-page").attr("data-page") || controller.getCurrentPageNumber();
        }
        return controller.getCurrentPageNumber();
      }
      function onInteractiveItemEnter(args) {
        var $t = $(this);
        var actionId = $t.attr("data-reporting-action");
        var a = getAction(actionId);
        if (a !== null && args.currentTarget === this) {
          controller.reportActionEnter({
            element: args.currentTarget,
            action: a
          });
        }
      }
      function onInteractiveItemLeave(args) {
        var $t = $(this);
        var actionId = $t.attr("data-reporting-action");
        var a = getAction(actionId);
        if (a !== null && args.currentTarget === this) {
          controller.reportActionLeave({
            element: args.currentTarget,
            action: a
          });
        }
      }
      function getAction(actionId) {
        if (actions) {
          return Array.from(actions).find(function(action) {
            return action.Id === actionId;
          });
        }
        return null;
      }
      function onToolTipItemEnter(args) {
        var $t = $(this);
        var title = $t.attr("data-tooltip-title");
        var text = $t.attr("data-tooltip-text");
        if (!title && !text) {
          return;
        }
        var toolTipArgs = {
          element: args.currentTarget,
          toolTip: {
            title: title || "",
            text: text || ""
          },
          cancel: false
        };
        controller.reportToolTipOpening(toolTipArgs);
        if (toolTipArgs.cancel) {
          return;
        }
        var content = applyToolTipTemplate(toolTipArgs);
        var viewportElement = args.currentTarget.viewportElement;
        var ktt = getToolTip($t, content);
        ktt.show($t);
        if (viewportElement && viewportElement.nodeName === "svg") {
          positionToolTip(ktt, args);
        }
      }
      function applyToolTipTemplate(toolTipArgs) {
        var toolTipTemplate = options.templates["trv-pages-area-kendo-tooltip"];
        var $container = $(toolTipTemplate);
        var $titleSpan = $container.find(".trv-pages-area-kendo-tooltip-title");
        var $textSpan = $container.find(".trv-pages-area-kendo-tooltip-text");
        $titleSpan.html(replaceNewLineSymbols(toolTipArgs.toolTip.title));
        $textSpan.html(replaceNewLineSymbols(toolTipArgs.toolTip.text));
        return $container.clone().wrap("<p>").parent().html();
      }
      function replaceNewLineSymbols(tooltipText) {
        tooltipText = escapeHtml(tooltipText);
        tooltipText = tooltipText.replaceAll("\r\n", "<br>");
        tooltipText = tooltipText.replaceAll("\n", "<br>");
        return tooltipText;
      }
      function positionToolTip(toolTip, e) {
        var x = e.pageX;
        var y = e.pageY;
        var container = toolTip.popup.element.closest("[role=region]");
        container.css({
          left: x + 10,
          top: y + 5
        });
      }
      function getToolTip(target, toolTipContent) {
        var ktt = target.data("kendoTooltip");
        if (!ktt) {
          try {
            ktt = target.kendoTooltip({
              content: toolTipContent,
              autohide: true,
              callout: false
            }).data("kendoTooltip");
          } catch (e) {
            console.error("Instantiation of Kendo Tooltip threw an exception", e);
            throw e;
          }
        }
        return ktt;
      }
      function onToolTipItemLeave(args) {
        var $t = $(this);
        var toolTip = $t.data("kendoTooltip");
        if (toolTip) {
          toolTip.hide();
        }
      }
      function updatePageStyle(page) {
        var styleId = "trv-" + controller.clientId() + "-styles";
        $("#" + styleId).remove();
        var pageStyles = $("<style id=" + styleId + "></style>");
        pageStyles.append(page.pageStyles);
        pageStyles.appendTo("head");
      }
      function setPageContent(page) {
        actions = JSON.parse(page.pageActions);
        updatePageStyle(page);
        var pageNumber = page.pageNumber;
        var wrapper = $($.parseHTML(page.pageContent));
        var $pageContent = wrapper.find("div.sheet");
        var $page = $('<div class="trv-report-page" data-page="' + pageNumber + '"></div>');
        $pageContent.css("margin", 0);
        $page.append($pageContent).append($('<div class="trv-page-overlay"></div>'));
        var $target = $pageWrapper.empty().removeData().data("pageNumber", pageNumber).append($page);
        controller.setCurrentPageNumber(pageNumber);
        if (controller.getViewMode() === ViewModes.INTERACTIVE) {
          $placeholder.removeClass("printpreview");
          $placeholder.addClass("interactive");
        } else {
          $placeholder.removeClass("interactive");
          $placeholder.addClass("printpreview");
        }
        setPageDimensions($target, pageScaleMode, pageScale);
        $pageContainer.scrollTop(0);
        $pageContainer.scrollLeft(0);
        navigateToElement(pendingElement);
      }
      function setScrollablePage(args) {
        var pageActions = JSON.parse(args.page.pageActions);
        if (!actions) {
          actions = pageActions;
        } else {
          actions = actions.concat(pageActions);
        }
        if (controller.getViewMode() === ViewModes.INTERACTIVE) {
          $placeholder.removeClass("printpreview");
          $placeholder.addClass("interactive");
        } else {
          $placeholder.removeClass("interactive");
          $placeholder.addClass("printpreview");
        }
        setPageDimensions(args.target, pageScaleMode, pageScale, args.page.pageNumber);
      }
      function setPageAreaImage() {
        var pageStyles = $("<style id=" + pageAreaImageID + "></style>");
        clearPageAreaImage();
        pageStyles.append(stringFormat(pageAreaImageStyle, [options.initialPageAreaImageUrl]));
        pageStyles.appendTo("head");
        showPageAreaImage = true;
      }
      function clearPageAreaImage() {
        $("#" + pageAreaImageID).remove();
      }
    }

    var parameterEditorsMatch = {
      // AvailableValues PROVIDED, MultiValue is TRUE and trv.parameters.editors.multiSelect is unset
      MultiSelect: function(parameter, editorsType) {
        return Boolean(parameter.availableValues) && parameter.multivalue && (!editorsType || !editorsType.multiSelect || editorsType.multiSelect !== ParameterEditorTypes.COMBO_BOX);
      },
      // AvailableValues PROVIDED, MultiValue is TRUE and trv.parameters.editors.multiSelect is set to COMBO_BOX
      MultiSelectCombo: function(parameter, editorsType) {
        return Boolean(parameter.availableValues) && parameter.multivalue && (editorsType && editorsType.multiSelect && editorsType.multiSelect === ParameterEditorTypes.COMBO_BOX);
      },
      // AvailableValues PROVIDED, MultiValue is FALSE and trv.parameters.editors.singleSelect is unset
      SingleSelect: function(parameter, editorsType) {
        return Boolean(parameter.availableValues) && !parameter.multivalue && (!editorsType || !editorsType.singleSelect || editorsType.singleSelect !== ParameterEditorTypes.COMBO_BOX);
      },
      // AvailableValues PROVIDED, MultiValue is FALSE and trv.parameters.editors.singleSelect is set to COMBO_BOX
      SingleSelectCombo: function(parameter, editorsType) {
        return Boolean(parameter.availableValues) && !parameter.multivalue && (editorsType && editorsType.singleSelect && editorsType.singleSelect === ParameterEditorTypes.COMBO_BOX);
      },
      // AvailableValues NOT PROVIDED and MultiValue is TRUE
      MultiValue: function(parameter) {
        return Boolean(parameter.multivalue);
      },
      DateTime: function(parameter) {
        return parameter.type === ParameterTypes.DATETIME;
      },
      String: function(parameter) {
        return parameter.type === ParameterTypes.STRING;
      },
      Number: function(parameter) {
        switch (parameter.type) {
          case ParameterTypes.INTEGER:
          case ParameterTypes.FLOAT:
            return true;
          default:
            return false;
        }
      },
      Boolean: function(parameter) {
        return parameter.type === ParameterTypes.BOOLEAN;
      },
      Default: function(parameter) {
        return true;
      }
    };
    var multivalueUtils = function() {
      var lineSeparator = "\n";
      return {
        formatValue: function(value) {
          var text = "";
          if (value) {
            [].concat(value).forEach(function(val) {
              if (text.length > 0) {
                text += lineSeparator;
              }
              text += val;
            });
          }
          return text;
        },
        parseValues: function(text) {
          return ("" + text).split(lineSeparator);
        }
      };
    }();
    function integerInputBehavior(input) {
      function isValid(newValue) {
        return /^(\-|\+)?([0-9]*)$/.test(newValue);
      }
      function onKeyPress(event) {
        if (isSpecialKey(event.keyCode)) {
          return true;
        }
        return isValid($(input).val() + String.fromCharCode(event.charCode));
      }
      function onPaste(event) {
      }
      function attach(input2) {
        $(input2).on("keypress", onKeyPress).on("paste", onPaste);
      }
      function detach(input2) {
        $(input2).off("keypress", onKeyPress).off("paste", onPaste);
      }
      attach(input);
      return {
        dispose: function() {
          detach(input);
        }
      };
    }
    function floatInputBehavior(input) {
      function isValid(newValue) {
        return /^(\-|\+)?([0-9]*(\.[0-9]*)?)$/.test(newValue);
      }
      function onKeyPress(event) {
        if (isSpecialKey(event.keyCode)) {
          return true;
        }
        return isValid($(input).val() + String.fromCharCode(event.charCode));
      }
      function onPaste(event) {
      }
      function attach(input2) {
        $(input2).on("keypress", onKeyPress).on("paste", onPaste);
      }
      function detach(input2) {
        $(input2).off("keypress", onKeyPress).off("paste", onPaste);
      }
      attach(input);
      return {
        dispose: function() {
          detach(input);
        }
      };
    }
    function applyClass(apply, cssClass, item) {
      var fn = apply ? $.fn.addClass : $.fn.removeClass;
      fn.call(item, cssClass);
    }
    function enableItem(item, enable) {
      applyClass(!enable, "k-disabled", item);
    }
    function selectItem(item, select) {
      applyClass(select, "k-selected", item);
      item.attr("aria-selected", select);
    }
    function addAccessibilityAttributes(editor, type, caption, additionalInfo, error) {
      if (!additionalInfo) {
        additionalInfo = "";
      }
      var label = stringFormat("{0}. {1} {2}. {3}", [caption, type, stringResources.ariaLabelParameter, additionalInfo]);
      editor.attr("aria-label", label);
      setAccessibilityErrorAttributes(editor, error);
    }
    var containerTabIndex;
    var editorsIndex = 0;
    function setEditorTabIndex(editor) {
      if (!containerTabIndex) {
        var $container = $("div.trv-parameters-area-content");
        if ($container.length > 0) {
          var tabIndexAttr = $container.attr("tabIndex");
          if (tabIndexAttr) {
            containerTabIndex = tryParseInt(tabIndexAttr);
          }
        }
        if (!containerTabIndex || isNaN(containerTabIndex)) {
          containerTabIndex = 300;
        }
      }
      var wrapper = editor.closest(".trv-parameter-value");
      var selectAll = wrapper.find(".trv-select-all");
      var clearSelection = wrapper.find(".trv-select-none");
      var widgetParent = editor.closest(".k-widget");
      var hasFocusableElement = widgetParent.find(".k-input");
      var isComboWidget = hasFocusableElement && hasFocusableElement.length;
      if (selectAll && selectAll.length) {
        selectAll.attr("tabindex", containerTabIndex + ++editorsIndex);
      }
      if (clearSelection && clearSelection.length) {
        clearSelection.attr("tabindex", containerTabIndex + ++editorsIndex);
      }
      if (isComboWidget) {
        hasFocusableElement.attr("tabindex", containerTabIndex + ++editorsIndex);
      } else {
        editor.attr("tabindex", containerTabIndex + ++editorsIndex);
      }
    }
    function setAccessibilityErrorAttributes(editor, error) {
      var errToken = stringFormat(" {0}:", [stringResources.ariaLabelErrorMessage]);
      var label = editor.attr("aria-label");
      if (!label) {
        return;
      }
      var errIdx = label.indexOf(errToken);
      if (errIdx > -1) {
        label = label.substring(0, errIdx);
      }
      if (error && error !== "") {
        editor.attr("aria-required", true);
        editor.attr("aria-invalid", true);
        label += errToken + error;
      } else {
        editor.removeAttr("aria-invalid");
      }
      editor.attr("aria-label", label);
    }
    function navigatableEnabledForList(enableAccessibility) {
      return kendo.version >= "2017.3.1018" || enableAccessibility;
    }
    var ParameterEditors = [
      {
        match: parameterEditorsMatch.MultiSelect,
        createEditor: function(placeholder, options) {
          var $placeholder = $(placeholder);
          var enabled = true;
          $placeholder.html(options.templates["trv-parameter-editor-available-values-multiselect"]);
          var $list = $placeholder.find(".trv-list");
          var $selectAll = $placeholder.find(".trv-select-all");
          var $selectNone = $placeholder.find(".trv-select-none");
          var listView;
          var parameter;
          var updateTimeout;
          var valueChangeCallback = options.parameterChanged;
          var initialized;
          $selectAll.text(stringResources[$selectAll.text()]);
          $selectAll.click(function(e) {
            e.preventDefault();
            if (!enabled)
              return;
            setSelectedItems(parameter.availableValues.map(function(av) {
              return av.value;
            }));
          });
          $selectNone.text(stringResources[$selectNone.text()]);
          $selectNone.click(function(e) {
            e.preventDefault();
            if (!enabled)
              return;
            setSelectedItems([]);
          });
          function onSelectionChanged(selection) {
            if (initialized) {
              applyAriaSelected(selection);
              notifyParameterChanged(selection);
            }
          }
          function applyAriaSelected(selection) {
            var children = $list.find(".trv-listviewitem");
            Array.from(children).forEach((item) => {
              var $item = $(item);
              var isSelected = selection.filter($item).length > 0;
              $item.attr("aria-selected", isSelected);
            });
          }
          function notifyParameterChanged(selection) {
            var availableValues = parameter.availableValues;
            var values = $.map(selection, function(item) {
              return availableValues[$(item).index()].value;
            });
            clearPendingChange();
            var immediateUpdate = !parameter.autoRefresh && !parameter.childParameters;
            updateTimeout = window.setTimeout(
              function() {
                if (!areEqualArrays(parameter.value, values)) {
                  valueChangeCallback(parameter, values);
                }
                updateTimeout = null;
              },
              immediateUpdate ? 0 : 1e3
            );
          }
          function clearPendingChange() {
            if (updateTimeout) {
              window.clearTimeout(updateTimeout);
            }
          }
          function getSelectedItems() {
            return $(listView.element).find(".k-selected");
          }
          function onItemClick(e) {
            if (!enabled)
              return;
            var clickedItem = $(e.target);
            var selectedItems = listView.select();
            if (clickedItem.hasClass("k-selected")) {
              selectedItems.splice($.inArray(clickedItem[0], selectedItems), 1);
            } else {
              selectedItems.push(clickedItem);
            }
            listView.clearSelection();
            listView.select(selectedItems);
            listView.trigger("change");
          }
          function onChange(e) {
            onSelectionChanged(getSelectedItems());
          }
          function onKeydown(event) {
            if (!enabled)
              return;
            if (event.which !== 32) {
              return;
            }
            var focused = listView.element.find(".k-focus");
            if (focused.length > 0) {
              focused.toggleClass("k-selected");
              onSelectionChanged(getSelectedItems());
              event.preventDefault();
            }
          }
          function init() {
            setEditorTabIndex($list);
            setSelectedItems(parameter.value);
            listView.element.off().on("touch click", ".trv-listviewitem", onItemClick);
            listView.element.on("keydown", onKeydown);
            initialized = true;
          }
          function clear() {
            initialized = false;
            if (listView) {
              listView.element.off("touch click", ".trv-listviewitem", onItemClick);
              listView.element.off("keydown", onKeydown);
            }
          }
          function setSelectedItems(items) {
            setSelectedItemsCore(items);
            onSelectionChanged(getSelectedItems());
          }
          function setSelectedItemsCore(items) {
            if (!Array.isArray(items)) {
              items = [items];
            }
            var children = $list.find(".trv-listviewitem");
            $.each(parameter.availableValues, (i, av) => {
              var selected = false;
              $.each(items, (j, v) => {
                var availableValue = av.value;
                if (v instanceof Date) {
                  availableValue = parseToLocalDate(av.value);
                }
                selected = areEqual(v, availableValue);
                return !selected;
              });
              selectItem($(children[i]), selected);
            });
          }
          return {
            beginEdit: function(param) {
              clear();
              parameter = param;
              try {
                listView = $list.kendoListView({
                  template: kendo.template('<div class="trv-listviewitem" style="cursor: pointer">${name}</div>'),
                  dataSource: { data: parameter.availableValues },
                  selectable: "MULTIPLE",
                  navigatable: navigatableEnabledForList(options.enableAccessibility),
                  change: onChange
                }).data("kendoListView");
              } catch (e) {
                console.error("Instantiation of Kendo ListView as MultiSelect parameter editor threw an exception", e);
                throw e;
              }
              init();
            },
            enable: function(enable) {
              enabled = enable;
              enableItem($list, enabled);
            },
            clearPendingChange,
            addAccessibility: function(param) {
              var info = stringFormat(stringResources.ariaLabelParameterInfo, [param.availableValues.length]);
              addAccessibilityAttributes($list, stringResources.ariaLabelMultiSelect, param.text, info, param.Error);
              $list.attr("aria-multiselectable", "true");
              var items = $list.find(".trv-listviewitem");
              Array.from(items).forEach((item) => {
                $(item).attr("aria-label", item.innerText);
              });
            },
            setAccessibilityErrorState: function(param) {
              setAccessibilityErrorAttributes($list, param.Error);
            },
            destroy: function() {
              listView.destroy();
            }
          };
        }
      },
      {
        match: parameterEditorsMatch.MultiSelectCombo,
        createEditor: function(placeholder, options) {
          var $placeholder = $(placeholder);
          var enabled = true;
          var selector = ".trv-combo";
          var template = "trv-parameter-editor-available-values-multiselect-combo";
          var valueChangeCallback = options.parameterChanged;
          var $editorDom;
          var $selectNone;
          var $selectAll;
          var editor;
          var updateTimeout;
          var popUpIsClosed = true;
          var parameter;
          $placeholder.html(options.templates[template]);
          $editorDom = $placeholder.find(selector);
          $selectNone = $placeholder.find(".trv-select-none");
          if ($selectNone) {
            $selectNone.text(stringResources[$selectNone.text()]);
            $selectNone.click(function(e) {
              e.preventDefault();
              editor.value([]);
              editor.trigger("change");
            });
          }
          $selectAll = $placeholder.find(".trv-select-all");
          if ($selectAll) {
            $selectAll.text(stringResources[$selectAll.text()]);
            $selectAll.click(function(e) {
              e.preventDefault();
              if (!enabled)
                return;
              var values = $.map(parameter.availableValues, function(dataItem) {
                return dataItem.value;
              });
              editor.value(values);
              editor.trigger("change");
            });
          }
          function onSelectionChanged(selection) {
            notifyParameterChanged(selection);
          }
          function notifyParameterChanged(values) {
            clearPendingChange();
            var immediateUpdate = !parameter.autoRefresh && !parameter.childParameters;
            updateTimeout = window.setTimeout(
              function() {
                if (!areEqualArrays(parameter.value, values)) {
                  valueChangeCallback(parameter, values);
                }
                updateTimeout = null;
              },
              immediateUpdate ? 0 : 1e3
            );
          }
          function clearPendingChange() {
            if (updateTimeout) {
              window.clearTimeout(updateTimeout);
            }
          }
          function getSelectedItems() {
            return editor.value();
          }
          function onChange() {
            if (popUpIsClosed) {
              onSelectionChanged(getSelectedItems());
            }
          }
          function init() {
            setEditorTabIndex($editorDom);
            editor.bind("change", onChange);
          }
          function reset() {
            if (editor) {
              editor.unbind("change", onChange);
            }
          }
          return {
            beginEdit: function(param) {
              reset();
              parameter = param;
              try {
                editor = $editorDom.kendoMultiSelect({
                  itemTemplate: '<div class="trv-editoritem">${name}</div>',
                  dataSource: parameter.availableValues,
                  dataTextField: "name",
                  dataValueField: "value",
                  value: parameter.value,
                  filter: "contains",
                  autoClose: false,
                  open: function() {
                    popUpIsClosed = false;
                  },
                  close: function(e) {
                    popUpIsClosed = true;
                    onChange();
                  },
                  autoWidth: true,
                  clearButton: false
                }).data("kendoMultiSelect");
              } catch (e) {
                console.error("Instantiation of Kendo MultiSelect as MultiSelectCombo parameter editor threw an exception", e);
                throw e;
              }
              init();
            },
            enable: function(enable) {
              enabled = enable;
              editor.enable(enable);
            },
            clearPendingChange,
            addAccessibility: function(param) {
              var $accessibilityDom = editor.input;
              var info = stringFormat(stringResources.ariaLabelParameterInfo, [param.availableValues.length]);
              addAccessibilityAttributes($accessibilityDom, stringResources.ariaLabelMultiSelect, param.text, info, param.Error);
              var items = editor.items();
              Array.from(items).forEach((item) => {
                $(item).attr("aria-label", item.innerText);
              });
            },
            setAccessibilityErrorState: function(param) {
              setAccessibilityErrorAttributes($editorDom, param.Error);
            },
            destroy: function() {
              editor.destroy();
            }
          };
        }
      },
      {
        match: parameterEditorsMatch.SingleSelect,
        createEditor: function(placeholder, options) {
          var $placeholder = $(placeholder);
          var enabled = true;
          $placeholder.html(options.templates["trv-parameter-editor-available-values"]);
          var $list = $placeholder.find(".trv-list");
          var $selectNone = $placeholder.find(".trv-select-none");
          var listView;
          var parameter;
          var valueChangeCallback = options.parameterChanged;
          if ($selectNone) {
            $selectNone.text(stringResources[$selectNone.text()]);
            $selectNone.click(function(e) {
              e.preventDefault();
              listView.clearSelection();
              listView.trigger("change");
            });
          }
          function onSelectionChanged(selection) {
            notifyParameterChanged(selection);
          }
          function notifyParameterChanged(selection) {
            var availableValues = parameter.availableValues;
            var values = $.map(selection, function(item) {
              return availableValues[$(item).index()].value;
            });
            if (Array.isArray(values)) {
              values = values[0];
            }
            valueChangeCallback(parameter, values);
          }
          function getSelectedItems() {
            return listView.select();
          }
          function onChange() {
            onSelectionChanged(getSelectedItems());
          }
          function init() {
            setEditorTabIndex($list);
            setSelectedItems(parameter.value);
            listView.bind("change", onChange);
          }
          function reset() {
            if (listView) {
              listView.unbind("change", onChange);
            }
          }
          function setSelectedItems(value) {
            var items = $list.find(".trv-listviewitem");
            $.each(parameter.availableValues, (i, av) => {
              var availableValue = av.value;
              if (value instanceof Date) {
                availableValue = parseToLocalDate(av.value);
              }
              if (areEqual(value, availableValue)) {
                listView.select(items[i]);
                return false;
              }
              return true;
            });
          }
          return {
            beginEdit: function(param) {
              reset();
              parameter = param;
              try {
                listView = $list.kendoListView({
                  template: '<div class="trv-listviewitem">${name}</div>',
                  dataSource: { data: parameter.availableValues },
                  selectable: true,
                  navigatable: navigatableEnabledForList(options.enableAccessibility)
                }).data("kendoListView");
              } catch (e) {
                console.error("Instantiation of Kendo ListView as SingleSelect parameter editor threw an exception", e);
                throw e;
              }
              init();
            },
            enable: function(enable) {
              enabled = enable;
              enableItem($list, enabled);
              if (enabled) {
                listView.bind("change", onChange);
                $list.addClass("k-selectable");
              } else {
                listView.unbind("change", onChange);
                $list.removeClass("k-selectable");
              }
            },
            addAccessibility: function(param) {
              var info = stringFormat(stringResources.ariaLabelParameterInfo, [param.availableValues.length]);
              addAccessibilityAttributes($list, stringResources.ariaLabelSingleValue, param.text, info, param.Error);
              var items = $list.find(".trv-listviewitem");
              Array.from(items).forEach((item) => {
                $(item).attr("aria-label", item.innerText);
              });
            },
            setAccessibilityErrorState: function(param) {
              setAccessibilityErrorAttributes($list, param.Error);
            },
            destroy: function() {
              listView.destroy();
            }
          };
        }
      },
      {
        match: parameterEditorsMatch.SingleSelectCombo,
        createEditor: function(placeholder, options) {
          var $placeholder = $(placeholder);
          var selector = ".trv-combo";
          var template = "trv-parameter-editor-available-values-combo";
          var valueChangeCallback = options.parameterChanged;
          var $editorDom;
          var $selectNone;
          var editor;
          var parameter;
          $placeholder.html(options.templates[template]);
          $editorDom = $placeholder.find(selector);
          $selectNone = $placeholder.find(".trv-select-none");
          if ($selectNone) {
            $selectNone.text(stringResources[$selectNone.text()]);
            $selectNone.click(function(e) {
              e.preventDefault();
              editor.value("");
              editor.trigger("change");
            });
          }
          function onSelectionChanged(selection, value) {
            notifyParameterChanged(selection, value);
          }
          function notifyParameterChanged(selection, value) {
            var values = value || "";
            var availableValues;
            if (!value && selection >= 0) {
              availableValues = parameter.availableValues;
              values = availableValues[selection].value;
            }
            valueChangeCallback(parameter, values);
          }
          function getSelectedItems() {
            return editor.select();
          }
          function onChange(e) {
            onSelectionChanged(getSelectedItems(), this.value());
          }
          function init() {
            setEditorTabIndex($editorDom);
            editor.bind("change", onChange);
          }
          function reset() {
            if (editor) {
              editor.unbind("change", onChange);
            }
          }
          return {
            beginEdit: function(param) {
              reset();
              parameter = param;
              try {
                editor = $editorDom.kendoComboBox({
                  template: '<div class="trv-editoritem">${name}</div>',
                  dataSource: parameter.availableValues,
                  dataTextField: "name",
                  dataValueField: "value",
                  value: parameter.value,
                  filter: "contains",
                  suggest: true,
                  clearButton: false
                }).data("kendoComboBox");
              } catch (e) {
                console.error("Instantiation of Kendo ComboBox as SingleSelectCombo parameter editor threw an exception", e);
                throw e;
              }
              init();
            },
            enable: function(enable) {
              editor.enable(enable);
            },
            addAccessibility: function(param) {
              var $accessibilityDom = editor.input;
              var info = stringFormat(stringResources.ariaLabelParameterInfo, [param.availableValues.length]);
              addAccessibilityAttributes($accessibilityDom, stringResources.ariaLabelSingleValue, param.text, info, param.Error);
              var items = editor.items();
              Array.from(items).forEach((item) => {
                $(item).attr("aria-label", item.innerText);
              });
            },
            setAccessibilityErrorState: function(param) {
              setAccessibilityErrorAttributes($editorDom, param.Error);
            },
            destroy: function() {
              editor.destroy();
            }
          };
        }
      },
      {
        match: parameterEditorsMatch.MultiValue,
        createEditor: function(placeholder, options) {
          var $placeholder = $(placeholder);
          var parameter;
          $placeholder.html(options.templates["trv-parameter-editor-multivalue"]);
          var $textArea = $placeholder.find("textarea").on("change", function() {
            if (options.parameterChanged) {
              options.parameterChanged(parameter, multivalueUtils.parseValues(this.value));
            }
          });
          function setValue(value) {
            parameter.value = value;
            $textArea.val(multivalueUtils.formatValue(value));
          }
          return {
            beginEdit: function(param) {
              parameter = param;
              setValue(param.value);
              setEditorTabIndex($textArea);
            },
            enable: function(enable) {
              enableItem($textArea, enable);
              $textArea.prop("disabled", !enable);
            },
            addAccessibility: function(param) {
              addAccessibilityAttributes($textArea, stringResources.ariaLabelMultiValue, param.text, null, param.Error);
            },
            setAccessibilityErrorState: function(param) {
              setAccessibilityErrorAttributes($textArea, param.Error);
            }
          };
        }
      },
      {
        match: parameterEditorsMatch.DateTime,
        createEditor: function(placeholder, options) {
          var $placeholder = $(placeholder);
          var parameter;
          $placeholder.html(options.templates["trv-parameter-editor-datetime"]);
          try {
            var $dateTimePicker = $placeholder.find("input[type=datetime]").kendoDatePicker({
              change: function() {
                var handler = options.parameterChanged;
                if (handler) {
                  var dtv = this.value();
                  if (null !== dtv) {
                    dtv = adjustTimezone(dtv);
                  }
                  handler(parameter, dtv);
                }
              }
            });
            var dateTimePicker = $dateTimePicker.data("kendoDatePicker");
          } catch (e) {
            console.error("Instantiation of Kendo DatePicker as DateTime parameter editor threw an exception", e);
            throw e;
          }
          function setValue(value) {
            parameter.value = value;
            var dt = null;
            try {
              if (value) {
                dt = unadjustTimezone(value);
              }
            } catch (e) {
              dt = null;
            }
            dateTimePicker.value(dt);
          }
          return {
            beginEdit: function(param) {
              parameter = param;
              setValue(param.value);
              setEditorTabIndex($dateTimePicker);
            },
            enable: function(enable) {
              dateTimePicker.enable(enable);
              enableItem($dateTimePicker, enable);
            },
            addAccessibility: function(param) {
              addAccessibilityAttributes($dateTimePicker, stringResources.ariaLabelParameterDateTime, param.text, null, param.Error);
              $dateTimePicker.attr("aria-live", "assertive");
            },
            setAccessibilityErrorState: function(param) {
              setAccessibilityErrorAttributes($dateTimePicker, param.Error);
            },
            destroy: function() {
              dateTimePicker.destroy();
            }
          };
        }
      },
      {
        match: parameterEditorsMatch.String,
        createEditor: function(placeholder, options) {
          var $placeholder = $(placeholder);
          var parameter;
          $placeholder.html(options.templates["trv-parameter-editor-text"]);
          var $input = $placeholder.find('input[type="text"]').change(function() {
            if (options.parameterChanged) {
              options.parameterChanged(parameter, $input.val());
            }
          });
          function setValue(value) {
            parameter.value = value;
            $input.val(value);
          }
          return {
            beginEdit: function(param) {
              parameter = param;
              setValue(param.value);
              setEditorTabIndex($input);
            },
            enable: function(enabled) {
              $input.prop("disabled", !enabled);
              enableItem($input, enabled);
            },
            addAccessibility: function(param) {
              addAccessibilityAttributes($input, stringResources.ariaLabelParameterString, param.text, null, param.Error);
              $input.attr("aria-live", "assertive");
            },
            setAccessibilityErrorState: function(param) {
              setAccessibilityErrorAttributes($input, param.Error);
            }
          };
        }
      },
      {
        match: parameterEditorsMatch.Number,
        createEditor: function(placeholder, options) {
          var $placeholder = $(placeholder);
          var parameter;
          var inputBehavior;
          $placeholder.html(options.templates["trv-parameter-editor-number"]);
          var $input = $placeholder.find("input[type=number]").on("change", function() {
            if (options.parameterChanged) {
              options.parameterChanged(parameter, $input.val());
            }
          });
          return {
            beginEdit: function(param) {
              if (inputBehavior) {
                inputBehavior.dispose();
              }
              parameter = param;
              $input.val(parameter.value);
              if (parameter.type === ParameterTypes.INTEGER) {
                inputBehavior = integerInputBehavior($input);
              } else {
                inputBehavior = floatInputBehavior($input);
              }
              setEditorTabIndex($input);
            },
            enable: function(enable) {
              $input.prop("disabled", !enable);
              enableItem($input, enable);
            },
            addAccessibility: function(param) {
              addAccessibilityAttributes($input, stringResources.ariaLabelParameterNumerical, param.text, null, param.Error);
              $input.attr("aria-live", "assertive");
            },
            setAccessibilityErrorState: function(param) {
              setAccessibilityErrorAttributes($input, param.Error);
            }
          };
        }
      },
      {
        match: parameterEditorsMatch.Boolean,
        createEditor: function(placeholder, options) {
          var $placeholder = $(placeholder);
          var parameter;
          $placeholder.html(options.templates["trv-parameter-editor-boolean"]);
          var $input = $placeholder.find("input[type=checkbox]").on("change", function() {
            if (options.parameterChanged) {
              options.parameterChanged(parameter, this.checked);
            }
          });
          function setValue(value) {
            parameter.value = value;
            $input[0].checked = value === true;
          }
          return {
            beginEdit: function(param) {
              parameter = param;
              setValue(param.value);
              setEditorTabIndex($input);
            },
            enable: function(enable) {
              enableItem($input, enable);
              $input.attr("disabled", !enable);
            },
            addAccessibility: function(param) {
              addAccessibilityAttributes($input, stringResources.ariaLabelParameterBoolean, param.text, null, param.Error);
              $input.attr("aria-live", "assertive");
            },
            setAccessibilityErrorState: function(param) {
              setAccessibilityErrorAttributes($input, param.Error);
            }
          };
        }
      },
      {
        match: parameterEditorsMatch.Default,
        createEditor: function(placeholder, options) {
          var $placeholder = $(placeholder);
          $placeholder.html('<div class="trv-parameter-editor-generic"></div>');
          return {
            beginEdit: function(parameter) {
              $placeholder.find(".trv-parameter-editor-generic").text(parameter.Error ? "(error)" : parameter.value);
            },
            enable: function(enable) {
            }
          };
        }
      }
    ];

    function PerspectiveManager(dom, controller) {
      var smallMenu = dom.querySelectorAll ? dom.querySelectorAll(".trv-menu-small")[0] : false;
      var perspectives = {
        "small": {
          documentMapVisible: false,
          parametersAreaVisible: false,
          onDocumentMapVisibleChanged: function(e, args) {
            if (args.visible) {
              controller.setParametersAreaVisible({ visible: false });
            }
          },
          onParameterAreaVisibleChanged: function(e, args) {
            if (args.visible) {
              controller.setDocumentMapVisible({ visible: false });
            }
          },
          onBeforeLoadReport: function() {
            controller.setParametersAreaVisible({ visible: false });
            controller.setDocumentMapVisible({ visible: false });
          },
          onNavigateToPage: function() {
            controller.setParametersAreaVisible({ visible: false });
            controller.setDocumentMapVisible({ visible: false });
          }
        },
        "large": {
          documentMapVisible: true,
          parametersAreaVisible: true
        }
      };
      var currentPerspective;
      function init() {
        currentPerspective = getPerspective();
        initStateFromController(perspectives["large"]);
      }
      function setPerspective(beforeApplyState) {
        var perspective = getPerspective();
        if (perspective !== currentPerspective) {
          var oldState = perspectives[currentPerspective];
          var newState = perspectives[perspective];
          currentPerspective = perspective;
          if (beforeApplyState) {
            beforeApplyState.call(void 0, oldState, newState);
          }
          applyState(newState);
        }
      }
      function onDocumentMapVisibleChanged(e, args) {
        dispatch("onDocumentMapVisibleChanged", arguments);
      }
      function onParameterAreaVisibleChanged(e, args) {
        dispatch("onParameterAreaVisibleChanged", arguments);
      }
      function onBeforeLoadReport() {
        dispatch("onBeforeLoadReport", arguments);
      }
      function onNavigateToPage() {
        dispatch("onNavigateToPage", arguments);
      }
      function onReportLoadComplete() {
        dispatch("onReportLoadComplete", arguments);
      }
      function onWindowResize() {
        setPerspective(
          function(oldState, newState) {
            initStateFromController(oldState);
          }
        );
      }
      function onCssLoaded() {
        setPerspective(null);
      }
      function dispatch(func, args) {
        var activePerspective = perspectives[currentPerspective];
        var handler = activePerspective[func];
        if (typeof handler === "function") {
          handler.apply(activePerspective, args);
        }
      }
      function attach() {
        window.addEventListener("resize", onWindowResize);
        controller.setDocumentMapVisible(onDocumentMapVisibleChanged);
        controller.setParametersAreaVisible(onParameterAreaVisibleChanged);
        controller.beforeLoadReport(onBeforeLoadReport);
        controller.navigateToPage(onNavigateToPage);
        controller.reportLoadComplete(onReportLoadComplete);
        controller.cssLoaded(onCssLoaded);
      }
      function dispose() {
        window.removeEventListener("resize", onWindowResize);
      }
      function getPerspective() {
        var windowWidthInEm = $(window).width() / parseFloat($("body").css("font-size"));
        var windowMinWidth = 40.5;
        return smallMenu && windowWidthInEm <= windowMinWidth ? "small" : "large";
      }
      function initStateFromController(state) {
        state.documentMapVisible = documentMapVisible();
        state.parametersAreaVisible = parametersAreaVisible();
      }
      function applyState(state) {
        documentMapVisible(state.documentMapVisible);
        parametersAreaVisible(state.parametersAreaVisible);
      }
      function documentMapVisible() {
        if (arguments.length === 0) {
          var args1 = {};
          controller.getDocumentMapState(args1);
          return args1.visible;
        }
        controller.setDocumentMapVisible({
          visible: Boolean(arguments[0])
        });
        return this;
      }
      function parametersAreaVisible() {
        if (arguments.length === 0) {
          var args1 = {};
          controller.getParametersAreaState(args1);
          return args1.visible;
        }
        controller.setParametersAreaVisible({
          visible: Boolean(arguments[0])
        });
        return this;
      }
      init();
      return {
        attach,
        dispose
      };
    }

    var __defProp$5 = Object.defineProperty;
    var __getOwnPropSymbols = Object.getOwnPropertySymbols;
    var __hasOwnProp = Object.prototype.hasOwnProperty;
    var __propIsEnum = Object.prototype.propertyIsEnumerable;
    var __defNormalProp$5 = (obj, key, value) => key in obj ? __defProp$5(obj, key, { enumerable: true, configurable: true, writable: true, value }) : obj[key] = value;
    var __spreadValues = (a, b) => {
      for (var prop in b || (b = {}))
        if (__hasOwnProp.call(b, prop))
          __defNormalProp$5(a, prop, b[prop]);
      if (__getOwnPropSymbols)
        for (var prop of __getOwnPropSymbols(b)) {
          if (__propIsEnum.call(b, prop))
            __defNormalProp$5(a, prop, b[prop]);
        }
      return a;
    };
    var JSON_CONTENT_TYPE = "application/json; charset=UTF-8";
    var URLENCODED_CONTENT_TYPE = "application/x-www-form-urlencoded; charset=UTF-8";
    var HTTP_GET = "GET";
    var HTTP_POST = "POST";
    var HTTP_DELETE = "DELETE";
    var defaultOptions$4 = {};
    function getHeaders(authorizationToken) {
      const headers = {
        "Accept": "application/json, text/javascript, */*; q=0.01"
      };
      if (authorizationToken) {
        headers["Authorization"] = "Bearer " + authorizationToken;
      }
      return headers;
    }
    function ServiceClient(options) {
      options = $.extend({}, defaultOptions$4, options);
      var baseUrl = rTrim(options.serviceUrl || options.baseUrl, "\\/");
      var loginPromise;
      var _ajax = $ajax;
      function validateClientID(clientID) {
        if (!clientID) {
          throw "Invalid clientID";
        }
      }
      function urlFromTemplate(template, args) {
        args = $.extend({}, { baseUrl }, args);
        return stringFormat(template, args);
      }
      function login() {
        if (!loginPromise) {
          var loginInfo = options.loginInfo;
          if (loginInfo && loginInfo.url && (loginInfo.username || loginInfo.password)) {
            const endPoint = loginInfo.url;
            loginPromise = _ajax(endPoint, {
              method: HTTP_POST,
              data: {
                grant_type: "password",
                username: loginInfo.username,
                password: loginInfo.password
              },
              dataType: "json",
              contentType: URLENCODED_CONTENT_TYPE
            }).then(function(result) {
              return result.access_token;
            });
          } else {
            loginPromise = Promise.resolve();
          }
        }
        return loginPromise;
      }
      return {
        _urlFromTemplate: urlFromTemplate,
        registerClient: function(settings) {
          const endPoint = `${baseUrl}/clients`;
          return login().then(function(authorizationToken) {
            return _ajax(endPoint, __spreadValues({
              headers: getHeaders(authorizationToken),
              method: HTTP_POST,
              dataType: "json",
              // Used to avoid Chrome caching functionality for simple async requests
              // when the first request is not completed the second request is not send
              // and the same response is used for both of the request. In this case the
              // second client is not registered to the service and the same clientId is used.
              data: JSON.stringify({ timeStamp: Date.now() })
            }, settings));
          }).then(function(clientData) {
            if (clientData.Message) {
              throw clientData.Message;
            }
            return clientData.clientId;
          });
        },
        unregisterClient: function(clientID, settings) {
          validateClientID(clientID);
          const endPoint = `${baseUrl}/clients/${clientID}`;
          return login().then(function(authorizationToken) {
            return _ajax(endPoint, __spreadValues({
              headers: getHeaders(authorizationToken),
              method: HTTP_DELETE
            }, settings));
          });
        },
        getParameters: function(clientID, report, parameterValues, settings) {
          validateClientID(clientID);
          const endPoint = `${baseUrl}/clients/${clientID}/parameters`;
          return login().then(function(authorizationToken) {
            return _ajax(endPoint, __spreadValues({
              headers: getHeaders(authorizationToken),
              method: HTTP_POST,
              contentType: JSON_CONTENT_TYPE,
              dataType: "json",
              data: JSON.stringify({ report, parameterValues })
            }, settings));
          });
        },
        createReportInstance: function(clientID, report, parameterValues, settings) {
          validateClientID(clientID);
          const endPoint = `${baseUrl}/clients/${clientID}/instances`;
          return login().then(function(authorizationToken) {
            return _ajax(endPoint, __spreadValues({
              headers: getHeaders(authorizationToken),
              method: HTTP_POST,
              contentType: JSON_CONTENT_TYPE,
              dataType: "json",
              data: JSON.stringify({ report, parameterValues })
            }, settings));
          }).then(function(instanceData) {
            return instanceData.instanceId;
          });
        },
        deleteReportInstance: function(clientID, instanceID, settings) {
          validateClientID(clientID);
          const endPoint = `${baseUrl}/clients/${clientID}/instances/${instanceID}`;
          return login().then(function(authorizationToken) {
            return _ajax(endPoint, __spreadValues({
              headers: getHeaders(authorizationToken),
              method: HTTP_DELETE
            }, settings));
          });
        },
        // eslint-disable-next-line max-params
        createReportDocument: function(clientID, instanceID, format, deviceInfo, useCache, baseDocumentID, actionID, settings) {
          validateClientID(clientID);
          const endPoint = `${baseUrl}/clients/${clientID}/instances/${instanceID}/documents`;
          return login().then(function(authorizationToken) {
            deviceInfo = deviceInfo || {};
            deviceInfo["BasePath"] = baseUrl;
            return _ajax(endPoint, __spreadValues({
              headers: getHeaders(authorizationToken),
              method: HTTP_POST,
              contentType: JSON_CONTENT_TYPE,
              dataType: "json",
              data: JSON.stringify({
                format,
                deviceInfo,
                useCache,
                baseDocumentID,
                actionID
              })
            }, settings));
          }).then(function(documentData) {
            return documentData.documentId;
          });
        },
        // eslint-disable-next-line max-params
        sendDocument: function(clientID, instanceID, documentID, mailArgs, settings) {
          validateClientID(clientID);
          const endPoint = `${baseUrl}/clients/${clientID}/instances/${instanceID}/documents/${documentID}/send`;
          return login().then(function(authorizationToken) {
            return _ajax(endPoint, __spreadValues({
              headers: getHeaders(authorizationToken),
              method: HTTP_POST,
              contentType: JSON_CONTENT_TYPE,
              data: JSON.stringify({
                from: mailArgs.from,
                to: mailArgs.to,
                cc: mailArgs.cc,
                subject: mailArgs.subject,
                body: mailArgs.body
              })
            }, settings));
          });
        },
        deleteReportDocument: function(clientID, instanceID, documentID, settings) {
          validateClientID(clientID);
          const endPoint = `${baseUrl}/clients/${clientID}/instances/${instanceID}/documents/${documentID}`;
          return login().then(function(authorizationToken) {
            return _ajax(endPoint, __spreadValues({
              headers: getHeaders(authorizationToken),
              method: HTTP_DELETE
            }, settings));
          });
        },
        getDocumentInfo: function(clientID, instanceID, documentID, settings) {
          validateClientID(clientID);
          const endPoint = `${baseUrl}/clients/${clientID}/instances/${instanceID}/documents/${documentID}/info`;
          return login().then(function(authorizationToken) {
            return _ajax(endPoint, __spreadValues({
              headers: getHeaders(authorizationToken),
              method: HTTP_GET,
              dataType: "json"
            }, settings));
          });
        },
        // eslint-disable-next-line max-params
        getPage: function(clientID, instanceID, documentID, pageNumber, settings) {
          validateClientID(clientID);
          const endPoint = `${baseUrl}/clients/${clientID}/instances/${instanceID}/documents/${documentID}/pages/${pageNumber}`;
          return login().then(function(authorizationToken) {
            return _ajax(endPoint, __spreadValues({
              headers: getHeaders(authorizationToken),
              method: HTTP_GET,
              dataType: "json"
            }, settings));
          });
        },
        get: function(url) {
          return _ajax(url, {
            method: HTTP_GET
          });
        },
        formatDocumentUrl: function(clientID, instanceID, documentID, queryString) {
          var url = `${baseUrl}/clients/${clientID}/instances/${instanceID}/documents/${documentID}`;
          if (queryString) {
            url += "?" + queryString;
          }
          return url;
        },
        getDocumentFormats: function(settings) {
          const endPoint = `${baseUrl}/formats`;
          return login().then(function(authorizationToken) {
            return _ajax(endPoint, __spreadValues({
              headers: getHeaders(authorizationToken),
              method: HTTP_GET,
              dataType: "json"
            }, settings));
          });
        },
        getServiceVersion: function(settings) {
          const endPoint = `${baseUrl}/version`;
          return login().then(function(authorizationToken) {
            return _ajax(endPoint, __spreadValues({
              headers: getHeaders(authorizationToken),
              method: HTTP_GET,
              dataType: "json"
            }, settings));
          });
        },
        // eslint-disable-next-line max-params
        getResource: function(clientID, instanceID, documentID, resourceID, settings) {
          validateClientID(clientID);
          const endPoint = `${baseUrl}/clients/${clientID}/instances/${instanceID}/documents/${documentID}/resources/${resourceID}`;
          return login().then(function(authorizationToken) {
            return _ajax(endPoint, __spreadValues({
              headers: getHeaders(authorizationToken),
              method: HTTP_GET,
              dataType: (settings == null ? void 0 : settings.dataType) || "json"
            }, settings));
          });
        },
        // eslint-disable-next-line max-params
        getSearchResults: function(clientID, instanceID, documentID, searchToken, matchCase, matchWholeWord, useRegex, settings) {
          validateClientID(clientID);
          const endPoint = `${baseUrl}/clients/${clientID}/instances/${instanceID}/documents/${documentID}/search`;
          return login().then(function(authorizationToken) {
            return _ajax(endPoint, __spreadValues({
              headers: getHeaders(authorizationToken),
              method: HTTP_POST,
              contentType: JSON_CONTENT_TYPE,
              dataType: "json",
              data: JSON.stringify({
                searchToken,
                matchCase,
                matchWholeWord,
                useRegularExpressions: useRegex
              })
            }, settings));
          });
        },
        setAccessToken: function(accessToken) {
          loginPromise = Promise.resolve(accessToken);
        },
        login,
        keepClientAlive: function(clientID, settings) {
          const endPoint = `${baseUrl}/clients/keepAlive/${clientID}`;
          return login().then(function(authorizationToken) {
            return _ajax(endPoint, __spreadValues({
              headers: getHeaders(authorizationToken),
              method: HTTP_POST
            }, settings));
          });
        },
        getClientsSessionTimeoutSeconds: function(settings) {
          const endPoint = `${baseUrl}/clients/sessionTimeout`;
          return login().then(function(authorizationToken) {
            return _ajax(endPoint, __spreadValues({
              headers: getHeaders(authorizationToken),
              method: HTTP_GET
            }, settings));
          }).then(function(sessionTimeoutData) {
            return sessionTimeoutData.clientSessionTimeout;
          });
        }
      };
    }

    function UIController(options) {
      var stateFlags = {
        ExportInProgress: 1 << 0,
        PrintInProgress: 1 << 1,
        RenderInProgress: 1 << 2
      };
      function getState(flags) {
        return (state & flags) != 0;
      }
      function setState(flags, value) {
        if (value) {
          state |= flags;
        } else {
          state &= ~flags;
        }
      }
      var controller = options.controller;
      var historyManager = options.history;
      var state = 0;
      var refreshUI;
      var commands = options.commands;
      if (!controller) {
        throw "No controller (telerikReporting.ReportViewerController) has been specified.";
      }
      function getDocumentMapState() {
        var args = {};
        controller.getDocumentMapState(args);
        return args;
      }
      function getParametersAreaState() {
        var args = {};
        controller.getParametersAreaState(args);
        return args;
      }
      function getSearchDialogState() {
        var args = {};
        controller.getSearchDialogState(args);
        return args;
      }
      function getSendEmailDialogState() {
        var args = {};
        controller.getSendEmailDialogState(args);
        return args;
      }
      function updateUI() {
        if (!refreshUI) {
          refreshUI = true;
          window.setTimeout(function() {
            try {
              updateUICore();
            } finally {
              refreshUI = false;
            }
          }, 10);
        }
      }
      function updateUICore() {
        var rs = controller.getReportSource();
        var pageCount = controller.getPageCount();
        var currentPageNumber = controller.getCurrentPageNumber();
        var hasReport = rs && rs.report;
        var hasPages = hasReport && pageCount > 0;
        var nextPage = hasPages && currentPageNumber < pageCount;
        var prevPage = hasPages && currentPageNumber > 1;
        var hasPage = hasPages && currentPageNumber;
        var documentMapState = getDocumentMapState();
        var parametersAreaState = getParametersAreaState();
        var searchDialogState = getSearchDialogState();
        var sendEmailDialogState = getSendEmailDialogState();
        var renderInProgress = getState(stateFlags.RenderInProgress);
        var printInProgress = getState(stateFlags.PrintInProgress);
        var exportInProgress = getState(stateFlags.ExportInProgress);
        commands.goToFirstPage.enabled(prevPage);
        commands.goToPrevPage.enabled(prevPage);
        commands.stopRendering.enabled(hasReport && renderInProgress);
        commands.goToLastPage.enabled(nextPage);
        commands.goToNextPage.enabled(nextPage);
        commands.goToPage.enabled(hasPages);
        commands.print.enabled(hasPages && !renderInProgress && !printInProgress);
        commands.export.enabled(hasPages && !renderInProgress && !exportInProgress);
        commands.refresh.enabled(hasReport);
        commands.historyBack.enabled(historyManager && historyManager.canMoveBack());
        commands.historyForward.enabled(historyManager && historyManager.canMoveForward());
        commands.toggleDocumentMap.enabled(hasReport && documentMapState.enabled).checked(documentMapState.enabled && documentMapState.visible);
        commands.toggleParametersArea.enabled(hasReport && parametersAreaState.enabled).checked(parametersAreaState.enabled && parametersAreaState.visible);
        commands.togglePrintPreview.enabled(hasPages).checked(controller.getViewMode() == ViewModes.PRINT_PREVIEW);
        commands.pageMode.enabled(hasPages).checked(controller.getPageMode() == PageModes.CONTINUOUS_SCROLL);
        commands.zoom.enabled(hasPage);
        commands.zoomIn.enabled(hasPage);
        commands.zoomOut.enabled(hasPage);
        commands.toggleZoomMode.enabled(hasPage).checked(controller.getScaleMode() == ScaleModes.FIT_PAGE || controller.getScaleMode() == ScaleModes.FIT_PAGE_WIDTH);
        commands.toggleSearchDialog.enabled(hasPages).checked(searchDialogState.visible);
        commands.toggleSendEmailDialog.enabled(hasPages).checked(sendEmailDialogState.visible);
        controller.updateUI(null);
        controller.pageNumberChange(currentPageNumber);
        controller.pageCountChange(pageCount);
      }
      controller.setParametersAreaVisible(updateUI);
      controller.setDocumentMapVisible(updateUI);
      controller.updateUIInternal(updateUI);
      controller.setSearchDialogVisible(updateUI);
      controller.setSendEmailDialogVisible(updateUI);
      controller.scaleModeChanged(updateUI);
      controller.currentPageChanged(updateUI);
      controller.beforeLoadReport(function() {
        setState(stateFlags.RenderInProgress, true);
        updateUI();
      });
      controller.reportLoadProgress(updateUI);
      controller.reportLoadComplete(function() {
        setState(stateFlags.RenderInProgress, false);
        updateUI();
      });
      controller.reportSourceChanged(updateUI);
      controller.viewModeChanged(updateUI);
      controller.pageModeChanged(function() {
        updateUI();
      });
      controller.setUIState(function(event, args) {
        setState(stateFlags[args.operationName], args.inProgress);
        updateUI();
      });
      controller.error(function() {
        setState(stateFlags.ExportInProgress, false);
        setState(stateFlags.PrintInProgress, false);
        setState(stateFlags.RenderInProgress, false);
        updateUI();
      });
      controller.renderingStopped(function() {
        setState(stateFlags.RenderInProgress, false);
        updateUI();
      });
      updateUI();
    }

    var defaultOptions$3 = {};
    function DocumentMapArea(placeholder, options, otherOptions) {
      options = $.extend({}, defaultOptions$3, options, otherOptions);
      var controller = options.controller;
      if (!controller) {
        throw "No controller (telerikReporting.reportViewerController) has been specified.";
      }
      var $placeholder = $(placeholder);
      var $documentMap;
      var documentMapVisible = options.documentMapVisible !== false;
      var enableAccessibility = options.enableAccessibility;
      var currentReport = null;
      var documentMapNecessary = false;
      init();
      function init() {
        $documentMap = $('<div id="' + options.viewerSelector + '-documentMap"></div>');
        $documentMap.appendTo(placeholder);
        attach();
        replaceStringResources($placeholder);
      }
      function onTreeViewSelectionChanged(e) {
        var documentMapNode = this.dataItem(e.node);
        var page = documentMapNode.page;
        var id = documentMapNode.id;
        controller.navigateToPage(page, {
          type: "bookmark",
          id
        });
      }
      function onTreeViewNodeExpand(e) {
        if (enableAccessibility) {
          window.setTimeout(function() {
            setNodeAccessibilityAttributes(e.node);
          }, 100);
        }
      }
      function setNodeAccessibilityAttributes(node) {
        var $items = $(node).find("li");
        Array.from($items).forEach((item) => {
          var $li = $(item);
          $li.attr("aria-label", $li[0].innerText);
        });
      }
      function resetReportAndClearItems() {
        documentMapNecessary = false;
        showDocumentMap(false);
        var r = controller.getReportSource().report;
        var clearMapItems = currentReport !== r || !isVisible();
        currentReport = r;
        if (clearMapItems) {
          clearDocumentMap();
        }
      }
      function clearDocumentMap() {
        displayDocumentMap([]);
      }
      function displayDocumentMap(documentMap) {
        var hasDocumentMap = documentMap && !$.isEmptyObject(documentMap);
        var $treeView = $documentMap.data("kendoTreeView");
        if (!$treeView) {
          try {
            $documentMap.kendoTreeView({
              dataTextField: "text",
              select: onTreeViewSelectionChanged
            });
            $treeView = $documentMap.data("kendoTreeView");
          } catch (e) {
            console.error("Instantiation of Kendo TreeView as Document Map threw an exception", e);
            throw e;
          }
        }
        $treeView.setDataSource(documentMap);
        if (enableAccessibility) {
          setAccessibilityAttributes($treeView);
        }
        showDocumentMap(hasDocumentMap);
      }
      function setAccessibilityAttributes(treeView) {
        treeView.bind("expand", onTreeViewNodeExpand);
        treeView.element.attr("aria-label", stringResources.ariaLabelDocumentMap);
        var listItems = treeView.element.find("ul");
        Array.from(listItems).forEach((list) => {
          setNodeAccessibilityAttributes(list);
        });
        if (documentMapNecessary) {
          setSplitbarAccessibilityAttributes();
        }
      }
      function setSplitbarAccessibilityAttributes() {
        var splitbar = $placeholder.next();
        if (options.documentMapAreaPosition === DocumentMapAreaPositions.RIGHT) {
          splitbar = $placeholder.prev();
        }
        splitbar.attr("aria-label", stringResources.ariaLabelDocumentMapSplitter);
      }
      function isVisible() {
        var args = {};
        controller.getDocumentMapState(args);
        return args.visible;
      }
      function beginLoad() {
        $placeholder.addClass("trv-loading");
      }
      function endLoad() {
        $placeholder.removeClass("trv-loading");
      }
      function showDocumentMap(show) {
        var splitter = GlobalSettings.viewerInstances.find((element) => element.id === options.viewerSelector + "-document-map-splitter").instance;
        var sibling = $placeholder.next();
        if (options.documentMapAreaPosition === DocumentMapAreaPositions.RIGHT) {
          sibling = $placeholder.prev();
        }
        if (splitter) {
          (documentMapNecessary ? $.fn.removeClass : $.fn.addClass).call(sibling, "trv-hidden");
          splitter.toggle(".trv-document-map", show);
        }
      }
      function attach() {
        controller.beforeLoadParameters(function() {
          resetReportAndClearItems();
        }).beginLoadReport(function() {
          beginLoad();
        }).reportLoadComplete(function(event, args) {
          if (args.documentMapAvailable) {
            documentMapNecessary = true;
            displayDocumentMap(args.documentMapNodes);
            controller.setDocumentMapVisible({ enabled: true, visible: documentMapVisible });
          } else {
            documentMapNecessary = false;
            showDocumentMap(documentMapNecessary);
          }
          endLoad();
        }).error(function(event, error) {
          endLoad();
          clearDocumentMap();
        }).renderingStopped(function() {
          documentMapNecessary = false;
          showDocumentMap(false);
        });
        controller.setDocumentMapVisible(function(event, args) {
          documentMapVisible = args.visible;
          showDocumentMap(documentMapVisible && documentMapNecessary);
        }).getDocumentMapState(function(event, args) {
          args.enabled = documentMapNecessary;
          args.visible = documentMapVisible;
        });
      }
      function replaceStringResources($documentMap2) {
        var $documentMapOverlay = $documentMap2.find(".trv-document-map-overlay");
        if (!$documentMapOverlay) {
          return;
        }
        $documentMapOverlay.attr("aria-label", stringResources[$documentMapOverlay.attr("aria-label")]);
      }
    }

    var defaultOptions$2 = {};
    var Events = {
      PARAMETERS_READY: "pa.parametersReady",
      ERROR: "pa.Error"
    };
    function ParametersArea(placeholder, options, otherOptions) {
      options = $.extend({}, defaultOptions$2, options, otherOptions);
      var parametersArea = {};
      var $parametersArea = $(parametersArea);
      var editors = {};
      var controller = options.controller;
      if (!controller) {
        throw "No controller (telerikReporting.reportViewerController) has been specified.";
      }
      var parameterEditors = [].concat(options.parameterEditors, ParameterEditors);
      var recentParameterValues;
      var parameters;
      var initialParameterValues;
      var processedParameterValues = void 0;
      var $placeholder = $(placeholder);
      var $content = $placeholder.find(".trv-parameters-area-content");
      var $errorMessage = $placeholder.find(".trv-error-message");
      var $previewButton = $placeholder.find(".trv-parameters-area-preview-button");
      $previewButton.text(stringResources[$previewButton.text()]);
      $previewButton.attr("aria-label", stringResources[$previewButton.attr("aria-label")]);
      $previewButton.on("click", function(e) {
        e.preventDefault();
        if (allParametersValid()) {
          applyParameters();
        }
      });
      var parameterContainerTemplate = options.templates["trv-parameter"];
      var parametersAreaVisible = options.parametersAreaVisible !== false;
      var enableAccessibility = options.enableAccessibility;
      init();
      function init() {
        replaceStringResources($placeholder);
      }
      function replaceStringResources($paramsArea) {
        var $previewButton2 = $paramsArea.find(".trv-parameters-area-preview-button");
        if (!$previewButton2) {
          return;
        }
        $previewButton2.attr("aria-label", stringResources[$previewButton2.attr("aria-label")]);
        $previewButton2.text(stringResources[$previewButton2.text()]);
      }
      function toggleElementDisplay(el, shouldHide) {
        el.style.display = shouldHide ? "none" : "";
      }
      function createParameterContainer() {
        return $(parameterContainerTemplate);
      }
      function createParameterUI(parameter) {
        var $container = createParameterContainer();
        var $editorPlaceholder = $container.find(".trv-parameter-value");
        var $title = $container.find(".trv-parameter-title");
        var $error = $container.find(".trv-parameter-error");
        var $errorMessage2 = $container.find(".trv-parameter-error-message");
        var $useDefaultValueCheckbox = $container.find(".trv-parameter-use-default input");
        var editorsTypes = options.parameters && options.parameters.editors ? options.parameters.editors : null;
        var editorFactory = selectParameterEditorFactory(parameter, editorsTypes);
        var parameterText = escapeHtml(parameter.text);
        var isHiddenParameter = !parameter.isVisible;
        if (isHiddenParameter) {
          parameterText += " [<b>hidden</b>]";
        }
        $title.html(parameterText).attr("title", parameterText);
        $errorMessage2.text(parameter.Error);
        (parameter.Error ? $.fn.show : $.fn.hide).call($error);
        var editor = editorFactory.createEditor(
          $editorPlaceholder,
          {
            templates: options.templates,
            parameterChanged: function(parameter2, newValue) {
              var invalidParameterLI = document.querySelector(".trv-pages-area .trv-error-message ul li." + parameter2.name);
              try {
                newValue = ParameterValidators.validate(parameter2, newValue);
                $error.hide();
                if (invalidParameterLI) {
                  toggleElementDisplay(invalidParameterLI, true);
                }
                onParameterChanged(parameter2, newValue);
              } catch (error) {
                if (invalidParameterLI) {
                  toggleElementDisplay(invalidParameterLI, false);
                }
                parameter2.Error = error;
                parameter2.value = [];
                $errorMessage2.text(error);
                $error.show();
                enablePreviewButton(false);
              } finally {
                setAccessibilityErrorState(parameter2);
              }
            },
            enableAccessibility
          }
        );
        editors[parameter.id] = editor;
        editor.beginEdit(parameter);
        if (enableAccessibility && !isHiddenParameter) {
          editor.addAccessibility(parameter);
        }
        if ($useDefaultValueCheckbox.length > 0) {
          $useDefaultValueCheckbox.on(
            "click",
            function() {
              var useDefaultValue = $(this).is(":checked");
              if (useDefaultValue) {
                delete recentParameterValues[parameter.id];
                delete initialParameterValues[parameter.id];
                invalidateChildParameters(parameter);
                updateParameters(onLoadParametersSuccess);
              } else {
                recentParameterValues[parameter.id] = parameter.value;
                initialParameterValues[parameter.id] = parameter.value;
              }
              editor.enable(!useDefaultValue);
              raiseParametersReady();
            }
          );
          var hasInitialValues = initialParameterValues !== null;
          if (hasInitialValues) {
            if (!(parameter.id in initialParameterValues)) {
              $useDefaultValueCheckbox.prop("checked", true);
              editor.enable(false);
            }
          } else if (isHiddenParameter) {
            $useDefaultValueCheckbox.prop("checked", true);
            editor.enable(false);
          }
        }
        return $container;
      }
      function setAccessibilityErrorState(parameter) {
        var editor = editors[parameter.id];
        if (!editor || !enableAccessibility) {
          return;
        }
        editor.setAccessibilityErrorState(parameter);
      }
      function enablePreviewButton(enabled) {
        if (enabled) {
          $previewButton.prop("disabled", false);
          $previewButton.removeClass("k-disabled");
        } else {
          $previewButton.prop("disabled", true);
          $previewButton.addClass("k-disabled");
        }
      }
      function selectParameterEditorFactory(parameter, editorsType) {
        var factory = Array.from(parameterEditors).find((editor) => {
          return editor.match(parameter, editorsType);
        });
        return factory;
      }
      function showError(error) {
        $errorMessage.text(error);
        (error ? $.fn.addClass : $.fn.removeClass).call($placeholder, "trv-error");
      }
      function showPreviewButton() {
        (allParametersAutoRefresh() && controller.isReportAutoRun() ? $.fn.removeClass : $.fn.addClass).call($placeholder, "preview");
      }
      function allParametersAutoRefresh() {
        var allAuto = Array.from(parameters).every((parameter) => {
          return !parameter.isVisible || parameter.autoRefresh;
        });
        return allAuto;
      }
      function allParametersValid() {
        var allValid = Array.from(parameters).every((parameter) => {
          return !parameter.Error;
        });
        return allValid;
      }
      function clearEditors() {
        Object.entries(editors).forEach(([key, editor]) => {
          if (typeof editor.destroy === "function") {
            editor.destroy();
          }
        });
        editors = {};
      }
      function fill(newParameters) {
        recentParameterValues = {};
        processedParameterValues = {};
        parameters = newParameters || [];
        clearEditors();
        var $parameterContainer;
        var $tempContainer = $("<div></div>");
        Array.from(parameters).forEach((parameter) => {
          try {
            parameter.value = ParameterValidators.validate(parameter, parameter.value);
          } catch (e) {
            parameter.Error = parameter.Error || e;
          }
          var hasError = Boolean(parameter.Error);
          var hasValue = !hasError;
          if (hasValue) {
            recentParameterValues[parameter.id] = parameter.value;
          } else {
            parameter.Error = stringResources.invalidParameter;
          }
          if (parameter.availableValues) {
            processedParameterValues[parameter.id] = { valueMember: parameter.value, displayMember: parameter.label, availableValues: parameter.availableValues, multivalue: parameter.multivalue };
          } else {
            processedParameterValues[parameter.id] = parameter.value;
          }
          if (parameter.isVisible || options.showHiddenParameters) {
            $parameterContainer = createParameterUI(parameter);
            if ($parameterContainer) {
              $tempContainer.append($parameterContainer);
            }
          }
        });
        if (initialParameterValues !== void 0) {
          if (null === initialParameterValues) {
            initialParameterValues = {};
            Array.from(parameters).forEach((parameter) => {
              if (parameter.isVisible) {
                initialParameterValues[parameter.id] = parameter.value;
              } else {
                delete recentParameterValues[parameter.id];
              }
            });
          } else {
            Array.from(parameters).forEach((parameter) => {
              if (!(parameter.id in initialParameterValues)) {
                delete recentParameterValues[parameter.id];
              }
            });
          }
        }
        $content.empty();
        if (parameters.length > 0) {
          $content.append($tempContainer.children());
          if (enableAccessibility) {
            $content.attr("aria-label", "Parameters area. Contains " + parameters.length + " parameters.");
          }
        }
        showPreviewButton();
        var allValid = allParametersValid();
        enablePreviewButton(allValid);
        applyProcessedParameters();
      }
      function applyParameters() {
        controller.setParameters($.extend({}, recentParameterValues));
        controller.previewReport(false);
      }
      function applyProcessedParameters() {
        controller.setProcessedParameter($.extend({}, processedParameterValues));
      }
      function allParametersValidForAutoRefresh() {
        var triggerAutoUpdate = true;
        for (var i = parameters.length - 1; triggerAutoUpdate && i >= 0; i--) {
          var p = parameters[i];
          triggerAutoUpdate = p.id in recentParameterValues && (Boolean(p.autoRefresh) || !p.isVisible);
        }
        return triggerAutoUpdate;
      }
      function raiseParametersReady() {
        parametersArea.parametersReady(recentParameterValues);
      }
      function tryRefreshReport() {
        raiseParametersReady();
        if (allParametersValidForAutoRefresh()) {
          applyParameters();
        }
      }
      function invalidateChildParameters(parameter) {
        if (parameter.childParameters) {
          Array.from(parameter.childParameters).forEach((parameterId) => {
            var childParameter = getParameterById(parameterId);
            if (childParameter) {
              invalidateChildParameters(childParameter);
            }
            delete recentParameterValues[parameterId];
            resetPendingParameterChange(parameterId);
          });
        }
      }
      function resetPendingParameterChange(parameterId) {
        if (editors) {
          var editor = editors[parameterId];
          if (editor && typeof editor.clearPendingChange === "function") {
            editor.clearPendingChange();
          }
        }
      }
      function onParameterChanged(parameter, newValue) {
        delete parameter["Error"];
        parameter.value = newValue;
        recentParameterValues[parameter.id] = newValue;
        if (initialParameterValues !== void 0) {
          if (parameter.id in initialParameterValues) {
            recentParameterValues[parameter.id] = newValue;
          }
        } else {
          recentParameterValues[parameter.id] = newValue;
        }
        invalidateChildParameters(parameter);
        if (parameter.childParameters) {
          updateParameters(tryRefreshReport);
        } else {
          var allValid = allParametersValid();
          enablePreviewButton(allValid);
          if (allValid) {
            var errorMsg = document.querySelector(".trv-pages-area .trv-error-message");
            if ($previewButton.is(":visible") && errorMsg) {
              errorMsg.textContent = stringResources.tryReportPreview;
            }
            tryRefreshReport();
          }
        }
      }
      function getParameterById(parameterId) {
        if (parameters) {
          for (var i = 0; i < parameters.length; i++) {
            var p = parameters[i];
            if (p.id === parameterId) {
              return p;
            }
          }
        }
        return null;
      }
      function hasVisibleParameters(params) {
        if (!params || null === params) {
          return false;
        }
        var hasVisible = Array.from(params).some((parameter) => {
          return parameter.isVisible;
        });
        return hasVisible;
      }
      var loadingCount = 0;
      function beginLoad() {
        loadingCount++;
        $placeholder.addClass("trv-loading");
      }
      function endLoad() {
        if (loadingCount > 0) {
          if (0 === --loadingCount) {
            $placeholder.removeClass("trv-loading");
          }
        }
      }
      var parametersAreaNecessary = false;
      function onLoadParametersComplete(params, successAction) {
        showParametersAreaIfNecessary(params);
        fill(params);
        showError("");
        if (parametersAreaNecessary && parametersAreaVisible) {
          showParametersArea(true);
          if (enableAccessibility) {
            setSplitbarAccessibilityAttributes();
          }
        }
        controller.updateUIInternal();
        if (typeof successAction === "function") {
          successAction();
        }
        endLoad();
      }
      function showParametersAreaIfNecessary(params) {
        parametersAreaNecessary = hasVisibleParameters(params) || !controller.isReportAutoRun();
        showParametersArea(parametersAreaNecessary);
      }
      function updateParameters(successAction) {
        acceptParameters(controller.loadParameters(recentParameterValues), successAction);
      }
      function acceptParameters(controllerLoadParametersPromise, successAction) {
        beginLoad();
        controllerLoadParametersPromise.then(function(parameters2) {
          onLoadParametersComplete(parameters2, successAction);
        }).catch(function(error) {
          endLoad();
          clear();
          if (!$placeholder.hasClass("trv-hidden")) {
            showError(error);
          }
          parametersArea.error(error);
        });
      }
      function getEventHandlerFromArguments(args) {
        var arg0;
        if (args && args.length) {
          arg0 = args[0];
        }
        if (typeof arg0 === "function") {
          return arg0;
        }
        return null;
      }
      function eventFactory(event, args) {
        var h = getEventHandlerFromArguments(args);
        if (h) {
          $parametersArea.on(event, h);
        } else {
          $parametersArea.trigger(event, args);
        }
        return controller;
      }
      function onLoadParametersSuccess() {
        if (initialParameterValues === null) {
          initialParameterValues = $.extend({}, recentParameterValues);
        }
        raiseParametersReady();
      }
      function showParametersArea(show) {
        var splitter = GlobalSettings.viewerInstances.find((element) => element.id === options.viewerSelector + "-parameters-splitter").instance;
        var sibling = $placeholder.prev();
        if (options.parametersAreaPosition === ParametersAreaPositions.TOP || options.parametersAreaPosition === ParametersAreaPositions.LEFT) {
          sibling = $placeholder.next();
        }
        if (splitter) {
          (parametersAreaNecessary ? $.fn.removeClass : $.fn.addClass).call(sibling, "trv-hidden");
          splitter.toggle(".trv-parameters-area", show);
        }
      }
      function setSplitbarAccessibilityAttributes() {
        var splitbar = $placeholder.prev();
        var tabIndex = $placeholder.find(".trv-parameters-area-content").attr("tabIndex");
        if (options.parametersAreaPosition === ParametersAreaPositions.TOP || options.parametersAreaPosition === ParametersAreaPositions.LEFT) {
          splitbar = $placeholder.next();
        }
        splitbar.attr("aria-label", stringResources.ariaLabelParametersAreaSplitter);
        splitbar.attr("tabIndex", tabIndex);
      }
      function onReloadParameters(event, controllerLoadParametersPromise) {
        showError();
        $content.empty();
        acceptParameters(controllerLoadParametersPromise, onLoadParametersSuccess);
      }
      controller.reloadParameters(onReloadParameters).beforeLoadReport(function() {
        loadingCount = 0;
        beginLoad();
      }).reportLoadComplete(function() {
        showParametersAreaIfNecessary(parameters);
        showPreviewButton();
      }).error(endLoad).pageReady(endLoad);
      controller.getParametersAreaState(function(event, args) {
        var parametersAreaNecessary2 = false;
        if (parameters) {
          parametersAreaNecessary2 = hasVisibleParameters(parameters);
        }
        args.enabled = parametersAreaNecessary2;
        args.visible = parametersAreaVisible;
      }).setParametersAreaVisible(function(event, args) {
        parametersAreaVisible = args.visible;
        showParametersArea(args.visible && hasVisibleParameters(parameters));
      });
      function clear() {
        fill([]);
      }
      $.extend(
        parametersArea,
        {
          allParametersValid: function() {
            return allParametersValid();
          },
          clear: function() {
            clear();
          },
          error: function() {
            return eventFactory(Events.ERROR, arguments);
          },
          parametersReady: function() {
            return eventFactory(Events.PARAMETERS_READY, arguments);
          },
          fill: function(parameters2) {
            return fill(parameters2);
          },
          setParameters: function(parameterValues) {
            initialParameterValues = null === parameterValues ? null : $.extend({}, parameterValues);
          }
        }
      );
      return parametersArea;
    }

    var lastSelectedMenuItem;
    var lastSelectedSubmenuItem;
    function MainMenu(dom, rootOptions, otherOptions) {
      var options = $.extend({}, rootOptions, otherOptions);
      var menu = $(dom).data("kendoMenu");
      var childrenL1 = dom.childNodes;
      var controller = options.controller;
      var enableAccessibility = options.enableAccessibility;
      if (!controller) {
        throw "No controller (telerikReporting.ReportViewerController) has been specified.";
      }
      if (!menu) {
        init();
      }
      controller.reportLoadComplete(function(e, args) {
        if (!enableAccessibility) {
          if (menu && menu._oldHoverItem) {
            menu._oldHoverItem.toggleClass("k-focus");
          }
        }
        if (!args.renderingExtensions) {
          controller.getDocumentFormats().then(fillFormats);
        } else {
          fillFormats(args.renderingExtensions);
        }
      });
      function init() {
        try {
          menu = $(dom).kendoMenu().data("kendoMenu");
        } catch (e) {
          console.error("Instantiation of Kendo Menu as Main Menu threw an exception", e);
          throw e;
        }
        menu.bind("activate", onSubmenuActivate);
        menu.bind("deactivate", onSubmenuDeactivate);
        menu.element.off("keydown", onMenuKeyDown);
        menu.element.on("keydown", onMenuKeyDown);
        if (options.enableAccessibility) {
          setTabIndexes();
        }
        replaceStringResources();
      }
      function setTabIndexes() {
        var $menus = $('[data-role="telerik_ReportViewer_MainMenu"]');
        Array.from($menus).forEach((menu2) => {
          var $menuArea = $(menu2);
          var listItems = $menuArea.find("li");
          var menuTabIndex = 0;
          var tabIndexAttr = $menuArea.attr("tabIndex");
          if (tabIndexAttr) {
            menuTabIndex = tryParseInt(tabIndexAttr);
            if (!menuTabIndex || isNaN(menuTabIndex)) {
              menuTabIndex = 0;
            }
          }
          setMenuItemsTabIndexes(listItems, menuTabIndex);
          var pager = listItems.find('input[data-role="telerik_ReportViewer_PageNumberInput"]');
          if (pager.length > 0) {
            pager.attr("tabindex", menuTabIndex);
          }
        });
      }
      function setMenuItemsTabIndexes(listItems, menuTabIndex) {
        Array.from(listItems).forEach((item) => {
          var $item = $(item);
          $item.attr("tabindex", menuTabIndex);
          $item.on("focus", (event) => {
            $item.addClass("k-focus");
          });
          $item.on("blur", (event) => {
            $item.removeClass("k-focus");
          });
          var anchor = $item.children("a");
          if (anchor.length > 0) {
            var $anchor = $(anchor);
            $anchor.attr("tabindex", -1);
            $item.attr("title", $anchor.attr("title"));
          }
          $item.off("keydown");
          $item.on("keydown", function(event) {
            if (event.which == kendo.keys.ENTER) {
              clickOnMenuItem($item);
              lastSelectedMenuItem = $item;
            }
          });
        });
      }
      function fillFormats(formats) {
        Array.from($(dom).find("ul[data-command-list=export-format-list]")).forEach((list) => {
          var $list = $(list);
          var $parent = $list.parents("li");
          var tabIndex = enableAccessibility ? $parent.attr("tabindex") : -1;
          if (!tabIndex) {
            tabIndex = 1;
          }
          $list.empty();
          Array.from(formats).forEach((format) => {
            var ariaLabel = enableAccessibility ? stringFormat('aria-label="{localizedName}" ', format) : " ";
            var li = "<li " + ariaLabel + stringFormat('tabindex="' + tabIndex + '"><a tabindex="-1" href="#" data-command="telerik_ReportViewer_export" data-command-parameter="{name}"><span>{localizedName}</span></a></li>', format);
            menu.append(li, $parent);
          });
          if (enableAccessibility) {
            setInternalListAccessibilityKeyEvents($parent.find("li"));
          }
        });
      }
      function setInternalListAccessibilityKeyEvents(listItems) {
        Array.from(listItems).forEach((item) => {
          var $item = $(item);
          $item.off("keydown");
          $item.on("keydown", (event) => {
            switch (event.which) {
              case kendo.keys.ENTER:
                clickOnMenuItem($item);
                break;
              case kendo.keys.UP:
                var $prev = $item.prev();
                if ($prev.length > 0) {
                  $prev.trigger("focus");
                } else {
                  $item.parents("li").trigger("focus");
                }
                break;
              case kendo.keys.DOWN:
                var $next = $item.next();
                if ($next.length > 0) {
                  $next.trigger("focus");
                } else {
                  $item.parent().children("li").first().trigger("focus");
                }
                break;
            }
          });
        });
      }
      function clickOnMenuItem(item) {
        if (isItemExportContainer(item)) {
          clickOnExportDropdown();
        } else if (item && item.length > 0) {
          var anchor = item.children("a");
          if (anchor.length > 0) {
            anchor.click();
          }
        }
      }
      function clickOnExportDropdown() {
        var dropDownItemID = "#trv-main-menu-export-command";
        var mainMenu = $("[data-role='telerik_ReportViewer_MainMenu']").data("kendoMenu");
        mainMenu.open(dropDownItemID);
      }
      function onSubmenuActivate(e) {
        var $item = $(e.item);
        focusOnFirstSubmenuItem($item);
      }
      function onSubmenuDeactivate(e) {
        lastSelectedSubmenuItem = void 0;
      }
      function focusOnFirstSubmenuItem(parentItem) {
        if (lastSelectedMenuItem && lastSelectedMenuItem.is(parentItem)) {
          window.setTimeout(function() {
            var li = parentItem.find("li");
            if (li.length > 0) {
              li[0].focus();
            }
          }, 100);
        }
      }
      function onMenuKeyDown(e) {
        switch (e.which) {
          case kendo.keys.ENTER:
            if (!enableAccessibility) {
              var $item = getFocusedItem();
              if ($item.length > 0) {
                if (isItemExportContainer($item) && lastSelectedSubmenuItem) {
                  $item = lastSelectedSubmenuItem;
                }
                clickOnMenuItem($item);
              }
            }
            break;
          case kendo.keys.RIGHT:
            enableAccessibility ? focusNextItemAccessibilitySelection() : focusNextItemNativeMenuSelection();
            break;
          case kendo.keys.LEFT:
            enableAccessibility ? focusPreviousItemAccessibilitySelection() : focusPreviousItemNativeMenuSelection();
            break;
          case kendo.keys.DOWN:
          case kendo.keys.UP:
            if (!enableAccessibility) {
              lastSelectedSubmenuItem = getKendoFocusedNestedItem();
            }
        }
      }
      function getFocusedItem() {
        var $item;
        var focusedItem = document.activeElement;
        if (focusedItem && focusedItem.localName == "li") {
          var items = $(childrenL1).filter("li.k-item");
          for (var i = 0; i < items.length; i++) {
            var listItem = items[i];
            if (focusedItem === listItem) {
              $item = $(listItem);
              break;
            }
          }
        } else if (focusedItem && focusedItem.localName == "input") {
          $item = $(focusedItem).closest("li.k-item");
        } else {
          $item = menu.element.children("li.k-item.k-focus");
          if ($item.length === 0) {
            $item = menu.element.children("li.k-item").first();
          }
        }
        return $item;
      }
      function focusNextItemAccessibilitySelection() {
        var $item = getFocusedItem();
        if (!$item || !$item.length > 0) {
          return;
        }
        var $next = $item.next();
        if ($next.css("display") == "none") {
          $next = $next.next();
        }
        if (!$next.length > 0) {
          $next = $(childrenL1).filter("li.k-item").first();
        }
        $next.focus();
      }
      var lastKendoFocusedItem;
      function focusNextItemNativeMenuSelection() {
        var allItems = menu.element.children("li.k-item");
        var $focused = allItems.filter(".k-focus");
        if (kendo.version >= "2017.3.913") {
          lastKendoFocusedItem = $focused;
          return;
        }
        if ($focused.hasClass("k-disabled")) {
          if (!lastKendoFocusedItem || $focused.is(lastKendoFocusedItem)) {
            var $next = $focused.next();
            if (!$next.length > 0) {
              $next = allItems.first();
            }
            $focused.toggleClass("k-focus");
            $next.toggleClass("k-focus");
            lastKendoFocusedItem = $next;
            menu._oldHoverItem = $next;
          } else {
            lastKendoFocusedItem = $focused;
          }
        } else {
          menu._oldHoverItem = $focused;
          lastKendoFocusedItem = $focused;
        }
      }
      function focusPreviousItemAccessibilitySelection() {
        var $item = getFocusedItem();
        if (!$item || !$item.length > 0) {
          return;
        }
        var $prev = $item.prev();
        if ($prev.css("display") == "none") {
          $prev = $prev.prev();
        }
        if (!$prev.length > 0) {
          $prev = $(childrenL1).filter("li.k-item").last();
        }
        $prev.focus();
      }
      function focusPreviousItemNativeMenuSelection() {
        var $focused = menu.element.children("li.k-item.k-focus");
        lastKendoFocusedItem = $focused;
      }
      function getKendoFocusedNestedItem() {
        var $focused = menu.element.find('li.k-item.k-focus [data-command="telerik_ReportViewer_export"]');
        if ($focused.length === 1) {
          return $focused.parent("li");
        }
        return void 0;
      }
      function isItemExportContainer(item) {
        if (item.length === 0) {
          return;
        }
        var id = item.attr("id");
        return id == "trv-main-menu-export-command" || id == "trv-side-menu-export-command";
      }
      function replaceStringResources() {
        var menuAreas = findMenuArea();
        if (!menuAreas) {
          return;
        }
        Array.from(menuAreas).forEach((menu2) => {
          var $menu = $(menu2);
          var menuItems = $menu.children("li.k-item");
          $menu.attr("aria-label", stringResources[$menu.attr("aria-label")]);
          Array.from(menuItems).forEach((menuItem) => {
            var $menuItem = $(menuItem);
            $menuItem.attr("aria-label", stringResources[$menuItem.attr("aria-label")]);
            if (!$menuItem.hasClass("trv-report-pager")) {
              var $a = $menuItem.find("a");
              if ($a) {
                $a.attr("title", stringResources[$a.attr("title")]);
              }
            } else {
              $menuItem.attr("title", stringResources[$menuItem.attr("title")]);
            }
          });
        });
      }
      function findMenuArea() {
        return $("ul[data-role=telerik_ReportViewer_MainMenu]");
      }
    }

    var defaultOptions$1 = {};
    function replaceStringResources$1($search) {
      if (!$search) {
        return;
      }
      var $searchCaption = $search.find(".trv-search-dialog-caption-label");
      var $searchOptions = $search.find(".trv-search-dialog-search-options");
      var $searchStopButton = $search.find("a[data-command='telerik_ReportViewer_searchDialog_StopSearch']");
      var $searchMatchCaseButton = $search.find("a[data-command='telerik_ReportViewer_searchDialog_MatchCase']");
      var $searchMatchWholeWordButton = $search.find("a[data-command='telerik_ReportViewer_searchDialog_MatchWholeWord']");
      var $searchUseRegexButton = $search.find("a[data-command='telerik_ReportViewer_searchDialog_UseRegex']");
      var $searchNavigateUpButton = $search.find("a[data-command='telerik_ReportViewer_searchDialog_NavigateUp']");
      var $searchNavigateDownButton = $search.find("a[data-command='telerik_ReportViewer_searchDialog_NavigateDown']");
      replaceAttribute$1($search, "aria-label");
      replaceAttribute$1($searchOptions, "aria-label");
      replaceText$1($searchCaption);
      replaceTitleAndAriaLabel($searchStopButton);
      replaceTitleAndAriaLabel($searchMatchCaseButton);
      replaceTitleAndAriaLabel($searchMatchWholeWordButton);
      replaceTitleAndAriaLabel($searchUseRegexButton);
      replaceTitleAndAriaLabel($searchNavigateUpButton);
      replaceTitleAndAriaLabel($searchNavigateDownButton);
    }
    function replaceTitleAndAriaLabel($a) {
      replaceAttribute$1($a, "title");
      replaceAttribute$1($a, "aria-label");
    }
    function replaceText$1($el) {
      if ($el) {
        $el.text(stringResources[$el.text()]);
      }
    }
    function replaceAttribute$1($el, attribute) {
      if ($el) {
        $el.attr(attribute, stringResources[$el.attr(attribute)]);
      }
    }
    class Search {
      constructor(element, options, viewerOptions) {
        this.options = $.extend({}, defaultOptions$1, options);
        this.viewerOptions = viewerOptions;
        this.element = element;
        this.controller = this.options.controller;
        this.initialized = false;
        this.dialogVisible = false;
        this.$element;
        this.$inputBox;
        this.$searchOptionsPlaceholder;
        this.searchOptionsMenu;
        this.$stopSearchPlaceholder;
        this.stopSearchMenu;
        this.$navigationPlaceholder;
        this.navigationMenu;
        this.$resultsLabel;
        this.$resultsPlaceholder;
        this.kendoComboBox;
        this.kendoSearchDialog;
        this.stopSearchCommand;
        this.optionsCommandSet;
        this.navigationCommandSet;
        this.searchResults;
        this.mruList = [];
        this.inputComboRebinding;
        this.searchMetadataRequested;
        this.searchMetadataLoaded;
        this.pendingHighlightItem;
        this.windowLocation;
        this.reportViewerWrapper = $("[data-selector='" + this.viewerOptions.viewerSelector + "']").find(".trv-report-viewer");
        this.lastSearch = "";
        this.highlightManager = {
          // the results that are found, but not selected (highlighted)
          shadedClassName: "trv-search-dialog-shaded-result",
          // the result that is currently selected (highlighted)
          highlightedClassName: "trv-search-dialog-highlighted-result",
          current: null,
          elements: []
        };
        if (!this.controller) {
          throw "No controller (telerikReporting.ReportViewerController) has been specified.";
        }
        this.controller.pageReady(this.onPageReady.bind(this)).scrollPageReady(this.onPageReady.bind(this)).beginLoadReport(this.closeAndClear.bind(this)).viewModeChanged(this.closeAndClear.bind(this));
        this.controller.setSendEmailDialogVisible((event, args) => {
          if (args.visible && this.dialogVisible) {
            this.toggle(!this.dialogVisible);
          }
        }).getSearchDialogState((event, args) => {
          args.visible = this.dialogVisible;
        }).setSearchDialogVisible((event, args) => {
          this.toggleSearchDialog(args.visible);
        });
        $(window).on("resize", () => {
          if (this.kendoSearchDialog && this.kendoSearchDialog.options.visible) {
            this.storeDialogPosition();
            this.adjustDialogPosition();
          }
        });
      }
      closeAndClear() {
        if (this.searchMetadataRequested) {
          return;
        }
        this.toggle(false);
        this.searchMetadataLoaded = false;
      }
      toggleSearchDialog(show) {
        this.dialogVisible = show;
        if (show) {
          var searchMetadataOnDemand = this.viewerOptions.searchMetadataOnDemand;
          if (searchMetadataOnDemand && !this.searchMetadataLoaded) {
            this.searchMetadataRequested = true;
            this.controller.reportLoadComplete((event, args) => {
              if (this.searchMetadataRequested) {
                this.toggle(true);
                this.searchMetadataRequested = false;
              }
            });
            this.controller.refreshReport(true);
            return;
          }
        }
        this.toggle(show);
      }
      toggle(show) {
        this.dialogVisible = show;
        if (show) {
          this.searchMetadataLoaded = true;
          this.ensureInitialized();
          this.kendoSearchDialog.open();
          this.kendoComboBox.value("");
          this.updateResultsUI(null);
          this.toggleErrorLabel(false, null);
        } else {
          this.clearColoredItems();
          if (this.kendoSearchDialog && this.kendoSearchDialog.options.visible) {
            this.kendoSearchDialog.close();
          }
        }
      }
      ensureInitialized() {
        if (!this.initialized) {
          this.$element = $(this.element);
          this.$inputBox = this.$element.find(".trv-search-dialog-input-box");
          this.$resultsLabel = this.$element.find(".trv-search-dialog-results-label");
          this.$resultsPlaceholder = this.$element.find(".trv-search-dialog-results-area");
          this.initResultsArea();
          replaceStringResources$1(this.$element);
          try {
            this.$searchOptionsPlaceholder = this.$element.find(".trv-search-dialog-search-options").kendoMenu();
            this.$stopSearchPlaceholder = this.$element.find(".trv-search-dialog-stopsearch-placeholder").kendoMenu();
            this.$navigationPlaceholder = this.$element.find(".trv-search-dialog-navigational-buttons").kendoMenu();
          } catch (error) {
            console.error("Instantiation of Kendo Menu for Search Dialog threw an exception", error);
            throw error;
          }
          this.searchOptionsMenu = this.$searchOptionsPlaceholder.data("kendoMenu");
          this.stopSearchMenu = this.$stopSearchPlaceholder.data("kendoMenu");
          this.navigationMenu = this.$navigationPlaceholder.data("kendoMenu");
          this.searchOptionsMenu.element.on("keydown", this.onKeyDown);
          this.stopSearchMenu.element.on("keydown", this.onKeyDown);
          this.navigationMenu.element.on("keydown", this.onKeyDown);
          this.overrideDefaultMenuStyling(".trv-search-dialog-search-options");
          try {
            this.kendoComboBox = this.$inputBox.kendoComboBox({
              dataTextField: "value",
              dataValueField: "value",
              dataSource: this.mruList,
              contentElement: "",
              change: this.kendoComboBoxSelect.bind(this),
              ignoreCase: false,
              // the actual search-when-typing performs on this event.
              filtering: this.onInputFiltering.bind(this),
              filter: "startswith",
              delay: 1e3,
              open: (event) => {
                if (this.inputComboRebinding) {
                  event.preventDefault();
                }
              },
              select: this.processComboBoxEvent.bind(this)
            }).data("kendoComboBox");
          } catch (error) {
            console.error("Instantiation of Kendo ComboBox as search input threw an exception", error);
            throw error;
          }
          try {
            this.kendoSearchDialog = this.reportViewerWrapper.find(".trv-search-window").kendoWindow({
              title: stringResources.searchDialogTitle,
              height: 390,
              width: 310,
              minWidth: 310,
              minHeight: 390,
              maxHeight: 700,
              scrollable: false,
              close: () => {
                this.storeDialogPosition();
                this.lastSearch = "";
              },
              open: () => {
                this.adjustDialogPosition();
              },
              deactivate: () => {
                this.controller.setSearchDialogVisible({
                  visible: false
                });
              },
              activate: () => {
                this.kendoComboBox.input.focus();
              }
            }).data("kendoWindow");
          } catch (error) {
            console.error("Instantiation of Kendo Window for Search dialog threw an exception", error);
            throw error;
          }
          this.kendoSearchDialog.wrapper.addClass("trv-search");
          this.initCommands();
          this.initialized = true;
        }
      }
      overrideDefaultMenuStyling(kendoMenuSelector) {
        var menuItems = $(kendoMenuSelector).find(".k-menu-item");
        for (var i = 0; i < menuItems.length; i++) {
          $(menuItems[i]).removeClass("k-item");
        }
      }
      onKeyDown(event) {
        var item = $(event.target).find(".k-focus");
        if (event.keyCode === 13 && item && item.length > 0) {
          var anchor = item.children("a");
          if (anchor.length > 0) {
            anchor.click();
          }
        }
      }
      storeDialogPosition() {
        var kendoWindow = this.kendoSearchDialog.element.parent(".k-window");
        this.windowLocation = kendoWindow.offset();
      }
      adjustDialogPosition() {
        var windowWidth = $(window).innerWidth();
        var windowHeight = $(window).innerHeight();
        var kendoWindow = this.kendoSearchDialog.wrapper;
        var width = kendoWindow.outerWidth(true);
        var height = kendoWindow.outerHeight(true);
        var padding = 10;
        if (!this.windowLocation) {
          var reportViewerCoords = this.reportViewerWrapper[0].getBoundingClientRect();
          kendoWindow.css({
            top: reportViewerCoords.top + padding,
            left: reportViewerCoords.right - width - padding
          });
          this.kendoSearchDialog.setOptions({
            position: {
              top: reportViewerCoords.top + padding,
              left: reportViewerCoords.right - width - padding
            }
          });
        } else {
          var left = this.windowLocation.left;
          var top = this.windowLocation.top;
          var right = left + width;
          var bottom = top + height;
          if (right > windowWidth - padding) {
            left = Math.max(padding, windowWidth - width - padding);
            kendoWindow.css({ left });
            this.kendoSearchDialog.setOptions({
              position: {
                left
              }
            });
          }
          if (bottom > windowHeight - padding) {
            top = Math.max(padding, windowHeight - height - padding);
            kendoWindow.css({ top });
            this.kendoSearchDialog.setOptions({
              position: {
                top
              }
            });
          }
        }
      }
      processComboBoxEvent(event) {
        if (!(window.event || window.event.type)) {
          return;
        }
        var evt = window.event;
        if (evt.type === "keydown") {
          event.preventDefault();
          if (evt.keyCode === 40) {
            this.moveListSelection(1);
          }
          if (evt.keyCode === 38) {
            this.moveListSelection(-1);
          }
        }
      }
      initCommands() {
        this.optionsCommandSet = {
          "searchDialog_MatchCase": new Command(),
          "searchDialog_MatchWholeWord": new Command(),
          "searchDialog_UseRegex": new Command()
        };
        Object.entries(this.optionsCommandSet).forEach(([commandName, command]) => {
          command.exec = () => {
            this.toggleCommand(command);
          };
        });
        Binder.attachCommands(this.$searchOptionsPlaceholder, this.optionsCommandSet, this.viewerOptions);
        this.stopSearchCommand = new Command(() => {
          this.stopSearch();
        });
        Binder.attachCommands(this.$stopSearchPlaceholder, { "searchDialog_StopSearch": this.stopSearchCommand }, this.viewerOptions);
        this.navigationCommandSet = {
          "searchDialog_NavigateUp": new Command(() => {
            this.moveListSelection(-1);
          }),
          "searchDialog_NavigateDown": new Command(() => {
            this.moveListSelection(1);
          })
        };
        Binder.attachCommands(this.$navigationPlaceholder, this.navigationCommandSet, this.viewerOptions);
      }
      initResultsArea() {
        try {
          this.$resultsPlaceholder.kendoListView({
            selectable: true,
            navigatable: true,
            dataSource: {},
            contentElement: "",
            template: "<div class='trv-search-dialog-results-row'><span>#: description #</span> <span class='trv-search-dialog-results-pageSpan'>" + stringResources.searchDialogPageText + " #:page#</span></div>",
            change: (event) => {
              var listView = event.sender;
              var index = listView.select().index();
              var view = listView.dataSource.view();
              var dataItem = view[index];
              this.onSelectedItem(dataItem);
              this.updateUI(index, view.length);
            }
          });
        } catch (error) {
          console.error("Instantiation of Kendo ListView as search result area threw an exception", error);
          throw error;
        }
      }
      stopSearch() {
        this.setStopButtonEnabledState(false);
      }
      toggleCommand(cmd) {
        cmd.checked(!cmd.checked());
        this.searchForCurrentToken();
      }
      setStopButtonEnabledState(enabledState) {
        this.stopSearchCommand.enabled(enabledState);
      }
      onPageReady(args, page) {
        if (this.dialogVisible) {
          this.colorPageElements(this.searchResults);
        }
      }
      onInputFiltering(event) {
        event.preventDefault();
        if (event.filter && event.filter.value !== this.lastSearch) {
          this.lastSearch = event.filter.value;
          this.searchForToken(this.lastSearch);
        }
      }
      kendoComboBoxSelect(event) {
        var newValue = event.sender.value();
        if (newValue && this.lastSearch !== newValue) {
          this.lastSearch = newValue;
          this.searchForToken(this.lastSearch);
        }
      }
      searchForCurrentToken() {
        if (this.kendoComboBox) {
          this.searchForToken(this.kendoComboBox.value());
        }
      }
      searchForToken(token) {
        this.onSearchStarted();
        this.addToMRU(token);
        this.controller.getSearchResults(
          {
            searchToken: token,
            matchCase: this.optionsCommandSet.searchDialog_MatchCase.checked(),
            matchWholeWord: this.optionsCommandSet.searchDialog_MatchWholeWord.checked(),
            useRegex: this.optionsCommandSet.searchDialog_UseRegex.checked()
          }
        ).then((results) => {
          this.updateResultsUI(results, null);
        }).catch((errorMessage) => {
          if (errorMessage) {
            this.updateResultsUI(null, errorMessage);
          }
        });
      }
      onSearchStarted() {
        this.$resultsLabel.text(stringResources.searchDialogSearchInProgress);
        this.clearColoredItems();
        this.searchResults = null;
        this.setStopButtonEnabledState(true);
        this.toggleErrorLabel(false, null);
      }
      updateResultsUI(results, error) {
        this.setStopButtonEnabledState(false);
        if (error) {
          this.toggleErrorLabel(true, error);
        }
        this.displayResultsList(results);
        this.searchResults = results;
        if (results && results.length > 0) {
          this.colorPageElements(results);
          this.selectFirstElement();
        } else {
          this.updateUI(-1, 0);
        }
      }
      addToMRU(token) {
        if (!token || token === "") {
          return;
        }
        var exists = this.mruList.filter((mru) => {
          return mru.value === token;
        });
        if (exists && exists.length > 0) {
          return;
        }
        this.mruList.unshift({ value: token });
        if (this.mruList.length > 10) {
          this.mruList.pop();
        }
        this.inputComboRebinding = true;
        this.kendoComboBox.dataSource.data(this.mruList);
        this.kendoComboBox.select((item) => {
          return item.value === token;
        });
        this.inputComboRebinding = false;
      }
      displayResultsList(results) {
        var $listView = this.$resultsPlaceholder.data("kendoListView");
        if (!results) {
          results = [];
        }
        $listView.dataSource.data(results);
      }
      colorPageElements(results) {
        if (!results || results.length === 0) {
          return;
        }
        var $parent = this.$element.parent();
        var $pageContainer = $parent.find(".trv-page-container");
        var elements = $pageContainer.find("[data-search-id]");
        Array.from(results).forEach((result) => {
          var $searchElement = elements.filter("[data-search-id=" + result.id + "]");
          if ($searchElement) {
            $searchElement.addClass(this.highlightManager.shadedClassName);
            this.highlightManager.elements.push($searchElement);
          }
        });
        this.highlightItem(this.pendingHighlightItem);
        this.pendingHighlightItem = null;
      }
      highlightItem(item) {
        if (item) {
          var currentItemId = item.id;
          var newHighlighted = $(this.highlightManager.elements.filter((i) => {
            return i.attr("data-search-id") === currentItemId;
          })).first();
          if (newHighlighted) {
            this.highlightManager.current = newHighlighted[0];
            if (this.highlightManager.current) {
              var current = $("[data-search-id='" + currentItemId + "']");
              current.removeClass(this.highlightManager.shadedClassName);
              current.addClass(this.highlightManager.highlightedClassName);
            }
          }
        }
      }
      selectFirstElement() {
        var $listView = this.$resultsPlaceholder.data("kendoListView");
        var first = $listView.element.children().first();
        $listView.select(first);
        $listView.trigger("change");
      }
      onSelectedItem(item) {
        if (!item) {
          return;
        }
        if (this.highlightManager.current) {
          this.highlightManager.current.removeClass(this.highlightManager.highlightedClassName);
          this.highlightManager.current.addClass(this.highlightManager.shadedClassName);
        }
        if (item.page === this.controller.getCurrentPageNumber()) {
          this.highlightItem(item);
        } else {
          if (this.controller.getPageMode() !== PageModes.CONTINUOUS_SCROLL) {
            this.clearColoredItems();
          } else {
            this.highlightItem(item);
          }
        }
        this.pendingHighlightItem = item;
        this.controller.navigateToPage(item.page, { type: "search", id: item.id });
      }
      updateUI(index, count) {
        var str = count === 0 ? stringResources.searchDialogNoResultsLabel : stringFormat(stringResources.searchDialogResultsFormatLabel, [index + 1, count]);
        this.$resultsLabel.text(str);
        var allowMoveUp = index > 0;
        var allowMoveDown = index < count - 1;
        this.navigationCommandSet.searchDialog_NavigateUp.enabled(allowMoveUp);
        this.navigationCommandSet.searchDialog_NavigateDown.enabled(allowMoveDown);
      }
      clearColoredItems() {
        if (this.highlightManager.elements && this.highlightManager.elements.length > 0) {
          Array.from(this.highlightManager.elements).forEach(($element) => {
            $element.removeClass(this.highlightManager.shadedClassName);
          });
        }
        if (this.highlightManager.current) {
          this.highlightManager.current.removeClass(this.highlightManager.highlightedClassName);
        }
        this.highlightManager.elements = [];
        this.highlightManager.current = null;
      }
      moveListSelection(offset) {
        var $listView = this.$resultsPlaceholder.data("kendoListView");
        var $selected = $listView.select();
        if (!$selected) {
          $selected = $listView.element.children().first();
          $listView.select($selected);
          $listView.trigger("change");
        } else {
          var index = $listView.select().trigger("change").index();
          var view = $listView.dataSource.view();
          var newIndex = Math.min(view.length - 1, Math.max(0, index + offset));
          if (newIndex !== index) {
            var dataItem = view[newIndex];
            var element = $listView.element.find('[data-uid="' + dataItem.uid + '"]');
            if (element) {
              $listView.select(element);
              $listView.trigger("change");
              this.scrollIfNeeded(element[0], $listView.element[0]);
            }
          }
        }
      }
      scrollIfNeeded(element, container) {
        if (element.offsetTop - element.clientHeight < container.scrollTop) {
          element.scrollIntoView();
        } else {
          var offsetBottom = element.offsetTop + element.offsetHeight;
          var scrollBottom = container.scrollTop + container.offsetHeight;
          if (offsetBottom > scrollBottom) {
            container.scrollTop = offsetBottom - container.offsetHeight;
          }
        }
      }
      toggleErrorLabel(show, message) {
        var $errorIcon = this.$searchOptionsPlaceholder.find("i[data-role='telerik_ReportViewer_SearchDialog_Error']");
        if (!$errorIcon || $errorIcon.length === 0) {
          console.log(message);
          return;
        }
        var menuItem = this.$searchOptionsPlaceholder.data("kendoMenu").element.find("li").last();
        if (show) {
          $errorIcon[0].title = message;
          menuItem.show();
        } else {
          menuItem.hide();
        }
      }
    }

    var defaultOptions = {};
    function replaceStringResources($sendEmailDialog) {
      if (!$sendEmailDialog) {
        return;
      }
      var labels = $sendEmailDialog.find(".trv-replace-string");
      var ariaLabel = $sendEmailDialog.find("[aria-label]");
      var titles = $sendEmailDialog.find("[title]");
      if (labels.length) {
        Array.from(labels).forEach((element) => {
          replaceText($(element));
        });
      }
      if (ariaLabel.length) {
        Array.from(ariaLabel).forEach((element) => {
          replaceAttribute($(element), "aria-label");
        });
      }
      if (titles.length) {
        Array.from(titles).forEach((element) => {
          replaceAttribute($(element), "title");
        });
      }
    }
    function replaceText($el) {
      if ($el) {
        $el.text(stringResources[$el.text()]);
      }
    }
    function replaceAttribute($el, attribute) {
      if ($el) {
        $el.attr(attribute, stringResources[$el.attr(attribute)]);
      }
    }
    function SendEmail(placeholder, options, viewerOptions) {
      options = $.extend({}, defaultOptions, options);
      var controller = options.controller;
      var initialized = false;
      var dialogVisible = false;
      var $placeholder;
      var kendoSendEmailDialog;
      var selector = viewerOptions.viewerSelector;
      var inputFrom;
      var inputTo;
      var inputCC;
      var inputSubject;
      var docFormat;
      var docFormatEl;
      var bodyEditorEl;
      var bodyEditor;
      var docFormatList;
      var optionsCommandSet;
      var windowLocation;
      var reportViewerWrapper = $("[data-selector='" + selector + "']").find(".trv-report-viewer");
      if (!controller) {
        throw "No controller (telerikReporting.ReportViewerController) has been specified.";
      }
      if (!viewerOptions.sendEmail || !viewerOptions.sendEmail.enabled) {
        var toolbarSendEmailItem = $("[data-selector='" + selector + "']").find("a[data-command='telerik_ReportViewer_toggleSendEmailDialog']").closest(".k-item ");
        toolbarSendEmailItem.hide();
        return;
      }
      controller.beginLoadReport(closeAndClear).viewModeChanged(closeAndClear);
      controller.getSendEmailDialogState(function(event, args) {
        args.visible = dialogVisible;
      }).setSendEmailDialogVisible(function(event, args) {
        toggle(args.visible);
      }).setSearchDialogVisible(function(event, args) {
        if (args.visible && dialogVisible) {
          toggle(!dialogVisible);
        }
      });
      controller.getDocumentFormats().then(function(formats) {
        docFormatList = formats;
      });
      function closeAndClear() {
        toggle(false);
      }
      function toggle(show) {
        dialogVisible = show;
        if (show) {
          ensureInitialized();
          setDefaultValues(viewerOptions.sendEmail);
          kendoSendEmailDialog.open();
        } else {
          if (kendoSendEmailDialog && kendoSendEmailDialog.options.visible) {
            kendoSendEmailDialog.close();
          }
        }
      }
      function getBody() {
        return bodyEditor ? bodyEditor.value() : "";
      }
      function ensureInitialized() {
        if (!initialized) {
          $placeholder = $(placeholder);
          inputFrom = $placeholder.find("[name='from']");
          inputTo = $placeholder.find("[name='to']");
          inputCC = $placeholder.find("[name='cc']");
          inputSubject = $placeholder.find("[name='subject']");
          docFormatEl = $placeholder.find("[name='format']");
          bodyEditorEl = $placeholder.find("textarea");
          setAttrs();
          initCommands();
          replaceStringResources($placeholder);
          try {
            kendoSendEmailDialog = reportViewerWrapper.find(".trv-send-email-window").kendoWindow({
              title: stringResources.sendEmailDialogTitle,
              minWidth: 350,
              minHeight: 350,
              maxHeight: 900,
              modal: true,
              close: function() {
                storeDialogPosition();
                clearValidation();
              },
              open: function() {
                adjustDialogSize();
                adjustDialogPosition();
              },
              deactivate: function() {
                controller.setSendEmailDialogVisible({
                  visible: false
                });
              },
              activate: function() {
                kendoSendEmailDialog.wrapper.find(".trv-send-email-fields input[type='email']:visible").first().focus();
                setTimeout(function() {
                  setValidation();
                }, 250);
              }
            }).data("kendoWindow");
          } catch (error) {
            console.error("Instantiation of Kendo Window for Send Email dialog threw an exception", error);
            throw error;
          }
          kendoSendEmailDialog.wrapper.addClass("trv-send-email");
          try {
            docFormat = docFormatEl.kendoComboBox({
              dataTextField: "localizedName",
              dataValueField: "name",
              dataSource: docFormatList || [],
              filter: "startswith",
              dataBound: function() {
                this.select(0);
                this.trigger("change");
              }
            }).data("kendoComboBox");
          } catch (error) {
            console.error("Instantiation of Kendo ComboBox as document format selector threw an exception", error);
            throw error;
          }
          $placeholder.on("keydown", '[name="format_input"]', function(event) {
            var tabkey = 9;
            if (event.keyCode === tabkey && bodyEditor) {
              setTimeout(function() {
                bodyEditor.focus();
              });
            }
          });
          try {
            bodyEditor = bodyEditorEl.kendoEditor({
              tools: [
                "bold",
                "italic",
                "underline",
                "strikethrough",
                "justifyLeft",
                "justifyCenter",
                "justifyRight",
                "justifyFull",
                "insertUnorderedList",
                "insertOrderedList",
                "indent",
                "outdent",
                "createLink",
                "unlink",
                "cleanFormatting",
                "formatting",
                "fontName",
                "fontSize",
                "foreColor",
                "backColor",
                "subscript",
                "superscript"
              ]
            }).data("kendoEditor");
          } catch (error) {
            console.error("Instantiation of Kendo Editor for Email body editor threw an exception", error);
            throw error;
          }
          setDefaultValues(viewerOptions.sendEmail);
          initialized = true;
        }
      }
      $(window).resize(function() {
        if (kendoSendEmailDialog && kendoSendEmailDialog.options.visible) {
          storeDialogPosition();
          adjustDialogSize();
          adjustDialogPosition();
        }
      });
      function setAttrs() {
        $placeholder.find(".trv-send-email-field input").each(function() {
          var el = $(this);
          var attrName = el.attr("name");
          el.attr("id", selector + "-" + attrName);
        });
        $placeholder.find(".trv-send-email-label label").each(function() {
          var el = $(this);
          var attrName = el.attr("for");
          el.attr("for", selector + "-" + attrName);
        });
      }
      function storeDialogPosition() {
        var kendoWindow = kendoSendEmailDialog.element.parent(".k-window");
        windowLocation = kendoWindow.offset();
      }
      function adjustDialogSize() {
        var kendoWindow = kendoSendEmailDialog.element.parent(".k-window");
        var windowWidth = $(window).width();
        var kendoWindowWidth = 350;
        if (windowWidth > 800) {
          kendoWindowWidth = 720;
        }
        kendoWindow.css({ width: kendoWindowWidth });
        kendoSendEmailDialog.refresh({
          width: kendoWindowWidth
        });
      }
      function adjustDialogPosition() {
        if (!windowLocation) {
          kendoSendEmailDialog.center();
        } else {
          var padding = 10;
          var windowWidth = $(window).innerWidth();
          var windowHeight = $(window).innerHeight();
          var kendoWindow = kendoSendEmailDialog.wrapper;
          var width = kendoWindow.outerWidth(true);
          var height = kendoWindow.outerHeight(true);
          var left = windowLocation.left;
          var top = windowLocation.top;
          var right = left + width;
          var bottom = top + height;
          if (right > windowWidth - padding) {
            left = Math.max(padding, windowWidth - width - padding);
            kendoWindow.css({ left });
            kendoSendEmailDialog.setOptions({
              position: {
                left
              }
            });
          }
          if (bottom > windowHeight - padding) {
            top = Math.max(padding, windowHeight - height - padding);
            kendoWindow.css({ top });
            kendoSendEmailDialog.setOptions({
              position: {
                top
              }
            });
          }
        }
      }
      function initCommands() {
        optionsCommandSet = {
          "sendEmail_Cancel": new Command(function() {
            closeWindow();
          }),
          "sendEmail_Send": new Command(function() {
            sendingEmail();
          })
        };
        Binder.attachCommands($placeholder.find(".trv-send-email-actions"), optionsCommandSet, viewerOptions);
      }
      function sendingEmail(cmd, args) {
        var sendEmailArgs = {
          from: inputFrom.val(),
          to: inputTo.val(),
          cc: inputCC.val(),
          subject: inputSubject.val(),
          format: docFormat.value(),
          body: getBody(),
          deviceInfo: {}
        };
        if (validateFields()) {
          controller.sendReport(sendEmailArgs);
          closeWindow();
        }
      }
      function setValidation() {
        inputFrom.off("blur").on("blur", function(event) {
          if (!isEmpty($(this))) {
            isValidEmail($(this), false);
          }
        });
        inputTo.off("blur").on("blur", function(event) {
          if (!isEmpty($(this))) {
            isValidEmail($(this), true);
          }
        });
        inputCC.off("blur").on("blur", function(event) {
          if ($(this).val().length) {
            isValidEmail($(this), true);
          } else {
            hideError($(this));
          }
        });
      }
      function validateFields() {
        var fromIsValid = isEmpty(inputFrom) || !isValidEmail(inputFrom, false);
        var toIsValid = isEmpty(inputTo) || !isValidEmail(inputTo, true);
        var ccIsValid = inputCC.val().length && !isValidEmail(inputCC, true);
        var hasFormat = docFormat.value().length;
        if (!hasFormat) {
          showError(docFormatEl, "data-required-msg");
        }
        if (fromIsValid || toIsValid || ccIsValid || !hasFormat) {
          return false;
        }
        return true;
      }
      function setDefaultValues(sendEmail) {
        inputFrom.val(sendEmail && sendEmail.from || "");
        inputTo.val(sendEmail && sendEmail.to || "");
        inputCC.val(sendEmail && sendEmail.cc || "");
        inputSubject.val(sendEmail && sendEmail.subject || "");
        if (sendEmail && sendEmail.format) {
          docFormat.value(sendEmail.format);
        } else {
          docFormat.select(0);
        }
        docFormat.trigger("change");
        bodyEditor.value(sendEmail && sendEmail.body || "");
      }
      function isEmpty($el) {
        if (!$el.val().length) {
          showError($el, "data-required-msg");
          return true;
        }
        hideError($el);
        return false;
      }
      function showError($el, tag) {
        var validationMsg = stringResources[$el.attr(tag)];
        $('[data-for="' + $el.attr("name") + '"]').addClass("-visible").text(validationMsg);
      }
      function hideError($el) {
        $('[data-for="' + $el.attr("name") + '"]').removeClass("-visible");
      }
      function isValidEmail($el, moreThenOneEmail) {
        var inputValue = $el.val();
        if (moreThenOneEmail) {
          var listEmailsAddress = inputValue.split(/[\s,;]+/);
          for (var i = 0; i < listEmailsAddress.length; i++) {
            if (!_validateEmail(listEmailsAddress[i].trim(), $el)) {
              return false;
            }
          }
          return true;
        }
        return _validateEmail(inputValue, $el);
      }
      function _validateEmail(email, $el) {
        var regexEmail = /\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*/;
        if (email.indexOf(",") > -1 || email.indexOf(";") > -1) {
          showError($el, "data-single-email-msg");
          return false;
        }
        if (!regexEmail.test(email)) {
          showError($el, "data-email-msg");
          return false;
        }
        return true;
      }
      function closeWindow() {
        kendoSendEmailDialog.close();
      }
      function clearValidation() {
        $(".k-invalid-msg").removeClass("-visible");
      }
    }

    function SideMenu(dom, rootOptions, otherOptions) {
      var options = $.extend({}, rootOptions, otherOptions);
      var menu = $(dom).data("kendoMenu");
      var enableAccessibility = options.enableAccessibility;
      var DEFAULT_TABINDEX = 3;
      var panelBar;
      var sideMenuVisible = false;
      var controller = options.controller;
      if (!controller) {
        throw "No controller (telerikReporting.ReportViewerController) has been specified.";
      }
      if (!menu) {
        init(dom);
      }
      function init(root) {
        var $root = $(root);
        try {
          panelBar = $root.children("ul").kendoPanelBar().data("kendoPanelBar");
        } catch (e) {
          console.error("Instantiation of Kendo PanelBar as side menu threw an exception", e);
          throw e;
        }
        panelBar.bind("expand", onSubmenuOpen);
        panelBar.element.off("keydown", onPanelKeyDown);
        panelBar.element.on("keydown", onPanelKeyDown);
        setTabIndexes($root);
        enableCloseOnClick($root);
        $root.click(function(e) {
          if (e.target == root) {
            controller.setSideMenuVisible({ visible: !sideMenuVisible });
          }
        });
        replaceStringResources();
      }
      controller.setSideMenuVisible(function(event, args) {
        setSideMenuVisibility();
        if (enableAccessibility) {
          panelBar.element.focus();
        }
        sideMenuVisible = args.visible;
        if (!sideMenuVisible) {
          panelBar.collapse($("#trv-side-menu-export-command"));
        }
      }).getSideMenuVisible(function(event, args) {
        args.visible = sideMenuVisible;
      });
      function setSideMenuVisibility() {
        var $root = panelBar.element.parent();
        var hidden = $root.position().left < 0 || !$root.is(":visible");
        if (hidden) {
          $root.show();
        } else {
          window.setTimeout(function() {
            $root.hide();
          }, 500);
        }
      }
      function onSubmenuOpen(e) {
        var $item = $(e.item);
        panelBar.unbind("expand", onSubmenuOpen);
        panelBar.append({ text: stringResources.loadingFormats, spriteCssClass: "k-icon k-loading" }, $item);
        options.controller.getDocumentFormats().then(fillFormats).then(function() {
          panelBar.expand($item);
        }).then(function() {
          panelBar.bind("expand", onSubmenuOpen);
        });
      }
      function fillFormats(formats) {
        Array.from($(dom).find("ul[data-command-list=export-format-list]")).forEach((list) => {
          var $list = $(list);
          var $parent = $list.parents("li");
          var tabIndex = $parent.attr("tabindex");
          if (!tabIndex) {
            tabIndex = DEFAULT_TABINDEX;
          }
          $list.empty();
          Array.from(formats).forEach((format) => {
            var ariaLabel = enableAccessibility ? stringFormat('aria-label="{localizedName}" ', format) : " ";
            var li = "<li " + ariaLabel + stringFormat('tabindex="' + tabIndex + '"><a tabindex="-1" href="#" data-command="telerik_ReportViewer_export" data-command-parameter="{name}"><span>{localizedName}</span></a></li>', format);
            panelBar.append(li, $parent);
          });
          setListItemsTabIndex($parent.find("li"), tabIndex);
          enableCloseOnClick($parent);
        });
      }
      function enableCloseOnClick(root) {
        Array.from(root.find("li")).forEach((listItem) => {
          var isLeaf = $(listItem).children("ul").length === 0;
          if (isLeaf) {
            $(listItem).children("a").on("click", (event) => {
              controller.setSideMenuVisible({ visible: !sideMenuVisible });
            });
          }
        });
      }
      function setTabIndexes(root) {
        if (!root) {
          return;
        }
        var $list = root.children("ul");
        var parentTabIndex = root.attr("tabindex");
        var listIndex = parentTabIndex ? parentTabIndex : DEFAULT_TABINDEX;
        setListItemsTabIndex($list, listIndex);
      }
      function setListItemsTabIndex(list, tabIndex) {
        list.attr("tabindex", tabIndex);
        var items = list.find("li");
        Array.from(items).forEach((item) => {
          var $item = $(item);
          $item.attr("tabindex", tabIndex);
          var anchor = $item.children("a");
          if (anchor.length > 0) {
            var $anchor = $(anchor);
            $anchor.attr("tabindex", -1);
          }
          $item.on("focus", (event) => {
            var anchor2 = $item.children("a");
            if (anchor2.length > 0) {
              anchor2.addClass("k-focus");
            }
          });
          $item.on("blur", (event) => {
            var anchor2 = $item.children("a");
            if (anchor2.length > 0) {
              anchor2.removeClass("k-focus");
            }
          });
          $item.off("keydown", onItemKeyDown);
          $item.on("keydown", onItemKeyDown);
        });
      }
      function onPanelKeyDown(e) {
        if (e.which == kendo.keys.ENTER) {
          var $item;
          var isSelectedFocusedItem = false;
          var focusedItem = document.activeElement;
          if (focusedItem && focusedItem.localName == "li") {
            var items = panelBar.element.find("li.k-item");
            for (var i = 0; i < items.length; i++) {
              var listItem = items[i];
              if (focusedItem === listItem) {
                $item = $(listItem);
                isSelectedFocusedItem = true;
                break;
              }
            }
          } else {
            $item = panelBar.select();
          }
          if (!$item || !$item.length > 0) {
            return;
          }
          handleItemSelect($item, isSelectedFocusedItem);
        }
      }
      function onItemKeyDown(e) {
        if (e.which == kendo.keys.ENTER) {
          handleItemSelect($(e.target), false);
        }
      }
      function handleItemSelect(item, handleExpandCollapse) {
        if (!item.length > 0) {
          return;
        }
        var isLeaf = item.children("ul").length === 0;
        if (!isLeaf) {
          if (handleExpandCollapse) {
            if (item.hasClass("k-active")) {
              panelBar.collapse(item);
            } else {
              panelBar.expand(item);
            }
          }
        } else {
          var $anchor = item.find("a");
          if ($anchor.length > 0) {
            $anchor[0].click();
          }
        }
      }
      function replaceStringResources() {
        var menuAreas = findMenuArea();
        if (!menuAreas) {
          return;
        }
        Array.from(menuAreas).forEach((menu2) => {
          var $menu = $(menu2);
          var menuItems = $menu.children("li.k-panelbar-header");
          $menu.attr("aria-label", stringResources[$menu.attr("aria-label")]);
          Array.from(menuItems).forEach((menuItem) => {
            var $menuItem = $(menuItem);
            var $a = $menuItem.find("a");
            $menuItem.attr("aria-label", stringResources[$menuItem.attr("aria-label")]);
            if ($a) {
              var $span = $a.find("span:not(.k-icon)");
              $a.attr("title", stringResources[$a.attr("title")]);
              if ($span) {
                $span.text(stringResources[$span.text()]);
              }
            }
          });
        });
      }
      function findMenuArea() {
        return $("div[data-role=telerik_ReportViewer_SideMenu] > ul");
      }
    }

    var __defProp$4 = Object.defineProperty;
    var __defNormalProp$4 = (obj, key, value) => key in obj ? __defProp$4(obj, key, { enumerable: true, configurable: true, writable: true, value }) : obj[key] = value;
    var __publicField$4 = (obj, key, value) => {
      __defNormalProp$4(obj, typeof key !== "symbol" ? key + "" : key, value);
      return value;
    };
    class MemStorage {
      // #endregion
      // #region constructor
      constructor() {
        // #region fields
        __publicField$4(this, "_data", {});
        this._data = {};
      }
      // #endregion
      // #region properties
      get length() {
        return Object.keys(this._data).length;
      }
      // #endregion
      // #region methods
      getItem(key) {
        return this._data[key] || null;
      }
      setItem(key, value) {
        this._data[key] = value;
      }
      removeItem(key) {
        delete this._data[key];
      }
      key(index) {
        return Object.keys(this._data)[index] || null;
      }
      clear() {
        this._data = {};
      }
      // #endregion
    }

    const TemplateCache = function() {
      var cache = {};
      return {
        load: function(url, serviceUrl, client) {
          var p = cache[url];
          if (!p) {
            cache[url] = p = client.get(url).then(function(html) {
              var templates = {};
              var styleSheets = [];
              var baseUri = rTrim(serviceUrl, "\\/") + "/";
              html = replaceAll(html, "{service}/", baseUri);
              html = replaceAll(html, "{service}", baseUri);
              var viewerTemplate = $("<div></div>").html(html);
              Array.from(viewerTemplate.find("template")).forEach((element) => {
                var $element = $(element);
                templates[$element.attr("id")] = trim($element.html(), "\n 	");
              });
              Array.from(viewerTemplate.find("link")).forEach((element) => {
                styleSheets.push(trim(element.outerHTML, "\n 	"));
              });
              styleSheets = filterUniqueLastOccurrence(styleSheets);
              return {
                templates,
                styleSheets
              };
            });
          }
          return p;
        }
      };
    }();

    var __defProp$3 = Object.defineProperty;
    var __defNormalProp$3 = (obj, key, value) => key in obj ? __defProp$3(obj, key, { enumerable: true, configurable: true, writable: true, value }) : obj[key] = value;
    var __publicField$3 = (obj, key, value) => {
      __defNormalProp$3(obj, typeof key !== "symbol" ? key + "" : key, value);
      return value;
    };
    class ReportViewerSettings {
      // #endregion
      // #region constructor
      constructor(id, storage, defaultSettings) {
        // #region fields
        __publicField$3(this, "_id");
        __publicField$3(this, "_storage");
        __publicField$3(this, "_defaults");
        this._id = id;
        this._storage = storage;
        this._defaults = defaultSettings || {};
      }
      // #endregion
      // #region methods
      formatKey(key) {
        return this._id + "_" + key;
      }
      getItem(key) {
        var value = this._storage.getItem(this.formatKey(key));
        if (typeof value === "string") {
          try {
            value = JSON.parse(value);
          } catch (e) {
            value = null;
          }
        }
        return value !== null && value !== void 0 ? value : this._defaults[key];
      }
      setItem(key, value) {
        var formattedKey = this.formatKey(key);
        this._storage.setItem(formattedKey, JSON.stringify(value));
      }
      // #endregion
      // #region accessors
      getViewMode() {
        return this.getItem("viewMode");
      }
      setViewMode(value) {
        this.setItem("viewMode", value);
      }
      getPageMode() {
        return this.getItem("pageMode");
      }
      setPageMode(value) {
        this.setItem("pageMode", value);
      }
      getPrintMode() {
        return this.getItem("printMode");
      }
      setPrintMode(value) {
        this.setItem("printMode", value);
      }
      getScale() {
        return this.getItem("scale");
      }
      setScale(value) {
        this.setItem("scale", value);
      }
      getScaleMode() {
        return this.getItem("scaleMode");
      }
      setScaleMode(value) {
        this.setItem("scaleMode", value);
      }
      getDocumentMapVisible() {
        return this.getItem("documentMapVisible");
      }
      setDocumentMapVisible(value) {
        this.setItem("documentMapVisible", value);
      }
      getParametersAreaVisible() {
        return this.getItem("parametersAreaVisible");
      }
      setParametersAreaVisible(value) {
        this.setItem("parametersAreaVisible", value);
      }
      getHistory() {
        return this.getItem("history");
      }
      setHistory(value) {
        this.setItem("history", value);
      }
      getClientId() {
        return this.getItem("clientId");
      }
      setClientId(value) {
        this.setItem("clientId", value);
      }
      getReportSource() {
        return this.getItem("reportSource");
      }
      setReportSource(value) {
        this.setItem("reportSource", value);
      }
      getPageNumber() {
        return this.getItem("pageNumber");
      }
      setPageNumber(value) {
        this.setItem("pageNumber", value);
      }
      getEnableAccessibility() {
        return this.getItem("enableAccessibility");
      }
      setEnableAccessibility(value) {
        this.setItem("enableAccessibility", value);
      }
      getAccessibilityKeyMap() {
        return this.getItem("accessibilityKeyMap");
      }
      setAccessibilityKeyMap(value) {
        this.setItem("accessibilityKeyMap", value);
      }
      getSearchMetadataOnDemand() {
        return this.getItem("searchMetadataOnDemand");
      }
      setSearchMetadataOnDemand(value) {
        this.setItem("searchMetadataOnDemand", value);
      }
      getKeepClientAlive() {
        return this.getItem("keepClientAlive");
      }
      setKeepClientAlive(value) {
        this.setItem("keepClientAlive", value);
      }
      // #endregion
    }

    var __defProp$2 = Object.defineProperty;
    var __defNormalProp$2 = (obj, key, value) => key in obj ? __defProp$2(obj, key, { enumerable: true, configurable: true, writable: true, value }) : obj[key] = value;
    var __publicField$2 = (obj, key, value) => {
      __defNormalProp$2(obj, typeof key !== "symbol" ? key + "" : key, value);
      return value;
    };
    class BaseComponent {
      // #endregion
      // #region constructor
      constructor(element, options) {
        // #region fields
        __publicField$2(this, "element");
        __publicField$2(this, "$element");
        __publicField$2(this, "options");
        this.element = element;
        this.$element = $(element);
        this.options = options;
      }
      // #endregion
    }

    var __defProp$1 = Object.defineProperty;
    var __defNormalProp$1 = (obj, key, value) => key in obj ? __defProp$1(obj, key, { enumerable: true, configurable: true, writable: true, value }) : obj[key] = value;
    var __publicField$1 = (obj, key, value) => {
      __defNormalProp$1(obj, typeof key !== "symbol" ? key + "" : key, value);
      return value;
    };
    class LinkButton extends BaseComponent {
      // #endregion
      // #region constructor
      constructor(element, options) {
        super(element, options);
        // #region fields
        __publicField$1(this, "cmd");
        var dataCommand = this.$element.attr("data-command");
        if (dataCommand) {
          this.cmd = this.options.commands[dataCommand];
        }
        if (this.cmd) {
          this.$element.on("click", (event) => {
            if (this.cmd.enabled()) {
              this.cmd.exec($(this).attr("data-command-parameter"));
            } else {
              event.preventDefault();
            }
          });
          $(this.cmd).on("enabledChanged", (event) => {
            (this.cmd.enabled() ? $.fn.removeClass : $.fn.addClass).call(this.$element, "disabled");
          }).on("checkedChanged", (event) => {
            (this.cmd.checked() ? $.fn.addClass : $.fn.removeClass).call(this.$element, "checked");
          });
        }
      }
      // #endregion
    }

    var __defProp = Object.defineProperty;
    var __defNormalProp = (obj, key, value) => key in obj ? __defProp(obj, key, { enumerable: true, configurable: true, writable: true, value }) : obj[key] = value;
    var __publicField = (obj, key, value) => {
      __defNormalProp(obj, typeof key !== "symbol" ? key + "" : key, value);
      return value;
    };
    class PageNumberInput extends BaseComponent {
      // #endregion
      // #region constructor
      constructor(element, options) {
        super(element, options);
        // #region fields
        __publicField(this, "cmd");
        __publicField(this, "_numeric");
        this.cmd = this.options.commands["goToPage"];
        this._numeric = new kendo.ui.NumericTextBox(this.element, {
          format: "0",
          decimals: 0,
          min: 0,
          spinners: false,
          change: this._onChange.bind(this),
          spin: this._onChange.bind(this)
        });
        this._numeric._text[0].dataset.role = "telerik_ReportViewer_PageNumberInput";
        this._numeric.element[0].dataset.role = "";
        this.options.controller.on("reportLoadComplete", (event, reportInfo) => {
          this._numeric.max(reportInfo.pageCount);
          this._numeric.min(Math.min(1, reportInfo.pageCount));
          this._numeric.value(Math.min(1, reportInfo.pageCount));
        }).on("loadedReportChange", (event) => {
          this._numeric.min(0);
          this._numeric.max(0);
          this._numeric.value(0);
        }).on("renderingStopped", (event) => {
          this._numeric.min(0);
          this._numeric.max(0);
          this._numeric.value(0);
        }).pageNumberChange((event, value) => {
          this._numeric.value(value);
        });
      }
      // #endregion
      // #region event handlers
      _onChange(event, data) {
        var val = this._numeric.value();
        var num = tryParseInt(val);
        if (!isNaN(num)) {
          this.cmd.exec(num);
        }
      }
      _onSpin(event, data) {
        return this._onChange(event, data);
      }
      // #endregion
    }

    class PageCountLabel extends BaseComponent {
      // #region constructor
      constructor(element, options) {
        super(element, options);
        this.options.controller.pageCountChange((event, value) => {
          this.$element.text(value);
        });
      }
      // #endregion
    }

    const Instances = GlobalSettings.viewerInstances;
    function getDefaultOptions(serviceUrl, version) {
      return {
        id: null,
        serviceUrl: null,
        templateUrl: rTrim(serviceUrl, "\\/") + "/resources/templates/telerikReportViewerTemplate-" + version + ".html/",
        reportSource: null,
        reportServer: null,
        authenticationToken: null,
        sendEmail: null,
        scale: 1,
        scaleMode: ScaleModes.FIT_PAGE,
        viewMode: ViewModes.INTERACTIVE,
        pageMode: PageModes.CONTINUOUS_SCROLL,
        parametersAreaPosition: ParametersAreaPositions.RIGHT,
        documentMapAreaPosition: DocumentMapAreaPositions.LEFT,
        parameters: {
          editors: {
            multiSelect: ParameterEditorTypes.LIST_VIEW,
            singleSelect: ParameterEditorTypes.LIST_VIEW
          }
        },
        persistSession: false,
        parameterEditors: [],
        disabledButtonClass: null,
        checkedButtonClass: null,
        parametersAreaVisible: true,
        documentMapVisible: true,
        enableAccessibility: false,
        searchMetadataOnDemand: false,
        initialPageAreaImageUrl: null,
        keepClientAlive: true
      };
    }
    function ReportViewer(dom, options) {
      var svcApiUrl = options.serviceUrl;
      var reportServerUrlSVCApiUrl = "";
      if (options.reportServer) {
        reportServerUrlSVCApiUrl = rTrim(options.reportServer.url, "\\/");
        svcApiUrl = reportServerUrlSVCApiUrl + "/api/reports";
      }
      var $placeholder = $(dom);
      var templates = {};
      var persistanceKey = options.id || `trv_${dom.id}`;
      var accessibility;
      var settings = {};
      var client = {};
      var controller = {};
      var perspectiveManager = {};
      var history = {};
      var commands = {};
      var viewer = {};
      var serviceClientOptions = {};
      var reportServerUrl = "";
      options.viewerSelector = "reportViewer-" + generateGuidString();
      $placeholder.attr("data-selector", options.viewerSelector);
      if (!validateOptions(options)) {
        return;
      }
      var version = "18.3.24.1112";
      options = $.extend({}, getDefaultOptions(svcApiUrl, version), options);
      settings = new ReportViewerSettings(
        persistanceKey,
        options.persistSession ? window.sessionStorage : new MemStorage(),
        {
          scale: options.scale,
          scaleMode: options.scaleMode,
          printMode: options.printMode ? options.printMode : options.directPrint,
          enableAccessibility: options.enableAccessibility,
          searchMetadataOnDemand: options.searchMetadataOnDemand,
          sendEmail: options.sendEmail,
          parametersAreaPosition: options.parametersAreaPosition,
          documentMapAreaPosition: options.documentMapAreaPosition,
          keepClientAlive: options.keepClientAlive
        }
      );
      if (options.reportServer) {
        reportServerUrl = rTrim(options.reportServer.url, "\\/");
        serviceClientOptions.serviceUrl = reportServerUrl + "/api/reports";
        serviceClientOptions.loginInfo = {
          url: reportServerUrl + "/Token",
          username: options.reportServer.username,
          password: options.reportServer.password
        };
      } else {
        serviceClientOptions.serviceUrl = options.serviceUrl;
      }
      client = new ServiceClient(serviceClientOptions);
      controller = options.controller;
      if (!controller) {
        controller = new ReportViewerController({
          serviceClient: client,
          settings
        });
      } else {
        controller.updateSettings(settings);
      }
      history = new HistoryManager({
        controller,
        settings
      });
      commands = new CommandSet({
        controller,
        history
      });
      new UIController({
        controller,
        history,
        commands
      });
      viewer = {
        stringResources,
        refreshReport: function(ignoreCache) {
          if (arguments.length === 0) {
            ignoreCache = true;
          }
          controller.refreshReport(ignoreCache, true);
          return viewer;
        },
        reportSource: function(rs) {
          if (rs || rs === null) {
            controller.setReportSource(rs);
            controller.refreshReport(false, true);
            return viewer;
          }
          return controller.getReportSource();
        },
        clearReportSource: function() {
          controller.clearReportSource();
          return viewer;
        },
        viewMode: function(vm) {
          if (vm) {
            controller.setViewMode(vm);
            return viewer;
          }
          return controller.getViewMode();
        },
        pageMode: function(psm) {
          if (psm) {
            controller.setPageMode(psm);
            return viewer;
          }
          return controller.getPageMode();
        },
        printMode: function(pm) {
          if (pm) {
            controller.setPrintMode(pm);
            return viewer;
          }
          return controller.getPrintMode();
        },
        scale: function({ scale, scaleMode } = {}) {
          if (scale || scaleMode) {
            controller.setScale(scale || controller.getScale());
            controller.setScaleMode(scaleMode || controller.getScaleMode());
            return viewer;
          }
          return {
            scale: controller.getScale(),
            scaleMode: controller.getScaleMode()
          };
        },
        currentPage: function() {
          return controller.getCurrentPageNumber();
        },
        pageCount: function() {
          return controller.getPageCount();
        },
        parametersAreaVisible: function(visible) {
          controller.setParametersAreaVisible({ visible });
        },
        getReportParameters: function() {
          return controller.getReportParameters();
        },
        isReportAutoRun: function() {
          return controller.isReportAutoRun();
        },
        authenticationToken: function(token) {
          if (token) {
            controller.setAuthenticationToken(token);
          }
          return viewer;
        },
        bind: function(eventName, eventHandler) {
          eventBinder(eventName, eventHandler, true);
        },
        unbind: function(eventName, eventHandler) {
          eventBinder(eventName, eventHandler, false);
        },
        accessibilityKeyMap: function(keyMap) {
          if (accessibility) {
            if (keyMap) {
              accessibility.setKeyMap(keyMap);
              return viewer;
            }
            return accessibility.getKeyMap();
          }
          return void 0;
        },
        commands,
        dispose: function() {
          controller.dispose();
          if (perspectiveManager) {
            perspectiveManager.dispose();
          }
        }
      };
      function validateOptions(options2) {
        if (!options2) {
          $placeholder.text("The report viewer configuration options are not initialized.");
          return false;
        }
        if (options2.reportServer) {
          if (!options2.reportServer.url) {
            $placeholder.text("The report server URL is not specified.");
            return false;
          }
        } else {
          if (!options2.serviceUrl) {
            $placeholder.text("The serviceUrl is not specified.");
            return false;
          }
        }
        return true;
      }
      function eventBinder(eventName, eventHandler, bind) {
        if (typeof eventHandler === "function") {
          if (bind) {
            $(viewer).on(eventName, { sender: viewer }, eventHandler);
          } else {
            $(viewer).off(eventName, eventHandler);
          }
        } else if (!eventHandler && !bind) {
          $(viewer).off(eventName);
        }
      }
      function attachEvents() {
        var eventMap = {
          EXPORT_BEGIN: controller.Events.EXPORT_STARTED,
          EXPORT_END: controller.Events.EXPORT_DOCUMENT_READY,
          PRINT_BEGIN: controller.Events.PRINT_STARTED,
          PRINT_END: controller.Events.PRINT_DOCUMENT_READY,
          RENDERING_BEGIN: controller.Events.BEFORE_LOAD_REPORT,
          RENDERING_END: controller.Events.REPORT_LOAD_COMPLETE,
          PAGE_READY: controller.Events.PAGE_READY,
          ERROR: controller.Events.ERROR,
          UPDATE_UI: controller.Events.UPDATE_UI,
          INTERACTIVE_ACTION_EXECUTING: controller.Events.INTERACTIVE_ACTION_EXECUTING,
          INTERACTIVE_ACTION_ENTER: controller.Events.INTERACTIVE_ACTION_ENTER,
          INTERACTIVE_ACTION_LEAVE: controller.Events.INTERACTIVE_ACTION_LEAVE,
          VIEWER_TOOLTIP_OPENING: controller.Events.TOOLTIP_OPENING,
          SEND_EMAIL_BEGIN: controller.Events.SEND_EMAIL_STARTED,
          SEND_EMAIL_END: controller.Events.SEND_EMAIL_READY,
          PARAMETERS_LOADED: controller.Events.PARAMETERS_LOADED
        };
        var $viewer = $(viewer);
        for (const [viewerEvent, controllerEvent] of Object.entries(eventMap)) {
          controller.on(controllerEvent, function(e, args) {
            $viewer.trigger(viewerEvent, args);
          });
        }
      }
      function attachEventHandlers() {
        eventBinder(Events$1.EXPORT_BEGIN, options.exportBegin, true);
        eventBinder(Events$1.EXPORT_END, options.exportEnd, true);
        eventBinder(Events$1.PRINT_BEGIN, options.printBegin, true);
        eventBinder(Events$1.PRINT_END, options.printEnd, true);
        eventBinder(Events$1.RENDERING_BEGIN, options.renderingBegin, true);
        eventBinder(Events$1.RENDERING_END, options.renderingEnd, true);
        eventBinder(Events$1.PAGE_READY, options.pageReady, true);
        eventBinder(Events$1.ERROR, options.error, true);
        eventBinder(Events$1.UPDATE_UI, options.updateUi, true);
        eventBinder(Events$1.INTERACTIVE_ACTION_EXECUTING, options.interactiveActionExecuting, true);
        eventBinder(Events$1.INTERACTIVE_ACTION_ENTER, options.interactiveActionEnter, true);
        eventBinder(Events$1.INTERACTIVE_ACTION_LEAVE, options.interactiveActionLeave, true);
        eventBinder(Events$1.VIEWER_TOOLTIP_OPENING, options.viewerToolTipOpening, true);
        eventBinder(Events$1.SEND_EMAIL_BEGIN, options.sendEmailBegin, true);
        eventBinder(Events$1.SEND_EMAIL_END, options.sendEmailEnd, true);
        eventBinder(Events$1.PARAMETERS_LOADED, options.parametersLoaded, true);
      }
      function init() {
        $placeholder.html(templates["trv-report-viewer"]);
        Binder.bind(
          $placeholder,
          {
            controller,
            commands,
            templates
          },
          options
        );
        perspectiveManager = new PerspectiveManager(dom, controller);
        perspectiveManager.attach();
        initSplitter();
        attachEvents();
        attachEventHandlers();
        initFromStorage();
        initAccessibility(options);
      }
      function initSplitter() {
        var parameterAreaPaneOptions = {
          max: "500px",
          min: "50px",
          size: "210px",
          collapsible: true
        };
        var parameterAreaTemplate = $placeholder.find(".trv-parameters-area");
        var parameterAreaPanes = [{}];
        var documentMapPaneOptions = {
          max: "500px",
          min: "50px",
          size: "210px",
          collapsible: true,
          collapsed: true
        };
        var documentMapTemplate = $placeholder.find(".trv-document-map");
        var documentMapPanes = [{}];
        var orientation = "horizontal";
        if (options.documentMapAreaPosition === DocumentMapAreaPositions.RIGHT) {
          documentMapTemplate.insertAfter($placeholder.find(".trv-pages-area"));
          documentMapPanes.push(documentMapPaneOptions);
        } else {
          documentMapPanes.unshift(documentMapPaneOptions);
        }
        if (options.parametersAreaPosition === ParametersAreaPositions.TOP || options.parametersAreaPosition === ParametersAreaPositions.BOTTOM) {
          orientation = "vertical";
          parameterAreaTemplate.addClass("-vertical");
          parameterAreaPaneOptions.size = "130px";
        }
        if (options.parametersAreaPosition === ParametersAreaPositions.LEFT || options.parametersAreaPosition === ParametersAreaPositions.TOP) {
          parameterAreaTemplate.insertBefore($placeholder.find(".trv-document-map-splitter"));
          parameterAreaPanes.unshift(parameterAreaPaneOptions);
        } else {
          parameterAreaPanes.push(parameterAreaPaneOptions);
        }
        try {
          var documentMapSplitter = $placeholder.find(".trv-document-map-splitter").kendoSplitter({
            panes: documentMapPanes,
            expand: function(e) {
              setSplitterPaneVisibility(e.pane, true);
            },
            collapse: function(e) {
              setSplitterPaneVisibility(e.pane, false);
            },
            resize: function(e) {
            }
          }).data("kendoSplitter");
        } catch (e) {
          console.error("Instantiation of Kendo Splitter as Document Map splitter threw an exception", e);
          throw e;
        }
        try {
          var parametersSplitter = $placeholder.find(".trv-parameters-splitter").kendoSplitter({
            panes: parameterAreaPanes,
            orientation,
            expand: function(e) {
              setSplitterPaneVisibility(e.pane, true);
            },
            collapse: function(e) {
              setSplitterPaneVisibility(e.pane, false);
            },
            resize: function(e) {
            }
          }).data("kendoSplitter");
        } catch (e) {
          console.error("Instantiation of Kendo Splitter as Parameters area splitter threw an exception", e);
          throw e;
        }
        var parametersSplitterInstance = {
          id: options.viewerSelector + "-parameters-splitter",
          instance: parametersSplitter
        };
        var documentMapSplitterInstance = {
          id: options.viewerSelector + "-document-map-splitter",
          instance: documentMapSplitter
        };
        Instances.push(parametersSplitterInstance);
        Instances.push(documentMapSplitterInstance);
      }
      function setSplitterPaneVisibility(pane, visible) {
        var paneID = $(pane).attr("data-id");
        switch (paneID) {
          case "trv-document-map":
            controller.setDocumentMapVisible({
              visible
            });
            break;
          case "trv-parameters-area":
            controller.setParametersAreaVisible({
              visible
            });
            break;
        }
      }
      function initFromStorage() {
        var vm = settings.getViewMode();
        var psm = settings.getPageMode();
        var pm = settings.getPrintMode();
        var s = settings.getScale();
        var sm = settings.getScaleMode();
        var dm = settings.getDocumentMapVisible();
        var pa = settings.getParametersAreaVisible();
        settings.getAccessibilityKeyMap();
        controller.setViewMode(vm ? vm : options.viewMode);
        controller.setPageMode(psm ? psm : options.pageMode);
        controller.setPrintMode(pm ? pm : options.printMode);
        controller.setScale(s ? s : options.scale);
        controller.setScaleMode(sm ? sm : options.scaleMode);
        controller.setDocumentMapVisible({
          visible: dm ? dm : options.documentMapVisible
        });
        controller.setParametersAreaVisible({
          visible: pa ? pa : options.parametersAreaVisible
        });
        controller.printModeChanged(function() {
          settings.setPrintMode(controller.getPrintMode());
        });
        controller.viewModeChanged(function() {
          settings.setViewMode(controller.getViewMode());
        });
        controller.pageModeChanged(function() {
          settings.setPageMode(controller.getPageMode());
        });
        controller.scaleChanged(function(event, scale) {
          settings.setScale(scale);
        });
        controller.scaleModeChanged(function(event, scaleMode) {
          settings.setScaleMode(scaleMode);
        });
        controller.setSideMenuVisible(function(event, args) {
          window.setTimeout(function() {
            (args.visible ? $.fn.addClass : $.fn.removeClass).call($placeholder, "trv-side-menu-visible");
          }, 1);
        });
        controller.setDocumentMapVisible(function() {
          var args = {};
          controller.getDocumentMapState(args);
          settings.setDocumentMapVisible(args.visible);
        });
        controller.setParametersAreaVisible(function() {
          var args = {};
          controller.getParametersAreaState(args);
          settings.setParametersAreaVisible(args.visible);
        });
      }
      function initAccessibility(options2) {
        if (options2.enableAccessibility) {
          accessibility = new Accessibility({
            controller,
            templates
          });
          var am = options2.accessibilityKeyMap;
          if (am) {
            accessibility.setKeyMap(am);
          }
          settings.contentTabIndex = getTemplateContentTabIndex();
        }
      }
      function getTemplateContentTabIndex() {
        var pageAreaSelector = "div.trv-pages-area";
        try {
          var $pagesArea = $placeholder.find(pageAreaSelector);
          if ($pagesArea.length === 0) {
            throw "Selector " + pageAreaSelector + " did not return a result.";
          }
          return parseInt($pagesArea.attr("tabindex"));
        } catch (e) {
          if (console)
            console.log(e);
          return 0;
        }
      }
      function start() {
        var pendingRefresh = false;
        init();
        controller.reportLoadComplete(function() {
          if (options.documentMapVisible === false) {
            controller.setDocumentMapVisible({ visible: false });
          }
        });
        var rs = settings.getReportSource();
        if (rs !== void 0) {
          controller.setReportSource(rs);
          var pageNumber = settings.getPageNumber();
          if (pageNumber !== void 0) {
            controller.navigateToPage(pageNumber);
          }
          pendingRefresh = true;
        } else {
          if (options.viewMode) {
            controller.setViewMode(options.viewMode);
          }
          if (options.pageMode) {
            controller.setPageMode(options.pageMode);
          }
          if (options.reportSource) {
            controller.setReportSource(options.reportSource);
            pendingRefresh = true;
          }
        }
        if (typeof options.ready === "function") {
          options.ready.call(viewer);
        }
        if (pendingRefresh) {
          controller.refreshReport(false, true);
        }
      }
      function loadStyleSheets(styleSheets) {
        if (!styleSheets)
          return Promise.resolve();
        var $head = $("head");
        var currentStyleLinks = $head.find("link").map(function(i, e) {
          return e.outerHTML;
        }).toArray();
        var promises = [];
        Array.from(styleSheets).forEach((element) => {
          if (currentStyleLinks.indexOf(element) === -1) {
            promises.push(
              new Promise(function(resolve, reject) {
                var $link = $(element);
                $link.on("load", resolve);
                $link.on("onerror", function() {
                  logError("error loading stylesheet " + element);
                  resolve();
                });
                $head.append($link);
              })
            );
          }
        });
        return Promise.all(promises).then(controller.cssLoaded);
      }
      function browserSupportsAllFeatures() {
        return window.Promise;
      }
      function ensureKendo(version2) {
        if (window.kendo) {
          return Promise.resolve();
        }
        var kendoUrl = rTrim(svcApiUrl, "\\/") + "/resources/js/telerikReportViewer.kendo-" + version2 + ".min.js/";
        return loadScript(kendoUrl).catch(function(errorData) {
          logError("Kendo could not be loaded automatically. Make sure 'options.serviceUrl' / 'options.reportServer.url' is correct and accessible. The error is: " + errorData.error);
        });
      }
      function main(version2) {
        ensureKendo(version2).then(function() {
        }).then(function() {
          viewer.authenticationToken(options.authenticationToken);
          controller.getServiceVersion().catch(function(ex) {
            var errorOutput = isApplicationExceptionInstance(ex) ? ex.exceptionMessage : stringFormat(stringResources.errorServiceUrl, [escapeHtml(svcApiUrl)]);
            $placeholder.text(errorOutput);
            return Promise.reject();
          }).then(function(data) {
            if (data !== version2) {
              $placeholder.text(stringFormat(stringResources.errorServiceVersion, [data, version2]));
              return Promise.reject();
            }
            TemplateCache.load(options.templateUrl, svcApiUrl, client).catch(function() {
              $placeholder.text(stringFormat(stringResources.errorLoadingTemplates, [escapeHtml(options.templateUrl)]));
              return Promise.reject();
            }).then(function(result) {
              templates = result.templates;
              return loadStyleSheets(result.styleSheets);
            }).then(start);
          });
        });
      }
      if (browserSupportsAllFeatures()) {
        main(version);
      } else {
        throw "The current browser does not support the Promise feature which is required for using the Report Viewer.";
      }
      return viewer;
    }
    var pluginName = "telerik_ReportViewer";
    $.fn[pluginName] = function(options) {
      if (this.selector && !options.selector) {
        options.selector = this.selector;
      }
      return this.each(function() {
        if (!$.data(this, pluginName)) {
          $.data(this, pluginName, new ReportViewer(this, options));
        }
      });
    };
    const plugins = [
      {
        name: "telerik_ReportViewer_DocumentMapArea",
        constructor: DocumentMapArea
      },
      {
        name: "telerik_ReportViewer_MainMenu",
        constructor: MainMenu
      },
      {
        name: "telerik_ReportViewer_PagesArea",
        constructor: PagesArea
      },
      {
        name: "telerik_ReportViewer_ParametersArea",
        constructor: ParametersArea
      },
      {
        name: "telerik_ReportViewer_SearchDialog",
        constructor: Search
      },
      {
        name: "telerik_ReportViewer_SendEmail",
        constructor: SendEmail
      },
      {
        name: "telerik_ReportViewer_SideMenu",
        constructor: SideMenu
      },
      {
        name: "telerik_ReportViewer_LinkButton",
        constructor: LinkButton
      },
      {
        name: "telerik_ReportViewer_PageNumberInput",
        constructor: PageNumberInput
      },
      {
        name: "telerik_ReportViewer_PageCountLabel",
        constructor: PageCountLabel
      }
    ];
    plugins.forEach((plugin) => {
      $.fn[plugin.name] = function(options, otherOptions) {
        return this.each(function() {
          if (!$.data(this, plugin.name)) {
            $.data(this, plugin.name, new plugin.constructor(this, options, otherOptions));
          }
        });
      };
    });

    exports.Accessibility = Accessibility;
    exports.Binder = Binder;
    exports.CommandSet = CommandSet;
    exports.DocumentMapAreaPositions = DocumentMapAreaPositions;
    exports.Events = Events$1;
    exports.GlobalSettings = GlobalSettings;
    exports.HistoryManager = HistoryManager;
    exports.PageModes = PageModes;
    exports.PagesArea = PagesArea;
    exports.ParameterEditorTypes = ParameterEditorTypes;
    exports.ParameterEditors = ParameterEditors;
    exports.ParameterTypes = ParameterTypes;
    exports.ParameterValidators = ParameterValidators;
    exports.ParametersArea = ParametersArea;
    exports.ParametersAreaPositions = ParametersAreaPositions;
    exports.PerspectiveManager = PerspectiveManager;
    exports.PrintManager = PrintManager;
    exports.PrintModes = PrintModes;
    exports.ReportViewer = ReportViewer;
    exports.ReportViewerController = ReportViewerController;
    exports.ScaleModes = ScaleModes;
    exports.Scroll = Scroll;
    exports.ServiceClient = ServiceClient;
    exports.TouchBehavior = TouchBehavior;
    exports.UIController = UIController;
    exports.UIFreezeCoordinator = UIFreezeCoordinator;
    exports.ViewModes = ViewModes;
    exports.domUtils = domUtils;
    exports.parameterEditorsMatch = parameterEditorsMatch;
    exports.utils = utils;

    Object.defineProperty(exports, '__esModule', { value: true });

    return exports;

})({});
