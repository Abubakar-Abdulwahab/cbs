using Parkway.CBS.Core.Models;
using Parkway.CBS.Core.Models.Enums;
using System;

namespace Parkway.CBS.ClientRepository.Repositories.Contracts
{
    public interface ITransactionLogDAOManager : IRepository<TransactionLog>
    {
        /// <summary>
        /// Create TransactionLog for Reference Data Records
        /// </summary>
        /// <param name="batchId"></param>
        /// <param name="revenueHead"></param>
        void CreateBatchInvoiceTransactionLog(long batchId, RevenueHead revenueHead);


        /// <summary>
        /// Create TransactionLog for NAGIS Old Invoice Migration
        /// </summary>
        /// <param name="batchId"></param>
        void CreateNAGISInvoiceTransactionLog(long batchId);


        /// <summary>
        /// Get the total amount received for transactions that
        /// have not been settled
        /// </summary>
        /// <param name="mdaId"></param>
        /// <param name="item"></param>
        /// <param name="bank3D"></param>
        /// <param name="web"></param>
        /// <returns>dynamic</returns>
        dynamic GetTotalAmountReceived(int mdaId, int item, PaymentProvider bank3D, PaymentChannel web, DateTime date);
        dynamic GetTotalAmountReceivedPG(int mdaId, int item, PaymentProvider bank3D, PaymentChannel web, DateTime date);
        dynamic GetTotalAmountReceivedAG1(int mdaId, int v, PaymentProvider readycash, PaymentChannel mOB, DateTime yesterday);
        dynamic GetTotalAmountReceivedAG2(int mdaId, int v, PaymentProvider readycash, PaymentChannel mOB, DateTime yesterday);
        dynamic GetTotalAmountReceivedSOH(int mdaId, int v, PaymentProvider readycash, PaymentChannel mOB, DateTime yesterday);
        dynamic GetTotalAmountReceivedNasPoly(int mdaId, int v, PaymentProvider bank3D, PaymentChannel web, DateTime yesterday);
        dynamic GetTotalAmountReceivedIDEC(int mdaId, int v, PaymentProvider bank3D, PaymentChannel web, DateTime yesterday);
        dynamic GetTotalAmountReceivedIDECNIBSS(int mdaId, int v, PaymentProvider nIBSS, PaymentChannel mOB, DateTime yesterday);
        dynamic GetTotalAmountReceivedForNiger(int mdaId, int v, PaymentProvider nIBSS, PaymentChannel mOB, DateTime yesterday);
        dynamic GetTotalAmountReceivedNasarawa(int mdaId, int v, PaymentProvider bank3D, PaymentChannel web, DateTime yesterday);
        dynamic GetTotalAmountReceivedNasarawaOnly(int mdaId, int v, PaymentProvider bank3D, PaymentChannel web, DateTime yesterday);
        dynamic GetTotalAmountReceivedNasarawaEx(int mdaId, int v, PaymentProvider bank3D, PaymentChannel web, DateTime yesterday);
        dynamic GetTotalAmountReceivedGeneric(int mdaId, int revId, PaymentProvider paymentProvider, PaymentChannel channel, DateTime yesterday);


        // MINISTRY OF TRADE AND INVESTMENT (MDA Id = 18)
        dynamic GetTotalAmountReceivedNasarawa18(int mdaId, PaymentProvider paymentProvider, PaymentChannel channel, DateTime yesterday);
        void SetSettledTransactionsToTrueNasarawa18(int mdaId, PaymentProvider bank3D, PaymentChannel web, DateTime yesterday, int who);




        void SetSettledTransactionsToTrue(int mdaId, int revenueHeadId, PaymentProvider provider, PaymentChannel channel, DateTime date, int who);
        void SetSettledTransactionsToTruePG(int mdaId, int revenueHeadId, PaymentProvider provider, PaymentChannel channel, DateTime date, int who);
        void SetSettledTransactionsToTrueAG1(int mdaId, int revenueHeadId, PaymentProvider provider, PaymentChannel channel, DateTime date, int who);
        void SetSettledTransactionsToTrueAG2(int mdaId, int revenueHeadId, PaymentProvider provider, PaymentChannel channel, DateTime date, int who);
        void SetSettledTransactionsToTrueSOH(int mdaId, int revenueHeadId, PaymentProvider provider, PaymentChannel channel, DateTime date, int who);
        void SetSettledTransactionsToTrueNasPoly(int mdaId, int revenueHeadId, PaymentProvider provider, PaymentChannel channel, DateTime date, int who);

