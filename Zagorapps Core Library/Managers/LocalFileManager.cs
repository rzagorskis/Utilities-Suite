﻿namespace Zagorapps.Core.Library.Managers
{
    using System;
    using System.Collections.Generic;
    using System.IO;

    public class LocalFileManager : IFileManager
    {
        public bool Exists(string filePath)
        {
            return File.Exists(filePath);
        }

        public void Move(string filePath, string movePath)
        {
            File.Move(filePath, movePath);
        }

        public IEnumerable<byte> ReadBytes(string filePath)
        {
            return File.ReadAllBytes(filePath);
        }

        public IEnumerable<string> ReadAllLines(string filePath)
        {
            return File.ReadAllLines(filePath);
        }

        public void Write(string filePath, string contents, bool append = false)
        {
            if (append)
            {
                File.AppendAllText(filePath, contents);
            }
            else
            {
                File.WriteAllText(filePath, contents);
            }
        }

        public string ReadAllText(string filePath)
        {
            return File.ReadAllText(filePath);
        }

        public void Serialize<T>(string filePath, T instance, Action<Stream, T> serializer)
        {
            using (Stream stream = File.Create(filePath))
            {
                serializer(stream, instance);
            }
        }

        public T Read<T>(string filePath, Func<Stream, T> deserializer)
        {
            using (Stream stream = File.OpenRead(filePath))
            {
                return deserializer(stream);
            }
        }

        public void Delete(string filePath)
        {
            File.Delete(filePath);
        }

        public void WriteAllText(string filePath, string contents)
        {
            File.WriteAllText(filePath, contents);
        }

        public void WriteAllBytes(string filePath, byte[] bytes, bool append = false)
        {
            if (append)
            {
                using (FileStream stream = new FileStream(filePath, FileMode.Append))
                {
                    stream.Write(bytes, 0, bytes.Length);
                }
            }
            else
            {
                File.WriteAllBytes(filePath, bytes);
            }
        }
    }
}