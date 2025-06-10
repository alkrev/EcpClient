using System.Collections.Generic;
using System.Threading.Tasks;

namespace Ecp.Portal
{
    public interface IEMD
    {
        Task<List<loadEMDSignBundleWindowReply>> loadEMDSignBundleWindow(string startDate, string endDate, int startIndex, int page, int limit);
        Task<List<loadEMDCertificateListReply>> loadEMDCertificateList();
        Task<checkBeforeSignReply> checkBeforeSign(string EMDRegistry_ObjectName, string EMDRegistry_ObjectID, string EMDCertificate_id, string EMDVersion_id);
        Task<getEMDVersionSignDataReply> getEMDVersionSignData(string EMDRegistry_ObjectName, string EMDRegistry_ObjectID, string EMDCertificate_id, int EMDVersion_VersionNum);
        Task<saveEMDSignaturesReply> saveEMDSignatures(string EMDRegistry_ObjectName, string EMDRegistry_ObjectID, string EMDVersion_id, string Signatures_Hash, string Signatures_SignedData, string EMDCertificate_id);
    }
}
