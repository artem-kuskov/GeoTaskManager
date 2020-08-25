using GeoTaskManager.Api.Core.Logging;
using GeoTaskManager.Api.Core.Models;
using GeoTaskManager.Api.Projects.Mappers;
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
using _ApiEntityCreateCommandType =
    GeoTaskManager.Api.Projects.Models.ApiProjectCreateCommand;
using _ApiEntityType = GeoTaskManager.Api.Projects.Models.ApiProject;
using _ApiEntityUpdateCommandType =
    GeoTaskManager.Api.Projects.Models.ApiProjectUpdateCommand;
using _ApiListQueryType =
    GeoTaskManager.Api.Projects.Models.ApiProjectListQuery;
using _EntityCreateCommandType =
    GeoTaskManager.Application.Projects.Commands.ProjectCreateCommand;
using _EntityType = GeoTaskManager.Application.Projects.Models.Project;
using _EntityUpdateCommandType =
    GeoTaskManager.Application.Projects.Commands.ProjectUpdateCommand;



namespace GeoTaskManager.Api.Controllers
{
    /// <summary>
    /// Projects controller
    /// </summary>
    [ApiController]
    [Route("[controller]/[action]")]
    [Authorize(Policy = "EmailVerified")]
    [SwaggerTag("Create, read, update and delete Projects")]
    public class ProjectsController : ControllerBase
    {
        private IMediator Mediator { get; }
        private ILogger<ProjectsController> Logger { get; set; }

        /// <summary>
        /// Project API controller
        /// </summary>
        /// <param name="mediator">IMediator class instance</param>
        /// <param name="logger">ILogger class instance</param>
        public ProjectsController(IMediator mediator,
            ILogger<ProjectsController> logger)
        {
            Mediator = mediator
                ?? throw new ArgumentNullException(nameof(mediator));
            Logger = logger;
        }

