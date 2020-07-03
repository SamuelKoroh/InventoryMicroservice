using Newtonsoft.Json.Linq;

namespace ApiGateway.GraphQLObj.Parameters
{
    public class GraphQLQueryParam
    {
        public string OperationName { get; set; }
        public string Query { get; set; }
        public JObject Variables { get; set; }
    }
}
