using System;

namespace ThorNet {
	public interface IThor {
		void AddOption(string name, string value);
		bool HasOption(string name);
	}
}