// -------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// -------------------------------------------------------------------------------------------------

using EnsureThat;
using Microsoft.Extensions.Logging;
using Microsoft.Health.Fhir.Client;
using Microsoft.Health.FitOnFhir.Common.Telemetry;

namespace Microsoft.Health.FitOnFhir.Common.ExtensionMethods
{
    public static class FhirClientValidatorExtensions
    {
        public static async Task<bool> ValidateFhirClientAsync(
            this IFhirClient client,
            ITelemetryLogger telemetryLogger,
            ILogger<TelemetryExceptionProcessor> logger)
        {
            EnsureArg.IsNotNull(client, nameof(client));
            EnsureArg.IsNotNull(telemetryLogger, nameof(telemetryLogger));
            EnsureArg.IsNotNull(logger, nameof(logger));

            try
            {
                await client.ReadAsync<Hl7.Fhir.Model.CapabilityStatement>("metadata?_summary=true").ConfigureAwait(false);
                return true;
            }
            catch (ArgumentException exception)
            {
                FhirServiceExceptionProcessor.ProcessException(exception, telemetryLogger, logger);
                return false;
            }
        }
    }
}
