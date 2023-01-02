namespace trabalhoISI_G10.models
{
    /// <summary>
    /// User Model
    /// </summary>
    public class User
    {
        private int id;
        private string name;

        /// <summary>
        /// Default Constructor
        /// </summary>
        public User() { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="name"></param>
        public User(int id, string name)
        {
            this.id = id;
            this.name = name;
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
        public string Name
        {
            get => name;
            set => name = value;
        }
    }
}
