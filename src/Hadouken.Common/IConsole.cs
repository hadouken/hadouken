namespace Hadouken.Common
{
    public interface IConsole
    {
        void Write(string format, params object[] args);

        void WriteLine(string format, params object[] args);
    }
}
