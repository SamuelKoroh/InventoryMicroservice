using ApiGateway.Domain.Models;
using ApiGateway.GraphQLObj.GraphQL;
using ApiGateway.RedisPubSub;
using ApiGateway.Utils;
using GraphQL.Authorization;
using GraphQL.Types;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;

namespace ApiGateway.GraphQLObj.GrahQLQueries
{                           
    public class RootQuery : BaseObjectGraphType
    {
        public RootQuery(IRedisPubSub redisPubSub, IHttpContextAccessor httpContextAccessor)
            :base(httpContextAccessor)
        {
            FieldAsync<ListGraphType<CategoryType>>("categories",
                resolve: async context => await redisPubSub
                    .HandleAndDeserialize<IEnumerable<Category>>("get-categories", "get-categories"))
                .AuthorizeWith(Policies.InventoryKeeper);
            

            FieldAsync<ListGraphType<ProductType>>("productsList",
                resolve: async context => 
                await redisPubSub
                .HandleAndDeserialize<IEnumerable<Product>>("get-products", "get-products"))
                .AuthorizeWith(Policies.InventoryKeeper);

            FieldAsync<ListGraphType<ProductType>>("products",
                resolve: async context =>
                await redisPubSub
                .HandleAndDeserialize<IEnumerable<Product>>("get-available-products", "get-available-products"))
                .AuthorizeWith(Policies.Requester);

            FieldAsync<ListGraphType<RequestType>>("myRequests",
                resolve: async context => await redisPubSub
                        .HandleAndDeserialize<IEnumerable<Request>>("get-my-requests", "get-my-requests-reply", GetUserId()))
                .AuthorizeWith(Policies.Requester);

            FieldAsync<ListGraphType<RequestType>>("allRequest",
                resolve: async context => await redisPubSub
                        .HandleAndDeserialize<IEnumerable<Request>>("get-all-requests", "get-all-requests"))
                .AuthorizeWith(Policies.Approver);
        }
    }
}