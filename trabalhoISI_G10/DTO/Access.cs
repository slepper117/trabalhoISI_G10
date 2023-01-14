namespace trabalhoISI_G10.DTO
{
    /// <summary>
    /// Access DTO
    /// </summary>
    public class Access
    {
        // Attributes
        private string status;
        private string name;
        private DateTime datetime;

        /// <summary>
        /// Default Constructor
        /// </summary>
        public Access() { }

        /// <summary>
        /// Constructor with all attributes
        /// </summary>
        /// <param name="status">Status Attribute</param>
        /// <param name="datetime">Datetime Attribute</param>
        public Access(string status, DateTime datetime)
        {
            this.status = status;
            this.datetime = datetime;
        }

        /// <summary>
        /// Constructor with all attributes
        /// </summary>
        /// <param name="status">Status Attribute</param>
        /// <param name="name">Name Attribute</param>
        /// <param name="datetime">Datetime Attribute</param>
        public Access(string status, string name, DateTime datetime)
        {
            this.status = status;
            this.name = name;
            this.datetime = datetime;
        }

        /// <summary>
        /// Property Status Attribute
        /// </summary>
        public string Status
        {
            get => status;
            set => status = value;
        }

        /// <summary>
        /// Property Name Attribute
        /// </summary>
        public string Name
        {
            get => name;
            set => name = value;
        }

        /// <summary>
        /// Property Datetime Attribute
        /// </summary>
        public DateTime DateTime
        {
            get => datetime;
            set => datetime = value;
        }
    }
}
