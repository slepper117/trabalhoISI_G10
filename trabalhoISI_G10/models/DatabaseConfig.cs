namespace trabalhoISI_G10.models
{
    /// <summary>
    /// Database Configuration Model
    /// </summary>
    public static class DatabaseConfig
    {
        // Attributes
        static string host;
        static string database;
        static string username;
        static string password;

        /// <summary>
        /// Default Constructor
        /// </summary>
        static DatabaseConfig()
        {
            host = "localhost";
            database = "database";
            username = "username";
            password = "password";
        }

        /// <summary>
        /// Property Host Attribute
        /// </summary>
        public static string Host 
        { 
            set => host = value; 
        }

        /// <summary>
        /// Property Database Attribute
        /// </summary>
        public static string Database 
        { 
            set => database = value; 
        }

        /// <summary>
        /// Property Username Attribute
        /// </summary>
        public static string Username 
        { 
            set => username = value; 
        }

        /// <summary>
        /// Property Password Attribute
        /// </summary>
        public static string Password 
        { 
            set => password = value; 
        }


        /// <summary>
        /// Function that returns an Connection String
        /// </summary>
        /// <returns></returns>
        public static String ConnectionString()
        {
            return $"Host={host};Database={database};Username={username};Password={password}";
        }

    }
}
