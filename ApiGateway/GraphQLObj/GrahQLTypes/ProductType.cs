using ApiGateway.Domain.Models;
using ApiGateway.MethodExtension;
using ApiGateway.RedisPubSub;
using GraphQL.Types;

namespace ApiGateway.GraphQLObj.GraphQL
{
    public class ProductType : ObjectGraphType<Product>
    {
        public ProductType(IRabbitMQPubSub rabbitMQPub)
        {
            Field(x => x.Id);
            Field(x => x.Name);
            Field(x => x.Price);
            Field(x => x.Quantity);
            Field(x => x.IsAvailable);
            Field(x => x.CategoryId);
            FieldAsync<CategoryType>("category",
              resolve: async context =>
              (await rabbitMQPub.Handle("get-category-by-id",
              context.Source.CategoryId.ToString())).Deserialize<Category>());
        }
    }
}
