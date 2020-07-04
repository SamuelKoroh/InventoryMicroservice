namespace ApiGateway.Configurations
{
    public class Channel
    {
        public const string CreateCategory = "create-category";
        public const string CategoryCreated = "category-created";
        public const string UpdateCategory = "update-category";
        public const string CategoryUpdated = "category-updated";
        public const string DeleteCategory = "delete-category";
        public const string CategoryDeleted = "category-deleted";

        public const string CreateProduct = "create-product";
        public const string ProductCreated = "product-created";
        public const string UpdateProduct = "update-product";
        public const string ProductUpdated = "product-updated";
        public const string DeleteProduct = "delete-product";
        public const string ProductDeleted = "product-deleted";

        public const string CreateAccount = "create-account";
        public const string AccountCreated = "account-created";
        public const string LoginRequest = "login-request";
        public const string LoginResponse = "login-response";

        public const string PlaceRequest = "place-request";
        public const string PlaceRequestResponse = "place-request-response";

        public const string RequestApproval = "approve-request";
        public const string RequestApprovalResponse = "approve-request-response";
    }
}
