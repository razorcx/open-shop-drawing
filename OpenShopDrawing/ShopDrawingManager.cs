using System.Collections.Generic;
using System.Linq;
using Tekla.Structures.Drawing;
using Tekla.Structures.Model;
using Tekla.Structures.Model.Operations;
using ModelObject = Tekla.Structures.Model.ModelObject;

namespace OpenShopDrawing
{
	public class ShopDrawingManager
	{
		private DrawingHandler Handler => new DrawingHandler();

		public void Run()
		{
			var selector = new Tekla.Structures.Model.UI.ModelObjectSelector();
			var modelObjects = selector.GetSelectedObjects();
			modelObjects.MoveNext();

			//Get picked part
			var picked = modelObjects.Current as Tekla.Structures.Model.Part;

			if (picked == null)
				return;

			//Check if numbering up to date
			if (!Operation.IsNumberingUpToDate(picked))
				return;

			//Get drawings
			var drawingCollection = Handler.GetDrawings();
			if (drawingCollection.GetSize() < 1)
				return;

			//Get drawings by type
			drawingCollection.SelectInstances = false;

			var drawings = new List<Drawing>();
			foreach (Drawing drawing in drawingCollection)
				drawings.Add(drawing);

			var assemblyDrawings = drawings.OfType<AssemblyDrawing>().ToList();
			var singlePartDrawings = drawings.OfType<SinglePartDrawing>().ToList();

			//Get assembly mark
			var assemblyMark = string.Empty;
			picked.GetReportProperty("ASSEMBLY_POS", ref assemblyMark);

			var partMark = string.Empty;
			picked.GetReportProperty("PART_POS", ref partMark);

			var singlePartDrawing = singlePartDrawings.AsParallel()
				.FirstOrDefault(s => GetDrawingUsableMark(s) == partMark);

			var assemblyDrawing = assemblyDrawings.AsParallel()
				.FirstOrDefault(s => GetDrawingUsableMark(s) == assemblyMark);

			if (singlePartDrawing != null)
				Handler.SetActiveDrawing(singlePartDrawing);
			else if (assemblyDrawing != null)
				Handler.SetActiveDrawing(assemblyDrawing);
		}

		private static string GetDrawingUsableMark(Drawing drawing)
		{
			var mark = string.Empty;
			if (drawing is AssemblyDrawing)
			{
				var part =
					new Model().SelectModelObject(((AssemblyDrawing)drawing).AssemblyIdentifier);
				part?.GetReportProperty("ASSEMBLY_POS", ref mark);
				return mark;
			}
			if (drawing is SinglePartDrawing)
			{
				var part =
					new Model().SelectModelObject(((SinglePartDrawing)drawing).PartIdentifier);
				part?.GetReportProperty("PART_POS", ref mark);
				return mark;
			}
			return string.Empty;
		}
	}

	public static class TeklaExtensionMethods
	{
		public static List<ModelObject> ToList(this ModelObjectEnumerator enumerator)
		{
			var modelObjects = new List<ModelObject>();
			while (enumerator.MoveNext())
			{
				var modelObject = enumerator.Current;
				if (modelObject == null) continue;
				modelObjects.Add(modelObject);
			}

			return modelObjects;
		}
	}
}