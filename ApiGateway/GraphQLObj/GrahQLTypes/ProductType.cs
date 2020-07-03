using ApiGateway.Domain.Models;
using ApiGateway.RedisPubSub.Publish;
using GraphQL.Types;

namespace ApiGateway.GraphQLObj.GraphQL
{
    public class ProductType : ObjectGraphType<Product>
    {
        public ProductType()
        {
            Field(x => x.Id);
            Field(x => x.Name);
            Field(x => x.Price);
            Field(x => x.Quantity);
            Field(x => x.IsAvailable);
            FieldAsync<CategoryType>("category",
               resolve: async context =>
               await CategoryPubSub.GetCategoryById(context.Source.CategoryId));
        }
    }
}
