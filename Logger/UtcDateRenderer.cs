using System.Globalization;
using System.Text;
using NLog;
using NLog.Config;
using NLog.LayoutRenderers;

namespace Corno.Web.Logger;

[LayoutRenderer("utc_date")]
public class UtcDateRenderer : LayoutRenderer
{

    ///
    /// Initializes a new instance of the  class.
    ///
    public UtcDateRenderer()
    {
        Format = "G";
        Culture = CultureInfo.InvariantCulture;
    }

    protected int GetEstimatedBufferSize(LogEventInfo ev)
    {
        // Dates can be 6, 8, 10 bytes so let's go with 10
        return 10;
    }

    ///
    /// Gets or sets the culture used for rendering.
    ///
    ///
    public CultureInfo Culture { get; set; }

    ///
    /// Gets or sets the date format. Can be any argument accepted by DateTime.ToString(format).
    ///
    ///
    [DefaultParameter]
    public string Format { get; set; }

    /// Renders the current date and appends it to the specified .
    protected override void Append(StringBuilder builder, LogEventInfo logEvent)
    {
        builder.Append(logEvent.TimeStamp.ToUniversalTime().ToString(Format, Culture));
    }

}