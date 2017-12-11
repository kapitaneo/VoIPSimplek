using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Sipek.Common.CallControl;
using Sipek.Sip;
using VOIPSimplek.Core;
using Sipek.Common;
using System.Media;
using System.Timers;

namespace VOIPSimplek
{
    /// <summary>
    /// 
    /// </summary>
    public partial class MainWindow : Window
    {
        string cs_Phone, cs_RegState, cs_CallState;
        public string PhoneNumber { get; set; }
        int IsAnswer = 0;
        CCallManager CallManager
        {
            get { return CCallManager.Instance; }
        }

        private IStateMachine v_hCall = null;
        private IStateMachine v_hIncomingCall = null;

        PhoneConfig Config = new PhoneConfig();
        private int currentSession;

        public MainWindow()
        {
            InitializeComponent();

            CallManager.CallStateRefresh += new DCallStateRefresh(CallManager_CallStateRefresh);
            CallManager.IncomingCallNotification += CallManager_IncomingCallNotification1;
            //CallManager.IncomingCallNotification += new DIncomingCallNotification(CallManager_IncomingCallNotification);
            //pjsipRegistrar.Instance.AccountStateChanged += new Sipek.Common.DAccountStateChanged(Instance_AccountStateChanged);
            CCallManager.Instance.MediaProxy = new CMediaPlayerProxy();

            CallManager.StackProxy = pjsipStackProxy.Instance;

            ICallProxyInterface.CallStateChanged += ICallProxyInterface_CallStateChanged;
            pjsipStackProxy.Instance.createCallProxy().holdCall();

            CallManager.Config = Config;
            pjsipStackProxy.Instance.Config = Config;
            pjsipRegistrar.Instance.Config = Config;

            CallManager.Initialize();

            pjsipPresenceAndMessaging.Instance.MessageReceived += Instance_MessageReceived;
            pjsipPresenceAndMessaging.Instance.BuddyStatusChanged += Instance_BuddyStatusChanged;
            var result = pjsipRegistrar.Instance.registerAccounts();
            var dssdf  = Sipek.Sip.SipConfigStruct.Instance;
            // Visibility = Visibility.Hidden;
            
        }

        private void ICallProxyInterface_CallStateChanged(int callId, ESessionState callState, string info)
        {
            // throw new NotImplementedException();
            
            var calls1 = CallManager.CallList;
            
            if (IsAnswer == 1)
            {

                IsAnswer++;
            }
            else if (IsAnswer == 2)
            {
                var calls = CallManager.CallList;
                calls1[1].Destroy();
                IsAnswer = 0;
            }


        }

        private void Instance_BuddyStatusChanged(int buddyId, int status, string text)
        {
            throw new NotImplementedException();
        }

        private void Instance_MessageReceived(string from, string text)
        {
            throw new NotImplementedException();
        }

        private void CallManager_IncomingCallNotification1(int sessionId, string number, string info, IEnumerable<KeyValuePair<string, string>> headers)
        {
            Dispatcher.Invoke(new DIncomingCallNotification(OnIncomingCallNotification), new Object[] { sessionId, number, number, headers });
        }

        //private void CallManager_IncomingCallNotification(int iSessionId, string szNumber, string szInfo)
        //{
        //    //if (Dispatcher.CheckAccess())
        //    //     Dispatcher.Invoke(new DIncomingCallNotification(OnIncomingCallNotification), new Object[] { iSessionId, szNumber, szInfo,  });
        //    //else
        //    //    OnIncomingCallNotification(iSessionId, szNumber, szInfo);
        //}

        private void Instance_AccountStateChanged(int iAccountId, int iAccState)
        {
            //if (Dispatcher.CheckAccess())
            //    Dispatcher.Invoke(new DAccountStateChanged(OnRegistrationUpdate), new Object[] { iAccountId, iAccState });
            //else
            //    OnRegistrationUpdate(iAccountId, iAccState);
        }

        private void CallManager_CallStateRefresh(int sessionId)
        {
            if (!Dispatcher.CheckAccess())
                Dispatcher.Invoke(new DCallStateRefresh(OnStateUpdate), new object[] { sessionId });
            else
                OnStateUpdate(sessionId);
        }


