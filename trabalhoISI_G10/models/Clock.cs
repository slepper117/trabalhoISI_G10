namespace trabalhoISI_G10.models
{
    /// <summary>
    /// Clock Model
    /// </summary>
    public class Clock
    {
        // Attributes
        private int id;
        private User user;
        private string direction;
        private DateTime datetime;
        private User log;

        /// <summary>
        /// Default Constructor
        /// </summary>
        public Clock() { }

        /// <summary>
        /// Constructor with all attributes
        /// </summary>
        /// <param name="id">ID Attribute</param>
        /// <param name="user">User Object Attribute</param>
        /// <param name="direction">Direction Attribute</param>
        /// <param name="datetime">Datetime Attribute</param>
        /// <param name="log">User Object Attribute</param>
        public Clock(int id, User user, string direction, DateTime datetime, User log)
        {
            this.id = id;
            this.user = user;
            this.direction = direction;
            this.datetime = datetime;
            this.log = log;
        }

        /// <summary>
        /// Property ID Attribute
        /// </summary>
        public int Id
        {
            get => id; 
            set 
            {
                if (id > 0) 
                    id = value;
            }
        }

        /// <summary>
        /// Property User Object Attribute
        /// </summary>
        public User User
        {
            get => user;
            set => user = value;
        }

        /// <summary>
        /// Property Direction Attribute
        /// </summary>
        public string Direction
        {
            get=> direction;
            set => direction = value;
        }

        /// <summary>
        /// Property Datetime Attribute
        /// </summary>
        public DateTime Datetime
        {
            get => datetime;
            set => datetime = value;
        }

        /// <summary>
        /// Property User Log Object Attribute
        /// </summary>
        public User Log
        {
            get => log;
            set => log = value;
        }
    }
}