        /// <summary>
        /// Get specific Project
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///     GET /projects/get/5f3ab0a596464e955096ca86
        /// </remarks>
        /// <param name="id">Id of the entity</param>
        [HttpGet("{id}")]
        [SwaggerResponse(200, Type = typeof(_ApiEntityType),
            Description = "Return answer")]
        [SwaggerResponse(400, Description = "Not found or error")]
        [SwaggerResponse(401, Description = "Unauthorized")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Globalization",
            "CA1303:Do not pass literals as localized parameters",
            Justification = "<Pending>")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Reliability",
            "CA2007:Consider calling ConfigureAwait on the awaited task",
            Justification = "<Pending>")]
        public async Task<IActionResult> Get([FromRoute] string id)
        {
            try
            {
                var currentPrincipal = HttpContext.User;
                var currentUserName = currentPrincipal?.Identity?.Name;

                using var logScope = Logger.BeginScope("{User}",
                    currentUserName);
                Logger.LogInformation(ApiLogEvent.ApiRequest,
                    "Get project request {Id}", id);

                var query = new EntityQuery<_EntityType>(id, currentPrincipal);
                var result = await Mediator.Send(query);

                if (result is null || !result.Success)
                {
                    Logger.LogWarning(ApiLogEvent.ApiErrorResponse,
                        "Get project error response. Error={Error}.",
                        result?.Errors);
                    return BadRequest();
                }
                return Ok(result.Entity.ToEntity<_ApiEntityType>());
            }
            catch (Exception ex)
                when (Logger.WriteScopeWhenException
                    (ApiLogEvent.ApiErrorResponse, ex))
            {
                return BadRequest();
            }
        }

        /// <summary>
        /// Gets filtered list of Projects
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///     GET /projects/get?[parameters]
        /// 
        /// <param name="request">
        /// Available parameters:
        /// 
        /// Offset (integer) - number of skipping element 
        /// from the beginning of list
        /// 
        /// Limit (integer) - maximum number of returning elements. 
        /// It could be limited by API service
        /// 
        /// Archived (boolean) - true returns only archived entities, 
        /// false - only not archived entities, 
        /// null - both archived and not archived entities
        /// 
        /// ContainsKeyWords (string) - returns entities 
        /// containing one or several words from the parameter 
        /// in the Title or/and Description
        /// </param>
        /// </remarks>
        [HttpGet]
        [Authorize]
        [SwaggerResponse(200, Type = typeof(ApiList<_ApiEntityType>),
            Description = "Return the list of entities " +
            "and total count of entities " +
            "without applying Limit and Offset parameters")]
        [SwaggerResponse(400, Description = "Error")]
        [SwaggerResponse(401, Description = "Unauthorized")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Globalization",
            "CA1303:Do not pass literals as localized parameters",
            Justification = "<Pending>")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Reliability",
            "CA2007:Consider calling ConfigureAwait on the awaited task",
            Justification = "<Pending>")]
        public async Task<IActionResult> Get
            ([FromQuery] _ApiListQueryType request)
        {
            try
            {
                var currentPrincipal = HttpContext.User;
                var currentUserName = currentPrincipal?.Identity?.Name;
                using var logScope = Logger.BeginScope("{User}",
                    currentUserName);
                Logger.LogInformation(ApiLogEvent.ApiRequest,
                    "Get project list {Request}",
                    request.ToDictionary());

                var query = request.ToListQuery<_EntityType>(currentPrincipal);
                var result = await Mediator.Send(query);
                if (result is null || !result.Success)
                {
                    Logger.LogWarning(ApiLogEvent.ApiErrorResponse,
                        "Get project list error response. Error={Error}.",
                        result?.Errors);
                    return BadRequest();
                }
                return Ok(result.ToEntityList<_ApiEntityType>());
            }
            catch (Exception ex)
                when (Logger.WriteScopeWhenException
                    (ApiLogEvent.ApiErrorResponse, ex))
            {
                return BadRequest();
            }
        }

        /// <summary>
        /// Add new Project object
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///     POST /projects/create
        ///     {
        ///         "Title": "New project title",
        ///         "Description": "Detail info",
        ///         "IsArchived": false,
        ///         "ProjectActorRoles":
        ///         {
        ///             "5ee8862e331ac14168f5e99c": 1
        ///         },
        ///         "IsMap": true,
        ///         "MapProvider": "GoogleMaps",
        ///         "MapParameters":
        ///         {
        ///             "Layer": "BaseLayer",
        ///             "ShowRuler": false,
        ///             "Zoom": 4,
        ///         },
        ///         "ShowMap": true,
        ///         "Layers":
        ///         [
        ///             {
        ///                 "Order": 1, 
        ///                 "IsHidden": false, 
        ///                 "GeoId": "5f3ab2d496464e955096ca88"
        ///             },
        ///             {
        ///                 "Order": 2, 
        ///                 "IsHidden": true, 
        ///                 "GeoId": "5f3ab24196464e955096ca87"
        ///             }
        ///         ]
        ///     }
        /// </remarks>
        /// <param name="command">New entity</param>
        /// <returns>Id (string) of created entity</returns>
        [HttpPost]
        [Authorize]
        [SwaggerResponse(201,
            Description = "Id of created entity and link to get entity")]
        [SwaggerResponse(400, Description = "Error")]
        [SwaggerResponse(401, Description = "Unauthorized")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Globalization",
            "CA1303:Do not pass literals as localized parameters",
            Justification = "<Pending>")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Reliability",
            "CA2007:Consider calling ConfigureAwait on the awaited task",
            Justification = "<Pending>")]
        public async Task<IActionResult> Create
            ([FromBody] _ApiEntityCreateCommandType command)
        {
            try
            {
                var currentPrincipal = HttpContext.User;
                var currentUserName = currentPrincipal?.Identity?.Name;

                using var logScope = Logger.BeginScope("{User}",
                    currentUserName);

                Logger.LogInformation(ApiLogEvent.ApiRequest,
                    "Get create project command {Command}",
                    command.ToDictionary());

                var query = command
                    .ToEntity<_EntityCreateCommandType>(currentPrincipal);
                var result = await Mediator.Send(query).ConfigureAwait(false);
                if (result is null || !result.Success)
                {
                    Logger.LogWarning(ApiLogEvent.ApiErrorResponse,
                        "Project create error response. Error={Error}.",
                        result?.Errors);
                    return BadRequest();
                }
                return CreatedAtAction(nameof(Get), new { id = result.Id },
                    result.Id);
            }
            catch (Exception ex)
                when (Logger.WriteScopeWhenException
                    (ApiLogEvent.ApiErrorResponse, ex))
            {
                return BadRequest();
            }
        }

        /// <summary>
        /// Update Project object
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///     POST /projects/update/5f3ab0a596464e955096ca86
        ///     {
        ///         "Title": "New project title",
        ///         "Description": "Detail info",
        ///         "IsArchived": false,
        ///         "ProjectActorRoles":
        ///         {
        ///             "5ee8862e331ac14168f5e99c": 1
        ///         },
        ///         "IsMap": true,
        ///         "MapProvider": "GoogleMaps",
        ///         "MapParameters":
        ///         {
        ///             "Layer": "BaseLayer",
        ///             "ShowRuler": false,
        ///             "Zoom": 4,
        ///         },
        ///         "ShowMap": true,
        ///         "Layers":
        ///         [
        ///             {
        ///                 "Order": 1, 
        ///                 "IsHidden": false, 
        ///                 "GeoId": "5f3ab37996464e955096ca89"
        ///             }
        ///         ]
        ///     }
        /// </remarks>
        /// <param name="id">Id of the entity</param>
        /// <param name="command">Update Project command</param>
        [HttpPut("{id}")]
        [Authorize]
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
            [FromBody] _ApiEntityUpdateCommandType command)
        {
            try
            {
                var currentPrincipal = HttpContext.User;
                var currentUserName = currentPrincipal?.Identity?.Name;

                using var logScope = Logger.BeginScope("{User}",
                    currentUserName);

                Logger.LogInformation(ApiLogEvent.ApiRequest,
                    "Project update command. Id={Id}. Command={Command}",
                    id, command.ToDictionary());

                if (command is null || String.IsNullOrWhiteSpace(id))
                {
                    Logger.LogWarning(ApiLogEvent.ApiArgumentError,
                        "Project update empty argument error.");
                    return BadRequest();
                }

                var appCommand = command.ToEntity<_EntityUpdateCommandType>(id,
                    currentPrincipal);
                var result = await Mediator.Send(appCommand);
                if (result is null || !result.Success)
                {
                    Logger.LogWarning(ApiLogEvent.ApiErrorResponse,
                        "Project update error response. Error={Error}.",
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
        /// Delete Project object
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///     DELETE /projects/5f3ab0a596464e955096ca86
        /// </remarks>
        /// <param name="id">Id of the deleting entity</param>
        /// <param name="hardMode">Use hard mode to delete. 
        /// true - completely delete entity, 
        /// false (default) - mark entity as archived. 
        /// </param>
        [HttpDelete("{id}")]
        [Authorize]
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
            [FromQuery] bool hardMode = false)
        {
            try
            {
                var currentPrincipal = HttpContext.User;
                var currentUserName = currentPrincipal?.Identity?.Name;

                using var logScope = Logger.BeginScope("{User}",
                    currentUserName);

                Logger.LogInformation(ApiLogEvent.ApiRequest,
                    "Project delete command. Id={Id}. HardMode={HardMode}",
                    id, hardMode);

                if (String.IsNullOrWhiteSpace(id))
                {
                    Logger.LogWarning(ApiLogEvent.ApiArgumentError,
                        "Project delete command empty id.");
                    return BadRequest();
                }

                var command = new DeleteCommand<_EntityType>
                {
                    Id = id,
                    HardMode = hardMode,
                    CurrentPrincipal = currentPrincipal,
                };
                var result = await Mediator.Send(command);
                if (result is null || !result.Success)
                {
                    Logger.LogWarning(ApiLogEvent.ApiErrorResponse,
                        "Project delete error response. Error={Error}.",
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
