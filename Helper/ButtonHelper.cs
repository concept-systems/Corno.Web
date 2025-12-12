using Kendo.Mvc.UI;
using Kendo.Mvc.UI.Fluent;

namespace Corno.Web.Helper;

public static class ButtonHelper
{
    /// <summary>
    /// Applies standard button settings: FillMode.Outline, Rounded.Full, with icon
    /// </summary>
    public static ButtonBuilder ApplyStandardSettings(
        this ButtonBuilder button,
        string icon = null,
        ComponentSize? size = null
    )
    {
        var builder = button
            .FillMode(ButtonFillMode.Outline)
            .Rounded(Rounded.Full);

        if (!string.IsNullOrEmpty(icon))
        {
            builder = builder.Icon(icon);
        }

        if (size.HasValue)
        {
            builder = builder.Size(size.Value);
        }

        return builder;
    }

    /// <summary>
    /// Applies standard settings with ThemeColor based on button type
    /// </summary>
    public static ButtonBuilder ApplyStandardSettingsWithType(
        this ButtonBuilder button,
        ButtonType buttonType = ButtonType.Primary,
        string icon = null,
        ComponentSize? size = null
    )
    {
        var themeColor = GetThemeColorForButtonType(buttonType);
        var defaultIcon = GetDefaultIconForButtonType(buttonType, icon);

        return button
            .ThemeColor(themeColor)
            .ApplyStandardSettings(defaultIcon, size);
    }

    /// <summary>
    /// Gets appropriate ThemeColor based on button type/content
    /// </summary>
    public static ThemeColor GetThemeColorForButtonType(ButtonType buttonType)
    {
        return buttonType switch
        {
            ButtonType.Primary => ThemeColor.Primary,
            ButtonType.Success => ThemeColor.Success,
            ButtonType.Warning => ThemeColor.Warning,
            ButtonType.Danger => ThemeColor.Error,
            ButtonType.Info => ThemeColor.Info,
            ButtonType.Secondary => ThemeColor.Secondary,
            ButtonType.Tertiary => ThemeColor.Tertiary,
            ButtonType.Import => ThemeColor.Info,
            ButtonType.Export => ThemeColor.Success,
            ButtonType.Edit => ThemeColor.Primary,
            ButtonType.Delete => ThemeColor.Error,
            ButtonType.View => ThemeColor.Info,
            ButtonType.Cancel => ThemeColor.Secondary,
            ButtonType.Save => ThemeColor.Success,
            ButtonType.Reset => ThemeColor.Secondary,
            _ => ThemeColor.Primary
        };
    }

    /// <summary>
    /// Gets default icon for button type if icon not provided
    /// </summary>
    public static string GetDefaultIconForButtonType(ButtonType buttonType, string customIcon = null)
    {
        if (!string.IsNullOrEmpty(customIcon))
            return customIcon;

        return buttonType switch
        {
            ButtonType.Primary => "plus",
            ButtonType.Success => "check",
            ButtonType.Warning => "warning",
            ButtonType.Danger => "close",
            ButtonType.Info => "info",
            ButtonType.Import => "import",
            ButtonType.Export => "export",
            ButtonType.Edit => "edit",
            ButtonType.Delete => "trash",
            ButtonType.View => "preview",
            ButtonType.Cancel => "cancel",
            ButtonType.Save => "save",
            ButtonType.Reset => "reload",
            _ => null
        };
    }
}

public enum ButtonType
{
    Primary,
    Success,
    Warning,
    Danger,
    Info,
    Secondary,
    Tertiary,
    Import,
    Export,
    Edit,
    Delete,
    View,
    Cancel,
    Save,
    Reset
}

