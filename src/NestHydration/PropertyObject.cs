namespace NestHydration
{
    /// <summary>
    /// A Property Object
    /// </summary>
    public class PropertyObject : IProperty
    {
        public PropertyObject(string name, Properties properties)
        {
            Name = name;
            Properties = properties;
        }

        /// <summary>
        /// Name of the Property
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// List of Properties <see cref="IProperty"/>
        /// </summary>
        public Properties Properties { get; set; }
    }
}