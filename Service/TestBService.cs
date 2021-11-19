using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplicationApi.IService;

namespace WebApplicationApi.Service
{
    public class TestBService : ITestBService
    {
        public TestBService(ITestCService testC)
        {
            Console.WriteLine("BBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBB");
            testC.show();
        }
        public void show()
        {
            Console.WriteLine("BBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBB==>Show()");
        }
    }
}
