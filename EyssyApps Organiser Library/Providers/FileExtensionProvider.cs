﻿namespace EyssyApps.Organiser.Library.Providers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using EyssyApps.Organiser.Library.Models.Organiser;

    public class FileExtensionProvider : IFileExtensionProvider
    {
        // TODO: this will only suffice for starters - we don't want to store this kind of data in memory
        protected readonly FileExtensionDatabaseModel Database;

        private readonly Lazy<IEnumerable<FileExtensionMetadata>> Metadatas;

        public FileExtensionProvider(FileExtensionDatabaseModel database)
        {
            this.Database = database;
            this.Metadatas = new Lazy<IEnumerable<FileExtensionMetadata>>(() => this.Database.Categories.SelectMany(c => c.Extensions).ToArray());
        }

        public IEnumerable<FileExtensionMetadata> Get()
        {
            return this.Metadatas.Value;
        }

        public FileExtensionCategory GetCategoryForExtension(string extension)
        {
            return this.Database.Categories.FirstOrDefault(c => c.Extensions.Any(e => e.Value == extension));
        }
    }
}