using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Win32;

namespace WpfApp1
{
    public class FileTypeUtility
    {
        Dictionary<string, string> _fileTypes = new Dictionary<string, string>();

        public FileTypeUtility()
        {

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
                    var typeKeyName = extKey.GetValue(null, String.Empty);

                    if (typeKeyName != null)
                    {
                        using (var typeKey = Registry.ClassesRoot.OpenSubKey(typeKeyName.ToString()))
                        {
                            if (typeKey != null)
                            {
                                var typeName = typeKey.GetValue(null, String.Empty).ToString();

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
