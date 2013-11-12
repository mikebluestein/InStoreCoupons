// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using MonoTouch.Foundation;
using System.CodeDom.Compiler;

namespace InStoreCoupons
{
	[Register ("InStoreCouponsViewController")]
	partial class InStoreCouponsViewController
	{
		[Outlet]
		MonoTouch.UIKit.UILabel CouponLabel { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIImageView CouponView { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIButton DealofDay { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (CouponView != null) {
				CouponView.Dispose ();
				CouponView = null;
			}

			if (CouponLabel != null) {
				CouponLabel.Dispose ();
				CouponLabel = null;
			}

			if (DealofDay != null) {
				DealofDay.Dispose ();
				DealofDay = null;
			}
		}
	}
}
