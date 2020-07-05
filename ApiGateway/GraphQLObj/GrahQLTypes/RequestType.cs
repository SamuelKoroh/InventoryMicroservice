using ApiGateway.Domain.Models;
using ApiGateway.MethodExtension;
using ApiGateway.RedisPubSub;
using GraphQL.Types;

namespace ApiGateway.GraphQLObj.GraphQL
{
    public class RequestType : ObjectGraphType<Request>
    {
        public RequestType(IRabbitMQPubSub rabbitMQPub)
        {
            Field(x => x.Id);
            Field(x => x.Requester);
            Field(x => x.ProductId);
            Field(x => x.Quantity);
            Field(x => x.IsApproved);
            Field(x => x.Status);
            FieldAsync<ProductType>("product",
              resolve: async context =>
              (await rabbitMQPub.Handle("get-product-by-id",
              context.Source.ProductId.ToString())).Deserialize<Product>());
        }
    }
}
