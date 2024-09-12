using P2PLoan.Models;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using P2PLoan.Interfaces.Services;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using P2PLoan.DTOs;

namespace P2PLoan.Controllers
{
    [Route("api/support")]
    [ApiController]
    public class SupportController : ControllerBase
    {
        private readonly IAIService _aiService;

        public SupportController(IAIService aiService)
        {
            _aiService = aiService;
        }

        /**
         * ChatWithBot - Chat with the AI chat bot
         * @param request - the chat request
         * @return a response containing the AI's chat response
         */
        [HttpPost]
        [Route("chat")]
        public async Task ChatWithBot([FromBody] ChatRequest request, CancellationToken cancellationToken)
        {
            var responseStream = Response.Body;
            bool hasResponse = false;

            try
            {
                await foreach (var chunk in _aiService.ChatWithAI(request, cancellationToken))
                {
                    hasResponse = true;
                    var chunkBytes = Encoding.UTF8.GetBytes(chunk);
                    await responseStream.WriteAsync(chunkBytes, 0, chunkBytes.Length, cancellationToken);
                    await responseStream.FlushAsync(cancellationToken);
                }

                if (!hasResponse)
                {
                    Response.StatusCode = StatusCodes.Status404NotFound;
                    var notFoundBytes = Encoding.UTF8.GetBytes("No response generated for the prompt.");
                    await responseStream.WriteAsync(notFoundBytes, 0, notFoundBytes.Length, cancellationToken);
                    await responseStream.FlushAsync(cancellationToken);
                }
            }
            catch (Exception ex)
            {
                Response.StatusCode = StatusCodes.Status500InternalServerError;
                var errorBytes = Encoding.UTF8.GetBytes("An error occurred while processing the request.");
                await responseStream.WriteAsync(errorBytes, 0, errorBytes.Length, cancellationToken);
                await responseStream.FlushAsync(cancellationToken);
            }
        }

    }

}