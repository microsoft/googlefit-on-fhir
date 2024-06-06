// -------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// -------------------------------------------------------------------------------------------------

using Microsoft.Health.FitOnFhir.Common.Telemetry;

namespace Microsoft.Health.FitOnFhir.Common.Exceptions
{
    public sealed class UnauthorizedAccessFhirServiceException : ThirdPartyLoggedFormattableException
    {
        private static readonly string _errorType = ErrorType.FHIRServiceError;

        public UnauthorizedAccessFhirServiceException()
        {
        }

        public UnauthorizedAccessFhirServiceException(string message)
            : base(message)
        {
        }

        public UnauthorizedAccessFhirServiceException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        public UnauthorizedAccessFhirServiceException(
            string message,
            Exception innerException,
            string helpLink,
            string errorName)
            : base(
                  message,
                  innerException,
                  name: $"{_errorType}{errorName}",
                  operation: "FHIRConversion")
        {
            HelpLink = helpLink;
        }

        public override string ErrType => _errorType;

        public override string ErrSeverity => ErrorSeverity.Critical;

        public override string ErrSource => nameof(ErrorSource.User);
    }
}
