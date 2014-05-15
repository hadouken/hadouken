namespace Hadouken.PluginHostProcess
{
    public static class ReflectionExtensions
    {
        public static T GetPropertyValue<T>(this object obj, string propertyName)
        {
            return (T) obj.GetType().GetProperty(propertyName).GetValue(obj);
        }

        public static object Invoke(this object obj, string methodName)
        {
            return obj.Invoke(methodName, null);
        }

        public static object Invoke(this object obj, string methodName, object[] args)
        {
            return obj.GetType().GetMethod(methodName).Invoke(obj, args);
        }
    }
}