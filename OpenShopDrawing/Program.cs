using System;
using System.Diagnostics;

namespace OpenShopDrawing
{
	public class Program
	{
		public static void Main(string[] args)
		{
			try
			{
				new ShopDrawingManager().Run();
			}
			catch (Exception ex)
			{
				Trace.WriteLine(ex.InnerException + ex.Message + ex.StackTrace);
			}
		}
	}
}
