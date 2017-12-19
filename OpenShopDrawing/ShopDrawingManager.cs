using Tekla.Structures.Drawing;
using Tekla.Structures.Model.Operations;
using Tekla.Structures.Model.UI;

namespace OpenShopDrawing
{
	public class ShopDrawingManager
	{
		public void OpenDrawing()
		{
			//Pick a part
			var picker = new Picker();
			var picked = picker.PickObject(Picker.PickObjectEnum.PICK_ONE_PART);

			if (picked == null)
				return;

			//Check if numbering up to date
			if (!Operation.IsNumberingUpToDate(picked))
				return;

			//Get assembly mark
			var assemblyMark = string.Empty;
			picked.GetReportProperty("ASSEMBLY_POS", ref assemblyMark);

			//Get part mark
			var partMark = string.Empty;
			picked.GetReportProperty("PART_POS", ref partMark);

			//Check if assembly or single part
			if (assemblyMark == partMark)
			{
				//Get assembly drawing
				var assemblyManager = new AssemblyDrawingManager();
				var assemblyDrawing = assemblyManager.GetAssemblyDrawing(partMark);

				//Set active drawing
				if (assemblyDrawing != null)
					assemblyManager.SetActiveDrawing(assemblyDrawing);
			}
			else
			{
				//Get single part drawing
				var singleDrawingManager = new SingleDrawingManager();
				var singlePartDrawing = singleDrawingManager.GetSinglePartDrawing(partMark);

				//Set active drawing
				if (singlePartDrawing != null)
					singleDrawingManager.SetActiveDrawing(singlePartDrawing);
			}
		}
	}
}

