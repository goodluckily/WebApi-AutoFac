using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplicationApi.IService;

namespace WebApplicationApi.Service
{
    public class TestDService : ITestDService
    {
        public ITestAService _testA = null;

        //构造函数注入
        public TestDService(ITestAService testA)
        {
            _testA = testA;
            Console.WriteLine($"{testA.GetType().Name},被构造");
        }

        //方法注入
        public void SetSevice(ITestAService testA) 
        {
            _testA = testA;
        }

        public void show()
        {
            _testA.show();
            Console.WriteLine($"{_testA.GetType().Name},被构造");
            Console.WriteLine("DDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDD==>Show()");
        }
    }
}
