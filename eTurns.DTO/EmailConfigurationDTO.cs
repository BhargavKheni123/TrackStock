using eTurns.DTO.Resources;

namespace eTurns.DTO
{
    public class EmailConfigurationDTO
    {
    }

    public class ResEmailConfiguration
    {
        private static string resourceFile = typeof(ResEmailConfiguration).Name;

        /// <summary>
        ///   Looks up a localized string similar to Rooms.
        /// </summary>
        public static string PageHeader
        {
            get
            {
                return ResourceRead.GetResourceValue("PageHeader", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to eTurns: Rooms.
        /// </summary>
        public static string PageTitle
        {
            get
            {
                return ResourceRead.GetResourceValue("PageTitle", resourceFile);
            }
        }
        /// <summary>
        ///   Looks up a localized string similar to eTurns: Rooms.
        /// </summary>
        public static string lbltemplate
        {
            get
            {
                return ResourceRead.GetResourceValue("lbltemplate", resourceFile);
            }
        }
        /// <summary>
        ///   Looks up a localized string similar to eTurns: Rooms.
        /// </summary>
        public static string btnSave
        {
            get
            {
                return ResourceRead.GetResourceValue("btnSave", resourceFile);
            }
        }
        public static string Language
        {
            get
            {
                return ResourceRead.GetResourceValue("Language", resourceFile);
            }
        }

    }
}
