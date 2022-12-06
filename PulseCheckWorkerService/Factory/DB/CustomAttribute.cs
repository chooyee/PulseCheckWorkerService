namespace Factory.DB
{
    [AttributeUsage(System.AttributeTargets.Property | AttributeTargets.Class | AttributeTargets.Struct)]
    public class TableAttribute:System.Attribute
    {
        public string PropertyName { get; private set; }

        public TableAttribute(string name)
        {
            this.PropertyName = name;
        }
    }
}
