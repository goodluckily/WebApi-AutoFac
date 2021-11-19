using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplicationApi.IService;

namespace WebApplicationApi.Service
{
    public class TestAService : ITestAService
    {
        public ITestBService testB { get; set; }
        public ITestCService testC { get; set; }
        public TestAService()
        {
            Console.WriteLine("AAAAAAAAAAAAAAAAAAAAAAAA");
        }
        public void show()
        {
            testB.show();
            testC.show();
            Console.WriteLine("AAAAAAAAAAAAAAAAAAAAAAAA==>show()");
        }
    }
}
