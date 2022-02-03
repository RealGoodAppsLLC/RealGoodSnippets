// <copyright file="RegisterExceptionPromptHandlerService.cs" company="Real Good Apps">
// Copyright (c) Real Good Apps. All rights reserved.
// </copyright>

namespace RealGoodApps.Exceptions.Services
{
    /// <inheritdoc cref="IRegisterExceptionPromptHandlerService"/>
    public sealed class RegisterExceptionPromptHandlerService : IRegisterExceptionPromptHandlerService
    {
        /// <inheritdoc cref="IRegisterExceptionPromptHandlerService"/>
        public void RegisterExceptionPromptHandler<TInput, TResult>(
            ReactiveCommand<TInput, TResult> reactiveCommand)
        {
            if (reactiveCommand == null)
            {
                throw new ArgumentNullException(nameof(reactiveCommand));
            }

            reactiveCommand
                .ThrownExceptions
                .SelectMany(async ex =>
                {
                    var displayMessage = GetDisplayMessageFromException(ex);

                    await Application.Current.MainPage.DisplayAlert(
                        "Error",
                        displayMessage,
                        "OK");

                    return Unit.Default;
                })
                .Subscribe();
        }

        private static string GetDisplayMessageFromException(Exception ex)
        {
            var unknownErrorMessage = "An unknown error has occurred.";

            if (!(ex is ApiException apiException))
            {
                return unknownErrorMessage;
            }

            // TODO: Give a different message for network exception.
            if (apiException.StatusCode != HttpStatusCode.BadRequest)
            {
                return unknownErrorMessage;
            }

            var validationErrors = apiException.Errors;

            var fullMessageBuilder = new StringBuilder();

            foreach (var (_, errors) in validationErrors)
            {
                foreach (var error in errors)
                {
                    fullMessageBuilder.AppendLine(error);
                }
            }

            var fullMessage = fullMessageBuilder.ToString().Trim();

            return string.IsNullOrWhiteSpace(fullMessage)
                ? unknownErrorMessage
                : fullMessage;
        }
    }
}
