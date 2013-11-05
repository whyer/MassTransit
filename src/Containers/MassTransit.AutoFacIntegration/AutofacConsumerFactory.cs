// Copyright 2007-2011 Chris Patterson, Dru Sellers, Travis Smith, et. al.
//  
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use 
// this file except in compliance with the License. You may obtain a copy of the 
// License at 
// 
//     http://www.apache.org/licenses/LICENSE-2.0 
// 
// Unless required by applicable law or agreed to in writing, software distributed 
// under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR 
// CONDITIONS OF ANY KIND, either express or implied. See the License for the 
// specific language governing permissions and limitations under the License.
namespace MassTransit.AutofacIntegration
{
    using System;
    using System.Collections.Generic;
    using Autofac;
    using Exceptions;
    using Pipeline;

    public class AutofacConsumerFactory<T> :
        IConsumerFactory<T>
        where T : class
    {
        readonly ILifetimeScope _scope;
        readonly string _name;

        public AutofacConsumerFactory(ILifetimeScope scope, string name)
        {
            _scope = scope;
            _name = name;
        }

        public IEnumerable<Action<IConsumeContext<TMessage>>> GetConsumer<TMessage>(
            IConsumeContext<TMessage> context, InstanceHandlerSelector<T, TMessage> selector)
            where TMessage : class
        {
            using (var innerScope = _scope.BeginLifetimeScope(_name))
            {
                var consumer = innerScope.Resolve<T>();
                if (consumer == null)
                    throw new ConfigurationException(string.Format("Unable to resolve type '{0}' from container: ",
                        typeof (T)));

                foreach (var handler in selector(consumer, context))
                {
                    yield return handler;
                }
            }
        }
    }
}