using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Npgsql;
using trabalhoISI_G10.Config;
using trabalhoISI_G10.DTO;

namespace trabalhoISI_G10.Controllers
{
    /// <summary>
    /// Access Controller
    /// </summary>
    [Authorize]
    [ApiController]
    [Route("access")]
    [ProducesResponseType(200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(401)]
    public class AccessController : ControllerBase
    {
        /// <summary>
        /// Create a ClockIn 
        /// </summary>
        /// <param name="tag">Object with the Tag properties</param>
        /// <returns>Access Object</returns>
        [HttpPost("clockin")]
        [ProducesResponseType(404)]
        public async Task<ActionResult<Access>> ClockIn([FromBody] Tag tag)
        {
            try
            {
                // Initialize Datasource and Area Object
                await using NpgsqlDataSource dataSource = NpgsqlDataSource.Create(Database.ConnectionString());
                Access access = new("authorized", DateTime.UtcNow);

                // Check if the User Tag exists
                string getUserQuery = $"SELECT id, name FROM setr.users WHERE tag = {tag.TagID};";
                await using NpgsqlCommand getUser = dataSource.CreateCommand(getUserQuery);
                await using NpgsqlDataReader rdrUser = await getUser.ExecuteReaderAsync();
                if (!rdrUser.HasRows) return NotFound("No User was found with the provided Tag");

                // Adds name to object
                await rdrUser.ReadAsync();
                access.Name = rdrUser.GetString(1);

                //Creates Clock on Database
                string query = $"INSERT INTO setr.clocks(id_user, direction, datetime, log) VALUES ({rdrUser.GetInt32(0)}, 'in', '{access.DateTime:s}', 1);";
                await using NpgsqlCommand cmd = dataSource.CreateCommand(query);
                await cmd.ExecuteNonQueryAsync();

                return Ok(access);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        /// <summary>
        /// Create a ClockOut
        /// </summary>
        /// <param name="tag">Object with the Tag properties</param>
        /// <returns>Access Object</returns>
        [HttpPost("clockout")]
        [ProducesResponseType(404)]
        public async Task<ActionResult<Access>> ClockOut([FromBody] Tag tag)
        {
            try
            {
                // Initialize Datasource and Area Object
                await using NpgsqlDataSource dataSource = NpgsqlDataSource.Create(Database.ConnectionString());
                Access access = new("authorized", DateTime.UtcNow);

                // Check if the User Tag exists
                string getUserQuery = $"SELECT id, name FROM setr.users WHERE tag = {tag.TagID};";
                await using NpgsqlCommand getUser = dataSource.CreateCommand(getUserQuery);
                await using NpgsqlDataReader rdrUser = await getUser.ExecuteReaderAsync();
                if (!rdrUser.HasRows) return NotFound("No User was found with the provided Tag");

                // Adds name to object
                await rdrUser.ReadAsync();
                access.Name = rdrUser.GetString(1);

                //Creates Clock on Database
                string query = $"INSERT INTO setr.clocks(id_user, direction, datetime, log) VALUES ({rdrUser.GetInt32(0)}, 'out', '{access.DateTime:s}', 1);";
                await using NpgsqlCommand cmd = dataSource.CreateCommand(query);
                await cmd.ExecuteNonQueryAsync();

                return Ok(access);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        /// <summary>
        /// Check an Area Authorization
        /// </summary>
        /// <param name="id">Id of Area</param>
        /// <param name="tag">Object with the Tag properties</param>
        /// <returns>Access Object</returns>
        [HttpPost("area/{id}")]
        [ProducesResponseType(404)]
        public async Task<ActionResult<Access>> AuthArea(int id, [FromBody] Tag tag)
        {
            try
            {
                // Initialize Datasource and Area Object
                await using NpgsqlDataSource dataSource = NpgsqlDataSource.Create(Database.ConnectionString());
                Access access = new("authorized", DateTime.UtcNow);

                // Check if the Area exists
                string getAreaQuery = $"SELECT id FROM setr.areas WHERE id = {id};";
                await using NpgsqlCommand getArea = dataSource.CreateCommand(getAreaQuery);
                await using NpgsqlDataReader rdrArea = await getArea.ExecuteReaderAsync();
                if (!rdrArea.HasRows) return NotFound("No Area was found with the provided ID");

                // Check if the User Tag exists
                string getUserQuery = $"SELECT id, name FROM setr.users WHERE tag = {tag.TagID};";
                await using NpgsqlCommand getUser = dataSource.CreateCommand(getUserQuery);
                await using NpgsqlDataReader rdrUser = await getUser.ExecuteReaderAsync();
                if (!rdrUser.HasRows) return NotFound("No User was found with the provided Tag");

                // Adds name to object
                await rdrUser.ReadAsync();
                access.Name = rdrUser.GetString(1);

                // Check if User is authorized
                string query = $"SELECT * FROM setr.users_areas WHERE id_user = {rdrUser.GetInt32(0)} AND id_area = {id};";
                await using NpgsqlCommand cmd = dataSource.CreateCommand(query);
                await using NpgsqlDataReader rdr = await cmd.ExecuteReaderAsync();
                if (!rdr.HasRows) return Unauthorized("User is not authorized to access this area");

                return Ok(access);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        /// <summary>
        /// Check an Room Authorization
        /// </summary>
        /// <param name="id">Id of Room</param>
        /// <param name="tag">Object with the Tag properties</param>
        /// <returns>Access Object</returns>
        [HttpPost("room/{id}")]
        [ProducesResponseType(404)]
        public async Task<ActionResult<Access>> AuthRoom(int id, [FromBody] Tag tag)
        {
            try
            {
                // Initialize Datasource and Area Object
                await using NpgsqlDataSource dataSource = NpgsqlDataSource.Create(Database.ConnectionString());
                Access access = new("authorized", DateTime.UtcNow);

                // Check if the Room exists
                string getRoomQuery = $"SELECT id FROM setr.room WHERE id = {id};";
                await using NpgsqlCommand getRoom = dataSource.CreateCommand(getRoomQuery);
                await using NpgsqlDataReader rdrRoom = await getRoom.ExecuteReaderAsync();
                if (!rdrRoom.HasRows) return NotFound("No Room was found with the provided ID");

                // Check if the User Tag exists
                string getUserQuery = $"SELECT id, name FROM setr.users WHERE tag = {tag.TagID};";
                await using NpgsqlCommand getUser = dataSource.CreateCommand(getUserQuery);
                await using NpgsqlDataReader rdrUser = await getUser.ExecuteReaderAsync();
                if (!rdrUser.HasRows) return NotFound("No User was found with the provided Tag");

                // Adds name to object
                await rdrUser.ReadAsync();
                access.Name = rdrUser.GetString(1);

                // Check if User is authorized
                string query = $"SELECT * FROM setr.bookings WHERE id_user = {rdrUser.GetInt32(0)} AND id_room = {id} AND validated = 'true' AND start <= '{access.DateTime:s}' AND end >= '{access.DateTime:s}'";
                await using NpgsqlCommand cmd = dataSource.CreateCommand(query);
                await using NpgsqlDataReader rdr = await cmd.ExecuteReaderAsync();
                if (!rdr.HasRows) return Unauthorized("User is not authorized to access this room");

                return Ok(access);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}
