using System;
using System.IO;
using System.Reflection;

namespace Hadouken.Common.Reflection
{
    public sealed class EmbeddedResource : IEmbeddedResource
    {
        private readonly Assembly _assembly;
        private readonly string _fullName;
        private readonly string _name;

        public EmbeddedResource(Assembly assembly, string fullName, string name)
        {
            if (assembly == null) throw new ArgumentNullException("assembly");
            if (fullName == null) throw new ArgumentNullException("fullName");
            if (name == null) throw new ArgumentNullException("name");

            _assembly = assembly;
            _fullName = fullName;
            _name = name;
        }

        public string FullName
        {
            get { return _fullName; }
        }

        public string Name
        {
            get { return _name; }
        }

        public Stream OpenRead()
        {
            return _assembly.GetManifestResourceStream(_fullName);
        }
    }
}
