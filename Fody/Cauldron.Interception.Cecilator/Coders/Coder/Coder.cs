using Cauldron.Interception.Cecilator.Extensions;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.Rocks;
using System;
using System.Collections.Generic;
using System.IO;

namespace Cauldron.Interception.Cecilator.Coders
{
    public sealed partial class Coder
    {
        public const string ReturnVariableName = "<>returnValue";
        public const string VariablePrefix = "<>var_";
        internal readonly InstructionContainer instructions = new InstructionContainer();
        internal readonly Method method;
        internal readonly ILProcessor processor;

        internal Coder(Method method)
        {
            this.method = method;
            this.processor = method.GetILProcessor();
            this.method.methodDefinition.Body.SimplifyMacros();
        }

        /// <summary>
        /// Generates a random name that can be used to name variables and methods.
        /// </summary>
        /// <returns></returns>
        public static string GenerateName() => Path.GetRandomFileName().Replace(".", DateTime.Now.Second.ToString());

        public Coder Append(IEnumerable<Instruction> instructions)
        {
            this.instructions.Append(instructions);
            return this;
        }

        public void InstructionDebug() => this.method.Log(LogTypes.Info, this.instructions);

        public CodeBlock ToCodeBlock() => new InstructionsCodeBlock(this);

        public override string ToString() => this.method.Fullname;

        internal CodeBlock ToCodeBlock(BuilderType castToType)
        {
            if (this.instructions.Count == 0)
                throw new Exception("Cannot convert to CodeBlock, since this Coder has no instructions in its list.");

            if (castToType == null)
                return new InstructionsCodeBlock(this);

            InstructionsCodeBlock GetCodeBlockFromLastType(TypeReference typeReference)
            {
                if (typeReference == null)
                    return null;

                if (typeReference.FullName == castToType.Fullname)
                    return new InstructionsCodeBlock(this);

                if (castToType.typeReference.IsPrimitive)
                {
                    this.instructions.Append(this.processor.Create(OpCodes.Unbox_Any, Builder.Current.Import(castToType.typeReference)));
                    return new InstructionsCodeBlock(this);
                }

                if (castToType.typeReference.Resolve().With(x => x.IsInterface || x.IsClass))
                {
                    this.instructions.Append(this.processor.Create(OpCodes.Isinst, Builder.Current.Import(castToType.typeReference)));
                    return new InstructionsCodeBlock(this);
                }

                var paramResult = new ParamResult
                {
                    Type = typeReference
                };

                this.processor.CastOrBoxValues(castToType.typeReference, paramResult, castToType.typeDefinition);
                this.instructions.Append(paramResult.Instructions);
                return new InstructionsCodeBlock(this);
            }

            var result = GetCodeBlockFromLastType(this.instructions.GetTypeOfValueInStack(this.method));

            if (result != null)
                return result;

            // This can cause exceptions in some cases
            this.instructions.Append(this.processor.Create(OpCodes.Isinst, Builder.Current.Import(castToType.typeReference)));
            return new InstructionsCodeBlock(this);
        }
    }
}