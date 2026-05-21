namespace mbt.webapi.Domain.Entities;

public interface IBaseItemExpanded
{
    UserProfile CreatedByUser { get; set; }
    UserProfile ModifiedByUser { get; set; }
}
