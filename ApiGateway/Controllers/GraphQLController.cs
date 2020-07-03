using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApiGateway.GraphQLObj.Parameters;
using GraphQL;
using GraphQL.Types;
using Microsoft.AspNetCore.Mvc;

namespace ApiGateway.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class GraphQLController : Controller
    {
        private readonly ISchema _schema;
        private readonly IDocumentExecuter _documentExecuter;

        public GraphQLController(ISchema schema, IDocumentExecuter documentExecuter)
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
                Inputs = inputs,
                OperationName = query.OperationName
            };

            var result = await _documentExecuter.ExecuteAsync(executionOptions);

            if (result.Errors?.Count > 0)
                return BadRequest(result.Errors);

            return Ok(result);
        }
    }
}