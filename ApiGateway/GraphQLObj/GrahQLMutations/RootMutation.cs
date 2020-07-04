using ApiGateway.Domain.Models;
using ApiGateway.GraphQLObj.GraphQLTypes;
using ApiGateway.RedisPubSub;
using GraphQL.Types;

namespace ApiGateway.GraphQLObj.GrahQLMutations
{
    public class RootMutation : ObjectGraphType
    {
        public RootMutation(IRedisPubSub redisPubSub)
        {
            FieldAsync<StringGraphType>(
                "addCategory",
                arguments: new QueryArguments(
                    new QueryArgument<NonNullGraphType<CategoryInputType>> { Name = "category" }),
                resolve: async context =>
                {
                    var category = context.GetArgument<Category>("category");
                     
                    return await redisPubSub.HandleAndReturnMessage("create-category", "category-created", category, true);
                });

            FieldAsync<StringGraphType>(
                "updateCategory",
                arguments: new QueryArguments(
                    new QueryArgument<NonNullGraphType<IdGraphType>> { Name = "categoryId" },
                    new QueryArgument<NonNullGraphType<CategoryInputType>> { Name = "category" }),
                resolve: async context =>
                {
                    var category = context.GetArgument<Category>("category");
                    category.Id = context.GetArgument<int>("categoryId");

                    return await redisPubSub.HandleAndReturnMessage("update-category", "category-updated", category, true);
                });

            FieldAsync<StringGraphType>(
                "deleteCategory",
                arguments: new QueryArguments(
                    new QueryArgument<NonNullGraphType<IdGraphType>> { Name = "categoryId" }),
                resolve: async context =>
                {
                    var categoryId = context.GetArgument<int>("categoryId");

                    return await redisPubSub.HandleAndReturnMessage("delete-category", "category-deleted", categoryId);
                });
        }
    }
}
