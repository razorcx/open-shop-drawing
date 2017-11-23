using System;
using System.Diagnostics;

namespace OpenShopDrawing
{
	public partial class Program
	{
		public static void Main(string[] args)
		{
			try
			{
				OpenShopDrawing.Run();
			}
			catch (Exception ex)
			{
				Trace.WriteLine(ex.InnerException + ex.Message + ex.StackTrace);
			}
		}
	}
}
