using System;
using Autofac;
using JetBrains.Annotations;
using AutofacContainer = Autofac.IContainer;

namespace Saltukkos.Container
{
    public sealed class Container : IDisposable
    {
        [NotNull]
        private readonly AutofacContainer _container;

        public Container([NotNull] AutofacContainer container)
        {
            _container = container;
        }

        [NotNull]
        public T Resolve<T>()
        {
            return _container.Resolve<T>();
        }

        public void Dispose()
        {
            _container.Dispose();
        }
    }
}