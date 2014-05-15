namespace Hadouken.Plugins.Metadata
{
    public sealed class EventHandler
    {
        private readonly string _name;
        private readonly string _target;

        public EventHandler(string name, string target)
        {
            _name = name;
            _target = target;
        }

        public string Name
        {
            get { return _name; }
        }

        public string Target
        {
            get { return _target; }
        }
    }
}
