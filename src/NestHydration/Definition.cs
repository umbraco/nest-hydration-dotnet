namespace NestHydration
{
    /// <summary>
    /// Definition for how to structure the data
    /// </summary>
    public class Definition
    {
        /// <summary>
        /// 
        /// </summary>
        public Definition()
        {
            Properties = new Properties();
        }

        /// <summary>
        /// List of Properties <see cref="IProperty"/>
        /// </summary>
        public Properties Properties { get; set; }
    }
}
