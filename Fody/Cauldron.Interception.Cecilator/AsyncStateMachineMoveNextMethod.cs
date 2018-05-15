using Mono.Cecil;
using Mono.Cecil.Cil;
using System;
using System.Collections.Generic;

namespace Cauldron.Interception.Cecilator
{
    public sealed class AsyncStateMachineMoveNextMethod : Method
    {
        internal AsyncStateMachineMoveNextMethod(
            BuilderType builderType,
            MethodDefinition moveNextMethodDefinition,
            Method originalMethod) : base(builderType, moveNextMethodDefinition)
        {
            this.OriginMethod = originalMethod;
        }

        public override BuilderType AsyncOriginType => this.AsyncMethodHelper.OriginType;

        /// <summary>
        /// Get the async method refering to this async state machine
        /// </summary>
        public Method OriginMethod { get; }

        internal Instruction BeginOfCode { get; set; }
        internal List<Tuple<Instruction, int>> OutOfBoundJumpIndex { get; } = new List<Tuple<Instruction, int>>();
    }
}