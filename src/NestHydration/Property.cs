namespace NestHydration
{
    /// <summary>
    /// A single Property
    /// </summary>
    public class Property : IProperty
    {
        public Property(string name) : this(name, name, "STRING")
        {
            
        }

        public Property(string name, string column) : this(name, column, "STRING")
        {

        }

        public Property(string name, string column, string type, bool isId = false)
        {
            Name = name;
            Column = column;
            Type = type;
            IsId = isId;
        }

        /// <summary>
        /// Name of the Property
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Name of the Column
        /// </summary>
        public string Column { get; set; }

        /// <summary>
        /// Type of the Property
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// Boolean which highlights that the current Property
        /// is an Id - the Id is used to identify one-to-many
        /// </summary>
        public bool IsId { get; set; }
    }
}