using GeoTaskManager.Api.Actors.Models;
using GeoTaskManager.Api.Controllers;
using GeoTaskManager.Api.Core.Models;
using GeoTaskManager.Application.Actors.Commands;
using GeoTaskManager.Application.Actors.Models;
using GeoTaskManager.Application.Actors.Queries;
using GeoTaskManager.Application.Core.Commands;
using GeoTaskManager.Application.Core.Queries;
using GeoTaskManager.Application.Core.Responses;
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
    public class ActorsControllerTests
    {
        [Fact]
        public async Task GetIdReturnsOkResultWhenSuccessAnswerAsync()
        {
            // Arrange
            var entityId = "0000000000000000";
            Actor actor = new Actor()
            {
                Id = entityId
            };
            ApiActor apiActor = new ApiActor()
            {
                Id = entityId
            };
            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.Send(It.IsAny<EntityQuery<Actor>>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(
                    new EntityResponse<Actor>()
                    {
                        Success = true,
                        Entity = actor
                    })
                .Verifiable("Query was not sent.");
            ActorsController controller = BuildController(mediator);

            // Act
            var answer = await controller.Get(entityId);

            // Assert
            Assert.IsType<OkObjectResult>(answer);
            Assert.Equal(apiActor.Id,
                (((OkObjectResult)answer).Value as ApiActor).Id);
            mediator.Verify(x => x.Send(It.IsAny<EntityQuery<Actor>>(),
                    It.IsAny<CancellationToken>()));
        }

        [Fact]
        public async Task GetIdReturnsBadResultWhenNotSuccessAnswerAsync()
        {
            // Arrange
            var entityId = "0000000000000000";
            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.Send(It.IsAny<EntityQuery<Actor>>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(
                    new EntityResponse<Actor>()
                    {
                        Success = false
                    })
                .Verifiable("Query was not sent.");
            ActorsController controller = BuildController(mediator);

            // Act
            var answer = await controller.Get(entityId);

            // Assert
            Assert.IsType<BadRequestResult>(answer);
            mediator.Verify(x => x.Send(It.IsAny<EntityQuery<Actor>>(),
                    It.IsAny<CancellationToken>()));
        }

        [Fact]
        public async Task GetListReturnsOkResultWhenSuccessAnswerAsync()
        {
            // Arrange
            var actors = new List<Actor>();
            var apiActors = new List<ApiActor>();
            actors.AddRange(Enumerable.Range(0, 5)
                .Select(x => new Actor()
                {
                    Id = x.ToString()
                }));
            var query = new ApiActorListQuery();
            apiActors.AddRange(Enumerable.Range(0, 5)
                .Select(x => new ApiActor()
                {
                    Id = x.ToString()
                }));
            var expectedAnswer = new ApiList<ApiActor>()
            {
                TotalCount = apiActors.Count
            };
            expectedAnswer.Entities.AddRange(apiActors);
            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.Send(It.IsAny<ActorListQuery>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(
                    new ListResponse<Actor>(actors, actors.Count))
                .Verifiable("Query was not sent.");
            ActorsController controller = BuildController(mediator);

            // Act
            var answer = await controller.Get(query);

            // Assert
            Assert.IsType<OkObjectResult>(answer);
            Assert.Equal(expectedAnswer.Entities.Select(x => x.Id),
                (((OkObjectResult)answer).Value as ApiList<ApiActor>)
                    .Entities.Select(x => x.Id));
            mediator.Verify(x => x.Send(It.IsAny<ActorListQuery>(),
                    It.IsAny<CancellationToken>()));
        }

        [Fact]
        public async Task GetListReturnsBadResultWhenNotSuccessAnswerAsync()
        {
            // Arrange
            var query = new ApiActorListQuery();
            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.Send(It.IsAny<ActorListQuery>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(
                    new ListResponse<Actor>(new string[] { "Error answer" }))
                .Verifiable("Query was not sent.");
            ActorsController controller = BuildController(mediator);

            // Act
            var answer = await controller.Get(query);

            // Assert
            Assert.IsType<BadRequestResult>(answer);
            mediator.Verify(x => x.Send(It.IsAny<ActorListQuery>(),
                    It.IsAny<CancellationToken>()));
        }

        [Fact]
        public async Task CreateReturnsBadResultWhenNotSuccessAnswerAsync()
        {
            // Arrange
            var query = new ApiActorCreateCommand()
            {
                FirstName = "Test User",
                Login = "testuser@example.com",
                IsArchived = false,
                Role = 1
            };
            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.Send(It.IsAny<ActorCreateCommand>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(
                    new CreateResult(new string[] { "Error" }))
                .Verifiable("Query was not sent.");
            ActorsController controller = BuildController(mediator);

            // Act
            var answer = await controller.Create(query);

            // Assert
            Assert.IsType<BadRequestResult>(answer);
            mediator.Verify(x => x.Send(It.IsAny<ActorCreateCommand>(),
                    It.IsAny<CancellationToken>()));
        }

        [Fact]
        public async Task CreateReturnsCreatedAtActionWhenSuccessAnswerAsync()
        {
            // Arrange
            var query = new ApiActorCreateCommand()
            {
                FirstName = "Test User",
                Login = "testuser@example.com",
                IsArchived = false,
                Role = 1
            };
            var newId = "0000000000000000";
            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.Send(It.IsAny<ActorCreateCommand>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(
                    new CreateResult(newId))
                .Verifiable("Query was not sent.");
            ActorsController controller = BuildController(mediator);

            // Act
            var answer = await controller.Create(query);

            // Assert
            Assert.IsType<CreatedAtActionResult>(answer);
            Assert.Equal(newId, ((CreatedAtActionResult)answer).Value);
            mediator.Verify(x => x.Send(It.IsAny<ActorCreateCommand>(),
                    It.IsAny<CancellationToken>()));
        }

        [Fact]
        public async Task UpdateReturnsOkResultWhenSuccessAnswerAsync()
        {
            // Arrange
            var apiQuery = new ApiActorUpdateCommand()
            {
                FirstName = "Test User",
                Login = "testuser@example.com",
                IsArchived = false,
                Role = 1
            };
            var id = "0000000000000000";
            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.Send(It.IsAny<ActorUpdateCommand>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(
                    new UpdateResult(success: true))
                .Verifiable("Query was not sent.");
            ActorsController controller = BuildController(mediator);

            // Act
            var appAnswer = await controller.Update(id, apiQuery);

            // Assert
            Assert.IsType<OkResult>(appAnswer);
            mediator.Verify(x => x.Send(It.IsAny<ActorUpdateCommand>(),
                    It.IsAny<CancellationToken>()));
        }

        [Fact]
        public async Task UpdateReturnsBadResultWhenNotSuccessAnswerAsync()
        {
            // Arrange
            var apiQuery = new ApiActorUpdateCommand()
            {
                FirstName = "Test User",
                Login = "testuser@example.com",
                IsArchived = false,
                Role = 1
            };
            var id = "0000000000000000";
            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.Send(It.IsAny<ActorUpdateCommand>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(
                    new UpdateResult(error: new string[] { "Error" }))
                .Verifiable("Query was not sent.");
            ActorsController controller = BuildController(mediator);

            // Act
            var appAnswer = await controller.Update(id, apiQuery);

            // Assert
            Assert.IsType<BadRequestResult>(appAnswer);
            mediator.Verify(x => x.Send(It.IsAny<ActorUpdateCommand>(),
                    It.IsAny<CancellationToken>()));
        }

        [Fact]
        public async Task DeleteReturnsBadResultWhenNotSuccessAnswerAsync()
        {
            // Arrange
            var id = "0000000000000000";
            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.Send(It.IsAny<DeleteCommand<Actor>>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(
                    new DeleteResult(errors: new string[] { "Error" }))
                .Verifiable("Query was not sent.");
            ActorsController controller = BuildController(mediator);

            // Act
            var appAnswer = await controller.Delete(id);

            // Assert
            Assert.IsType<BadRequestResult>(appAnswer);
            mediator.Verify(x => x.Send(It.IsAny<DeleteCommand<Actor>>(),
                    It.IsAny<CancellationToken>()));
        }

        [Fact]
        public async Task DeleteReturnsOkResultWhenSuccessAnswerAsync()
        {
            // Arrange
            var id = "0000000000000000";
            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.Send(It.IsAny<DeleteCommand<Actor>>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(
                    new DeleteResult(success: true))
                .Verifiable("Query was not sent.");
            ActorsController controller = BuildController(mediator);

            // Act
            var appAnswer = await controller.Delete(id);

            // Assert
            Assert.IsType<OkResult>(appAnswer);
            mediator.Verify(x => x.Send(It.IsAny<DeleteCommand<Actor>>(),
                    It.IsAny<CancellationToken>()));
        }

        private static ActorsController BuildController(Mock<IMediator> mediator)
        {
            var logger = new Mock<ILogger<ActorsController>>();
            var controller = new ActorsController(mediator.Object,
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