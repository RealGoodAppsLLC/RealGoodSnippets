// <copyright file="IRegisterExceptionPromptHandlerService.cs" company="Real Good Apps">
// Copyright (c) Real Good Apps. All rights reserved.
// </copyright>

namespace RealGoodApps.Exceptions.Interfaces
{
    public interface IRegisterExceptionPromptHandlerService
    {
        void RegisterExceptionPromptHandler<TInput, TResult>(
            ReactiveCommand<TInput, TResult> reactiveCommand);
    }
}
