using ApiGateway.Domain.Models;
using GraphQL.Types;

namespace ApiGateway.GraphQLObj.GraphQL
{
    public class RequestType : ObjectGraphType<Request>
    {
        public RequestType()
        {
            Field(x => x.Id);
            Field(x => x.Requester);
            Field(x => x.ProductId);
            Field(x => x.Quantity);
            Field(x => x.IsApproved);
            Field(x => x.Status);
        }
    }
}
