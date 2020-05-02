namespace NestHydration
{
    /// <summary>
    /// A Property Array - this is basically the same as a Property Object, but the type
    /// infers that its a list of Objects.
    /// </summary>
    public class PropertyArray : IProperty
    {
        public PropertyArray(string name, Properties properties)
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