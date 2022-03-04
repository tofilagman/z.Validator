using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NUnit.Framework;
using System.Net;
using System.Threading.Tasks;
using z.ServiceProvider;
using z.Validator.Test.Models;
using z.Validator.Test.Services;

namespace z.Validator.Test
{
    public class ValidateTest
    {
        private IHost Provider;

        [SetUp]
        public void Setup()
        {
            var builder = Host.CreateDefaultBuilder();
            builder.ConfigureServices(services =>
            {
                services.AddHttpContextAccessor();
                services.AddSingleton<IServiceProviderProxy, HttpContextServiceProviderProxy>();

                services.AddScoped<SampleService>();
                services.AddScoped<IThrowableValidator<TestThrowableToken>, SampleService>();
                services.AddScoped<IThrowableValidator<TestAsyncToken>, SampleService>();
                services.AddScoped<IAutoUniqueValidator<UniqueToken>, SampleService>();
            });

            Provider = builder.Build();

            ServiceLocator.Initialize(Provider.Services.GetService<IServiceProviderProxy>());
        }

        [Test]
        public async Task TestPositive()
        {
            var serv = Provider.Services.GetRequiredService<SampleService>();

            var token = new TestToken
            {
                LastName = "Sample"
            };

            var res = await serv.TestAsync(token);

            Assert.AreEqual(token.LastName, res.LastName);
        }

        [Test]
        public async Task TestNegative()
        {
            var serv = Provider.Services.GetRequiredService<SampleService>();

            var token = new TestToken
            {
                LastName = null
            };

            var exception = Assert.ThrowsAsync<ModelValidationException>(async () =>
         {
             await serv.TestAsync(token);
         });

            Assert.AreEqual(1, exception.Results.Count);
            await Task.CompletedTask;
        }

        [Test]
        public async Task TestThrowablePositive()
        {
            var serv = Provider.Services.GetRequiredService<SampleService>();

            var token = new TestThrowableToken
            {
                LastName = "Penduko"
            };

            var res = await serv.TestThrowableAsync(token);


            Assert.AreEqual(res.LastName, token.LastName);
        }

        [Test]
        public async Task TestThrowableNegative()
        {
            var serv = Provider.Services.GetRequiredService<SampleService>();

            var token = new TestThrowableToken
            {
                LastName = "Rizal"
            };

            var exception = Assert.ThrowsAsync<ModelValidationException>(async () =>
            {
                await serv.TestThrowableAsync(token);
            });

            Assert.AreEqual(1, exception.Results.Count);
            await Task.CompletedTask;
        }

        [Test]
        public async Task TestAsyncPositive()
        {
            var serv = Provider.Services.GetRequiredService<SampleService>();

            var token = new TestAsyncToken
            {
                LastName = "Penduko"
            };

            var res = await serv.TestAsyncronousValidation(token);


            Assert.AreEqual(res.LastName, token.LastName);
        }

        [Test]
        public async Task TestAsyncNegative()
        {
            var serv = Provider.Services.GetRequiredService<SampleService>();

            var token = new TestAsyncToken
            {
                LastName = "Rizal"
            };

            var exception = Assert.ThrowsAsync<ModelValidationException>(async () =>
            {
                await serv.TestAsyncronousValidation(token);
            });

            Assert.AreEqual(1, exception.Results.Count);
            await Task.CompletedTask;
        }

        [Test]
        public async Task TestUnique()
        {
            var serv = Provider.Services.GetRequiredService<SampleService>();

            var token = new UniqueToken
            {
                LastName = "Rizal"
            };

            var res = await serv.TestUnique(token);

            Assert.AreEqual(res.LastName, token.LastName);

        }
    }
}