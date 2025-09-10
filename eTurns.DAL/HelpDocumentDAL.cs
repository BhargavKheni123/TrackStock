using eTurns.DTO;
using System.Collections.Generic;
using System.Linq;

namespace eTurns.DAL
{
    public class HelpDocumentDAL : eTurnsBaseDAL
    {

        #region [Class Constructor]

        //public HelpDocumentDAL(base.DataBaseName)
        //{

        //}

        public HelpDocumentDAL(string DbName)
        {
            base.DataBaseName = DbName;
        }

        //public HelpDocumentDAL(string DbName, string DBServerNm, string DBUserNm, string DBPswd)
        //{
        //    DataBaseName = DbName;
        //    DBServerName = DBServerNm;
        //    DBUserName = DBUserNm;
        //    DBPassword = DBPswd;
        //}

        #endregion


        public IEnumerable<HelpDocumentMasterDTO> GetAllRecord()
        {
            List<HelpDocumentMasterDTO> ObjCache = new List<HelpDocumentMasterDTO>();

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                ObjCache = (from im in context.HelpDocumentMasters
                            select new HelpDocumentMasterDTO
                            {
                                ID = im.ID,
                                ModuleName = im.ModuleName,
                                ModuleDocName = im.ModuleDocName,
                                ModuleDocPath = im.ModuleDocPath
                            }).ToList();
            }
            return ObjCache;
        }
    }
}
