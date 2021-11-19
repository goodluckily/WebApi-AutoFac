using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplicationApi.AutoFacExtension;
using WebApplicationApi.IService;

namespace WebApplicationApi.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        //标注控制器里面 该属性是支持注入的
        [CustomProperty]
        private ITestAService testAService { get; set; }
        private ITestBService testBService { get; set; }
        private ITestCService testCService { get; set; }
        private ITestDService testDService { get; set; }
        public TestController()
        {
            Console.WriteLine("TestController");
        }

        [HttpGet]
        public Random Get()
        {
            testAService.show();
            var rng = new Random();
            return rng;
        }
    }
}
