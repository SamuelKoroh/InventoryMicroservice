using GraphQL.Authorization;
using System.Collections.Generic;
using System.Security.Claims;

namespace ApiGateway.AuthValidationRules
{
    public class GraphQLUserContext : Dictionary<string, object>, IProvideClaimsPrincipal
    {
        public ClaimsPrincipal User { get; set; }
    }
}
