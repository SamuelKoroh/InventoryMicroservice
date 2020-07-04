using GraphQL.Types;
using Microsoft.AspNetCore.Http;
using System.Linq;
using System.Security.Claims;

namespace ApiGateway.GraphQLObj
{
    public class BaseObjectGraphType : ObjectGraphType
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public BaseObjectGraphType(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public string GetUserId()
        {
            return _httpContextAccessor.HttpContext.User?.Claims?
                .FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier).Value;
        }
    }
}
