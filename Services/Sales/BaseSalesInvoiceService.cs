using System;
using System.Linq;
using System.Threading.Tasks;
using Corno.Web.Areas.DemoProject.Dtos;
using Corno.Web.Models.Sales;
using Corno.Web.Repository.Interfaces;
using Corno.Web.Services.Sales.Interfaces;
using Mapster;

namespace Corno.Web.Services.Sales;

public class BaseSalesInvoiceService : TransactionService<SalesInvoice>, IBaseSalesInvoiceService
{
    #region -- Constructors --
    public BaseSalesInvoiceService(IGenericRepository<SalesInvoice> genericRepository) : base(genericRepository)
    {
        // Do not enable includes globally; use explicit methods when details are needed.
        SetIncludes(nameof(SalesInvoice.SalesInvoiceDetails));
    }
    #endregion

    #region -- Public Methods --

    /// <summary>
    /// Get SalesInvoice by id without loading details (faster, default for read-only scenarios)
    /// </summary>
    public async Task<SalesInvoice> GetByIdWithoutDetailsAsync(int id)
    {
        var list = await GetAsync<SalesInvoice>(i => i.Id == id, i => i, null, ignoreInclude: true)
            .ConfigureAwait(false);
        return list.FirstOrDefault();
    }

    /// <summary>
    /// Get SalesInvoice by id with SalesInvoiceDetails loaded
    /// </summary>
    public async Task<SalesInvoice> GetByIdWithDetailsAsync(int id)
    {
        var list = await GetAsync<SalesInvoice>(i => i.Id == id, i => i, null, ignoreInclude: false)
            .ConfigureAwait(false);
        return list.FirstOrDefault();
    }

    /// <inheritdoc />
    public async Task<SalesInvoice> SaveInvoiceAsync(SalesInvoiceDto dto)
    {
        if (dto == null) throw new ArgumentNullException(nameof(dto));

        // New invoice
        if (dto.Id <= 0)
        {
            var entity = dto.Adapt<SalesInvoice>();

            // Generate next serial number
            var nextSerialNo = await GetNextSerialNoAsync().ConfigureAwait(false);
            entity.SerialNo = nextSerialNo;

            // Generate invoice code as: HAF/FinancialYear/SerialNo
            var invoiceDate = dto.InvoiceDate == default ? DateTime.Now : dto.InvoiceDate;
            entity.Code = GenerateInvoiceCode(invoiceDate, nextSerialNo);

            await AddAndSaveAsync(entity).ConfigureAwait(false);

            // Push back generated values to DTO for caller convenience
            dto.Id = entity.Id;
            dto.Code = entity.Code;

            return entity;
        }

        // Update existing invoice
        var existing = await FirstOrDefaultAsync(s => s.Id == dto.Id, s => s).ConfigureAwait(false);
        if (existing == null)
            throw new Exception("Invalid Id");

        // Replace details with incoming collection
        existing.SalesInvoiceDetails.Clear();
        dto.Adapt(existing); // Mapster will map header + new details

        await UpdateAndSaveAsync(existing).ConfigureAwait(false);

        // Ensure DTO reflects latest values
        dto.Code = existing.Code;

        return existing;
    }

    #endregion

    #region -- Private Helpers --

    /// <summary>
    /// Returns financial year string in format YYYY-YY (e.g. 2025-26) based on invoice date.
    /// Assumes financial year from April to March.
    /// </summary>
    private static string GetFinancialYearString(DateTime invoiceDate)
    {
        var startYear = invoiceDate.Month >= 4 ? invoiceDate.Year : invoiceDate.Year - 1;
        var endYearShort = (startYear + 1) % 100;
        return $"{startYear}-{endYearShort:00}";
    }

    /// <summary>
    /// Generates invoice code in format: HAF/FinancialYear/SerialNo
    /// Example: HAF/2025-26/123
    /// </summary>
    private static string GenerateInvoiceCode(DateTime invoiceDate, int serialNo)
    {
        var fy = GetFinancialYearString(invoiceDate);
        return $"HAF/{fy}/{serialNo}";
    }

    #endregion
}