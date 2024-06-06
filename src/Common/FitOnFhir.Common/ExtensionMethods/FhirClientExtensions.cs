// -------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// -------------------------------------------------------------------------------------------------

using EnsureThat;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Health.FitOnFhir.Common.Handlers;
using Microsoft.Health.FitOnFhir.Common.Interfaces;
using Microsoft.Health.FitOnFhir.Common.Services;
using Microsoft.Health.FitOnFhir.Common.Telemetry;
using FhirClient = Microsoft.Health.Fhir.Client.FhirClient;
using IFhirClient = Microsoft.Health.Fhir.Client.IFhirClient;

namespace Microsoft.Health.FitOnFhir.Common.ExtensionMethods
{
    public static class FhirClientExtensions
    {
        public static IServiceCollection AddFhirClient(this IServiceCollection serviceCollection, IConfiguration configuration, IAzureCredentialProvider credentialProvider = null)
        {
            EnsureArg.IsNotNull(serviceCollection, nameof(serviceCollection));
            EnsureArg.IsNotNull(configuration, nameof(configuration));

            Uri url = new (configuration.GetValue<string>("FhirService:Url"));
            bool useManagedIdentity = configuration.GetValue<bool>("FhirClient:UseManagedIdentity");

            serviceCollection.TryAddSingleton<IFhirTokenProvider>(sp =>
            {
                var tokenProvider = sp.GetService<IAzureExternalIdentityCredentialProvider>() ?? sp.GetService<IAzureCredentialProvider>();

                if (useManagedIdentity)
                {
                    return new ManagedIdentityAuthService();
                }
                else if (tokenProvider != null)
                {
                    return new ManagedIdentityAuthService(tokenProvider);
                }
                else
                {
                    throw new NotSupportedException();
                }
            });

            serviceCollection.AddHttpClient<IFhirClient, FhirClient>((client, sp) =>
            {
                client.BaseAddress = url;
                client.Timeout = TimeSpan.FromSeconds(60);

                ITelemetryLogger telemetryLogger = sp.GetRequiredService<ITelemetryLogger>();
                ILogger<TelemetryExceptionProcessor> logger = sp.GetRequiredService<ILogger<TelemetryExceptionProcessor>>();

                // Using discard because we don't need result
                var fhirClient = new FhirClient(client);
                _ = fhirClient.ValidateFhirClientAsync(telemetryLogger, logger);

                return fhirClient;
            })
            .AddAuthenticationHandler(url);

            return serviceCollection;
        }

        public static void AddAuthenticationHandler(
           this IHttpClientBuilder httpClientBuilder,
           Uri uri)
        {
            EnsureArg.IsNotNull(httpClientBuilder, nameof(httpClientBuilder));
            EnsureArg.IsNotNull(uri, nameof(uri));

            httpClientBuilder.AddHttpMessageHandler(sp =>
                    new BearerTokenAuthorizationMessageHandler(uri, sp.GetRequiredService<IFhirTokenProvider>(), sp.GetRequiredService<ITelemetryLogger>()));
        }
    }
}
