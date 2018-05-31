using Cauldron.Interception.Cecilator;
using Mono.Cecil;
using System;

namespace Cauldron.Interception.Fody
{
    public partial class BuilderTypeExtensionsReflection : TypeSystemExBase
    {
        /// <summary>
        /// Represents the following method:
        /// <para />
        /// System.Object CreateInstance(System.Type, System.Object[])<para/>
        /// System.Object CreateInstance(System.Reflection.ConstructorInfo, System.Object[])<para/>
        /// </summary>
        public Method GetMethod_CreateInstance()
        {
            if (this.var_createinstance_0_2 == null)
                this.var_createinstance_0_2 = this.builderType.GetMethod("CreateInstance", true, "System.Type", "System.Object[]").Import();

            return this.var_createinstance_0_2;
        }
    }

    public partial class BuilderTypeFactory : TypeSystemExBase
    {
        private Method _OnObjectCreation;

        public Method GetMethod_Create(TypeReference pcontractName)
        {
            if (typeof(System.String).AreEqual(pcontractName))
            {
                if (this.var_create_0_2 == null)
                    this.var_create_0_2 = this.builderType.GetMethod("Create", true, "System.String", "System.Object[]").Import();

                return this.var_create_0_2;
            }

            if (typeof(System.Type).AreEqual(pcontractName))
            {
                if (this.var_create_1_2 == null)
                    this.var_create_1_2 = this.builderType.GetMethod("Create", true, "System.Type", "System.Object[]").Import();

                return this.var_create_1_2;
            }

            throw new Exception("Method with defined parameters not found.");
        }

        public Method GetMethod_CreateFirst(TypeReference pcontractType)
        {
            if (typeof(System.Type).AreEqual(pcontractType))
            {
                if (this.var_createfirst_0_2 == null)
                    this.var_createfirst_0_2 = this.builderType.GetMethod("CreateFirst", true, "System.String", "System.Object[]").Import();

                return this.var_createfirst_0_2;
            }

            if (typeof(System.String).AreEqual(pcontractType))
            {
                if (this.var_createfirst_1_2 == null)
                    this.var_createfirst_1_2 = this.builderType.GetMethod("CreateFirst", true, "System.Type", "System.Object[]").Import();

                return this.var_createfirst_1_2;
            }

            throw new Exception("Method with defined parameters not found.");
        }

        public Method GetMethod_CreateMany(TypeReference pcontractName)
        {
            if (typeof(System.String).AreEqual(pcontractName))
            {
                if (this.var_createmany_0_2 == null)
                    this.var_createmany_0_2 = this.builderType.GetMethod("CreateMany", true, "System.String", "System.Object[]").Import();

                return this.var_createmany_0_2;
            }

            if (typeof(System.Type).AreEqual(pcontractName))
            {
                if (this.var_createmany_1_2 == null)
                    this.var_createmany_1_2 = this.builderType.GetMethod("CreateMany", true, "System.Type", "System.Object[]").Import();

                return this.var_createmany_1_2;
            }

            throw new Exception("Method with defined parameters not found.");
        }

        public Method GetMethod_CreateManyOrdered(TypeReference pcontractName)
        {
            if (typeof(System.String).AreEqual(pcontractName))
            {
                if (this.var_createmanyordered_0_2 == null)
                    this.var_createmanyordered_0_2 = this.builderType.GetMethod("CreateManyOrdered", true, "System.String", "System.Object[]").Import();

                return this.var_createmanyordered_0_2;
            }

            if (typeof(System.Type).AreEqual(pcontractName))
            {
                if (this.var_createmanyordered_1_2 == null)
                    this.var_createmanyordered_1_2 = this.builderType.GetMethod("CreateManyOrdered", true, "System.Type", "System.Object[]").Import();

                return this.var_createmanyordered_1_2;
            }

            throw new Exception("Method with defined parameters not found.");
        }
    }

    public partial class BuilderTypeIFactoryTypeInfo : TypeSystemExBase
    {
    }

    public partial class BuilderTypeInterceptionRuleAttribute : TypeSystemExBase
    {
        private Method constructor;

        public Method GetConstructor()
        {
            if (this.constructor == null)
                this.constructor = this.builderType.GetMethod("Equals", 2, true).Import();

            return this.constructor;
        }
    }

    public partial class BuilderTypeIPropertySetterInterceptor : TypeSystemExBase
    {
    }

    public partial class BuilderTypeISyncRoot : TypeSystemExBase
    {
    }
}