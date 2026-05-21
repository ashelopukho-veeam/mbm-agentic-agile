using FluentValidation;

namespace mbt.webapi.Endpoints.BudgetLevels;

public class CreateBudgetLevelsRequest
{
     public string ShortTitle { get; set; }
     public int Level { get; set; }
     public string Title { get; set; }
}


public class CreateBudgetLevelsRequestValidator : AbstractValidator<CreateBudgetLevelsRequest>
{
    public CreateBudgetLevelsRequestValidator()
    {
        RuleFor(x => x.ShortTitle).NotEmpty();
        RuleFor(x => x.Level).InclusiveBetween(1, 3);
        RuleFor(x => x.Title).NotEmpty();
    }
}
