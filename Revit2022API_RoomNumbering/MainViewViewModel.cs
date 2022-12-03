using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.UI;
using Prism.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Revit2022API_RoomNumbering
{
    public class MainViewViewModel
    {
        private ExternalCommandData _commandData;

        public List<Level> Levels { get; }
        public Level SelectedLevel { get; set; }

        public DelegateCommand NumberingOnSelectedLevel { get; }
        public DelegateCommand NumberingAll { get; }
        public DelegateCommand CreateRoom { get; }

        string numberString = string.Empty;
        public string Prefix { get; set; }

        private int numberBeginValue;
        public int NumberBeginValue
        {
            get
            {
                return numberBeginValue;
            }
            set
            {
                try
                {
                    numberBeginValue = Convert.ToInt32(value);
                }
                catch (FormatException e)
                {
                    numberBeginValue = 1;
                }
            }        
        }

        public MainViewViewModel(ExternalCommandData commandData)
        {
            _commandData = commandData;
            Document doc = _commandData.Application.ActiveUIDocument.Document;

            Levels = Utils.GetAllLevels(doc);

            NumberingOnSelectedLevel = new DelegateCommand(OnNumberingOnSelectedLevel);
            NumberingAll = new DelegateCommand(OnNumberingAll);
            CreateRoom = new DelegateCommand(OnCreateRoom);
        }

        private void OnNumberingOnSelectedLevel()
        {
            Document doc = _commandData.Application.ActiveUIDocument.Document;

            bool errorFlag = Utils.ChekOnNull(SelectedLevel, "Выберите уровень для расстановки нумерации");

            List<Room> rooms = new List<Room>();
            if (errorFlag == false)
            {
                rooms = Utils.FindRoomsOnlevel(doc, SelectedLevel);
            }

            errorFlag = Utils.ChekOnNull(rooms, "Не найдено созданных помещений на выбранном уровне.Разместите помещения на плане");

            if (errorFlag == false)
            {
                Utils.SetNumber(doc, rooms, Prefix, numberBeginValue);
            }
        }

        private void OnNumberingAll()
        {
            Document doc = _commandData.Application.ActiveUIDocument.Document;

            List<Room> rooms = Utils.FindAllRooms(doc);

            bool errorFlag = false;
            errorFlag = Utils.ChekOnNull(rooms, "Не найдено созданных помещений. Разместите помещения на плане");

            if (errorFlag == false)
            {
                Utils.SetNumber(doc, rooms, Prefix, numberBeginValue);
            }
        }

        private void OnCreateRoom()
        {
            Document doc = _commandData.Application.ActiveUIDocument.Document;

            bool errorFlag = Utils.ChekOnNull(SelectedLevel, "Выберите уровень для расстановки нумерации");

            if (errorFlag == false)
            {
                Utils.CreateAllRoomOnLevel(doc, SelectedLevel);
            }
        }

        public event EventHandler CloseReqest;

        private void RaiseCloseReqest()
        {
            CloseReqest?.Invoke(this, EventArgs.Empty);
        }

   }
}
