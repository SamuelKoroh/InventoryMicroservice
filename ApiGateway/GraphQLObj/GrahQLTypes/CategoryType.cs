using GraphQL.Types;
using ApiGateway.Domain.Models;
using ApiGateway.RedisPubSub.Publish;

namespace ApiGateway.GraphQLObj.GraphQL
{
    public class CategoryType : ObjectGraphType<Category>
    {
        public CategoryType()
        {
            Field(x => x.Id);
            Field(x => x.Name);
            FieldAsync<ListGraphType<ProductType>>("productsx",
              resolve: async context =>
              await ProductPubSub.GetProductsByCategoryId(context.Source.Id));
        }
    }
}
