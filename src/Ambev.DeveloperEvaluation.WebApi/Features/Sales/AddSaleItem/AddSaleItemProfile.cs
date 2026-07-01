using AutoMapper;
using Ambev.DeveloperEvaluation.Application.Sales.AddSaleItem;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.AddSaleItem;

public class AddSaleItemProfile : Profile
{
    public AddSaleItemProfile()
    {
        CreateMap<AddSaleItemRequest, AddSaleItemCommand>();
    }
}
