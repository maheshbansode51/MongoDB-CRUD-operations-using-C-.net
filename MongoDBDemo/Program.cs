using MongoDBDAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MongoDBDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            OrderRepository repo = new OrderRepository();

            repo.CreateOrder().GetAwaiter().GetResult();

            var orders = repo.GetOrder().GetAwaiter().GetResult();

            var filteredOrders = repo.GetOrderByBorough().GetAwaiter().GetResult();


            var sortedOrders = repo.SortOrders().GetAwaiter().GetResult();

            repo.UpdateOrder().GetAwaiter().GetResult();

            repo.DeleteOrders().GetAwaiter().GetResult();

            repo.DeleteAllOrders().GetAwaiter().GetResult();         
                    
            Console.ReadKey();
        }
    }
}
