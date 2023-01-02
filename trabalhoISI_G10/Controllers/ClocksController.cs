using Microsoft.AspNetCore.Mvc;
using Npgsql;
using System.Data;
using trabalhoISI_G10.models;
using static trabalhoISI_G10.Functions;

namespace trabalhoISI_G10.Controllers
{
    /// <summary>
    /// Clocks Controller
    /// </summary>
    [ApiController]
    [Route("clocks")]
    public class ClocksController : ControllerBase
    {
        /// <summary>
        /// Lists all Clocks
        /// </summary>
        /// <returns>An array of Clocks</returns>
        [HttpGet]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]

        public async Task<ActionResult<List<Clock>>> GetClocks()
        {
            try
            {
                // Initialize List and Datasource
                List<Clock> clocks = new();
                await using NpgsqlDataSource dataSource = NpgsqlDataSource.Create(DatabaseConfig.ConnectionString());

                // Fetch all Clocks
                string query = $"SELECT u.id, u.name, c.id, direction, datetime, l.id, l.name FROM setr.clocks c JOIN setr.users u ON c.id_user = u.id JOIN setr.users l ON c.log = l.id;";
                await using NpgsqlCommand list = dataSource.CreateCommand(query);
                await using NpgsqlDataReader rdr = await list.ExecuteReaderAsync();

                // Check if Clocks exits
                if (!rdr.HasRows) return NotFound("No Clocks were found with the given parameters");

                // Construct List
                while (await rdr.ReadAsync())
                {
                    User user = new(rdr.GetInt32(0), rdr.GetString(1));
                    User log = new(rdr.GetInt32(5), rdr.GetString(6));
                    clocks.Add(new Clock(rdr.GetInt32(2), user, rdr.GetString(3), rdr.GetDateTime(4), log));
                }

                return Ok(clocks);

            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        /// <summary>
        /// Creates a Clock
        /// </summary>
        /// <param name="newClock">Object with the Clock properties</param>
        /// <returns>A newly created Clock</returns>
        [HttpPost]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]

        public async Task<ActionResult<Clock>> CreateClock([FromBody] Clock newClock)
        {
            try
            {
                // Initialize Datasource
                await using NpgsqlDataSource dataSource = NpgsqlDataSource.Create(DatabaseConfig.ConnectionString());

                // Check if the User ID exists, and builds User Object
                string getUserQuery = $"SELECT id, name FROM setr.users WHERE id = {newClock.User.Id}";
                await using NpgsqlCommand getUser = dataSource.CreateCommand(getUserQuery);
                await using NpgsqlDataReader rdrUser = await getUser.ExecuteReaderAsync();
                if (!rdrUser.HasRows) return NotFound("No User was found with the provided ID");
                await rdrUser.ReadAsync();
                User user = new(rdrUser.GetInt32(0), rdrUser.GetString(1));

                // Validates Direction
                string[] directions = { "in", "out" };
                if (newClock.Direction != directions[0] || newClock.Direction != directions[1]) return BadRequest("Property direction must be 'in' or 'out'");

                // Get User to Log

                // Insert Clock
                string query = $"INSERT INTO setr.clocks(id_user, direction, datetime, log) VALUES ({newClock.User.Id}, '{newClock.Direction}', '{newClock.Datetime:s}', 1) RETURNING *;";
                await using NpgsqlCommand insert = dataSource.CreateCommand(query);
                await using NpgsqlDataReader rdr = await insert.ExecuteReaderAsync();
                await rdr.ReadAsync();
                Clock clock = new(rdr.GetInt32(0), user, rdr.GetString(2), rdr.GetDateTime(3), user);

                return Ok(clock);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        /// <summary>
        /// Fetchs a Clock, by ID
        /// </summary>
        /// <param name="id">ID of the Clock</param>
        /// <returns>The fetched Clock</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]

        public async Task<ActionResult<Clock>> GetClock(int id)
        {
            try
            {
                // Initialize Datasource
                await using NpgsqlDataSource dataSource = NpgsqlDataSource.Create(DatabaseConfig.ConnectionString());

                // Get Clock
                string query = $"SELECT u.id, u.name, c.id, direction, datetime, l.id, l.name FROM setr.clocks c JOIN setr.users u ON c.id_user = u.id JOIN setr.users l ON c.log = l.id WHERE c.id = {id};";
                await using NpgsqlCommand get = dataSource.CreateCommand(query);
                await using NpgsqlDataReader rdr = await get.ExecuteReaderAsync();

                // Check if the ID exists
                if (!rdr.HasRows) return NotFound("No Clock was found with the provided ID");

                // Construct Object
                await rdr.ReadAsync();
                User user = new(rdr.GetInt32(0), rdr.GetString(1));
                User log = new(rdr.GetInt32(5), rdr.GetString(6));
                Clock clock = new(rdr.GetInt32(2), user, rdr.GetString(3), rdr.GetDateTime(4), log);

                return Ok(clock);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        /// <summary>
        /// Updates a clock, by ID.
        /// </summary>
        /// <param name="id">ID of the clock</param>
        /// <param name="updateClock">Object with the Clock properties to update</param>
        /// <returns>The updated Clock</returns>
        [HttpPut("{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]

        public async Task<ActionResult<Clock>> UpdateClock(int id, [FromBody] Clock updateClock)
        {
            try
            {
                // Initialize Datasource
                await using NpgsqlDataSource dataSource = NpgsqlDataSource.Create(DatabaseConfig.ConnectionString());

                // Get Clock
                string getClockQuery = $"SELECT * FROM setr.clocks WHERE id = {id}";
                await using NpgsqlCommand getBooking = dataSource.CreateCommand(getClockQuery);
                await using NpgsqlDataReader rdrGetClock = await getBooking.ExecuteReaderAsync();

                // Check if the ID exists
                if (!rdrGetClock.HasRows) return NotFound("No Clock was found with the provided ID");

                // Get Values
                await rdrGetClock.ReadAsync();
                int idUser = rdrGetClock.GetInt32(1);
                string direction = rdrGetClock.GetString(2);
                DateTime datetime = rdrGetClock.GetDateTime(3);
                int idLog = rdrGetClock.GetInt32(4);

                // Validate User
                if (updateClock.User.Id != idUser)
                {
                    bool checkUser = await CheckID(dataSource, "users", updateClock.User.Id);
                    if (!checkUser) return NotFound("No User was found with the provided ID");

                    // Set new Value
                    idUser = updateClock.User.Id;
                }

                // Validate Direction
                if (updateClock.Direction != direction)
                {
                    string[] directions = { "in", "out" };
                    if (updateClock.Direction != directions[0] || updateClock.Direction != directions[1]) return BadRequest("Property direction must be 'in' or 'out'");
                }

                // Validate Log

                // Updates Clock with new values
                string updateQuery = $"UPDATE setr.clocks SET (id_user, direction, datetime, log) = ({idUser}, '{direction}', '{datetime}', {idLog}) WHERE id = {id}";
                await using NpgsqlCommand update = dataSource.CreateCommand(updateQuery);
                await update.ExecuteNonQueryAsync();

                // Get updated Clock
                string query = $"SELECT u.id, u.name, c.id, direction, datetime, l.id, l.name FROM setr.clocks c JOIN setr.users u ON c.id_user = u.id JOIN setr.users l ON c.log = l.id WHERE c.id = {id};";
                await using NpgsqlCommand get = dataSource.CreateCommand(query);
                await using NpgsqlDataReader rdr = await get.ExecuteReaderAsync();

                // Construct Object
                await rdr.ReadAsync();
                User user = new(rdr.GetInt32(0), rdr.GetString(1));
                User log = new(rdr.GetInt32(5), rdr.GetString(6));
                Clock clock = new(rdr.GetInt32(2), user, rdr.GetString(3), rdr.GetDateTime(4), log);

                return Ok(clock);

            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        /// <summary>
        /// Deletes a Clock, by ID
        /// </summary>
        /// <param name="id">ID of the Clock</param>
        /// <returns>The deleted Clock</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]

        public async Task<ActionResult<Clock>> DeleteClock(int id)
        {
            try
            {
                // Initialize Datasource
                await using NpgsqlDataSource dataSource = NpgsqlDataSource.Create(DatabaseConfig.ConnectionString());

                // Get Clock
                string query = $"SELECT u.id, u.name, c.id, direction, datetime, l.id, l.name FROM setr.clocks c JOIN setr.users u ON c.id_user = u.id JOIN setr.users l ON c.log = l.id WHERE c.id = {id};";
                await using NpgsqlCommand get = dataSource.CreateCommand(query);
                await using NpgsqlDataReader rdr = await get.ExecuteReaderAsync();

                // Check if the ID exists
                if (!rdr.HasRows) return NotFound("No Clock was found with the provided ID");

                // Deletes Clock
                string deleteQuery = $"DELETE FROM setr.clocks WHERE id = {id}";
                await using NpgsqlCommand delete = dataSource.CreateCommand(deleteQuery);
                await delete.ExecuteNonQueryAsync();

                // Construct Object
                await rdr.ReadAsync();
                User user = new(rdr.GetInt32(0), rdr.GetString(1));
                User log = new(rdr.GetInt32(5), rdr.GetString(6));
                Clock clock = new(rdr.GetInt32(2), user, rdr.GetString(3), rdr.GetDateTime(4), log);

                return Ok(clock);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}
