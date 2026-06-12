using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Globalization;

namespace ClinicManager.Utils
{
    public sealed class InvariantDoubleModelBinder : IModelBinder
    {
        public Task BindModelAsync(ModelBindingContext ctx)
        {
            var result = ctx.ValueProvider.GetValue(ctx.ModelName);
            if (result == ValueProviderResult.None)
                return Task.CompletedTask;

            ctx.ModelState.SetModelValue(ctx.ModelName, result);
            var raw = result.FirstValue;

            if (string.IsNullOrWhiteSpace(raw))
            {
                ctx.Result = ModelBindingResult.Success(0.0);
                return Task.CompletedTask;
            }

            if (double.TryParse(raw, NumberStyles.Any, CultureInfo.InvariantCulture, out var value))
                ctx.Result = ModelBindingResult.Success(value);
            else
                ctx.ModelState.TryAddModelError(ctx.ModelName, "Please enter a valid number.");

            return Task.CompletedTask;
        }
    }
}
