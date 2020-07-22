namespace GeoTaskManager.Api.Core.Filters
{
    internal class ApiErrorModel
    {
        public string FieldName { get; set; }
        public string Message { get; set; }

        public ApiErrorModel()
        {

        }

        public ApiErrorModel(string fieldName, string message)
        {
            FieldName = fieldName;
            Message = message;
        }
    }
}
