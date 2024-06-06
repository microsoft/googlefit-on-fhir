// -------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// -------------------------------------------------------------------------------------------------

using System.Net.Http.Headers;
using Azure.Core;
using EnsureThat;
using Microsoft.Health.FitOnFhir.Common.Interfaces;
using Microsoft.Health.FitOnFhir.Common.Telemetry;

namespace Microsoft.Health.FitOnFhir.Common.Handlers
{
    public class BearerTokenAuthorizationMessageHandler : DelegatingHandler
    {
        public BearerTokenAuthorizationMessageHandler(Uri uri, IFhirTokenProvider tokenProvider, ITelemetryLogger telemetryLogger)
        {
            TokenProvider = EnsureArg.IsNotNull(tokenProvider, nameof(tokenProvider));
            Uri = EnsureArg.IsNotNull(uri, nameof(uri));
            TelemetryLogger = EnsureArg.IsNotNull(telemetryLogger, nameof(telemetryLogger));
            Scopes = new string[] { Uri.ToString().EndsWith('/'.ToString(), StringComparison.InvariantCultureIgnoreCase) ? Uri + ".default" : Uri + "/.default" };
        }

        private ITelemetryLogger TelemetryLogger { get; }

        private IFhirTokenProvider TokenProvider { get; }

        private Uri Uri { get; }

        private string[] Scopes { get; }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            EnsureArg.IsNotNull(request, nameof(request));

            var requestContext = new TokenRequestContext(Scopes);
            var accessToken = await TokenProvider.GetTokenCredential()
                .GetTokenAsync(requestContext, cancellationToken);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken.Token);

            var response = await base.SendAsync(request, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                var statusDescription = response.ReasonPhrase.Replace(" ", string.Empty, StringComparison.InvariantCultureIgnoreCase);
                var severity = response.StatusCode == System.Net.HttpStatusCode.TooManyRequests ? ErrorSeverity.Informational : ErrorSeverity.Critical;
                TelemetryLogger.LogMetric(FhirClientMetrics.HandledException($"{ErrorType.FHIRServiceError}{statusDescription}", severity), 1);
            }

            return response;
        }
    }
}
