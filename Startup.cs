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
            //AutoFac �ṩ������֧��
            //1   �滻���������滻����
            //1.1 ����ָ���������� ����������
            services.Replace(ServiceDescriptor.Transient<IControllerActivator, ServiceBasedControllerActivator>());


            #region һ������ע�� ��ʶ
            IServiceCollection serviceDescriptors = new ServiceCollection();
            serviceDescriptors.AddScoped<ITestAService, TestAService>();
            ServiceProvider serviceProvider = serviceDescriptors.BuildServiceProvider();

            ////1.ͬһ�������� ��ȡ��ʵ�� ʱͬһ��
            //var testA = (ITestAService)serviceProvider.GetService<ITestAService>();
            //var testA1 = (ITestAService)serviceProvider.GetService<ITestAService>();
            //var isEq = object.ReferenceEquals(testA, testA1);

            ////2.�ٴ�Buildʱ ��Χ�������� �Ѿ������˱仯
            //ServiceProvider serviceProvider1 = serviceDescriptors.BuildServiceProvider();
            //var testA2 = (ITestAService)serviceProvider1.GetService<ITestAService>();

            //var isEq1 = object.ReferenceEquals(testA1, testA2);

            //testA.show();
            #endregion


            #region AutoFac���� ʹ��
            ////����������
            //ContainerBuilder containerBuilder = new ContainerBuilder();
            ////
            //containerBuilder.RegisterType<TestAService>().As<ITestAService>();
            //IContainer container = containerBuilder.Build();//����
            ////��ȡ����
            //ITestAService testA3 = container.Resolve<ITestAService>();
            //testA3.show();

            #endregion

            #region AutoFac���� Ĭ��֧�ֹ��캯��ע��
            ContainerBuilder containerBuilder1 = new ContainerBuilder();
            containerBuilder1.RegisterType<TestAService>().As<ITestAService>();
            containerBuilder1.RegisterType<TestBService>().As<ITestBService>();
            IContainer container1 = containerBuilder1.Build();

            ITestBService testB = container1.Resolve<ITestBService>();
            testB.show();

            #endregion

            #region AutoFac���� ����ע�� PropertiesAutowired()
            ContainerBuilder containerBuilder2 = new ContainerBuilder();
            containerBuilder2.RegisterType<TestAService>().As<ITestAService>();
            containerBuilder2.RegisterType<TestBService>().As<ITestBService>();
            containerBuilder2.RegisterType<TestCService>().As<ITestCService>().PropertiesAutowired();//֧������ע��ķ���
            IContainer container2 = containerBuilder2.Build();

            ITestCService testC = container2.Resolve<ITestCService>();
            testC.show();
            #endregion

            #region AutoFac���� ����ע�� 
            ContainerBuilder containerBuilder3 = new ContainerBuilder();
            containerBuilder3.RegisterType<TestAService>().As<ITestAService>();
            containerBuilder3.RegisterType<TestBService>().As<ITestBService>();
            containerBuilder3.RegisterType<TestCService>().As<ITestCService>().PropertiesAutowired();//֧������ע��ķ���

            //����ע��
            containerBuilder3.RegisterType<TestDService>().OnActivated(a => a.Instance.SetSevice(a.Context.Resolve<ITestAService>())).As<ITestDService>();


            IContainer container3 = containerBuilder3.Build();

            ITestDService testD = container3.Resolve<ITestDService>();
            testD.show();
            #endregion


            #region AutoFac �������� ˲ʱ(ÿ�λ�ȡ������һ��ȫ�µ�ʵ��)

            ContainerBuilder containerBuilder4 = new ContainerBuilder();
            containerBuilder4.RegisterType<TestAService>().As<ITestAService>().InstancePerRequest();

            //InstancePerDependency ˲ʱ
            //InstancePerLifetimeScope()  ��Χ
            // InstancePerMatchingLifetimeScope("name����") *ƥ�� name* �������ڷ�Χʵ��
            //SingleInstance  ����
            //InstancePerRequest ÿһ������,һ��ʵ��

            IContainer container4 = containerBuilder4.Build();

            ITestAService testA4 = container4.Resolve<ITestAService>();
            ITestAService testA5 = container4.Resolve<ITestAService>();
            var IsEq = object.ReferenceEquals(testA4, testA5);
            Console.WriteLine(IsEq);

            #endregion


            #region AutoFac ֧�������ļ� (��ȫ�Ͽ��� ʵ�������)
            //��װ ֧�ֵ�Nuget��
            //1.Autofac.Extensions.DependencyInjection
            //2.Autofac.Configuration



            ////ҵ���߼������ڳ��������ռ�
            //Assembly service = Assembly.Load("Fenge.BLL");
            ////�ӿڲ����ڳ��������ռ�
            //Assembly repository = Assembly.Load("Fenge.DAL");
            ////�Զ�ע��
            //builder.RegisterAssemblyTypes(service, repository)
            //    .Where(t => t.Name.EndsWith("BLL"))
            //    .AsImplementedInterfaces();

            //builder.RegisterAssemblyTypes(service, repository)
            //   .Where(t => t.Name.EndsWith("DAL"))
            //   .AsImplementedInterfaces();

            ////ע��ִ�������IRepository�ӿڵ�Repository��ӳ��
            //builder.RegisterGeneric(typeof(BaseBLL<>))
            //    //InstancePerDependency��Ĭ��ģʽ��ÿ�ε��ã���������ʵ��������ÿ�����󶼴���һ���µĶ���
            //    .As(typeof(IBaseBLL<>)).InstancePerDependency();

            //builder.RegisterGeneric(typeof(BaseDAL<>))
            ////InstancePerDependency��Ĭ��ģʽ��ÿ�ε��ã���������ʵ��������ÿ�����󶼴���һ���µĶ���
            //.As(typeof(IBaseDAL<>)).InstancePerDependency();

            #endregion


            services.AddControllers().AddControllersAsServices();
        }

        public static void MyBuild(ContainerBuilder builder)
        {
            //Assembly[] assemblies = Helpers.ReflectionHelper.GetAllAssembliesWeb();
            //Assembly assemblies = Assembly.Load("WebApplicationApi");
            //ע��ִ� && Service
            //var loadAssembly =  builder.RegisterAssemblyTypes(assemblies)//���������о����ࣨconcrete classes��
            //    .Where(cc => cc.Name.EndsWith("Repository") |//ɸѡ
            //                 cc.Name.EndsWith("Service"))
            //    .PublicOnly()//ֻҪpublic����Ȩ�޵�
            //    .Where(cc => cc.IsClass)//ֻҪclass�ͣ���ҪΪ���ų�ֵ��interface���ͣ�
            //    .AsImplementedInterfaces();//�Զ�����ʵ�ֵ����нӿ����ͱ�¶������IDisposable�ӿڣ�



            //var controllerTypes = typeof(Startup).GetTypeInfo().Assembly.DefinedTypes.
            //    Where(x => x.IsClass && typeof(ControllerBase).GetTypeInfo().IsAssignableFrom(x)).
            //    Select(x => x.AsType()).
            //    ToArray();

            //builder.RegisterTypes(controllerTypes).AsSelf();

            //var container = builder.Build();
            //return container.Resolve<IServiceProvider>();

            //����
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
            //containerBuilder3.RegisterType<TestCService>().As<ITestCService>().PropertiesAutowired();//֧������ע��ķ���

            ////ע�᷺�Ͳִ�
            //builder.RegisterGeneric(typeof(BaseRepository<>)).As(typeof(IBaseRepository<>));

            ////ע��Controller
            ////����1���Լ����ݷ���ע��
            ////builder.RegisterAssemblyTypes(assemblies)
            ////    .Where(cc => cc.Name.EndsWith("Controller"))
            ////    .AsSelf();
            ////����2����AutoFac�ṩ��ר������ע��MvcController����չ����
            //Assembly mvcAssembly = assemblies.FirstOrDefault(x => x.FullName.Contains(".NetFrameworkMvc"));
            //builder.RegisterControllers(mvcAssembly);
        }


        /// <summary>
        /// AutoFac �Լ������������� ����ע��
        /// 1.����ע�� ���ַ���
        /// 2.ServiceCollection ע���,Ҳͬ���ǿ���ʹ�õ�
        /// 3.��֧�ֿ����� ���������ע��
        /// </summary>
        /// <param name="builder"></param>
        public void ConfigureContainer(ContainerBuilder builder)
        {
            Assembly serviceDLL = Assembly.Load(new AssemblyName("WebApplicationApi"));

            //1.Service ��׺��ķ��� ע��
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
                        .AsImplementedInterfaces()//�Զ�����ʵ�ֵ����нӿ����ͱ�¶������IDisposable�ӿڣ�
                        .InstancePerLifetimeScope()
                        .PropertiesAutowired();//֧������ע��ķ���;
                }
            }

            //2   ������Ҫ��Service,�����ṩ֧�� services.Replace(.......)
            //2.1 ������ʵ����ע�� 
            var controllerTypes = typeof(Startup).GetTypeInfo().Assembly.DefinedTypes.
                Where(x => x.IsClass && typeof(ControllerBase).GetTypeInfo().IsAssignableFrom(x)).
                Select(x => x.AsType()).
                ToArray();
            builder.RegisterTypes(controllerTypes)
                //֧������ע��ķ���;
                //CustomPropertySelector ������Щ�ǿ��Ա�ע��� (ָ����������ע���֧��)
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
