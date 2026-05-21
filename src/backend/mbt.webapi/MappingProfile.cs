using AutoMapper;
using mbt.webapi.Domain.Entities;
using mbt.webapi.Endpoints.BudgetStructure.dto;
using mbt.webapi.Endpoints.CommonConfig;
using mbt.webapi.Endpoints.GroupedActivities;
using mbt.webapi.Endpoints.IncrementalFunds;
using mbt.webapi.Endpoints.IncrementalFunds.dto;
using mbt.webapi.Endpoints.PaidMediaSet.dto;
using mbt.webapi.Endpoints.Tasks;
using mbt.webapi.Endpoints.Transfers;
using mbt.webapi.Endpoints.Transfers.dto;
using mbt.webapi.UseCases.PaidMediaSet;

namespace mbt.webapi;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Budget, BudgetStructureDto>();
        CreateMap<BudgetPlan, BudgetPlanDto>();
        // CreateMap<BudgetStructureExpanded, BudgetStructureExpandedDto>();

        CreateMap<GroupedActivity, GroupedActivityDto>();
        CreateMap<GroupedActivityExpanded, GroupedActivityExpandedDto>();

        CreateMap<CreateIncrementalFundRequest, IncrementalFund>();
        CreateMap<IncrementalFund, IncrementalFundDto>();
        CreateMap<IncrementalFundExpanded, IncrementalFundExpandedDto>();

        CreateMap<MbtTask, MbtTaskDto>();
        CreateMap<MbtTaskExpanded, MbtTaskDto>();

        CreateMap<Transfer, TransferDto>();
        CreateMap<EditTransferRequest, Transfer>();
        CreateMap<TransferExpanded, TransferExpandedDto>();
        CreateMap<TransferExpanded, TransferWithDetailsDto>()
            .ForMember(d => d.FromBudgetName,
                opt => opt.MapFrom(src => src.FromBudget.Title))
            .ForMember(d => d.ToBudgetName,
                opt => opt.MapFrom(src => src.ToBudget.Title));

        CreateMap<IncrementalFundExpanded, IncrementalFundWithDetailsDto>()
            .ForMember(d => d.ToBudgetName,
                opt => opt.MapFrom(src => src.ToBudget.Title));


        CreateMap<CommonConfig, CommonConfigDto>();
        CreateMap<SetCommonConfigRequest, CommonConfig>();

        CreateMap<BudgetPlan, BudgetPlanHistoryItem>()
            .ForMember(d => d.OriginalId,
                opt => opt.MapFrom(src => src.Id))
            .ForMember(d => d.Id, opt => opt.Ignore());

        CreateMap<PaidMediaSetWithLinkedItem, PaidMediaSetDto>();
    }
}
