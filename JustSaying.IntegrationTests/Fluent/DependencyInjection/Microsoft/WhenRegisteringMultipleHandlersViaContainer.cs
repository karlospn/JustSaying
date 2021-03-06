using System;
using JustSaying.IntegrationTests.TestHandlers;
using JustSaying.Messaging.MessageHandling;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Xunit;
using Xunit.Abstractions;

namespace JustSaying.IntegrationTests.Fluent.DependencyInjection.Microsoft
{
    public class WhenRegisteringMultipleHandlersViaContainer : IntegrationTestBase
    {
        public WhenRegisteringMultipleHandlersViaContainer(ITestOutputHelper outputHelper)
            : base(outputHelper)
        {
        }

        [AwsFact]
        public void Then_An_Exception_Is_Thrown()
        {
            // Arrange
            var future = new Future<OrderPlaced>();

            var serviceProvider = GivenJustSaying()
                .ConfigureJustSaying((builder) => builder.WithLoopbackQueue<OrderPlaced>(UniqueName))
                .AddTransient<IHandlerAsync<OrderPlaced>, OrderProcessor>()
                .AddTransient<IHandlerAsync<OrderPlaced>, OrderDispatcher>()
                .AddTransient<Future<OrderPlaced>>()
                .BuildServiceProvider();

            // Act and Assert
            var exception = Assert.Throws<NotSupportedException>(() => serviceProvider.GetService<IMessagingBus>());
            exception.Message.ShouldBe("2 handlers for message type JustSaying.IntegrationTests.TestHandlers.OrderPlaced are registered. Only one handler is supported per message type.");
        }
    }
}
