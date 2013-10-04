namespace Hadouken.Framework.Http.TypeScript
{
    public interface ITypeScriptCompiler
    {
        /// <summary>
        /// Compiles the TypeScript file and returns the content of the output.
        /// </summary>
        /// <param name="file">The TypeScript file to compile.</param>
        /// <returns>The content of the compiled TypeScript file.</returns>
        string Compile(string file);
    }
}
