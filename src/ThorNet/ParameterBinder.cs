using System;
using System.Collections.Generic;

namespace ThorNet
{
    internal class ParameterBinder
    {
        public IEnumerable<BindingResult> Bind(List<string> textArgs, IParameter[] parameters, out object[] args)
        {
            args = new object[parameters.Length];
            var results = new List<BindingResult>();
            int arrayIndex = 0;
            int parameterIndex = 0;
            for (int argIndex = 0; argIndex < textArgs.Count; argIndex++)
            {
                string textArg = textArgs[argIndex];
                IParameter parameter = parameters[parameterIndex];

                if (parameter.Type.IsArray)
                {
                    if (parameterIndex == parameters.Length - 1)
                    {
                        Type elementType = parameter.Type.GetElementType();
                        Array array = (Array)args[parameterIndex]
                            ?? Array.CreateInstance(elementType, textArgs.Count - parameterIndex);

                        try
                        {
                            array.SetValue(
                                TypeHelper.Convert(textArg, elementType),
                                arrayIndex++);
                        }
                        catch (FormatException)
                        {
                            results.Add(new BindingResult(parameter.Name, BindingResultType.InvalidFormat));
                        }

                        args[parameterIndex] = array;
                    }
                    else
                    {
                        results.Add(new BindingResult(parameter.Name, BindingResultType.InvalidArrayPosition));
                    }
                }
                else
                {
                    try
                    {
                        args[argIndex] = TypeHelper.Convert(textArg, parameter.Type);
                    }
                    catch (FormatException)
                    {
                        results.Add(new BindingResult(parameter.Name, BindingResultType.InvalidFormat));
                    }
                    parameterIndex++;
                }
            }

            // Account for optional arguments.
            if (textArgs.Count < args.Length)
            {
                for (int i = 0; i < args.Length; i++)
                {
                    object arg = args[i];
                    if (arg == null)
                    {
                        IParameter parameter = parameters[i];
                        if (parameter.HasDefaultValue)
                        {
                            args[i] = parameter.DefaultValue;
                        }
                        else
                        {
                            results.Add(new BindingResult(parameter.Name, BindingResultType.Missing));
                        }
                    }
                }
            }

            return results;
        }
    }
}
