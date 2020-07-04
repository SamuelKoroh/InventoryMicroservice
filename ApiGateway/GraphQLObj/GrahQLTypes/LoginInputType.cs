using GraphQL.Types;

namespace ApiGateway.GraphQLObj.GraphQLTypes
{
    public class LoginInputType : InputObjectGraphType
    {
        public LoginInputType()
        {
            Field<NonNullGraphType<StringGraphType>>("email");
            Field<NonNullGraphType<StringGraphType>>("password");
        }
    }
}
