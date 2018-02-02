using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using samples.microservice.core;
using samples.microservice.entities;

namespace samples.microservice.api.Controllers
{
    public class VehicleController : Controller
    {
        private readonly ILogger _logger;
        private readonly IRepository _repository;
        private IConfiguration _configuration;

        /// <summary>
        ///     Controller representing communication with Cosmos Db
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="repository"></param>
        /// <param name="loggerFactory"></param>
        public VehicleController(IConfiguration configuration, IRepository repository, ILoggerFactory loggerFactory)
        {
            _configuration = configuration;
            _repository = repository;
            _logger = loggerFactory.CreateLogger<VehicleController>();
            _logger.LogInformation($"Entering Vehicle Controller");
        }

        /// <summary>
        ///     API to get multiple documents, default count is 10
        /// </summary>
        /// <returns></returns>
        [HttpGet("/vehicle")]
        public async Task<List<VehicleData>> GetAsync(int maxcount = 10)
        {
            try
            {
                return await _repository.ReadAsync<VehicleData>();
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Error in Get {e.Message} details: {e}");
                throw;
            }
        }

        /// <summary>
        ///     API to get a specific document
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("/vehicle/{id}")]
        public async Task<VehicleData> GetAsync(string id)
        {
            try
            {
                return await _repository.ReadSingularAsync<VehicleData>(id);
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Error in Get {e.Message} details: {e}");
                throw;
            }
        }

        /// <summary>
        ///     Upsert API for handling Insert and Update operations
        /// </summary>
        /// <param name="document"></param>
        /// <returns></returns>
        [HttpPost("/vehicle")]
        [HttpPut("/vehicle")]
        public async Task<IActionResult> Save([FromBody] VehicleData document)
        {
            try
            {
                if (document == null) return BadRequest();
                var result = await _repository.SaveAsync(document);
                return Json(result);
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Error in Save {e.Message} details: {e}");
                throw;
            }
        }

        /// <summary>
        ///     API to delete a document using its identifier
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("/vehicle/{id}")]
        public async Task<IActionResult> Remove([FromRoute] string id)
        {
            try
            {
                if (id == null) return BadRequest();
                var result = await _repository.DeleteAsync<VehicleData>(id);
                return Json(result);
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Error in Remove {e.Message} details: {e}");
                throw;
            }
        }
    }
}