using FluentAssertions;
using Slender.Dependencies.Internals;
using Slender.Dependencies.Tests.Support;
using System;
using System.Collections.Generic;
using Xunit;

namespace Slender.Dependencies.Tests.Unit.Internals
{

    public class TypeExtensionTests
    {

        #region - - - - - - GetTypeDefinition Tests - - - - - -

        [Theory]
        [MemberData(nameof(GetTypeDefinition_VariousTypes_ReturnsExpectedResult_GetTestData))]
        public void GetTypeDefinition_VariousTypes_ReturnsExpectedResult(Type type, Type expected)
            => type.GetTypeDefinition().Should().Be(expected);

        public static IEnumerable<object[]> GetTypeDefinition_VariousTypes_ReturnsExpectedResult_GetTestData()
            => new List<object[]>
            {
                new object[] { typeof(IService), typeof(IService) },
                new object[] { typeof(IGenericService<>), typeof(IGenericService<>) },
                new object[] { typeof(IGenericService<object>), typeof(IGenericService<>) },
                new object[] { typeof(ClosedGenericServiceImplementation), typeof(ClosedGenericServiceImplementation) },
                new object[] { typeof(OpenGenericServiceImplementation<>), typeof(OpenGenericServiceImplementation<>) },
                new object[] { typeof(OpenGenericServiceImplementation<object>), typeof(OpenGenericServiceImplementation<>) },
                new object[] { typeof(Enum), typeof(Enum) },
                new object[] { typeof(int), typeof(int) }
            };

        #endregion GetTypeDefinition Tests

    }

}
