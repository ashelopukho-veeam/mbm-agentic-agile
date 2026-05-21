using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using mbt.webapi.Services;

namespace mbt.webapi.Utils;

public enum BreakOn
{
    None,
    FirstError
}

public static class BulkOperation
{
    public static async Task<BulkOperationResult> Run<T>(List<T> items,
        Func<T, Task> operationFunc, string operationName, BreakOn breakOn = BreakOn.None)
    {
        BulkOperationResult result = new(operationName);

        for (var i = 0; i < items.Count; i++)
        {
            try
            {
                var item = items[i];
                await operationFunc(item);
            }
            catch (Exception e)
            {
                result.AddError(i.ToString(), e.Message);

                if (breakOn == BreakOn.FirstError)
                {
                    break;
                }
            }
        }

        return result;
    }
}
