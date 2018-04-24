using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cauldron.Interception.Cecilator
{
    public partial class BuilderType
    {
        private static BuilderType _boolean;
        private static BuilderType _byte;
        private static BuilderType _char;
        private static BuilderType _convert;
        private static BuilderType _dateTime;
        private static BuilderType _decimal;
        private static BuilderType _double;
        private static BuilderType _enum;
        private static BuilderType _enumerable;
        private static BuilderType _iEnumerable;
        private static BuilderType _iEnumerableGeneric;
        private static BuilderType _int16;
        private static BuilderType _int32;
        private static BuilderType _int64;
        private static BuilderType _intPtr;
        private static BuilderType _methodBase;
        private static BuilderType _nullable;
        private static BuilderType _object;
        private static BuilderType _sByte;
        private static BuilderType _single;
        private static BuilderType _string;
        private static BuilderType _task;
        private static BuilderType _type;
        private static BuilderType _uInt16;
        private static BuilderType _uInt32;
        private static BuilderType _uInt64;
        private static BuilderType _uIntPtr;
        private static BuilderType _void;

        public static BuilderType Boolean
        {
            get
            {
                if (_boolean == null) _boolean = Builder.Current.GetType(typeof(bool)).Import();
                return _boolean;
            }
        }

        public static BuilderType Byte
        {
            get
            {
                if (_byte == null) _byte = Builder.Current.GetType(typeof(byte)).Import();
                return _byte;
            }
        }

        public static BuilderType Char
        {
            get
            {
                if (_char == null) _char = Builder.Current.GetType(typeof(char)).Import();
                return _char;
            }
        }

        public static BuilderType Convert
        {
            get
            {
                if (_convert == null) _convert = Builder.Current.GetType(typeof(Convert)).Import();
                return _convert;
            }
        }

        public static BuilderType DateTime
        {
            get
            {
                if (_dateTime == null) _dateTime = Builder.Current.GetType(typeof(DateTime)).Import();
                return _dateTime;
            }
        }

        public static BuilderType Decimal
        {
            get
            {
                if (_decimal == null) _decimal = Builder.Current.GetType(typeof(decimal)).Import();
                return _decimal;
            }
        }

        public static BuilderType Double
        {
            get
            {
                if (_double == null) _double = Builder.Current.GetType(typeof(double)).Import();
                return _double;
            }
        }

        public static BuilderType Enum
        {
            get
            {
                if (_enum == null) _enum = Builder.Current.GetType(typeof(Enum)).Import();
                return _enum;
            }
        }

        public static BuilderType Enumerable
        {
            get
            {
                if (_enumerable == null) _enumerable = Builder.Current.GetType(typeof(Enumerable)).Import();
                return _enumerable;
            }
        }

        public static BuilderType IEnumerable
        {
            get
            {
                if (_iEnumerable == null) _iEnumerable = Builder.Current.GetType(typeof(IEnumerable)).Import();
                return _iEnumerable;
            }
        }

        /// <summary>
        /// Gets a <see cref="BuilderType"/> that represents <see cref="IEnumerable{T}"/>
        /// </summary>
        public static BuilderType IEnumerable1
        {
            get
            {
                if (_iEnumerableGeneric == null) _iEnumerableGeneric = Builder.Current.GetType(typeof(IEnumerable<>)).Import();
                return _iEnumerableGeneric;
            }
        }

        public static BuilderType Int16
        {
            get
            {
                if (_int16 == null) _int16 = Builder.Current.GetType(typeof(short)).Import();
                return _int16;
            }
        }

        public static BuilderType Int32
        {
            get
            {
                if (_int32 == null) _int32 = Builder.Current.GetType(typeof(int)).Import();
                return _int32;
            }
        }

        public static BuilderType Int64
        {
            get
            {
                if (_int64 == null) _int64 = Builder.Current.GetType(typeof(long)).Import();
                return _int64;
            }
        }

        public static BuilderType IntPtr
        {
            get
            {
                if (_intPtr == null) _intPtr = Builder.Current.GetType(typeof(IntPtr)).Import();
                return _intPtr;
            }
        }

        public static BuilderType MethodBase
        {
            get
            {
                if (_methodBase == null) _methodBase = Builder.Current.GetType(typeof(System.Reflection.MethodBase)).Import();
                return _methodBase;
            }
        }

        public static BuilderType Nullable
        {
            get
            {
                if (_nullable == null) _nullable = Builder.Current.GetType(typeof(Nullable<>)).Import();
                return _nullable;
            }
        }

        public static BuilderType Object
        {
            get
            {
                if (_object == null) _object = Builder.Current.GetType(typeof(object)).Import();
                return _object;
            }
        }

        public static BuilderType SByte
        {
            get
            {
                if (_sByte == null) _sByte = Builder.Current.GetType(typeof(sbyte)).Import();
                return _sByte;
            }
        }

        public static BuilderType Single
        {
            get
            {
                if (_single == null) _single = Builder.Current.GetType(typeof(float)).Import();
                return _single;
            }
        }

        public static BuilderType String
        {
            get
            {
                if (_string == null) _string = Builder.Current.GetType(typeof(string)).Import();
                return _string;
            }
        }

        public static BuilderType Task
        {
            get
            {
                if (_task == null) _task = Builder.Current.GetType(typeof(Task)).Import();
                return _task;
            }
        }

        public static BuilderType Type
        {
            get
            {
                if (_type == null) _type = Builder.Current.GetType(typeof(Type)).Import();
                return _type;
            }
        }

        public static BuilderType UInt16
        {
            get
            {
                if (_uInt16 == null) _uInt16 = Builder.Current.GetType(typeof(ushort)).Import();
                return _uInt16;
            }
        }

        public static BuilderType UInt32
        {
            get
            {
                if (_uInt32 == null) _uInt32 = Builder.Current.GetType(typeof(uint)).Import();
                return _uInt32;
            }
        }

        public static BuilderType UInt64
        {
            get
            {
                if (_uInt64 == null) _uInt64 = Builder.Current.GetType(typeof(ulong)).Import();
                return _uInt64;
            }
        }

        public static BuilderType UIntPtr
        {
            get
            {
                if (_uIntPtr == null) _uIntPtr = Builder.Current.GetType(typeof(UIntPtr)).Import();
                return _uIntPtr;
            }
        }

        public static BuilderType Void
        {
            get
            {
                if (_void == null) _void = Builder.Current.GetType("System.Void");
                return _void;
            }
        }
    }
}