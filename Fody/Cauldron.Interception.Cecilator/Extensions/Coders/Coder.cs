using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.Rocks;
using System;
using System.Collections.Generic;
using System.IO;

namespace Cauldron.Interception.Cecilator.Extensions
{
    public sealed class Coder
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
            if (castToType == null)
                return new InstructionsCodeBlock(this);

            var lastInstruction = this.instructions.LastOrDefault();
            if (lastInstruction.IsCallOrNew())
            {
                var lastType = (lastInstruction.Operand as MethodReference)?.ReturnType;

                if (lastType != null && lastType.FullName == castToType.Fullname)
                    return new InstructionsCodeBlock(this);

                if (lastType != null && lastType.IsPrimitive)
                {
                    var paramResult = new ParamResult
                    {
                        Type = lastType
                    };

                    this.processor.CastOrBoxValues(castToType.typeReference, paramResult, castToType.typeDefinition);
                    this.instructions.Append(paramResult.Instructions);
                    return new InstructionsCodeBlock(this);
                }
            }

            // Add parameter, field and variable if required later on
            this.instructions.Append(this.processor.Create(OpCodes.Isinst, Builder.Current.Import(castToType.typeReference)));
            return new InstructionsCodeBlock(this);
        }
    }
}