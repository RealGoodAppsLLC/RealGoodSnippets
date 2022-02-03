// <copyright file="StandaloneValidator.cs" company="Real Good Apps">
// Copyright (c) Real Good Apps. All rights reserved.
// </copyright>

namespace RealGoodApps.Validation
{
    public static class StandaloneValidator
    {
        public static async Task<(TRequest? Request, ModelStateDictionary ModelState)> ReadAndValidateAsync<TRequest>(HttpRequest req)
            where TRequest : class
        {
            var requestJson = await req.ReadAsStringAsync();

            TRequest? requestModel;
            var modelState = new ModelStateDictionary();

            try
            {
                requestModel = JsonConvert.DeserializeObject<TRequest>(requestJson);
            }
            catch (Exception)
            {
                modelState.AddModelError(string.Empty, "The request is no formatted properly.");
                return (null, modelState);
            }

            if (requestModel == null)
            {
                modelState.AddModelError(string.Empty, "Your request must not be empty.");
                return (null, modelState);
            }

            var context = new ValidationContext(requestModel, null, null);
            var validationResults = new List<ValidationResult>();

            Validator.TryValidateObject(requestModel, context, validationResults, true);

            var validationResultsValueImmutableList = validationResults.ToValueImmutableList();

            foreach (var validationResult in validationResultsValueImmutableList)
            {
                foreach (var memberName in validationResult.MemberNames)
                {
                    modelState.AddModelError(memberName, validationResult.ErrorMessage);
                }
            }

            return (requestModel, modelState);
        }
    }
}
