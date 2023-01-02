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

        /// <summary>
        /// Default Constructor
        /// </summary>
        public Area() { }

        /// <summary>
        /// Constructor with all attributes
        /// </summary>
        /// <param name="id">ID Attribute<</param>
        /// <param name="name">Name Attribute</param>
        public Area(int id, string name)
        {
            this.id = id;
            this.name = name;
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
    }
}