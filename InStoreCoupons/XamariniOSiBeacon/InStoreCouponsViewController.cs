using System;
using System.Drawing;
using CoreBluetooth;
using CoreFoundation;
using CoreGraphics;
using CoreImage;
using CoreLocation;
using Foundation;
using MultipeerConnectivity;
using UIKit;

namespace XamariniOSiBeacon
{
	public partial class InStoreCouponsViewController : UIViewController
	{
		static readonly string storeId = "ice";
		static readonly string uuid = "B9407F30-F5F8-466E-AFF9-25556B57FE6D";
		static readonly ushort major = 31892;
		static readonly ushort minor = 65114;

		Customer customer;
		CLLocationManager locationMgr;
		CLBeaconRegion beaconRegion;
		bool isAdventureInProgress = false;

		public InStoreCouponsViewController() : base("InStoreCouponsViewController", null)
		{
			View.BackgroundColor = UIColor.White;
			customer = new Customer { FirstName = "Rupert", LastName = "Smith", Id = 1 };
		}
		#region App LifeCycle
		public override void ViewDidLoad()
		{
			base.ViewDidLoad();

			var storeUUID = new NSUuid(uuid);

			// Use this to get information only of one specific iBeacon
			// beaconRegion = new CLBeaconRegion(storeUUID, major, minor, storeId)
			beaconRegion = new CLBeaconRegion(storeUUID, storeId)
			{
				NotifyEntryStateOnDisplay = true,
				NotifyOnEntry = true,
				NotifyOnExit = true
			};
			isAdventureInProgress = true;
			locationMgr = new CLLocationManager();
			locationMgr.RequestWhenInUseAuthorization();
			locationMgr.DidDetermineState += LocationMgr_DidDetermineState;
			locationMgr.DidRangeBeacons += LocationMgr_DidRangeBeacons;

			StartBeaconMonitoring();
		}

		public override void ViewDidAppear(bool animated)
		{
			base.ViewDidAppear(animated);
		}
		#endregion App LifeCycle

		#region Private Methods
		void StartBeaconMonitoring()
		{
			var isAvailable = CLLocationManager.IsMonitoringAvailable(typeof(CLRegion));

			if (CLLocationManager.IsRangingAvailable && isAvailable)
			{
				locationMgr.StartRangingBeacons(beaconRegion);
				locationMgr.StartMonitoring(beaconRegion);
			}
			Console.WriteLine("CLLocationManager.IsRangingAvailable && CLLocationManager.IsMonitoringAvailable(typeof(CLRegion)");
			Console.WriteLine(CLLocationManager.IsRangingAvailable.ToString() + " & " + CLLocationManager.IsMonitoringAvailable(typeof(CLRegion)));
		}
		#endregion Private Methods

		#region Event Handlers
		void LocationMgr_DidDetermineState(object sender, CLRegionStateDeterminedEventArgs e)
		{
			if (e.State == CLRegionState.Inside)
			{
				var notification = new UILocalNotification { AlertBody = String.Format("Welcome back {0}", customer.FirstName) };
				UIApplication.SharedApplication.PresentLocalNotificationNow(notification);
				isAdventureInProgress = !isAdventureInProgress;
				Console.WriteLine("CLRegionState.Inside");
			}
			Console.WriteLine("CLRegionState.NotInside");
		}

		void LocationMgr_DidRangeBeacons(object sender, CLRegionBeaconsRangedEventArgs e)
		{
			foreach(var beacon in e.Beacons)
			{
				Console.WriteLine("BEACON " + beacon.DebugDescription + beacon.Major + beacon.ProximityUuid);
			}

			// The next 20 lines were added purely for testing purposes and can be deleted!
			if (e.Beacons[0] != null)
			{
				switch (e.Beacons[0].Proximity)
				{
					case CLProximity.Far:
						View.BackgroundColor = UIColor.Black;
						break;
					case CLProximity.Near:
						View.BackgroundColor = UIColor.Gray;
						break;
					case CLProximity.Immediate:
						View.BackgroundColor = UIColor.Blue;
						break;
					case CLProximity.Unknown:
						View.BackgroundColor = UIColor.Yellow;
						break;
				}
			}
			else
				View.BackgroundColor = UIColor.White;

			Console.WriteLine("LocationMgr_DidRangeBeacons function");
		}
  		#endregion Event Handlers
	}
}

