using System;
using System.Collections.Generic;

namespace ThorNet
{
    /// <summary>
    /// Indicates that parameters were missing during binding.
    /// </summary>
    internal class MissingParameterException : Exception
    {
        public MissingParameterException(string[] missingParameters, string message)
            : base(message)
        {
            MissingParameters = missingParameters;
        }

        public MissingParameterException(string[] missingParameters, string message, Exception innerException)
            : base(message, innerException)
        {
            MissingParameters = missingParameters;
        }

        public IReadOnlyList<string> MissingParameters { get; }
    }
}
