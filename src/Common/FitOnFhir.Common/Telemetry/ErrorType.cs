// -------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// -------------------------------------------------------------------------------------------------

namespace Microsoft.Health.FitOnFhir.Common.Telemetry
{
    public static class ErrorType
    {
        /// <summary>
        /// A metric type for authentication errors
        /// </summary>
        public static string AuthenticationError => nameof(AuthenticationError);

        /// <summary>
        /// A metric type for errors of unknown type (e.g. unhandled exceptions)
        /// </summary>
        public static string GeneralError => nameof(GeneralError);

        /// <summary>
        /// A metric type for information
        /// </summary>
        public static string ServiceInformation => nameof(ServiceInformation);

        /// <summary>
        /// A metric type for errors that occur when interacting with FHIR resources.
        /// </summary>
        public static string FHIRResourceError => nameof(FHIRResourceError);

        /// <summary>
        /// A metric type for errors that occur when interacting with the FHIR server.
        /// </summary>
        public static string FHIRServiceError => nameof(FHIRServiceError);
    }
}
