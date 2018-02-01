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
    [Route("/cosmos")]
    public class CosmosController: Controller
    {
        private IConfiguration _configuration;
        private ILogger _logger;
        private IRepository _repository;

        /// <summary>
        /// Controller representing communication with Cosmos Db
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="repository"></param>
        /// <param name="logger"></param>
        public CosmosController(IConfiguration configuration,  IRepository repository, ILogger logger)
        {
            _configuration = configuration;
            _repository = repository;
            _logger.LogInformation($"Entering Cosmos Controller");
        }

        // GET
        [HttpGet("/documents")]
        public async Task<List<MyDocument>> GetAsync()
        {
            try
            {
                return await _repository.ReadAsync<MyDocument>();
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Error in Get {e.Message}");
                throw;
            }
        }

        [HttpGet("/documents/{id}")]
        public async Task<MyDocument> GetAsync(string id)
        {
            try
            {
                return await _repository.ReadAsync<MyDocument>(id);
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Error in Get {e.Message}");
                throw;
            }
        }

        [HttpPost ("/documents")]
        public async Task<bool> Save([FromBody] MyDocument document)
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