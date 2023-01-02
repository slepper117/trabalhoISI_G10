using Microsoft.AspNetCore.Mvc;
using Npgsql;
using trabalhoISI_G10.models;

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
                    areas.Add(new Area(rdr.GetInt32(0), rdr.GetString(1)));
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
                // Initialize Datasource
                await using NpgsqlDataSource dataSource = NpgsqlDataSource.Create(DatabaseConfig.ConnectionString());

                // Insert Area
                string query = $"INSERT INTO setr.areas(name) VALUES ('{newArea.Name}') RETURNING *;";
                await using NpgsqlCommand cmd = dataSource.CreateCommand(query);
                await using NpgsqlDataReader rdr = await cmd.ExecuteReaderAsync();
                await rdr.ReadAsync();
                Area area = new(rdr.GetInt32(0), rdr.GetString(1));

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
                // Initialize Datasource
                await using NpgsqlDataSource dataSource = NpgsqlDataSource.Create(DatabaseConfig.ConnectionString());

                // Get Area
                string query = $"SELECT * FROM setr.areas WHERE id = {id};";
                await using NpgsqlCommand cmd = dataSource.CreateCommand(query);
                await using NpgsqlDataReader rdr = await cmd.ExecuteReaderAsync();

                // Check if the ID exists
                if (!rdr.HasRows) return NotFound("No Room was found with the provided ID");


                // Validates Name
                if (updateArea.Name.Length == 0) return BadRequest("Name property cannot be empty");

                // Updates Area with new values
                string updateQuery = $"UPDATE setr.areas SET name = '{updateArea.Name}' WHERE id = {id};";
                await using NpgsqlCommand update = dataSource.CreateCommand(updateQuery);
                await update.ExecuteNonQueryAsync();

                // Construct Objects
                await rdr.ReadAsync();
                Area area = new(rdr.GetInt32(0), rdr.GetString(1));

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

                // Get Room
                string query = $"SELECT * FROM setr.areas WHERE id = {id};";
                await using NpgsqlCommand cmd = dataSource.CreateCommand(query);
                await using NpgsqlDataReader rdr = await cmd.ExecuteReaderAsync();

                // Check if the ID exists
                if (!rdr.HasRows) return NotFound("No Area was found with the provided ID");

                // Deletes Room
                string deleteQuery = $"DELETE FROM setr.users_areas WHERE id_area = {id}; DELETE FROM setr.areas WHERE id = {id};";
                await using NpgsqlCommand delete = dataSource.CreateCommand(deleteQuery);
                await delete.ExecuteNonQueryAsync();

                // Construct Object
                await rdr.ReadAsync();
                Area area = new(rdr.GetInt32(0), rdr.GetString(1));

                return Ok(area);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}