using System;
using System.Data;
using FA.DataLayer;

namespace FA.BusinessLayer
{
    class CityBL
    {

        internal static int CityId { get; set; }

        internal static int StateId { get; set; }


        internal static int CountryId { get; set; }

        internal static String CityName { get; set; }


        internal static DataTable GetCityList()
        {
            return CityDL.GetCityList();
        }

        internal static void UpdateCity()
        {
            CityDL.UpdateCity();
        }

        internal static void InsertCity()
        {
            CityDL.InsertCity();
        }

    }
}
