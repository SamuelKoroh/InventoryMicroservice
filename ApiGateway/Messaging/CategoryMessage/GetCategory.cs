using ApiGateway.Domain.Models;
using MassTransit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiGateway.Messaging.CategoryMessage
{
    public class GetCategory
    {
        private static IBusControl _bus;

        public GetCategory(IBusControl bus)
        {
            _bus = bus;
        }
        public static async Task<IEnumerable<Category>> GetCategories() 
        {
            var uri = new Uri("rabbitmq://localhost/get-categories");
            var endPoint = await _bus.GetSendEndpoint(uri);
            await endPoint.Send("");

            return new List<Category>
            {
                new Category{Id = 1, Name="Category 1"},
                new Category{Id = 2, Name="Category 2"},
                new Category{Id = 3, Name="Category 3"},
            };
        }
    }
}
