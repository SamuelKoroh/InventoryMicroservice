using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApiGateway.AuthValidationRules;
using ApiGateway.GraphQLObj.Parameters;
using GraphQL;
using GraphQL.Types;
using GraphQL.Validation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ApiGateway.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [Authorize]
    public class GraphQLController : Controller
    {
        private readonly ISchema _schema;
        private readonly IDocumentExecuter _documentExecuter;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public GraphQLController(ISchema schema, 
            IDocumentExecuter documentExecuter, 
            IHttpContextAccessor httpContextAccessor
            )
        {
            _schema = schema;
            _documentExecuter = documentExecuter;
            _httpContextAccessor = httpContextAccessor;
        }
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Post([FromBody] GraphQLQueryParam query, [FromServices] IEnumerable<IValidationRule> validationRules)
        {
            if (query == null)
                throw new ArgumentNullException(nameof(query));

            var inputs = query.Variables?.ToInputs();
            var executionOptions = new ExecutionOptions
            {
                Schema = _schema,
                Query = query.Query,
                Inputs = inputs,
                OperationName = query.OperationName,
                ValidationRules = validationRules,
                UserContext = _httpContextAccessor.HttpContext.User
            };

            var result = await _documentExecuter.ExecuteAsync(executionOptions);

            if (result.Errors?.Count > 0)
                return BadRequest(result.Errors);

            return Ok(result);
        }
    }
}