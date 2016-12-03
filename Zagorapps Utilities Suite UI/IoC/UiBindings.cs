﻿namespace Zagorapps.Utilities.Suite.UI.IoC
{
    using System;
    using System.Configuration;
    using System.Windows.Controls;
    using Commands;
    using Controls;
    using Library.Facilitators;
    using Newtonsoft.Json;
    using Services;
    using SimpleInjector;
    using Zagorapps.Configuration.Library;
    using Zagorapps.Core.Library.Managers;
    using Zagorapps.Utilities.Library.Factories;
    using Zagorapps.Utilities.Library.Managers;
    using Zagorapps.Utilities.Library.Models.Organiser;
    using Zagorapps.Utilities.Library.Providers;
    using FileIO = System.IO.File;
    using PathIO = System.IO.Path;

    public class UiBindings : CommonBindings
    {
        protected const string KeyApplicationPath = "Path_Application",
            KeyFileExtensionJsonFile = "Name_FileExtensionJson",
            KeyConfigurationFileName = "Name_ConfigurationFile",
            KeyHistoryStore = "Store_History",
            KeySettingsStore = "Store_Settings",
            KeyTasksStore = "Store_Tasks";

        protected override void LoadBindings()
        {
            base.LoadBindings();

            this.BindWindows();
            this.BindViews();
            this.BindFacilitators();
            this.BindCommunications();
            this.BindControls();
            this.BindProviders();
            this.BindLoggers();
        }

        protected virtual void BindFacilitators()
        {
            this.Bind<IFacilitateConnectivityDataProcessing>(container =>
            {
                return new DataProcesingFacilitator();
            }, lifestyle: Lifestyle.Transient);

            this.Bind<IFacilitateOrganiserDataProcessing>(container =>
            {
                return new DataProcesingFacilitator();
            }, lifestyle: Lifestyle.Transient);

            this.Bind<IFacilitateDashboardDataProcessing>(container =>
            {
                return new DataProcesingFacilitator();
            }, lifestyle: Lifestyle.Transient);
        }

        protected virtual void BindCommunications()
        {
            throw new NotImplementedException();
        }

        protected virtual void BindLoggers()
        {
        }

        protected virtual void BindProviders()
        {
            this.Bind<ICommandProvider, CommandProvider>();

            this.Bind<IFileExtensionProvider>((container) =>
            {
                string fileExtensionJson = ConfigurationManager.AppSettings[UiBindings.KeyFileExtensionJsonFile];
                string filePath = PathIO.Combine(this.GetApplicationPath(), fileExtensionJson);

                string data = FileIO.ReadAllText(filePath);

                FileExtensionDatabaseModel result = JsonConvert.DeserializeObject<FileExtensionDatabaseModel>(data);

                return new FileExtensionProvider(result);
            }, lifestyle: Lifestyle.Singleton);

            this.Bind<ITaskHistoryProvider>(container =>
            {
                return new TaskHistoryProvider(
                    ConfigurationManager.AppSettings[UiBindings.KeyHistoryStore],
                    container.GetInstance<IFileManager>(),
                    container.GetInstance<IDirectoryManager>());
            }, lifestyle: Lifestyle.Transient);

            this.Bind<IOrganiserSettingsProvider>(container =>
            {
                return new OrganiserSettingsProvider(
                    ConfigurationManager.AppSettings[UiBindings.KeySettingsStore],
                    container.GetInstance<IFileManager>(),
                    container.GetInstance<IDirectoryManager>());
            }, lifestyle: Lifestyle.Transient);

            this.Bind<ITaskProvider>(container =>
            {
                return new TaskProvider(
                    ConfigurationManager.AppSettings[UiBindings.KeyTasksStore],
                    container.GetInstance<IOrganiserFactory>(),
                    container.GetInstance<IFileManager>(),
                    container.GetInstance<IDirectoryManager>());
            }, lifestyle: Lifestyle.Transient);
        }

        protected override void BindServices()
        {
            base.BindServices();

            this.Bind<IFormsService, FormsService>(lifestyle: Lifestyle.Singleton);
            this.Bind<ISuiteService, SuiteService>(lifestyle: Lifestyle.Singleton);
        }

        protected virtual void BindWindows()
        {
            this.Bind<IMainWindow, MainWindow>();
        }

        protected virtual void BindViews()
        {
            // TODO: bind views
        }

        protected override void BindManagers()
        {
            base.BindManagers();

            this.Bind<ITaskManager>(container =>
            {
                return new SimpleTaskManager(
                    container.GetInstance<IOrganiserFactory>(),
                    container.GetInstance<ITaskProvider>(),
                    container.GetInstance<ITaskHistoryProvider>());
            }, lifestyle: Lifestyle.Singleton);

            this.Bind<IApplicationConfigurationManager>(container =>
            {
                string configName = ConfigurationManager.AppSettings[UiBindings.KeyConfigurationFileName];

                return new ApplicationConfigurationManager(PathIO.Combine(this.GetApplicationPath(), configName));
            }, lifestyle: Lifestyle.Singleton);
        }

        protected virtual void BindControls()
        {
            this.Bind<ISystemTrayControl>(container =>
            {
                ContextMenu menu = App.Current.TryFindResource(App.ControlTrayContextMenu) as ContextMenu;

                return new SystemTrayControl(
                    menu,
                    UiResources.App,
                    App.Name);
            }, Lifestyle.Singleton);
        }

        private string GetApplicationPath()
        {
            return ConfigurationManager.AppSettings[UiBindings.KeyApplicationPath];
        }
    }
}