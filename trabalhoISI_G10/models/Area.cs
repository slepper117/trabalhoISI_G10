namespace trabalhoISI_G10.models
{
    /// <summary>
    /// 
    /// </summary>
    public class Area
    {
        private int id;
        private string name;

        /// <summary>
        /// 
        /// </summary>
        public Area() 
        { 
            name = string.Empty;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="name"></param>
        public Area(int id, string name)
        {
            this.id = id;
            this.name = name;
        }

        /// <summary>
        /// 
        /// </summary>
        public int Id {
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
        public string Name { 
            get => name; 
            set => name = value; 
        }
    }
}