namespace trabalhoISI_G10.Config
{
    /// <summary>
    /// 
    /// </summary>
    public static class Jwt
    {
        static string issuer;
        static string audience;
        static string secret;

        static Jwt()
        {
            issuer = string.Empty;
            audience = string.Empty;
            secret = string.Empty;
        }

        /// <summary>
        /// 
        /// </summary>
        public static string Issuer
        { 
            get => issuer; 
            set => issuer = value;
        }

        /// <summary>
        /// 
        /// </summary>
        public static string Audience
        { 
            get => audience;
            set => audience = value;
        }

        /// <summary>
        /// 
        /// </summary>
        public static string Secret
        { 
            get => secret;
            set => secret = value;
        }
    }
}
