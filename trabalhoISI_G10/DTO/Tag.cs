namespace trabalhoISI_G10.DTO
{
    /// <summary>
    /// Tag DTO
    /// </summary>
    public class Tag
    {
        // Attributes
        private string tagID;

        /// <summary>
        /// Default Constructor
        /// </summary>
        public Tag() { }

        /// <summary>
        /// Constructor with all attributes
        /// </summary>
        /// <param name="tagID">TagID Attributes</param>
        public Tag(string tagID)
        {
            this.tagID = tagID;
        }

        /// <summary>
        /// Property TagID Attributes
        /// </summary>
        public string TagID
        {
            get => tagID;
            set => tagID = value; 
        }

    }
}
