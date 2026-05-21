using mbt.webapi.Domain.Entities;

namespace mbt.webapi.Endpoints.PaidMediaSet.dto;

public class PaidMediaSetDto : BaseItemDto
{
    public UserProfile CreatedByUser { get; set; }
    public UserProfile ModifiedByUser { get; set; }

    public string PaidMediaSetType { get; set; }
    public string LinkedItemId { get; set; }
    public BaseLinkedItem LinkedItem { get; set; }

    public string Comment { get; set; }
}
