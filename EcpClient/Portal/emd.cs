using Ecp.Web;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Ecp.Portal
{
    public class EMD : IEMD
    {
        IClient wc;
        public EMD(IClient wc)
        {
            this.wc = wc;
        }
        /**
         * Выполняем поиск документов для подачи в РЭМД по диапазону дат
         */
        public async Task<List<loadEMDSignBundleWindowReply>> loadEMDSignBundleWindow(string startDate, string endDate, int startIndex, int page, int limit)
        {
            string url = $"?c=EMD&m=loadEMDSignBundleWindow&_dc={DateTime.UtcNow.ToUniversalTime().Subtract(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds}";
            string referer = "?c=promed";
            var parameters = new Dictionary<string, string>() {
                { "LpuBuilding_id","null" },
                { "EMDRegistry_EMDDate_period",$"{startDate} - {endDate}"},
                { "EMDVersion_RegistrationDate_period","" },
                { "EMDRegistry_Num","" },
                { "Person_FIO","" },
                { "EMDDocumentType_Code","null" },
                { "EMDVersionStatus","" },
                { "ReceptType_id","" },
                { "isLpuSignNeeded","on" },
                { "page",$"{page}"},
                { "limit",$"{limit}" },
                { "isMOSign","true" },
                { "start",$"{startIndex}" }
            };
            return await wc.PostJson<List<loadEMDSignBundleWindowReply>>(url, parameters, referer);
        }
        /**
         * Выполняем поиск сертификатов пользователя в ECP
         */
        public async Task<List<loadEMDCertificateListReply>> loadEMDCertificateList()
        {
            string url = $"?c=EMD&m=loadEMDCertificateList&_dc={DateTime.UtcNow.ToUniversalTime().Subtract(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds}";
            string referer = "?c=promed";
            var parameters = new Dictionary<string, string>() {
                { "excludeExpire","true" },
                { "excludeIsNotUse","true" },
                { "isMOSign","1" },
                { "page","1" },
                { "start","0" },
                { "limit","25" },
            };
            return await wc.PostJson<List<loadEMDCertificateListReply>>(url, parameters, referer);
        }
        /**
         * Проверка перед подписанием
         */
        public async Task<checkBeforeSignReply> checkBeforeSign(string EMDRegistry_ObjectName, string EMDRegistry_ObjectID, string EMDCertificate_id, string EMDVersion_id)
        {
            string url = $"?c=EMD&m=checkBeforeSign";
            string referer = "?c=promed";
            var parameters = new Dictionary<string, string>() {
                { "EMDRegistry_ObjectName", EMDRegistry_ObjectName },
                { "EMDRegistry_ObjectID", EMDRegistry_ObjectID },
                { "EMDCertificate_id", EMDCertificate_id },
                { "EMDVersion_id", EMDVersion_id },
                { "isMOSign","true" },
                { "isPreview","" },
            };
            return await wc.PostJson<checkBeforeSignReply>(url, parameters, referer);
        }
        /**
         * Получаем документ для подписания
         */
        public async Task<getEMDVersionSignDataReply> getEMDVersionSignData(string EMDRegistry_ObjectName, string EMDRegistry_ObjectID, string EMDCertificate_id, int EMDVersion_VersionNum)
        {
            string url = $"?c=EMD&m=getEMDVersionSignData";
            string referer = "?c=promed";
            var parameters = new Dictionary<string, string>() {
                { "EMDRegistry_ObjectName", EMDRegistry_ObjectName },
                { "EMDRegistry_ObjectID", EMDRegistry_ObjectID },
                { "isMOSign", "true" },
                { "EMDCertificate_id", EMDCertificate_id },
                { "isPreview","" },
                { "EMDVersion_VersionNum",EMDVersion_VersionNum.ToString()},
            };
            return await wc.PostJson<getEMDVersionSignDataReply>(url, parameters, referer);
        }
        /**
         * Сохраняем подпись
         */
        public async Task<saveEMDSignaturesReply> saveEMDSignatures(string EMDRegistry_ObjectName, string EMDRegistry_ObjectID, string EMDVersion_id, string Signatures_Hash, string Signatures_SignedData, string EMDCertificate_id)
        {
            string url = $"?c=EMD&m=saveEMDSignatures";
            string referer = "?c=promed";
            var parameters = new Dictionary<string, string>() {
                { "EMDRegistry_ObjectName", EMDRegistry_ObjectName },
                { "EMDRegistry_ObjectID", EMDRegistry_ObjectID },
                { "EMDVersion_id", EMDVersion_id },
                { "Signatures_Hash", Signatures_Hash },
                { "Signatures_SignedData", Signatures_SignedData },
                { "EMDCertificate_id", EMDCertificate_id },
                { "signType", "cryptopro" },
                { "isMOSign", "true" },
                { "LpuSection_id", "" },
                { "MedService_id", "" },
            };
            return await wc.PostJson<saveEMDSignaturesReply>(url, parameters, referer);
        }
    }
    public class loadEMDSignBundleWindowReply
    {
        ///<summary>Наименование документа</summary>
        public string Document_Name = "";
        ///<summary>Номер документа</summary>
        public string Document_Num = "";
        ///<summary>ID документа</summary>
        public string EMDRegistry_ObjectID = "";
        ///<summary>Тип документа</summary>
        public string EMDRegistry_ObjectName = "";
        ///<summary>ID версии документа</summary>
        public string EMDVersion_id = "";
        ///<summary>Ошибка</summary>
        public string Error_Msg = "";
        ///<summary>Статус подписания (IsSigned == "2" - требуется подпись МО)</summary>
        public string IsSigned = "";
        ///<summary>Номер версии документа</summary>
        public int EMDVersion_VersionNum;
    }
    public class loadEMDCertificateListReply
    {
        public string EMDCertificate_id = "";
        public string EMDCertificate_SHA1 = "";
    }
    public class checkBeforeSignReply
    {
        public string Error_Msg = "";
        public bool success = false;
    }
    public class getEMDVersionSignDataReply
    {
        public string Error_Msg { get; set; }
        public Tosign[] toSign { get; set; }
        public bool success { get; set; }
    }
    public class Tosign
    {
        public string link { get; set; }
        public string hashBase64 { get; set; }
        public string docBase64 { get; set; }
        public string EMDVersion_id { get; set; }
    }
    public class saveEMDSignaturesReply
    {
        public string Error_Msg = "";
        public bool success = false;
        public string EMDSignatures_id = "";
    }
}
