using System.Collections.Generic;
using System.Threading.Tasks;

public interface IAdvisorRepository
{
    Task<IEnumerable<Advisor>> GetAllAsync();
    Task<Advisor> GetByIdAsync(int id);
    Task<Advisor> CreateAsync(Advisor advisor);
    Task<Advisor> UpdateAsync(Advisor advisor);
    Task<bool> DeleteAsync(int id);
}
