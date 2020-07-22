using GeoTaskManager.Application.Core.Responses;
using MediatR;

namespace GeoTaskManager.MongoDb.Core.Commands
{
    public class DbInitDatabaseCommand : IRequest<InitResult>
    {
        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; }

        public DbInitDatabaseCommand()
        {

        }

        public DbInitDatabaseCommand(string connectionString,
            string databaseName)
        {
            ConnectionString = connectionString;
            DatabaseName = databaseName;
        }
    }

}
