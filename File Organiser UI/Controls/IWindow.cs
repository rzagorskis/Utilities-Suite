﻿namespace File.Organiser.UI.Controls
{
    using System.ComponentModel;

    public interface IWindow : INotifyPropertyChanged
    {
        IViewControl ActiveView { get; }

        object TryRetrieveResource(string name);

        void ShowWindow();

        void CloseWindow();
    }
}