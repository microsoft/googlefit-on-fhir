﻿// -------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// -------------------------------------------------------------------------------------------------

using EnsureThat;
using FitOnFhir.Common.Exceptions;
using FitOnFhir.Common.Repositories;
using FitOnFhir.Common.Services;
using FitOnFhir.GoogleFit.Client.Responses;
using Microsoft.Extensions.Logging;

namespace FitOnFhir.GoogleFit.Services
{
    public class GoogleFitTokensService : TokensServiceBase<AuthTokensResponse>
    {
        private readonly IGoogleFitAuthService _googleFitAuthService;
        private readonly ILogger<GoogleFitTokensService> _logger;

        public GoogleFitTokensService()
        : base()
        {
        }

        public GoogleFitTokensService(
            IGoogleFitAuthService googleFitAuthService,
            IUsersKeyVaultRepository usersKeyVaultRepository,
            ILogger<GoogleFitTokensService> logger)
        : base(usersKeyVaultRepository, logger)
        {
            _googleFitAuthService = EnsureArg.IsNotNull(googleFitAuthService, nameof(googleFitAuthService));
            _logger = EnsureArg.IsNotNull(logger, nameof(logger));
        }

        protected override async Task<AuthTokensResponse> UpdateRefreshToken(string refreshToken, CancellationToken cancellationToken)
        {
            try
            {
                return await _googleFitAuthService.RefreshTokensRequest(refreshToken, cancellationToken);
            }
            catch (Google.Apis.Auth.OAuth2.Responses.TokenResponseException ex)
            {
                throw new TokenRefreshException(ex.Message);
            }
        }
    }
}
