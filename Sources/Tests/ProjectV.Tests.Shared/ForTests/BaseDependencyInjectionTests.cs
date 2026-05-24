using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ProjectV.Tests.Shared.ForTests
{
    /// <summary>
    /// Base class for tests that exercise dependency-injection container
    /// registration (e.g. <c>AddProjectVCore()</c>-style extension methods).
    /// Provides factory helpers for an empty service collection and a host
    /// application builder, plus AwesomeAssertions-based assertions for
    /// service presence and implementation type.
    /// </summary>
    public abstract class BaseDependencyInjectionTests : BaseMockTest
    {
        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="BaseDependencyInjectionTests" /> class.
        /// </summary>
        protected BaseDependencyInjectionTests()
        {
        }

        /// <summary>
        /// Creates an empty <see cref="IServiceCollection" /> for testing
        /// container registration extensions.
        /// </summary>
        protected static IServiceCollection CreateServiceCollection()
        {
            return new ServiceCollection();
        }

        /// <summary>
        /// Creates an <see cref="IHostApplicationBuilder" /> via
        /// <see cref="Host.CreateApplicationBuilder()" /> for tests that
        /// register options or hosted services.
        /// </summary>
        protected static IHostApplicationBuilder CreateHostAppBuilder()
        {
            return Host.CreateApplicationBuilder();
        }

        /// <summary>
        /// Asserts that the specified service type is NOT registered in the
        /// supplied <paramref name="services" /> collection.
        /// </summary>
        /// <typeparam name="T">Service type to check.</typeparam>
        /// <param name="services">Service collection under test.</param>
        protected static void AssertOn_NotRegistered<T>(IServiceCollection services)
        {
            var descriptor = services.FirstOrDefault(d => d.ServiceType == typeof(T));
            descriptor.Should().BeNull(
                $"service {typeof(T).Name} should NOT be registered.");
        }

        /// <summary>
        /// Asserts that the specified service type is registered (at least
        /// once) in the supplied <paramref name="services" /> collection.
        /// </summary>
        /// <typeparam name="T">Service type to check.</typeparam>
        /// <param name="services">Service collection under test.</param>
        protected static void AssertOn_RegisteredService<T>(IServiceCollection services)
        {
            var descriptors = services
                .Where(d => d.ServiceType == typeof(T))
                .ToList();
            descriptors.Should().NotBeEmpty(
                $"service {typeof(T).Name} should be registered.");
        }

        /// <summary>
        /// Asserts that the specified service type is registered with the
        /// requested implementation type (or instance type).
        /// </summary>
        /// <typeparam name="TService">Service type to look up.</typeparam>
        /// <typeparam name="TExpected">Expected implementation type.</typeparam>
        /// <param name="services">Service collection under test.</param>
        protected static void AssertOn_RegisteredServiceBeOfType<TService, TExpected>(
            IServiceCollection services)
        {
            var descriptor = services.FirstOrDefault(
                d => d.ServiceType == typeof(TService)
                     && (d.ImplementationType == typeof(TExpected)
                         || d.ImplementationInstance?.GetType() == typeof(TExpected)));
            descriptor.Should().NotBeNull(
                $"service {typeof(TService).Name} should be registered as {typeof(TExpected).Name}.");
        }
    }
}
