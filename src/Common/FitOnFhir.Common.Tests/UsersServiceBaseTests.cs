﻿// -------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// -------------------------------------------------------------------------------------------------

using FitOnFhir.Common.Models;
using FitOnFhir.Common.Repositories;
using Microsoft.Health.Extensions.Fhir.Service;
using NSubstitute;
using Xunit;

namespace FitOnFhir.Common.Tests
{
    public abstract class UsersServiceBaseTests
    {
        private static IFhirService _fhirService;

        public UsersServiceBaseTests()
        {
            _fhirService = Substitute.For<IFhirService>();
            ResourceService = new ResourceManagementService(_fhirService);
            UsersTableRepository = Substitute.For<IUsersTableRepository>();

            // Default responses.
            _fhirService.SearchForResourceAsync(Hl7.Fhir.Model.ResourceType.Patient, Arg.Any<string>()).Returns(Task.FromResult(PatientBundle));
            _fhirService.CreateResourceAsync(Arg.Any<Hl7.Fhir.Model.Patient>()).Returns(Task.FromResult(PatientBundle.Entry[0].Resource as Hl7.Fhir.Model.Patient));
            UsersTableRepository.GetById(Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns(new User(Guid.NewGuid()));
        }

        protected ResourceManagementService ResourceService { get; }

        protected IUsersTableRepository UsersTableRepository { get; }

        protected abstract Hl7.Fhir.Model.Bundle PatientBundle { get; }

        protected abstract string ExpectedPatientId { get; }

        protected abstract string ExpectedPatientIdentifierSystem { get; }

        protected abstract string ExpectedPlatformUserId { get; }

        protected abstract string ExpectedPlatform { get; }

        protected abstract Func<Task> ExecuteAuthorizationCallback { get; }

        [Fact]
        public async Task GivenPatientDoesNotExist_WhenProcessAuthorizationCallbackCalled_NewPatientIsCreated()
        {
            _fhirService.SearchForResourceAsync(Hl7.Fhir.Model.ResourceType.Patient, Arg.Any<string>()).Returns(Task.FromResult(new Hl7.Fhir.Model.Bundle()));

            await ExecuteAuthorizationCallback();

            await _fhirService.Received(1).CreateResourceAsync(Arg.Is<Hl7.Fhir.Model.Patient>(x => IsExpected(x)));
        }

        [Fact]
        public async Task GivenPatientExists_WhenProcessAuthorizationCallbackCalled_CreateResourceIsNotCalled()
        {
            await ExecuteAuthorizationCallback();

            await _fhirService.DidNotReceive().CreateResourceAsync(Arg.Any<Hl7.Fhir.Model.Patient>());
        }

        [Fact]
        public async Task GivenUserDoesNotExist_WhenProcessAuthorizationCallbackCalled_NewUserIsCreated()
        {
            UsersTableRepository.GetById(Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns(Task.FromResult<User>(null));

            await ExecuteAuthorizationCallback();

            await UsersTableRepository.Received().Insert(Arg.Is<User>(x => IsExpected(x)), Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task GivenUserExists_WhenProcessAuthorizationCallbackCalled_CreateUserIsNotCalled()
        {
            await ExecuteAuthorizationCallback();

            await UsersTableRepository.DidNotReceive().Insert(Arg.Any<User>(), Arg.Any<CancellationToken>());
        }

        private bool IsExpected(Hl7.Fhir.Model.Patient patient)
        {
            return string.Equals(ExpectedPatientIdentifierSystem, patient.Identifier[0].System, StringComparison.OrdinalIgnoreCase) &&
                string.Equals(ExpectedPlatformUserId, patient.Identifier[0].Value, StringComparison.OrdinalIgnoreCase);
        }

        private bool IsExpected(User user)
        {
            return string.Equals(ExpectedPatientId, user.Id, StringComparison.OrdinalIgnoreCase) &&
                user.GetPlatformUserInfo().Any(x =>
                {
                    return string.Equals(ExpectedPlatform, x.PlatformName, StringComparison.OrdinalIgnoreCase) &&
                        string.Equals(ExpectedPlatformUserId, x.UserId, StringComparison.OrdinalIgnoreCase);
                });
        }
    }
}
