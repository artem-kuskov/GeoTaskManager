using GeoTaskManager.Api.Core.Logging;
using GeoTaskManager.Api.Core.Models;
using GeoTaskManager.Api.Geos.Mappers;
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
    GeoTaskManager.Api.Geos.Models.ApiGeoCreateCommand;
using _ApiEntityType =
    GeoTaskManager.Api.Geos.Models.ApiGeo;
using _ApiEntityUpdateCommandType =
    GeoTaskManager.Api.Geos.Models.ApiGeoUpdateCommand;
using _ApiListQueryType =
    GeoTaskManager.Api.Geos.Models.ApiGeoListQuery;
using _EntityCreateCommandType =
    GeoTaskManager.Application.Geos.Commands.GeoCreateCommand;
using _EntityType =
    GeoTaskManager.Application.Geos.Models.Geo;
using _EntityUpdateCommandType =
    GeoTaskManager.Application.Geos.Commands.GeoUpdateCommand;

namespace GeoTaskManager.Api.Controllers
{
    /// <summary>
    /// Geospatial (geo) entities controller
    /// </summary>
    [ApiController]
    [Route("[controller]/[action]")]
    [Authorize(Policy = "EmailVerified")]
    [SwaggerTag("Create, read, update and delete Geospatial entities")]
    public class GeosController : ControllerBase
    {
        private IMediator Mediator { get; }
        private ILogger<GeosController> Logger { get; set; }

        /// <summary>
        /// Geo API controller
        /// </summary>
        /// <param name="mediator">IMediator class instance</param>
        /// <param name="logger">ILogger class instance</param>
        public GeosController(IMediator mediator,
            ILogger<GeosController> logger)
        {
            Mediator = mediator
                ?? throw new ArgumentNullException(nameof(mediator));
            Logger = logger;
        }

        /// <summary>
        /// Get specific Geo
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///     GET /geos/get/1234567890123456789012345
        /// </remarks>
        /// <param name="id">Id (required) of the entity</param>
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
                    "Get geo request {Id}", id);

                var query = new EntityQuery<_EntityType>(id, currentPrincipal);
                var result = await Mediator.Send(query);

