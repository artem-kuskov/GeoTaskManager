using GeoTaskManager.Api.Core.Commands;
using GeoTaskManager.Api.Core.Logging;
using GeoTaskManager.Api.Core.Models;
using GeoTaskManager.Api.GeoTasks.Mappers;
using GeoTaskManager.Api.GeoTasks.Models;
using GeoTaskManager.Api.Projects.Mappers;
using GeoTaskManager.Application.Core.Commands;
using GeoTaskManager.Application.Core.Queries;
using GeoTaskManager.Application.GeoTasks.Mappers;
using GeoTaskManager.Application.GeoTasks.Models;
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
    /// Geo Tasks controller
    /// </summary>
    [ApiController]
    [Route("[controller]/[action]")]
    [Authorize(Policy = "EmailVerified")]
    [SwaggerTag("Create, read, update and delete Geo Tasks")]
    public class TasksController : ControllerBase
    {
        private IMediator Mediator { get; }
        private ILogger<TasksController> Logger { get; set; }

        /// <summary>
        /// GeoTask API controller
        /// </summary>
        /// <param name="mediator">IMediator class instance</param>
        /// <param name="logger">ILogger class instance</param>
        public TasksController(IMediator mediator, ILogger<TasksController> logger)
        {
            Mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            Logger = logger;
        }

        /// <summary>
        /// Get specific Geo Task
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///     GET /tasks/get/1234567890123456789012345
        /// </remarks>
        /// <param name="id">Id of the entity</param>
        [HttpGet("{id}")]
        [SwaggerResponse(200, Type = typeof(ApiGeoTask),
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
                    "Get task request {id}", id);

                var query = new EntityQuery<GeoTask>(id, currentPrincipal);
                var result = await Mediator.Send(query);

                if (result is null || !result.Success)
                {
                    Logger.LogWarning(ApiLogEvent.ApiErrorResponse,
                        "Get task error response. Error={Error}.",
                        result?.Errors);
                    return BadRequest();
                }
                return Ok(result.Entity.ToApiGeoTask());
            }
            catch (Exception ex)
                when (Logger.WriteScopeWhenException
                    (ApiLogEvent.ApiErrorResponse, ex))
            {
                return BadRequest();
            }
        }

        /// <summary>
        /// Gets filtered list of Geo Tasks
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///     GET /tasks/get?[parameters]
        /// 
        /// <param name="request">
        /// Available parameters:
        /// 
        /// Offset (integer) - number of skipping element from the beginning of list
        /// 
        /// Limit (integer) - maximum number of returning elements. It could be limited by API service
        /// 
        /// Archived (boolean) - true returns only archived entities, false - only not archived entities, null - both archived and not archived entities
        /// 
        /// ProjectId (string) - filter entities by project with this Id
        /// 
        /// ActorId (string) - filter entities by actor with this Id
        /// 
        /// ActorRoleMask (integer) - filter entities by ActorId's role in Geo Task.
        /// 
        ///     0 - any role;
        ///     1 - Creator;
        ///     2 - Responsible;
        ///     4 - Assistant;
        ///     8 - Observer;
        ///     or sum of several possible roles, ex. 6 for responsible or assistant role
        ///     
        /// TaskStatusMask (integer) - filter entities by Geo Task status.
        ///
        ///     0 - any status;
        ///     1 - new;
        ///     2 - in work;
        ///     4 - finish requested;
        ///     8 - finished;
        ///     16 - cancel requested;
        ///     32 - canceled;
        ///     or sum of several possible statuses, ex. 48 for canceled or requested for cancellation Geo Tasks
        ///     
        /// MaxTimeToDeadline (string) - maximum time lag from current time to planned finish time. It can be negative time.
        ///     Example: 
        ///     
        ///         "6.20:20:10" returns Geo Tasks where the planned finish time was in the past or will be in the next 6 days 20 hours 20 minutes and 10 seconds
        ///         "-6.20:20:10" returns Geo Tasks where the planned finish time was 6 days 20 hours 20 minutes and 10 seconds ago
        ///         
        /// ContainsKeyWords (string) - returns entities containing one or several words from the parameter in the Title or/and Description
        /// 
        /// GeoId (string) - Filter by Id of the Geospatial entity, the GeoTask is linked to. 
        /// </param>
        /// </remarks>
        [HttpGet]
        [Authorize]
        [SwaggerResponse(200, Type = typeof(ApiList<ApiGeoTask>), Description = "Return list of entities and total count of entities without applying Limit and Offset parameters")]
        [SwaggerResponse(400, Description = "Error")]
        [SwaggerResponse(401, Description = "Unauthorized")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Globalization", "CA1303:Do not pass literals as localized parameters", Justification = "<Pending>")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Reliability", "CA2007:Consider calling ConfigureAwait on the awaited task", Justification = "<Pending>")]
        public async Task<IActionResult> Get
            ([FromQuery] ApiGeoTaskListQuery request)
        {
            try
            {
                var currentPrincipal = HttpContext.User;
                var currentUserName = currentPrincipal?.Identity?.Name;
                using var logScope = Logger.BeginScope("{User}",
                    currentUserName);
                Logger.LogInformation(ApiLogEvent.ApiRequest,
                    "Get task collection request {query}",
                    request.ToDictionary());

                var query = request.ToGeoTaskListQuery(currentPrincipal);
                var result = await Mediator.Send(query);
                if (result is null || !result.Success)
                {
                    Logger.LogWarning(ApiLogEvent.ApiErrorResponse,
                        "Get task collection error response. Error={Error}.",
                        result?.Errors);
                    return BadRequest();
                }
                return Ok(result.ToApiGeoTaskList());
            }
            catch (Exception ex)
                when (Logger.WriteScopeWhenException
                    (ApiLogEvent.ApiErrorResponse, ex))
            {
                return BadRequest();
            }
        }

        /// <summary>
        /// Add new Geo Task object
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///     POST /tasks/create
        ///     {
        ///         "Title": "New task title",
        ///         "Description": "Detail info",
        ///         "PlanStartAt": "2020-05-12T07:10:21.485Z",
        ///         "PlanFinishAt": "2020-05-20T11:12:23Z",
        ///         "ProjectId": "12345678901345678901234",
        ///         "ResponsibleActorId": "00000000000000000000000",
        ///         "AssistentActorsIds": 
        ///             [
        ///                 "00000000000000000000000",
        ///                 "111111111111111111111111",
        ///                 "222222222222222222222222"
        ///             ],
        ///         "ObserverActorsIds": 
        ///             [
        ///                 "00000000000000000000000",
        ///                 "111111111111111111111111"
        ///             ],
        ///         "GeosIds": 
        ///             [
        ///                 "33333333333333333333333",
        ///                 "444444444444444444444444"
        ///             ]
        ///     }
        /// </remarks>
        /// <param name="task">New entity</param>
        /// <returns>Id (string) of created entity</returns>
        [HttpPost]
        [Authorize]
        [SwaggerResponse(201, Description = "Id of created entity and link to get entity")]
        [SwaggerResponse(400, Description = "Error")]
        [SwaggerResponse(401, Description = "Unauthorized")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Globalization", "CA1303:Do not pass literals as localized parameters", Justification = "<Pending>")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Reliability", "CA2007:Consider calling ConfigureAwait on the awaited task", Justification = "<Pending>")]
        public async Task<IActionResult> Create([FromBody] ApiGeoTaskCreateCommand task)
        {
            try
            {
                var currentPrincipal = HttpContext.User;
                var currentUserName = currentPrincipal?.Identity?.Name;
                using var logScope = Logger.BeginScope("{User}", currentUserName);
                Logger.LogInformation(ApiLogEvent.ApiRequest,
                    "Get Create Task Command {Command}", task.ToDictionary());

                var query = task.ToAppGeoTaskCreateCommand(currentPrincipal);
                var result = await Mediator.Send(query).ConfigureAwait(false);
                if (result is null || !result.Success)
                {
                    Logger.LogWarning(ApiLogEvent.ApiErrorResponse,
                        "Create Task Command error response. Error={Error}.",
                        result?.Errors);
                    return BadRequest();
                }
                return CreatedAtAction(nameof(Get), new { id = result.Id }, result.Id);
            }
            catch (Exception ex)
                when (Logger.WriteScopeWhenException(ApiLogEvent.ApiErrorResponse, ex))
            {
                return BadRequest();
            }

        }

        /// <summary>
        /// Update Geo Task object
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///     POST /tasks/update/888888888888888888888888
        ///     {
        ///         "Title": "Updated task title",
        ///         "Description": "Updated Detail info",
        ///         "PlanStartAt": "2020-05-12T07:10:21.485Z",
        ///         "PlanFinishAt": "2020-05-20T07:10:21.485Z",
        ///         "ProjectId": "12345678901345678901234",
        ///         "ResponsibleActorId": "00000000000000000000000",
        ///         "AssistentActorsIds": 
        ///             [
        ///                 "00000000000000000000000",
        ///                 "111111111111111111111111",
        ///                 "222222222222222222222222"
        ///             ],
        ///         "ObserverActorsIds": 
        ///             [
        ///                 "00000000000000000000000",
        ///                 "111111111111111111111111"
        ///             ],
        ///         "GeosIds": 
        ///             [
        ///                 "33333333333333333333333",
        ///                 "444444444444444444444444"
        ///             ],
        ///         "MessageTitle": "The title of change description",
        ///         "MessageDescription": "The details of change description"
        ///     }
        /// </remarks>
        /// <param name="id">Id of the entity</param>
        /// <param name="task">Updated Geo Task object</param>
        [HttpPut("{id}")]
        [Authorize]
        [SwaggerResponse(200, Description = "OK")]
        [SwaggerResponse(400, Description = "Error")]
        [SwaggerResponse(401, Description = "Unauthorized")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Globalization", "CA1303:Do not pass literals as localized parameters", Justification = "<Pending>")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Reliability", "CA2007:Consider calling ConfigureAwait on the awaited task", Justification = "<Pending>")]
        public async Task<IActionResult> Update([FromRoute] string id, [FromBody] ApiGeoTaskUpdateCommand task)
        {
            try
            {
                var currentPrincipal = HttpContext.User;
                var currentUserName = currentPrincipal?.Identity?.Name;
                using var logScope = Logger.BeginScope("{User}", currentUserName);
                Logger.LogInformation(ApiLogEvent.ApiRequest,
                    "Task Update Command. Id={Id}. Command={Command}", id,
                    task.ToDictionary());

                if (task is null || String.IsNullOrWhiteSpace(id))
                {
                    Logger.LogWarning(ApiLogEvent.ApiArgumentError,
                        "Task Update Command empty argument error.");
                    return BadRequest();
                }

                var command = task.ToGeoTaskUpdateCommand(id,
                    currentPrincipal);
                var result = await Mediator.Send(command);
                if (result is null || !result.Success)
                {
                    Logger.LogWarning(ApiLogEvent.ApiErrorResponse,
                        "Task Update Command error response. Error={Error}.",
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
        /// Patch Geo Task object
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///     PATCH /tasks/888888888888888888888888
        ///     {
        ///         "Patch": 
        ///             [
        ///                 { "op": "replace", "path": "/Title", "value": "New title" },
        ///                 { "op": "add", "path": "/AssistentActorsIds", "value": ["66666666666666666666666666"] },
        ///                 { "op": "remove", "path": "/Description"}
        ///             ],
        ///         "MessageTitle": "The title of change description",
        ///         "MessageDescription": "The details of change description"
        ///     }
        /// </remarks>
        /// <param name="id">Id of patching Geo Task object</param>
        /// <param name="patch">patch command for Geo Task object</param>
        [HttpPatch("{id}")]
        [Authorize]
        [SwaggerResponse(200, Description = "OK")]
        [SwaggerResponse(400, Description = "Error")]
        [SwaggerResponse(401, Description = "Unauthorized")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Globalization", "CA1303:Do not pass literals as localized parameters", Justification = "<Pending>")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Reliability", "CA2007:Consider calling ConfigureAwait on the awaited task", Justification = "<Pending>")]
        public async Task<IActionResult> Patch([FromRoute] string id,
                                               [FromBody] ApiPatchCommand<ApiGeoTask> patch)
        {
            try
            {
                var currentPrincipal = HttpContext.User;
                var currentUserName = currentPrincipal?.Identity?.Name;
                using var logScope = Logger.BeginScope("{User}",
                    currentUserName);
                Logger.LogInformation(ApiLogEvent.ApiRequest,
                    "Task patch command. Id={Id}. Command={Command}",
                    id, patch.ToDictionary());

                if (patch?.Patch is null || String.IsNullOrWhiteSpace(id))
                {
                    Logger.LogWarning(ApiLogEvent.ApiArgumentError,
                        "Patch task empty argument.");
                    return BadRequest();
                }

                var getQuery = new EntityQuery<GeoTask>(id, currentPrincipal);
                var getResult = await Mediator.Send(getQuery);
                if (getResult is null || !getResult.Success)
                {
                    Logger.LogWarning(ApiLogEvent.ApiErrorResponse,
                        "Get task error response. Error={Error}.",
                        getResult?.Errors);
                    return BadRequest();
                }

                var apiGeoTask = getResult.Entity.ToApiGeoTask();
                patch.Patch.ApplyTo(apiGeoTask);
                var command = apiGeoTask.ToGeoTaskUpdateCommand(id,
                    currentPrincipal, patch.MessageTitle,
                    patch.MessageDescription);
                var result = await Mediator.Send(command);
                if (result is null || !result.Success)
                {
                    Logger.LogWarning(ApiLogEvent.ApiErrorResponse,
                        "Task update error response. Error={Error}.",
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
        /// Delete Geo Task object
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///     DELETE /tasks/888888888888888888888888
        /// </remarks>
        /// <param name="id">Id of the deleting entity</param>
        /// <param name="hardMode">Use hard mode to delete. true - completely delete entity, false - mark entity as archived. Default value is false</param>
        /// <param name="commitTitle">Title of the message adding to the history when delete in soft mode</param>
        /// <param name="commitDescription">Message adding to the history when delete in soft mode</param>
        [HttpDelete("{id}")]
        [Authorize]
        [SwaggerResponse(200, Description = "OK")]
        [SwaggerResponse(400, Description = "Error")]
        [SwaggerResponse(401, Description = "Unauthorized")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Globalization", "CA1303:Do not pass literals as localized parameters", Justification = "<Pending>")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Reliability", "CA2007:Consider calling ConfigureAwait on the awaited task", Justification = "<Pending>")]
        public async Task<IActionResult> Delete([FromRoute] string id,
            [FromQuery] bool hardMode = false, [FromQuery]
        string commitTitle = null, [FromQuery] string commitDescription = null)
        {
            try
            {
                var currentPrincipal = HttpContext.User;
                var currentUserName = currentPrincipal?.Identity?.Name;
                using var logScope = Logger.BeginScope("{User}", currentUserName);
                Logger.LogInformation(ApiLogEvent.ApiRequest, "Task delete command. Id={id}. HardMode={hardMode}", id, hardMode);

                if (String.IsNullOrWhiteSpace(id))
                {
                    Logger.LogWarning(ApiLogEvent.ApiArgumentError, "Task delete error empty id.");
                    return BadRequest();
                }

                var command = new DeleteCommand<GeoTask>
                {
                    Id = id,
                    HardMode = hardMode,
                    CurrentPrincipal = currentPrincipal,
                    MessageDescription = commitDescription,
                    MessageTitle = commitTitle
                };
                var result = await Mediator.Send(command);
                if (result is null || !result.Success)
                {
                    Logger.LogWarning(ApiLogEvent.ApiErrorResponse, "Task delete error response. Errors={errors}", result?.Errors);
                    return BadRequest();
                }
                return Ok();
            }
            catch (Exception ex)
                when (Logger.WriteScopeWhenException(ApiLogEvent.ApiErrorResponse, ex))
            {
                return BadRequest();
            }
        }
    }
}
