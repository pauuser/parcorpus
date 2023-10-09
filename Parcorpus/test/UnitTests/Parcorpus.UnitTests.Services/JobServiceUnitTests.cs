using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Parcorpus.Core.Configuration;
using Parcorpus.Core.Exceptions;
using Parcorpus.Core.Interfaces;
using Parcorpus.Core.Models;
using Parcorpus.Services.JobService;
using Parcorpus.UnitTests.Common.Factories;
using Parcorpus.UnitTests.Common.Factories.CoreModels;
using Parcorpus.UnitTests.Common.Helpers;

namespace Parcorpus.UnitTests.Services;

public class JobServiceUnitTests
{
    private readonly IJobService _jobService;

    private readonly Mock<IQueueProducerService> _mockQueueProducerService = new();
    private readonly Mock<IJobRepository> _mockJobRepository = new();
    private readonly PagingConfiguration _pagingConfiguration;

    public JobServiceUnitTests()
    {
        var pagingOptions = ConfigurationHelper.InitConfiguration<PagingConfiguration>();
        _pagingConfiguration = pagingOptions.Value;

        _jobService = new JobService(_mockQueueProducerService.Object,
            _mockJobRepository.Object,
            pagingOptions,
            NullLogger<JobService>.Instance);
    }

    [Fact]
    public async Task GetJobByIdOkTest()
    {
        // Arrange
        var jobId = Guid.NewGuid();
        var job = JobFactory.Create(jobId: jobId);

        _mockJobRepository.Setup(s => s.GetJobById(jobId))
            .ReturnsAsync(job);

        // Act
        var actualJob = await _jobService.GetJobById(jobId);

        // Assert
        Assert.Equal(job, actualJob);
    }
    
    [Fact]
    public async Task GetJobByIdFailTest()
    {
        // Arrange
        var jobId = Guid.NewGuid();

        _mockJobRepository.Setup(s => s.GetJobById(jobId))
            .ThrowsAsync(new JobNotFoundException());

        // Act & Assert
        await Assert.ThrowsAsync<JobNotFoundException>(() => _jobService.GetJobById(jobId));
    }
    
    [Fact]
    public async Task GetUserJobsOkTest()
    {
        // Arrange
        var userId = Guid.NewGuid();

        var paging = new PaginationParameters(pageNumber: 1, pageSize: 3);
        
        var jobs = Enumerable.Range(1, paging.PageSize!.Value).Select(_ => JobFactory.Create(userId: userId)).ToList();
        var expectedPageJobs = new Paged<ProgressJob>(pageNumber: paging.PageNumber, pageSize: paging.PageSize,
            totalCount: 20, items: jobs);
        _mockJobRepository.Setup(s => s.GetUserJobs(userId, paging))
            .ReturnsAsync(expectedPageJobs);

        // Act
        var actualJobs = await _jobService.GetUserJobs(userId, paging);

        // Assert
        Assert.Equal(expectedPageJobs, actualJobs);
    }
    
    [Fact]
    public async Task GetUserJobsUnspecifiedPagingTest()
    {
        // Arrange
        var userId = Guid.NewGuid();

        var paging = new PaginationParameters(pageNumber: null, pageSize: null);
        
        var jobs = Enumerable.Range(1, 5).Select(_ => JobFactory.Create(userId: userId)).ToList();
        var expectedPageJobs = new Paged<ProgressJob>(pageNumber: 1, pageSize: 5,
            totalCount: 5, items: jobs);
        _mockJobRepository.Setup(s => s.GetUserJobs(userId, paging))
            .ReturnsAsync(expectedPageJobs);

        // Act
        var actualJobs = await _jobService.GetUserJobs(userId, paging);

        // Assert
        Assert.Equal(expectedPageJobs, actualJobs);
    }
    
    [Theory]
    [InlineData(1)]
    [InlineData(-1)]
    public async Task GetUserJobsInvalidPagingTest(int add)
    {
        // Arrange
        var userId = Guid.NewGuid();

        var pageSize = add > 0 ? _pagingConfiguration.MaxPageSize + add : _pagingConfiguration.MinPageSize + add;
        var paging = new PaginationParameters(pageNumber: 1, pageSize: pageSize);
        
        // Act & Assert
        await Assert.ThrowsAsync<InvalidPagingException>(() => _jobService.GetUserJobs(userId, paging));
    }

    [Fact]
    public async Task UploadTextOkTest()
    {
        // Arrange
        var userId = Guid.NewGuid();

        UploadJob? uploadJob = null;
        _mockJobRepository.Setup(s => s.AddJob(It.IsAny<ProgressJob>()))
            .Returns((ProgressJob job) => Task.FromResult(job));
        _mockQueueProducerService.Setup(s => s.SendMessage(It.IsAny<UploadJob>()))
            .Callback<UploadJob>(async (UploadJob job) => uploadJob = job);
        var text = BiTextFactory.Create();

        // Act
        var job = await _jobService.UploadText(userId, text);

        // Assert
        Assert.NotNull(job);
        Assert.NotEqual(Guid.Empty, job.JobId);
        Assert.Equal(userId, job.UserId);
        Assert.Equal(text, uploadJob!.BiText);
        Assert.Equal(userId, uploadJob.UserId);
        Assert.Equal(job.JobId, uploadJob.JobId);
    }
}