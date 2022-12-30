using Microsoft.AspNetCore.Mvc;
using Npgsql;
using trabalhoISI_G10.models;
using static trabalhoISI_G10.Functions;

namespace trabalhoISI_G10.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    [ApiController]
    [Route("rooms")]
    public class RoomsController : ControllerBase
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]

        public async Task<ActionResult<List<Room>>> GetRooms()
        {
            try
            {
                List<Room> rooms = new();

                await using NpgsqlDataSource dataSource = DatabaseConfig.Create();

                // Fetch all Rooms
                string sql = ("SELECT r.id, r.name, count(b.id)::INTEGER AS bookings FROM setr.rooms r LEFT JOIN setr.bookings b ON r.id = b.id_room GROUP BY r.id ORDER BY bookings DESC;");
                await using NpgsqlCommand list = dataSource.CreateCommand(sql);
                await using NpgsqlDataReader rdr = await list.ExecuteReaderAsync();

                // Check if rooms exists
                if (!rdr.HasRows) return NotFound("No Rooms were found with the given parameters");

                // Construct List
                while (await rdr.ReadAsync())
                {
                    rooms.Add(new Room { Id = rdr.GetInt32(0), Name = rdr.GetString(1), Bookings = rdr.GetInt32(3) });
                }

                return Ok(rooms);

            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="newRoom"></param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]

        public async Task<ActionResult<Room>> CreateRoom([FromBody] Room newRoom)
        {
            try
            {
                await using NpgsqlDataSource dataSource = DatabaseConfig.Create();

                // Insert Room
                string query = $"INSERT INTO setr.rooms (name) VALUES ('{newRoom.Name}') RETURNING *";
                await using NpgsqlCommand insert = dataSource.CreateCommand(query);
                await using NpgsqlDataReader rdr = await insert.ExecuteReaderAsync();
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
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]

        public async Task<ActionResult<Room>> GetRoom(int id)
        {
            try
            {
                await using NpgsqlDataSource dataSource = DatabaseConfig.Create();

                // Check if the ID exists
                bool checkID = await CheckID(dataSource, "rooms", id);
                if (!checkID) return NotFound("No Rooms was found with the ID");

                // Fetch the object
                string query = $"SELECT r.id, r.name, count(b.id)::INTEGER AS bookings FROM setr.rooms r WHERE r.id = {id} LEFT JOIN setr.bookings b ON r.id = b.id_room GROUP BY r.id";
                await using NpgsqlCommand get = dataSource.CreateCommand(query);
                await using NpgsqlDataReader rdr = await get.ExecuteReaderAsync();
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
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="updateRoom"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]

        public async Task<ActionResult<Room>> UpdateRoom(int id, [FromBody] Room updateRoom)
        {
            try
            {
                await using NpgsqlDataSource dataSource = DatabaseConfig.Create();

                // Check if the ID exists
                bool checkID = await CheckID(dataSource, "rooms", id);
                if (!checkID) return NotFound("No Room was found with the ID");

                // Update Room
                await using NpgsqlCommand update = dataSource.CreateCommand($"UPDATE setr.rooms SET name = '{updateRoom.Name}' WHERE id = {id};");
                await update.ExecuteNonQueryAsync();

                // Fetch the object
                string query = $"SELECT r.id, r.name, count(b.id)::INTEGER AS bookings FROM setr.rooms r WHERE r.id = {id} LEFT JOIN setr.bookings b ON r.id = b.id_room GROUP BY r.id";
                await using NpgsqlCommand get = dataSource.CreateCommand(query);
                await using NpgsqlDataReader rdr = await get.ExecuteReaderAsync();
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
        /// 
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
                await using NpgsqlDataSource dataSource = DatabaseConfig.Create();

                // Check if the ID exists
                bool checkID = await CheckID(dataSource, "rooms", id);
                if (!checkID) return NotFound("No Room was found with the ID");

                // Fetch the object
                string query = $"SELECT r.id, r.name, count(b.id)::INTEGER AS bookings FROM setr.rooms r WHERE r.id = {id} LEFT JOIN setr.bookings b ON r.id = b.id_room GROUP BY r.id";
                await using NpgsqlCommand get = dataSource.CreateCommand(query);
                await using NpgsqlDataReader rdr = await get.ExecuteReaderAsync();
                await rdr.ReadAsync();
                Room room = new(rdr.GetInt32(0), rdr.GetString(1), rdr.GetInt32(2));

                // Update Room
                await using NpgsqlCommand delete = dataSource.CreateCommand($"DELETE FROM setr.rooms WHERE id = {id};");
                await delete.ExecuteNonQueryAsync();

                return Ok(room);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}