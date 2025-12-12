using System;
using System.Collections.Generic;
using System.Linq;

namespace Corno.Web.Helpers;

public static class MenuIconHelper
{
    // Whitelist of commonly available Kendo icons (ensure class names follow k-i-*
    private static readonly HashSet<string> KendoIconWhitelist = new(StringComparer.OrdinalIgnoreCase)
    {
        "k-i-home","k-i-user","k-i-cog","k-i-gear","k-i-calendar","k-i-package","k-i-palette","k-i-map-marker-target",
        "k-i-group","k-i-sort-asc","k-i-grid","k-i-grid-layout","k-i-arrow-60-up","k-i-arrow-60-down","k-i-truck",
        "k-i-lightbulb","k-i-settings","k-i-dashboard-outline","k-i-upload","k-i-refresh","k-i-lock","k-i-list-unordered",
        "k-i-sum","k-i-table","k-i-layout","k-i-export","k-i-warning","k-i-track-changes-enable","k-i-time",
        "k-i-zoom","k-i-information","k-i-mobile","k-i-logout","k-i-close","k-i-search","k-i-menu","k-i-tag"
    };

    // Common icon map used for Home dashboard and TreeView. Use safer icons from whitelist.
    private static readonly Dictionary<string, string> IconMapInternal = new(StringComparer.OrdinalIgnoreCase)
    {
        // File / common actions
        { "file", "k-i-menu" },
        { "logout", "k-i-logout" },
        { "exit", "k-i-close" },

        // Masters
        { "masters", "k-i-cog" },
        { "unit", "k-i-gear" },
        { "warehouse", "k-i-home" },
        { "item", "k-i-grid" },
        { "items", "k-i-grid" },
        { "customer", "k-i-user" },
        { "product", "k-i-grid" },
        { "products", "k-i-grid" },
        { "packingtype", "k-i-package" },
        { "shade", "k-i-palette" },
        { "color", "k-i-palette" },
        { "location", "k-i-map-marker-target" },

        // Planning
        { "planning", "k-i-calendar" },
        { "plan", "k-i-calendar" },

        // Labels (use tag icon for label-related)
        { "labels", "k-i-tag" },
        { "label", "k-i-tag" },
        { "partlabel", "k-i-tag" },
        { "storekhalapur", "k-i-dashboard-outline" },
        { "storeshirwal", "k-i-dashboard-outline" },
        { "stiffenerlabel", "k-i-tag" },
        { "trolleylabel", "k-i-tag" },

        // Productions / operations
        { "productions", "k-i-cog" },
        { "production", "k-i-cog" },
        { "kitting", "k-i-group" },
        { "sorting", "k-i-sort-asc" },
        { "subassembly", "k-i-grid" },

        // Packing
        { "packing", "k-i-package" },
        { "nonweighing", "k-i-weight" }, // if not available, will fallback
        { "weighing", "k-i-weight" }, // if not available, will fallback
        { "carcass", "k-i-grid-layout" },

        // WMS
        { "wms", "k-i-grid" },
        { "palletin", "k-i-arrow-60-up" },
        { "rackin", "k-i-arrow-60-up" },
        { "rackout", "k-i-arrow-60-down" },
        { "dispatch", "k-i-truck" },

        // Put To Light
        { "puttolight", "k-i-lightbulb" },
        { "trolleyconfig", "k-i-settings" },
        { "trolley", "k-i-grid" },

        // Reports
        { "reports", "k-i-dashboard-outline" },
        { "report", "k-i-dashboard-outline" },
        { "summary", "k-i-sum" },
        { "detail", "k-i-table" },
        { "panel", "k-i-layout" },
        { "carton", "k-i-package" },
        { "handover", "k-i-export" },
        { "delivery", "k-i-truck" },
        { "finaldispatch", "k-i-truck" },
        { "shortage", "k-i-warning" },
        { "pallettracking", "k-i-track-changes-enable" },
        { "wip", "k-i-time" },
        { "scanning", "k-i-zoom" },
        { "racking", "k-i-grid" },
        { "currentrackinstatus", "k-i-information" },

        // Import / Android
        { "import", "k-i-upload" },
        { "autoimport", "k-i-refresh" },
        { "dashboard", "k-i-dashboard-outline" },
        { "android", "k-i-mobile" },

        // Admin
        { "admin", "k-i-settings" },
        { "users", "k-i-user" },
        { "user", "k-i-user" },
        { "roles", "k-i-lock" },
        { "role", "k-i-lock" },
        { "changepassword", "k-i-lock" },
        { "logs", "k-i-list-unordered" },
        { "log", "k-i-list-unordered" }
    };

    public static IReadOnlyDictionary<string, string> IconMap => IconMapInternal;

    private static string ValidateIcon(string iconClass)
    {
        if (string.IsNullOrWhiteSpace(iconClass))
            return "k-i-folder";

        // normalize legacy patterns
        iconClass = iconClass.Replace("k-icon-", "k-i-");

        return KendoIconWhitelist.Contains(iconClass) ? iconClass : "k-i-folder";
    }

    /// <summary>
    /// Returns a Kendo icon CSS class based on menu metadata.
    /// Logic: normalize all hints (icon/name/title), then if any IconMap key is contained in a hint, use that icon; otherwise folder icon.
    /// Validates against known Kendo icons and falls back to folder if not found.
    /// </summary>
    public static string GetIconClass(string title, string name = null, string icon = null)
    {
        /*// Explicit icon takes precedence
        if (!string.IsNullOrWhiteSpace(icon))
            return ValidateIcon(icon);*/

        // Collect hints in order: name, then title
        var hints = new[] { name, title }
            .Where(h => !string.IsNullOrWhiteSpace(h))
            .Select(h => h.Replace(" ", "").ToLowerInvariant())
            .ToList();
        if (!hints.Any())
            return "k-i-folder";

        foreach (var hint in hints)
        {
            var matchedKey = IconMapInternal.Keys.FirstOrDefault(k => hint.Contains(k));
            if (!string.IsNullOrEmpty(matchedKey))
                return ValidateIcon(IconMapInternal[matchedKey]);
        }

        return "k-i-folder";
    }
}