        #region synchronized callbacks   b835bf744efb4134b5b2ca52cbbc6459
        private void OnRegistrationUpdate(Int32 iAccountId, Int32 iAccState)
        {
            cs_RegState = iAccState.ToString();
        }

        private void OnStateUpdate(Int32 iSessionId)
        {
            //cs_CallState = CallManager.getCall(iSessionId).StateId.ToString();
            //if (cs_CallState == "ACTIVE")
            //{
            //    pjsipPresenceAndMessaging.Instance.setStatus(EUserStatus.BUSY);
            //}
        }

        private void OnIncomingCallNotification(Int32 iSessionId, String szNumber, String szInfo, IEnumerable<KeyValuePair<string, string>> headers)
        {
            v_hIncomingCall = CallManager.getCall(iSessionId);
            currentSession = iSessionId;
            var list = CallManager.enumCallsInState(EStateId.INCOMING);
            PhoneNumber = szNumber;
            cs_CallState = v_hIncomingCall.StateId.ToString();
        }
        #endregion

        private void cs_MakeCall_Click(Object hSender, EventArgs hArgs)
        {
            v_hCall = CallManager.createOutboundCall(cs_Phone);
        }

        private void cs_Release_Click(Object hSender, EventArgs hArgs)
        {
            cs_Phone= "";
            CallManager.onUserRelease(v_hCall.Session);

        }

        private void cs_Answer_Click(object sender, RoutedEventArgs e)
        {
            CallManager.onUserAnswer(v_hIncomingCall.Session);
            IsAnswer = 1;
            //var list = CallManager.enumCallsInState(EStateId.ACTIVE);
            // var list = CallManager.CallList;
        }

        private void cs_Answer_Click(Object hSender, EventArgs hArgs)
        {
            
        }

        private void button1_Click(Object hSender, EventArgs hArgs)
        {
            CallManager.onUserTransfer(v_hIncomingCall.Session, "901");
        }
    }
    public class CMediaPlayerProxy : IMediaProxyInterface
    {
        SoundPlayer player = new SoundPlayer();

        public int playTone(ETones toneId)
        {
            string fname;

            switch (toneId)
            {
                case ETones.EToneDial:
                    fname = "Sounds/dial.wav";
                    break;
                case ETones.EToneCongestion:
                    fname = "Sounds/congestion.wav";
                    break;
                case ETones.EToneRingback:
                    fname = "Sounds/ringback.wav";
                    break;
                case ETones.EToneRing:
                    fname = "Sounds/ring.wav";
                    break;
                default:
                    fname = "";
                    break;
            }

            player.SoundLocation = fname;
            player.Load();
            player.PlayLooping();

            return 1;
        }

        public int stopTone()
        {
            player.Stop();
            return 1;
        }
    }
    //public class GUITimer : ITimer
    //{
    //    Timer _guiTimer;
    //    MainWindow _form;

    //    public GUITimer(MainWindow mf)
    //    {
    //        _form = mf;
    //        _guiTimer = new Timer();
    //        if (this.Interval > 0) _guiTimer.Interval = this.Interval;
    //        _guiTimer.Interval = 100;
    //        _guiTimer.Enabled = true;
    //        _guiTimer.Elapsed += new ElapsedEventHandler(_guiTimer_Tick);
    //    }

    //    void _guiTimer_Tick(object sender, EventArgs e)
    //    {
    //        _guiTimer.Stop();
    //        //_elapsed(sender, e);
    //        // Synchronize thread with GUI because SIP stack works with GUI thread only
    //        if (!_form.Disposing)
    //            _form.Invoke(_elapsed, new object[] { sender, e });
    //    }

    //    public bool Start()
    //    {
    //        _guiTimer.Start();
    //        return true;
    //    }

    //    public bool Stop()
    //    {
    //        _guiTimer.Stop();
    //        return true;
    //    }

    //    private int _interval;
    //    public int Interval
    //    {
    //        get { return _interval; }
    //        set { _interval = value; _guiTimer.Interval = value; }
    //    }

    //    private TimerExpiredCallback _elapsed;
    //    public TimerExpiredCallback Elapsed
    //    {
    //        set
    //        {
    //            _elapsed = value;
    //        }
    //    }
    //}
}
