using System;

namespace ThorNet {
	public interface IParameter {
		object DefaultValue { get; }
		bool HasDefaultValue { get; }
		string Name { get; }
		Type Type { get; }
	}
}