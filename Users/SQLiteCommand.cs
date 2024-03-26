using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Common;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Users
{
    public static class SQLiteBase
    {
        //Путь к исполняемому файлу
        public static string path = Environment.CurrentDirectory;

        public static SQLiteConnection Connect;
        static SQLiteBase()
        {
            Connect = new SQLiteConnection("Data Source=" + path + "\\user_id.db", true);
            Connect.Open();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="UserLoginDomain"></param>
        /// <returns></returns>
        public static string GetUserAxaptaName(string UserLoginDomain) 
        {
            var Command_SQL = new SQLiteCommand
            (
                "SELECT axapta_name FROM USERS where domain_name like '" + UserLoginDomain + "' ", Connect
            );
            
            SQLiteDataReader read = Command_SQL.ExecuteReader();
            
            
            if (read.Read())
            {
                return read["axapta_name"].ToString();
            }
            else
            {
                return "";
            }
        }
        public static int? GetLocationsByName(string Name) 
        {
            var Command_SQL = new SQLiteCommand
            (
                "SELECT * FROM ObjectsMap where type_object = 1 and name like '" + Name + "' ", Connect
            );
            
            SQLiteDataReader read = Command_SQL.ExecuteReader();
            
            
            if (read.Read())
            {
                return Convert.ToInt32(read["locations_id"]);
            }
            else
            {
                return null;
            }
        }

        public static string GetNameLocationsFromId(int Id) 
        {
            var Command_SQL = new SQLiteCommand
            (
                "select name from locations where id = " + Id, Connect
            );
            
            SQLiteDataReader read = Command_SQL.ExecuteReader();


            if (read.Read())
            {
                return read["name"].ToString();
            }
            else
            {
                return "";
            }
        }
        public static string GetMapImageSource(int Id) 
        {
            var Command_SQL = new SQLiteCommand
            (
                "select * from locations_image_source where fk_locations = " + Id, Connect
            );
            
            SQLiteDataReader read = Command_SQL.ExecuteReader();
            
            
            if (read.Read())
            {
                return read["image_source"].ToString();
            }
            else
            {
                return "";
            }
        }
        public static List<TypeObject> GetTypeObjectMap() 
        {
           
            var Command_SQL = new SQLiteCommand
            (
                "select * from TypeObjectsMap ", Connect
            );
            
            SQLiteDataReader read = Command_SQL.ExecuteReader();

            List<TypeObject> ListTypesObjects = new List<TypeObject>();
            foreach (DbDataRecord Result in read)
            {
                ListTypesObjects.Add(new TypeObject
                    (
                    Convert.ToInt32(Result["id"]), 
                    Result["name"].ToString(), 
                    Result["color"].ToString(), 
                    Result["image"].ToString())
                    ) ;
            }
            return ListTypesObjects;
        }
        public static ObservableCollection<MapObject> GetObjectMap(int IdLocation, List<TypeObject> FilterType = null, string ObjectName = null) 
        {
            string ExecuteFilter = "";

            if (ObjectName != null && ObjectName.Length > 0)
            {
                ExecuteFilter = ExecuteFilter.Replace("_", @"\_");
                ExecuteFilter += " AND ObjectsMap.name LIKE '%"+ ObjectName + @"%'  ESCAPE '\'";
            }

            if (FilterType != null && FilterType.Count > 0)
            {
                ExecuteFilter += " AND type_object in (";
                foreach (var Type in FilterType)
                {
                    ExecuteFilter += Type.Id + ",";
                }
                //Удаляем последнюю запятую
                ExecuteFilter = ExecuteFilter.Remove(ExecuteFilter.Length - 1);
                ExecuteFilter += ")";
            }
            var Command_SQL = new SQLiteCommand
            (
                "SELECT " +
                    "*, " +
                    "ObjectsMap.id as 'id_object'," +
                    "TypeObjectsMap.id as 'id_type'  " +
                "FROM ObjectsMap LEFT JOIN TypeObjectsMap ON(ObjectsMap.type_object = TypeObjectsMap.id) WHERE ObjectsMap.locations_id = "+ IdLocation + ExecuteFilter, Connect
            );
            
            SQLiteDataReader read = Command_SQL.ExecuteReader();

            if (read.FieldCount > 0)
            {
                ObservableCollection<MapObject> ListTypesObjects = new ObservableCollection<MapObject>();
                foreach (DbDataRecord Result in read)
                {
                    ListTypesObjects.Add(new MapObject
                        (
                        Convert.ToDouble(Result["x_coordinate"].ToString().Replace(".", ",")),
                        Convert.ToDouble(Result["y_coordinate"].ToString().Replace(".", ",")),
                        new TypeObject(Convert.ToInt32(Result["id_type"]),
                        Result["name"].ToString(),
                        Result["color"].ToString(),
                        Result["image"].ToString())
                        )
                        );
                    ListTypesObjects[ListTypesObjects.Count - 1].Id = Convert.ToInt32(Result["id_object"]);
                    ListTypesObjects[ListTypesObjects.Count - 1].Name = Result["name"].ToString();
                    ListTypesObjects[ListTypesObjects.Count - 1].Description = Result["descriptions"].ToString();
                    ListTypesObjects[ListTypesObjects.Count - 1].Location = Convert.ToInt32(Result["locations_id"]);
                    
                    if ((Application.Current.MainWindow as MainWindow).CutBufferObject != null && (Application.Current.MainWindow as MainWindow).CutBufferObject.Id == Convert.ToInt32(Result["id_object"]))
                    {
                        ListTypesObjects[ListTypesObjects.Count - 1].IsCut = true;
                        (Application.Current.MainWindow as MainWindow).CutBufferObject = ListTypesObjects[ListTypesObjects.Count - 1];
                    }
                }
                return ListTypesObjects;
            }
            else return null;
        }
        public static List<MenuItemLocations> GetMenuItem(int Id) 
        {
            //System.Windows.MessageBox.Show("select * from Locations WHERE parent_id is " + (Id < 0 ? "null" : Id.ToString()));
            var Command_SQL = new SQLiteCommand
            (
                "select * from Locations WHERE parent_id is " + (Id < 0 ? "null" : Id.ToString()), Connect
            );
            
            SQLiteDataReader read = Command_SQL.ExecuteReader();

            List<MenuItemLocations> ListLocations = new List<MenuItemLocations>();

            if (read.FieldCount > 0)
            {
                foreach (DbDataRecord Result in read)
                {
                    ListLocations.Add(new MenuItemLocations());
                    ListLocations[ListLocations.Count - 1].Id = Convert.ToInt32(Result["id"]);
                    if (Result["parent_id"] is DBNull)
                    {
                        ListLocations[ListLocations.Count - 1].IdParent = -1;
                    }
                    else
                    {
                        ListLocations[ListLocations.Count - 1].IdParent = Convert.ToInt32(Result["parent_id"]);
                    }

                    ListLocations[ListLocations.Count - 1].Header = Result["Name"];

                }
                return ListLocations;
            }
            else return null;
        }

        public static bool DeleteElement(int? elementId)
        {
            if (elementId != null)
            {
                var Command_SQL = new SQLiteCommand
                    ($@"
                    DELETE FROM ObjectsMap
                    WHERE id={elementId}", Connect
                    );
                Command_SQL.ExecuteNonQuery();
            }
            return true;
        }


        public static bool EditObject(MapObject editObject)
        {
            if (editObject != null)
            {
                var Command_SQL = new SQLiteCommand
                    ($@"
                    UPDATE ObjectsMap
                    SET name = '{editObject.Name}',
	                    type_object = {editObject.TypeObject.Id},
	                    descriptions = '{editObject.Description}'
                    WHERE id = {editObject.Id}
                    ", Connect);
                Command_SQL.ExecuteNonQuery();
            }
            return true;
        }        
        
        public static bool ChangeLocationObject(MapObject editObject, int location, double coordinateX, double coordinateY)
        {
            if (editObject != null)
            {
                var Command_SQL = new SQLiteCommand
                    ($@"
                    UPDATE ObjectsMap
                    SET locations_id = '{location}',
                    x_coordinate = {coordinateX.ToString(System.Globalization.CultureInfo.GetCultureInfo("en-US"))},
                    y_coordinate = {coordinateY.ToString(System.Globalization.CultureInfo.GetCultureInfo("en-US"))}
                    WHERE id = {editObject.Id}
                    ", Connect);
                Command_SQL.ExecuteNonQuery();
            }
            return true;
        }

        public static bool SaveElements(ObservableCollection<MapObject> ArraySaveObjects, int Id) 
        {
            foreach (MapObject SaveObject in ArraySaveObjects)
            {
                //Если Id не null значит этот объект из базы, просто обновляем
                if (SaveObject.Id != null)
                {
                    //Проверяем, вдруг его удалили из базы, гна всякий случай
                    var Command_SQL = new SQLiteCommand("select * from ObjectsMap where id=" + SaveObject.Id+ " and locations_id="+Id.ToString(), Connect);
                    SQLiteDataReader read = Command_SQL.ExecuteReader();
                    //Если есть, то обновляем
                    if (read.Read())
                    {
                        Command_SQL = new SQLiteCommand(
                            $"UPDATE ObjectsMap SET " +
                            "name = '" + SaveObject.Name + "', " +
                            "descriptions = '" + SaveObject.Description + "', " +
                            "x_coordinate = " + SaveObject.XCoordinate.ToString(System.Globalization.CultureInfo.GetCultureInfo("en-US")) + ", "+
                            "y_coordinate = " + SaveObject.YCoordinate.ToString(System.Globalization.CultureInfo.GetCultureInfo("en-US")) + ", "+
                            "type_object = " + SaveObject.TypeObject.Id+" where id=" + SaveObject.Id, Connect);
                            Command_SQL.ExecuteNonQuery();
                    }
                    //Если нет, то создаем
                    else 
                    {
                        
                        Command_SQL = new SQLiteCommand(
                            "INSERT INTO ObjectsMap (name, descriptions, x_coordinate, y_coordinate, type_object, locations_id) VALUES (" +
                            "'" + SaveObject.Name + "', " +
                            "'" + SaveObject.Description + "', " +
                            SaveObject.XCoordinate.ToString(System.Globalization.CultureInfo.GetCultureInfo("en-US")) + ", " +
                            SaveObject.YCoordinate.ToString(System.Globalization.CultureInfo.GetCultureInfo("en-US")) + ", " +
                            SaveObject.TypeObject.Id+", " +
                            Id.ToString()+")"
                            , Connect);
                        Command_SQL.ExecuteNonQuery();
                        //Делаем запрос для определения id новой записи
                        Command_SQL = new SQLiteCommand(
                            "SELECT MAX(id) as 'id' FROM ObjectsMap", Connect);
                        read = Command_SQL.ExecuteReader();

                        if (read.Read()) SaveObject.Id = Convert.ToInt32( read["id"] );
                    }
                }
                else
                {
                    var  Command_SQL = new SQLiteCommand(
                        "INSERT INTO ObjectsMap (name, descriptions, x_coordinate, y_coordinate, type_object, locations_id) VALUES (" +
                        "'" + SaveObject.Name + "', " +
                        "'" + SaveObject.Description + "', " +
                        SaveObject.XCoordinate.ToString(System.Globalization.CultureInfo.GetCultureInfo("en-US")) + ", " +
                        SaveObject.YCoordinate.ToString(System.Globalization.CultureInfo.GetCultureInfo("en-US")) + ", " +
                        SaveObject.TypeObject.Id + ", " +
                        Id.ToString()+")"
                        , Connect);
                    Command_SQL.ExecuteNonQuery();
                    //Делаем запрос для определения id новой записи
                    Command_SQL = new SQLiteCommand(
                        "SELECT MAX(id) as 'id' FROM ObjectsMap", Connect);
                    SQLiteDataReader read = Command_SQL.ExecuteReader();

                    if (read.Read()) SaveObject.Id = Convert.ToInt32(read["id"]);
                }

            }
            return true;
        }
        

    }
}
