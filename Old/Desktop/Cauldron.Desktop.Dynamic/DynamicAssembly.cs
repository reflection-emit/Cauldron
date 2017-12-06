using Cauldron.Core;
using Cauldron.Core.Extensions;
using Cauldron.Cryptography;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.SymbolStore;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using System.Threading;

namespace Cauldron.Dynamic
{
    /// <summary>
    /// Provides methods to define a dynamic assembly
    /// </summary>
    public static class DynamicAssembly
    {
        private static AssemblyBuilder assemblyBuilder;
        private static ConcurrentDictionary<string, Type> definedTypes = new ConcurrentDictionary<string, Type>();
        private static ModuleBuilder moduleBuilder;
        private static MethodAttributes publicGetterSetterAttrib;
        private static ISymbolDocumentWriter symbolDocumentWriter;

        static DynamicAssembly()
        {
            var name = Assembly.GetExecutingAssembly().GetName().Name + ".Dynamic";
            var assemblyName = new AssemblyName(name);
            var thisDomain = Thread.GetDomain();
            assemblyBuilder = thisDomain.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.RunAndSave);

            if (Assemblies.IsDebugging)
            {
                var debuggableAttribute = new CustomAttributeBuilder(
                    typeof(DebuggableAttribute).GetConstructor(new Type[] { typeof(DebuggableAttribute.DebuggingModes) }),
                    new object[] { DebuggableAttribute.DebuggingModes.DisableOptimizations | DebuggableAttribute.DebuggingModes.IgnoreSymbolStoreSequencePoints | DebuggableAttribute.DebuggingModes.Default });

                assemblyBuilder.SetCustomAttribute(debuggableAttribute);
                moduleBuilder = assemblyBuilder.DefineDynamicModule(assemblyBuilder.GetName().Name, "cauldron.dynamic.dll", true);
                symbolDocumentWriter = moduleBuilder.DefineDocument("cauldrondynamicsource.cs", Guid.Empty, Guid.Empty, Guid.Empty);
            }
            else
                moduleBuilder = assemblyBuilder.DefineDynamicModule(assemblyBuilder.GetName().Name, "cauldron.dynamic.dll", false);

            publicGetterSetterAttrib = MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig | MethodAttributes.Virtual;
        }

        /// <summary>
        /// Gets a collection of the types defined in this assembly.
        /// </summary>
        public static IEnumerable<Type> DefinedTypes { get { return definedTypes.Values; } }

        /// <summary>
        /// Constructs a <see cref="TypeBuilder"/> with the given name. The contructed <see cref="Type"/> is public sealed.
        /// </summary>
        /// <param name="name">The full path of the type. name cannot contain embedded nulls.</param>
        /// <returns>A <see cref="Builder"/> instance that helps to implement methods and properties</returns>
        public static Builder CreateBuilder(string name) =>
            CreateBuilder(name, null);

        /// <summary>
        /// Constructs a <see cref="TypeBuilder"/> with the given name. The contructed <see cref="Type"/> is public sealed.
        /// </summary>
        /// <param name="type">The type to copy the name from. If argument type is an interface, the new <see cref="Type"/> will be implementing the interface</param>
        /// <returns>A <see cref="Builder"/> instance that helps to implement methods and properties</returns>
        public static Builder CreateBuilder(Type type)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            var builder = CreateBuilder("Dynamic_" + type.FullName.Replace('.', '_') + "_Type", type.IsInterface ? null : type);
            builder.inheritsFrom.Add(type);

            if (type.IsInterface)
                builder.typeBuilder.AddInterfaceImplementation(type);

