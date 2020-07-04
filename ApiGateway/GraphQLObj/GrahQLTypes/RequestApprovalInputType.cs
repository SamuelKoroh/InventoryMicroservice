using GraphQL.Types;

namespace ApiGateway.GraphQLObj.GraphQLTypes
{
    public class RequestApprovalInputType : InputObjectGraphType
    {
        public RequestApprovalInputType()
        {
            Field<NonNullGraphType<IntGraphType>>("requestId");
            Field<NonNullGraphType<BooleanGraphType>>("isApproved");
        }
    }
}
