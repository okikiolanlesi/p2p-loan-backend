using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using P2PLoan.DTOs;
using P2PLoan.DTOs.SearchParams;

namespace P2PLoan.ModelBinders;

public class LoanRequestOrderByModelBinder : IModelBinder
{
    public Task BindModelAsync(ModelBindingContext bindingContext)
    {
        var orderByValues = bindingContext.ValueProvider.GetValue("orderBy").ToString().Split(',');

        var orderByList = new List<(LoanRequestOrderByField Field, SortDirection Direction)>();
        foreach (var value in orderByValues)
        {
            var parts = value.Split(' ');
            if (Enum.TryParse<LoanRequestOrderByField>(parts[0], true, out var field) &&
                Enum.TryParse<SortDirection>(parts[1], true, out var direction))
            {
                if (!orderByList.Any(o => o.Field == field))
                {
                    orderByList.Add((field, direction));
                }
            }
        }

        bindingContext.Result = ModelBindingResult.Success(orderByList);
        return Task.CompletedTask;
    }
}