        void SetSettledTransactionsToTrueIDEC(int mdaId, int revenueHeadId, PaymentProvider provider, PaymentChannel channel, DateTime date, int who);

        void SetSettledTransactionsToTrueIDECNIBSS(int mdaId, int revenueHeadId, PaymentProvider provider, PaymentChannel channel, DateTime date, int who);
        void SetSettledTransactionsToTrueNiger(int mdaId, int revenueHeadId, PaymentProvider provider, PaymentChannel channel, DateTime date, int who);
        void SetSettledTransactionsToTrueNasarawa(int v1, int v2, PaymentProvider bank3D, PaymentChannel web, DateTime yesterday, int v3);
        void SetSettledTransactionsToTrueNasarawaOnly(int v1, int v2, PaymentProvider p, PaymentChannel c, DateTime yesterday, int v3);
        void SetSettledTransactionsToTrueNasarawaEx(int v1, int v2, PaymentProvider p, PaymentChannel c, DateTime yesterday, int v3);
        void SetSettledTransactionsToTrueGeneric(int mdaId, int revId, PaymentProvider paymentProvider, PaymentChannel channel, DateTime yesterday, int who);




        /// <summary>
        /// SOHT
        /// </summary>
        /// <param name="mdaId"></param>
        /// <param name="v"></param>
        /// <param name="bank3D"></param>
        /// <param name="web"></param>
        /// <param name="yesterday"></param>
        /// <returns></returns>
        dynamic GetTotalAmountReceivedSOHT(int mdaId, int revId, PaymentProvider paymentProvider, PaymentChannel channel, DateTime yesterday);
        void SetSettledTransactionsToTrueSOHT(int mdaId, int revId, PaymentProvider bank3D, PaymentChannel web, DateTime yesterday, int who);

        //NASPOLY CONSULT OND (MDA Id = 5)
        dynamic GetTotalAmountReceivedNasarawa15(int mdaId, PaymentProvider p, PaymentChannel c, DateTime yesterday);
        void SetSettledTransactionsToTrueNasarawa15(int mdaId1, PaymentProvider p, PaymentChannel c, DateTime yesterday, int who);


        //NASPOLY NEW PORTAL
        dynamic GetTotalAmountReceivedNasPolyNew(int mdaId, PaymentProvider p, PaymentChannel c, DateTime yesterday);
        void SetSettledTransactionsToTrueNasPolyNew(int mdaId1, PaymentProvider p, PaymentChannel c, DateTime yesterday, int who);

        //NASPOLY CONSULT HND (MDA Id = 5)
        dynamic GetTotalAmountReceivedNasarawa5(int mdaId, PaymentProvider p, PaymentChannel c, DateTime yesterday);
        void SetSettledTransactionsToTrueNasarawa5(int mdaId1, PaymentProvider p, PaymentChannel c, DateTime yesterday, int who);



        //COLLEGE OF AGRIC – NON-PORTAL SERVICES (MDA Id = 10)
        dynamic GetTotalAmountReceivedNasarawaCollAgr(int mdaId, PaymentProvider p, PaymentChannel c, DateTime yesterday);
        void SetSettledTransactionsToTrueNasarawaCollAgr(int mdaId, PaymentProvider p, PaymentChannel c, DateTime yesterday, int who);



        dynamic GetTotalAmountReceivedNasarawaAllMDA(int mdaId, PaymentProvider p, PaymentChannel c, DateTime yesterday);
        void SetSettledTransactionsToTrueNasarawaAllMDA(int mdaId, PaymentProvider p, PaymentChannel c, DateTime yesterday, int who);

        dynamic GetTotalAmountReceivedAllMDA(int mdaId, PaymentProvider p, DateTime yesterday);
        void SetSettledTransactionsToTrueAllMDA(int mdaId, PaymentProvider p, DateTime yesterday);

