using GeoTaskManager.Api.Controllers;
using GeoTaskManager.Api.Core.Models;
using GeoTaskManager.Api.GeoTasks.Models;
using GeoTaskManager.Application.Core.Commands;
using GeoTaskManager.Application.Core.Queries;
using GeoTaskManager.Application.Core.Responses;
using GeoTaskManager.Application.GeoTasks.Commands;
using GeoTaskManager.Application.GeoTasks.Models;
using GeoTaskManager.Application.GeoTasks.Queries;
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
    public class TasksControllerTests
    {
        [Fact]
        public async Task GetIdReturnsOkResultWhenSuccessAnswerAsync()
        {
            // Arrange
            var entityId = "0000000000000000";
            var appEntity = new GeoTask()
            {
                Id = entityId,
                IsArchived = false,
                Title = "Test GeoTask"
            };
            var apiEntity = new ApiGeoTask()
            {
                Id = entityId,
                IsArchived = false,
                Title = "Test GeoTask"
            };
            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.Send(It.IsAny<EntityQuery<GeoTask>>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(
                    new EntityResponse<GeoTask>()
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
                (((OkObjectResult)controllerAnswer).Value as ApiGeoTask).Id);
            mediator.Verify(x => x.Send(It.IsAny<EntityQuery<GeoTask>>(),
                    It.IsAny<CancellationToken>()));
        }

        [Fact]
        public async Task GetIdReturnsBadResultWhenNotSuccessAnswerAsync()
        {
            // Arrange
            var entityId = "0000000000000000";
            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.Send(It.IsAny<EntityQuery<GeoTask>>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(
                    new EntityResponse<GeoTask>()
                    {
                        Success = false
                    })
                .Verifiable("Query was not sent.");
            var controller = BuildController(mediator);

            // Act
            var answer = await controller.Get(entityId);

            // Assert
            Assert.IsType<BadRequestResult>(answer);
            mediator.Verify(x => x.Send(It.IsAny<EntityQuery<GeoTask>>(),
                    It.IsAny<CancellationToken>()));
        }

        [Fact]
        public async Task GetListReturnsOkResultWhenSuccessAnswerAsync()
        {
            // Arrange
            var appEntities = new List<GeoTask>();
            var apiEntities = new List<ApiGeoTask>();
            appEntities.AddRange(Enumerable.Range(0, 5)
                .Select(x => new GeoTask()
                {
                    Id = x.ToString()
                }));
            var query = new ApiGeoTaskListQuery();
            apiEntities.AddRange(Enumerable.Range(0, 5)
                .Select(x => new ApiGeoTask()
                {
                    Id = x.ToString()
                }));
            var expectedAnswer = new ApiList<ApiGeoTask>()
            {
                TotalCount = apiEntities.Count
            };
            expectedAnswer.Entities.AddRange(apiEntities);
            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.Send(It.IsAny<GeoTaskListQuery>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(
                    new ListResponse<GeoTask>(appEntities, appEntities.Count))
                .Verifiable("Query was not sent.");
            var controller = BuildController(mediator);

            // Act
            var answer = await controller.Get(query);

            // Assert
            Assert.IsType<OkObjectResult>(answer);
            Assert.Equal(expectedAnswer.Entities.Select(x => x.Id),
                (((OkObjectResult)answer).Value as ApiList<ApiGeoTask>)
                    .Entities.Select(x => x.Id));
            mediator.Verify(x => x.Send(It.IsAny<GeoTaskListQuery>(),
                    It.IsAny<CancellationToken>()));
        }

        [Fact]
        public async Task GetListReturnsBadResultWhenNotSuccessAnswerAsync()
        {
            // Arrange
            var query = new ApiGeoTaskListQuery();
            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.Send(It.IsAny<GeoTaskListQuery>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(
                    new ListResponse<GeoTask>(new string[] { "Error answer" }))
                .Verifiable("Query was not sent.");
            var controller = BuildController(mediator);

            // Act
            var answer = await controller.Get(query);

            // Assert
            Assert.IsType<BadRequestResult>(answer);
            mediator.Verify(x => x.Send(It.IsAny<GeoTaskListQuery>(),
                    It.IsAny<CancellationToken>()));
        }

        [Fact]
        public async Task CreateReturnsBadResultWhenNotSuccessAnswerAsync()
        {
            // Arrange
            var query = new ApiGeoTaskCreateCommand()
            {
                Title = "Test GeoTask"
            };
            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.Send(It.IsAny<GeoTaskCreateCommand>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(
                    new CreateResult(new string[] { "Error" }))
                .Verifiable("Query was not sent.");
            var controller = BuildController(mediator);

            // Act
            var answer = await controller.Create(query);

            // Assert
            Assert.IsType<BadRequestResult>(answer);
            mediator.Verify(x => x.Send(It.IsAny<GeoTaskCreateCommand>(),
                    It.IsAny<CancellationToken>()));
        }

        [Fact]
        public async Task CreateReturnsCreatedAtActionWhenSuccessAnswerAsync()
        {
            // Arrange
            var query = new ApiGeoTaskCreateCommand()
            {
                Title = "Test GeoTask"
            };
            var newId = "0000000000000000";
            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.Send(It.IsAny<GeoTaskCreateCommand>(),
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
            mediator.Verify(x => x.Send(It.IsAny<GeoTaskCreateCommand>(),
                    It.IsAny<CancellationToken>()));
        }

        [Fact]
        public async Task UpdateReturnsOkResultWhenSuccessAnswerAsync()
        {
            // Arrange
            var apiQuery = new ApiGeoTaskUpdateCommand()
            {
                Title = "Test GeoTask"
            };
            var id = "0000000000000000";
            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.Send(It.IsAny<GeoTaskUpdateCommand>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(
                    new UpdateResult(success: true))
                .Verifiable("Query was not sent.");
            var controller = BuildController(mediator);

            // Act
            var appAnswer = await controller.Update(id, apiQuery);

            // Assert
            Assert.IsType<OkResult>(appAnswer);
            mediator.Verify(x => x.Send(It.IsAny<GeoTaskUpdateCommand>(),
                    It.IsAny<CancellationToken>()));
        }

        [Fact]
        public async Task UpdateReturnsBadResultWhenNotSuccessAnswerAsync()
        {
            // Arrange
            var apiQuery = new ApiGeoTaskUpdateCommand()
            {
                Title = "Test GeoTask",
            };
            var id = "0000000000000000";
            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.Send(It.IsAny<GeoTaskUpdateCommand>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(
                    new UpdateResult(error: new string[] { "Error" }))
                .Verifiable("Query was not sent.");
            var controller = BuildController(mediator);

            // Act
            var appAnswer = await controller.Update(id, apiQuery);

            // Assert
            Assert.IsType<BadRequestResult>(appAnswer);
            mediator.Verify(x => x.Send(It.IsAny<GeoTaskUpdateCommand>(),
                    It.IsAny<CancellationToken>()));
        }

        [Fact]
        public async Task DeleteReturnsBadResultWhenNotSuccessAnswerAsync()
        {
            // Arrange
            var id = "0000000000000000";
            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.Send(It.IsAny<DeleteCommand<GeoTask>>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(
                    new DeleteResult(errors: new string[] { "Error" }))
                .Verifiable("Query was not sent.");
            var controller = BuildController(mediator);

            // Act
            var appAnswer = await controller.Delete(id);

            // Assert
            Assert.IsType<BadRequestResult>(appAnswer);
            mediator.Verify(x => x.Send(It.IsAny<DeleteCommand<GeoTask>>(),
                    It.IsAny<CancellationToken>()));
        }

        [Fact]
        public async Task DeleteReturnsOkResultWhenSuccessAnswerAsync()
        {
            // Arrange
            var id = "0000000000000000";
            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.Send(It.IsAny<DeleteCommand<GeoTask>>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(
                    new DeleteResult(success: true))
                .Verifiable("Query was not sent.");
            var controller = BuildController(mediator);

            // Act
            var appAnswer = await controller.Delete(id);

            // Assert
            Assert.IsType<OkResult>(appAnswer);
            mediator.Verify(x => x.Send(It.IsAny<DeleteCommand<GeoTask>>(),
                    It.IsAny<CancellationToken>()));
        }

        private static TasksController BuildController(Mock<IMediator> mediator)
        {
            var logger = new Mock<ILogger<TasksController>>();
            var controller = new TasksController(mediator.Object,
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