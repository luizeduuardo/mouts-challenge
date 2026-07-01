using AutoMapper;
using Ambev.DeveloperEvaluation.Application.Sales.UpdateSaleItem;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.UpdateSaleItem;

public class UpdateSaleItemProfile : Profile
{
    public UpdateSaleItemProfile()
    {
        CreateMap<UpdateSaleItemRequest, UpdateSaleItemCommand>();
    }
}
