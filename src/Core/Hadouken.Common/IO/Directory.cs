﻿using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Hadouken.Common.IO
{
    internal sealed class Directory : IDirectory
    {
        private readonly DirectoryInfo _directory;
        private readonly DirectoryPath _path;

        public DirectoryPath Path
        {
            get { return _path; }
        }

        public bool Exists
        {
            get { return _directory.Exists; }
        }

        public bool Hidden
        {
            get { return _directory.Attributes.HasFlag(FileAttributes.Hidden); }
        }

        public Directory(DirectoryPath path)
        {
            _path = path;
            _directory = new DirectoryInfo(_path.FullPath);
        }

        public void Create()
        {
            _directory.Create();
        }

        public void Delete(bool recursive)
        {
            _directory.Delete(recursive);
        }

        public IEnumerable<IDirectory> GetDirectories(string filter, SearchScope scope)
        {
            var option = scope == SearchScope.Current ? SearchOption.TopDirectoryOnly : SearchOption.AllDirectories;
            return _directory.GetDirectories(filter, option).Select(directory => new Directory(directory.FullName));
        }

        public IEnumerable<IFile> GetFiles(string filter, SearchScope scope)
        {
            var option = scope == SearchScope.Current ? SearchOption.TopDirectoryOnly : SearchOption.AllDirectories;
            return _directory.GetFiles(filter, option).Select(file => new File(file.FullName));
        }

        public IDirectory GetParent()
        {
            if (_directory.Parent == null) return null;
            return new Directory(_directory.Parent.FullName);
        }
    }
}