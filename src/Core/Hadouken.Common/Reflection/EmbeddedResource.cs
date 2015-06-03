using System;
using System.IO;
using System.Reflection;

namespace Hadouken.Common.Reflection {
    public sealed class EmbeddedResource : IEmbeddedResource {
        private readonly Assembly _assembly;
        private readonly string _fullName;
        private readonly string _name;

        public EmbeddedResource(Assembly assembly, string fullName, string name) {
            if (assembly == null) {
                throw new ArgumentNullException("assembly");
            }
            if (fullName == null) {
                throw new ArgumentNullException("fullName");
            }
            if (name == null) {
                throw new ArgumentNullException("name");
            }

            this._assembly = assembly;
            this._fullName = fullName;
            this._name = name;
        }

        public string FullName {
            get { return this._fullName; }
        }

        public string Name {
            get { return this._name; }
        }

        public Stream OpenRead() {
            return this._assembly.GetManifestResourceStream(this._fullName);
        }
    }
}