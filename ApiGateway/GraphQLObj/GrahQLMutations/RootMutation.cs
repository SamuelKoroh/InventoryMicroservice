using ApiGateway.Configurations;
using ApiGateway.Domain.Models;
using ApiGateway.Domian.Models;
using ApiGateway.GraphQLObj.GraphQLTypes;
using ApiGateway.RedisPubSub;
using ApiGateway.Utils;
using GraphQL.Authorization;
using GraphQL.Types;
using Microsoft.AspNetCore.Http;

namespace ApiGateway.GraphQLObj.GrahQLMutations
{
    public class RootMutation : BaseObjectGraphType
    {
        public RootMutation(IRedisPubSub redisPubSub, IHttpContextAccessor httpContextAccessor)
            :base(httpContextAccessor)
        {
            FieldAsync<StringGraphType>(
                "addCategory",
                arguments: new QueryArguments(
                    new QueryArgument<NonNullGraphType<CategoryInputType>> { Name = "category" }),
                resolve: async context =>
                {
                    var category = context.GetArgument<Category>("category");
                     
                    return await redisPubSub.HandleAndReturnMessage(Channel.CreateCategory, 
                        Channel.CategoryCreated, category, true);
                }).AuthorizeWith(Policies.InventoryKeeper);

            FieldAsync<StringGraphType>(
                "updateCategory",
                arguments: new QueryArguments(
                    new QueryArgument<NonNullGraphType<IdGraphType>> { Name = "categoryId" },
                    new QueryArgument<NonNullGraphType<CategoryInputType>> { Name = "category" }),
                resolve: async context =>
                {
                    var category = context.GetArgument<Category>("category");
                    category.Id = context.GetArgument<int>("categoryId");

                    return await redisPubSub.HandleAndReturnMessage(Channel.UpdateCategory, 
                        Channel.CategoryUpdated, category, true);
                }).AuthorizeWith(Policies.InventoryKeeper);

            FieldAsync<StringGraphType>(
                "deleteCategory",
                arguments: new QueryArguments(
                    new QueryArgument<NonNullGraphType<IdGraphType>> { Name = "categoryId" }),
                resolve: async context =>
                {
                    var categoryId = context.GetArgument<int>("categoryId");

                    return await redisPubSub.HandleAndReturnMessage(Channel.DeleteCategory, 
                        Channel.CategoryDeleted, categoryId);
                }).AuthorizeWith(Policies.InventoryKeeper);

            FieldAsync<StringGraphType>(
                "addProduct",
                arguments: new QueryArguments(
                    new QueryArgument<NonNullGraphType<ProductInputType>> { Name = "product" }),
                resolve: async context =>
                {
                    var product = context.GetArgument<Product>("product");

                    return await redisPubSub.HandleAndReturnMessage(Channel.CreateProduct, 
                        Channel.ProductCreated, product, true);
                }).AuthorizeWith(Policies.InventoryKeeper);

            FieldAsync<StringGraphType>(
                "updateProduct",
                arguments: new QueryArguments(
                    new QueryArgument<NonNullGraphType<IdGraphType>> { Name = "productId" },
                    new QueryArgument<NonNullGraphType<ProductInputType>> { Name = "product" }),
                resolve: async context =>
                {
                    var product = context.GetArgument<Product>("product");
                    product.Id = context.GetArgument<int>("productId");

                    return await redisPubSub.HandleAndReturnMessage(Channel.UpdateProduct, 
                        Channel.ProductUpdated, product, true);
                }).AuthorizeWith(Policies.InventoryKeeper);

            FieldAsync<StringGraphType>(
                "deleteProduct",
                arguments: new QueryArguments(
                    new QueryArgument<NonNullGraphType<IdGraphType>> { Name = "productId" }),
                resolve: async context =>
                {
                    var productId = context.GetArgument<int>("productId");

                    return await redisPubSub.HandleAndReturnMessage(Channel.DeleteProduct, 
                        Channel.ProductDeleted, productId);
                }).AuthorizeWith(Policies.InventoryKeeper);

            FieldAsync<StringGraphType>(
                "register",
                arguments: new QueryArguments(
                    new QueryArgument<NonNullGraphType<RegisterInputType>> { Name = "register" }),
                resolve: async context =>
                {
                    var register = context.GetArgument<Register>("register");

                    return await redisPubSub.HandleAndReturnMessage(Channel.CreateAccount, 
                        Channel.AccountCreated, register, true);
                });

            FieldAsync<StringGraphType>(
                "login",
                arguments: new QueryArguments(
                    new QueryArgument<NonNullGraphType<LoginInputType>> { Name = "login" }),
                resolve: async context =>
                {
                    var login = context.GetArgument<Login>("login");

                    return await redisPubSub.HandleAndReturnMessage(Channel.LoginRequest, 
                        Channel.LoginResponse, login, true);
                });

            FieldAsync<StringGraphType>(
                "request",
                arguments: new QueryArguments(
                    new QueryArgument<NonNullGraphType<RequestInputType>> { Name = "request" }),
                resolve: async context =>
                {
                    var request = context.GetArgument<Request>("request");
                    request.Requester = GetUserId();

                    return await redisPubSub.HandleAndReturnMessage(Channel.PlaceRequest, 
                        Channel.PlaceRequestResponse, request, true);
                }).AuthorizeWith(Policies.Requester);

            FieldAsync<StringGraphType>(
                "approveRequest",
                arguments: new QueryArguments(
                    new QueryArgument<NonNullGraphType<RequestApprovalInputType>> 
                    { 
                        Name = "approveRequest" 
                    }),
                resolve: async context =>
                {
                    var request = context.GetArgument<RequestApproval>("approveRequest");

                    return await redisPubSub
                        .HandleAndReturnMessage(Channel.RequestApproval, 
                            Channel.RequestApprovalResponse, request, true);
                }).AuthorizeWith(Policies.Approver);
        }
    }
}
