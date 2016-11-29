﻿namespace Zagorapps.Configuration.Library
{
    using System;
    using System.Collections.Generic;
    using Core.Library.Extensions;
    using Core.Library.Factories;
    using Core.Library.Managers;
    using Core.Library.Timing;
    using Core.Library.Windows;
    using Core.Library.Windows.Registry;
    using Extensions;
    using Organiser.Library.Factories;
    using SimpleInjector;

    public class CommonBindings
    {
        protected readonly IList<BindingMetadata> Bindings;

        public CommonBindings()
        {
            this.Bindings = new List<BindingMetadata>();

            this.LoadBindings();
        }

        public void RegisterBindingsToContainer(Container container)
        {
            this.Bindings.ForEach(b =>
            {
                if (b.IsFactoy)
                {
                    container.BindFactory(b.Service);
                }
                else
                {
                    if (b.InstanceCreator == null)
                    {
                        container.Register(b.Service, b.Implementation, b.Lifestyle);
                    }
                    else
                    {
                        container.Register(b.Service, () => b.InstanceCreator(container), b.Lifestyle);
                    }
                }
            });

            this.Bindings.Clear();
        }

        protected virtual void LoadBindings()
        {
            this.BindFactories();
            this.BindTimers();
            this.BindManagers();
            this.BindServices();
        }

        protected virtual void BindServices()
        {
            this.Bind<IIniFileManager, IniFileManager>();
            this.Bind<IWinSystemService, WinSystemService>();
        }

        protected virtual void BindManagers()
        {
            this.Bind<IFileManager, LocalFileManager>();
            this.Bind<IDirectoryManager, LocalDirectoryManager>();
            this.Bind<IApplicationRegistryManager>(container =>
            {
                return new ApplicationRegistryManager("File-Organiser");
            }, Lifestyle.Singleton);
        }

        protected virtual void BindFactories()
        {
            this.Bind<IFactory>(container => container.GetInstance<IOrganiserFactory>(), Lifestyle.Singleton);
            this.BindFactory<IOrganiserFactory>();
        }

        protected virtual void BindTimers()
        {
            this.Bind<ITimer, ThreadedTimer>();
        }

        protected void BindFactory<TFactory>()
        {
            this.Bind(typeof(TFactory), null, true, Lifestyle.Singleton);
        }

        protected void Bind<TService, TImplementation>(bool isFactory = false, Lifestyle lifestyle = null)
            where TService : class
            where TImplementation : class, TService
        {
            Type service = typeof(TService);
            Type implementation = typeof(TImplementation);

            this.Bind(service, implementation, isFactory, lifestyle);
        }

        protected void Bind<TService>(Func<Container, object> instanceCreator, Lifestyle lifestyle = null)
        {
            this.Bind(typeof(TService), instanceCreator, lifestyle);
        }

        protected void Bind(Type service, Func<Container, object> instanceCreator, Lifestyle lifestyle = null)
        {
            if (service == null)
            {
                throw new ArgumentNullException(nameof(service), "Service type has not been provided - The container would not be able to register the implementation to the service."); // TODO: resources
            }

            if (instanceCreator == null)
            {
                throw new ArgumentNullException(nameof(instanceCreator), "Implementation type has not been provided - The container would not be able to register the service to the requested implementation");
            }

            BindingMetadata metadata = this.CreateMetadata(service, null, instanceCreator, false, lifestyle ?? Lifestyle.Transient);

            this.Bindings.Add(metadata);
        }

        protected void Bind(Type service, Type implementation, bool isFactory = false, Lifestyle lifesyle = null)
        {
            if (service == null)
            {
                throw new ArgumentNullException(nameof(service), "Service type has not been provided - The container would not be able to register the implementation to the service.");
            }

            BindingMetadata metadata;
            if (isFactory)
            {
                metadata = this.CreateMetadata(service, null, null, true, lifesyle ?? Lifestyle.Transient);
            }
            else
            {
                if (implementation == null)
                {
                    throw new ArgumentNullException(nameof(implementation), "Implementation type has not been provided - The container would not be able to register the service to the requested implementation");
                }

                metadata = this.CreateMetadata(service, implementation, null, false, lifesyle ?? Lifestyle.Transient);
            }

            this.Bindings.Add(metadata);
        }

        protected sealed class BindingMetadata
        {
            public bool IsFactoy { get; set; }

            public Func<Container, object> InstanceCreator { get; set; }

            public Type Service { get; set; }

            public Type Implementation { get; set; }

            public Lifestyle Lifestyle { get; set; }
        }

        protected BindingMetadata CreateMetadata(Type service, Type implementation, Func<Container, object> instanceCreator, bool isFactory, Lifestyle lifesyle)
        {
            return new BindingMetadata
            {
                Service = service,
                Implementation = implementation,
                InstanceCreator = instanceCreator,
                IsFactoy = isFactory,
                Lifestyle = lifesyle
            };
        }
    }
}