using Microsoft.AspNetCore.Mvc;
using Npgsql;
using trabalhoISI_G10.models;

namespace trabalhoISI_G10.Controllers
{
    /// <summary>
    /// Rooms Controllers
    /// </summary>
    [ApiController]
    [Route("rooms")]
    public class RoomsController : ControllerBase
    {
        /// <summary>
        /// Lists all Rooms
        /// </summary>
        /// <returns>An array of Rooms</returns>
        [HttpGet]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]

        public async Task<ActionResult<List<Room>>> GetRooms()
        {
            try
            {
                // Initialize List and Datasource
                List<Room> rooms = new();
                await using NpgsqlDataSource dataSource = NpgsqlDataSource.Create(DatabaseConfig.ConnectionString());

                // Fetch all Rooms
                string query = $"SELECT r.id, r.name, count(b.id)::INTEGER FROM setr.rooms r LEFT JOIN setr.bookings b ON r.id = b.id_room GROUP BY r.id";
                await using NpgsqlCommand cmd = dataSource.CreateCommand(query);
                await using NpgsqlDataReader rdr = await cmd.ExecuteReaderAsync();

                // Check if rooms exists
                if (!rdr.HasRows) return NotFound("No Rooms were found with the given parameters");

                // Construct List
                while (await rdr.ReadAsync())
                {
                    rooms.Add(new Room(rdr.GetInt32(0), rdr.GetString(1), rdr.GetInt32(3)));
                }

                return Ok(rooms);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        /// <summary>
        /// Creates a Rooms
        /// </summary>
        /// <param name="newRoom">Object with the Room properties</param>
        /// <returns>The newly created Room</returns>
        [HttpPost]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]

        public async Task<ActionResult<Room>> CreateRoom([FromBody] Room newRoom)
        {
            try
            {
                // Initialize Datasource
                await using NpgsqlDataSource dataSource = NpgsqlDataSource.Create(DatabaseConfig.ConnectionString());

                // Insert Room
                string query = $"INSERT INTO setr.rooms(name) VALUES ('{newRoom.Name}') RETURNING *";
                await using NpgsqlCommand cmd = dataSource.CreateCommand(query);
                await using NpgsqlDataReader rdr = await cmd.ExecuteReaderAsync();
                await rdr.ReadAsync();
                Room room = new(rdr.GetInt32(0), rdr.GetString(1), 0);

                return Ok(room);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        /// <summary>
        /// Fetchs a Room, by ID
        /// </summary>
        /// <param name="id">ID of the Room</param>
        /// <returns>The fetched Room</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]

        public async Task<ActionResult<Room>> GetRoom(int id)
        {
            try
            {
                // Initialize Datasource
                await using NpgsqlDataSource dataSource = NpgsqlDataSource.Create(DatabaseConfig.ConnectionString());

                // Get Room
                string query = $"SELECT r.id, r.name, count(b.id)::INTEGER FROM setr.rooms r LEFT JOIN setr.bookings b ON r.id = b.id_room WHERE r.id = {id} GROUP BY r.id";
                await using NpgsqlCommand cmd = dataSource.CreateCommand(query);
                await using NpgsqlDataReader rdr = await cmd.ExecuteReaderAsync();

                // Check if the ID exists
                if (!rdr.HasRows) return NotFound("No Room was found with the provided ID");

                // Construct Object
                await rdr.ReadAsync();
                Room room = new(rdr.GetInt32(0), rdr.GetString(1), rdr.GetInt32(2));

                return Ok(room);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        /// <summary>
        /// Updates a Room, by a ID
        /// </summary>
        /// <param name="id">ID of the Room</param>
        /// <param name="updateRoom">Object with the Room properties to update</param>
        /// <returns>The updated Room</returns>
        [HttpPut("{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]

        public async Task<ActionResult<Room>> UpdateRoom(int id, [FromBody] Room updateRoom)
        {
            try
            {
                // Initialize Datasource
                await using NpgsqlDataSource dataSource = NpgsqlDataSource.Create(DatabaseConfig.ConnectionString());

                // Get Room
                string query = $"SELECT r.id, r.name, count(b.id)::INTEGER FROM setr.rooms r LEFT JOIN setr.bookings b ON r.id = b.id_room WHERE r.id = {id} GROUP BY r.id";
                await using NpgsqlCommand cmd = dataSource.CreateCommand(query);
                await using NpgsqlDataReader rdr = await cmd.ExecuteReaderAsync();

                // Check if the ID exists
                if (!rdr.HasRows) return NotFound("No Room was found with the provided ID");

                // Validates Name
                if (updateRoom.Name.Length == 0) return BadRequest("Name property cannot be empty");

                // Updates Room with new values
                string updateQuery = $"UPDATE setr.rooms SET name = '{updateRoom.Name}' WHERE id = {id};";
                await using NpgsqlCommand update = dataSource.CreateCommand(updateQuery);
                await update.ExecuteNonQueryAsync();

                // Construct Objects
                await rdr.ReadAsync();
                Room room = new(rdr.GetInt32(0), updateRoom.Name, rdr.GetInt32(2));

                return Ok(room);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }

        }

        /// <summary>
        /// Deletes a Room, by ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]

        public async Task<ActionResult<Room>> DeleteRoom(int id)
        {
            try
            {
                // Initialize Datasource
                await using NpgsqlDataSource dataSource = NpgsqlDataSource.Create(DatabaseConfig.ConnectionString());

                // Get Room
                string query = $"SELECT r.id, r.name, count(b.id)::INTEGER FROM setr.rooms r LEFT JOIN setr.bookings b ON r.id = b.id_room WHERE r.id = {id} GROUP BY r.id";
                await using NpgsqlCommand cmd = dataSource.CreateCommand(query);
                await using NpgsqlDataReader rdr = await cmd.ExecuteReaderAsync();

                // Check if the ID exists
                if (!rdr.HasRows) return NotFound("No Room was found with the provided ID");

                // Deletes Room
                string deleteQuery = $"DELETE FROM setr.bookings WHERE id_room = {id}; DELETE FROM setr.rooms WHERE id = {id}";
                await using NpgsqlCommand delete = dataSource.CreateCommand(deleteQuery);
                await delete.ExecuteNonQueryAsync();

                // Construct Object
                await rdr.ReadAsync();
                Room room = new(rdr.GetInt32(0), rdr.GetString(1), rdr.GetInt32(2));

                return Ok(room);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}