        dynamic GetTotalAmountReceivedAllMDA(int mdaId, PaymentProvider p, PaymentChannel c, DateTime yesterday);
        void SetSettledTransactionsToTrueAllMDA(int mdaId, PaymentProvider p, PaymentChannel c, DateTime yesterday);


        dynamic GetTotalAmountReceivedForChitHub(int mdaId, PaymentProvider p, PaymentChannel c, DateTime yesterday);
        void SetSettledTransactionsToTrueForChitHub(int mdaId, PaymentProvider p, PaymentChannel c, DateTime yesterday, int who);

        //
        dynamic GetTotalAmountReceivedRevListAllChannels(string revList, PaymentProvider provider, DateTime date);
        void SetSettledTransactionsToTrueRevListAllChannels(string revList, PaymentProvider provider, DateTime date);


        dynamic GetTotalAmountReceivedMDAAndRevList(int mdaId, string revList, PaymentProvider provider, PaymentChannel channel, DateTime yesterday);
        void SetSettledTransactionsToTrueMDAAndRevList(int mdaId, string revList, PaymentProvider provider, PaymentChannel channel, DateTime yesterday);

        dynamic GetTotalAmountReceivedMDAAndRevList(int mdaId, string revList, PaymentProvider provider, DateTime yesterday);
        void SetSettledTransactionsToTrueMDAAndRevList(int mdaId, string revList, PaymentProvider provider, DateTime yesterday);


        dynamic GetTotalAmountReceivedMDAAndOneRev(int mdaId, int revId, PaymentProvider provider, PaymentChannel channel, DateTime yesterday);
        void SetSettledTransactionsToTrueMDAAndOneRev(int mdaId, int revId, PaymentProvider provider, PaymentChannel channel, DateTime yesterday);


        //less settlement fee
        dynamic GetTotalAmountReceivedMDAAndOneRevLessSettlementFee(int mdaId, int revId, PaymentProvider provider, PaymentChannel channel, DateTime yesterday);


        dynamic GetTotalAmountReceivedMDAAndOneRevAllChannel(int mdaId, int revId, PaymentProvider provider, DateTime yesterday);
        void SetSettledTransactionsToTrueMDAAndOneRevAllChannel(int mdaId, int revId, PaymentProvider provider, DateTime yesterday);


        //DASH01
        dynamic GetTotalAmountReceivedDASH01(int mdaId, PaymentProvider provider, PaymentChannel channel, DateTime yesterday);

        void SetSettledTransactionsToTrueDASH01(int mdaId, PaymentProvider provider, PaymentChannel channel, DateTime yesterday, int who);

        //DASH02
        dynamic GetTotalAmountReceivedDASH02(int mdaId, PaymentProvider provider, PaymentChannel channel, DateTime yesterday);

        void SetSettledTransactionsToTrueDASH02(int mdaId, PaymentProvider provider, PaymentChannel channel, DateTime yesterday, int who);

        //DASH03
        dynamic GetTotalAmountReceivedDASH03(int mdaId, PaymentProvider provider, PaymentChannel channel, DateTime yesterday);

        void SetSettledTransactionsToTrueDASH03(int mdaId, PaymentProvider provider, PaymentChannel channel, DateTime yesterday, int who);

        //DASH04
        dynamic GetTotalAmountReceivedDASH04(int mdaId, PaymentProvider provider, PaymentChannel channel, DateTime yesterday);

        void SetSettledTransactionsToTrueDASH04(int mdaId, PaymentProvider provider, PaymentChannel channel, DateTime yesterday, int who);


        //DASH05
        dynamic GetTotalAmountReceivedDASH05(int mdaId, PaymentProvider provider, PaymentChannel channel, DateTime yesterday);

        void SetSettledTransactionsToTrueDASH05(int mdaId, PaymentProvider provider, PaymentChannel channel, DateTime yesterday, int who);

        //CEA128
        dynamic GetTotalAmountReceivedCEA128(int mdaId, PaymentProvider provider, PaymentChannel channel, DateTime yesterday);

        void SetSettledTransactionsToTrueCEA128(int mdaId, PaymentProvider provider, PaymentChannel channel, DateTime yesterday, int who);


    }
}
