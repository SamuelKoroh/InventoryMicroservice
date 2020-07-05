using ApiGateway.Domain.Models;
using ApiGateway.GraphQLObj.GraphQL;
using ApiGateway.MethodExtension;
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
        public RootQuery(
            IRabbitMQPubSub rabbitMQPub,
            IHttpContextAccessor httpContextAccessor)
            :base(httpContextAccessor)
        {
            FieldAsync<ListGraphType<CategoryType>>("categories",
                resolve: async context => 
                        (await rabbitMQPub.Handle("get-all-category"))
                            .Deserialize<IEnumerable<Category>>())
                             .AuthorizeWith(Policies.InventoryKeeper);

            FieldAsync<ListGraphType<ProductType>>("productsList",
                resolve: async context =>
                (await rabbitMQPub.Handle("get-products"))
                            .Deserialize<IEnumerable<Product>>())
                             .AuthorizeWith(Policies.InventoryKeeper);

            FieldAsync<ListGraphType<ProductType>>("products",
                resolve: async context =>
                (await rabbitMQPub.Handle("get-available-products"))
                            .Deserialize<IEnumerable<Product>>())
                             .AuthorizeWith(Policies.Requester);

            FieldAsync<ListGraphType<RequestType>>("myRequests",
                resolve: async context =>
                (await rabbitMQPub.Handle("get-my-requests", GetUserId()))
                            .Deserialize<IEnumerable<Request>>())
                             .AuthorizeWith(Policies.Requester);

            FieldAsync<ListGraphType<RequestType>>("allRequest",
                resolve: async context =>
                (await rabbitMQPub.Handle("get-all-requests"))
                            .Deserialize<IEnumerable<Request>>())
                             .AuthorizeWith(Policies.Approver);

        }
    }
}