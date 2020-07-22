namespace GeoTaskManager.Api.Actors.Models
{
    /// <summary>
    /// Command to update exist Actor entity.
    /// </summary>
    public class ApiActorUpdateCommand
    {
        /// <summary>
        /// User login
        /// </summary>
        public string Login { get; set; }

        /// <summary>
        /// Soft delete attribute:
        ///     true - the entity is archived,
        ///     false - not archived.
        /// </summary>
        public bool IsArchived { get; set; }

        /// <summary>
        /// Actor's title
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Actor's description
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Actor's department
        /// </summary>
        public string Department { get; set; }

        /// <summary>
        /// Actor's first name
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// Actor's last name
        /// </summary>
        public string LastName { get; set; }

        /// <summary>
        /// Contact phone
        /// </summary>
        public string Phone { get; set; }

        /// <summary>
        /// Contact e-mail
        /// </summary>
        public string EMail { get; set; }

        /// <summary>
        /// Contact Skype
        /// </summary>
        public string Skype { get; set; }

        /// <summary>
        /// Actor's global role
        /// </summary>
        public int Role { get; set; }
    }
}
