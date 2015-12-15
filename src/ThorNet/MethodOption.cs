using System;

namespace ThorNet {
	public class MethodOption {
		public MethodOption(string alias) {
			Alias = alias;
		}
		public string Alias { get; }
		public bool AllowFlag { get; set; }
		public string Value { get; set; }
	}
}