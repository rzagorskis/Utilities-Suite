﻿namespace Zagorapps.Utilities.Suite.Library.Providers
{
    using System;
    using System.IO;
    using System.Text;
    using Core.Library.Managers;
    using Tasks;

    public class TaskHistoryProvider : LocalStoreProviderBase, ITaskHistoryProvider
    {
        protected const string FileFormat = "{0}.txt",
            TimeStampFormat = "F";

        public TaskHistoryProvider(string historyPath, IFileManager fileManager, IDirectoryManager directoryManager)
            : base(historyPath, fileManager, directoryManager)
        {
        }

        public void Log(ITask task, LogTaskType logType, string message)
        {
            switch (logType)
            {
                case LogTaskType.Created:
                    this.TaskCreated(task, message);
                    break;
                case LogTaskType.Deleted:
                    this.TaskDeleted(task, message);
                    break;
                case LogTaskType.FailureRaised:
                    this.Failure(task, message);
                    break;
                case LogTaskType.StateChanged:
                    this.StateChanged(task);
                    break;
            }
        }

        public string GetHistory(ITask task)
        {
            string filePath = this.GenerateFilePath(task);

            if (this.FileManager.Exists(filePath))
            {
                return this.FileManager.ReadAllText(filePath);
            }

            return null;
        }

        public string GetStorePath(ITask task)
        {
            return this.GenerateStorePath(task);
        }

        protected void Failure(ITask task, string message)
        {
            this.PerformLoggingAction(
                task,
                t =>
                {
                    StringBuilder builder = new StringBuilder();
                    builder.Append(this.GetCurrentTimeStamp());
                    builder.Append(" -- [ERROR] -> ");

                    if (string.IsNullOrWhiteSpace(message))
                    {
                        message = "There was an error with the task";
                    }

                    builder.Append(message);
                    builder.AppendLine();

                    return builder.ToString();
                });
        }

        protected void StateChanged(ITask task)
        {
            this.PerformLoggingAction(
                task,
                t =>
                {
                    StringBuilder builder = new StringBuilder();
                    builder.Append(this.GetCurrentTimeStamp());
                    builder.Append(" -- ");
                    builder.Append(t.State.ToString());
                    builder.AppendLine();

                    return builder.ToString();
                });
        }

        protected void TaskDeleted(ITask task, string message)
        {
            this.PerformLoggingAction(
                task, 
                t =>
                {
                    StringBuilder builder = new StringBuilder();
                    builder.Append(this.GetCurrentTimeStamp());
                    builder.Append(" --> ");

                    if (string.IsNullOrWhiteSpace(message))
                    {
                        message = "Task was deleted";
                    }

                    builder.Append(message);
                    builder.AppendLine();
                    builder.AppendLine(this.GetLogSection());
                    builder.AppendLine();

                    return builder.ToString();
                });
        }

        protected void TaskCreated(ITask task, string message)
        {
            this.PerformLoggingAction(
                task, 
                t =>
                {
                    StringBuilder builder = new StringBuilder();
                    builder.AppendLine();
                    builder.AppendLine(this.GetLogSection());
                    builder.Append(this.GetCurrentTimeStamp());
                    builder.Append(" --> ");

                    if (string.IsNullOrWhiteSpace(message))
                    {
                        message = "Task was deleted";
                    }

                    builder.Append(message);
                    builder.AppendLine();

                    return builder.ToString();
                });
        }

        protected void PerformLoggingAction(ITask task, Func<ITask, string> contentBuilder)
        {
            string storePath = this.GenerateStorePath(task);

            if (this.DirectoryManager.Exists(storePath, create: true))
            {
                string filePath = this.GenerateFilePath(storePath, string.Format(TaskHistoryProvider.FileFormat, task.Name));

                string contents = contentBuilder(task);

                this.FileManager.Write(filePath, contents, append: true);
            }
        }

        protected string GenerateStorePath(ITask task)
        {
            return Path.Combine(this.BaseDirectory, task.Identity.ToString());
        }

        protected string GenerateFilePath(ITask task)
        {
            return this.GenerateFilePath(this.GenerateStorePath(task), string.Format(TaskHistoryProvider.FileFormat, task.Name));
        }

        protected string GenerateFilePath(string rootPath, string fileName)
        {
            return Path.Combine(rootPath, fileName);
        }

        protected string GetCurrentTimeStamp()
        {
            return DateTime.Now.ToString(TaskHistoryProvider.TimeStampFormat);
        }

        protected string GetLogSection()
        {
            return "=========================================================";
        }
    }
}