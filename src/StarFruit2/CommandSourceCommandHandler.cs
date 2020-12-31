// Copyright (c) .NET Foundation and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.CommandLine.Binding;
using System.CommandLine.Invocation;
using System.Threading.Tasks;

namespace StarFruit2
{
    public class CommandSourceCommandHandler : ICommandHandler
    {
        private readonly Func<BindingContext, int> _action;

        public CommandSourceCommandHandler(
            Func<BindingContext, int> action)
        {
            _action = action ?? throw new ArgumentNullException(nameof(action)); ;
        }

        public Task<int> InvokeAsync(InvocationContext context)
        {
            return Task.FromResult(_action(context.BindingContext));
        }
    }
}