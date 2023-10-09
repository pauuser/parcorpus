using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Moq.EntityFrameworkCore;
using Parcorpus.Core.Exceptions;
using Parcorpus.Core.Interfaces;
using Parcorpus.Core.Models;
using Parcorpus.Core.Models.Enums;
using Parcorpus.DataAccess.Context;
using Parcorpus.DataAccess.Models;
using Parcorpus.DataAccess.Repositories;
using Parcorpus.UnitTests.Common.Factories.CoreModels;
using Parcorpus.UnitTests.Common.Factories.DbModels;
using Parcorpus.UnitTests.Common.Helpers;

namespace Parcorpus.UnitTests.DataAccess;

public class JobRepositoryUnitTests
{
    private readonly IJobRepository _jobRepository;
    private readonly Mock<ParcorpusDbContext> _context = new();

    public JobRepositoryUnitTests()
    {
        _jobRepository = new JobRepository(NullLogger<JobRepository>.Instance, _context.Object);
    }

    [Fact]
    public async Task AddJobOkTest()
    {
        // Arrange
        var job = JobFactory.Create();
        
        var internalJobs = new List<JobDbModel>();
        _context.Setup(s => s.Jobs.AddAsync(It.IsAny<JobDbModel>(), It.IsAny<CancellationToken>()))
            .Returns((JobDbModel job, CancellationToken _) =>
            {
                var entry = EntityHelper.GetMockEntityEntry(job);
                internalJobs.Add(entry.Entity);
                
                return ValueTask.FromResult(entry);
            });
        
        // Act
        var actualJob = await _jobRepository.AddJob(job);

        // Assert
        Assert.NotEmpty(internalJobs);
        Assert.Equal(job, actualJob);
    }
    
    [Fact]
    public async Task AddJobExceptionTest()
    {
        // Arrange
        var job = JobFactory.Create();
        
        _context.Setup(s => s.Jobs.AddAsync(It.IsAny<JobDbModel>(), It.IsAny<CancellationToken>()))
            .Throws<Exception>();
        
        // Act & Assert
        await Assert.ThrowsAsync<JobRepositoryException>(() => _jobRepository.AddJob(job));
    }

    [Fact]
    public async Task GetJobByIdOkTest()
    {
        // Arrange
        var jobId = Guid.NewGuid();
        var jobDbModel = JobDbModelFactory.Create(jobId: jobId);
        var expectedJob = JobFactory.Create(jobDbModel);

        _context.Setup<DbSet<JobDbModel>>(s => s.Jobs)
            .ReturnsDbSet(new List<JobDbModel> { jobDbModel });

        // Act
        var actualJob = await _jobRepository.GetJobById(jobId);

        // Assert
        Assert.Equal(expectedJob, actualJob);
    }
    
    [Fact]
    public async Task GetJobByIdNotFoundTest()
    {
        // Arrange
        var jobId = Guid.NewGuid();

        _context.Setup<DbSet<JobDbModel>>(s => s.Jobs)
            .ReturnsDbSet(new List<JobDbModel> { });

        // Act & Assert
        await Assert.ThrowsAsync<JobNotFoundException>(() => _jobRepository.GetJobById(jobId));
    }
    
    [Fact]
    public async Task GetJobByIdExceptionTest()
    {
        // Arrange
        var jobId = Guid.NewGuid();

        _context.Setup<DbSet<JobDbModel>>(s => s.Jobs)
            .Throws<Exception>();

        // Act & Assert
        await Assert.ThrowsAsync<JobRepositoryException>(() => _jobRepository.GetJobById(jobId));
    }
    
    [Theory]
    [InlineData(JobStatus.Finished)]
    [InlineData(JobStatus.Failed)]
    [InlineData(JobStatus.Aligning)]
    [InlineData(JobStatus.Uploaded)]
    [InlineData(JobStatus.Saving)]
    public async Task UpdateJobStatusOkTest(JobStatus status)
    {
        // Arrange
        var jobId = Guid.NewGuid();
        var jobDbModel = JobDbModelFactory.Create(jobId: jobId);

        _context.Setup<DbSet<JobDbModel>>(s => s.Jobs)
            .ReturnsDbSet(new List<JobDbModel> { jobDbModel });

        // Act
        var actualJob = await _jobRepository.UpdateStatus(jobId, status);

        // Assert
        Assert.Equal(status, actualJob.Status);
    }
    
