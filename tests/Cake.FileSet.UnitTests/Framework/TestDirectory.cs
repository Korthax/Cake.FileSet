using System;
using System.Collections.Generic;
using Microsoft.Extensions.FileSystemGlobbing.Abstractions;

namespace Cake.FileSet.UnitTests.Framework
{
    public class TestDirectory : DirectoryInfoBase
    {
        public override DirectoryInfoBase ParentDirectory { get; }
        public override string FullName => _fullName;
        public override string Name => _name;

        private readonly Dictionary<string, TestDirectory> _children;
        private readonly Dictionary<string, FileInfoBase> _files;
        private readonly string _fullName;
        private readonly string _name;

        public static TestDirectory Root(string name, string fullName)
        {
            return new TestDirectory(name, fullName.TrimEnd('/'), null);
        }

        public TestDirectory(string name, string fullName, DirectoryInfoBase parent)
        {
            ParentDirectory = parent;
            _name = name;
            _fullName = fullName;
            _children = new Dictionary<string, TestDirectory>();
            _files = new Dictionary<string, FileInfoBase>();
        }

        public TestDirectory Add(string path)
        {
            return Add(new Queue<string>(path.Split('/')));
        }

        private TestDirectory Add(Queue<string> parts)
        {
            if (parts.Count == 0)
                return this;

            var nextPart = parts.Dequeue().Trim('/');
            var path = $"{_fullName}/{nextPart}";

            if (parts.Count == 0)
            {
                if (!_files.ContainsKey(nextPart))
                    _files.Add(nextPart, new TestFile(nextPart, path, this));

                return this;
            }

            if (!_children.ContainsKey(path))
                _children.Add(path, new TestDirectory(nextPart, path, this));

            _children[path].Add(parts);
            return this;
        }

        public override IEnumerable<FileSystemInfoBase> EnumerateFileSystemInfos()
        {
            var fileSystemInfos = new List<FileSystemInfoBase>();
            fileSystemInfos.AddRange(_children.Values);
            fileSystemInfos.AddRange(_files.Values);
            return fileSystemInfos;
        }

        public override DirectoryInfoBase GetDirectory(string path)
        {
            return _children[path];
        }

        public override FileInfoBase GetFile(string path)
        {
            return _files[path];
        }

        protected bool Equals(TestDirectory other)
        {
            return string.Equals(_name, other._name);
        }

        public override bool Equals(object obj)
        {
            if(ReferenceEquals(null, obj))
                return false;

            if(ReferenceEquals(this, obj))
                return true;

            return obj.GetType() == GetType() && Equals((TestDirectory)obj);
        }

        public override int GetHashCode()
        {
            return _name != null ? _name.GetHashCode() : 0;
        }

        public class TestFile : FileInfoBase
        {
            public override DirectoryInfoBase ParentDirectory { get; }
            public override string FullName { get; }
            public override string Name { get; }

            public TestFile(string name, string fullName, DirectoryInfoBase parentDirectory)
            {
                Name = name;
                FullName = fullName;
                ParentDirectory = parentDirectory;
            }

            protected bool Equals(TestFile other)
            {
                return string.Equals(FullName, other.FullName) && string.Equals(Name, other.Name);
            }

            public override bool Equals(object obj)
            {
                if(ReferenceEquals(null, obj))
                    return false;

                if(ReferenceEquals(this, obj))
                    return true;

                return obj.GetType() == GetType() && Equals((TestFile)obj);
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    return ((FullName != null ? FullName.GetHashCode() : 0) * 397) ^ (Name != null ? Name.GetHashCode() : 0);
                }
            }
        }
    }
}