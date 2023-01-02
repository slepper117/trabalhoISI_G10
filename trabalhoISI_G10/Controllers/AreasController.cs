using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Mvc;
using Npgsql;
using trabalhoISI_G10.models;
using static trabalhoISI_G10.Functions;

namespace trabalhoISI_G10.Controllers
{
    /// <summary>
    /// Areas Controller
    /// </summary>
    [ApiController]
    [Route("areas")]
    public class AreasController : ControllerBase
    {
        /// <summary>
        /// Lists all Areas
        /// </summary>
        /// <returns>An array of Areas</returns>
        [HttpGet]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]

        public async Task<ActionResult<List<Area>>> GetAreas()
        {
            try
            {
                // Initialize List and Datasource
                List<Area> areas = new();
                await using NpgsqlDataSource dataSource = NpgsqlDataSource.Create(DatabaseConfig.ConnectionString());

                // Fetch all Areas
                string query = "SELECT * FROM setr.areas;";
                await using NpgsqlCommand cmd = dataSource.CreateCommand(query);
                await using NpgsqlDataReader rdr = await cmd.ExecuteReaderAsync();

                // Check if there are items
                if (!rdr.HasRows) return NotFound("No Areas were found with the given parameters");

                // Construct List
                while (await rdr.ReadAsync())
                {
                    // Construct Area 
                    Area area = new(rdr.GetInt32(0), rdr.GetString(1));

                    // Fetch all Users of a given Areas
                    string getUserQuery = $"SELECT u.id, u.name FROM setr.users_areas ua JOIN setr.users u ON ua.id_user = u.id WHERE ua.id_area = {rdr.GetInt32(0)};";
                    await using NpgsqlCommand getUser = dataSource.CreateCommand(getUserQuery);
                    await using NpgsqlDataReader rdrGetUser = await getUser.ExecuteReaderAsync();

                    // Construct Users List 
                    while (await rdrGetUser.ReadAsync())
                    {
                        User user = new(rdrGetUser.GetInt32(0), rdrGetUser.GetString(1));
                        area.Users.Add(user);
                    }

                    // Add Area to the List
                    areas.Add(area);
                }

                return Ok(areas);
            } catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        /// <summary>
        /// Creates a Area
        /// </summary>
        /// <param name="newArea">Object with the Area properties</param>
        /// <returns>The newly created Area</returns>
        [HttpPost]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]

