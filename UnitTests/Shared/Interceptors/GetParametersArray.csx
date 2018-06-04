using System;
using System.Text;
using Cauldron.Interception.Cecilator;
using Cauldron.Interception.Fody;
using Cauldron.Interception.Fody.HelperTypes;
using System.Linq;
using Cauldron.Interception.Cecilator.Coders;
using System.Diagnostics;

public static class GetParametersArray
{
    public static void Implement(Builder builder)
    {
        var attribute = builder.GetType("CecilatorTestClasses.GetParametersArrayTestsAttribute");
        var writeLineMethod = attribute.GetMethod("Assert", 1, true);
        var attributedMethods = builder.FindMethodsByAttribute(attribute);

        foreach (var attributedMethod in attributedMethods)
        {
            attributedMethod.Method.NewCoder()
                .Context(x => x.Call(writeLineMethod, x.GetParametersArray()).End)
                .Insert(InsertionPosition.Beginning);

            // We dont need the attribute on the method anymore... So lets get rid of it.
            attributedMethod.Attribute.Remove();
        }
    }
}