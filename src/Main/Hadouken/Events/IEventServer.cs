using System.Threading.Tasks;

namespace Hadouken.Events
{
    public interface IEventServer
    {
        void Open();

        void Close();
    }
}
