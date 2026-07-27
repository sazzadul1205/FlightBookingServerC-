using FlightBooking.Models;
using FlightBooking.Services;
using Microsoft.AspNetCore.Mvc;

namespace FlightBooking.Controllers;

[ApiController]
[Route("[controller]s")]
public class MarkupCommissionController : ControllerBase
{
    public MarkupCommissionController()
    {
    }

    // GET all action
    [HttpGet]
    public ActionResult<List<MarkupCommission>> GetAll()
    {
        return Ok(MarkupCommissionServices.GetAll());
    }

    // GET by Id action
    [HttpGet("{id}")]
    public ActionResult<MarkupCommission> Get(int id)
    {
        var markupCommission = MarkupCommissionServices.Get(id);
        if (markupCommission == null)
            return NotFound();
        return Ok(markupCommission);
    }

    // POST action
    [HttpPost]
    public ActionResult<MarkupCommission> Create([FromBody] MarkupCommission markupCommission)
    {
        try
        {
            MarkupCommissionServices.Add(markupCommission);
            return CreatedAtAction(nameof(Get), new { id = markupCommission.Id }, markupCommission);
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

    // PUT action - FIXED: Using DTO
    [HttpPut("{id}")]
    public ActionResult<MarkupCommission> Update(int id, [FromBody] MarkupCommissionUpdate updateDto)
    {
        try
        {
            MarkupCommissionServices.Update(id, updateDto);
            var updatedMarkupCommission = MarkupCommissionServices.Get(id);
            return Ok(updatedMarkupCommission);
        }
        catch (KeyNotFoundException)
        {
            return NotFound(new { error = $"MarkupCommission with ID {id} not found" });
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { error = ex.Message });
        }
    }

    // DELETE action
    [HttpDelete("{id}")]
    public ActionResult<object> Delete(int id)
    {
        try
        {
            MarkupCommissionServices.Delete(id);
            return Ok(new { message = $"MarkupCommission with ID {id} deleted successfully" });
        }
        catch (KeyNotFoundException)
        {
            return NotFound(new { error = $"MarkupCommission with ID {id} not found" });
        }
    }
}