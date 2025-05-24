using System.Net;
using Livefront.Referrals.DataAccess.Models.DeeplinkApi;
using Livefront.Referrals.DataAccess.Models.DeeplinkApi.Models;
using Livefront.Referrals.DataAccess.Services;

namespace Livefront.Referrals.UnitTests.DataAccess.Services.ExternalDeeplinkApiServiceTests;

[TestFixture]
public class WhenDeletingLink : BaseDeeplinkApiTestFixture
{
        private static readonly Uri LinkDeleteEndpoint =
            new(LinkApiBaseAddress, DeeplinkApiConstants.DeleteLinkEndpoint);

        [SetUp]
        public void Setup()
        {
            CreateMockHttpHandlerAndHttpClient();
            externalDeeplinkApiService = new ExternalDeeplinkApiService(mockHttpClient, logger);
        }

        [Test]
        public async Task GoldenPath()
        {
            //Arrange
            var deepLink = new DeepLink(
                1,
                DateTime.UtcNow,
                DateTime.UtcNow.AddDays(1),
                "https://generated-link.com?channel=default");
            var createLinkRequest = new DeleteDeeplinkApiRequest(deepLink);
            // This point will match the id of the endpoint specifically
            var deleteEndpoint = new Uri(LinkDeleteEndpoint.AbsoluteUri + "/" + createLinkRequest.Id);
            var mockedRequestEndpoint =
                SetRequestHandler<DeepLink, DeleteDeeplinkApiRequest>(deepLink,
                    null,
                    HttpStatusCode.OK,
                    HttpMethod.Delete,
                    deleteEndpoint);

            //Act
            var deleteLink = await externalDeeplinkApiService.DeleteLink(deepLink, cancellationToken);

            //Assert
            Assert.That(deleteLink?.Id, Is.EqualTo(deepLink.Id));
            Assert.That(deleteLink?.DateCreated, Is.EqualTo(deepLink.DateCreated));
            Assert.That(deleteLink?.Id, Is.EqualTo(deepLink.Id));
            Assert.That(deleteLink?.ExpirationDate, Is.EqualTo(deepLink.ExpirationDate));
            Assert.That(deleteLink?.Link, Is.EqualTo(deepLink.Link));
            Assert.That(mockHttpHandler.GetMatchCount(mockedRequestEndpoint), Is.EqualTo(1));
            mockHttpHandler.VerifyNoOutstandingExpectation();
        }


        [Test]
        public void GivenNullDeepLink_ThenThrowNullReferenceException()
        {
            //Arrange
            //This is a random link third party id
            var deleteEndpoint = new Uri(LinkDeleteEndpoint.AbsoluteUri + "/" + 23);

            var mockedRequestEndpoint =
                SetRequestHandler<UpdateDeeplinkApiRequest, DeepLink>(
                    null,
                    null,
                    HttpStatusCode.OK,
                    HttpMethod.Put,
                    deleteEndpoint);
            //Act/Assert
            //Adding null to the delete call in the place of the actual request object
            var exception = Assert.ThrowsAsync<NullReferenceException>(async () =>
                await externalDeeplinkApiService.UpdateLinkTimeToLive(null, cancellationToken));
            Assert.That(exception.Message, Contains.Substring("deepLink"));
            Assert.That(mockHttpHandler.GetMatchCount(mockedRequestEndpoint), Is.EqualTo(0));
        }

        [Test]
        public void GivenDeepLinkThirdPartyIdIsInvalid_ThenThrowArgumentException()
        {
            //Arrange
            var deepLink = new DeepLink(
                -3,
                DateTime.UtcNow,
                DateTime.UtcNow.AddDays(1),
                "https://generated-link.com?channel=default");

            var createLinkRequest = new DeleteDeeplinkApiRequest(deepLink);
            // This point will match the id of the endpoint specifically
            var deleteEndpoint = new Uri(LinkDeleteEndpoint.AbsoluteUri + "/" + createLinkRequest.Id);

            var mockedRequestEndpoint =
                SetRequestHandler<DeepLink, DeleteDeeplinkApiRequest>(deepLink,
                    null,
                    HttpStatusCode.OK,
                    HttpMethod.Delete,
                    deleteEndpoint);

            //Act/Assert
            var exception = Assert.ThrowsAsync<ArgumentException>(async () =>
                await externalDeeplinkApiService.UpdateLinkTimeToLive(deepLink, cancellationToken));
            Assert.That(exception.Message, Contains.Substring("Id"));
            Assert.That(mockHttpHandler.GetMatchCount(mockedRequestEndpoint), Is.EqualTo(0));
        }

        [TearDown]
        public void TearDown()
        {
            mockHttpClient.Dispose();
            mockHttpHandler.Dispose();
        }
}