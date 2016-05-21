using System;
using System.Collections.Generic;

namespace ThorNet
{
    /// <summary>
    /// Indicates that parameters were missing during binding.
    /// </summary>
    internal class MismatchedBindingException : Exception
    {
        public MismatchedBindingException(BindingResult[] bindingResults, string message)
            : base(message)
        {
            BindingResults = bindingResults;
        }

        public MismatchedBindingException(BindingResult[] bindingResults, string message, Exception innerException)
            : base(message, innerException)
        {
            BindingResults = bindingResults;
        }

        public IReadOnlyList<BindingResult> BindingResults { get; }
    }
}
