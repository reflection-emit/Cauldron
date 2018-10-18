using Cauldron;
using Cauldron.Interception;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Reflection;
using System.Threading.Tasks;

namespace MainTests.BasicInterceptors
{
    [TestClass]
    public class Bug67_Async_Method_Validation_Tests
    {
        /* Also see https://github.com/Capgemini/Cauldron/issues/67
         Outside try-catch-finally | Try | Catch | Finally | Failure
                                -- | --  |    -- |      -- | --
        1                        3 |  0  |    0  |      0  | 0
        2                        2 |  1  |    0  |      0  | 1
        3                        2 |  0  |    1  |      0  | 1
        4                        2 |  0  |    0  |      1  | 0
        5                        1 |  2  |    0  |      0  | 0
        6                        1 |  0  |    2  |      0  | 0
        7                        1 |  0  |    0  |      2  | 0
        8                        1 |  1  |    1  |      0  | 0
        9                        1 |  1  |    0  |      1  | 1
        10                       1 |  0  |    1  |      1  | 1
        11                       0 |  3  |    0  |      0  | 0
        12                       0 |  0  |    3  |      0  | 0
        13                       0 |  0  |    0  |      3  | 0
        14                       0 |  2  |    1  |      0  | 0
        15                       0 |  2  |    0  |      1  | 0
        16                       0 |  1  |    2  |      0  | 1
        17                       0 |  1  |    0  |      2  | 1
        18                       0 |  1  |    1  |      1  | 0
        19                       0 |  0  |    2  |      1  | 0
        20                       0 |  0  |    1  |      2  | 1
        */

        // These tests will only verify if the modified IL code is executable / valid.

        [TestMethod]
        public void Bug67_Async_Method_With_Try_Catch_Test()
        {
            this.Case_1_Test_MethodAsync().RunSync();
            this.Case_2_Test_MethodAsync().RunSync();
            this.Case_3_Test_MethodAsync().RunSync();
            this.Case_4_Test_MethodAsync().RunSync();
            this.Case_5_Test_MethodAsync().RunSync();
            this.Case_6_Test_MethodAsync().RunSync();
            this.Case_7_Test_MethodAsync().RunSync();
            this.Case_8_Test_MethodAsync().RunSync();
            this.Case_9_Test_MethodAsync().RunSync();
            this.Case_10_Test_MethodAsync().RunSync();
            this.Case_11_Test_MethodAsync().RunSync();
            this.Case_12_Test_MethodAsync().RunSync();
            this.Case_13_Test_MethodAsync().RunSync();
            this.Case_14_Test_MethodAsync().RunSync();
            this.Case_15_Test_MethodAsync().RunSync();
            this.Case_16_Test_MethodAsync().RunSync();
            this.Case_17_Test_MethodAsync().RunSync();
            this.Case_18_Test_MethodAsync().RunSync();
            this.Case_19_Test_MethodAsync().RunSync();
            this.Case_20_Test_MethodAsync().RunSync();
            Assert.IsTrue(true);
        }

        [TestingAnnotation]
        public async Task Case_20_Test_MethodAsync()
        {
            try
            {
            }
            catch (Exception)
            {
                await Task.Delay(1);
            }
            finally
            {
                await Task.Delay(1);
                await Task.Delay(1);
            }
        }

        [TestingAnnotation]
        public async Task Case_19_Test_MethodAsync()
        {
            try
            {
            }
            catch (Exception)
            {
                await Task.Delay(1);
                await Task.Delay(1);
            }
            finally
            {
                await Task.Delay(1);
            }
        }

        [TestingAnnotation]
        public async Task Case_18_Test_MethodAsync()
        {
            try
            {
                await Task.Delay(1);
            }
            catch (Exception)
            {
                await Task.Delay(1);
            }
            finally
            {
                await Task.Delay(1);
            }
        }

        [TestingAnnotation]
        public async Task Case_17_Test_MethodAsync()
        {
            try
            {
                await Task.Delay(1);
            }
            catch (Exception)
            {
            }
            finally
            {
                await Task.Delay(1);
                await Task.Delay(1);
            }
        }

        [TestingAnnotation]
        public async Task Case_16_Test_MethodAsync()
        {
            try
            {
                await Task.Delay(1);
            }
            catch (Exception)
            {
                await Task.Delay(1);
                await Task.Delay(1);
            }
        }