            return builder;
        }

        /// <summary>
        /// Constructs a <see cref="TypeBuilder"/> with the given name. The contructed <see cref="Type"/> is public sealed.
        /// </summary>
        /// <typeparam name="T">The type to copy the name from. If argument type is an interface, the new <see cref="Type"/> will be implementing the interface</typeparam>
        /// <returns>A <see cref="Builder{T}"/> instance that helps to implement methods and properties</returns>
        public static Builder<T> CreateBuilder<T>()
        {
            var builder = CreateBuilder(typeof(T));
            return new Builder<T>(builder.constructorIL, builder.typeBuilder, builder.inheritsFrom);
        }

        /// <summary>
        /// Constructs a <see cref="TypeBuilder"/> with the given name. The contructed <see cref="Type"/> is public sealed.
        /// </summary>
        /// <param name="name">The full path of the type. name cannot contain embedded nulls.</param>
        /// <param name="parentClass">A non-sealed type that is inherited from</param>
        /// <returns>A <see cref="Builder"/> instance that helps to implement methods and properties</returns>
        /// <exception cref="ArgumentException"><paramref name="parentClass"/> is a sealed type</exception>
        public static Builder CreateBuilder(string name, Type parentClass)
        {
            if (parentClass != null && parentClass.IsSealed)
                throw new ArgumentException($"Type '{parentClass.FullName}' is sealed.");

            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException(nameof(name));

            if (definedTypes.ContainsKey(name))
                name += (definedTypes.Count + 1).ToString();

            var typeBuilder = moduleBuilder.DefineType(name, TypeAttributes.Public | TypeAttributes.Class | TypeAttributes.Sealed, parentClass);
            var constructor = typeBuilder.DefineConstructor(MethodAttributes.Public, CallingConventions.Standard, Type.EmptyTypes);

            var ctorIL = constructor.GetILGenerator();

            if (parentClass != null && !parentClass.IsInterface)
            {
                // Check if the parent has a parameterless constructor
                var ctor = parentClass.GetConstructor(Type.EmptyTypes);

                if (ctor == null)
                    throw new NotSupportedException($"'{typeof(DynamicAssembly).FullName}' does not support inheriting from types without a parameterless constructor.");

                ctorIL.Emit(OpCodes.Ldarg_0);
                ctorIL.Emit(OpCodes.Call, ctor);
            }

            return new Builder(ctorIL, typeBuilder);
        }

        /// <summary>
        /// Creates an instance of the specified type. Creates a new type if neccessary
        /// </summary>
        /// <typeparam name="T">The type to copy the name from. If argument type is an interface, the new <see cref="Type"/> will be implementing the interface</typeparam>
        /// <returns>A reference to the newly created object.</returns>
        /// <exception cref="ArgumentException"><typeparamref name="T"/> is a sealed type</exception>
        public static T CreateInstance<T>() where T : class
        {
            var interfaceOrBaseClass = typeof(T);

            if (interfaceOrBaseClass.IsSealed && !interfaceOrBaseClass.IsInterface)
                throw new ArgumentException($"Type '{interfaceOrBaseClass.FullName}' is sealed.");

            var name = "Dynamic_" + interfaceOrBaseClass.FullName.Replace('.', '_') + "_Type";

            if (definedTypes.ContainsKey(name))
                return Activator.CreateInstance(definedTypes[name]) as T;

            using (var builder = DynamicAssembly.CreateBuilder(interfaceOrBaseClass))
            {
                var propertiesToDefine = interfaceOrBaseClass.GetPropertiesEx(BindingFlags.Public | BindingFlags.Instance);

                foreach (var property in propertiesToDefine)
                    builder.Implement(property);

                foreach (var method in interfaceOrBaseClass.GetMethodsEx(BindingFlags.Public | BindingFlags.Instance)
                    .Where(x => !x.IsSpecialName && ((!interfaceOrBaseClass.IsInterface && x.IsAbstract) || interfaceOrBaseClass.IsInterface)))
                    builder.Implement(method);

                return Activator.CreateInstance(builder.CreateType()) as T;
            }
        }

        /// <summary>
        /// Puts a call or callvirt instruction onto the Microsoft intermediate language (MSIL) stream to call a varargs method.
        /// </summary>
        /// <param name="il">The <see cref="ILGenerator"/> to use generate the code</param>
        /// <param name="opcode">The MSIL instruction to be emitted onto the stream. Must be <see cref="OpCodes.Call"/>, <see cref="OpCodes.Newobj"/> or <see cref="OpCodes.Callvirt"/></param>
        /// <param name="type">The type the where the method or property resides</param>
        /// <param name="methodOrPropertyName">The name of the method or property</param>
        /// <param name="parameterTypes">The type of parameters the method should have</param>
        /// <param name="boxResult">if true the result value of the method is boxed</param>
        /// <param name="parameters">Action performed before the call is pushed to the stream</param>
        /// <param name="bindingFlags">A bitmask comprised of one or more BindingFlags that specify how the search is conducted.</param>
        /// <exception cref="ArgumentException"><paramref name="opcode"/> does not specify a method call.</exception>
        public static void EmitCall(this ILGenerator il, OpCode opcode, Type type, string methodOrPropertyName, Type[] parameterTypes, bool boxResult, Action parameters, BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static)
        {
            parameters();
            il.EmitCall(opcode, type, methodOrPropertyName, parameterTypes, boxResult, bindingFlags);
        }

        /// <summary>
        /// Puts a call or callvirt instruction onto the Microsoft intermediate language (MSIL) stream to call a varargs method.
        /// </summary>
        /// <param name="il">The <see cref="ILGenerator"/> to use generate the code</param>
        /// <param name="opcode">The MSIL instruction to be emitted onto the stream. Must be <see cref="OpCodes.Call"/>, <see cref="OpCodes.Newobj"/> or <see cref="OpCodes.Callvirt"/></param>
        /// <param name="type">The type the where the method or property resides</param>
        /// <param name="methodOrPropertyName">The name of the method or property</param>
        /// <param name="parameterTypes">The type of parameters the method should have</param>
        /// <param name="boxResult">if true the result value of the method is boxed</param>
        /// <param name="bindingFlags">A bitmask comprised of one or more BindingFlags that specify how the search is conducted.</param>
        /// <exception cref="ArgumentException"><paramref name="opcode"/> does not specify a method call.</exception>
        public static void EmitCall(this ILGenerator il, OpCode opcode, Type type, string methodOrPropertyName, Type[] parameterTypes, bool boxResult, BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static)
        {
            var methodInfo = type.GetMethodEx(methodOrPropertyName, parameterTypes, bindingFlags);

            if (methodInfo == null)
            {
                var property = type.GetPropertyEx(methodOrPropertyName, bindingFlags);
                methodInfo = property == null ? null : property.GetMethod;
            }

            if (methodInfo == null)
                throw new InvalidOperationException($"A method or property '{methodOrPropertyName}' was not found in type '{type.FullName}");

            il.EmitCall(opcode, methodInfo, boxResult);
        }

        /// <summary>
        /// Puts a call or callvirt instruction onto the Microsoft intermediate language (MSIL) stream to call a varargs method.
        /// </summary>
        /// <param name="il">The <see cref="ILGenerator"/> to use generate the code</param>
        /// <param name="opcode">The MSIL instruction to be emitted onto the stream. Must be <see cref="OpCodes.Call"/>, <see cref="OpCodes.Newobj"/> or <see cref="OpCodes.Callvirt"/></param>
        /// <param name="methodbase">The varargs method to be called.</param>
        /// <param name="boxResult">if true the result value of the method is boxed</param>
        /// <param name="parameters">Action performed before the call is pushed to the stream</param>
        /// <exception cref="ArgumentException"><paramref name="opcode"/> does not specify a method call.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="methodbase"/> is null</exception>
        public static void EmitCall(this ILGenerator il, OpCode opcode, MethodBase methodbase, bool boxResult = false, Action parameters = null)
        {
            if (methodbase == null)
                throw new ArgumentNullException(nameof(methodbase));

            if (opcode == OpCodes.Call || opcode == OpCodes.Callvirt || opcode == OpCodes.Newobj)
            {
                if (parameters != null)
                    parameters();

                if (methodbase is MethodInfo)
                {
                    var methodInfo = methodbase as MethodInfo;
                    il.Emit(opcode, methodInfo);

                    if (boxResult && methodInfo.ReturnType != typeof(void))
                        il.Emit(OpCodes.Box, methodInfo.ReturnType);
                }
                else if (methodbase is PropertyPathInfo)
                {
                    var propertyInfo = methodbase as PropertyPathInfo;

                    if (propertyInfo.Methods.Count > 0)
                    {
                        il.Emit(OpCodes.Call, propertyInfo.Methods[0]);

                        for (int i = 1; i < propertyInfo.Methods.Count; i++)
                            il.Emit(OpCodes.Callvirt, propertyInfo.Methods[i]);
                    }
                    else
                        il.Emit(opcode, propertyInfo.GetMethod);

                    if (boxResult && propertyInfo.PropertyType != typeof(void))
                        il.Emit(OpCodes.Box, propertyInfo.PropertyType);
                }
                else if (methodbase is ConstructorInfo)
                    il.Emit(opcode, methodbase as ConstructorInfo);
            }
            else
                throw new ArgumentException("opcode does not specify a method call.");
        }

        /// <summary>
        /// Performs a for loop
        /// </summary>
        /// <param name="il">The <see cref="ILGenerator"/> to use generate the code</param>
        /// <param name="loops">The number of loops</param>
        /// <param name="action">
        /// The first parameter of the action is the indexer.
        /// The second parameter marks the end of the for loop
        /// </param>
        public static void EmitForEach(this ILGenerator il, int loops, Action<LocalBuilder, Label> action) =>
            il.EmitForEach(() => il.Emit(OpCodes.Ldc_I4, loops), action);

        /// <summary>
        /// Performs a for loop
        /// </summary>
        /// <param name="il">The <see cref="ILGenerator"/> to use generate the code</param>
        /// <param name="array">The array that is looped through</param>
        /// <param name="action">
        /// The first parameter of the action is the indexer.
        /// The second parameter marks the end of the for loop
        /// </param>
        public static void EmitForEach(this ILGenerator il, LocalBuilder array, Action<LocalBuilder, Label> action) =>
            il.EmitForEach(() =>
            {
                il.Emit(OpCodes.Ldloc, array);
                il.Emit(OpCodes.Ldlen);
            }, action);

        /// <summary>
        /// Performs a simple if-else-statement
        /// </summary>
        /// <param name="il">The <see cref="ILGenerator"/> to use generate the code</param>
        /// <param name="condition">
        /// The condition of the if statement. The first parameter of <see cref="Action{T1}"/> labels the if statement. The second parameter of <see cref="Action{T1}"/> labels the else statement
        /// </param>
        /// <param name="ifStatement">The if statement. The first parameter of <see cref="Action{T1, T2}"/> labels the end of the if-else-statement</param>
        /// <param name="elseStatement">The else statement. The first parameter of <see cref="Action{T1, T2}"/> labels the end of the if-else-statement</param>
        public static void EmitIfElse(this ILGenerator il, Action<Label, Label> condition, Action<Label> ifStatement, Action<Label> elseStatement)
        {
            var statementEndLabel = il.DefineLabel();
            var elseStatementLabel = il.DefineLabel();
            var ifStatementLabel = il.DefineLabel();

            condition(ifStatementLabel, elseStatementLabel);

            il.MarkLabel(ifStatementLabel);
            ifStatement(statementEndLabel);
            il.Emit(OpCodes.Br, statementEndLabel);
            il.MarkLabel(elseStatementLabel);
            elseStatement(statementEndLabel);
            il.MarkLabel(statementEndLabel);
        }

        /// <summary>
        /// Puts a methodBase.Result == null onto the Microsoft intermediate language (MSIL) stream
        /// </summary>
        /// <param name="il">The <see cref="ILGenerator"/> to use generate the code</param>
        /// <param name="condition">If true then the program will jump to <paramref name="jumpTo"/> if equal to null, otherwise it will jump on false</param>
        /// <param name="instance">The instance where <paramref name="methodBase"/> resides</param>
        /// <param name="methodBase">The method or property to call</param>
        /// <param name="jumpTo">A label to jump to if the condition is fulfilled</param>
        public static void EmitIfNull(this ILGenerator il, bool condition, LocalBuilder instance, MethodBase methodBase, Label jumpTo)
        {
            Action<MethodInfo> nullableValue = m =>
            {
                var nullable = il.DeclareLocal(m.ReturnType);

                il.Emit(OpCodes.Callvirt, m);
                il.Emit(OpCodes.Stloc, nullable);
                il.Emit(OpCodes.Ldloca_S, nullable);
                il.Emit(OpCodes.Call, m.ReturnType.GetProperty("HasValue").GetMethod);

                il.Emit(OpCodes.Ldc_I4_0);
                il.Emit(OpCodes.Ceq);

                if (condition)
                    il.Emit(OpCodes.Brtrue, jumpTo);
                else
                    il.Emit(OpCodes.Brfalse, jumpTo);
            };

            Action<MethodInfo> objectTypes = m =>
            {
                il.Emit(OpCodes.Callvirt, m);
                il.Emit(OpCodes.Ldnull);
                il.Emit(OpCodes.Ceq);

                if (condition)
                    il.Emit(OpCodes.Brtrue, jumpTo);
                else
                    il.Emit(OpCodes.Brfalse, jumpTo);
            };

            Func<MethodInfo, bool> emit = m =>
            {
                if (m.ReturnType.IsValueType && !m.ReturnType.IsNullable())
                    return false;

                if (m.ReturnType.IsNullable())
                    nullableValue(m);
                else
                    objectTypes(m);

                return true;
            };

            // Push the starting instance

            if (methodBase is MethodInfo)
            {
                il.Emit(OpCodes.Ldloc, instance);
                emit(methodBase as MethodInfo);
            }
            else if (methodBase is PropertyPathInfo)
            {
                var propertyInfo = methodBase as PropertyPathInfo;
                if (propertyInfo.Methods.Count > 0)
                {
                    // Dont proceed if the path has some value type in them
                    for (int starter = 0; starter < propertyInfo.Methods.Count; starter++)
                    {
                        il.Emit(OpCodes.Ldloc, instance);
                        if (starter == 0)
                            emit(propertyInfo.Methods[0]);
                        else
                            il.Emit(OpCodes.Call, propertyInfo.Methods[0]);

                        for (int i = 1; i < starter + 1; i++)
                        {
                            if (i == starter)
                                emit(propertyInfo.Methods[i]);
                            else
                                il.Emit(OpCodes.Callvirt, propertyInfo.Methods[i]);
                        }
                    }
                }
                else
                {
                    il.Emit(OpCodes.Ldloc, instance);
                    emit(propertyInfo);
                }
            }
        }

        /// <summary>
        /// Puts a <see cref="string.Empty"/> onto the Microsoft intermediate language (MSIL) stream
        /// </summary>
        /// <param name="il">The <see cref="ILGenerator"/> to use generate the code</param>
        public static void EmitStringEmpty(this ILGenerator il) => il.Emit(OpCodes.Ldsfld, typeof(string).GetField("Empty"));

        /// <summary>
        /// Marks a sequence point in the Microsoft intermediate language (MSIL) stream.
        /// </summary>
        /// <param name="il">The <see cref="ILGenerator"/> to use generate the code</param>
        /// <param name="startLine">The line where the sequence point begins.</param>
        /// <param name="startColumn">The column in the line where the sequence point begins. </param>
        /// <param name="endLine">The line where the sequence point ends. </param>
        /// <param name="endColumn">The column in the line where the sequence point ends. </param>
        public static void MarkSequencePoint(this ILGenerator il, int startLine, int startColumn, int endLine, int endColumn)
        {
            if (symbolDocumentWriter == null)
                return;

            il.MarkSequencePoint(symbolDocumentWriter, startLine, startColumn, endLine, endColumn);
        }

        /// <summary>
        /// Marks a sequence point in the Microsoft intermediate language (MSIL) stream.
        /// </summary>
        /// <param name="il">The <see cref="ILGenerator"/> to use generate the code</param>
        /// <param name="line">The line where the sequence point begins.</param>
        public static void MarkSequencePoint(this ILGenerator il, int line)
        {
            if (symbolDocumentWriter == null)
                return;

            il.MarkSequencePoint(symbolDocumentWriter, line, 1, line, 100);
        }

        /// <summary>
        /// Saves this dynamic assembly to disk.
        /// </summary>
        public static void Save() => assemblyBuilder.Save("cauldron.dynamic.dll");

        private static void EmitForEach(this ILGenerator il, Action conditionAction, Action<LocalBuilder, Label> action)
        {
            var condition = il.DefineLabel();
            var body = il.DefineLabel();
            var end = il.DefineLabel();

            // var i = indexerStart;
            var indexer = il.DeclareLocal(typeof(int));
            il.Emit(OpCodes.Ldc_I4_0);
            il.Emit(OpCodes.Stloc, indexer);
            il.Emit(OpCodes.Br, condition);

            // body
            il.MarkLabel(body);
            action(indexer, end);

            // increment
            il.Emit(OpCodes.Ldloc, indexer);
            il.Emit(OpCodes.Ldc_I4_1);
            il.Emit(OpCodes.Add);
            il.Emit(OpCodes.Stloc, indexer);

            // condition
            il.MarkLabel(condition);
            il.Emit(OpCodes.Ldloc, indexer);
            conditionAction();
            il.Emit(OpCodes.Conv_I4);
            il.Emit(OpCodes.Blt, body);
            il.Emit(OpCodes.Br, end);
            il.MarkLabel(end);
        }

        /// <summary>
        /// Provides methods to create properties and methods
        /// </summary>
        /// <typeparam name="T">The type of interface or base class</typeparam>
        public sealed class Builder<T> : Builder
        {
            internal Builder(ILGenerator constructorIL, TypeBuilder typeBuilder) : base(constructorIL, typeBuilder)
            {
            }

            internal Builder(ILGenerator constructorIL, TypeBuilder typeBuilder, IEnumerable<Type> inheritsFrom) : base(constructorIL, typeBuilder, inheritsFrom)
            {
            }

            /// <summary>
            /// Implements a method using an exception as its implementation
            /// </summary>
            /// <typeparam name="TReturn">The return <see cref="Type"/> of the method</typeparam>
            /// <param name="name">The name of the method</param>
            /// <param name="expression"></param>
            /// <exception cref="InvalidOperationException">Type is already created</exception>
            /// <exception cref="ArgumentException">The length of <paramref name="name"/> is zero.</exception>
            /// <exception cref="ArgumentNullException">name is null.</exception>
            public void Implement<TReturn>(string name, Expression<Func<T, TReturn>> expression) =>
                this.ImplementMethod(name, null, veryIL =>
                {
                    var mb = this.CreateExpressionStaticInnerMethod();
                    expression.CompileToMethod(mb);
                    return mb;
                }, typeof(TReturn));

            /// <summary>
            /// Implements a method using an <see cref="Expression"/> as its implementation
            /// </summary>
            /// <typeparam name="TReturn">The return <see cref="Type"/> of the method</typeparam>
            /// <param name="methodInfo">The method whose declaration is to be used.</param>
            /// <param name="expression"></param>
            /// <exception cref="InvalidOperationException">Type is already created</exception>
            public void Implement<TReturn>(MethodInfo methodInfo, Expression<Func<T, TReturn>> expression) =>
                ImplementMethod(methodInfo.Name, methodInfo, veryIL =>
                {
                    var mb = this.CreateExpressionStaticInnerMethod();
                    expression.CompileToMethod(mb);
                    return mb;
                }, methodInfo.ReturnType, methodInfo.GetParameters().Select(x => x.ParameterType).ToArray());

            /// <summary>
            /// Implements a method using an <see cref="Expression"/> as its implementation
            /// </summary>
            /// <typeparam name="T1">The first parameter of the method</typeparam>
            /// <typeparam name="TReturn">The return <see cref="Type"/> of the method</typeparam>
            /// <param name="name">The name of the method</param>
            /// <param name="expression"></param>
            /// <exception cref="InvalidOperationException">Type is already created</exception>
            /// <exception cref="ArgumentException">The length of <paramref name="name"/> is zero.</exception>
            /// <exception cref="ArgumentNullException">name is null.</exception>
            public void Implement<T1, TReturn>(string name, Expression<Func<T, T1, TReturn>> expression) =>
                this.ImplementMethod(name, null, veryIL =>
                {
                    var mb = this.CreateExpressionStaticInnerMethod();
                    expression.CompileToMethod(mb);
                    return mb;
                }, typeof(TReturn), new Type[] { typeof(T1) });

            /// <summary>
            /// Implements a method using an <see cref="Expression"/> as its implementation
            /// </summary>
            /// <typeparam name="T1">The first parameter of the method</typeparam>
            /// <typeparam name="T2">The second parameter of the method</typeparam>
            /// <typeparam name="TReturn">The return <see cref="Type"/> of the method</typeparam>
            /// <param name="name">The name of the method</param>
            /// <param name="expression"></param>
            /// <exception cref="InvalidOperationException">Type is already created</exception>
            /// <exception cref="ArgumentException">The length of <paramref name="name"/> is zero.</exception>
            /// <exception cref="ArgumentNullException">name is null.</exception>
            public void Implement<T1, T2, TReturn>(string name, Expression<Func<T, T1, T2, TReturn>> expression) =>
                this.ImplementMethod(name, null, veryIL =>
                {
                    var mb = this.CreateExpressionStaticInnerMethod();
                    expression.CompileToMethod(mb);
                    return mb;
                }, typeof(TReturn), new Type[] { typeof(T1), typeof(T2) });
        }

        /// <summary>
        /// Provides methods to create properties and methods
        /// </summary>
        public class Builder : DisposableBase
        {
            internal readonly ILGenerator constructorIL;
            internal readonly TypeBuilder typeBuilder;
            internal List<Type> inheritsFrom = new List<Type>();
            internal Type newType;

            internal Builder(ILGenerator constructorIL, TypeBuilder typeBuilder, IEnumerable<Type> inheritsFrom) : this(constructorIL, typeBuilder)
            {
                this.inheritsFrom.AddRange(inheritsFrom);
            }

            internal Builder(ILGenerator constructorIL, TypeBuilder typeBuilder)
            {
                this.constructorIL = constructorIL;
                this.typeBuilder = typeBuilder;
            }

            /// <summary>
            /// Gets the <see cref="ILGenerator"/> of the default constructor
            /// </summary>
            public ILGenerator DefaultConstructor { get { return constructorIL; } }

            /// <summary>
            /// Creates a <see cref="Type"/> object for the class. After defining fields and methods on the class, <see cref="Builder.CreateType"/> is called in order to load its <see cref="Type"/> object.
            /// </summary>
            /// <param name="builder">The type builder to use</param>
            public static implicit operator Type(Builder builder) =>
                builder.CreateType();

            /// <summary>
            /// Adds an interface that this type implements.
            /// </summary>
            /// <param name="interfaceType">The interface that this type implements.</param>
            public void AddInterfaceImplementation(Type interfaceType)
            {
                if (this.inheritsFrom.Contains(interfaceType))
                    return;

                this.typeBuilder.AddInterfaceImplementation(interfaceType);
                this.inheritsFrom.Add(interfaceType);
            }

            /// <summary>
            /// Creates a <see cref="Type"/> object for the class. After defining fields and methods on the class, <see cref="Builder.CreateType"/> is called in order to load its <see cref="Type"/> object.
            /// </summary>
            /// <returns>Returns the new <see cref="Type"/> object for this class.</returns>
            /// <exception cref="InvalidOperationException">
            /// he enclosing type has not been created.
            /// <para/>
            /// -or-
            /// <para/>
            /// This type is non-abstract and contains an abstract method.
            /// <para/>
            /// -or-
            /// <para/>
            /// This type is not an abstract class or an interface and has a method without a method body.
            /// </exception>
            /// <exception cref="NotSupportedException">
            /// The type contains invalid Microsoft intermediate language (MSIL) code.
            /// <para/>
            /// -or-
            /// <para/>
            /// The branch target is specified using a 1-byte offset, but the target is at a distance greater than 127 bytes from the branch.
            /// </exception>
            /// <exception cref="TypeLoadException">
            /// The type cannot be loaded. For example, it contains a static method that has the calling convention <see cref="CallingConventions.HasThis"/>.
            /// </exception>
            public Type CreateType()
            {
                if (this.newType != null)
                    return this.newType;

                // Finish the constructor
                this.constructorIL.Emit(OpCodes.Ret);
                // Create the type
                this.newType = this.typeBuilder.CreateType();

                DynamicAssembly.definedTypes.TryAdd(this.newType.Name, this.newType);

                return this.newType;
            }

            /// <summary>
            /// Adds a new private field to the type, with the given name and field type.
            /// </summary>
            /// <param name="fieldName">The name of the field. fieldName cannot contain embedded nulls.</param>
            /// <param name="fieldType">The type of the field</param>
            /// <returns>The defined field.</returns>
            /// <exception cref="ArgumentException">The length of <paramref name="fieldName"/> is zero.</exception>
            /// <exception cref="ArgumentNullException"><paramref name="fieldName"/> is null.</exception>
            /// <exception cref="InvalidOperationException">Type is already created</exception>
            public FieldBuilder DefineField(string fieldName, Type fieldType)
            {
                if (this.newType != null)
                    throw new InvalidOperationException($"Type '{this.newType.FullName}' was already created");

                if (fieldName == null)
                    throw new ArgumentNullException(nameof(fieldName));

                if (fieldName.Length == 0)
                    throw new ArgumentException(nameof(fieldName));

                return typeBuilder.DefineField($"field_{fieldName}", fieldType, FieldAttributes.Private);
            }

            /// <summary>
            /// Adds a new private field to the type, with the given name and field type.
            /// </summary>
            /// <typeparam name="TFieldType">The type of the field</typeparam>
            /// <param name="fieldName">The name of the field. fieldName cannot contain embedded nulls.</param>
            /// <returns>The defined field.</returns>
            /// <exception cref="ArgumentException">The length of <paramref name="fieldName"/> is zero.</exception>
            /// <exception cref="ArgumentNullException"><paramref name="fieldName"/> is null.</exception>
            /// <exception cref="InvalidOperationException">Type is already created</exception>
            public FieldBuilder DefineField<TFieldType>(string fieldName) => DefineField(fieldName, typeof(TFieldType));

            /// <summary>
            /// Implements a method using a <see cref="MethodInfo"/>
            /// </summary>
            /// <param name="methodInfo">The method whose declaration is to be used.</param>
            /// <exception cref="InvalidOperationException">Type is already created</exception>
            public void Implement(MethodInfo methodInfo) =>
                ImplementMethod(methodInfo.Name, methodInfo, null, methodInfo.ReturnType, methodInfo.GetParameters().Select(x => x.ParameterType).ToArray());

            /// <summary>
            /// Implements a property using a <see cref="PropertyInfo"/>
            /// </summary>
            /// <param name="propertyInfo">The property whose declaration is to be used.</param>
            /// <returns>The <see cref="FieldBuilder"/> of the backing field</returns>
            /// <exception cref="InvalidOperationException">Type is already created</exception>
            /// <exception cref="ArgumentNullException"><paramref name="propertyInfo"/> is null</exception>
            public FieldBuilder Implement(PropertyInfo propertyInfo)
            {
                if (propertyInfo == null)
                    throw new ArgumentNullException(nameof(propertyInfo));

                return this.ImplementProperty(propertyInfo.Name, propertyInfo.PropertyType, propertyInfo.Attributes);
            }

            /// <summary>
            /// Implements a method using an <see cref="Expression"/> as its implementation
            /// </summary>
            /// <typeparam name="TReturn">The return <see cref="Type"/> of the method</typeparam>
            /// <param name="name">The name of the method</param>
            /// <param name="expression">The expression to use as the implementation</param>
            /// <exception cref="InvalidOperationException">Type is already created</exception>
            /// <exception cref="ArgumentException">The length of <paramref name="name"/> is zero.</exception>
            /// <exception cref="ArgumentNullException">name is null.</exception>
            public void Implement<TReturn>(string name, Expression<Func<object, TReturn>> expression) =>
                this.ImplementMethod(name, null, veryIL =>
                {
                    var mb = this.CreateExpressionStaticInnerMethod();
                    expression.CompileToMethod(mb);
                    return mb;
                }, typeof(TReturn));

            /// <summary>
            /// Implements a method using an <see cref="Expression"/> as its implementation
            /// </summary>
            /// <typeparam name="T1">The first parameter of the method</typeparam>
            /// <typeparam name="TReturn">The return <see cref="Type"/> of the method</typeparam>
            /// <param name="name">The name of the method</param>
            /// <param name="expression">The expression to use as the implementation</param>
            /// <exception cref="InvalidOperationException">Type is already created</exception>
            /// <exception cref="ArgumentException">The length of <paramref name="name"/> is zero.</exception>
            /// <exception cref="ArgumentNullException">name is null.</exception>
            public void Implement<T1, TReturn>(string name, Expression<Func<object, T1, TReturn>> expression) =>
                this.ImplementMethod(name, null, veryIL =>
                {
                    var mb = this.CreateExpressionStaticInnerMethod();
                    expression.CompileToMethod(mb);
                    return mb;
                }, typeof(TReturn), new Type[] { typeof(T1) });

            /// <summary>
            /// Implements a method using an <see cref="Expression"/> as its implementation
            /// </summary>
            /// <typeparam name="T1">The first parameter of the method</typeparam>
            /// <typeparam name="T2">The second parameter of the method</typeparam>
            /// <typeparam name="TReturn">The return <see cref="Type"/> of the method</typeparam>
            /// <param name="name">The name of the method</param>
            /// <param name="expression">The expression to use as the implementation</param>
            /// <exception cref="InvalidOperationException">Type is already created</exception>
            /// <exception cref="ArgumentException">The length of <paramref name="name"/> is zero.</exception>
            /// <exception cref="ArgumentNullException">name is null.</exception>
            public void Implement<T1, T2, TReturn>(string name, Expression<Func<object, T1, T2, TReturn>> expression) =>
                this.ImplementMethod(name, null, veryIL =>
                {
                    var mb = this.CreateExpressionStaticInnerMethod();
                    expression.CompileToMethod(mb);
                    return mb;
                }, typeof(TReturn), new Type[] { typeof(T1), typeof(T2) });

            /// <summary>
            /// Implements a method using an <see cref="Expression"/> as its implementation
            /// </summary>
            /// <typeparam name="TReturn">The return <see cref="Type"/> of the method</typeparam>
            /// <param name="methodInfo">The method whose declaration is to be used.</param>
            /// <param name="expression">The expression to use as the implementation</param>
            /// <exception cref="InvalidOperationException">Type is already created</exception>
            public void Implement<TReturn>(MethodInfo methodInfo, Expression<Func<object, TReturn>> expression) =>
                ImplementMethod(methodInfo.Name, methodInfo, veryIL =>
                {
                    var mb = this.CreateExpressionStaticInnerMethod();
                    expression.CompileToMethod(mb);
                    return mb;
                }, methodInfo.ReturnType, methodInfo.GetParameters().Select(x => x.ParameterType).ToArray());

            /// <summary>
            /// Implements a method
            /// </summary>
            /// <param name="name">The name of the method</param>
            /// <param name="builder">An action that is invoked after the <see cref="MethodBuilder"/> has been created</param>
            /// <param name="returnType">The return type of the method</param>
            /// <param name="parameterTypes">The parameters of the method</param>
            /// <exception cref="ArgumentException">The length of <paramref name="name"/> is zero.</exception>
            /// <exception cref="ArgumentNullException">name is null.</exception>
            public void Implement(string name, Action<ILGenerator> builder, Type returnType, params Type[] parameterTypes) =>
                ImplementMethod(name, null, veryIL =>
                {
                    builder(veryIL);
                    return null;
                }, returnType, parameterTypes);

            /// <summary>
            /// Implements a method
            /// </summary>
            /// <param name="methodInfo">The method whose declaration is to be used.</param>
            /// <param name="builder">An action that is invoked after the <see cref="MethodBuilder"/> has been created</param>
            /// <param name="returnType">The return type of the method</param>
            /// <param name="parameterTypes">The parameters of the method</param>
            public void Implement(MethodInfo methodInfo, Action<ILGenerator> builder, Type returnType, params Type[] parameterTypes) =>
                ImplementMethod(methodInfo.Name, methodInfo, veryIL =>
                {
                    builder(veryIL);
                    return null;
                }, returnType, parameterTypes);

            internal MethodBuilder CreateExpressionStaticInnerMethod() =>
                            typeBuilder.DefineMethod($"Inner_{CryptoUtils.BrewPassword(CryptoUtils.AlphaNumericCharactersSet, 24)}", MethodAttributes.Private | MethodAttributes.Static);

            internal void ImplementMethod(string name, MethodInfo methodInfo, Func<ILGenerator, MethodBuilder> builder, Type returnType, params Type[] parameterTypes)
            {
                if (this.newType != null)
                    throw new InvalidOperationException($"Type '{this.newType.FullName}' was already created");

                if (name == null)
                    throw new ArgumentNullException(nameof(name));

                if (name.Length == 0)
                    throw new ArgumentException(nameof(name));

                MethodAttributes methodAttribute;

                if (this.inheritsFrom != null && methodInfo == null)
                    // Check if the base class or interface has already this method
                    methodInfo = this.inheritsFrom.Select(x => x.GetMethodEx(name, parameterTypes, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)).FirstOrDefault(x => x != null);

                if (methodInfo != null)
                {
                    methodAttribute = methodInfo.IsPublic ? MethodAttributes.Public : MethodAttributes.Private;

                    if (methodInfo.IsAbstract || methodInfo.IsVirtual)
                        methodAttribute |= MethodAttributes.Virtual;
                }
                else
                    methodAttribute = MethodAttributes.Public | MethodAttributes.HideBySig;

                var methodBuilder = typeBuilder.DefineMethod(name, methodAttribute, returnType, parameterTypes);

                if (builder == null) // Implement NotImplementedException
                {
                    var methodIL = methodBuilder.GetILGenerator();
                    methodIL.Emit(OpCodes.Ldstr, "Generated by Cauldron Toolkit");
                    methodIL.Emit(OpCodes.Newobj, typeof(NotImplementedException).GetConstructor(new Type[] { typeof(string) }));
                    methodIL.Emit(OpCodes.Throw);
                }
                else
                {
                    var methodIL = methodBuilder.GetILGenerator();
                    var innerMethodBuilder = builder(methodIL);

                    if (innerMethodBuilder != null)
                    {
                        methodIL.Emit(OpCodes.Ldarg_0);

                        for (int i = 1; i <= parameterTypes.Length; i++)
                            methodIL.Emit(OpCodes.Ldarg, i);

                        methodIL.Emit(OpCodes.Call, innerMethodBuilder);

                        methodIL.Emit(OpCodes.Ret);
                    }
                }

                if (methodInfo != null && (methodInfo.IsAbstract || methodInfo.IsVirtual))
                    typeBuilder.DefineMethodOverride(methodBuilder, methodInfo);
            }

            /// <summary>
            /// Occures after <see cref="IDisposable.Dispose"/> has been invoked
            /// </summary>
            /// <param name="disposeManaged">true if managed resources requires disposing</param>
            protected override void OnDispose(bool disposeManaged)
            {
                if (disposeManaged)
                    this.CreateType();
            }

            private FieldBuilder ImplementProperty(string propertyName, Type propertyType, PropertyAttributes attributes)
            {
                if (this.newType != null)
                    throw new InvalidOperationException($"Type '{this.newType.FullName}' was already created");

                var fieldBuilder = typeBuilder.DefineField($"_{propertyName}_BackingField", propertyType, FieldAttributes.Private);
                var propertyBuilder = typeBuilder.DefineProperty(propertyName, attributes, CallingConventions.HasThis, propertyType, null);

                // Create the getter method
                var getterMethodBuilder = typeBuilder.DefineMethod($"get_{propertyName}", publicGetterSetterAttrib, propertyType, Type.EmptyTypes);
                // Create the setter method
                var setterMethodBuilder = typeBuilder.DefineMethod($"set_{propertyName}", publicGetterSetterAttrib, null, new Type[] { propertyType });

                // Let's dive into IL code
                var getterIL = getterMethodBuilder.GetILGenerator();
                getterIL.Emit(OpCodes.Ldarg_0);
                getterIL.Emit(OpCodes.Ldfld, fieldBuilder);
                getterIL.Emit(OpCodes.Ret);

                var setterIL = setterMethodBuilder.GetILGenerator();
                setterIL.Emit(OpCodes.Ldarg_0);
                setterIL.Emit(OpCodes.Ldarg_1);
                setterIL.Emit(OpCodes.Stfld, fieldBuilder);
                setterIL.Emit(OpCodes.Ret);

                // Map the getter and setter method to the property
                propertyBuilder.SetGetMethod(getterMethodBuilder);
                propertyBuilder.SetSetMethod(setterMethodBuilder);

                return fieldBuilder;
            }
        }
    }
}