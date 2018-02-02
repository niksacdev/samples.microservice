using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace samples.microservice.api.Controllers
{
    public class ErrorController : Controller
    {
        /// <summary>
        ///     Captures all errors on HTTP
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        [Route("error/{code}")]
        public async Task Error(int code)
        {
            await HttpContext.Response.WriteAsync($"An error occurred, Error code: {code}");
        }
    }
}