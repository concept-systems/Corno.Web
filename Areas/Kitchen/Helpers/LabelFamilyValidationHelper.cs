using System;
using System.Collections.Generic;
using System.Linq;
using Corno.Web.Globals;
using Corno.Web.Models.Packing;
using Corno.Web.Models.Plan;

namespace Corno.Web.Areas.Kitchen.Helper;

/// <summary>
/// Helper class for validating labels belonging to families 22 and 23
/// </summary>
public static class LabelFamilyValidationHelper
{
    private static readonly string[] Families2223 = { "FGWN22", "FGWN23" };

    /// <summary>
    /// Validates if a label belongs to families 22 or 23
    /// </summary>
    /// <param name="group">The group/family code from PlanItemDetail</param>
    /// <returns>True if the label belongs to families 22 or 23, false otherwise</returns>
    public static bool IsFamily22Or23(string group)
    {
        return !string.IsNullOrEmpty(group) && Families2223.Contains(group);
    }

    /// <summary>
    /// Validates label status for families 22 & 23 during assembly.
    /// For families 22 & 23, at least one label should be sorted.
    /// For other families, all labels must be sorted.
    /// </summary>
    /// <param name="labels">List of labels to validate</param>
    /// <param name="plan">Plan containing plan item details</param>
    /// <param name="expectedStatus">Expected status for labels (default: Sorted)</param>
    /// <exception cref="Exception">Thrown when validation fails</exception>
    public static void ValidateLabelsForFamilies2223(IReadOnlyList<Label> labels, Plan plan, string[] expectedStatus = null)
    {
        if (labels == null || labels.Count == 0)
            return;

        if (expectedStatus == null)
            expectedStatus = new[] { StatusConstants.Sorted };

        for (var index = 0; index < labels.Count; index++)
        {
            var label = labels[index];
            if (!expectedStatus.Contains(label.Status))
            {
                // Check whether family is 22 / 23
                var planItemDetail = plan?.PlanItemDetails.FirstOrDefault(d => d.Position == label.Position);
                if (IsFamily22Or23(planItemDetail?.Group))
                {
                    if (labels.All(d => d.Status != StatusConstants.Sorted))
                        throw new Exception($"At least single label should be sorted.");
                }
                else
                {
                    throw new Exception($"Label '{label.Barcode}' has '{label.Status}' status. Expected status is '{string.Join(",", expectedStatus)}'.");
                }
            }
        }
    }

    /// <summary>
    /// Validates subassembled labels for families 22 & 23 during packing.
    /// For families 22 & 23, at least one label should be sorted.
    /// For other families, all labels must be sorted.
    /// </summary>
    /// <param name="labels">List of labels to validate</param>
    /// <param name="plan">Plan containing plan item details</param>
    /// <param name="expectedStatus">Expected status for labels (default: Sorted)</param>
    /// <exception cref="Exception">Thrown when validation fails</exception>
    public static void ValidateSubAssembledLabelsForFamilies2223(IReadOnlyList<Label> labels, Plan plan, string[] expectedStatus = null)
    {
        if (labels == null || labels.Count == 0)
            return;

        if (expectedStatus == null)
            expectedStatus = new[] { StatusConstants.Sorted };

        // Check if any label belongs to families 22 or 23
        var hasFamily2223 = labels.Any(label =>
        {
            var planItemDetail = plan?.PlanItemDetails.FirstOrDefault(d => d.Position == label.Position);
            return IsFamily22Or23(planItemDetail?.Group);
        });

        if (hasFamily2223)
        {
            // For families 22 & 23, at least one label should be sorted
            if (labels.All(d => d.Status != StatusConstants.Sorted))
                throw new Exception($"At least single label should be sorted for families 22 & 23.");
        }
        else
        {
            // For other families, all labels must be sorted
            var nonSortedLabels = labels.Where(l => !expectedStatus.Contains(l.Status)).ToList();
            if (nonSortedLabels.Any())
            {
                var barcodes = string.Join(", ", nonSortedLabels.Select(l => l.Barcode));
                throw new Exception($"Labels '{barcodes}' have invalid status. Expected status is '{string.Join(",", expectedStatus)}'.");
            }
        }
    }
}

