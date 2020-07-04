using GraphQL.Types;
using ApiGateway.Domain.Models;

namespace ApiGateway.GraphQLObj.GraphQL
{
    public class CategoryType : ObjectGraphType<Category>
    {
        public CategoryType()
        {
            Field(x => x.Id);
            Field(x => x.Name);
        }
    }
}
