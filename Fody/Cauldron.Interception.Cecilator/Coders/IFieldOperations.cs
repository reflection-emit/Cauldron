using System;

namespace Cauldron.Interception.Cecilator.Coders
{
    public interface IFieldOperations<T>
    {
        T Load(Field field);

        T Load(Func<BuilderType, Field> field);
    }

    public interface IFieldOperationsExtended<T> : IFieldOperations<T>
    {
        Coder SetValue(Field field, object value);

        Coder SetValue(Field field, Func<Coder, object> value);

        Coder SetValue(Func<BuilderType, Field> field, object value);

        Coder SetValue(Func<BuilderType, Field> field, Func<Coder, object> value);
    }
}