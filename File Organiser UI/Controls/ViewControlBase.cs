﻿namespace File.Organiser.UI.Controls
{
    using System;
    using System.ComponentModel;
    using System.Windows.Controls;
    using EyssyApps.Core.Library.Events;
    using EyssyApps.Organiser.Library.Factories;
    using EyssyApps.UI.Library.Controls;

    public abstract class ViewControlBase : UserControl, IViewControl
    {
        protected readonly IOrganiserFactory Factory;

        private readonly string viewName;
        private readonly bool isDefault; // TODO: perhaps make this as an attribute that is attached to a single view control ?

        protected ViewControlBase(string viewName, bool isDefault, IOrganiserFactory factory)
        {
            if (factory == null)
            {
                throw new ArgumentNullException(nameof(factory), "No factory has been provided");
            }

            if (string.IsNullOrWhiteSpace(viewName))
            {
                throw new ArgumentNullException(nameof(viewName), "No view name has been provided - the main window would not be able to route between the views.");
            }

            this.Factory = factory;

            this.viewName = viewName;
            this.isDefault = isDefault;
        }

        public string ViewControlName
        {
            get { return this.viewName; }
        }

        public bool IsDefault
        {
            get { return this.isDefault; }
        }

        public bool IsActive { get; set; }

        public event EventHandler<EventArgs<string>> OnChangeView;

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnViewChange(string viewName)
        {
            Invoker.Raise(ref this.OnChangeView, this, viewName);
        }

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = this.PropertyChanged;

            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}