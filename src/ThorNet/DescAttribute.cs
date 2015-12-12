using System;

namespace ThorNet {
	[AttributeUsage(AttributeTargets.Method)]
    public class DescAttribute : Attribute {
        public DescAttribute(string example, string description) {
            Description = description;
            Example = example;
        }
        
        public string Description { get; }
        public string Example { get; }
    }
}