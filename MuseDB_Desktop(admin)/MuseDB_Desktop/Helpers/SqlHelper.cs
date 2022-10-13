using System.Configuration;
namespace MuseDB_Desktop.Helpers
{
    public static class SqlHelper {
        public static string CnnVal(string name) {
            return ConfigurationManager.ConnectionStrings[name].ConnectionString;
        }
    }
}