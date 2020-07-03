using GraphQL;
using GraphQL.Types;
using IdentityService.GraphQLConfig.Queries;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace IdentityService.Controllers
{
    [Route("[controller]")]
    [Route("graphql")]
    [ApiController]
    public class IdentityController : ControllerBase
    {
        private readonly ISchema _schema;
        private readonly IDocumentExecuter _documentExecuter;

        public IdentityController(ISchema schema, IDocumentExecuter documentExecuter)
        {
            _schema = schema;
            _documentExecuter = documentExecuter;
        }
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] GraphQLQueryParam query)
        {
            if (query == null)
                throw new ArgumentNullException(nameof(query));

            var inputs = query.Variables?.ToInputs();
            var executionOptions = new ExecutionOptions
            {
                Schema = _schema,
                Query = query.Query,
                Inputs = inputs
            };

            var result = await _documentExecuter.ExecuteAsync(executionOptions);

            if (result.Errors?.Count > 0)
                return BadRequest(result.Errors);

            return Ok(result);
        }
    }
}