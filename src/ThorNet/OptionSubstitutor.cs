using System;
using System.Collections.Generic;

namespace ThorNet
{

    internal class OptionSubstitutor
    {
        public void Substitute(
            IThor host,
            Dictionary<string, Option> options,
            List<string> textArgs)
        {

            int i = textArgs.Count;
            while (--i >= 0)
            {
                string textArg = textArgs[i];
                Option option;
                if (TrySubstituteOption(options, textArg, out option))
                {
                    textArgs.RemoveAt(i);
                    host.AddOption(option.Name, option.Value);
                }
            }

        }

        private bool TrySubstituteOption(
            Dictionary<string, Option> options,
            string text,
            out Option option)
        {

            option = null;
            if (text.Length > 0 && text[0] == '-')
            {
                string alias;
                string textValue;

                bool isFlag = false;
                int position;
                int offset = 0;
                if (text.Length > 2 && text[1] == '-')
                {
                    position = text.IndexOf("=");

                    // Handle flags.
                    if (position == -1)
                    {
                        position = text.Length;
                        offset = 0;
                        isFlag = true;
                    }
                    else
                    {
                        offset = 1;
                    }
                }
                else
                {
                    position = 2;
                }

                if (position > 0)
                {
                    alias = text.Substring(0, position);
                    textValue = text.Substring(position + offset, text.Length - position - offset);
                }
                else
                {
                    alias = null;
                    textValue = null;
                }

                if (alias != null)
                {
                    if (options.TryGetValue(alias, out option))
                    {
                        // Determine if flags are allowed
                        if (isFlag && !option.AllowFlag)
                        {
                            option = null;
                            return false;
                        }

                        if (option.ShouldUseValue(textValue))
                        {
                            option.Value = textValue;
                            return true;
                        }
                    }
                }
            }

            return false;
        }
    }
}