        public async Task<ActionResult<Area>> CreateArea([FromBody] Area newArea)
        {
            try
            {
                // Initialize Datasource and Area Object
                await using NpgsqlDataSource dataSource = NpgsqlDataSource.Create(DatabaseConfig.ConnectionString());
                Area area = new();

                // Validates Users
                if (newArea.Users.Count > 0)
                {
                    foreach (var item in newArea.Users)
                    {
                        // Check if the User ID exists, and builds User Object
                        string getUserQuery = $"SELECT id, name FROM setr.users WHERE id = {item.Id};";
                        await using NpgsqlCommand getUser = dataSource.CreateCommand(getUserQuery);
                        await using NpgsqlDataReader rdrUser = await getUser.ExecuteReaderAsync();
                        if (!rdrUser.HasRows) return NotFound("No User was found with the provided ID");
                        await rdrUser.ReadAsync();
                        User user = new(rdrUser.GetInt32(0), rdrUser.GetString(1));
                        area.Users.Add(user);
                    }
                }

                // Insert Area
                string query = $"INSERT INTO setr.areas(name) VALUES ('{newArea.Name}') RETURNING *;";
                await using NpgsqlCommand cmd = dataSource.CreateCommand(query);
                await using NpgsqlDataReader rdr = await cmd.ExecuteReaderAsync();
                await rdr.ReadAsync();
                area.Id = rdr.GetInt32(0);
                area.Name = rdr.GetString(1);

                // Inserts Users on Area
                if (newArea.Users.Count > 0)
                {
                    foreach (var item in newArea.Users)
                    {
                        string insertUserQuery = $"INSERT INTO setr.users_areas(id_user, id_area) VALUES ({item.Id}, {rdr.GetInt32(0)});";
                    }
                }

                return Ok(area);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        /// <summary>
        /// Fetchs a Area, by ID
        /// </summary>
        /// <param name="id">ID of the Area</param>
        /// <returns>The fetched Area</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]

        public async Task<ActionResult<Area>> GetArea(int id)
        {
            try
            {
                // Initialize Datasource
                await using NpgsqlDataSource dataSource = NpgsqlDataSource.Create(DatabaseConfig.ConnectionString());

                // Get Area
                string query = $"SELECT * FROM setr.areas WHERE id = {id};";
                await using NpgsqlCommand cmd = dataSource.CreateCommand(query);
                await using NpgsqlDataReader rdr = await cmd.ExecuteReaderAsync();

                // Check if the ID exists
                if (!rdr.HasRows) return NotFound("No Area was found with the provided ID");

                // Construct Object
                await rdr.ReadAsync();
                Area area = new(rdr.GetInt32(0), rdr.GetString(1));

                // Fetch all Users of a given Area
                string getUserQuery = $"SELECT u.id, u.name FROM setr.users_areas ua JOIN setr.users u ON ua.id_user = u.id WHERE ua.id_area = {rdr.GetInt32(0)};";
                await using NpgsqlCommand getUser = dataSource.CreateCommand(getUserQuery);
                await using NpgsqlDataReader rdrGetUser = await getUser.ExecuteReaderAsync();

                // Construct Users List 
                while (await rdrGetUser.ReadAsync())
                {
                    User user = new(rdrGetUser.GetInt32(0), rdrGetUser.GetString(1));
                    area.Users.Add(user);
                }

                return Ok(area);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        /// <summary>
        /// Updates a Area, by a ID
        /// </summary>
        /// <param name="id">ID of the Area</param>
        /// <param name="updateArea">Object with the Area properties to update</param>
        /// <returns>The updated Area</returns>
        [HttpPut("{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]

        public async Task<ActionResult<Area>> UpdateArea(int id, [FromBody] Area updateArea)
        {
            try
            {
                // Initialize Datasource and Area Object
                await using NpgsqlDataSource dataSource = NpgsqlDataSource.Create(DatabaseConfig.ConnectionString());
                Area area = new();

                // Get Area
                string query = $"SELECT * FROM setr.areas WHERE id = {id};";
                await using NpgsqlCommand cmd = dataSource.CreateCommand(query);
                await using NpgsqlDataReader rdr = await cmd.ExecuteReaderAsync();

                // Check if the ID exists
                if (!rdr.HasRows) return NotFound("No Area was found with the provided ID");

                // Updates object Area
                await rdr.ReadAsync();
                area.Id = rdr.GetInt32(0);
                area.Name = rdr.GetString(1);

                // Validates Users
                if (updateArea.Users.Count > 0)
                {
                    foreach (var item in updateArea.Users)
                    {
                        // Check if the User ID exists, and builds User Object
                        string getUserQuery = $"SELECT id, name FROM setr.users WHERE id = {item.Id};";
                        await using NpgsqlCommand getUser = dataSource.CreateCommand(getUserQuery);
                        await using NpgsqlDataReader rdrUser = await getUser.ExecuteReaderAsync();
                        if (!rdrUser.HasRows) return NotFound("No User was found with the provided ID");
                        await rdrUser.ReadAsync();
                        User user = new(rdrUser.GetInt32(0), rdrUser.GetString(1));
                        area.Users.Add(user);
                    }

                    // Deletes all the users in Area
                    string deleteUsersQuery = $"DELETE FROM setr.users_areas WHERE id_area = {id}";
                    await using NpgsqlCommand deleteUsers = dataSource.CreateCommand(deleteUsersQuery);
                    await deleteUsers.ExecuteNonQueryAsync();

                    foreach (var item in updateArea.Users)
                    {
                        // Insert new Users into Area
                        string insertUserQuery = $"INSERT INTO setr.users_areas(id_user, id_area) VALUES ({item.Id}, {id});";
                        await using NpgsqlCommand insertUser = dataSource.CreateCommand(insertUserQuery);
                        await insertUser.ExecuteNonQueryAsync();
                    }
                }
                else
                {
                    // Fetch all Users of a given Area
                    string getUserQuery = $"SELECT u.id, u.name FROM setr.users_areas ua JOIN setr.users u ON ua.id_user = u.id WHERE ua.id_area = {id};";
                    await using NpgsqlCommand getUser = dataSource.CreateCommand(getUserQuery);
                    await using NpgsqlDataReader rdrGetUser = await getUser.ExecuteReaderAsync();

                    // Construct Users List 
                    while (await rdrGetUser.ReadAsync())
                    {
                        User user = new(rdrGetUser.GetInt32(0), rdrGetUser.GetString(1));
                        area.Users.Add(user);
                    }
                }

                if (updateArea.Name.Length != 0)
                {
                    string updateQuery = $"UPDATE setr.bookings SET (name) = ('{updateArea.Name}');";
                    await using NpgsqlCommand getUser = dataSource.CreateCommand(updateQuery);
                    await getUser.ExecuteReaderAsync();
                    area.Name = updateArea.Name;
                }

                return Ok(area);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }

        }

        /// <summary>
        /// Deletes a Area
        /// </summary>
        /// <param name="id">The ID of the area to be deleted</param>
        /// <returns>The area deleted</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]

        public async Task<ActionResult<Area>> DeleteArea(int id)
        {
            try
            {
                // Initialize Datasource
                await using NpgsqlDataSource dataSource = NpgsqlDataSource.Create(DatabaseConfig.ConnectionString());

                // Get Area
                string query = $"SELECT * FROM setr.areas WHERE id = {id};";
                await using NpgsqlCommand cmd = dataSource.CreateCommand(query);
                await using NpgsqlDataReader rdr = await cmd.ExecuteReaderAsync();

                // Check if the ID exists
                if (!rdr.HasRows) return NotFound("No Area was found with the provided ID");

                // Construct Object
                await rdr.ReadAsync();
                Area area = new(rdr.GetInt32(0), rdr.GetString(1));

                // Fetch all Users of a given Areas
                string getUserQuery = $"SELECT u.id, u.name FROM setr.users_areas ua JOIN setr.users u ON ua.id_user = u.id WHERE ua.id_area = {rdr.GetInt32(0)};";
                await using NpgsqlCommand getUser = dataSource.CreateCommand(getUserQuery);
                await using NpgsqlDataReader rdrGetUser = await getUser.ExecuteReaderAsync();

                // Construct Users List 
                while (await rdrGetUser.ReadAsync())
                {
                    User user = new(rdrGetUser.GetInt32(0), rdrGetUser.GetString(1));
                    area.Users.Add(user);
                }

                // Deletes Area
                string deleteQuery = $"DELETE FROM setr.users_areas WHERE id_area = {id}; DELETE FROM setr.areas WHERE id = {id};";
                await using NpgsqlCommand delete = dataSource.CreateCommand(deleteQuery);
                await delete.ExecuteNonQueryAsync();

                return Ok(area);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}