using System.Linq;

namespace ThorNet
{
    internal class Option
    {
        readonly string[] _possibleValues;

        public Option(OptionAttribute attribute)
            : this(attribute.Alias, attribute.Name, attribute.GetPossibleValues())
        {
            AllowFlag = attribute.Flag;
        }

        public Option(string alias, string name = null, string[] possibleValues = null)
        {
            Alias = alias;
            Name = name ?? alias;
            _possibleValues = possibleValues;
        }

        public string Alias { get; }
        public bool AllowFlag { get; set; }
        public string Name { get; }
        public string Value { get; set; }

        internal bool ShouldUseValue(string value)
        {
            if (_possibleValues is null) return true;

            return _possibleValues.Contains(value);
        }
    }
}