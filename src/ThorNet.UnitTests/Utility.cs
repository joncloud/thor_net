using System;

namespace ThorNet.UnitTests {
	public static class Utility {
		public static string[] ToArray(string list) {
			return list.Split(new [] { ',' }, StringSplitOptions.RemoveEmptyEntries);
		}
	}
}
