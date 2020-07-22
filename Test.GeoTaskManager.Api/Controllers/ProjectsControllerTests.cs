using GeoTaskManager.Api.Controllers;
using GeoTaskManager.Api.Core.Models;
using GeoTaskManager.Api.Projects.Models;
using GeoTaskManager.Application.Core.Commands;
using GeoTaskManager.Application.Core.Queries;
using GeoTaskManager.Application.Core.Responses;
using GeoTaskManager.Application.Projects.Commands;
using GeoTaskManager.Application.Projects.Models;
using GeoTaskManager.Application.Projects.Queries;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Test.GeoTaskManager.Api.Controllers
{
    public class ProjectsControllerTests
    {
        [Fact]
        public async Task GetIdReturnsOkResultWhenSuccessAnswerAsync()
        {
            // Arrange
            var entityId = "0000000000000000";
            var appEntity = new Project()
            {
                Id = entityId,
                IsArchived = false,
                Title = "Test Project"
            };
            var apiEntity = new ApiProject()
            {
                Id = entityId,
                IsArchived = false,
                Title = "Test Project"
            };
            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.Send(It.IsAny<EntityQuery<Project>>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(
                    new EntityResponse<Project>()
                    {
                        Success = true,
                        Entity = appEntity
                    })
                .Verifiable("Query was not sent.");
            var controller = BuildController(mediator);

            // Act
            var controllerAnswer = await controller.Get(entityId);

            // Assert
            Assert.IsType<OkObjectResult>(controllerAnswer);
            Assert.Equal(apiEntity.Id,
                (((OkObjectResult)controllerAnswer).Value as ApiProject).Id);
            mediator.Verify(x => x.Send(It.IsAny<EntityQuery<Project>>(),
                    It.IsAny<CancellationToken>()));
        }

        [Fact]
        public async Task GetIdReturnsBadResultWhenNotSuccessAnswerAsync()
        {
            // Arrange
            var entityId = "0000000000000000";
            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.Send(It.IsAny<EntityQuery<Project>>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(
                    new EntityResponse<Project>()
                    {
                        Success = false
                    })
                .Verifiable("Query was not sent.");
            var controller = BuildController(mediator);

            // Act
            var answer = await controller.Get(entityId);

            // Assert
            Assert.IsType<BadRequestResult>(answer);
            mediator.Verify(x => x.Send(It.IsAny<EntityQuery<Project>>(),
                    It.IsAny<CancellationToken>()));
        }

        [Fact]
        public async Task GetListReturnsOkResultWhenSuccessAnswerAsync()
        {
            // Arrange
            var appEntities = new List<Project>();
            var apiEntities = new List<ApiProject>();
            appEntities.AddRange(Enumerable.Range(0, 5)
                .Select(x => new Project()
                {
                    Id = x.ToString()
                }));
            var query = new ApiProjectListQuery();
            apiEntities.AddRange(Enumerable.Range(0, 5)
                .Select(x => new ApiProject()
                {
                    Id = x.ToString()
                }));
            var expectedAnswer = new ApiList<ApiProject>()
            {
                TotalCount = apiEntities.Count
            };
            expectedAnswer.Entities.AddRange(apiEntities);
            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.Send(It.IsAny<ProjectListQuery>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(
                    new ListResponse<Project>(appEntities, appEntities.Count))
                .Verifiable("Query was not sent.");
            var controller = BuildController(mediator);

            // Act
            var answer = await controller.Get(query);

            // Assert
            Assert.IsType<OkObjectResult>(answer);
            Assert.Equal(expectedAnswer.Entities.Select(x => x.Id),
                (((OkObjectResult)answer).Value as ApiList<ApiProject>)
                    .Entities.Select(x => x.Id));
            mediator.Verify(x => x.Send(It.IsAny<ProjectListQuery>(),
                    It.IsAny<CancellationToken>()));
        }

        [Fact]
        public async Task GetListReturnsBadResultWhenNotSuccessAnswerAsync()
        {
            // Arrange
            var query = new ApiProjectListQuery();
            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.Send(It.IsAny<ProjectListQuery>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(
                    new ListResponse<Project>(new string[] { "Error answer" }))
                .Verifiable("Query was not sent.");
            var controller = BuildController(mediator);

            // Act
            var answer = await controller.Get(query);

            // Assert
            Assert.IsType<BadRequestResult>(answer);
            mediator.Verify(x => x.Send(It.IsAny<ProjectListQuery>(),
                    It.IsAny<CancellationToken>()));
        }

        [Fact]
        public async Task CreateReturnsBadResultWhenNotSuccessAnswerAsync()
        {
            // Arrange
            var query = new ApiProjectCreateCommand()
            {
                Title = "Test Project"
            };
            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.Send(It.IsAny<ProjectCreateCommand>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(
                    new CreateResult(new string[] { "Error" }))
                .Verifiable("Query was not sent.");
            var controller = BuildController(mediator);

            // Act
            var answer = await controller.Create(query);

            // Assert
            Assert.IsType<BadRequestResult>(answer);
            mediator.Verify(x => x.Send(It.IsAny<ProjectCreateCommand>(),
                    It.IsAny<CancellationToken>()));
        }

        [Fact]
        public async Task CreateReturnsCreatedAtActionWhenSuccessAnswerAsync()
        {
            // Arrange
            var query = new ApiProjectCreateCommand()
            {
                Title = "Test Project"
            };
            var newId = "0000000000000000";
            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.Send(It.IsAny<ProjectCreateCommand>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(
                    new CreateResult(newId))
                .Verifiable("Query was not sent.");
            var controller = BuildController(mediator);

            // Act
            var answer = await controller.Create(query);

            // Assert
            Assert.IsType<CreatedAtActionResult>(answer);
            Assert.Equal(newId, ((CreatedAtActionResult)answer).Value);
            mediator.Verify(x => x.Send(It.IsAny<ProjectCreateCommand>(),
                    It.IsAny<CancellationToken>()));
        }

        [Fact]
        public async Task UpdateReturnsOkResultWhenSuccessAnswerAsync()
        {
            // Arrange
            var apiQuery = new ApiProjectUpdateCommand()
            {
                Title = "Test Project"
            };
            var id = "0000000000000000";
            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.Send(It.IsAny<ProjectUpdateCommand>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(
                    new UpdateResult(success: true))
                .Verifiable("Query was not sent.");
            var controller = BuildController(mediator);

            // Act
            var appAnswer = await controller.Update(id, apiQuery);

            // Assert
            Assert.IsType<OkResult>(appAnswer);
            mediator.Verify(x => x.Send(It.IsAny<ProjectUpdateCommand>(),
                    It.IsAny<CancellationToken>()));
        }

        [Fact]
        public async Task UpdateReturnsBadResultWhenNotSuccessAnswerAsync()
        {
            // Arrange
            var apiQuery = new ApiProjectUpdateCommand()
            {
                Title = "Test Project",
            };
            var id = "0000000000000000";
            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.Send(It.IsAny<ProjectUpdateCommand>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(
                    new UpdateResult(error: new string[] { "Error" }))
                .Verifiable("Query was not sent.");
            var controller = BuildController(mediator);

            // Act
            var appAnswer = await controller.Update(id, apiQuery);

            // Assert
            Assert.IsType<BadRequestResult>(appAnswer);
            mediator.Verify(x => x.Send(It.IsAny<ProjectUpdateCommand>(),
                    It.IsAny<CancellationToken>()));
        }

        [Fact]
        public async Task DeleteReturnsBadResultWhenNotSuccessAnswerAsync()
        {
            // Arrange
            var id = "0000000000000000";
            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.Send(It.IsAny<DeleteCommand<Project>>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(
                    new DeleteResult(errors: new string[] { "Error" }))
                .Verifiable("Query was not sent.");
            var controller = BuildController(mediator);

            // Act
            var appAnswer = await controller.Delete(id);

            // Assert
            Assert.IsType<BadRequestResult>(appAnswer);
            mediator.Verify(x => x.Send(It.IsAny<DeleteCommand<Project>>(),
                    It.IsAny<CancellationToken>()));
        }

        [Fact]
        public async Task DeleteReturnsOkResultWhenSuccessAnswerAsync()
        {
            // Arrange
            var id = "0000000000000000";
            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.Send(It.IsAny<DeleteCommand<Project>>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(
                    new DeleteResult(success: true))
                .Verifiable("Query was not sent.");
            var controller = BuildController(mediator);

            // Act
            var appAnswer = await controller.Delete(id);

            // Assert
            Assert.IsType<OkResult>(appAnswer);
            mediator.Verify(x => x.Send(It.IsAny<DeleteCommand<Project>>(),
                    It.IsAny<CancellationToken>()));
        }

        private static ProjectsController BuildController(Mock<IMediator> mediator)
        {
            var logger = new Mock<ILogger<ProjectsController>>();
            var controller = new ProjectsController(mediator.Object,
                logger.Object)
            {
                ControllerContext = new ControllerContext()
            };
            controller.ControllerContext.HttpContext = new DefaultHttpContext
            {
                User = new ClaimsPrincipal(new ClaimsIdentity("",
                        "testUserName", "testUserRole"))
            };
            return controller;
        }
    }
}