                if (result is null || !result.Success)
                {
                    Logger.LogWarning(ApiLogEvent.ApiErrorResponse,
                        "Get geo error response. Error={Error}.",
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
        /// Gets filtered list of Geos
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///     GET /geos/get?[parameters]
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
        /// ProjectId (string) - Id of the project, containing Geos, 
        /// {id} - only entities from specified project, 
        /// null - entities from any project
        /// 
        /// DistanceLong (double) - set maximum distance (in meters) 
        /// to the entities from the CenterLong along Longitude coordinate 
        /// (a half of wide of the limit box with the center 
        /// in CenterLong, CenterLat), 
        /// {number} - distance in meters, 
        /// 0 - Geo spatial filter is not applied
        ///
        /// DistanceLat (double) - set maximum distance (in meters) 
        /// to the entities from the CenterLat along Latitude coordinate 
        /// (a half of height of the limit box with the center 
        /// in CenterLong, CenterLat), 
        /// {number} - distance in meters, 
        /// 0 - Geo spatial filter is not applied
        /// 
        /// CenterLong (double) - longitude of the limit box center coordinate,
        /// from which DistanceLong is counted (values from -180 to 180).
        /// 
        /// CenterLat (double) - latitude of the limit box center coordinate,
        /// from which DistanceLat is counted (values from -90 to 90).
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
                    "Get geo list {Request}",
                    request.ToDictionary());

                var query = request.ToListQuery<_EntityType>(currentPrincipal);
                var result = await Mediator.Send(query);
                if (result is null || !result.Success)
                {
                    Logger.LogWarning(ApiLogEvent.ApiErrorResponse,
                        "Get geo list error response. Error={Error}.",
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
        /// Add new Geo object
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///     POST /geos/create
        ///     {
        ///         "isArchived": false,
        ///         "title": "My Geo Polygons",
        ///         "description": "Homeland",
        ///         "projectId": "5f02eda4624d825f2425a87c",
        ///         "geoJson": "{ \"type\" : \"FeatureCollection\", 
        ///             \"features\" : [{ \"type\" : \"Feature\", 
        ///             \"geometry\" : 
        ///             { \"type\" : \"Polygon\", 
        ///             \"coordinates\" : 
        ///                 [[[37.88935661315918, 55.700275346622817], 
        ///                 [37.88884162902832, 55.69800200857496], 
        ///                 [37.891416549682617, 55.697518302569328], 
        ///                 [37.893905639648438, 55.699017771703225], 
        ///                 [37.88935661315918, 55.700275346622817]]] }, 
        ///                 \"properties\" : { } }, 
        ///             { \"type\" : \"Feature\", 
        ///             \"geometry\" : 
        ///             { \"type\" : \"Polygon\", 
        ///             \"coordinates\" : 
        ///                 [[[37.888647372040346, 55.697333455403793], 
        ///                 [37.889465039458663, 55.697333455403793], 
        ///                 [37.889465039458663, 55.697872447701002], 
        ///                 [37.888647372040346, 55.697872447701002], 
        ///                 [37.888647372040346, 55.697333455403793]]] }, 
        ///             \"properties\" : { } }, 
        ///             { \"type\" : \"Feature\", 
        ///             \"geometry\" : 
        ///             { \"type\" : \"Point\", 
        ///             \"coordinates\" : 
        ///                 [37.895793914794915, 55.696998311937186] },
        ///             \"properties\" : { } }] }"
        ///     }
        /// </remarks>
        /// <param name="command">New entity 
        /// (geoJson contains serialized string of 
        /// GeoJson FeatureCollection value)</param>
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
                    "Get create geo command {Command}",
                    command.ToDictionary());

                var query = command
                    .ToEntity<_EntityCreateCommandType>(currentPrincipal);
                var result = await Mediator.Send(query).ConfigureAwait(false);
                if (result is null || !result.Success)
                {
                    Logger.LogWarning(ApiLogEvent.ApiErrorResponse,
                        "Geo create error response. Error={Error}.",
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
        /// Update Geo object
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///     POST /geos/update/888888888888888888888888
        ///     {
        ///         "isArchived": true,
        ///         "title": "My New Geo Polygons",
        ///         "description": "Homeland",
        ///         "geoJson": "{
        ///             \"type\" : \"FeatureCollection\", 
        ///             \"features\" : 
        ///             [
        ///                 {
        ///                     \"type\" : \"Feature\", 
        ///                     \"geometry\" : 
        ///                     { 
        ///                         \"type\" : \"Polygon\", 
        ///                         \"coordinates\" : 
        ///                         [[[37.88935661315918, 55.700275346622817], 
        ///                         [37.88884162902832, 55.69800200857496], 
        ///                         [37.891416549682617, 55.697518302569328], 
        ///                         [37.893905639648438, 55.699017771703225], 
        ///                         [37.88935661315918, 55.700275346622817]]] 
        ///                     }, 
        ///                     \"properties\" : { } 
        ///                 }
        ///             ]
        ///         }"
        ///     }
        /// </remarks>
        /// <param name="id">Id of the entity</param>
        /// <param name="command">Update Geo command (geoJson contains 
        /// serialized string of GeoJson FeatureCollection value)</param>
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
                    "Geo update command. Id={Id}. Command={Command}",
                    id, command.ToDictionary());

                if (command is null || String.IsNullOrWhiteSpace(id))
                {
                    Logger.LogWarning(ApiLogEvent.ApiArgumentError,
                        "Geo update empty argument error.");
                    return BadRequest();
                }

                var appCommand = command.ToEntity<_EntityUpdateCommandType>(id,
                    currentPrincipal);
                var result = await Mediator.Send(appCommand);
                if (result is null || !result.Success)
                {
                    Logger.LogWarning(ApiLogEvent.ApiErrorResponse,
                        "Geo update error response. Error={Error}.",
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
        /// Delete Geo object
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///     DELETE /geos/888888888888888888888888?hardMode=true
        /// </remarks>
        /// <param name="id">Id of the deleting entity</param>
        /// <param name="hardMode">Use hard mode to delete. 
        /// true - completely delete entity. 
        /// Also remove links to that Geo entity in GeoTask and Project entities.
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
                    "Geo delete command. Id={Id}. HardMode={HardMode}",
                    id, hardMode);

                if (String.IsNullOrWhiteSpace(id))
                {
                    Logger.LogWarning(ApiLogEvent.ApiArgumentError,
                        "Geo delete command empty id.");
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
                        "Geo delete error response. Error={Error}.",
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