    [Fact]
    public async Task UpdateJobStatusNotFoundTest()
    {
        // Arrange
        var jobId = Guid.NewGuid();

        _context.Setup<DbSet<JobDbModel>>(s => s.Jobs)
            .ReturnsDbSet(new List<JobDbModel> { });

        // Act & Assert
        await Assert.ThrowsAsync<JobNotFoundException>(() => _jobRepository.UpdateStatus(jobId, JobStatus.Aligning));
    }
    
    [Fact]
    public async Task UpdateJobStatusExceptionTest()
    {
        // Arrange
        var jobId = Guid.NewGuid();

        _context.Setup<DbSet<JobDbModel>>(s => s.Jobs)
            .Throws<Exception>();

        // Act & Assert
        await Assert.ThrowsAsync<JobRepositoryException>(() => _jobRepository.UpdateStatus(jobId, JobStatus.Aligning));
    }

    [Fact]
    public async Task GetUserJobsNoPaginationTest()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var numberObJobs = 15;

        var jobsDb = Enumerable.Range(1, numberObJobs).Select(_ => JobDbModelFactory.Create(userId: userId)).ToList();
        var expectedJobs = new Paged<ProgressJob>(pageNumber: 1,
            pageSize: jobsDb.Count,
            totalCount: jobsDb.Count,
            items: jobsDb.Select(JobFactory.Create).ToList());
        _context.Setup<DbSet<JobDbModel>>(s => s.Jobs)
            .ReturnsDbSet(jobsDb.AsQueryable());

        var paging = new PaginationParameters(pageNumber: null, pageSize: null);
        
        // Act
        var actualJobs = await _jobRepository.GetUserJobs(userId, paging);
        
        // Assert
        expectedJobs.Items = expectedJobs.Items.OrderBy(j => j.StartedTimeUtc).ToList();
        actualJobs.Items = actualJobs.Items.OrderBy(j => j.StartedTimeUtc).ToList();
        
        Assert.Equal(expectedJobs, actualJobs);
    }
    
    [Theory]
    [InlineData(1, 1)]
    [InlineData(1, 5)]
    [InlineData(1, 150)]
    [InlineData(5, 7)]
    [InlineData(14, 11)]
    public async Task GetUserJobsValidPaginationTest(int pageNumber, int pageSize)
    {
        // Arrange
        var userId = Guid.NewGuid();
        var numberObJobs = 150;

        var jobsDb = Enumerable.Range(1, numberObJobs).Select(_ => JobDbModelFactory.Create(userId: userId)).ToList();
        _context.Setup<DbSet<JobDbModel>>(s => s.Jobs)
            .ReturnsDbSet(jobsDb.AsQueryable());

        var paging = new PaginationParameters(pageNumber: pageNumber, pageSize: pageSize);
        var expectedJobs = new Paged<ProgressJob>(pageNumber: pageNumber,
            pageSize: pageSize,
            totalCount: jobsDb.Count,
            items: jobsDb.Select(JobFactory.Create)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList());
        
        // Act
        var actualJobs = await _jobRepository.GetUserJobs(userId, paging);
        
        // Assert
        Assert.Equal(expectedJobs, actualJobs);
    }

    [Theory]
    [InlineData(0, 1, 150)]
    [InlineData(15, 11, 150)]
    [InlineData(1, 1, 0)]
    public async Task GetUserJobsInvalidPagingTest(int pageNumber, int pageSize, int numberOfJobs)
    {
        // Arrange
        var userId = Guid.NewGuid();

        var jobsDb = Enumerable.Range(1, numberOfJobs).Select(_ => JobDbModelFactory.Create(userId: userId)).ToList();
        _context.Setup<DbSet<JobDbModel>>(s => s.Jobs)
            .ReturnsDbSet(jobsDb.AsQueryable());

        var paging = new PaginationParameters(pageNumber: pageNumber, pageSize: pageSize);
        
        // Act
        await Assert.ThrowsAsync<InvalidPagingException>(() => _jobRepository.GetUserJobs(userId, paging));
    }
}