using GraphQL.Types;

namespace ApiGateway.GraphQLObj.GraphQLTypes
{
    public class RequestInputType : InputObjectGraphType
    {
        public RequestInputType()
        {
            Field<NonNullGraphType<IntGraphType>>("productId");
            Field<NonNullGraphType<IntGraphType>>("quantity");
        }
    }
}
