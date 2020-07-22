using GeoTaskManager.Api.Actors.Mappers;
using GeoTaskManager.Api.Actors.Models;
using GeoTaskManager.Api.Core.Logging;
using GeoTaskManager.Api.Core.Models;
using GeoTaskManager.Application.Actors.Models;
using GeoTaskManager.Application.Core.Commands;
using GeoTaskManager.Application.Core.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Threading.Tasks;

namespace GeoTaskManager.Api.Controllers
{
    /// <summary>
    /// Actors controller
    /// </summary>
    [ApiController]
    [Route("[controller]/[action]")]
    [Authorize(Policy = "EmailVerified")]
    [SwaggerTag("Create, read, update and delete Actors")]
    public class ActorsController : ControllerBase
    {
        private IMediator Mediator { get; }
        private ILogger<ActorsController> Logger { get; set; }

        /// <summary>
        /// Actors API controller
        /// </summary>
        /// <param name="mediator">IMediator class instance</param>
        /// <param name="logger">ILogger class instance</param>
        public ActorsController(IMediator mediator, ILogger<ActorsController> logger)
        {
            Mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            Logger = logger;
        }

        /// <summary>
        /// Get specific Actor
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///     GET /actors/get/1234567890123456789012345
        /// </remarks>
        /// <param name="id">Id of the entity</param>
        [HttpGet("{id}")]
        [SwaggerResponse(200, Type = typeof(ApiActor), Description = "Return answer")]
        [SwaggerResponse(400, Description = "Not found or error")]
        [SwaggerResponse(401, Description = "Unauthorized")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Globalization", "CA1303:Do not pass literals as localized parameters", Justification = "<Pending>")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Reliability", "CA2007:Consider calling ConfigureAwait on the awaited task", Justification = "<Pending>")]
        public async Task<IActionResult> Get([FromRoute] string id)
        {
            try
            {
                var currentPrincipal = HttpContext.User;

                using var logScope = Logger.BeginScope("{User}", currentPrincipal?.Identity?.Name);
                Logger.LogInformation(ApiLogEvent.ApiRequest, "Get Actor request {id}", id);

                var query = new EntityQuery<Actor>(id, currentPrincipal);
                var result = await Mediator.Send(query);

                if (result is null || !result.Success)
                {
                    Logger.LogWarning(ApiLogEvent.ApiErrorResponse, "Get Actor error response. Errors={errors}", result?.Errors);
                    return BadRequest();
                }
                return Ok(result.Entity.ToApiActor());
            }
            catch (Exception ex)
                when (Logger.WriteScopeWhenException(ApiLogEvent.ApiErrorResponse, ex))
            {
                return BadRequest();
            }
        }

