using GraphQL.Types;
using ApiGateway.Domain.Models;
using ApiGateway.RedisPubSub;
using Newtonsoft.Json;
using System.Collections.Generic;
using ApiGateway.MethodExtension;

namespace ApiGateway.GraphQLObj.GraphQL
{
    public class CategoryType : ObjectGraphType<Category>
    {
        public CategoryType(IRabbitMQPubSub rabbitMQPub)
        {
            Field(x => x.Id);
            Field(x => x.Name);
            FieldAsync<ListGraphType<ProductType>>("products",
              resolve: async context =>
              (await rabbitMQPub.Handle("get-products-by-category-id",
              context.Source.Id.ToString())).Deserialize<IEnumerable<Product>>());
        }
    }
}
