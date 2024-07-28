using Microsoft.EntityFrameworkCore;

public class AdvisorRepository : IAdvisorRepository
{
    private readonly DataContext _context;

    public AdvisorRepository(DataContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Advisor>> GetAllAsync()
    {
        return await _context.Advisors.ToListAsync();
    }

    public async Task<Advisor> GetByIdAsync(int id)
    {
        return await _context.Advisors.FindAsync(id);
    }

    public async Task<Advisor> CreateAsync(Advisor advisor)
    {
        if (await _context.Advisors.AnyAsync(a => a.SIN == advisor.SIN)) 
        { 
            throw new Exception("Advisor with this SIN already exists."); 
        }
        _context.Advisors.Add(advisor);
        await _context.SaveChangesAsync();
        return advisor;
    }

    public async Task<Advisor> UpdateAsync(Advisor advisor)
    {
        if (await _context.Advisors.AnyAsync(a => a.Id != advisor.Id && a.SIN == advisor.SIN))
        {
            throw new Exception("Advisor with this SIN already exists.");
        }
        _context.Entry(advisor).State = EntityState.Modified;
        await _context.SaveChangesAsync();
        return advisor;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var advisor = await _context.Advisors.FindAsync(id);
        if (advisor == null)
        {
            return false;
        }

        _context.Advisors.Remove(advisor);
        await _context.SaveChangesAsync();
        return true;
    }
}
