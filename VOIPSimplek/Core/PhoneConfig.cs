using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sipek.Common;

namespace VOIPSimplek.Core
{
    internal class PhoneConfig : IConfiguratorInterface
    {
        public bool DNDFlag { get; set; }
        public bool AAFlag { get; set; }
        public bool CFUFlag { get { return false; } set => throw new NotImplementedException(); }
        public string CFUNumber { get ; set; }
        public bool CFNRFlag { get { return false; } set => throw new NotImplementedException(); }
        public string CFNRNumber { get; set ; }
        public bool CFBFlag { get ; set ; }
        public string CFBNumber { get; set; }
        public int SIPPort { get => 5060; set => throw new NotImplementedException(); }

        public int DefaultAccountIndex => 0;

        public List<String> CodecList
        {
            get;
            //{
            //    //List<String> slCodecs = new List<String>();
            //    //slCodecs.Add("PCMA");
            //    //return slCodecs;
            //}
            set;// { }

        }
        public bool PublishEnabled { get => false; set => throw new NotImplementedException(); }

        public List<IAccount> Accounts { get { return _acclist; } }

        public bool IsNull => false;

        public IAccount Account => _acclist.FirstOrDefault();

        public void Save()
        {
        }

        List<IAccount> _acclist = new List<IAccount>();
        internal PhoneConfig()
        {
            _acclist.Add(new AccountConfig());
        }
    }
    internal class AccountConfig : IAccount
    {
        int _regState = 0;
        public int Index { get; set; }
        public string AccountName { get => "901"; set => throw new NotImplementedException(); }
        public string HostName { get => "193.93.185.190"; set => throw new NotImplementedException(); }
        public string Id { get => "myId"; set => throw new NotImplementedException(); }
        public string UserName { get => "901"; set => throw new NotImplementedException(); }
        public string Password { get => "9d49c145849b09428e2e03a58477fa2f"; set => throw new NotImplementedException(); }
        public string DisplayName { get => "901"; set => throw new NotImplementedException(); }
        public string DomainName { get => "*"; set => throw new NotImplementedException(); }
        public int RegState { get { return _regState; } set { _regState = value; } }
        public string ProxyAddress { get => ""; set => throw new NotImplementedException(); }
        public ETransportMode TransportMode { get => ETransportMode.TM_UDP; set => throw new NotImplementedException(); }
        public bool Enabled { get => true; set => throw new NotImplementedException(); }
    }
}
