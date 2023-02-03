using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.Cfg;
using System.Data.SqlClient;
using System.Data;
using System.Web;
using System.Runtime.Remoting.Messaging;

namespace Parkway.Tools.NHibernate
{
    internal class TransactionBlock
    {
        private static readonly string TransactionCountKey = ":::TransactionBlock:::TransactionCountKey:::";
        private static readonly string TransactionKey = ":::TransactionBlock:::TransactionKey:::";
        private static readonly string SessionKey = ":::TransactionBlock:::SessionKey:::";

        protected static int TransactionCount
        {
            get
            {
                if (HttpContext.Current != null)
                {
                    if (HttpContext.Current.Items[TransactionCountKey] == null)
                    {
                        HttpContext.Current.Items[TransactionCountKey] = -1;
                    }
                    return (int)HttpContext.Current.Items[TransactionCountKey];
                }
                else
                {
                    if (CallContext.GetData(TransactionCountKey) == null)
                    {
                        CallContext.SetData(TransactionCountKey, -1);
                    }
                    return (int)CallContext.GetData(TransactionCountKey);
                }

            }
            set
            {
                if (HttpContext.Current != null)
                {
                    HttpContext.Current.Items[TransactionCountKey] = value;
                }
                else
                {
                    CallContext.SetData(TransactionCountKey, value);
                }

            }
        }

        protected static ITransaction Transaction
        {
            get
            {
                if (HttpContext.Current != null)
                {
                    return HttpContext.Current.Items[TransactionKey] as ITransaction;
                }
                else
                {
                    return CallContext.GetData(TransactionKey) as ITransaction;
                }
            }
            set
            {
                if (HttpContext.Current != null)
                {
                    HttpContext.Current.Items[TransactionKey] = value;
                }
                else
                {
                    CallContext.SetData(TransactionKey, value);
                }
            }
        }

        internal static ISession Session
        {
            get
            {
                if (HttpContext.Current != null)
                {
                    return HttpContext.Current.Items[SessionKey] as ISession;
                }
                else
                {
                    return CallContext.GetData(SessionKey) as ISession;
                }
            }
            set
            {
                if (HttpContext.Current != null)
                {
                    HttpContext.Current.Items[SessionKey] = value;
                }
                else
                {
                    CallContext.SetData(SessionKey, value);
                }
            }
        }

        public static int BeginTransaction()
        {
            return BeginTransaction(IsolationLevel.Unspecified);
        }

        public static int BeginTransaction(IsolationLevel isolationLevel)
        {
            return BeginTransaction(Session, isolationLevel);
        }

        public static int BeginTransaction(ISession session, IsolationLevel isolationLevel)
        {
            if (TransactionCount < 0)
            {
                Session = session;
                if (isolationLevel == IsolationLevel.Unspecified)
                    Transaction = session.BeginTransaction();
                else
                    Transaction = session.BeginTransaction(isolationLevel);
            }
            return ++TransactionCount;
        }

        public static int CommitTransaction()
        {
            if (TransactionCount < 0)
            {
                throw new Exception("No transaction was started");
                //throw new NoTransactionStartedException();
            }
            else if (TransactionCount == 0)
            {
                //Commit Transaction
                Transaction.Commit();
            }

            return --TransactionCount;
        }

        public static int RollbackTransaction()
        {
            if (TransactionCount < 0)
            {
                throw new Exception("No transaction was started");
                //throw new NoTransactionStartedException();
            }
            else// if (TransactionCount == 0)
                //{
                //Rollback Transaction
                Transaction.Rollback();
            //}
            return --TransactionCount;
        }
    }
}