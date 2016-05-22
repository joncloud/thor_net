namespace ThorNet
{
    internal class BindingResult
    {
        public BindingResult(string name, BindingResultType type)
        {
            Name = name;
            Type = type;
        }

        public string Name { get; }
        public BindingResultType Type { get; }
    }
}
