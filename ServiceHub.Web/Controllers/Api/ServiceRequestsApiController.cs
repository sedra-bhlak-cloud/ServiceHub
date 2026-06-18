using Microsoft.AspNetCore.Mvc;
using ServiceHub.Web.DTOs;
using ServiceHub.Web.Services; // <-- Importing your new Service Layer
using System.Security.Claims;

namespace ServiceHub.Web.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class ServiceRequestsApiController : ControllerBase
    {
        private readonly IServiceRequestService _requestService;

        // Dependency Injection handles passing the service here seamlessly
        public ServiceRequestsApiController(IServiceRequestService requestService)
        {
            _requestService = requestService;
        }

        // 1. GET: api/ServiceRequestsApi
        [HttpGet]
        public async Task<IActionResult> GetAllRequests()
        {
            var requests = await _requestService.GetAllRequestsAsync();
            return Ok(requests);
        }

        // 2. POST: api/ServiceRequestsApi
        [HttpPost]
        public async Task<IActionResult> CreateRequest([FromBody] ServiceRequestCreateDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Extract user info if authenticated, otherwise default to system-api
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "system-api";
            
            // Pass the execution down to the service layer
            var newId = await _requestService.CreateRequestAsync(dto, userId);

            return StatusCode(201, new { message = "Request created successfully!", id = newId });
        }

        // 3. GET: api/ServiceRequestsApi/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetRequestById(int id)
        {
            var request = await _requestService.GetRequestByIdAsync(id);
            if (request == null)
            {
                return NotFound(new { message = $"Request with ID {id} was not found." });
            }

            return Ok(request);
        }

        // 4. PUT: api/ServiceRequestsApi/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateRequest(int id, [FromBody] ServiceRequestUpdateDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var isUpdated = await _requestService.UpdateRequestAsync(id, dto);
            if (!isUpdated)
            {
                return NotFound(new { message = $"Request with ID {id} was not found." });
            }

            return Ok(new { message = "Request updated successfully!" });
        }

        // 5. DELETE: api/ServiceRequestsApi/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRequest(int id)
        {
            var isDeleted = await _requestService.DeleteRequestAsync(id);
            if (!isDeleted)
            {
                return NotFound(new { message = $"Request with ID {id} was not found." });
            }

            return Ok(new { message = "Request deleted successfully!" });
        }
    }
}