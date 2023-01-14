namespace trabalhoISI_G10.models
{
    /// <summary>
    /// User Model
    /// </summary>
    public class User
    {
        // Attributes
        private int id;
        private string name;

        /// <summary>
        /// Default Constructor
        /// </summary>
        public User() { }

        /// <summary>
        /// Constructor with all attributes
        /// </summary>
        /// <param name="id"> ID Attribute</param>
        /// <param name="name">Name Attribute</param>
        public User(int id, string name)
        {
            this.id = id;
            this.name = name;
        }

        /// <summary>
        /// Property ID Attribute
        /// </summary>
        public int Id 
        {
            get => id;
            set => id = value;
        }

        /// <summary>
        /// Property Name Attribute
        /// </summary>
        public string Name
        {
            get => name;
            set => name = value;
        }
    }
}
