using ApiGateway.Domain.Models;
using ApiGateway.RedisPubSub;
using ApiGateway.RedisPubSub.Publish;
using GraphQL.Types;

namespace ApiGateway.GraphQLObj.GraphQL
{
    public class ProductType : ObjectGraphType<Product>
    {
        public ProductType(IRedisPubSub redisPubSub)
        {
            Field(x => x.Id);
            Field(x => x.Name);
            Field(x => x.Price);
            Field(x => x.Quantity);
            Field(x => x.IsAvailable);
            FieldAsync<CategoryType>("category",
               resolve: async context =>
               await redisPubSub.HandleAndDeserialize<Category>
               ("get-category-by-id", "get-category-by-id-reply", context.Source.CategoryId.ToString()));
        }
    }
}
