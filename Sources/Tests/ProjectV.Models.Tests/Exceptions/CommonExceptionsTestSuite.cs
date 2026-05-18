using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using AwesomeAssertions;
using ProjectV.Models.Exceptions;
using Xunit;

namespace ProjectV.Models.Tests.Exceptions
{
    /// <summary>
    /// Reflection-driven enforcement of the 3-constructor convention across
    /// every sealed <see cref="Exception" /> subclass in the
    /// <c>ProjectV.Models.Exceptions</c> namespace. New custom exception
    /// types are picked up automatically when they land in
    /// <c>Sources/Libraries/ProjectV.Models/Exceptions/</c> — no manual
    /// <c>[InlineData]</c> wiring is needed.
    /// </summary>
    /// <remarks>
    /// The convention every custom exception in ProjectV follows (see
    /// CONVENTIONS.md):
    /// <list type="bullet">
    ///     <item><description>Public parameterless ctor.</description></item>
    ///     <item><description>Public ctor (<see cref="string" /> message).</description></item>
    ///     <item><description>Public ctor (<see cref="string" /> message,
    ///         <see cref="Exception" /> innerException).</description></item>
    /// </list>
    /// Asserting via reflection complements the per-type
    /// <see cref="BaseExceptionTests{TException}" /> suites (which verify the
    /// ctors actually behave correctly): this suite confirms NO custom
    /// exception silently drops one of the three.
    /// </remarks>
    [Trait("Category", "Unit")]
    public sealed class CommonExceptionsTestSuite
    {
        public CommonExceptionsTestSuite()
        {
        }

        /// <summary>
        /// Enumerates every sealed <see cref="Exception" /> subtype declared
        /// in the <see cref="CannotGetTmdbConfigException" /> assembly.
        /// Reflection over <see cref="Assembly.GetExportedTypes" /> means new
        /// exception types added to <c>ProjectV.Models.Exceptions</c> are
        /// auto-discovered.
        /// </summary>
        public static IEnumerable<object[]> SealedExceptionTypes()
        {
            var assembly = typeof(CannotGetTmdbConfigException).Assembly;
            foreach (Type type in assembly.GetExportedTypes())
            {
                if (type.IsSealed
                    && typeof(Exception).IsAssignableFrom(type)
                    && !type.IsAbstract)
                {
                    yield return new object[] { type };
                }
            }
        }

        [Theory]
        [MemberData(nameof(SealedExceptionTypes))]
        public void ExceptionTypeHasDefaultConstructor(Type exceptionType)
        {
            // Arrange.
            exceptionType.Should().NotBeNull();

            // Act.
            ConstructorInfo? ctor = exceptionType.GetConstructor(
                Type.EmptyTypes);

            // Assert.
            ctor.Should().NotBeNull(
                $"{exceptionType.FullName} must declare a public parameterless constructor."
            );
        }

        [Theory]
        [MemberData(nameof(SealedExceptionTypes))]
        public void ExceptionTypeHasMessageConstructor(Type exceptionType)
        {
            // Arrange.
            exceptionType.Should().NotBeNull();

            // Act.
            ConstructorInfo? ctor = exceptionType.GetConstructor(
                new[] { typeof(string) });

            // Assert.
            ctor.Should().NotBeNull(
                $"{exceptionType.FullName} must declare a public (string message) constructor."
            );
        }

        [Theory]
        [MemberData(nameof(SealedExceptionTypes))]
        public void ExceptionTypeHasMessageAndInnerExceptionConstructor(Type exceptionType)
        {
            // Arrange.
            exceptionType.Should().NotBeNull();

            // Act.
            ConstructorInfo? ctor = exceptionType.GetConstructor(
                new[] { typeof(string), typeof(Exception) });

            // Assert.
            ctor.Should().NotBeNull(
                $"{exceptionType.FullName} must declare a public " +
                $"(string message, Exception innerException) constructor."
            );
        }

        [Fact]
        public void DiscoversAtLeastOneSealedExceptionType()
        {
            // Arrange. / Act.
            var types = SealedExceptionTypes().ToList();

            // Assert.
            types.Should().NotBeEmpty(
                "ProjectV.Models must declare at least one sealed exception type " +
                "(CannotGetTmdbConfigException at minimum)."
            );
        }
    }
}
