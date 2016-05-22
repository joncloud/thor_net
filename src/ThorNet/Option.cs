namespace ThorNet
{
    internal class Option
    {
        public Option(OptionAttribute attribute)
            : this(attribute.Alias, attribute.Name)
        {
            AllowFlag = attribute.Flag;
        }

        public Option(string alias, string name = null)
        {
            Alias = alias;
            Name = name ?? alias;
        }

        public string Alias { get; }
        public bool AllowFlag { get; set; }
        public string Name { get; }
        public string Value { get; set; }
    }
}