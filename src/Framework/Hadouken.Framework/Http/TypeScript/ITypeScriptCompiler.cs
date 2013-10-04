namespace Hadouken.Framework.Http.TypeScript
{
    public interface ITypeScriptCompiler
    {
        /// <summary>
        /// Compiles the TypeScript file and returns the path to the compiled file.
        /// </summary>
        /// <param name="file">The TypeScript file to compile.</param>
        /// <returns>The path to the compiled TypeScript file.</returns>
        string Compile(string file);
    }
}
