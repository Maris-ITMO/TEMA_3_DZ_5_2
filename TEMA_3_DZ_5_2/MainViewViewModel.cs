using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Prism.Commands;

using TEMA_3_DZ_5_2_Library;

namespace TEMA_3_DZ_5_2
{
    public class MainViewViewModel
    {
        private ExternalCommandData _commandData;

        public DelegateCommand SaveWallTypeCommand { get; }
        public List<Element> PickedObjects { get; }
        public List<WallType> WallTypes { get; } = new List<WallType>();
        public WallType SelectedWallType { get; set; }

        public MainViewViewModel(ExternalCommandData commandData)
        {
            _commandData = commandData;
            SaveWallTypeCommand = new DelegateCommand(OnSaveWallTypeCommand);
            PickedObjects = SelectionUtils.PickObjects(commandData);
            WallTypes = WallsUtils.GetWallElementTypes(commandData);
        }

        private void OnSaveWallTypeCommand()
        {
            UIApplication uiapp = _commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Document doc = uidoc.Document;

            if (PickedObjects.Count == 0 || SelectedWallType == null)
            {
                return;
            }

            using (var ts = new Transaction(doc, "Set wall type"))
            {
                ts.Start();

                foreach (var pickedObject in PickedObjects)
                {
                    if (pickedObject is Wall)
                    {
                        var oWall = pickedObject as Wall;
                        oWall.ChangeTypeId(SelectedWallType.Id);
                    }

                }

                ts.Commit();
            }

            RaiseCloseRequest();
        }
        public event EventHandler CloseRequest;
        private void RaiseCloseRequest()
        {
            CloseRequest?.Invoke(this, EventArgs.Empty);
        }
    }
}

