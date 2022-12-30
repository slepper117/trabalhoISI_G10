namespace trabalhoISI_G10.models
{
    /// <summary>
    /// 
    /// </summary>
    public class Booking
    {
        private int id;
        private int idUser;
        private int idRoom;
        private DateTime start;
        private DateTime end;
        private string description;
        private bool validated;

        /// <summary>
        /// 
        /// </summary>
        public Booking()
        {
            description = string.Empty;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="idUser"></param>
        /// <param name="idRoom"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="description"></param>
        /// <param name="validated"></param>
        public Booking(int id, int idUser, int idRoom, DateTime start, DateTime end, string description, bool validated)
        {
            this.id = id;
            this.idUser = idUser;
            this.idRoom = idRoom;
            this.start = start;
            this.end = end;
            this.description = description;
            this.validated = validated;
        }

        /// <summary>
        /// 
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
        /// 
        /// </summary>
        public int IdUser
        { 
            get => idUser;
            set {
                if (idUser > 0)
                    idUser = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public int IdRoom
        {
            get => idRoom;
            set {
                if (idRoom > 0)
                    idRoom = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public DateTime Start
        {
            get => start; 
            set => start = value;
        }

        /// <summary>
        /// 
        /// </summary>
        public DateTime End
        {
            get => end; 
            set
            {
                if (end > start) 
                    end = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public string Description
        {
            get => description; 
            set => description = value;
        }

        /// <summary>
        /// 
        /// </summary>
        public bool Validated
        {
            get => validated; 
            set => validated = value;
        }
    }
}