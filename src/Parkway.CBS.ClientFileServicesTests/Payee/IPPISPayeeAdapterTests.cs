using NUnit.Framework;
using Parkway.CBS.Payee.PayeeAdapters.IPPIS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parkway.CBS.ClientFileServicesTests.Payee
{
    [TestFixture]
    public class IPPISPayeeAdapterTests
    {
        [Test]
        public void assert_exception_if_file_404()
        {
            var adapter = new IPPISPayeeAdapter();
            string filePath = "C:\\Parkway\\Repository\\CBS\\src\\Parkway.CBS.ClientFileServicesTests\\testfiles\\nosuchfile.xlsx";
            int month = 1;
            int year = 2019;
            Assert.Throws(typeof(System.IO.FileNotFoundException), () => adapter.GetPayeeResponseModels<IPPISPayeeResponse>(filePath, null, null, month, year));
        }


        [Test]
        public void test_that_payee_response_returns_an_invalid_header_obj_for_org_code()
        {
            var adapter = new IPPISPayeeAdapter();
            string filePath = "C:\\Parkway\\Repository\\CBS\\src\\Parkway.CBS.ClientFileServicesTests\\testfiles\\orgcodemissingheaderfile.xlsx";
            int month = 1;
            int year = 2019;
            IPPISPayeeResponse payeResponse = adapter.GetPayeeResponseModels<IPPISPayeeResponse>(filePath, null, null, month, year);

            string messageForMissingOrgCode = "ORG_CODE header not found".ToLower();
            //contactaddressandphonemissingheader
            Assert.IsTrue(payeResponse.HeaderValidateObject.Error);
            Assert.AreEqual(payeResponse.HeaderValidateObject.ErrorMessage, messageForMissingOrgCode);
        }


        [Test]
        public void test_that_payee_response_returns_an_invalid_header_obj_for_contact_address_and_phone()
        {
            var adapter = new IPPISPayeeAdapter();
            string filePath = "C:\\Parkway\\Repository\\CBS\\src\\Parkway.CBS.ClientFileServicesTests\\testfiles\\contactaddressandphonemissingheader.xlsx";
            int month = 1;
            int year = 2019;
            IPPISPayeeResponse payeResponse = adapter.GetPayeeResponseModels<IPPISPayeeResponse>(filePath, null, null, month, year);

            string message = "CONTACT_ADDRESS header not found".ToLower() + "\nMOBILE_PHONE header not found".ToLower();
            Assert.IsTrue(payeResponse.HeaderValidateObject.Error);
            Assert.AreEqual(payeResponse.HeaderValidateObject.ErrorMessage, message);
        }


        [Test]
        public void test_that_payee_response_returns_an_invalid_header_obj_for_employee_number()
        {
            var adapter = new IPPISPayeeAdapter();
            string filePath = "C:\\Parkway\\Repository\\CBS\\src\\Parkway.CBS.ClientFileServicesTests\\testfiles\\employeenumbermissingheader.xlsx";
            int month = 1;
            int year = 2019;
            IPPISPayeeResponse payeResponse = adapter.GetPayeeResponseModels<IPPISPayeeResponse>(filePath, null, null, month, year);

            string message = "EMPLOYEE_NUMBER header not found".ToLower();
            //contactaddressandphonemissingheader
            Assert.IsTrue(payeResponse.HeaderValidateObject.Error);
            Assert.AreEqual(payeResponse.HeaderValidateObject.ErrorMessage, message);
        }


        [Test]
        public void test_that_payee_records_validation_for_allow_empty_ministry_name()
        {
            var adapter = new IPPISPayeeAdapter();
            string filePath = "C:\\Parkway\\Repository\\CBS\\src\\Parkway.CBS.ClientFileServicesTests\\testfiles\\badministryentry.xlsx";
            int month = 1;
            int year = 2019;
            IPPISPayeeResponse payeResponse = adapter.GetPayeeResponseModels<IPPISPayeeResponse>(filePath, null, null, month, year);

            Assert.IsFalse(payeResponse.HeaderValidateObject.Error);
            IPPISAssessmentLineRecordModel singleValue = null;

            Assert.Throws(typeof(InvalidOperationException), () => singleValue = payeResponse.Payees.Single(x => x.HasError == true));
            Assert.AreEqual(singleValue, null);
        }


        [Test]
        public void test_that_payee_records_validation_for_ignore_error_flag_on_large_ministry_name()
        {
            var adapter = new IPPISPayeeAdapter();
            string filePath = "C:\\Parkway\\Repository\\CBS\\src\\Parkway.CBS.ClientFileServicesTests\\testfiles\\largeministryentry.xlsx";
            int month = 1;
            int year = 2019;
            IPPISPayeeResponse payeResponse = adapter.GetPayeeResponseModels<IPPISPayeeResponse>(filePath, null, null, month, year);

            Assert.IsFalse(payeResponse.HeaderValidateObject.Error);
            IPPISAssessmentLineRecordModel singleValue = null;

            Assert.Throws(typeof(InvalidOperationException), () => singleValue = payeResponse.Payees.Single(x => x.HasError == true));
            Assert.AreEqual(singleValue, null);
        }


        [Test]
        public void test_that_payee_records_validation_for_org_code_that_do_not_allow_empty_checking_for_just_1_field()
        {
            var adapter = new IPPISPayeeAdapter();
            string filePath = "C:\\Parkway\\Repository\\CBS\\src\\Parkway.CBS.ClientFileServicesTests\\testfiles\\emptyorgcodeentry.xlsx";
            int month = 1;
            int year = 2019;
            IPPISPayeeResponse payeResponse = adapter.GetPayeeResponseModels<IPPISPayeeResponse>(filePath, null, null, month, year);

            string expectMsg = "ORG_CODE is empty.".ToLower();

            Assert.IsFalse(payeResponse.HeaderValidateObject.Error);
            IPPISAssessmentLineRecordModel singleValue = null;

            singleValue = payeResponse.Payees.Single(x => x.HasError == true);
            Assert.IsTrue(singleValue.HasError);
            Assert.AreEqual(expectMsg, singleValue.ErrorMessages.ToLower().Trim());
        }

        [Test]
        public void test_that_payee_records_validation_for_employee_number_that_do_not_allow_empty_checking_for_just_1_field()
        {
            var adapter = new IPPISPayeeAdapter();
            string filePath = "C:\\Parkway\\Repository\\CBS\\src\\Parkway.CBS.ClientFileServicesTests\\testfiles\\emptyemployeenumberentry.xlsx";
            int month = 1;
            int year = 2019;
            IPPISPayeeResponse payeResponse = adapter.GetPayeeResponseModels<IPPISPayeeResponse>(filePath, null, null, month, year);

            string expectMsg = "EMPLOYEE_NUMBER is empty.".ToLower();

            Assert.IsFalse(payeResponse.HeaderValidateObject.Error);
            IPPISAssessmentLineRecordModel singleValue = null;

            singleValue = payeResponse.Payees.Single(x => x.HasError == true);
            Assert.IsTrue(singleValue.HasError);
            Assert.AreEqual(expectMsg, singleValue.ErrorMessages.ToLower().Trim());
        }


        [Test]
        public void test_that_payee_records_validation_for_org_code_max_characters_is_checked()
        {
            var adapter = new IPPISPayeeAdapter();
            string filePath = "C:\\Parkway\\Repository\\CBS\\src\\Parkway.CBS.ClientFileServicesTests\\testfiles\\maxorgcodeentry.xlsx";
            int month = 1;
            int year = 2019;
            IPPISPayeeResponse payeResponse = adapter.GetPayeeResponseModels<IPPISPayeeResponse>(filePath, null, null, month, year);

            string expectMsg = "ORG_CODE value is too long. Enter a valid ORG_CODE. Maximum number of characters is 250".ToLower();

            Assert.IsFalse(payeResponse.HeaderValidateObject.Error);
            IPPISAssessmentLineRecordModel singleValue = null;

            singleValue = payeResponse.Payees.Single(x => x.HasError == true);
            Assert.IsTrue(singleValue.HasError);
            Assert.AreEqual(expectMsg, singleValue.ErrorMessages.ToLower().Trim());
        }


        [Test]
        public void test_that_payee_records_validation_for_employee_number_max_characters_is_checked()
        {
            var adapter = new IPPISPayeeAdapter();
            string filePath = "C:\\Parkway\\Repository\\CBS\\src\\Parkway.CBS.ClientFileServicesTests\\testfiles\\maxemployeenumberentry.xlsx";
            int month = 1;
            int year = 2019;
            IPPISPayeeResponse payeResponse = adapter.GetPayeeResponseModels<IPPISPayeeResponse>(filePath, null, null, month, year);

            string expectMsg = "EMPLOYEE_NUMBER value is too long. Enter a valid EMPLOYEE_NUMBER. Maximum number of characters is 100".ToLower();

            Assert.IsFalse(payeResponse.HeaderValidateObject.Error);
            IPPISAssessmentLineRecordModel singleValue = null;

            singleValue = payeResponse.Payees.Single(x => x.HasError == true);
            Assert.IsTrue(singleValue.HasError);
            Assert.AreEqual(expectMsg, singleValue.ErrorMessages.ToLower().Trim());
        }

        [Test]
        public void phone_number_validation_should_not_return_error_true_if_field_is_empty()
        {
            var adapter = new IPPISPayeeAdapter();
            string filePath = "C:\\Parkway\\Repository\\CBS\\src\\Parkway.CBS.ClientFileServicesTests\\testfiles\\phonenumberemptyentry.xlsx";
            int month = 1;
            int year = 2019;
            IPPISPayeeResponse payeResponse = adapter.GetPayeeResponseModels<IPPISPayeeResponse>(filePath, null, null, month, year);

            Assert.IsFalse(payeResponse.HeaderValidateObject.Error);
            IPPISAssessmentLineRecordModel singleValue = null;

            Assert.Throws(typeof(InvalidOperationException), () => singleValue = payeResponse.Payees.Single(x => x.HasError == true));
            Assert.AreEqual(singleValue, null);
        }

        [Test]
        public void check_that_when_an_empty_tax_amount_is_added_the_row_has_error()
        {
            var adapter = new IPPISPayeeAdapter();
            string filePath = "C:\\Parkway\\Repository\\CBS\\src\\Parkway.CBS.ClientFileServicesTests\\testfiles\\taxamountemptyentry.xlsx";
            int month = 1;
            int year = 2019;
            IPPISPayeeResponse payeResponse = adapter.GetPayeeResponseModels<IPPISPayeeResponse>(filePath, null, null, month, year);

            string expectMsg = "TAX is empty.".ToLower();

            Assert.IsFalse(payeResponse.HeaderValidateObject.Error);
            IPPISAssessmentLineRecordModel singleValue = null;

            singleValue = payeResponse.Payees.Single(x => x.HasError == true);
            Assert.IsTrue(singleValue.HasError);
            Assert.AreEqual(expectMsg, singleValue.ErrorMessages.ToLower().Trim());
        }

        [Test]
        public void check_that_for_non_numeric_tax_amount_has_error()
        {
            var adapter = new IPPISPayeeAdapter();
            string filePath = "C:\\Parkway\\Repository\\CBS\\src\\Parkway.CBS.ClientFileServicesTests\\testfiles\\taxamountmaxedoutentry.xlsx";
            int month = 1;
            int year = 2019;
            IPPISPayeeResponse payeResponse = adapter.GetPayeeResponseModels<IPPISPayeeResponse>(filePath, null, null, month, year);

            string expectMsg = "TAX is not a valid amount.".ToLower();

            Assert.IsFalse(payeResponse.HeaderValidateObject.Error);
            IPPISAssessmentLineRecordModel singleValue = null;

            singleValue = payeResponse.Payees.Single(x => x.HasError == true);
            Assert.IsTrue(singleValue.HasError);
            Assert.AreEqual(expectMsg, singleValue.ErrorMessages.ToLower().Trim());
        }
    }
}
