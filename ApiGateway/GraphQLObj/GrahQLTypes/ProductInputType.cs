using GraphQL.Types;

namespace ApiGateway.GraphQLObj.GraphQLTypes
{
    public class ProductInputType : InputObjectGraphType
    {
        public ProductInputType()
        {
            Field<NonNullGraphType<StringGraphType>>("name");
            Field<NonNullGraphType<IntGraphType>>("categoryId");
            Field<NonNullGraphType<IntGraphType>>("quantity");
            Field<NonNullGraphType<DecimalGraphType>>("price");
            Field<NonNullGraphType<BooleanGraphType>>("isAvailable");
        }
    }
}
