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
        private readonly Func<BindingContext, Task<int>> _action;

        public CommandSourceCommandHandler(
            Func<BindingContext, Task<int>> action)
        {
            _action = action ?? throw new ArgumentNullException(nameof(action)); ;
        }

        public async Task<int> InvokeAsync(InvocationContext context)
            => await _action(context.BindingContext);
    }
}