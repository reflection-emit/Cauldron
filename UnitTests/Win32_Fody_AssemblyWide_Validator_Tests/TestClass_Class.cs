using Cauldron.Interception;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;

namespace Cauldron.UnitTest.AssemblyValidation
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Constructor | AttributeTargets.Method | AttributeTargets.Property, AllowMultiple = true)]
    public sealed class Exception_Class_Interceptor : Attribute, IMethodInterceptor, ISyncRoot, IConstructorInterceptor, IPropertyInterceptor, ISimpleMethodInterceptor
    {
        [AssignMethod("ACoolIntMethod", true)]
        public Func<int> func = null;

        [AssignMethod("ACoolIntMethodS", true)]
        public Func<int> func2 = null;

        [AssignMethod("get_Bla", true)]
        public Func<string> func3 = null;

        public object SyncRoot { get; set; }

        public void OnBeforeInitialization(Type declaringType, MethodBase methodbase, object[] values)
        {
        }

        public void OnEnter(Type declaringType, object instance, MethodBase methodbase, object[] values)
        {
        }

        public bool OnException(Exception e) => true;

        public void OnExit()
        {
        }

        public void OnGet(PropertyInterceptionInfo propertyInterceptionInfo, object value)
        {
        }

        public void OnInitialize(PropertyInterceptionInfo propertyInterceptionInfo, object value)
        {
        }

        public bool OnSet(PropertyInterceptionInfo propertyInterceptionInfo, object oldValue, object newValue)
        {
            return false;
        }
    }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Constructor | AttributeTargets.Method | AttributeTargets.Property)]
    public sealed class Exception_Class_Interceptor2 : Attribute, ISimpleMethodInterceptor
    {
        public void OnEnter(Type declaringType, object instance, MethodBase methodbase, object[] values)
        {
        }
    }

    public class TestClass_One_Class
    {
        [Exception_Class_Interceptor]
        static TestClass_One_Class()
        {
        }

        [Exception_Class_Interceptor]
        public TestClass_One_Class()
        {
        }

        [Exception_Class_Interceptor]
        public string Bla { get; set; }

        public static int ACoolIntMethodS()
        {
            return 9;
        }

        [Exception_Class_Interceptor]
        [Exception_Class_Interceptor]
        public int ACoolIntMethod()
        {
            var ss = "Jkj";
            return Blabla(ss);
        }

        [Exception_Class_Interceptor2]
        public int ACoolIntMethod(int p)
        {
            return 3;
        }

        public Task ATaskAsync() => Task.FromResult(0);

        public void AVeryVoidMethod(string name)
        {
        }

        public int Blabla(object p)
        {
            return 3;
        }

        public async Task BTaskAsync()
        {
            await Task.Run(() => { });
        }

        public async Task<int> CTaskAsync()
        {
            await Task.Run(() => { });
            return 12;
        }

        public async Task<TestClass_One_Class> DTaskAsync()
        {
            await Task.Run(() => { });
            return new TestClass_One_Class();
        }

        public Task<TestClass_One_Class> ETaskAsync()
        {
            return DTaskAsync();
        }

        public string OneSizeFitsAll(long l)
        {
            return "uii";
        }

        public string OneSizeFitsAll(IEnumerable<string> target) => "kl";
    }

    internal class TestClass_Two_Class
    {
        public int ACoolIntMethod()
        {
            return 9;
        }

        [Exception_Class_Interceptor2]
        public int ACoolIntMethod(int p)
        {
            return 3;
        }

        public void AVeryVoidMethod(string name)
        {
        }

        public bool Bla(Exception e)
        {
            return true;
        }

        public async Task BTaskAsync()
        {
            try
            {
                await Task.Run(() => { });
            }
            catch (Exception)
            {
            }
        }

        public async Task<TestClass_One_Class> DTaskAsync()
        {
            await Task.Run(() => { });
            return new TestClass_One_Class();
        }

        public Task<TestClass_One_Class> ETaskAsync()
        {
            try
            {
                return DTaskAsync();
            }
            catch (Exception e)
            {
                if (Bla(e) & Bla(e))
                    throw;

                return Task.FromResult(new TestClass_One_Class());
            }
        }

        public string OneSizeFitsAll(long l)
        {
            return "uii";
        }

        public string OneSizeFitsAll(IEnumerable<string> target) => "kl";
    }
}