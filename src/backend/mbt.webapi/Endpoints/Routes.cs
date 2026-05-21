namespace mbt.webapi.Endpoints
{
    public static class IncrementalFundsRoutes
    {
        public const string Base = "api/incrementalFunds";
        public const string Submit = Base + "/{Id}/submit";
    }

  public static class TransferRoutes
  {
    public const string Base = "api/transfers";
    public const string Get = Base + "/{Id}";
    public const string Update = Base;
  }


  public static class BudgetPlanRoutes
  {
    public const string Base = "api/budgetplan";
    public const string Update = Base;
    public const string Finalize = Base + "/finalize";
  }

  public static class PaidMediaTeamApproverRoutes
  {
    private const string Base = "api/admin/paidmediateamapprovers";
    public const string List = Base;
    public const string Set = Base;
    public const string Delete = Base + "/{Id}";
  }
}
