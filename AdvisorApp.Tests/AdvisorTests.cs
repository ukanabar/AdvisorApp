using Xunit;
using Moq;
using Microsoft.AspNetCore.Mvc;


public class AdvisorsControllerTests
{
    private readonly AdvisorsController _controller;
    private readonly Mock<IAdvisorRepository> _mockRepo;
    private readonly MRUCache<int, Advisor> _cache;

    public AdvisorsControllerTests()
    {
        _mockRepo = new Mock<IAdvisorRepository>();
        _controller = new AdvisorsController(_mockRepo.Object);
        _cache = new MRUCache<int, Advisor>(5);
    }

    [Fact]
    public async Task GetAdvisors_ReturnsOkResult_WithListOfAdvisors()
    {
        // Arrange
        var advisors = new List<Advisor> { new Advisor { Id = 1, Name = "Advisor1" } };
        _mockRepo.Setup(repo => repo.GetAllAsync()).ReturnsAsync(advisors);

        // Act
        var result = await _controller.GetAdvisors();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnAdvisors = Assert.IsType<List<Advisor>>(okResult.Value);
        Assert.Single(returnAdvisors);
    }

    [Fact]
    public async Task GetAdvisor_ReturnsOkResult_WithAdvisor()
    {
        // Arrange
        var advisor = new Advisor { Id = 1, Name = "Advisor1" };
        _mockRepo.Setup(repo => repo.GetByIdAsync(1)).ReturnsAsync(advisor);

        // Act
        var result = await _controller.GetAdvisor(1);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnAdvisor = Assert.IsType<Advisor>(okResult.Value);
        Assert.Equal(1, returnAdvisor.Id);
    }

    [Fact]
    public async Task GetAdvisor_ReturnsNotFound_WhenAdvisorDoesNotExist()
    {
        // Arrange
        _mockRepo.Setup(repo => repo.GetByIdAsync(1)).ReturnsAsync((Advisor)null);

        // Act
        var result = await _controller.GetAdvisor(1);

        // Assert
        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task PostAdvisor_ReturnsCreatedAtAction_WithAdvisor()
    {
        // Arrange
        var advisor = new Advisor { Id = 1, Name = "Advisor1" };
        _mockRepo.Setup(repo => repo.CreateAsync(advisor)).ReturnsAsync(advisor);

        // Act
        var result = await _controller.PostAdvisor(advisor);

        // Assert
        var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result.Result);
        var returnAdvisor = Assert.IsType<Advisor>(createdAtActionResult.Value);
        Assert.Equal(1, returnAdvisor.Id);
    }

    [Fact]
    public async Task PostAdvisor_ReturnsConflict_WhenAdvisorWithSameSinExists()
    {
        // Arrange
        var advisor = new Advisor { Id = 1, Name = "Advisor1" };
        _mockRepo.Setup(repo => repo.CreateAsync(advisor)).ThrowsAsync(new Exception("Advisor with this SIN already exists."));

        // Act
        var result = await _controller.PostAdvisor(advisor);

        // Assert
        var conflictResult = Assert.IsType<ConflictObjectResult>(result.Result);
        var returnMessage = conflictResult?.Value?.GetType();
        Assert.Equal("Advisor with this SIN already exists.", returnMessage.GetProperty("message").GetValue(conflictResult?.Value, null).ToString());
    }

    [Fact]
    public async Task PutAdvisor_ReturnsNoContent_WhenAdvisorIsUpdated()
    {
        // Arrange
        var advisor = new Advisor { Id = 1, Name = "Advisor1" };
        _mockRepo.Setup(repo => repo.UpdateAsync(advisor)).ReturnsAsync(advisor);

        // Act
        var result = await _controller.PutAdvisor(1, advisor);

        // Assert
        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task PutAdvisor_ReturnsBadRequest_WhenIdMismatch()
    {
        // Arrange
        var advisor = new Advisor { Id = 2, Name = "Advisor1" };

        // Act
        var result = await _controller.PutAdvisor(1, advisor);

        // Assert
        Assert.IsType<BadRequestResult>(result);
    }

    [Fact]
    public async Task DeleteAdvisor_ReturnsNoContent_WhenAdvisorIsDeleted()
    {
        // Arrange
        _mockRepo.Setup(repo => repo.DeleteAsync(1)).ReturnsAsync(true);

        // Act
        var result = await _controller.DeleteAdvisor(1);

        // Assert
        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task DeleteAdvisor_ReturnsNotFound_WhenAdvisorDoesNotExist()
    {
        // Arrange
        _mockRepo.Setup(repo => repo.DeleteAsync(1)).ReturnsAsync(false);

        // Act
        var result = await _controller.DeleteAdvisor(1);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }
}
