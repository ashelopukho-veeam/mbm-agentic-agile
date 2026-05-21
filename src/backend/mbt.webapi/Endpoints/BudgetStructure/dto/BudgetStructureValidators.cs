using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using mbt.webapi.BuiltIn;
using mbt.webapi.Domain.Entities;
using mbt.webapi.Repositories;
using mbt.webapi.Services.Interfaces;

namespace mbt.webapi.Endpoints.BudgetStructure.dto;

public static class BudgetStructureValidators
{
  public static async Task<bool> IsLevelExists(
      IApiService apiService,
      string levelTitle, int level)
  {
    var levels = await apiService.GetBudgetLevels();
    var hasLevel = levels.Any(l => l.Level == level && l.Title == levelTitle);
    return hasLevel;
  }

  public static async Task<bool> IsCostCenterExists(
      IApiService apiService,
      string costCenter, CancellationToken token)
  {
    var costCenters = await apiService.GetCostCenters();
    return costCenters.Any(c => c.Title == costCenter);
  }

  public static async Task<bool> IsBudgetTypeExists(
      IDbBaseRepository<DictionaryDocument> dictionaryRepository,
      string budgetType, CancellationToken token)
  {
    var budgetTypes =
        await dictionaryRepository.FindOneAsync(
            d => d.InternalName == BuiltInDictionariesInternalNames.BudgetTypes);
    return budgetTypes.Items.Any(i => i == budgetType);
  }

  public static async Task<bool> IsUserExists(
      IUserService userService,
      Guid? userId, CancellationToken token)
  {
    if (userId == null) return false;

    var user = await userService.GetAsync(userId.ToString());
    return user != null;
  }
}
