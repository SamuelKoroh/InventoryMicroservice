using ApiGateway.Domain.Models;
using ApiGateway.GraphQLObj.GraphQL;
using ApiGateway.Messaging.CategoryMessage;
using ApiGateway.RedisPubSub;
using ApiGateway.RedisPubSub.Publish;
using GraphQL.Types;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;

namespace ApiGateway.GraphQLObj.GrahQLQueries
{                           
    public class RootQuery : ObjectGraphType
    {
        public RootQuery(IRedisPubSub redisPubSub)
        {
            FieldAsync<ListGraphType<CategoryType>>("categories",
                resolve: async context => await redisPubSub.HandleAndDeserialize<IEnumerable<Category>>("get-categories", "get-categories"));
            

            FieldAsync<ListGraphType<ProductType>>("products",
                resolve: async context => await redisPubSub.HandleAndDeserialize<IEnumerable<Product>>("get-products", "get-products"));
            
        }
    }
}