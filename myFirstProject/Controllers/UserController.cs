using Microsoft.AspNetCore.Mvc;
using myFirstProject.Models;
using myFirstProject.Services;

namespace myFirstProject.Controllers
{
    [Route("api/users")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        // Inject IUserService via constructor
        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        public IActionResult GetAllUsers()
        {
            return Ok(_userService.GetAllUsers());
        }

        [HttpGet("{id}")]
        public IActionResult GetUserById(long id)
        {
            try
            {
                var user = _userService.GetUserById(id);
                return Ok(user);
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpGet("search")]
        public IActionResult GetUsersByName([FromQuery] string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return BadRequest("Name parameter is required");

            var results = _userService.GetUsersByName(name);
            return results.Count > 0 ? Ok(results) : NotFound("No matches found");
        }

        [HttpPost("update")]
        public IActionResult UpdateUserName([FromBody] UpdateUserRequest request) 
        {
            try
            {
                var updatedUser = _userService.UpdateUser(request); 
                return Ok(updatedUser);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteUser(long id)
        {
            try
            {
                _userService.DeleteUser(id); // Returns void
                return Ok("User deleted successfully");
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
        }
    }
}