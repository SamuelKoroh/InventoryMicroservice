using ApiGateway.Domain.Models;
using ApiGateway.GraphQLObj.GraphQL;
using ApiGateway.Messaging.CategoryMessage;
using ApiGateway.RedisPubSub.Publish;
using GraphQL.Types;
using MassTransit;
using System;
using System.Collections.Generic;

namespace ApiGateway.GraphQLObj.GrahQLQueries
{
    public class RootQuery : ObjectGraphType
    {
        public RootQuery()
        {
            FieldAsync<ListGraphType<CategoryType>>("categories",
                resolve:  async context => await CategoryPublish.GetCategories());
        }
    }
}
