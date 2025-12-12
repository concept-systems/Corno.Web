using System.Web.Mvc;

namespace Corno.Web.Extensions;

public static class ModelStateExtensions
{
    public static void ClearFields(this ModelStateDictionary modelState, params string[] fieldsToClear)
    {
        foreach (var field in fieldsToClear)
        {
            if (modelState.ContainsKey(field))
            {
                var value = modelState[field];
                value.Value = null; // Clear the value
            }
        }
    }
}
