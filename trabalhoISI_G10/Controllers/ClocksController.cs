using Microsoft.AspNetCore.Mvc;
using Npgsql;
using trabalhoISI_G10.models;
using static trabalhoISI_G10.Functions;

namespace trabalhoISI_G10.Controllers
{
    /// <summary>
    /// Clocks Controller Class
    /// </summary>
    [ApiController]
    [Route("clocks")]
    public class ClocksController : ControllerBase
    {
        /// <summary>
        /// Lists all Clocks
        /// </summary>
        /// <returns>An Array of objects with tje clocks</returns>
        [HttpGet]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]

        public async Task<ActionResult<List<Clock>>> GetClocks()
        {
            try
            {
                List<Clock> clocks = new();

                await using NpgsqlDataSource dataSource = DatabaseConfig.Create();

                // Fetch all Clocks
                await using NpgsqlCommand list = dataSource.CreateCommand("SELECT * FROM setr.clocks;");
                await using NpgsqlDataReader rdr = await list.ExecuteReaderAsync();

                // Check if there is items
                if (!rdr.HasRows) return NotFound("No Clocks were found with the given parameters");

                // Construct List
                while (await rdr.ReadAsync())
                {
                    clocks.Add(new Clock(rdr.GetInt32(0), rdr.GetInt32(1), rdr.GetString(2), rdr.GetDateTime(3), rdr.GetInt32(4)));
                }

                return Ok(clocks);

            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        /// <summary>
        /// Create a Clock for a given user
        /// </summary>
        /// <param name="newClock">Object with the clock</param>
        /// <returns>A newly created Clock</returns>
        [HttpPost]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]

        public async Task<ActionResult<Clock>> CreateClock([FromBody] Clock newClock)
        {
            try
            {
                await using NpgsqlDataSource dataSource = DatabaseConfig.Create();

                // Check if User exists
                bool checkID = await CheckID(dataSource, "users", newClock.IdUser);
                if (!checkID) return NotFound("User ID dosen't exists");

                // Insert Clock
                string query = $"INSERT INTO setr.clocks(id_user, direction, datetime, log) VALUES ({newClock.IdUser}, '{newClock.Direction}', '{newClock.DateTime:s}', 1) RETURNING *;";
                await using NpgsqlCommand insert = dataSource.CreateCommand(query);
                await using NpgsqlDataReader rdr = await insert.ExecuteReaderAsync();
                await rdr.ReadAsync();
                Clock clock = new(rdr.GetInt32(0), rdr.GetInt32(1), rdr.GetString(2), rdr.GetDateTime(3), rdr.GetInt32(4));

                return Ok(clock);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        /// <summary>
        /// Gets a Clock by ID
        /// </summary>
        /// <param name="id">ID of the Clock</param>
        /// <returns>The Clock fetched</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]

        public async Task<ActionResult<Clock>> GetClock(int id)
        {
            try
            {
                await using NpgsqlDataSource dataSource = DatabaseConfig.Create();

                // Check if the ID exists
                bool checkID = await CheckID(dataSource, "clocks", id);
                if (!checkID) return NotFound("No Clock was found with the ID");

                // Fetch the object
                await using NpgsqlCommand get = dataSource.CreateCommand($"SELECT * FROM setr.clocks WHERE id = {id};");
                await using NpgsqlDataReader rdr = await get.ExecuteReaderAsync();
                await rdr.ReadAsync();
                Clock clock = new(rdr.GetInt32(0), rdr.GetInt32(1), rdr.GetString(2), rdr.GetDateTime(3), rdr.GetInt32(4));

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
        /// <param name="updateClock">Object with props to update</param>
        /// <returns>A uptadated Clock</returns>
        [HttpPut("{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]

        public async Task<ActionResult<Clock>> UpdateClock(int id, [FromBody] Clock updateClock)
        {
            try
            {
                await using NpgsqlDataSource dataSource = DatabaseConfig.Create();

                // Check if the ID exists
                bool checkID = await CheckID(dataSource, "clocks", id);
                if (!checkID) return NotFound("No Clock was found with the ID");

                // If new User ID
                if (updateClock.IdUser > 0)
                {
                    bool checkUserID = await CheckID(dataSource, "clocks", updateClock.IdUser);
                    if (!checkUserID) return NotFound("No Clock was found with the ID");
                }

                // Update Clock
                string query = 
                    $"UPDATE setr.clocks SET " +
                    $"{(updateClock.IdUser > 0 ? $"id_user = {updateClock.IdUser}" : "")}" +
                    $"{(updateClock.Direction.Length != 0 ? $"direction = '{updateClock.Direction}'" : "")}" +
                    $"{(updateClock.DateTime.ToString("s").Length != 0 ? $"datetime = '{updateClock.DateTime:s}'" : "")} " +
                    $"RETURNING *";
                await using NpgsqlCommand update = dataSource.CreateCommand(query);
                await using NpgsqlDataReader rdr = await update.ExecuteReaderAsync();
                await rdr.ReadAsync();
                Clock clock = new(rdr.GetInt32(0), rdr.GetInt32(1), rdr.GetString(2), rdr.GetDateTime(3), rdr.GetInt32(4));

                return Ok(clock);

            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        /// <summary>
        /// Deletes a Clock
        /// </summary>
        /// <param name="id">ID of the Clock</param>
        /// <returns>A Clock deleted</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]

        public async Task<ActionResult<Clock>> DeleteClock(int id)
        {
            try
            {
                await using NpgsqlDataSource dataSource = DatabaseConfig.Create();

                // Check if the ID exists
                bool checkID = await CheckID(dataSource, "clocks", id);
                if (!checkID) return NotFound("No Clock was found with the ID");

                // Fetch the object
                await using NpgsqlCommand get = dataSource.CreateCommand($"SELECT * FROM setr.clocks WHERE id = {id};");
                await using NpgsqlDataReader rdr = await get.ExecuteReaderAsync();
                await rdr.ReadAsync();
                Clock clock = new(rdr.GetInt32(0), rdr.GetInt32(1), rdr.GetString(2), rdr.GetDateTime(3), rdr.GetInt32(4));

                // Deletes the Object
                await using NpgsqlCommand delete = dataSource.CreateCommand($"DELETE FROM setr.clocks WHERE id = {id};");
                await delete.ExecuteNonQueryAsync();
                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}
