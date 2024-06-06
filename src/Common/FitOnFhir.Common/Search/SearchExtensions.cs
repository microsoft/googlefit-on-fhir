// -------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// -------------------------------------------------------------------------------------------------

using EnsureThat;
using Hl7.Fhir.Model;

namespace Microsoft.Health.FitOnFhir.Common.Search
{
    public static class SearchExtensions
    {
        public const string SearchDateFormat = "yyyy-MM-ddTHH:mm:ssZ";

        public static string ToSearchToken(this Identifier identifier)
        {
            EnsureArg.IsNotNull(identifier, nameof(identifier));

            var token = string.Empty;
            if (!string.IsNullOrEmpty(identifier.System))
            {
                token += $"{identifier.System}|";
            }

            token += identifier.Value;
            return token;
        }

        public static string ToSearchQueryParameter(this Hl7.Fhir.Model.Identifier identifier)
        {
            EnsureArg.IsNotNull(identifier, nameof(identifier));

            return $"identifier={identifier.ToSearchToken()}";
        }
    }
}
