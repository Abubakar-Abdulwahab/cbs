using System;
using System.Threading;
using System.Threading.Tasks;
using NHibernate;
using NUnit.Framework;
using Parkway.CBS.ClientRepository;
using Parkway.CBS.ClientRepository.Repositories;
using Parkway.CBS.Core.Models;
using Parkway.Tools.NHibernate;

namespace Parkway.CBS.ClientRepositoryTests
{
    [TestFixture]
    public class SessionTests
    {
        [Test]
        public void check_that_when_two_or_more_instances_on_different_threads_are_created_they_have_different_sessions()
        {
            IUoW thread0UoWOutSide = null;
            IUoW thread1UoWOutSide = null;
            SessionManager SessManager0 = null;
            SessionManager SessManager1 = null;
            string sessionId0 = null;
            string sessionId1 = null;
            ISession Sess0, Sess1 = null;
            TenantCBSSettings t0, t1 = null;

            //create two instance of the same process class on different threads
            //Console.WriteLine("Starting unit of work 1");
            //thread0UoW = new UoW("Nasarawa_SessionFactory", "FileServices");
            //Repository<TenantCBSSettings> repo = new Repository<TenantCBSSettings>(thread0UoW);
            //var value = repo.Get(1);
            //string sessionId0 = thread0UoW.Session.GetSessionImplementation().SessionId.ToString();

            var rSessManager0 = Task.Factory.StartNew(() =>
            {
                Console.WriteLine("Running second process in the second unit of work instance");
                //Thread.Sleep(5000);
                Console.WriteLine("Creating a new unit of work instance UoW2");
                IUoW thread0UoW = new UoW("Nasarawa_SessionFactory", "FileServices");
                Repository<TenantCBSSettings> repo1 = new Repository<TenantCBSSettings>(thread0UoW);
                //Thread.Sleep(500);

                var value1 = repo1.Get(1);
                sessionId0 = thread0UoW.Session.GetSessionImplementation().SessionId.ToString();
                Console.WriteLine("");

                Thread.Sleep(5000);
                return value1;
                //return thread0UoW.Session;
                //return thread0UoW;

            }, TaskCreationOptions.LongRunning);
            //}, TaskCreationOptions.LongRunning).ConfigureAwait(false);

            var rSessManager1 = Task.Factory.StartNew(() =>
            {
                Console.WriteLine("Running second process in the second unit of work instance");
                Thread.Sleep(2000);
                Thread.Sleep(2000);
                Console.WriteLine("Creating a new unit of work instance UoW2");
                IUoW thread1UoW = new UoW("Nasarawa_SessionFactory", "FileServices");
                Repository<TenantCBSSettings> repo1 = new Repository<TenantCBSSettings>(thread1UoW);
                var value1 = repo1.Get(1);
                sessionId1 = thread1UoW.Session.GetSessionImplementation().SessionId.ToString();
                Console.WriteLine("");
                //Thread.Sleep(1000);
                return value1;
                //return thread1UoW.Session;

            }, TaskCreationOptions.LongRunning);

            Thread.Sleep(5000);
            t0 = rSessManager1.Result;
            t1 = rSessManager0.Result;
            //Sess0 = rSessManager1.Result;
            //Sess1 = rSessManager0.Result;

            //Assert.AreNotEqual(sessionId0, sessionId1);
            
            Assert.AreNotEqual(t0, t1);
            //Assert.AreNotEqual(Sess0, Sess1);
            //Assert.AreNotEqual(SessManager0, SessManager1);
        }

        [Test]
        public void do_a_valid_update_operation()
        {
            string oldValue = null;
            IUoW uow = new UoW("Nasarawa_SessionFactory", "FileServices");
            uow.BeginTransaction();
            Repository<TenantCBSSettings> repo1 = new Repository<TenantCBSSettings>(uow);

            var value1 = repo1.Get(1);
            oldValue = value1.Identifier;
            value1.Identifier = "ChangedIdentifier";
            //AIRS_ABIA
            uow.Commit();
            //we have changed the identifier
            //lets assert that the Identifier has changed
            uow.BeginTransaction();
            var revertObj = repo1.Get(1);

            Assert.AreEqual(revertObj.Identifier, "ChangedIdentifier");
            //set the old value back
            revertObj.Identifier = oldValue;
            uow.Commit();
            uow.Dispose();
        }

        [Test]
        public void test_that_when_transaction_is_inactive_a_commit_cannot_be_made()
        {
            bool threwException = false;
            bool valueHere = false;
            try
            {
                string oldValue = null;
                IUoW uow = new UoW("Nasarawa_SessionFactory", "FileServices");
                uow.BeginTransaction();
                //dispose of the transaction value
                uow.Rollback();
                Repository<TenantCBSSettings> repo1 = new Repository<TenantCBSSettings>(uow);

                var value1 = repo1.Get(1);
                oldValue = value1.Identifier;
                value1.Identifier = "ChangedIdentifier";
                //AIRS_ABIA
                valueHere = true;
                uow.Commit();
                uow.Dispose();
            }
            catch (Exception exception)
            { threwException = true; }

            Assert.IsTrue(threwException);
            Assert.IsTrue(valueHere);
        }

        [Test]
        public void commit_without_an_active_null_transaction()
        {
            bool threwException = false;
            bool valueHere = false;
            try
            {
                string oldValue = null;
                IUoW uow = new UoW("Nasarawa_SessionFactory", "FileServices");
                //dispose of the transaction value
                //uow.Rollback();
                Repository<TenantCBSSettings> repo1 = new Repository<TenantCBSSettings>(uow);

                var value1 = repo1.Get(1);
                oldValue = value1.Identifier;
                value1.Identifier = "ChangedIdentifier";
                //AIRS_ABIA
                valueHere = true;
                uow.Commit();
                uow.Dispose();
            }
            catch (Exception exception)
            { threwException = true; }

            Assert.IsTrue(threwException);
            Assert.IsTrue(valueHere);
        }
    }
}
