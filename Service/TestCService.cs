using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplicationApi.IService;

namespace WebApplicationApi.Service
{
    public class TestCService : ITestCService
    {
        public TestCService()
        {
            Console.WriteLine("CCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCC");
        }
        public void show()
        {
            Console.WriteLine("CCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCC==>Show()");
        }
    }
}
