using ApiGateway.Configurations;
using ApiGateway.Domain.Models;
using ApiGateway.Domian.Models;
using ApiGateway.GraphQLObj.GraphQLTypes;
using ApiGateway.RedisPubSub;
using ApiGateway.Utils;
using GraphQL.Authorization;
using GraphQL.Types;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace ApiGateway.GraphQLObj.GrahQLMutations
{
    public class RootMutation : BaseObjectGraphType
    {
        private string SerializeObject(object data) => JsonConvert.SerializeObject(data);
        public RootMutation(IRabbitMQPubSub rabbitMQPubSub, IHttpContextAccessor httpContextAccessor)
            :base(httpContextAccessor)
        {
            FieldAsync<StringGraphType>(
                "addCategory",
                arguments: new QueryArguments(
                    new QueryArgument<NonNullGraphType<CategoryInputType>> { Name = "category" }),
                resolve: async context =>
                {
                    var category = context.GetArgument<Category>("category");
                    return await rabbitMQPubSub.Handle(Channel.CreateCategory, SerializeObject(category));
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

                    return await rabbitMQPubSub.Handle(Channel.UpdateCategory, SerializeObject(category));
                }).AuthorizeWith(Policies.InventoryKeeper);

            FieldAsync<StringGraphType>(
                "deleteCategory",
                arguments: new QueryArguments(
                    new QueryArgument<NonNullGraphType<IdGraphType>> { Name = "categoryId" }),
                resolve: async context =>
                {
                    var categoryId = context.GetArgument<int>("categoryId");

                    return await rabbitMQPubSub.Handle(Channel.DeleteCategory,categoryId.ToString());
                }).AuthorizeWith(Policies.InventoryKeeper);

            FieldAsync<StringGraphType>(
                "addProduct",
                arguments: new QueryArguments(
                    new QueryArgument<NonNullGraphType<ProductInputType>> { Name = "product" }),
                resolve: async context =>
                {
                    var product = context.GetArgument<Product>("product");

                    return await rabbitMQPubSub.Handle(Channel.CreateProduct, SerializeObject(product));
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

                    return await rabbitMQPubSub.Handle(Channel.UpdateProduct, SerializeObject(product));
                }).AuthorizeWith(Policies.InventoryKeeper);

            FieldAsync<StringGraphType>(
                "deleteProduct",
                arguments: new QueryArguments(
                    new QueryArgument<NonNullGraphType<IdGraphType>> { Name = "productId" }),
                resolve: async context =>
                {
                    var productId = context.GetArgument<int>("productId");

                    return await rabbitMQPubSub.Handle(Channel.DeleteProduct, productId.ToString());
                }).AuthorizeWith(Policies.InventoryKeeper);

            FieldAsync<StringGraphType>(
                "register",
                arguments: new QueryArguments(
                    new QueryArgument<NonNullGraphType<RegisterInputType>> { Name = "register" }),
                resolve: async context =>
                {
                    var register = context.GetArgument<Register>("register");

                    return await rabbitMQPubSub.Handle(Channel.CreateAccount, SerializeObject(register));
                });

            FieldAsync<StringGraphType>(
                "login",
                arguments: new QueryArguments(
                    new QueryArgument<NonNullGraphType<LoginInputType>> { Name = "login" }),
                resolve: async context =>
                {
                    var login = context.GetArgument<Login>("login");

                    return await rabbitMQPubSub.Handle(Channel.LoginRequest, SerializeObject(login));
                });

            FieldAsync<StringGraphType>(
                "request",
                arguments: new QueryArguments(
                    new QueryArgument<NonNullGraphType<RequestInputType>> { Name = "request" }),
                resolve: async context =>
                {
                    var request = context.GetArgument<Request>("request");
                    request.Requester = GetUserId();
                    request.Status = "pending";

                    return await rabbitMQPubSub.Handle(Channel.PlaceRequest, SerializeObject(request));
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

                    return await rabbitMQPubSub
                        .Handle(Channel.RequestApproval, SerializeObject(request));
                }).AuthorizeWith(Policies.Approver);
        }
    }
}
