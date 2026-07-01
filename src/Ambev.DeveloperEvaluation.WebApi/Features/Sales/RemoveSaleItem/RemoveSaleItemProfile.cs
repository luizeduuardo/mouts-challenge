using AutoMapper;
using Ambev.DeveloperEvaluation.Application.Sales.RemoveSaleItem;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.RemoveSaleItem;

public class RemoveSaleItemProfile : Profile
{
    public RemoveSaleItemProfile()
    {
        CreateMap<RemoveSaleItemRequest, RemoveSaleItemCommand>();
    }
}
