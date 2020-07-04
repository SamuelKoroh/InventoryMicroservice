using GraphQL.Types;

namespace ApiGateway.GraphQLObj.GraphQLTypes
{
    public class RegisterInputType : InputObjectGraphType
    {
        public RegisterInputType()
        {
            Field<NonNullGraphType<StringGraphType>>("firstName");
            Field<NonNullGraphType<StringGraphType>>("lastName");
            Field<NonNullGraphType<StringGraphType>>("email");
            Field<NonNullGraphType<StringGraphType>>("password");
            Field<NonNullGraphType<StringGraphType>>("role");
        }
    }
}