        /// <summary>
        /// Gets filtered list of Actors
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///     GET /actors/get?[parameters]
        /// 
        /// <param name="request">
        /// Available parameters:
        /// 
        /// Offset (integer) - number of skipping element from the beginning of list
        /// 
        /// Limit (integer) - maximum number of returning elements. It could be limited by API service
        /// 
        /// Archived (bool) - true returns only archived entities, false - only not archived entities, null - both archived and not archived entities
        /// 
        /// ActorRoleMask (integer) - filter entities by Actor's global role.
        /// 
        ///     0 - any role;
        ///     1 - Admin;
        ///     2 - Manager;
        ///     4 - Actor;
        ///     8 - Observer;
        ///     or sum of several possible roles, for example, 6 for Manager or Actor role
        ///     
        /// ContainsKeyWords (string) - returns entities containing one 
        ///     or several words from the parameter in the Title, Description, 
        ///     FirstName, LastName, Department, Login
        /// </param>
        /// </remarks>
        [HttpGet]
        [SwaggerResponse(200, Type = typeof(ApiList<ApiActor>),
            Description = "Return list of entities and total count " +
            "of entities without applying Limit and Offset parameters")]
        [SwaggerResponse(400, Description = "Error")]
        [SwaggerResponse(401, Description = "Unauthorized")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Globalization",
            "CA1303:Do not pass literals as localized parameters",
            Justification = "<Pending>")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Reliability",
            "CA2007:Consider calling ConfigureAwait on the awaited task",
            Justification = "<Pending>")]
        public async Task<IActionResult> Get
            ([FromQuery] ApiActorListQuery request)
        {
            try
            {
                var currentPrincipal = HttpContext.User;
                using var logScope = Logger.BeginScope("{User}",
                    currentPrincipal?.Identity?.Name);
                Logger.LogInformation(ApiLogEvent.ApiRequest,
                    "Get Actor List request {query}", request.ToDictionary());

                var query = request.ToActorListQuery(currentPrincipal);
                var result = await Mediator.Send(query);
                if (result is null || !result.Success)
                {
                    Logger.LogWarning(ApiLogEvent.ApiErrorResponse,
                        "Get Actors List error response. Errors={errors}",
                        result?.Errors);
                    return BadRequest();
                }
                return Ok(result.ToApiActorList());
            }
            catch (Exception ex)
                when (Logger.WriteScopeWhenException
                    (ApiLogEvent.ApiErrorResponse, ex))
            {
                return BadRequest();
            }
        }


        /// <summary>
        /// Add new Actor object
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///     POST /actors/create
        ///     {
        ///         "Login": "Login identity from authentication service",
        ///         "Title": "Title",
        ///         "IsArchived": false,
        ///         "Description": "description",
        ///         "Department": "department",
        ///         "FirstName": "First Name",
        ///         "LastName": "Last Name",
        ///         "Phone": "+71112223344",
        ///         "Email": "user@example.com",
        ///         "Skype": "artimon77",
        ///         "Role": 1
        ///     }
        /// </remarks>
        /// <param name="actor">New entity</param>
        /// <returns>Id (string) of created entity</returns>
        [HttpPost]
        [SwaggerResponse(201,
            Description = "Id of created entity and link to get entity")]
        [SwaggerResponse(400, Description = "Error")]
        [SwaggerResponse(401, Description = "Unauthorized")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Globalization",
            "CA1303:Do not pass literals as localized parameters",
            Justification = "<Pending>")]
        public async Task<IActionResult> Create
            ([FromBody] ApiActorCreateCommand actor)
        {
            try
            {
                var currentPrincipal = HttpContext.User;
                using var logScope = Logger.BeginScope("{User}",
                    currentPrincipal?.Identity?.Name);
                Logger.LogInformation(ApiLogEvent.ApiRequest,
                    "Get Actor Create command {command}",
                    actor.ToDictionary());

                var query = actor.ToActorCreateCommand(currentPrincipal);
                var result = await Mediator.Send(query).ConfigureAwait(false);
                if (result is null || !result.Success)
                {
                    Logger.LogWarning(ApiLogEvent.ApiErrorResponse,
                        "Actor Create error response. Errors={errors}",
                        result?.Errors);
                    return BadRequest();
                }
                return CreatedAtAction(nameof(Get),
                    new { id = result.Id }, result.Id);
            }
            catch (Exception ex)
                when (Logger.WriteScopeWhenException
                (ApiLogEvent.ApiErrorResponse, ex))
            {
                return BadRequest();
            }
        }


        /// <summary>
        /// Update Actor
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///     POST /actors/update/888888888888888888888888
        ///     {
        ///         "Login": "Login identity from authentication service",
        ///         "Title": "Title",
        ///         "IsArchived": false,
        ///         "Description": "description",
        ///         "Department": "department",
        ///         "FirstName": "First Name",
        ///         "LastName": "Last Name",
        ///         "Phone": "+71112223344",
        ///         "Email": "user@example.com",
        ///         "Skype": "artimon77",
        ///         "Role": 1
        ///     }
        /// </remarks>
        /// <param name="id">Id of the entity</param>
        /// <param name="actor">Updated Actor object</param>
        [HttpPut("{id}")]
        [SwaggerResponse(200, Description = "OK")]
        [SwaggerResponse(400, Description = "Error")]
        [SwaggerResponse(401, Description = "Unauthorized")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Globalization",
            "CA1303:Do not pass literals as localized parameters",
            Justification = "<Pending>")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Reliability",
            "CA2007:Consider calling ConfigureAwait on the awaited task",
            Justification = "<Pending>")]
        public async Task<IActionResult> Update([FromRoute] string id,
            [FromBody] ApiActorUpdateCommand actor)
        {
            try
            {
                var currentPrincipal = HttpContext.User;
                var currentUserName = currentPrincipal?.Identity?.Name;
                using var logScope = Logger.BeginScope("{User}",
                    currentUserName);
                Logger.LogInformation(ApiLogEvent.ApiRequest,
                    "Actor update command. Id={id}. Command={command}", id,
                    actor.ToDictionary());

                if (actor is null || String.IsNullOrWhiteSpace(id))
                {
                    Logger.LogWarning(ApiLogEvent.ApiArgumentError,
                        "Empty Actor Create Command.");
                    return BadRequest();
                }

                var command = actor.ToActorUpdateCommand(id, currentPrincipal);
                var result = await Mediator.Send(command);
                if (result is null || !result.Success)
                {
                    Logger.LogWarning(ApiLogEvent.ApiErrorResponse,
                        "Actor Update error response. Error={Error}.",
                        result?.Errors);
                    return BadRequest();
                }
                return Ok();
            }
            catch (Exception ex)
                when (Logger.WriteScopeWhenException
                    (ApiLogEvent.ApiErrorResponse, ex))
            {
                return BadRequest();
            }
        }

        /// <summary>
        /// Delete Actor
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///     DELETE /actors/888888888888888888888888
        /// </remarks>
        /// <param name="id">Id of the deleting entity</param>
        /// <param name="hardMode">Use hard mode to delete. true - completely delete entity, false - mark entity as archived. Default value is false</param>
        /// <param name="commitTitle">Title of the message adding to the history when delete in soft mode</param>
        /// <param name="commitDescription">Message adding to the history when delete in soft mode</param>
        [HttpDelete("{id}")]
        [SwaggerResponse(200, Description = "OK")]
        [SwaggerResponse(400, Description = "Error")]
        [SwaggerResponse(401, Description = "Unauthorized")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Globalization",
            "CA1303:Do not pass literals as localized parameters",
            Justification = "<Pending>")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Reliability",
            "CA2007:Consider calling ConfigureAwait on the awaited task",
            Justification = "<Pending>")]
        public async Task<IActionResult> Delete([FromRoute] string id,
            [FromQuery] bool hardMode = false,
            [FromQuery] string commitTitle = null,
            [FromQuery] string commitDescription = null)
        {
            try
            {
                var currentPrincipal = HttpContext.User;
                var currentUserName = currentPrincipal?.Identity?.Name;
                using var logScope = Logger.BeginScope("{User}",
                    currentUserName);
                Logger.LogInformation(ApiLogEvent.ApiRequest,
                    "Actor Delete Command. Id={Id}. HardMode={HardMode}",
                    id, hardMode);

                if (String.IsNullOrWhiteSpace(id))
                {
                    Logger.LogWarning(ApiLogEvent.ApiArgumentError,
                        "Actor Delete Command empty id.");
                    return BadRequest();
                }

                var command = new DeleteCommand<Actor>
                {
                    Id = id,
                    HardMode = hardMode,
                    CurrentPrincipal = currentPrincipal,
                    MessageTitle = commitTitle,
                    MessageDescription = commitDescription
                };
                var result = await Mediator.Send(command);
                if (result is null || !result.Success)
                {
                    Logger.LogWarning(ApiLogEvent.ApiErrorResponse,
                        "Actor Delete Command error response. Error={Error}.",
                        result?.Errors);
                    return BadRequest();
                }
                return Ok();
            }
            catch (Exception ex)
                when (Logger.WriteScopeWhenException
                    (ApiLogEvent.ApiErrorResponse, ex))
            {
                return BadRequest();
            }
        }
    }
}
