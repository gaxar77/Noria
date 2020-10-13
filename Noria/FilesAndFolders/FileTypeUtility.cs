﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Markup;
using Microsoft.Win32;

namespace Noria.FilesAndFolders
{
    public class FileSystemPath
    {
        public string Path { get; private set; }
        public FileSystemPathComponent[] PathComponents { get; private set; }
        public FileSystemPath(string path)
        {
            Path = path;
            PathComponents = GetPathComponents(path);
        }

        private FileSystemPathComponent[] GetPathComponents(string path)
        {
            var pathComponents = new List<FileSystemPathComponent>();

            while (path != null)
            {
                var fileSystemObjectName = System.IO.Path.GetFileName(path);
                if (fileSystemObjectName == String.Empty)
                    fileSystemObjectName = path;

                var pathComponent = new FileSystemPathComponent(fileSystemObjectName, path);

                path = System.IO.Path.GetDirectoryName(path);

                pathComponents.Add(pathComponent);
            }

            pathComponents.Reverse();

            return pathComponents.ToArray();
        }
    }
    public class FileSystemPathComponent
    {
        public string FileSystemObjectName { get; private set; }
        public string Path { get; private set; }
        public FileSystemPathComponent(string fileSystemObjectName, string path)
        {
            FileSystemObjectName = fileSystemObjectName;
            Path = path;
        }
    }
    public class FileTypeUtility
    {
        Dictionary<string, string> _fileTypes = new Dictionary<string, string>();

        public FileTypeUtility()
        {

        }

        public void Reset()
        {
            _fileTypes.Clear();
        }

        public string GetFileType(string fileName)
        {
            var extension = Path.GetExtension(fileName);
            if (String.IsNullOrEmpty(extension))
            {
                return String.Empty;
            }

            if (_fileTypes.ContainsKey(extension))
            {
                return _fileTypes[extension];
            }

            using (var extKey = Registry.ClassesRoot.OpenSubKey(extension))
            {
                if (extKey != null)
                {
                    var typeKeyName = (string)extKey.GetValue(null, String.Empty);

                    if (typeKeyName != String.Empty)
                    {
                        using (var typeKey = Registry.ClassesRoot.OpenSubKey(typeKeyName.ToString()))
                        {
                            if (typeKey != null)
                            {
                                var typeName = (string)typeKey.GetValue(null, String.Empty);

                                _fileTypes[extension] = typeName;
                                return typeName;
                            }
                        }
                    }
                }
            }

            _fileTypes[extension] = String.Empty;
            
            return String.Empty;
        }
    }
}