using Cauldron.Interception.Cecilator;
using Cauldron.Interception.Cecilator.Coders;
using Cauldron.Interception.Fody;
using Cauldron.Interception.Fody.HelperTypes;
using System;
using System.Collections.Generic;
using System.Linq;

public interface IMethodBuilderInfoItem
{
    AttributedMethod Attribute { get; }

    bool HasSyncRootInterface { get; }

    bool IsSuppressed { get; }
}

public sealed class MethodBuilderInfo<T> where T : IMethodBuilderInfoItem
{
    private Field _syncRoot;

    public MethodBuilderInfo(MethodKey key, IEnumerable<T> items)
    {
        this.Key = key;
        this.Items = items.Where(x => !x.IsSuppressed).ToArray();
    }

    public T[] Items { get; private set; }

    public MethodKey Key { get; private set; }

    public bool RequiresSyncRootField => this.Items?.Any(x => x.HasSyncRootInterface) ?? false;

    public Field SyncRoot
    {
        get
        {
            if (_syncRoot == null)
            {
                var name = $"<{this.Key.Method.Name}>_syncObject_{this.Key.Method.Identification}";
                _syncRoot = this.Key.Method.DeclaringType.GetField(name, false);

                if (_syncRoot == null)
                {
                    _syncRoot = this.Key.Method.DeclaringType.CreateField(this.Key.Method.Modifiers.GetPrivate(), typeof(object), name);
                    _syncRoot.CustomAttributes.AddNonSerializedAttribute();
                }
            }

            return _syncRoot;
        }
    }
}

public sealed class MethodBuilderInfoItem<T1, T2> : IMethodBuilderInfoItem
    where T1 : TypeSystemExBase
    where T2 : TypeSystemExBase
{
    public MethodBuilderInfoItem(AttributedMethod attribute, T1 interfaceA, T2 interfaceB)
    {
        this.Attribute = attribute;
        this.InterfaceA = interfaceA;
        this.InterfaceB = interfaceB;
        this.AssignMethodAttributeInfos = AssignMethodAttributeInfo.GetAllAssignMethodAttributedFields(attribute);
        this.InterceptorInfo = new InterceptorInfo(this.Attribute.Attribute.Type);
        this.HasSyncRootInterface = attribute.Attribute.Type.Implements(BuilderTypes.ISyncRoot);
        this.HasInterfaceA = this.Attribute.Attribute.Type.Implements(this.InterfaceA.BuilderType);
        this.HasInterfaceB = this.Attribute.Attribute.Type.Implements(this.InterfaceB.BuilderType);

        var name = $"<{attribute.Method.Name}>_{attribute.Identification}";
        var newInterceptor = this.InterceptorInfo.AlwaysCreateNewInstance ?
            attribute.Method.GetOrCreateVariable(this.InterfaceType) as CecilatorBase :
            attribute.Method.DeclaringType.CreateField(attribute.Method.Modifiers.GetPrivate(), this.InterfaceType, name);

        this.Interceptor = newInterceptor;
        this.FieldOrVariable = attribute.Method.IsAsync ? attribute.Method.AsyncMethodHelper.InsertFieldToAsyncStateMachine(name, this.InterfaceType, z => newInterceptor) : newInterceptor;
        (newInterceptor as Field)?.CustomAttributes.AddNonSerializedAttribute();
    }

    public AssignMethodAttributeInfo[] AssignMethodAttributeInfos { get; }

    public AttributedMethod Attribute { get; }

    public CecilatorBase FieldOrVariable { get; }
    public bool HasInterfaceA { get; }
    public bool HasInterfaceB { get; }
    public bool HasSyncRootInterface { get; }
    public CecilatorBase Interceptor { get; }
    public InterceptorInfo InterceptorInfo { get; }

    public T1 InterfaceA { get; }
    public T2 InterfaceB { get; }
    public BuilderType InterfaceType => this.HasInterfaceB ? this.InterfaceB.BuilderType : this.InterfaceA.BuilderType;

    public bool IsSuppressed => InterceptorInfo.GetIsSupressed(this.InterceptorInfo, this.Attribute.Method.DeclaringType, this.Attribute.Method.CustomAttributes, this.Attribute.Attribute, this.Attribute.Method.Name, true);
}

public sealed class MethodBuilderInfoItem<T> : IMethodBuilderInfoItem
{
    public MethodBuilderInfoItem(AttributedMethod attribute, T @interface)
    {
        this.Attribute = attribute;
        this.Interface = @interface;
        this.AssignMethodAttributeInfos = AssignMethodAttributeInfo.GetAllAssignMethodAttributedFields(attribute);
        this.InterceptorInfo = new InterceptorInfo(this.Attribute.Attribute.Type);
        this.HasSyncRootInterface = attribute.Attribute.Type.Implements(BuilderTypes.ISyncRoot);
    }

    public AssignMethodAttributeInfo[] AssignMethodAttributeInfos { get; private set; }

    public AttributedMethod Attribute { get; private set; }

    public bool HasSyncRootInterface { get; private set; }

    public InterceptorInfo InterceptorInfo { get; private set; }

    public T Interface { get; private set; }

    public bool IsSuppressed => InterceptorInfo.GetIsSupressed(this.InterceptorInfo, this.Attribute.Method.DeclaringType, this.Attribute.Method.CustomAttributes, this.Attribute.Attribute, this.Attribute.Method.Name, true);
}

public sealed class MethodKey
{
    public MethodKey(Method method, Method asyncMethod)
    {
        this.Method = method;
        this.AsyncMethod = asyncMethod;
    }

    public Method AsyncMethod { get; private set; }

    public Method Method { get; private set; }

    public override bool Equals(object obj) => (obj as MethodKey)?.Method.Equals(this.Method) ?? false;

    public override int GetHashCode() => this.Method.GetHashCode();
}