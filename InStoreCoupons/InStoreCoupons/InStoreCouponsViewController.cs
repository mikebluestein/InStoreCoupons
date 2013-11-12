using System;
using System.Drawing;
using MonoTouch.AVFoundation;
using MonoTouch.CoreBluetooth;
using MonoTouch.CoreFoundation;
using MonoTouch.CoreLocation;
using MonoTouch.Foundation;
using MonoTouch.MultipeerConnectivity;
using MonoTouch.UIKit;
using MonoTouch.CoreImage;
using MonoTouch.CoreGraphics;

namespace InStoreCoupons
{
    public partial class InStoreCouponsViewController : UIViewController
    {
        static readonly string storeId = "BigStore42";
        static readonly string serviceType = "InStoreCoupons";
        static readonly string uuid = "85A622A1-C5FE-4E75-ACF7-013656D418A7";

        static bool UserInterfaceIdiomIsPhone {
            get { return UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Phone; }
        }

        Customer customer;
        CBPeripheralManager peripheralMgr;
        BTPeripheralDelegate peripheralDelegate;
        CLLocationManager locationMgr;
        MCSession session;
        MCPeerID peer;
        MCBrowserViewController browser;
        MCAdvertiserAssistant assistant;
        MySessionDelegate sessionDel;
        MyBrowserDelegate browserDel;
        NSDictionary dict;

        public InStoreCouponsViewController () : base (UserInterfaceIdiomIsPhone ? "InStoreCouponsViewController_iPhone" : "InStoreCouponsViewController_iPad", null)
        {
            View.BackgroundColor = UIColor.White;
            customer = new Customer { FirstName = "Rupert", LastName = "Smith", Id = 1 };

            sessionDel = new MySessionDelegate (this);
            browserDel = new MyBrowserDelegate ();
            dict = new NSDictionary ();
        }

        public override void ViewDidLoad ()
        {
            base.ViewDidLoad ();

            var storeUUID = new NSUuid (uuid);
            var beaconRegion = new CLBeaconRegion (storeUUID, storeId) {
                NotifyEntryStateOnDisplay = true,
                NotifyOnEntry = true,
                NotifyOnExit = true
            };

            if (!UserInterfaceIdiomIsPhone) {

                NSMutableDictionary peripheralData = beaconRegion.GetPeripheralData (new NSNumber (-59));
                peripheralDelegate = new BTPeripheralDelegate ();
                peripheralMgr = new CBPeripheralManager (peripheralDelegate, DispatchQueue.DefaultGlobalQueue);
                peripheralMgr.StartAdvertising (peripheralData);

            } else {

                locationMgr = new CLLocationManager ();

                locationMgr.RegionEntered += (object sender, CLRegionEventArgs e) => {
                    if (e.Region.Identifier == storeId) {
                        WelcomeBackCustomer ();
                    }
                };

                locationMgr.DidDetermineState += (object sender, CLRegionStateDeterminedEventArgs e) => {
                    if(e.State == CLRegionState.Inside){
                        if(!DealofDay.Enabled){
                            WelcomeBackCustomer ();
                        }
                    }
                };

                locationMgr.RegionLeft += (object sender, CLRegionEventArgs e) => {
                    DealofDay.Enabled = false;
                };

                locationMgr.StartMonitoring (beaconRegion);

                DealofDay.TouchUpInside += (sender, e) => {

                    StartMultipeerAdvertiser ();
                };
            }
        }

        public override void ViewDidAppear (bool animated)
        {
            base.ViewDidAppear (animated);

            if (!UserInterfaceIdiomIsPhone) {
                StartMultipeerBrowser ();
            }
        }

        void WelcomeBackCustomer ()
        {
            var notification = new UILocalNotification { AlertBody = String.Format ("Welcome back {0}", customer.FirstName) };
            UIApplication.SharedApplication.PresentLocationNotificationNow (notification);

            DealofDay.Enabled = true;
        }

        void StartMultipeerAdvertiser ()
        {
            peer = new MCPeerID (customer.ToString ());

            session = new MCSession (peer) {
                Delegate = sessionDel
            };

            assistant = new MCAdvertiserAssistant (serviceType, dict, session); 
            assistant.Start ();
        }

        void StartMultipeerBrowser ()
        {
            peer = new MCPeerID (storeId);

            session = new MCSession (peer) {
                Delegate = sessionDel
            };

            browser = new MCBrowserViewController (serviceType, session) {
                Delegate = browserDel,
                ModalPresentationStyle = UIModalPresentationStyle.None
            };

            PresentViewController (browser, true, null);	
        }

        void SendCoupon (string message, MCPeerID peer)
        {
            NSError error;

            session.SendData (
                NSData.FromString (message), 
                new MCPeerID[]{peer},
                MCSessionSendDataMode.Reliable,
                out error);
        }

        void CreateCouponImage (string text)
        {
            CouponLabel.Text = text;

            var qrCode = new CIQRCodeGenerator {
                Message = NSData.FromString (text),
                CorrectionLevel = "Q"
            }.OutputImage;

            var ctx = CIContext.FromOptions (null);
            CouponView.Image = UIImage.FromImage (ctx.CreateCGImage (qrCode, qrCode.Extent));
        }

        class MySessionDelegate : MCSessionDelegate
        {
            InStoreCouponsViewController vc;

            public MySessionDelegate (InStoreCouponsViewController vc)
            {
                this.vc = vc;
            }

            public override void DidChangeState (MCSession session, MCPeerID peerID, MCSessionState state)
            {
                switch (state) {
                case MCSessionState.Connected:
                    if (peerID.DisplayName != storeId) {
                        vc.SendCoupon ("buy 1 widget, get 1 free", peerID);
                    }
                    break;
                case MCSessionState.Connecting:
                case MCSessionState.NotConnected:
                    break;
                }
            }

            public override void DidReceiveData (MCSession session, NSData data, MCPeerID peerID)
            {
                InvokeOnMainThread (() => {
                    string s = data.ToString();
                    vc.CreateCouponImage (s);
                });
            }

            public override void DidStartReceivingResource (MCSession session, string resourceName, MCPeerID fromPeer, NSProgress progress)
            {
            }

            public override void DidFinishReceivingResource (MCSession session, string resourceName, MCPeerID formPeer, NSUrl localUrl, out NSError error)
            {
                error = null;
            }

            public override void DidReceiveStream (MCSession session, NSInputStream stream, string streamName, MCPeerID peerID)
            {
            }
        }

        class MyBrowserDelegate : MCBrowserViewControllerDelegate
        {
            public override void DidFinish (MCBrowserViewController browserViewController)
            {
                InvokeOnMainThread (() => {
                    browserViewController.DismissViewController (true, null);
                });
            }

            public override void WasCancelled (MCBrowserViewController browserViewController)
            {
                InvokeOnMainThread (() => {
                    browserViewController.DismissViewController (true, null);
                });
            }
        }

        class BTPeripheralDelegate : CBPeripheralManagerDelegate
        {
            public override void StateUpdated (CBPeripheralManager peripheral)
            {
                if (peripheral.State == CBPeripheralManagerState.PoweredOn) {
                    Console.WriteLine ("powered on");
                }
            }
        }
       
    }
}

