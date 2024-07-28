using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

[Route("api/[controller]")]
[ApiController]
public class AdvisorsController : ControllerBase
{
    private readonly IAdvisorRepository _repository;
    private readonly MRUCache<int, Advisor> _cache;

    public AdvisorsController(IAdvisorRepository repository)
    {
        _repository = repository;
        _cache = new MRUCache<int, Advisor>(5);
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Advisor>>> GetAdvisors()
    {
        var advisors = await _repository.GetAllAsync();
        return Ok(advisors);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Advisor>> GetAdvisor(int id)
    {
        var advisor = _cache.Get(id);
        if (advisor == null)
        {
            advisor = await _repository.GetByIdAsync(id);
            if (advisor == null)
            {
                return NotFound();
            }
            _cache.Put(id, advisor);
        }
        return Ok(advisor);
    }

    [HttpPost]
    public async Task<ActionResult<Advisor>> PostAdvisor(Advisor advisor)
    {
        try 
        {
            advisor.HealthStatus = GenerateHealthStatus();
            var createdAdvisor = await _repository.CreateAsync(advisor); 
            _cache.Put(createdAdvisor.Id, createdAdvisor); 
            return CreatedAtAction(nameof(GetAdvisor), new { id = createdAdvisor.Id }, createdAdvisor); 
        } 
        catch (Exception ex) 
        { 
            if (ex.Message.Contains("Advisor with this SIN already exists.")) 
            { 
                return Conflict(new { message = ex.Message }); 
            } 
            throw; 
        }      
        
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> PutAdvisor(int id, Advisor advisor)
    {
        if (id != advisor.Id)
        {
            return BadRequest();
        }

        try
        {
            var updatedAdvisor = await _repository.UpdateAsync(advisor);
            _cache.Put(id, updatedAdvisor);
            return NoContent();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!AdvisorExists(id))
            {
                return NotFound();
            }
            else
            {
                throw;
            }
        }
        catch (Exception ex)
        {
            if (ex.Message.Contains("Advisor with this SIN already exists."))
            {
                return Conflict(new { message = ex.Message });
            }
            throw;
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAdvisor(int id)
    {
        var success = await _repository.DeleteAsync(id);
        if (!success)
        {
            return NotFound();
        }

        _cache.Delete(id);
        return NoContent();
    }

    private bool AdvisorExists(int id)
    {
        return _repository.GetByIdAsync(id) != null;
    }

    private string GenerateHealthStatus()
    {
        var random = new Random();
        var roll = random.Next(0, 100);
        if (roll < 60) return "Green";
        if (roll < 80) return "Yellow";
        return "Red";
    }
}
