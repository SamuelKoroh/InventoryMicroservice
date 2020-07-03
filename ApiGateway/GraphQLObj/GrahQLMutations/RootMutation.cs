using ApiGateway.Domain.Models;
using ApiGateway.GraphQLObj.GraphQLTypes;
using ApiGateway.RedisPubSub.Publish;
using GraphQL.Types;

namespace ApiGateway.GraphQLObj.GrahQLMutations
{
    public class RootMutation : ObjectGraphType
    {
        public RootMutation()
        {
            FieldAsync<StringGraphType>(
                "addCategory",
                arguments: new QueryArguments(
                    new QueryArgument<NonNullGraphType<CategoryInputType>> { Name = "category" }),
                resolve: async context =>
                {
                    var category = context.GetArgument<Category>("category");
                     
                    return await CategoryPubSub.CreateCategory(category);
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

                    return await CategoryPubSub.UpdateCategory(category);
                });

            FieldAsync<StringGraphType>(
                "deleteCategory",
                arguments: new QueryArguments(
                    new QueryArgument<NonNullGraphType<IdGraphType>> { Name = "categoryId" }),
                resolve: async context =>
                {
                    var categoryId = context.GetArgument<int>("categoryId");
                    return await CategoryPubSub.DeleteCategory(categoryId);
                });
        }
    }
}
