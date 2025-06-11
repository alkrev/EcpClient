using Xunit;
using FluentAssertions;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Ecp.Portal;
using Ecp.Web;

namespace EcpClient.Tests.Portal
{
    public class EMDTests
    {
        private readonly Mock<IClient> _clientMock;
        private readonly EMD _emd;

        public EMDTests()
        {
            _clientMock = new Mock<IClient>();
            _emd = new EMD(_clientMock.Object);
        }

        [Fact]
        public async Task loadEMDSignBundleWindow_ShouldReturnData_WhenRequestSucceeds()
        {
            // Arrange
            var expected = new List<loadEMDSignBundleWindowReply> { new() { Document_Name = "Doc1" } };
            _clientMock
                .Setup(c => c.PostJson<List<loadEMDSignBundleWindowReply>>(It.IsAny<string>(), It.IsAny<Dictionary<string, string>>(), It.IsAny<string>()))
                .ReturnsAsync(expected);

            // Act
            var result = await _emd.loadEMDSignBundleWindow("2024-01-01", "2024-01-31", 0, 1, 10);

            // Assert
            result.Should().BeEquivalentTo(expected);
        }
        [Fact]
        public async Task loadEMDCertificateList_ShouldReturnData_WhenRequestSucceeds()
        {
            // Arrange
            var expected = new List<loadEMDCertificateListReply> { new() { EMDCertificate_id = "cert123" } };
            _clientMock
                .Setup(c => c.PostJson<List<loadEMDCertificateListReply>>(It.IsAny<string>(), It.IsAny<Dictionary<string, string>>(), It.IsAny<string>()))
                .ReturnsAsync(expected);

            // Act
            var result = await _emd.loadEMDCertificateList();

            // Assert
            result.Should().BeEquivalentTo(expected);
        }
        [Fact]
        public async Task checkBeforeSign_ShouldReturnReply_WhenSuccess()
        {
            // Arrange
            var expected = new checkBeforeSignReply { success = true };
            _clientMock
                .Setup(c => c.PostJson<checkBeforeSignReply>(It.IsAny<string>(), It.IsAny<Dictionary<string, string>>(), It.IsAny<string>()))
                .ReturnsAsync(expected);

            // Act
            var result = await _emd.checkBeforeSign("name", "id", "cert", "ver");

            // Assert
            result.Should().BeEquivalentTo(expected);
        }
        [Fact]
        public async Task getEMDVersionSignData_ShouldReturnData_WhenSuccess()
        {
            // Arrange
            var expected = new getEMDVersionSignDataReply
            {
                Error_Msg = null,
                success = true,
                toSign = new[] { new Tosign {
                    link = "link",
                    hashBase64 = "hashBase64",
                    docBase64 = "docBase64",
                    EMDVersion_id = "EMDVersion_id"
                    }
                }
            };

            _clientMock
                .Setup(c => c.PostJson<getEMDVersionSignDataReply>(It.IsAny<string>(), It.IsAny<Dictionary<string, string>>(), It.IsAny<string>()))
                .ReturnsAsync(expected);

            // Act
            var result = await _emd.getEMDVersionSignData("name", "id", "cert", 1);

            // Assert
            result.Should().BeEquivalentTo(expected);
        }
        [Fact]
        public async Task saveEMDSignatures_ShouldReturnReply_WhenSuccess()
        {
            // Arrange
            var expected = new saveEMDSignaturesReply { success = true, EMDSignatures_id = "sig123" };
            _clientMock
                .Setup(c => c.PostJson<saveEMDSignaturesReply>(It.IsAny<string>(), It.IsAny<Dictionary<string, string>>(), It.IsAny<string>()))
                .ReturnsAsync(expected);

            // Act
            var result = await _emd.saveEMDSignatures("objName", "objId", "verId", "hash", "signed", "certId");

            // Assert
            result.Should().BeEquivalentTo(expected);
        }
    }
}
