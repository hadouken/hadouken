namespace Hadouken.Startup
{
    public interface IStartupTask
    {
        void Execute(string[] args);
    }
}
