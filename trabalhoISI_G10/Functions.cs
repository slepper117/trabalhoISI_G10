using Npgsql;

namespace trabalhoISI_G10
{
    /// <summary>
    /// Functions Class
    /// </summary>
    public static class Functions
    {
        /// <summary>
        /// Check if an ID exists in a given relation
        /// </summary>
        /// <param name="dataSource"></param>
        /// <param name="relation"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public static async Task<bool> CheckID(NpgsqlDataSource dataSource, string relation, int id)
        {
            try
            {
                await using NpgsqlCommand getClocks = dataSource.CreateCommand($"SELECT id FROM setr.{relation} WHERE id = {id};");
                await using NpgsqlDataReader rdr = await getClocks.ExecuteReaderAsync();
                if (rdr.HasRows) return true;

                return false;
            } catch (Exception)
            {
                throw;
            }
        }
    }
}
