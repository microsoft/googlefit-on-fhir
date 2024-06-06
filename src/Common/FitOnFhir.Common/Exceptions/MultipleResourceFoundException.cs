// -------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// -------------------------------------------------------------------------------------------------

using System.Text;
using Microsoft.Health.FitOnFhir.Common.Interfaces;
using Microsoft.Health.FitOnFhir.Common.Telemetry;

namespace Microsoft.Health.FitOnFhir.Common.Exceptions
{
    public class MultipleResourceFoundException<T> : ThirdPartyLoggedFormattableException
    {
        public MultipleResourceFoundException(int resourceCount)
            : base(GenerateErrorMessage<T>(resourceCount, null))
        {
        }

        public MultipleResourceFoundException()
        {
        }

        public MultipleResourceFoundException(int resourceCount, IEnumerable<IResourceMetadata> metadata)
            : base(GenerateErrorMessage<T>(resourceCount, metadata))
        {
        }

        public MultipleResourceFoundException(string message)
            : base(message)
        {
        }

        public MultipleResourceFoundException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        public override string ErrName => $"Multiple{typeof(T).Name}FoundException";

        public override string ErrType => ErrorType.FHIRResourceError;

        public override string Operation => "FHIRConversion";

        private static string GenerateErrorMessage<TResource>(int resourceCount, IEnumerable<IResourceMetadata> metadata)
        {
            var sb = new StringBuilder($"Multiple resources {resourceCount} of type {typeof(T)} found, expected one.");

            if (metadata != null)
            {
                sb.Append(" Resource metadata: ")
                    .Append(string.Join(", ", metadata.Select(m => $"[Resource {nameof(m.Id)}:{m.Id}; {nameof(m.VersionId)}:{m.VersionId}; {nameof(m.LastUpdated)}:{m.LastUpdated}]")))
                    .Append('.');
            }

            return sb.ToString();
        }
    }
}
