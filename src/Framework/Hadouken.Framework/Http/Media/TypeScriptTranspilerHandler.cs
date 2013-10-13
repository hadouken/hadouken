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

        public TypeScriptTranspilerHandler(IFileSystem fileSystem, ITypeScriptCompiler typeScriptCompiler) : base(fileSystem, ".js")
        {
            _fileSystem = fileSystem;
            _typeScriptCompiler = typeScriptCompiler;

            MediaType = "text/javascript";
        }

        public override HandleResult Handle(IMedia media)
        {
            var fileName = Path.GetFileNameWithoutExtension(media.Path);
            var dirName = Path.GetDirectoryName(media.Path);

            if (dirName == null)
                return new ContentResult(_fileSystem, MediaType, media.Path);

            var path = Path.Combine(dirName, fileName + ".ts");

            if (_fileSystem.FileExists(path))
                return CompileFile(media, path);

            if (!_fileSystem.FileExists(media.Path))
                return new HttpNotFoundResult();

             return new ContentResult(_fileSystem, MediaType, media.Path);;
        }

        private HandleResult CompileFile(IMedia media, string path)
        {
            var compiledPath = _typeScriptCompiler.Compile(path);
            media.Path = compiledPath;

            return new ContentResult(_fileSystem, MediaType, media.Path);
        }
    }
}
