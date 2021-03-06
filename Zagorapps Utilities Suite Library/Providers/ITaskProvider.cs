﻿namespace Zagorapps.Utilities.Suite.Library.Providers
{
    using System;
    using System.Collections.Generic;
    using Tasks;

    public interface ITaskProvider
    {
        IEnumerable<ITask> GetAll();

        ITask Get(Guid identity);

        void Save(ITask task);

        void Delete(ITask task);
    }
}