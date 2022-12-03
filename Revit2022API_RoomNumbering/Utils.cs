using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Revit2022API_RoomNumbering
{
    public static class Utils
    {
        public static List<Level> GetAllLevels(Document document)
        {
            var levels = new FilteredElementCollector(document)
                .OfClass(typeof(Level))
                .OfType<Level>()
                .ToList();

            return levels;
        }

        public static List<Room> FindAllRooms(Document document)
        {
            FilteredElementCollector collector = new FilteredElementCollector(document);
            collector.OfCategory(BuiltInCategory.OST_Rooms);
            List<Room> rooms = new List<Room>();
            foreach (Element element in collector)
            {
                Room room = element as Room;

                if (room == null)
                    continue;

                rooms.Add(room);
            }
            return rooms;
        }

        public static void SetNumber(Document doc, List<Room> rooms, string prefix, int beginValue)
        {
            using (Transaction transaction = new Transaction(doc, "Numbering rooms"))
            {
                transaction.Start();

                int numberValue = beginValue;
                string numberString;

                foreach (var room in rooms)
                {
                    numberString = prefix + numberValue;
                    Parameter numberParameter = room.get_Parameter(BuiltInParameter.ROOM_NUMBER);
                    numberParameter.Set(numberString);
                    numberValue++;
                }
                transaction.Commit();
            }
        }

        public static void CreateAllRoomOnLevel(Document doc, Level level)
        {

            using (Transaction transaction = new Transaction(doc, "Create rooms"))
            {
                transaction.Start();

                PlanTopology pt = doc.get_PlanTopology(level);

                foreach (PlanCircuit pc in pt.Circuits)
                {
                    if (!pc.IsRoomLocated)
                    {
                        Room room = doc.Create.NewRoom(null, pc);
                    }
                }

                transaction.Commit();
            }
        }

        public static List<Room> FindRoomsOnlevel(Document document, Level level)
        {
            FilteredElementCollector collector = new FilteredElementCollector(document);
            collector.OfCategory(BuiltInCategory.OST_Rooms);

            List<Room> allRooms = new List<Room>();

            foreach (Element element in collector)
            {
                Room room = element as Room;

                if (room == null)
                    continue;

                allRooms.Add(room);
            }

            List<Room> roomsOnLevel = allRooms
                .Where(x => (x.LevelId == level.Id))
                .ToList(); 

            return roomsOnLevel;

            //List<Room> rooms = new FilteredElementCollector(document)
            //    .OfClass(typeof(Room))
            //    .OfType<Room>()
            //    .ToList();

        }

        public static bool ChekOnNull<t>(t value, string textError)
        {
            if (value == null)
            {
                TaskDialog.Show("Ошибка", textError);
                return true;
            }
            else
            {
                return false;
            }
        }

    }
}
