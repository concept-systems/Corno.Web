namespace Corno.Web;

public static class MapsterConfig
{
    public static void RegisterMappings()
    {
        /*TypeAdapterConfig<ProductDto, Product>
            .NewConfig()
            .Map(dest => dest.ProductItemDetails, src => src.ProductItemDtos.Adapt<List<ProductItemDetail>>())
            .Map(dest => dest.ProductPacketDetails, src => src.ProductPacketDtos.Adapt<List<ProductPacketDetail>>())
            .Map(dest => dest.ProductStockDetails, src => src.ProductStockDtos.Adapt<List<ProductStockDetail>>())
            .AfterMapping((src, dest) =>
            {
                dest.Id = src.Id;
                for (var index = 0; index < src.ProductItemDtos.Count; index++)
                    dest.ProductItemDetails[index].Id = src.ProductItemDtos[index].Id;
                for (var index = 0; index < src.ProductPacketDtos.Count; index++)
                    dest.ProductPacketDetails[index].Id = src.ProductPacketDtos[index].Id;
                for (var index = 0; index < src.ProductStockDtos.Count; index++)
                    dest.ProductStockDetails[index].Id = src.ProductStockDtos[index].Id;
            });
        TypeAdapterConfig<Product, ProductDto>
            .NewConfig()
            .Map(dest => dest.ProductItemDtos, src => src.ProductItemDetails.Adapt<List<ProductItemDto>>())
            .Map(dest => dest.ProductPacketDtos, src => src.ProductPacketDetails.Adapt<List<ProductPacketDto>>())
            .Map(dest => dest.ProductStockDtos, src => src.ProductStockDetails.Adapt<List<ProductStockDto>>())
            .AfterMapping((src, dest) =>
            {
                dest.Id = src.Id;
                for (var index = 0; index < src.ProductItemDetails.Count; index++)
                    dest.ProductItemDtos[index].Id = src.ProductItemDetails[index].Id;
                for (var index = 0; index < src.ProductPacketDetails.Count; index++)
                    dest.ProductPacketDtos[index].Id = src.ProductPacketDetails[index].Id;
                for (var index = 0; index < src.ProductStockDetails.Count; index++)
                    dest.ProductStockDtos[index].Id = src.ProductStockDetails[index].Id;
            });*/
    }
}
