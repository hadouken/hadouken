using Hadouken.Framework.Http.TypeScript;
using Hadouken.Framework.IO;

namespace Hadouken.Framework.Http.Media
{
    public class TypeScriptTranspilerTypeHandler : BasicMediaTypeHandler
    {
        private readonly IFileSystem _fileSystem;
        private readonly ITypeScriptCompiler _typeScriptCompiler;

        public TypeScriptTranspilerTypeHandler(IFileSystem fileSystem) : this(fileSystem, TypeScriptCompiler.Create()) { }

        public TypeScriptTranspilerTypeHandler(IFileSystem fileSystem, ITypeScriptCompiler typeScriptCompiler) : base(".js")
        {
            _fileSystem = fileSystem;
            _typeScriptCompiler = typeScriptCompiler;
        }

        public override IMedia Handle(IMedia media)
        {
            if (_fileSystem.FileExists(media.Path))
                return media;

            var compiledPath = _typeScriptCompiler.Compile(media.Path);

            media.Path = compiledPath;
            return media;
        }
    }
}
