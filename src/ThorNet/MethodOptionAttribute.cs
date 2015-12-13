using System;

namespace ThorNet {
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class MethodOptionAttribute : Attribute {
        public MethodOptionAttribute(string name, string alias, string description) {
            Alias = alias;
            Name = name;
            Description = description;
        }
        
        public string Alias { get; }
        public string DefaultValue { get; set; }
        public string Description { get; }
        public string Name { get; }
    }
}