﻿using System.Linq;
using System.Reflection;

namespace AjaxControlToolkit.BackwardCompatibilityTests
{
    public static class CompatibilityAssert
    {
        public static void Assert(this CompatibilityTestData testData)
        {
            var targetClassName = testData.ClassName;
            var newClassMembers = testData.ClassMembersFound;
            var memberInOldClass = testData.MemberToTest;

            var memberInNewClass = newClassMembers.FirstOrDefault(m => FindMatch(m, memberInOldClass));

            if (memberInOldClass.MemberType == MemberTypes.Constructor ||
                memberInOldClass.MemberType == MemberTypes.Method)
            {
                var oldClassCi = (memberInOldClass as MethodBase);
                var oldParams = oldClassCi.GetParameters();

                if (memberInNewClass == null)
                {
                    var prms = string.Join(",", oldParams.Select(p => p.ParameterType.Name).ToArray());
                    NUnit.Framework.Assert.Fail("{0} {1}({2}) is not found",
                                               memberInOldClass.MemberType,
                                               memberInOldClass.MemberType == MemberTypes.Constructor ?
                                               targetClassName : targetClassName +"."+memberInOldClass.Name, prms);
                }
            }
            else
            {
                // Assert member is exists
                NUnit.Framework.Assert.NotNull(memberInNewClass,
                                               "{0} {1} is not found on {2}", 
                                               memberInOldClass.MemberType, 
                                               memberInOldClass.Name, targetClassName);
            }

            // Assert member type is equal
            NUnit.Framework.Assert.AreEqual(memberInOldClass.MemberType, memberInNewClass.MemberType, 
                "Member type of {0} on {1} is not match.", memberInOldClass.Name, targetClassName);

            // Assert underlying member type is equal
            NUnit.Framework.Assert.AreEqual(CompatibilityTestCase.GetMemberUnderlyingTypeName(memberInOldClass), 
                CompatibilityTestCase.GetMemberUnderlyingTypeName(memberInNewClass), 
                "Underlying member type of {0} on {1} is not match.", memberInOldClass.Name, targetClassName);
        }

        static bool FindMatch(MemberInfo memberInNewClass, MemberInfo memberInOldClass)
        {
            if (memberInNewClass.Name != memberInOldClass.Name)
                return false;

            if (memberInOldClass.MemberType == MemberTypes.Constructor || memberInOldClass.MemberType == MemberTypes.Method)
            {
                var oldClassCi = (memberInOldClass as MethodBase);
                var oldParams = oldClassCi.GetParameters();

                var newClassCi = (memberInNewClass as MethodBase);
                var newParams = newClassCi.GetParameters();

                if (newParams.Length != oldParams.Length)
                    return false;

                for (var i = 0; i < oldParams.Length; i++)
                {
                    if (oldParams[i].ParameterType.Name !=
                        newParams[i].ParameterType.Name)
                        return false;
                }
            }

            return true;
        }

        
    }
}
