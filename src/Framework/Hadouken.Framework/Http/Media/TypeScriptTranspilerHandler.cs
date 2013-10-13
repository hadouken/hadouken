using Hadouken.Framework.Http.TypeScript;
using Hadouken.Framework.IO;
using System.IO;

namespace Hadouken.Framework.Http.Media
{
    public class TypeScriptTranspilerHandler : BasicMediaTypeHandler
    {
        private readonly IFileSystem _fileSystem;
        private readonly ITypeScriptCompiler _typeScriptCompiler;

        public TypeScriptTranspilerHandler(IFileSystem fileSystem) : this(fileSystem, TypeScriptCompiler.Create()) { }

        public TypeScriptTranspilerHandler(IFileSystem fileSystem, ITypeScriptCompiler typeScriptCompiler) : base(".js")
        {
            _fileSystem = fileSystem;
            _typeScriptCompiler = typeScriptCompiler;

            MediaType = "text/javascript";
        }

        public override IMedia Handle(IMedia media)
        {
            var fileName = Path.GetFileNameWithoutExtension(media.Path);
            var dirName = Path.GetDirectoryName(media.Path);

            if (dirName == null)
                return media;

            var path = Path.Combine(dirName, fileName + ".ts");

            if (_fileSystem.FileExists(path))
                return CompileFile(media, path);

            return media;
        }

        private IMedia CompileFile(IMedia media, string path)
        {
            var compiledPath = _typeScriptCompiler.Compile(path);
            media.Path = compiledPath;

            return media;
        }
    }
}
