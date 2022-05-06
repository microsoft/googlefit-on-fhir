// -------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// -------------------------------------------------------------------------------------------------

using System.Threading;
using System.Threading.Tasks;
using Google.Apis.Auth.OAuth2;
using Google.Apis.PeopleService.v1;
using Google.Apis.Services;
using GoogleFitOnFhir.Clients.GoogleFit.Responses;

namespace GoogleFitOnFhir.Clients.GoogleFit.Requests
{
    public class MyEmailRequest
    {
        private readonly PeopleServiceService _peopleService;

        public MyEmailRequest(string accessToken)
        {
            _peopleService = new PeopleServiceService(
                new BaseClientService.Initializer()
            {
                HttpClientInitializer =
                    GoogleCredential.FromAccessToken(accessToken),
            });
        }

        public async Task<MyEmailResponse> ExecuteAsync(CancellationToken cancellationToken)
        {
            var request = _peopleService.People.Get("people/me");
            request.PersonFields = "emailAddresses";

            var data = await request.ExecuteAsync(cancellationToken);

            var response = new MyEmailResponse
            {
                EmailAddress = data.EmailAddresses != null ? data.EmailAddresses[0].Value : string.Empty,
            };

            return response;
        }
    }
}