namespace trabalhoISI_G10.models
{
    /// <summary>
    /// Model of Clock
    /// </summary>
    public class Clock
    {
        private int id;
        private int idUser;
        private string direction;
        private DateTime datetime;
        private int log;

        /// <summary>
        /// Default Constructor
        /// </summary>
        public Clock() 
        {
            direction = String.Empty;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="id"></param>
        /// <param name="idUser"></param>
        /// <param name="direction"></param>
        /// <param name="datetime"></param>
        /// <param name="log"></param>
        public Clock(int id, int idUser, string direction, DateTime datetime, int log)
        {
            this.id = id;
            this.idUser = idUser;
            this.direction = direction;
            this.datetime = datetime;
            this.log = log;
        }

        /// <summary>
        /// 
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
        /// 
        /// </summary>
        public int IdUser
        {
            get => idUser;
            set
            {
                if (idUser > 0) 
                    idUser = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public string Direction
        {
            get=> direction;
            set => direction = value;
        }

        /// <summary>
        /// 
        /// </summary>
        public DateTime DateTime
        {
            get => datetime;
            set => datetime = value;
        }

        /// <summary>
        /// 
        /// </summary>
        public int Log
        {
            get => log; 
            set {
                if (idUser > 0)
                    idUser = value;
            }
        }
    }
}
