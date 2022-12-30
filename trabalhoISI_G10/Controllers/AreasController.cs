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
    [Route("areas")]
    public class AreasController : ControllerBase
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]

        public async Task<ActionResult<List<Area>>> GetAreas()
        {
            try
            {
                List<Area> areas = new();

                await using NpgsqlDataSource dataSource = DatabaseConfig.Create();

                // Fetch all Areas
                await using NpgsqlCommand list = dataSource.CreateCommand("SELECT * FROM setr.areas;");
                await using NpgsqlDataReader rdr = await list.ExecuteReaderAsync();

                // Check if areas exists
                if (!rdr.HasRows) return NotFound("No Areas were found with the given parameters");

                // Construct List
                while (await rdr.ReadAsync())
                {
                    areas.Add(new Area( rdr.GetInt32(0), rdr.GetString(1) ));
                }

                return Ok(areas);

            } catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="newArea"></param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]

        public async Task<ActionResult<Area>> CreateArea([FromBody] Area newArea)
        {
            try
            {
                await using NpgsqlDataSource dataSource = DatabaseConfig.Create();

                // Insert Area
                string query = $"INSERT INTO setr.areas (name) VALUES ('{newArea.Name}') RETURNING *";
                await using NpgsqlCommand insert = dataSource.CreateCommand(query);
                await using NpgsqlDataReader rdr = await insert.ExecuteReaderAsync();
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
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]

        public async Task<ActionResult<Area>> GetArea(int id)
        {
            try
            {
                await using NpgsqlDataSource dataSource = DatabaseConfig.Create();

                // Check if the ID exists
                bool checkID = await CheckID(dataSource, "areas", id);
                if (!checkID) return NotFound("No Area was found with the ID");

                // Fetch the object
                await using NpgsqlCommand get = dataSource.CreateCommand($"SELECT * FROM setr.areas WHERE id = {id}");
                await using NpgsqlDataReader rdr = await get.ExecuteReaderAsync();
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
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="updateArea"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]

        public async Task<ActionResult<Area>> UpdateArea(int id, [FromBody] Area updateArea)
        {
            try
            {
                await using NpgsqlDataSource dataSource = DatabaseConfig.Create();

                // Check if the ID exists
                bool checkID = await CheckID(dataSource, "areas", id);
                if (!checkID) return NotFound("No Area was found with the ID");

                // Update Area
                await using NpgsqlCommand update = dataSource.CreateCommand($"UPDATE setr.areas SET name = '{updateArea.Name}' WHERE id = {id} RETURNING *;");
                await using NpgsqlDataReader rdr = await update.ExecuteReaderAsync();
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
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]

        public async Task<ActionResult<Area>> DeleteArea(int id)
        {
            try
            {
                await using NpgsqlDataSource dataSource = DatabaseConfig.Create();

                // Check if the ID exists
                bool checkID = await CheckID(dataSource, "areas", id);
                if (!checkID) return NotFound("No Area was found with the ID");

                // Fetch the object
                await using NpgsqlCommand get = dataSource.CreateCommand($"SELECT * FROM setr.areas WHERE id = {id}");
                await using NpgsqlDataReader rdr = await get.ExecuteReaderAsync();
                await rdr.ReadAsync();
                Area area = new(rdr.GetInt32(0), rdr.GetString(1));

                // Update Room
                await using NpgsqlCommand delete = dataSource.CreateCommand($"DELETE FROM setr.users_areas WHERE id_areas = {id};DELETE FROM setr.areas WHERE id = {id};");
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