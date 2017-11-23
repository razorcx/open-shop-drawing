using System.Collections.Generic;
using System.Linq;
using Tekla.Structures.Drawing;
using Tekla.Structures.Model;
using Tekla.Structures.Model.Operations;

namespace OpenShopDrawing
{
	public partial class Program
	{
		public static class OpenShopDrawing
		{
			private static DrawingHandler Handler => new DrawingHandler();

			public static void Run()
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
				var drawings = new List<Drawing>();
				foreach (Drawing drawing in drawingCollection)
					drawings.Add(drawing);

				var assemblyDrawings = new List<AssemblyDrawing>(drawings.OfType<AssemblyDrawing>());
				var singlePartDrawings = new List<SinglePartDrawing>(drawings.OfType<SinglePartDrawing>());

				//Get assembly mark
				var assemblyMark = string.Empty;
				picked.GetReportProperty("ASSEMBLY_POS", ref assemblyMark);
				if (string.IsNullOrEmpty(assemblyMark))
					return;

				var singlePartDrawing = singlePartDrawings
					.FirstOrDefault(s => GetDrawingUsableMark(s) == assemblyMark);

				var assemblyDrawing = assemblyDrawings
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
					part?.GetReportProperty("ASSEMBLY_POS", ref mark);
					return mark;
				}
				return string.Empty;
			}
		}
	}
}
