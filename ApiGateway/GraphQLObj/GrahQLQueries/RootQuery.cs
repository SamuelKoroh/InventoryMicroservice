using ApiGateway.Domain.Models;
using ApiGateway.GraphQLObj.GraphQL;
using ApiGateway.Messaging.CategoryMessage;
using ApiGateway.RedisPubSub;
using ApiGateway.RedisPubSub.Publish;
using GraphQL.Types;
using MassTransit;
using System;
using System.Collections.Generic;

namespace ApiGateway.GraphQLObj.GrahQLQueries
{
    public class RootQuery : ObjectGraphType
    {
        public RootQuery(IRedisPubSub redisPubSub)
        {
            FieldAsync<ListGraphType<CategoryType>>("categories",
                resolve: async context => await redisPubSub.Handler<IEnumerable<Category>>("get-categories", "get-categories-reply"));
            //async context => await CategoryPubSub.GetCategories());

            FieldAsync<ListGraphType<ProductType>>("products",
                resolve: async context => await redisPubSub.Handler<IEnumerable<Product>>("get-products", "get-products-reply"));
            //await ProductPubSub.GetProducts());

        }
    }
}
//resolve:  async context => await redisPubSub.Handler<IEnumerable<Category>>("get-categories", "get-categories"));

