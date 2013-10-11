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
        }

        public override IMedia Handle(IMedia media)
        {
            var directory = Path.GetDirectoryName(media.Path);
            var fileName = Path.GetFileNameWithoutExtension(media.Path);
            var typeScriptFile = Path.Combine(directory, fileName + ".ts");
            var javaScriptFile = Path.Combine(directory, fileName + ".js");

            var typeScriptDate = _fileSystem.LastWriteTime(typeScriptFile);
            var javaScriptDate = _fileSystem.LastWriteTime(javaScriptFile);

            if (_fileSystem.FileExists(javaScriptFile))
                return media;
            
            if(javaScriptDate == null || (typeScriptDate != null && typeScriptDate >  javaScriptDate))
            {
                return CompileFile(media);
            }

            return media;
        }

        private IMedia CompileFile(IMedia media)
        {
            var compiledPath = _typeScriptCompiler.Compile(media.Path);
            media.Path = compiledPath;

            return media;
        }
    }
}
