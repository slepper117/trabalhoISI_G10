using Microsoft.AspNetCore.Mvc;
using Npgsql;
using trabalhoISI_G10.models;
using static trabalhoISI_G10.Functions;

namespace trabalhoISI_G10.Controllers
{
    /// <summary>
    /// Bookings Controller
    /// </summary>
    [ApiController]
    [Route("bookings")]
    public class BookingsController : ControllerBase
    {
        /// <summary>
        /// Lists all Bookings
        /// </summary>
        /// <returns>An array of Bookings</returns>
        [HttpGet]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<List<Booking>>> GetBookings()
        {
            try
            {
                // Initialize List and Datasource
                List<Booking> bookings = new();
                await using NpgsqlDataSource dataSource = NpgsqlDataSource.Create(DatabaseConfig.ConnectionString());

                // Fetch all Bookings
                string query = "SELECT u.id, u.name, r.id, r.name, (SELECT count(b.id)::INTEGER FROM setr.rooms r LEFT JOIN setr.bookings b ON r.id = b.id_room GROUP BY r.id) AS bookings, b.id, b.start, b.final, b.description, b.validated FROM setr.bookings b JOIN setr.users u ON b.id_user = u.id JOIN setr.rooms r ON b.id_room = r.id;";
                await using NpgsqlCommand cmd = dataSource.CreateCommand(query);
                await using NpgsqlDataReader rdr = await cmd.ExecuteReaderAsync();

                // Check if Bookings exists
                if (!rdr.HasRows) return NotFound("No Bookings were found with the given parameters");

                // Construct List
                while (await rdr.ReadAsync())
                {
                    User user = new(rdr.GetInt32(0), rdr.GetString(1));
                    Room room = new(rdr.GetInt32(2), rdr.GetString(3), rdr.GetInt32(4));
                    bookings.Add(new Booking(rdr.GetInt32(5), user, room, rdr.GetDateTime(6), rdr.GetDateTime(7), rdr.GetString(8), rdr.GetBoolean(9)));
                }

                return Ok(bookings);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        /// <summary>
        /// Creates a Booking
        /// </summary>
        /// <param name="newBooking">Object with the Booking properties</param>
        /// <returns>The newly created Booking</returns>
        [HttpPost]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<Booking>> CreateBooking([FromBody] Booking newBooking)
        {
            try
            {
                // Initialize Datasource
                await using NpgsqlDataSource dataSource = NpgsqlDataSource.Create(DatabaseConfig.ConnectionString());

                // Check if the User ID exists, and builds User Object
                string getUserQuery = $"SELECT id, name FROM setr.users WHERE id = {newBooking.User.Id};";
                await using NpgsqlCommand getUser = dataSource.CreateCommand(getUserQuery);
                await using NpgsqlDataReader rdrUser = await getUser.ExecuteReaderAsync();
                if (!rdrUser.HasRows) return NotFound("No User was found with the provided ID");
                await rdrUser.ReadAsync();
                User user = new(rdrUser.GetInt32(0), rdrUser.GetString(1));

                // Check if the Room ID exists, and builds Room Object
                string getRoomQuery = $"SELECT r.id, r.name, count(b.id)::INTEGER AS bookings FROM setr.rooms r LEFT JOIN setr.bookings b ON r.id = b.id_room WHERE r.id = {newBooking.Room.Id} GROUP BY r.id;";
                await using NpgsqlCommand getRoom = dataSource.CreateCommand(getRoomQuery);
                await using NpgsqlDataReader rdrRoom = await getRoom.ExecuteReaderAsync();
                if (!rdrRoom.HasRows) return NotFound("No Room was found with the provided ID");
                await rdrRoom.ReadAsync();
                Room room = new(rdrRoom.GetInt32(0), rdrRoom.GetString(1), rdrRoom.GetInt32(2) + 1);

                // Check if Start Date is bigger than End Date
                if (newBooking.Start > newBooking.End) BadRequest("Start date cannot be bigger than end date");

                // Check Availabilaty
                bool checkAvailabilaty = await CheckAvailabilaty(dataSource, newBooking.Room.Id, newBooking.Start, newBooking.End);
                if (!checkAvailabilaty) return BadRequest("Cannot validate because there is another booking on time interval");

                // Insert Booking
                string query = $"INSERT INTO setr.bookings(id_user, id_room, start, final, description, validated) VALUES ({newBooking.User.Id}, {newBooking.Room.Id}, '{newBooking.Start:s}', '{newBooking.End:s}', '{newBooking.Description}', 'false') RETURNING *;";
                await using NpgsqlCommand cmd = dataSource.CreateCommand(query);
                await using NpgsqlDataReader rdr = await cmd.ExecuteReaderAsync();
                await rdr.ReadAsync();
                Booking booking = new(rdr.GetInt32(0), user, room, rdr.GetDateTime(3), rdr.GetDateTime(4), rdr.GetString(5), rdr.GetBoolean(6));

                return Ok(booking);
            } catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        /// <summary>
        /// Fetchs a Booking, by ID.
        /// </summary>
        /// <param name="id">ID of the Booking</param>
        /// <returns>The fetched Booking</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<Booking>> GetBooking(int id)
        {
            try
            {
                // Initialize Datasource
                await using NpgsqlDataSource dataSource = NpgsqlDataSource.Create(DatabaseConfig.ConnectionString());

                // Get Booking
                string query = $"SELECT u.id, u.name, r.id, r.name, (SELECT count(b.id)::INTEGER FROM setr.rooms r LEFT JOIN setr.bookings b ON r.id = b.id_room GROUP BY r.id) AS bookings, b.id, b.start, b.final, b.description, b.validated FROM setr.bookings b JOIN setr.users u ON b.id_user = u.id JOIN setr.rooms r ON b.id_room = r.id WHERE b.id = {id};";
                await using NpgsqlCommand cmd = dataSource.CreateCommand(query);
                await using NpgsqlDataReader rdr = await cmd.ExecuteReaderAsync();

                // Check if the ID exists
                if (!rdr.HasRows) return NotFound("No Booking was found with the provided ID");

                // Construct Object
                await rdr.ReadAsync();
                User user = new(rdr.GetInt32(0), rdr.GetString(1));
                Room room = new(rdr.GetInt32(2), rdr.GetString(3), rdr.GetInt32(4));
                Booking booking = new(rdr.GetInt32(5), user, room, rdr.GetDateTime(6), rdr.GetDateTime(7), rdr.GetString(8), rdr.GetBoolean(9));

                return Ok(booking);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        /// <summary>
        /// Updates a Booking, by a ID
        /// </summary>
        /// <param name="id">ID of the Booking</param>
        /// <param name="updateBooking">Object with the Booking properties to update</param>
        /// <returns>The updated Booking</returns>
        [HttpPut("{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<Booking>> UpdateBooking(int id, [FromBody] Booking updateBooking)
        {
            try
            {
                // Initialize Datasource
                await using NpgsqlDataSource dataSource = NpgsqlDataSource.Create(DatabaseConfig.ConnectionString());

                // Get Booking
                string getBookingQuery = $"SELECT * FROM setr.bookings WHERE id = {id};";
                await using NpgsqlCommand getBooking = dataSource.CreateCommand(getBookingQuery);
                await using NpgsqlDataReader rdrGetBooking = await getBooking.ExecuteReaderAsync();

                // Check if the ID exists
                if (!rdrGetBooking.HasRows) return NotFound("No Booking was found with the provided ID");

                // Get Values
                await rdrGetBooking.ReadAsync();
                int idUser = rdrGetBooking.GetInt32(1);
                int idRoom = rdrGetBooking.GetInt32(2);
                DateTime start = rdrGetBooking.GetDateTime(3);
                DateTime end = rdrGetBooking.GetDateTime(4);
                string description = rdrGetBooking.GetString(5);
                bool validated = rdrGetBooking.GetBoolean(6);

                // Check if Booking is validated
                if (validated) return BadRequest("Cannot update Booking, because it was validated");

                // Validate User
                if (updateBooking.User.Id != idUser)
                {
                    bool checkUser = await CheckID(dataSource, "users", updateBooking.User.Id);
                    if (!checkUser) return NotFound("No User was found with the provided ID");

                    // Set new Value
                    idUser = updateBooking.User.Id;
                }

                // Validate Room
                if (updateBooking.Room.Id != idRoom)
                {
                    bool checkRoom = await CheckID(dataSource, "rooms", updateBooking.Room.Id);
                    if (!checkRoom) return NotFound("No Room was found with the provided ID");

                    // Check availability ou new room
                    bool checkAvailabilaty = await CheckAvailabilaty(dataSource, updateBooking.Room.Id, start, end);
                    if (!checkAvailabilaty) return BadRequest("Cannot change Room because theres another booking validated");

                    // Set new Value
                    idRoom = updateBooking.Room.Id;
                }

                // Validates Dates
                bool updateStart = (updateBooking.Start.ToString("s").Length != 0);
                bool updateEnd = (updateBooking.End.ToString("s").Length != 0);

                if (updateStart || updateEnd)
                {
                    if (updateStart && updateEnd)
                    {
                        // Check if Start Date is bigger than End Date
                        if (updateBooking.Start > updateBooking.End) BadRequest("Start date cannot be bigger than end date");

                        // Check Availabilaty
                        bool checkAvailabilaty = await CheckAvailabilaty(dataSource, idRoom, updateBooking.Start, updateBooking.End);
                        if (!checkAvailabilaty) return BadRequest("Cannot update because there is another booking validated on time interval");

                        // Set new Value
                        start = updateBooking.Start;
                        end = updateBooking.End;
                    }
                    else if (updateStart)
                    {
                        // Check if Start Date is bigger than End Date
                        if (updateBooking.Start > end) BadRequest("Start date cannot be bigger than end date");

                        // Check Availabilaty
                        bool checkAvailabilaty = await CheckAvailabilaty(dataSource, idRoom, updateBooking.Start, end);
                        if (!checkAvailabilaty) return BadRequest("Cannot update because there is another booking validated on time interval");

                        // Set new Value
                        start = updateBooking.Start;
                    }
                    else if (updateEnd)
                    {
                        // Check if Start Date is bigger than End Date
                        if (start > updateBooking.End) BadRequest("Start date cannot be bigger than end date");

                        // Check Availabilaty
                        bool checkAvailabilaty = await CheckAvailabilaty(dataSource, idRoom, start, updateBooking.End);
                        if (!checkAvailabilaty) return BadRequest("Cannot update because there is another booking validated on time interval");

                        // Set new Value
                        end = updateBooking.End;
                    }
                }

                // Validates Booking
                if (updateBooking.Description.Length != 0) description = updateBooking.Description;

                // Updates Booking with new values
                string updateQuery = $"UPDATE setr.bookings SET (id_user, id_room, start, final, description) = ({idUser},  {idRoom}, '{start}', '{end}', '{description}') WHERE id = {id};";
                await using NpgsqlCommand update = dataSource.CreateCommand(updateQuery);
                await update.ExecuteNonQueryAsync();

                // Get updated Booking
                string query = $"SELECT u.id, u.name, r.id, r.name, (SELECT count(b.id)::INTEGER FROM setr.rooms r LEFT JOIN setr.bookings b ON r.id = b.id_room GROUP BY r.id) AS count, b.id, b.start, b.final, b.description, b.validated FROM setr.bookings b JOIN setr.users u ON b.id_user = u.id JOIN setr.rooms r ON b.id_room = r.id WHERE b.id = {id};";
                await using NpgsqlCommand cmd = dataSource.CreateCommand(query);
                await using NpgsqlDataReader rdr = await cmd.ExecuteReaderAsync();

                // Construct Objects
                await rdr.ReadAsync();
                User user = new(rdr.GetInt32(0), rdr.GetString(1));
                Room room = new(rdr.GetInt32(2), rdr.GetString(3), rdr.GetInt32(4));
                Booking booking = new(rdr.GetInt32(5), user, room, rdr.GetDateTime(6), rdr.GetDateTime(7), rdr.GetString(8), rdr.GetBoolean(9));

                return Ok(booking);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        /// <summary>
        /// Deletes a Booking, by ID
        /// </summary>
        /// <param name="id">ID of the Booking</param>
        /// <returns>The deleted Booking</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<Booking>> DeleteBooking(int id)
        {
            try
            {
                // Initialize Datasource
                await using NpgsqlDataSource dataSource = NpgsqlDataSource.Create(DatabaseConfig.ConnectionString());

                // Get Booking
                string query = $"SELECT u.id, u.name, r.id, r.name, (SELECT count(b.id)::INTEGER FROM setr.rooms r LEFT JOIN setr.bookings b ON r.id = b.id_room GROUP BY r.id) AS count, b.id, b.start, b.final, b.description, b.validated FROM setr.bookings b JOIN setr.users u ON b.id_user = u.id JOIN setr.rooms r ON b.id_room = r.id WHERE b.id = {id};";
                await using NpgsqlCommand cmd = dataSource.CreateCommand(query);
                await using NpgsqlDataReader rdr = await cmd.ExecuteReaderAsync();

                // Check if the ID exists
                if (!rdr.HasRows) return NotFound("No Booking was found with the provided ID");

                // Check if Booking is validated
                await rdr.ReadAsync();
                if (rdr.GetBoolean(9)) return BadRequest("Cannot delete Booking, because it was validated");

                // Deletes Booking
                string deleteQuery = $"DELETE FROM setr.bookings WHERE id = {id};";
                await using NpgsqlCommand delete = dataSource.CreateCommand(deleteQuery);
                await delete.ExecuteNonQueryAsync();

                // Construct Objects
                User user = new(rdr.GetInt32(0), rdr.GetString(1));
                Room room = new(rdr.GetInt32(2), rdr.GetString(3), rdr.GetInt32(4));
                Booking booking = new(rdr.GetInt32(5), user, room, rdr.GetDateTime(6), rdr.GetDateTime(7), rdr.GetString(8), rdr.GetBoolean(9));

                return Ok(booking);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        /// <summary>
        /// Validate Booking, by ID
        /// </summary>
        /// <param name="id">ID of the Booking</param>
        /// <param name="updateBooking">Object with Bookings validated property to update</param>
        /// <returns>The updated Booking</returns>
        [HttpPut("{id}/validate")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<Booking>> ValidateBooking(int id,[FromBody] Booking updateBooking)
        {
            try
            {
                await using NpgsqlDataSource dataSource = NpgsqlDataSource.Create(DatabaseConfig.ConnectionString());

                // Get Booking
                string query = $"SELECT u.id, u.name, r.id, r.name, (SELECT count(b.id)::INTEGER FROM setr.rooms r LEFT JOIN setr.bookings b ON r.id = b.id_room GROUP BY r.id) AS count, b.id, b.start, b.final, b.description, b.validated FROM setr.bookings b JOIN setr.users u ON b.id_user = u.id JOIN setr.rooms r ON b.id_room = r.id WHERE b.id = {id};";
                await using NpgsqlCommand cmd = dataSource.CreateCommand(query);
                await using NpgsqlDataReader rdr = await cmd.ExecuteReaderAsync();
                await rdr.ReadAsync();

                // Check if the ID exists
                if (!rdr.HasRows) return NotFound("No Booking was found with the provided ID"); ;

                // Check Availabilaty
                bool checkAvailabilaty = await CheckAvailabilaty(dataSource, rdr.GetInt32(2), rdr.GetDateTime(6), rdr.GetDateTime(7));
                if(!checkAvailabilaty) return BadRequest("Cannot validate because there is another booking on time interval");

                // Updates Validated Status
                string updateQuery = $"UPDATE setr.bookings SET validated = '{updateBooking.Validated}' WHERE id = {id};";
                await using NpgsqlCommand update = dataSource.CreateCommand(updateQuery);
                await update.ExecuteNonQueryAsync();

                // Construct Objects
                User user = new(rdr.GetInt32(0), rdr.GetString(1));
                Room room = new(rdr.GetInt32(2), rdr.GetString(3), rdr.GetInt32(4));
                Booking booking = new(rdr.GetInt32(5), user, room, rdr.GetDateTime(6), rdr.GetDateTime(7), rdr.GetString(8), updateBooking.Validated);

                return Ok(booking);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}
