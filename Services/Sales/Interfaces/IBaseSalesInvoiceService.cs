using System.Threading.Tasks;
using Corno.Web.Areas.DemoProject.Dtos;
using Corno.Web.Models.Sales;
using Corno.Web.Services.Interfaces;

namespace Corno.Web.Services.Sales.Interfaces;

public interface IBaseSalesInvoiceService : ITransactionService<SalesInvoice>
{
    /// <summary>
    /// Creates or updates a <see cref="SalesInvoice"/> based on the supplied DTO,
    /// taking care of serial number and invoice code generation for new invoices.
    /// </summary>
    /// <param name="dto">Invoice DTO coming from UI.</param>
    /// <returns>The persisted <see cref="SalesInvoice"/> entity.</returns>
    Task<SalesInvoice> SaveInvoiceAsync(SalesInvoiceDto dto);
}
