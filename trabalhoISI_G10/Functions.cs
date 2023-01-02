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
        /// <param name="dataSource">DataSource to make the query</param>
        /// <param name="relation">Table were to check</param>
        /// <param name="value">ID of check if exists</param>
        /// <returns>True if exists, False if not</returns>
        public static async Task<bool> CheckID(NpgsqlDataSource dataSource, string relation, int value)
        {
            try
            {
                await using NpgsqlCommand getClocks = dataSource.CreateCommand($"SELECT id FROM setr.{relation} WHERE id = {value};");
                await using NpgsqlDataReader rdr = await getClocks.ExecuteReaderAsync();
                if (rdr.HasRows) return true;

                return false;
            } catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Function to verify if a two dates overlaps in the same room
        /// </summary>
        /// <param name="dataSource">DataSource to make the query</param>
        /// <param name="room">Room were to perform the verification</param>
        /// <param name="start">Start Datetime</param>
        /// <param name="end">End Datetime</param>
        /// <returns>True if there is no overlapping, False if has a overlapping</returns>
        public static async Task<bool> CheckAvailabilaty(NpgsqlDataSource dataSource, int room, DateTime start, DateTime end)
        {
            try
            {
                string query = $"SELECT * FROM setr.bookings WHERE id_room = {room} AND validated = 'true' AND start <= '{start:s}' AND end >= '{end:s}'";
                await using NpgsqlCommand get = dataSource.CreateCommand(query);
                await using NpgsqlDataReader rdr = await get.ExecuteReaderAsync();
                if (rdr.HasRows) return false;

                return true;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
