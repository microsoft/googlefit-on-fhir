// -------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// -------------------------------------------------------------------------------------------------

using Azure.Core;
using Azure.Identity;
using EnsureThat;
using Microsoft.Health.FitOnFhir.Common.Interfaces;

namespace Microsoft.Health.FitOnFhir.Common.Services
{
    public class ManagedIdentityAuthService : TokenCredential, IFhirTokenProvider
    {
        private readonly TokenCredential _tokenCredential;

        public ManagedIdentityAuthService()
        {
            _tokenCredential = new DefaultAzureCredential();
        }

        public ManagedIdentityAuthService(IAzureCredentialProvider azureCredentialProvider)
        {
            EnsureArg.IsNotNull(azureCredentialProvider, nameof(azureCredentialProvider));

            _tokenCredential = azureCredentialProvider.GetCredential();
        }

        public override AccessToken GetToken(TokenRequestContext requestContext, CancellationToken cancellationToken)
        {
            return _tokenCredential.GetToken(requestContext, cancellationToken);
        }

        public async override ValueTask<AccessToken> GetTokenAsync(TokenRequestContext requestContext, CancellationToken cancellationToken)
        {
            return await _tokenCredential.GetTokenAsync(requestContext, cancellationToken);
        }

        public TokenCredential GetTokenCredential()
        {
            return this;
        }
    }
}
