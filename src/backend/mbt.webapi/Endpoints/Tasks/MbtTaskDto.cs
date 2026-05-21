using mbt.webapi.Domain.Entities;

namespace mbt.webapi.Endpoints.Tasks;

public class MbtTaskDto : BaseItemDto
{
    public string Type { get; set; }
    public string AssignedTo { get; set; }
    public string Outcome { get; set; }
    public string Status { get; set; }
    public string Comment { get; set; }

    public string AssociatedItemId { get; set; }
    public string AssociatedItemUrl { get; set; }
    public string Details { get; set; }

    public UserProfile AssignedToUser { get; set; }
    public UserProfile CreatedByUser { get; set; }
    public UserProfile ModifiedByUser { get; set; }
}
