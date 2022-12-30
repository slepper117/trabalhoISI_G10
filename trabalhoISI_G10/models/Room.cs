namespace trabalhoISI_G10.models
{
    /// <summary>
    /// 
    /// </summary>
    public class Room
    {
        private int id;
        private string name;
        private int bookings;

        /// <summary>
        /// 
        /// </summary>
        public Room() 
        {
            name = string.Empty;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="name"></param>
        /// <param name="bookings"></param>
        public Room(int id, string name, int bookings)
        {
            this.id = id;
            this.name = name;
            this.bookings = bookings;
        }

        /// <summary>
        /// 
        /// </summary>
        public int Id
        {
            get => id;
            set => id = value;
        }

        /// <summary>
        /// 
        /// </summary>
        public string Name { 
            get => name; 
            set => name = value; 
        }

        /// <summary>
        /// 
        /// </summary>
        public int Bookings
        {
            get => bookings;
            set => bookings = value;
        }
    }
}