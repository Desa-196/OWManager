using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.DirectoryServices;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace Users
{
    public static class DomainSearch
    {
		public static DirectorySearcher Search;

		static DomainSearch()
		{
			DirectoryEntry deOU = new DirectoryEntry("LDAP://DC=LDF,DC=RU");

			Search = new DirectorySearcher(deOU);

			Search.PageSize = Properties.Settings.Default.MaxSearchUser;

			
		}
		private static string GetUserProperties(SearchResult SearchElement, string PropertyName)
		{
			try
			{
				return (SearchElement.Properties[PropertyName][0]).ToString();
			}
			catch {
				return "";
			}
		}
		public static async Task<List<User>> AsyncSearchUsers(string Name = "", string TelephoneNumber = "", string ComputerName = "")
		{
			var UserList = new List<User>();
			
			await Task.Run(()=> 
			{
				string FilterString = "";

				if (Name != "" && Name != null)
				{
					FilterString += "(displayname=*" + Name + "*)";
				}
				if (TelephoneNumber != "" && TelephoneNumber != null)
				{
					FilterString += "(telephonenumber=" + TelephoneNumber + "*)";
				}
				if (ComputerName != "" && ComputerName != null)
				{
					FilterString += "(wwwhomepage=" + ComputerName + "*)";
				}

				Search.Filter = "(&(ObjectClass=User)(givenname=*)" + FilterString + ")";

				SearchResultCollection result = Search.FindAll();
				if (result.Count > Properties.Settings.Default.MaxSearchUser)
				{
					throw new Exception("Найдено более " + Properties.Settings.Default.MaxSearchUser + " пользователей, уточните параметры поиска.");
				}
				
				for (int i = 0; i < result.Count; i++)
				{
					User User = new User(GetUserProperties(result[i], "displayname"));

					//Если userAccountControl == 514 то аккаунт не true, т.е. отключен
					User.EnableAccount = !(GetUserProperties(result[i], "userAccountControl") == "514");

					//Достаем номер телефона
					User.UserName = GetUserProperties(result[i], "samaccountname");
					//Достаем номер телефона
					User.TelephoneNumber = GetUserProperties(result[i], "telephonenumber");
					//Достаем имя компьютера
					User.ComputerName = GetUserProperties(result[i], "wwwhomepage");
					//Достаем описание пользователя
					User.Description = GetUserProperties(result[i], "description");

					//Достаем Axapta логин пользователя
					User.AxaptaId = SQLiteBase.GetUserAxaptaName(User.UserName);

					//Ищем есть ли такой компьютер на карте
					if (User.ComputerName != null & User.ComputerName.Length > 0)
					{
						if (SQLiteBase.GetLocationsByName(User.ComputerName) != null)
						{
							User.IsAddToMap = true;
						}
					}

					foreach (string a in (GetUserProperties(result[i], "distinguishedname")).Split(','))
					{
						if (a.Contains("OU"))
						{
							User.DomainOU = (a.Replace("OU=", "")) + "->" + User.DomainOU;
						}
					}
					User.DomainOU = User.DomainOU.Remove(User.DomainOU.Length - 2);
					User.DomainOU = "LDF->" + User.DomainOU;


					//Добавляем Пользователя в список найденных пользователей
					UserList.Add(User);

				}

			});
			return UserList;
		}
		public static async void searchUsers( ObservableCollection<User> UsersList, string Name="", string TelephoneNumber = "", string ComputerName = "")
		{
			string FilterString = "";

			if (Name != "" && Name != null)
			{
				FilterString += "(displayname=*" + Name + "*)";
			}
			if (TelephoneNumber != "" && TelephoneNumber != null)
			{
				FilterString += "(telephonenumber=" + TelephoneNumber + "*)";
			}
			if (ComputerName != "" && ComputerName != null)
			{
				FilterString += "(wwwhomepage=" + ComputerName + "*)";
			}

			Search.Filter = "(&(ObjectClass=User)(givenname=*)" + FilterString + ")";

			SearchResultCollection result = Search.FindAll();
			if (result.Count > Properties.Settings.Default.MaxSearchUser) 
			{
				throw new Exception("количество найденных пользователей больше 30");
			}
			await Task.Run(() =>
			{
				for (int i = 0; i < result.Count; i++)
				{
					User User = new User(GetUserProperties(result[i], "displayname"));

					//Если userAccountControl == 514 то аккаунт не true, т.е. отключен
					User.EnableAccount = !(GetUserProperties(result[i], "userAccountControl") == "514");

					//Достаем номер телефона
					User.UserName = GetUserProperties(result[i], "samaccountname");
					//Достаем номер телефона
					User.TelephoneNumber = GetUserProperties(result[i], "telephonenumber");
					//Достаем имя компьютера
					User.ComputerName = GetUserProperties(result[i], "wwwhomepage");
					//Достаем описание пользователя
					User.Description = GetUserProperties(result[i], "description");

					//Достаем Axapta логин пользователя
					User.AxaptaId = SQLiteBase.GetUserAxaptaName(User.UserName);

					foreach (string a in (GetUserProperties(result[i], "distinguishedname")).Split(','))
					{
						if (a.Contains("OU"))
						{
							User.DomainOU = (a.Replace("OU=", "")) + "->" + User.DomainOU;
						}
					}
					User.DomainOU = User.DomainOU.Remove(User.DomainOU.Length - 2);
					User.DomainOU = "LDF->" + User.DomainOU;

					Application.Current.Dispatcher.BeginInvoke((Action)delegate ()
					{
						
							//Добавляем Пользователя в список найденных пользователей
							UsersList.Add(User);
						

					});
				

				}
			});

		}
	}
}