        [TestingAnnotation]
        public async Task Case_15_Test_MethodAsync()
        {
            try
            {
                await Task.Delay(1);
                await Task.Delay(1);
            }
            catch (Exception)
            {
            }
            finally
            {
                await Task.Delay(1);
            }
        }

        [TestingAnnotation]
        public async Task Case_14_Test_MethodAsync()
        {
            try
            {
                await Task.Delay(1);
                await Task.Delay(1);
            }
            catch (Exception)
            {
                await Task.Delay(1);
            }
        }

        [TestingAnnotation]
        public async Task Case_13_Test_MethodAsync()
        {
            try
            {
            }
            catch (Exception)
            {
            }
            finally
            {
                await Task.Delay(1);
                await Task.Delay(1);
                await Task.Delay(1);
            }
        }

        [TestingAnnotation]
        public async Task Case_12_Test_MethodAsync()
        {
            try
            {
            }
            catch (Exception)
            {
                await Task.Delay(1);
                await Task.Delay(1);
                await Task.Delay(1);
            }
        }

        [TestingAnnotation]
        public async Task Case_11_Test_MethodAsync()
        {
            try
            {
                await Task.Delay(1);
                await Task.Delay(1);
                await Task.Delay(1);
            }
            catch (Exception)
            {
            }
        }

        [TestingAnnotation]
        public async Task Case_10_Test_MethodAsync()
        {
            await Task.Delay(1);

            try
            {
            }
            catch (Exception)
            {
                await Task.Delay(1);
            }
            finally
            {
                await Task.Delay(1);
            }
        }

        [TestingAnnotation]
        public async Task Case_9_Test_MethodAsync()
        {
            await Task.Delay(1);

            try
            {
                await Task.Delay(1);
            }
            catch (Exception)
            {
            }
            finally
            {
                await Task.Delay(1);
            }
        }

        [TestingAnnotation]
        public async Task Case_8_Test_MethodAsync()
        {
            await Task.Delay(1);

            try
            {
                await Task.Delay(1);
            }
            catch (Exception)
            {
                await Task.Delay(1);
            }
        }

        [TestingAnnotation]
        public async Task Case_7_Test_MethodAsync()
        {
            await Task.Delay(1);

            try
            {
            }
            catch (Exception)
            {
            }
            finally
            {
                await Task.Delay(1);
                await Task.Delay(1);
            }
        }

        [TestingAnnotation]
        public async Task Case_6_Test_MethodAsync()
        {
            await Task.Delay(1);

            try
            {
            }
            catch (Exception)
            {
                await Task.Delay(1);
                await Task.Delay(1);
            }
        }

        [TestingAnnotation]
        public async Task Case_5_Test_MethodAsync()
        {
            await Task.Delay(1);

            try
            {
                await Task.Delay(1);
                await Task.Delay(1);
            }
            catch (Exception)
            {
            }
        }

        [TestingAnnotation]
        public async Task Case_4_Test_MethodAsync()
        {
            await Task.Delay(1);
            await Task.Delay(1);

            try
            {
            }
            catch (Exception)
            {
            }
            finally
            {
                await Task.Delay(1);
            }
        }

        [TestingAnnotation]
        public async Task Case_3_Test_MethodAsync()
        {
            await Task.Delay(1);
            await Task.Delay(1);

            try
            {
                await Task.Delay(1);
            }
            catch (Exception)
            {
                await Task.Delay(1);
            }
        }

        [TestingAnnotation]
        public async Task Case_2_Test_MethodAsync()
        {
            await Task.Delay(1);
            await Task.Delay(1);

            try
            {
                await Task.Delay(1);
            }
            catch (Exception)
            {
            }
        }

        [TestingAnnotation]
        public async Task Case_1_Test_MethodAsync()
        {
            await Task.Delay(1);
            await Task.Delay(1);
            await Task.Delay(1);

            try
            {
            }
            catch (Exception)
            {
            }
        }
    }

    [InterceptorOptions(AlwaysCreateNewInstance = true)]
    public class TestingAnnotation : Attribute, IMethodInterceptor
    {
        public void OnEnter(Type declaringType, object instance, MethodBase methodbase, object[] values)
        {
        }

        public bool OnException(Exception e)
        {
            return true;
        }

        public void OnExit()
        {
        }
    }
}