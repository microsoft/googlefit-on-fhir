﻿// -------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// -------------------------------------------------------------------------------------------------

using System.Net;
using EnsureThat;
using Microsoft.Extensions.Logging;
using Microsoft.Health.Fhir.Client;
using Microsoft.Health.FitOnFhir.Common.Exceptions;
using Microsoft.Health.FitOnFhir.Common.Resources;
using Microsoft.Identity.Client;

namespace Microsoft.Health.FitOnFhir.Common.Telemetry
{
    public static class FhirServiceExceptionProcessor
    {
        private static readonly ExceptionTelemetryProcessor _exceptionTelemetryProcessor = new ExceptionTelemetryProcessor();

        public static void ProcessException(Exception exception, ITelemetryLogger telemetryLogger, ILogger<TelemetryExceptionProcessor> logger)
        {
            EnsureArg.IsNotNull(telemetryLogger, nameof(telemetryLogger));
            EnsureArg.IsNotNull(logger, nameof(logger));

            var (customException, errorName) = CustomizeException(exception);

            telemetryLogger.LogError(customException);

            string exceptionName = customException.Equals(exception) ? $"{ErrorType.FHIRServiceError}{errorName}" : customException.GetType().Name;
            _exceptionTelemetryProcessor.LogExceptionMetric(customException, telemetryLogger, FhirClientMetrics.HandledException(exceptionName, ErrorSeverity.Critical));
        }

        public static (Exception customException, string errorName) CustomizeException(Exception exception)
        {
            EnsureArg.IsNotNull(exception, nameof(exception));

            string message;
            string errorName;

            switch (exception)
            {
                case FhirClientException fhirClientException:
                    var status = fhirClientException.StatusCode;
                    switch (status)
                    {
                        case HttpStatusCode.Forbidden:
                            message = FhirResources.FhirServiceAccessForbidden;
                            string helpLink = "https://docs.microsoft.com/azure/healthcare-apis/iot/deploy-iot-connector-in-azure#accessing-the-iot-connector-from-the-fhir-service";
                            errorName = nameof(FhirServiceErrorCode.AuthorizationError);
                            return (new UnauthorizedAccessFhirServiceException(message, exception, helpLink, errorName), errorName);
                        case HttpStatusCode.NotFound:
                            message = FhirResources.FhirServiceNotFound;
                            errorName = nameof(FhirServiceErrorCode.ConfigurationError);
                            return (new InvalidFhirServiceException(message, exception, errorName), errorName);
                        default:
                            return (exception, status.ToString());
                    }

                case ArgumentException argumentException:
                    var paramName = argumentException.ParamName;
                    if (paramName.Contains("endpoint", StringComparison.OrdinalIgnoreCase))
                    {
                        message = FhirResources.FhirServiceEndpointInvalid;
                        errorName = nameof(FhirServiceErrorCode.ConfigurationError);
                        return (new InvalidFhirServiceException(message, exception, errorName), errorName);
                    }

                    return (exception, $"{FhirServiceErrorCode.ArgumentError}{paramName}");

                case UriFormatException _:
                    message = FhirResources.FhirServiceUriFormatInvalid;
                    errorName = nameof(FhirServiceErrorCode.ConfigurationError);
                    return (new InvalidFhirServiceException(message, exception, errorName), errorName);

                case HttpRequestException _:

                    if (exception.Message.Contains(FhirResources.HttpRequestErrorNotKnown, StringComparison.CurrentCultureIgnoreCase))
                    {
                        message = FhirResources.FhirServiceHttpRequestError;
                        errorName = nameof(FhirServiceErrorCode.ConfigurationError);
                        return (new InvalidFhirServiceException(message, exception, errorName), errorName);
                    }

                    var statusCode = ((HttpRequestException)exception).StatusCode;
                    return (exception, $"{FhirServiceErrorCode.HttpRequestError}{statusCode}");

                case MsalServiceException msalServiceException:
                    var errorCode = msalServiceException.ErrorCode;
                    if (string.Equals(errorCode, "invalid_resource", StringComparison.OrdinalIgnoreCase)
                        || string.Equals(errorCode, "invalid_scope", StringComparison.OrdinalIgnoreCase))
                    {
                        message = FhirResources.FhirServiceMsalServiceError;
                        errorName = nameof(FhirServiceErrorCode.ConfigurationError);
                        return (new InvalidFhirServiceException(message, exception, errorName), errorName);
                    }

                    return (exception, $"{FhirServiceErrorCode.MsalServiceError}{errorCode}");

                default:
                    return (exception, nameof(FhirServiceErrorCode.GeneralError));
            }
        }
    }
}
