using GraphQL.Types;

namespace ApiGateway.GraphQLObj.GraphQLTypes
{
    public class CategoryInputType : InputObjectGraphType
    {
        public CategoryInputType()
        {
            Field<NonNullGraphType<StringGraphType>>("name");
        }
    }
}
