using NSubstitute;
using NUnit.Framework;
using Parkway.CBS.ClientFileServices.Implementations.IPPIS;
using Parkway.CBS.ClientRepository;
using Parkway.CBS.ClientRepository.Repositories.Contracts;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Core.Models.Enums;
using Parkway.CBS.Entities.VMs;
using Parkway.CBS.Payee.PayeeAdapters.Contracts;
using Parkway.CBS.Payee.PayeeAdapters.IPPIS;
using System;
using System.IO;

namespace Parkway.CBS.ClientFileServicesTests.IPPIS
{
    /// <summary>
    /// Summary description for IPPISFileServicesTests
    /// </summary>
    [TestFixture]
    public class IPPISFileServicesTests
    {
        [Test]
        public void if_bad_directory_info_is_passed_for_month_check_that_an_error_occurs()
        {
            string path = "C:\\Parkway\\Repository\\CBS\\src\\Parkway.CBS.ClientFileServicesTests\\testfiles\\taxamountemptyentry.xlsx";
            DirectoryInfo dirInfo = new DirectoryInfo(path);
            var serviceProps = new FileServiceHelper { TenantName = "Nasarawa", StateId = 2, UnknownTaxPayerCodeId = 63, Month = 8, Year = 2019 };
            var process = new IPPISFileProcessor();
            Assert.Throws(typeof(FormatException), () => process.ValidateTheFile(serviceProps, path));
        }

        [Test]
        public void error_if_bad_header_values_are_in_file()
        {
            //C:\SFTP\2019\March
            string path = "C:\\SFTP\\2019\\March\\badheadervalue.xlsx";
            DirectoryInfo dirInfo = new DirectoryInfo(path);
            var process = new IPPISFileProcessor();
            //set unit of work
            process.UoW = Substitute.For<IUoW>();
            process.UoW.BeginTransaction().Returns(process.UoW);
            process.UoW.Commit();

            process.BatchDAO = Substitute.For<IIPPISBatchDAOManager>();
            IPPISBatch nullValue = null;
            IPPISBatch somethingValue = new IPPISBatch { Id = 0 };
            process.BatchDAO.GetRecordForMonthAndYear(3, 2019).Returns<IPPISBatch>(nullValue);
            process.BatchDAO.Add(somethingValue);

            process.PayeeAdapter = Substitute.For<IIPPISPayeeAdapter>();
            process.PayeeAdapter.GetPayeeResponseModels<IPPISPayeeResponse>(path, null, null, 3, 2019).Returns(new IPPISPayeeResponse { HeaderValidateObject = new CBS.Payee.HeaderValidateObject { Error = true} });

            var serviceProps = new FileServiceHelper { TenantName = "Nasarawa", StateId = 2, UnknownTaxPayerCodeId = 63, Month = 8, Year = 2019 };

            var actualResponse = process.ValidateTheFile(serviceProps, path);
            Assert.AreEqual(true, actualResponse.ErrorOccurred);
            Assert.AreEqual(0, actualResponse.BatchId);
        }


        [Test]
        public void check_that_exception_is_thrown_if_the_file_has_been_processed()
        {
            //C:\SFTP\2019\March
            string path = "C:\\SFTP\\2019\\March\\badheadervalue.xlsx";
            DirectoryInfo dirInfo = new DirectoryInfo(path);
            var process = new IPPISFileProcessor();
            //set unit of work
            process.UoW = Substitute.For<IUoW>();
            process.BatchDAO = Substitute.For<IIPPISBatchDAOManager>();
            process.BatchDAO.GetRecordForMonthAndYear(3, 2019).Returns<IPPISBatch>(new IPPISBatch { Id = 0, ProccessStage = (int)IPPISProcessingStages.Processed });

            var serviceProps = new FileServiceHelper { TenantName = "Nasarawa", StateId = 2, UnknownTaxPayerCodeId = 63, Month = 8, Year = 2019 };

            var exception = Assert.Catch<InvalidOperationException>(() => process.ValidateTheFile(serviceProps, path));
            Assert.AreEqual("IPPIS processing for this month 3 and year 2019 have already been completed.", exception.Message);
        }

