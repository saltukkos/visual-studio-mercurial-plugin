using System;
using System.Collections.Concurrent;
using Autofac;
using JetBrains.Annotations;
using Saltukkos.Utils;

namespace Saltukkos.Container
{
    internal class LifetimeExpandingController : ILifetimeExpandingController
    {
        [NotNull]
        private readonly ConcurrentDictionary<object, ConcurrentBag<Action>> _pendingActions =
            new ConcurrentDictionary<object, ConcurrentBag<Action>>();

        public LifetimeDisposer StartNestedLifetime<TScope>(
            ILifetimeScope parentLifetimeScope,
            Action<Autofac.ContainerBuilder> builder)
        {
            ThrowIf.Null(builder, nameof(builder));
            ThrowIf.Null(parentLifetimeScope, nameof(parentLifetimeScope));

            LifetimeStartExpanding<TScope>();

            var syncRoot = new object();
            ILifetimeScope createdNestedLifetime = null;
            var noNeedToCreateLifetime = false;

            if (_pendingActions.TryGetValue(Ensure.NotNull(parentLifetimeScope.Tag), out var actions))
            {
                actions.Add(() =>
                {
                    lock (syncRoot)
                    {
                        if (noNeedToCreateLifetime)
                        {
                            return;
                        }

                        createdNestedLifetime = parentLifetimeScope.BeginLifetimeScope(typeof(TScope), builder);
                        LifetimeEndExpanding<TScope>();
                    }
                });
            }
            else
            {
                createdNestedLifetime = parentLifetimeScope.BeginLifetimeScope(typeof(TScope), builder);
                LifetimeEndExpanding<TScope>();
            }

            // ReSharper disable once ImplicitlyCapturedClosure
            return () =>
            {
                lock (syncRoot)
                {
                    noNeedToCreateLifetime = true;
                    createdNestedLifetime?.Dispose();
                }
            };
        }

        private void LifetimeStartExpanding<TScope>()
        {
            if (!_pendingActions.TryAdd(typeof(TScope), new ConcurrentBag<Action>()))
            {
                throw new InvalidOperationException($"Lifetime scope {typeof(TScope)} is already starting");
            }
        }

        private void LifetimeEndExpanding<TScope>()
        {
            if (!_pendingActions.TryRemove(typeof(TScope), out var actions))
            {
                throw new InvalidOperationException($"Lifetime scope {typeof(TScope)} was not registered for starting");
            }

            foreach (var action in actions)
            {
                action.Invoke();
            }
        }
    }
}