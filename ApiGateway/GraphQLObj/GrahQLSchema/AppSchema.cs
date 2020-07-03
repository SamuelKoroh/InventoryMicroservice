using ApiGateway.GraphQLObj.GrahQLMutations;
using ApiGateway.GraphQLObj.GrahQLQueries;
using GraphQL;
using GraphQL.Types;

namespace ApiGateway.GraphQLObj.GrahQLSchema
{
    public class AppSchema : Schema
    {
        public AppSchema(IDependencyResolver resolver)
            : base(resolver)
        {
            Query = resolver.Resolve<RootQuery>();
            Mutation = resolver.Resolve<RootMutation>();
        }
    }
}
