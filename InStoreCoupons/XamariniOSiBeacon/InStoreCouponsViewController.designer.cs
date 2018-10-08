// WARNING
//
// This file has been generated automatically by Visual Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace XamariniOSiBeacon
{
	[Register ("InStoreCouponsViewController")]
	partial class InStoreCouponsViewController
	{
		[Outlet]
		UIKit.UIButton Button { get; set; }

		[Outlet]
		UIKit.UIImageView ImageView { get; set; }

		[Outlet]
		UIKit.UILabel Label { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (Label != null) {
				Label.Dispose ();
				Label = null;
			}

			if (ImageView != null) {
				ImageView.Dispose ();
				ImageView = null;
			}

			if (Button != null) {
				Button.Dispose ();
				Button = null;
			}
		}
	}
}
