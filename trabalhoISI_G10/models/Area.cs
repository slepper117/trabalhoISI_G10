namespace trabalhoISI_G10.models
{
    /// <summary>
    /// Area Model
    /// </summary>
    public class Area
    {
        // Attributes
        private int id;
        private string name;
        private List<User> users;

        /// <summary>
        /// Default Constructor
        /// </summary>
        public Area() 
        {
            users = new List<User>();
        }

        /// <summary>
        /// Constructor with all attributes
        /// </summary>
        /// <param name="id">ID Attribute</param>
        /// <param name="name">Name Attribute</param>
        public Area(int id, string name)
        {
            this.id = id;
            this.name = name;
            users = new List<User>();
        }

        /// <summary>
        /// Property ID Attribute
        /// </summary>
        public int Id {
            get => id;
            set
            {
                if (id > 0) 
                    id = value;
            }
        }

        /// <summary>
        /// Property Name Attribute
        /// </summary>
        public string Name { 
            get => name; 
            set => name = value; 
        }

        /// <summary>
        /// Property List Users
        /// </summary>
        public List<User> Users
        {
            get => users;
            set => users = value;
        }
    }
}