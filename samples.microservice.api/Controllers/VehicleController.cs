using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using samples.microservice.core;
using samples.microservice.entities;
using Microsoft.Extensions.Logging;

namespace samples.microservice.api.Controllers
{
    public class VehicleController: Controller
    {
        private IConfiguration _configuration;
        private readonly ILogger _logger;
        private readonly IRepository _repository;

        /// <summary>
        /// Controller representing communication with Cosmos Db
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="repository"></param>
        /// <param name="loggerFactory"></param>
        public VehicleController(IConfiguration configuration,  IRepository repository, ILoggerFactory loggerFactory)
        {
            _configuration = configuration;
            _repository = repository;
            _logger = loggerFactory.CreateLogger<VehicleController>();
            _logger.LogInformation($"Entering Cosmos Controller");
        }

        // GET
        [HttpGet("/vehicle")]
        public async Task<List<VehicleData>> GetAsync()
        {
            try
            {
                return await _repository.ReadAsync<VehicleData>();
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Error in Get {e.Message}");
                throw;
            }
        }

        [HttpGet("/vehicle/{id}")]
        public async Task<VehicleData> GetAsync(string id)
        {
            try
            {
                return await _repository.ReadSingularAsync<VehicleData>(id);
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Error in Get {e.Message}");
                throw;
            }
        }

        [HttpPost ("/vehicle")]
        public async Task<bool> Save([FromBody] VehicleData document)
        {
            try
            {
                return await _repository.SaveAsync(document);
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Error in Get {e.Message}");
                throw;
            }
        }
    }


}