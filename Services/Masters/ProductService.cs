using System.Linq;
using Corno.Web.Models.Masters;
using Corno.Web.Repository.Interfaces;
using Corno.Web.Services.Interfaces;
using Corno.Web.Services.Masters.Interfaces;

namespace Corno.Web.Services.Masters;

public class ProductService : MasterService<Product>, IProductService
{
    #region -- Constructors --
    public ProductService(IGenericRepository<Product> genericRepository, 
        IMiscMasterService miscMasterService) : base(genericRepository)
    {
        _miscMasterService = miscMasterService;
        // Do not enable includes globally; use explicit methods when details are needed.
        SetIncludes($"{nameof(Product.ProductItemDetails)}," +
                    //$"{nameof(Product.ProductAssemblyDetails)}," +
                    $"{nameof(Product.ProductPacketDetails)}," +
                    $"{nameof(Product.ProductStockDetails)}");
    }
    #endregion

    #region -- Data Members --
    private readonly IMiscMasterService _miscMasterService;
    #endregion

    #region -- Public Methods --

    /// <summary>
    /// Get product with related details (item, packet, stock) loaded.
    /// </summary>
    public async System.Threading.Tasks.Task<Product> GetByIdWithDetailsAsync(int id)
    {
        /*SetIncludes($"{nameof(Product.ProductItemDetails)}," +
                    $"{nameof(Product.ProductPacketDetails)}," +
                    $"{nameof(Product.ProductStockDetails)}");*/
        var list = await GetAsync<Product>(p => p.Id == id, p => p, null, ignoreInclude: false)
            .ConfigureAwait(false);
        return list.FirstOrDefault();
    }

    #endregion

    #region -- Protected Methods --
    protected override void UpdateFields(Product entity, Product newEntity)
    {
        newEntity.Mrp = entity.Mrp;
        newEntity.SalePrice = entity.SalePrice;
        newEntity.CostPrice = entity.CostPrice;
        newEntity.Weight = entity.Weight;
        newEntity.WidthTolerance = entity.WidthTolerance;
    }
    #endregion
}