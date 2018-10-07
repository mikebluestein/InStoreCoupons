using System;
namespace XamariniOSiBeacon
{
	public class Customer
	{
		public Customer()
		{
		}

		public int Id
		{
			get;
			set;
		}

		public string FirstName
		{
			get;
			set;
		}

		public string LastName
		{
			get;
			set;
		}

		public override string ToString()
		{
			return string.Format("[Customer: Id={0}, FirstName={1}, LastName={2}]", Id, FirstName, LastName);
		}
	}
}
