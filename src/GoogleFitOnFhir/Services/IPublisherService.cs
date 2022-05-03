﻿// -------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// -------------------------------------------------------------------------------------------------

using System.Threading;
using System.Threading.Tasks;
using GoogleFitOnFhir.Models;
using Microsoft.Health.Common.Handler;

namespace GoogleFitOnFhir.Services
{
    public interface IPublisherService
    {
        /// <summary>
        /// Passes along a <see cref="QueueMessage"/> to an <see cref="IResponsibilityHandler{TRequest,TResult}"/>, which
        /// can then evaluate it and have the appropriate platform specific data publishing handler take action.
        /// </summary>
        /// <param name="message">The <see cref="QueueMessage"/> to take action on.</param>
        /// <param name="cancellationToken">A cancellation token for graceful recovery.</param>
        public Task PublishTo(QueueMessage message, CancellationToken cancellationToken);
    }
}