        [Test]
        public void check_that_exception_is_thrown_if_the_processing_of_the_file_has_been_tagged_for_processing()
        {
            //C:\SFTP\2019\March
            string path = "C:\\SFTP\\2019\\March\\badheadervalue.xlsx";
            DirectoryInfo dirInfo = new DirectoryInfo(path);
            var process = new IPPISFileProcessor();
            //set unit of work
            process.UoW = Substitute.For<IUoW>();
            process.BatchDAO = Substitute.For<IIPPISBatchDAOManager>();
            process.BatchDAO.GetRecordForMonthAndYear(3, 2019).Returns<IPPISBatch>(new IPPISBatch { Id = 0, ProccessStage = (int)IPPISProcessingStages.CategorizationOfTaxPayerByCode });
            var serviceProps = new FileServiceHelper { TenantName = "Nasarawa", StateId = 2, UnknownTaxPayerCodeId = 63 };
            var exception = Assert.Catch<InvalidOperationException>(() => process.ValidateTheFile(serviceProps, path));
            Assert.AreEqual("IPPIS processing for this month 3 and year 2019 is still ongoing.", exception.Message);
        }


        [Test]
        public void returns_correct_result_when_file_is_ok()
        {
            string path = "C:\\SFTP\\2019\\March\\validfilex.xlsx";
            DirectoryInfo dirInfo = new DirectoryInfo(path);
            var process = new IPPISFileProcessor();
            //set unit of work
            process.UoW = Substitute.For<IUoW>();
            process.UoW.BeginTransaction().Returns(process.UoW);

            int h = 1;
            process.UoW.When(x => x.Commit()).Do(x => h++);

            process.BatchDAO = Substitute.For<IIPPISBatchDAOManager>();
            IPPISBatch somethingValue = null;
            process.BatchDAO.GetRecordForMonthAndYear(3, 2019).Returns(somethingValue);

            process.PayeeAdapter = Substitute.For<IIPPISPayeeAdapter>();
            var payeelist = new System.Collections.Concurrent.ConcurrentStack<IPPISAssessmentLineRecordModel> { };
            payeelist.Push(new IPPISAssessmentLineRecordModel { });

            process.PayeeAdapter.GetPayeeResponseModels<IPPISPayeeResponse>(path, null, null, 3, 2019).Returns(new IPPISPayeeResponse { HeaderValidateObject = new CBS.Payee.HeaderValidateObject { Error = false, }, Payees = payeelist });

            process.BatchRecordsDAO = Substitute.For<IIPPISBatchRecordsDAOManager>();
            int count = payeelist.Count;
            process.BatchRecordsDAO.SaveIPPISRecords(0, payeelist).Returns(count);
            var serviceProps = new FileServiceHelper { TenantName = "Nasarawa", StateId = 2, UnknownTaxPayerCodeId = 63 };

            var actualResponse = process.ValidateTheFile(serviceProps, path);

            Assert.AreEqual(false, actualResponse.ErrorOccurred);
            Assert.AreEqual(0, actualResponse.BatchId);
        }


        [Test]
        public void returns_correct_result_when_file_is_ok_and_there_was_an_already_existing_batch_record_that_has_not_started_processing()
        {
            string path = "C:\\SFTP\\2019\\March\\validfilex.xlsx";
            DirectoryInfo dirInfo = new DirectoryInfo(path);
            var process = new IPPISFileProcessor();
            //set unit of work
            process.UoW = Substitute.For<IUoW>();
            process.UoW.BeginTransaction().Returns(process.UoW);

            int h = 1;
            process.UoW.When(x => x.Commit()).Do(x => h++);

            process.BatchDAO = Substitute.For<IIPPISBatchDAOManager>();
            IPPISBatch somethingValue = new IPPISBatch { Id = 90 };
            process.BatchDAO.GetRecordForMonthAndYear(3, 2019).Returns(somethingValue);

            process.PayeeAdapter = Substitute.For<IIPPISPayeeAdapter>();
            var payeelist = new System.Collections.Concurrent.ConcurrentStack<IPPISAssessmentLineRecordModel> { };
            payeelist.Push(new IPPISAssessmentLineRecordModel { });

            process.PayeeAdapter.GetPayeeResponseModels<IPPISPayeeResponse>(path, null, null, 3, 2019).Returns(new IPPISPayeeResponse { HeaderValidateObject = new CBS.Payee.HeaderValidateObject { Error = false, }, Payees = payeelist });

            process.BatchRecordsDAO = Substitute.For<IIPPISBatchRecordsDAOManager>();
            int count = payeelist.Count;
            process.BatchRecordsDAO.SaveIPPISRecords(somethingValue.Id, payeelist).Returns(count);
            var serviceProps = new FileServiceHelper { TenantName = "Nasarawa", StateId = 2, UnknownTaxPayerCodeId = 63 };

            var actualResponse = process.ValidateTheFile(serviceProps, path);

            Assert.AreEqual(false, actualResponse.ErrorOccurred);
            Assert.AreEqual(somethingValue.Id, actualResponse.BatchId);
        }
    }
}
