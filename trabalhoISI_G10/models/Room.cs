namespace trabalhoISI_G10.models
{
    /// <summary>
    /// Room Model
    /// </summary>
    public class Room
    {
        // Attributes
        private int id;
        private string name;
        private int bookings;

        /// <summary>
        /// Default Constructor
        /// </summary>
        public Room() { }

        /// <summary>
        /// Constructor with all attributes
        /// </summary>
        /// <param name="id">ID Attribute</param>
        /// <param name="name">Name Attribute</param>
        /// <param name="bookings">Bookings Count Attribute</param>
        public Room(int id, string name, int bookings)
        {
            this.id = id;
            this.name = name;
            this.bookings = bookings;
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
        public string Name { 
            get => name; 
            set => name = value; 
        }

        /// <summary>
        /// Property Bookings Count Attribute
        /// </summary>
        public int Bookings
        {
            get => bookings;
            set => bookings = value;
        }
    }
}