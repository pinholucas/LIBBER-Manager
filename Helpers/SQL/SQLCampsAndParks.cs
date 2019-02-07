using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Libber_Manager.Helpers
{
    class SQLCampsAndParks
    {
        public static List<SQLRelationships.Relations> campingsRelations;
        public static List<SQLRelationships.Relations> parksRelations;

        public static List<SQLRelationships.ListItem> CampingsCaP;
        public static List<SQLRelationships.ListItem> ParksCaP;        

        public async static void LoadCampingsAndParks(int intiID)
        {
            // Carrega a lista de Campings e suas relações com o intiID
            campingsRelations = new List<SQLRelationships.Relations>();
            CampingsCaP = new List<SQLRelationships.ListItem>();

            await SQLRelationships.LoadTableAndRelationships("Campings", "camping", intiID, CampingsCaP, campingsRelations);

            // Carrega a lista de Parques e suas relações com o intiID
            parksRelations = new List<SQLRelationships.Relations>();
            ParksCaP = new List<SQLRelationships.ListItem>();

            await SQLRelationships.LoadTableAndRelationships("Parks", "park", intiID, ParksCaP, parksRelations);
        }   
    }
}
