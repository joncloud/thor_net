using System;
using System.Collections.Generic;

namespace ThorNet {
	
	public class OptionSubstitutor {
		public void Substitute(
			IThor host,
			Dictionary<string, MethodOption> options, 
			List<string> textArgs) {
		   
			int i = textArgs.Count;
			while (--i >= 0) {
				string textArg = textArgs[i];
				MethodOption option;
				if (TrySubstituteOption(options, textArg, out option)) {
					textArgs.RemoveAt(i);
					host.AddOption(option.Alias, option.Value);
				}
			}
			
		}
		
		private bool TrySubstituteOption(
			Dictionary<string, MethodOption> options, 
			string text,
			out MethodOption option) {

			option = null;
			if (text.Length > 0 && text[0] == '-') {
				string alias;
				string textValue;
				
				int position;
				int offset = 0;
				if (text.Length > 2 && text[1] == '-') {
					position = text.IndexOf("=");
					offset = 1;
				}
				else {
					position = 2;
				}
				
				if (position > 0) {
					alias = text.Substring(0, position);
					textValue = text.Substring(position + offset, text.Length - position - offset);
				}
				else { 
					alias = null;
					textValue = null; 
				}
				
				if (alias != null) {
					if (options.TryGetValue(alias, out option)) {
						option.Value = textValue;
						return true; 
					}
					else { 
						return false;
					}
				}
			}
			
			return false;
		}
	}
}
