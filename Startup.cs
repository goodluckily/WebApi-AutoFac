using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplicationApi.IService;
using WebApplicationApi.Service;
using Autofac;
using System.Reflection;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.Extensions.DependencyInjection.Extensions;
using WebApplicationApi.AutoFacExtension;

namespace WebApplicationApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //AutoFac 提供控制器支持
            //1   替换控制器的替换规则
            //1.1 可以指定控制器让 容器来创建
            services.Replace(ServiceDescriptor.Transient<IControllerActivator, ServiceBasedControllerActivator>());


            #region 一般容器注入 初识
            IServiceCollection serviceDescriptors = new ServiceCollection();
            serviceDescriptors.AddScoped<ITestAService, TestAService>();
            ServiceProvider serviceProvider = serviceDescriptors.BuildServiceProvider();

            ////1.同一个作用域 获取的实例 时同一个
            //var testA = (ITestAService)serviceProvider.GetService<ITestAService>();
            //var testA1 = (ITestAService)serviceProvider.GetService<ITestAService>();
            //var isEq = object.ReferenceEquals(testA, testA1);

            ////2.再次Build时 范围声明周期 已经发生了变化
            //ServiceProvider serviceProvider1 = serviceDescriptors.BuildServiceProvider();
            //var testA2 = (ITestAService)serviceProvider1.GetService<ITestAService>();

            //var isEq1 = object.ReferenceEquals(testA1, testA2);

            //testA.show();
            #endregion


            #region AutoFac容器 使用
            ////容器建造者
            //ContainerBuilder containerBuilder = new ContainerBuilder();
            ////
            //containerBuilder.RegisterType<TestAService>().As<ITestAService>();
            //IContainer container = containerBuilder.Build();//构建
            ////获取服务
            //ITestAService testA3 = container.Resolve<ITestAService>();
            //testA3.show();

            #endregion

            #region AutoFac容器 默认支持构造函数注入
            ContainerBuilder containerBuilder1 = new ContainerBuilder();
            containerBuilder1.RegisterType<TestAService>().As<ITestAService>();
            containerBuilder1.RegisterType<TestBService>().As<ITestBService>();
            IContainer container1 = containerBuilder1.Build();

            ITestBService testB = container1.Resolve<ITestBService>();
            testB.show();

            #endregion

            #region AutoFac容器 属性注入 PropertiesAutowired()
            ContainerBuilder containerBuilder2 = new ContainerBuilder();
            containerBuilder2.RegisterType<TestAService>().As<ITestAService>();
            containerBuilder2.RegisterType<TestBService>().As<ITestBService>();
            containerBuilder2.RegisterType<TestCService>().As<ITestCService>().PropertiesAutowired();//支持属性注入的方法
            IContainer container2 = containerBuilder2.Build();

            ITestCService testC = container2.Resolve<ITestCService>();
            testC.show();
            #endregion

            #region AutoFac容器 方法注入 
            ContainerBuilder containerBuilder3 = new ContainerBuilder();
            containerBuilder3.RegisterType<TestAService>().As<ITestAService>();
            containerBuilder3.RegisterType<TestBService>().As<ITestBService>();
            containerBuilder3.RegisterType<TestCService>().As<ITestCService>().PropertiesAutowired();//支持属性注入的方法

            //属性注入
            containerBuilder3.RegisterType<TestDService>().OnActivated(a => a.Instance.SetSevice(a.Context.Resolve<ITestAService>())).As<ITestDService>();


            IContainer container3 = containerBuilder3.Build();

            ITestDService testD = container3.Resolve<ITestDService>();
            testD.show();
            #endregion


            #region AutoFac 声明周期 瞬时(每次获取对象都是一个全新的实例)

            ContainerBuilder containerBuilder4 = new ContainerBuilder();
            containerBuilder4.RegisterType<TestAService>().As<ITestAService>().InstancePerRequest();

            //InstancePerDependency 瞬时
            //InstancePerLifetimeScope()  范围
            // InstancePerMatchingLifetimeScope("name名称") *匹配 name* 声明周期范围实例
            //SingleInstance  单例
            //InstancePerRequest 每一个请求,一个实例

            IContainer container4 = containerBuilder4.Build();

            ITestAService testA4 = container4.Resolve<ITestAService>();
            ITestAService testA5 = container4.Resolve<ITestAService>();
            var IsEq = object.ReferenceEquals(testA4, testA5);
            Console.WriteLine(IsEq);

            #endregion


            #region AutoFac 支持配置文件 (完全断开对 实体的依赖)
            //安装 支持的Nuget包
            //1.Autofac.Extensions.DependencyInjection
            //2.Autofac.Configuration



            ////业务逻辑层所在程序集命名空间
            //Assembly service = Assembly.Load("Fenge.BLL");
            ////接口层所在程序集命名空间
            //Assembly repository = Assembly.Load("Fenge.DAL");
            ////自动注入
            //builder.RegisterAssemblyTypes(service, repository)
            //    .Where(t => t.Name.EndsWith("BLL"))
            //    .AsImplementedInterfaces();

            //builder.RegisterAssemblyTypes(service, repository)
            //   .Where(t => t.Name.EndsWith("DAL"))
            //   .AsImplementedInterfaces();

            ////注册仓储，所有IRepository接口到Repository的映射
            //builder.RegisterGeneric(typeof(BaseBLL<>))
            //    //InstancePerDependency：默认模式，每次调用，都会重新实例化对象；每次请求都创建一个新的对象；
            //    .As(typeof(IBaseBLL<>)).InstancePerDependency();

            //builder.RegisterGeneric(typeof(BaseDAL<>))
            ////InstancePerDependency：默认模式，每次调用，都会重新实例化对象；每次请求都创建一个新的对象；
            //.As(typeof(IBaseDAL<>)).InstancePerDependency();

            #endregion


            services.AddControllers().AddControllersAsServices();
        }

        public static void MyBuild(ContainerBuilder builder)
        {
            //Assembly[] assemblies = Helpers.ReflectionHelper.GetAllAssembliesWeb();
            //Assembly assemblies = Assembly.Load("WebApplicationApi");
            //注册仓储 && Service
            //var loadAssembly =  builder.RegisterAssemblyTypes(assemblies)//程序集内所有具象类（concrete classes）
            //    .Where(cc => cc.Name.EndsWith("Repository") |//筛选
            //                 cc.Name.EndsWith("Service"))
            //    .PublicOnly()//只要public访问权限的
            //    .Where(cc => cc.IsClass)//只要class型（主要为了排除值和interface类型）
            //    .AsImplementedInterfaces();//自动以其实现的所有接口类型暴露（包括IDisposable接口）



            //var controllerTypes = typeof(Startup).GetTypeInfo().Assembly.DefinedTypes.
            //    Where(x => x.IsClass && typeof(ControllerBase).GetTypeInfo().IsAssignableFrom(x)).
            //    Select(x => x.AsType()).
            //    ToArray();

            //builder.RegisterTypes(controllerTypes).AsSelf();

            //var container = builder.Build();
            //return container.Resolve<IServiceProvider>();

            //构建
            //IContainer builder1111 = builder.Build();

            //ITestAServiceService testA1111111 = builder1111.Resolve<ITestAServiceService>();
            //testA1111111.show();

            //ITestBService testB11111 = builder1111.Resolve<ITestBService>();
            //testB11111.show();


            //ITestCService testC11111 = builder1111.Resolve<ITestCService>();
            //testC11111.show();

            //ITestDService testD11111 = builder1111.Resolve<ITestDService>();
            //testD11111.show();

            //containerBuilder3.RegisterType<TestAService>().As<ITestAServiceService>();
            //containerBuilder3.RegisterType<TestBService>().As<ITestBService>();
            //containerBuilder3.RegisterType<TestCService>().As<ITestCService>().PropertiesAutowired();//支持属性注入的方法

            ////注册泛型仓储
            //builder.RegisterGeneric(typeof(BaseRepository<>)).As(typeof(IBaseRepository<>));

            ////注册Controller
            ////方法1：自己根据反射注册
            ////builder.RegisterAssemblyTypes(assemblies)
            ////    .Where(cc => cc.Name.EndsWith("Controller"))
            ////    .AsSelf();
            ////方法2：用AutoFac提供的专门用于注册MvcController的扩展方法
            //Assembly mvcAssembly = assemblies.FirstOrDefault(x => x.FullName.Contains(".NetFrameworkMvc"));
            //builder.RegisterControllers(mvcAssembly);
        }


        /// <summary>
        /// AutoFac 自己会调用这个方法 进行注册
        /// 1.负责注册 各种服务
        /// 2.ServiceCollection 注册的,也同样是可以使用的
        /// 3.还支持控制器 里面的属性注入
        /// </summary>
        /// <param name="builder"></param>
        public void ConfigureContainer(ContainerBuilder builder)
        {
            Assembly serviceDLL = Assembly.Load(new AssemblyName("WebApplicationApi"));

            //1.Service 后缀类的反射 注入
            var serviceTypes = serviceDLL.GetTypes().Where(t => t.Name.EndsWith("Service") && !t.GetTypeInfo().IsAbstract);
            foreach (var serviceType in serviceTypes)
            {
                //var asdfasf = serviceType.Name;
                foreach (var interType in serviceType.GetInterfaces())
                {
                    var sname = serviceType.Name;
                    var Iname = interType.Name;
                    Console.WriteLine($"{sname}--->{Iname}");
                    builder.RegisterType(serviceType).As(interType).InstancePerDependency()
                        .AsImplementedInterfaces()//自动以其实现的所有接口类型暴露（包括IDisposable接口）
                        .InstancePerLifetimeScope()
                        .PropertiesAutowired();//支持属性注入的方法;
                }
            }

            //2   首先需要在Service,里面提供支持 services.Replace(.......)
            //2.1 控制器实例的注入 
            var controllerTypes = typeof(Startup).GetTypeInfo().Assembly.DefinedTypes.
                Where(x => x.IsClass && typeof(ControllerBase).GetTypeInfo().IsAssignableFrom(x)).
                Select(x => x.AsType()).
                ToArray();
            builder.RegisterTypes(controllerTypes)
                //支持属性注入的方法;
                //CustomPropertySelector 设置哪些是可以被注入的 (指定特性属性注入的支持)
                .PropertiesAutowired(new CustomPropertySelector());

        }


        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
