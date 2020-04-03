using System.Reflection;
using System.Linq;

namespace Web.API.Authorization
{
    public class ControllerDetails
    {
        public string Controller { get; set; }
        public string Action { get; set; }
        // public string ReturnType { get; set; }
        public string Method { get; set; }

        private IOrderedEnumerable<ControllerDetails> GetControllerDetails<T>()
        {
            Assembly asm = Assembly.GetExecutingAssembly();
            return asm.GetTypes()
                .Where(type => typeof(T).IsAssignableFrom(type))
                .SelectMany(type => type.GetMethods(BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.Public))
                .Where(m => !m.GetCustomAttributes(typeof(System.Runtime.CompilerServices.CompilerGeneratedAttribute), true).Any())
                .Select(x => new ControllerDetails
                {
                    Controller = x.DeclaringType.Name,
                    Action = x.Name,
                    // ReturnType = x.ReturnType.Name,
                    Method = x.GetCustomAttributes().Select(a => a.GetType().Name.Replace("Attribute", "")).Where(str => str.Contains("Http")).FirstOrDefault()
                })
                .OrderBy(x => x.Controller).ThenBy(x => x.Action);
        }
    }
}