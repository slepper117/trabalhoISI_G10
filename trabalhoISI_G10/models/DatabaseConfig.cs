using Npgsql;

namespace trabalhoISI_G10.models
{
    /// <summary>
    /// Configuração da Ligação há base de dados
    /// </summary>
    public static class DatabaseConfig
    {
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
        /// 
        /// </summary>
        public static string Host 
        { 
            set => host = value; 
        }

        /// <summary>
        /// 
        /// </summary>
        public static string Database 
        { 
            set => database = value; 
        }

        /// <summary>
        /// 
        /// </summary>
        public static string Username 
        { 
            set => username = value; 
        }

        /// <summary>
        /// 
        /// </summary>
        public static string Password 
        { 
            set => password = value; 
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static NpgsqlDataSource Create() 
        {
            return NpgsqlDataSource.Create($"Host={host};Database={database};Username={username};Password={password}");
        }
    }
}
