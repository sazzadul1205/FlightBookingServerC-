using FlightBooking.Models;
using FlightBooking.Services;
using Microsoft.AspNetCore.Mvc;

namespace FlightBooking.Controllers;

[ApiController]
[Route("[controller]")]
public class UserController : ControllerBase
{
    public UserController()
    {
    }

    // GET all action
    [HttpGet]
    public ActionResult<List<object>> GetAll()
    {
        return Ok(UserServices.GetAllSafe());
    }

    // GET by Id action
    [HttpGet("{id}")]
    public ActionResult<object> Get(int id)
    {
        var user = UserServices.GetSafeUser(id);

        if (user == null)
            return NotFound();

        return Ok(user);
    }

    // GET by Email action
    [HttpGet("email/{email}")]
    public ActionResult<object> GetByEmail(string email)
    {
        var user = UserServices.FindByEmail(email);

        if (user == null)
            return NotFound();

        // Return safe user data (no password)
        return Ok(new
        {
            user.Id,
            user.Username,
            user.Email,
            user.CreatedAt,
            user.UpdatedAt
        });
    }

    // POST Register action
    [HttpPost("register")]
    public ActionResult<object> Register(User user)
    {
        try
        {
            UserServices.Add(user);

            // Return safe user data (no password)
            return CreatedAtAction(nameof(Get), new { id = user.Id }, new
            {
                user.Id,
                user.Username,
                user.Email,
                user.CreatedAt,
                user.UpdatedAt
            });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { error = ex.Message });
        }
    }

    // POST Login action
    [HttpPost("login")]
    public ActionResult<object> Login([FromForm] string email, [FromForm] string password)
    {
        var user = UserServices.Authenticate(email, password);

        if (user == null)
            return Unauthorized(new { error = "Invalid email or password" });

        // Return safe user data (no password)
        return Ok(new
        {
            user.Id,
            user.Username,
            user.Email,
            user.CreatedAt,
            user.UpdatedAt
        });
    }

    // PUT action
    [HttpPut("{id}")]
    public ActionResult<object> Update(int id, User user)
    {
        if (id != user.Id)
            return BadRequest(new { error = "ID mismatch" });

        try
        {
            UserServices.Update(user);

            // Return the updated user without password
            var updatedUser = UserServices.GetSafeUser(id);
            return Ok(updatedUser);
        }
        catch (KeyNotFoundException)
        {
            return NotFound(new { error = $"User with ID {id} not found" });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    // DELETE action
    [HttpDelete("{id}")]
    public ActionResult Delete(int id)
    {
        try
        {
            UserServices.Delete(id);
            return Ok(new { message = $"User with ID {id} deleted successfully" });
        }
        catch (KeyNotFoundException)
        {
            return NotFound(new { error = $"User with ID {id} not found" });
        }
    }
}