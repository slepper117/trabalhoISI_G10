namespace trabalhoISI_G10.DTO
{
    /// <summary>
    /// User Login DTO
    /// </summary>
    public class UserLogin
    {
        // Attributes
        private string username;
        private string password;

        /// <summary>
        /// Default Constructor
        /// </summary>
        public UserLogin() { }

        /// <summary>
        /// Constructor with all attributes
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        public UserLogin(string username, string password)
        {
            this.username = username;
            this.password = password;
        }

        /// <summary>
        /// Property Username Attribute
        /// </summary>
        public string Username
        {
            get => username; 
            set => username = value; 
        }

        /// <summary>
        /// Property Password Attribute
        /// </summary>
        public string Password
        {
            get => password;
            set => password = value;
        }
    }
}
