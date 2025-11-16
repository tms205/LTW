using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DA_LTW.Models;

namespace DA_LTW.Helpers
{
    public class LoaiMonHelper
    {
        public static List<LOAIMON> GetAll()
        {
            using (QL_NHAHANGEntities db = new QL_NHAHANGEntities())
            {
                return db.LOAIMONs.ToList();
            }
        }
    }
}