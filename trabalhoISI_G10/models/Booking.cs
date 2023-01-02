using System.ComponentModel;

namespace trabalhoISI_G10.models
{
    /// <summary>
    /// Booking Model
    /// </summary>
    public class Booking
    {
        // Attributes
        private int id;
        private User user;
        private Room room;
        private DateTime start;
        private DateTime end;
        private string description;
        private bool validated;

        /// <summary>
        /// Default Constructor
        /// </summary>
        public Booking() { }

        /// <summary>
        /// Constructor with all attributes
        /// </summary>
        /// <param name="id">ID Attribute</param>
        /// <param name="user">User Object Attribute</param>
        /// <param name="room">Room Object Attribute</param>
        /// <param name="start">Start Datetime Attribute</param>
        /// <param name="end">End Datetime Attribute</param>
        /// <param name="description">Description Attribute</param>
        /// <param name="validated">Vaidated Attribute</param>
        public Booking(int id, User user, Room room, DateTime start, DateTime end, string description, bool validated)
        {
            this.id = id;
            this.user = user;
            this.room = room;
            this.start = start;
            this.end = end;
            this.description = description;
            this.validated = validated;
        }

        /// <summary>
        /// Property ID Attribute
        /// </summary>
        public int Id
        {
            get => id;
            set {
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
        /// Property Room Object Attribute
        /// </summary>
        public Room Room
        {
            get => room;
            set => room = value;
        }

        /// <summary>
        /// Property Start Datetime Attribute
        /// </summary>
        public DateTime Start
        {
            get => start; 
            set => start = value;
        }

        /// <summary>
        /// Property End Datetime Attribute
        /// </summary>
        public DateTime End
        {
            get => end; 
            set => end = value;
        }

        /// <summary>
        /// Property Description Attribute
        /// </summary>
        [DefaultValue("Descritivo da Reserva")]
        public string Description
        {
            get => description; 
            set => description = value;
        }

        /// <summary>
        /// Property Validated Attribute
        /// </summary>
        [DefaultValue(false)]
        public bool Validated
        {
            get => validated; 
            set => validated = value;
        }
    }
}