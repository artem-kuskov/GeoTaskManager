using Microsoft.AspNetCore.JsonPatch;

namespace GeoTaskManager.Api.Core.Commands
{
    /// <summary>
    /// API command to patch entity.
    /// </summary>
    /// <typeparam name="TEntity">Type of patching entity</typeparam>
    public class ApiPatchCommand<TEntity> where TEntity : class
    {
        /// <summary>
        /// Patch object with operations for entity
        /// <seealso>
        ///     <a href="https://tools.ietf.org/html/rfc6902">RFC6902</a>
        /// </seealso>
        /// </summary>
        public JsonPatchDocument<TEntity> Patch { get; set; }

        /// <summary>
        /// Commit title
        /// </summary>
        public string MessageTitle { get; set; }

        /// <summary>
        /// Commit description
        /// </summary>
        public string MessageDescription { get; set; }
    }
}
